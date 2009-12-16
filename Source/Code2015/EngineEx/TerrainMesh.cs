using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Effects;

namespace Code2015.EngineEx
{
    unsafe class TerrainMesh : Resource, IRenderable
    {
        public const int TerrainBlockSize = 33;
        public const int LocalLodCount = 4;

        struct TerrainVertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;

            static VertexElement[] elements;

            static TerrainVertex()
            {
                elements = new VertexElement[2];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
                elements[1] = new VertexElement(Vector3.SizeInBytes, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }

            public static int Size
            {
                get { return Vector3.SizeInBytes + Vector2.SizeInBytes; }
            }
        }

        //static readonly int[] AuxLodSize = new int[] { 513, 129, 33, 9 };
        //static readonly int[] DataLodSize = new int[] { 1025, 257, 65, 17 };

        FileLocation resLoc;

        int dataEdgeSize;
        int dataEdgeSizeLow;

        VertexDeclaration vtxDecl;
        VertexBuffer vtxBuffer;

        IndexBuffer[] indexBuffer = new IndexBuffer[LocalLodCount];


        int dataLevel;

        RenderSystem renderSystem;
        ObjectFactory factory;

        Queue<TerrainTreeNode> bfsQueue;
        TerrainTreeNode rootNode;

        FastList<RenderOperation> opBuffer;

        Material material;

        BoundingSphere BoundingSphere;

        int blockCount;
        int blockEdgeCount;

        int[] levelLengths;

        /// <summary>
        ///  在不同lod级别下一个单元的跨度
        /// </summary>
        int[] cellSpan;

        /// <summary>
        ///  lod 权值
        /// </summary>
        float[] lodLevelThreshold;

        int[] levelPrimConut;

        int[] levelVertexCount;


        public static string GetHashString(int x, int y, int lod)
        {
            return "TM" + x.ToString() + y.ToString() + lod.ToString();
        }

        public TerrainMesh(RenderSystem rs, int x, int y, int lod)
            : base(TerrainMeshManager.Instance, GetHashString(x, y, lod))
        {
            this.bfsQueue = new Queue<TerrainTreeNode>();
            this.opBuffer = new FastList<RenderOperation>();

            resLoc = FileSystem.Instance.TryLocate("tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_" + lod.ToString() + TDMPIO.Extension, FileLocateRule.Default);// GameFileLocs.Terrain);
            dataLevel = lod;
            renderSystem = rs;
            factory = rs.ObjectFactory;

            material = new Material(rs);
            material.CullMode = CullMode.CounterClockwise;

            //material.Ambient = terrData.MaterialAmbient;
            //material.Diffuse = terrData.MaterialDiffuse;
            //material.Emissive = terrData.MaterialEmissive;
            //material.Specular = terrData.MaterialSpecular;
            //material.Power = terrData.MaterialPower;
            material.SetEffect(EffectManager.Instance.GetModelEffect(TerrainEffectFactory.Name));
        }

        public override int GetSize()
        {
            return dataEdgeSize * dataEdgeSize * TerrainVertex.Size;
        }

        protected override void load()
        {
            TDMPIO data = new TDMPIO();
            data.Load(resLoc);

            MeshData meshData = new MeshData(renderSystem);

            #region 顶点数据

            dataEdgeSize = data.Width;
            dataEdgeSizeLow = data.Width / 2;

            int edgeVtxCount = dataEdgeSize;
            int vertexCount = edgeVtxCount * edgeVtxCount;


            ResourceInterlock.EnterAtomicOp();
            try
            {
                vtxDecl = factory.CreateVertexDeclaration(TerrainVertex.Elements);

                vtxBuffer = factory.CreateVertexBuffer(vertexCount, vtxDecl, BufferUsage.WriteOnly);
                TerrainVertex* vertices = (TerrainVertex*)vtxBuffer.Lock(LockMode.None);

                for (int i = 0; i < edgeVtxCount; i++)
                {
                    for (int j = 0; j < edgeVtxCount; j++)
                    {
                        vertices[i * edgeVtxCount + j].Position =
                            new Vector3(j * TerrainMeshManager.TerrainScale,
                                ComputeTerrainHeight(data.Data[i * edgeVtxCount + j]),
                                i * TerrainMeshManager.TerrainScale);
                    }
                }

                vtxBuffer.Unlock();
            }
            finally
            {
                ResourceInterlock.ExitAtomicOp();
            }

            #endregion

            #region 索引数据
            int edgeLen = edgeVtxCount - 1;
            this.blockEdgeCount = edgeLen / (TerrainBlockSize - 1);
            this.blockCount = MathEx.Sqr(blockEdgeCount);

            levelLengths = new int[LocalLodCount];
            cellSpan = new int[LocalLodCount];
            lodLevelThreshold = new float[LocalLodCount];

            levelPrimConut = new int[LocalLodCount];
            levelVertexCount = new int[LocalLodCount];

            for (int k = 0, levelLength = edgeLen; k < LocalLodCount; k++, levelLength /= 2)
            {
                int cellLength = edgeLen / levelLength;


                lodLevelThreshold[k] = (edgeVtxCount * MathEx.Root2 * 0.25f) / (float)(k + 1);
                lodLevelThreshold[k] = MathEx.Sqr(lodLevelThreshold[k]);

                cellSpan[k] = cellLength;
                levelLengths[k] = levelLength;

                int indexCount = MathEx.Sqr(levelLength) * 2 * 3;

                levelPrimConut[k] = MathEx.Sqr(levelLength) * 2;
                levelVertexCount[k] = MathEx.Sqr(levelLength + 1);

                ResourceInterlock.EnterAtomicOp();
                try
                {
                    indexBuffer[k] = factory.CreateIndexBuffer(IndexBufferType.Bit32, indexCount, BufferUsage.WriteOnly);

                    int* iptr = (int*)indexBuffer[k].Lock(0, 0, LockMode.None);

                    for (int i = 0; i < levelLength; i++)
                    {
                        for (int j = 0; j < levelLength; j++)
                        {
                            int x = i * cellLength;
                            int y = j * cellLength;

                            (*iptr) = y * edgeVtxCount + x;
                            iptr++;
                            (*iptr) = y * edgeVtxCount + (x + cellLength);
                            iptr++;
                            (*iptr) = (y + cellLength) * edgeVtxCount + (x + cellLength);
                            iptr++;

                            (*iptr) = y * edgeVtxCount + x;
                            iptr++;
                            (*iptr) = (y + cellLength) * edgeVtxCount + (x + cellLength);
                            iptr++;
                            (*iptr) = (y + cellLength) * edgeVtxCount + x;
                            iptr++;
                        }
                    }
                    indexBuffer[k].Unlock();
                }
                finally
                {
                    ResourceInterlock.ExitAtomicOp();
                }
            }
            #endregion

            BuildTerrainTree(data.Data, blockEdgeCount);

        }

        float ComputeTerrainHeight(float inp)
        {
            return inp * TerrainMeshManager.HeightScale - TerrainMeshManager.ZeroLevel;
        }

        void BuildTerrainTree(float[] dmData, int blockEdgeLen)
        {
            TerrainBlock[] blocks = new TerrainBlock[blockCount];


            int index = 0;

            for (int i = 0; i < blockEdgeCount; i++)
            {
                for (int j = 0; j < blockEdgeCount; j++)
                {
                    Vector3 center = new Vector3();

                    blocks[index] = new TerrainBlock(j * blockEdgeLen, i * blockEdgeLen);

                    // 检查该块中是否有特殊单元
                    if (!false)
                    {
                        blocks[index].IndexBuffers = indexBuffer;
                    }
                    else
                    {
                        // 为这个block创建特殊的IB
                    }

                    GeomentryData gd = new GeomentryData(this);
                    gd.VertexDeclaration = vtxDecl;

                    gd.VertexSize = TerrainVertex.Size;
                    gd.VertexBuffer = vtxBuffer;
                    gd.IndexBuffer = indexBuffer[0];
                    gd.PrimCount = levelPrimConut[0];// levelLengths[0] * levelLengths[0] * 2;
                    gd.VertexCount = levelVertexCount[0];// MathEx.Sqr(levelLengths[0] + 1);

                    gd.PrimitiveType = RenderPrimitiveType.TriangleList;

                    int x = (j == 0) ? 0 : j * blockEdgeLen;
                    int y = (i == 0) ? 0 : i * blockEdgeLen;

                    gd.BaseVertex = y * dataEdgeSize + x;

                    blocks[index].GeoData = gd;

                    for (int ii = 0; ii < TerrainBlockSize; ii++)
                    {
                        for (int jj = 0; jj < TerrainBlockSize; jj++)
                        {
                            int dmY = i * blockEdgeLen + ii;
                            int dmX = j * blockEdgeLen + jj;

                            center.X += TerrainMeshManager.TerrainScale * dmX;
                            center.Y += ComputeTerrainHeight(dmData[dmY * dataEdgeSize + dmX]);
                            center.Z += TerrainMeshManager.TerrainScale * dmY;
                        }
                    }

                    float invVtxCount = 1f / (float)(TerrainBlockSize * TerrainBlockSize);
                    center.X *= invVtxCount;
                    center.Y *= invVtxCount;
                    center.Z *= invVtxCount;


                    float radius = 0;
                    for (int ii = 0; ii < TerrainBlockSize; ii++)
                    {
                        for (int jj = 0; jj < TerrainBlockSize; jj++)
                        {
                            int dmY = i * blockEdgeLen + ii;
                            int dmX = j * blockEdgeLen + jj;

                            Vector3 vtxPos = new Vector3(
                                TerrainMeshManager.TerrainScale * dmX,
                                ComputeTerrainHeight(dmData[dmY * dataEdgeSize + dmX]),
                                TerrainMeshManager.TerrainScale * dmY);
                            float dist = Vector3.Distance(vtxPos, center);
                            if (dist > radius)
                            {
                                radius = dist;
                            }
                        }
                    }
                    blocks[index].Radius = radius;
                    blocks[index].Center = center;

                    index++;
                }
            }

            rootNode = new TerrainTreeNode(new FastList<TerrainBlock>(blocks), (dataEdgeSize - 1) / 2, (dataEdgeSize - 1) / 2, 1, dataEdgeSize);

            this.BoundingSphere = rootNode.BoundingVolume;
        }

        protected override void unload()
        {
            vtxBuffer.Dispose();
            vtxDecl.Dispose();
            vtxBuffer = null;
            vtxDecl = null;

            for (int i = 0; i < LocalLodCount; i++) 
            {
                indexBuffer[i].Dispose();
                indexBuffer[i] = null;
            }
        }

        public void PrepareVisibleObjects(ICamera cam)
        {
            if (State == ResourceState.Loaded)
            {
                opBuffer.Clear();

                Frustum frus = cam.Frustum;
                Vector3 camPos = cam.Position;

                Vector3 c = rootNode.BoundingVolume.Center;

                if (frus.IntersectsSphere(ref c, rootNode.BoundingVolume.Radius))
                {
                    bfsQueue.Enqueue(rootNode);

                    while (bfsQueue.Count > 0)
                    {
                        TerrainTreeNode node = bfsQueue.Dequeue();
                        TerrainTreeNode[] nodes = node.Children;

                        if (nodes != null)
                        {
                            // 遍历子节点
                            for (int i = 0; i < node.Children.Length; i++)
                            {
                                c = node.Children[i].BoundingVolume.Center;

                                if (frus.IntersectsSphere(ref c, node.Children[i].BoundingVolume.Radius))
                                {
                                    bfsQueue.Enqueue(node.Children[i]);
                                }
                            }
                        }
                        else
                        {
                            if (node.Block != null)
                            {
                                c = node.BoundingVolume.Center;

                                if (frus.IntersectsSphere(ref c, node.BoundingVolume.Radius))
                                {
                                    float dist = MathEx.DistanceSquared(ref c, ref camPos);

                                    RenderOperation op;

                                    op.Material = material;
                                    op.Geomentry = node.Block.GeoData;

                                    int lodLevel = 3;

                                    for (int lod = 0; lod < 4; lod++)
                                    {
                                        if (dist <= lodLevelThreshold[3 - lod])
                                        {
                                            lodLevel = lod;
                                            break;
                                        }
                                    }

                                    op.Geomentry.IndexBuffer = node.Block.IndexBuffers[lodLevel];
                                    op.Geomentry.PrimCount = levelPrimConut[lodLevel];// levelLengths[lodLevel] * levelLengths[lodLevel] * 2;
                                    op.Geomentry.VertexCount = levelVertexCount[lodLevel];// MathEx.Sqr(levelLengths[lodLevel] + 1);

                                    op.Transformation = Matrix.Identity;

                                    opBuffer.Add(op);
                                }
                            }
                        }

                    }
                }
            }
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (State == ResourceState.Loaded)
            {
                return opBuffer.Elements;
            }
            return null;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion
    }
}
