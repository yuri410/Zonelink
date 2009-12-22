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
using Code2015.World;

namespace Code2015.EngineEx
{
    unsafe class TerrainMesh : Resource, IRenderable
    {
        public const int TerrainBlockSize = 33;
        public const int LocalLodCount = 4;

        const float PlanetRadius = PlanetEarth.PlanetRadius;

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


        FileLocation resLoc;

        int terrEdgeSize;


        VertexDeclaration vtxDecl;
        VertexBuffer vtxBuffer;

        IndexBuffer[] indexBuffer = new IndexBuffer[LocalLodCount];


        int dataLevel;

        RenderSystem renderSystem;
        ObjectFactory factory;

        GeomentryData noDataGeo;

        Queue<TerrainTreeNode> bfsQueue;
        TerrainTreeNode rootNode;

        FastList<RenderOperation> opBuffer;

        Material material;

        public BoundingSphere BoundingSphere;
        public Matrix Transformation = Matrix.Identity;


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


        float topLen;
        float bottomLen;
        float tileCol;
        float tileLat;


        public float TileCol
        {
            get { return tileCol; }
        }
        public float TileLat
        {
            get { return tileLat; }
        }


        public static string GetHashString(int x, int y, int lod)
        {
            return "TM" + x.ToString() + y.ToString() + lod.ToString();
        }

        public TerrainMesh(RenderSystem rs, int x, int y, int lod)
            : base(TerrainMeshManager.Instance, GetHashString(x, y, lod))
        {
            this.bfsQueue = new Queue<TerrainTreeNode>();
            this.opBuffer = new FastList<RenderOperation>();

            resLoc = FileSystem.Instance.TryLocate(
                "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_" + lod.ToString() + TDMPIO.Extension, GameFileLocs.Terrain);
            dataLevel = lod;
            renderSystem = rs;
            factory = rs.ObjectFactory;

            material = new Material(rs);
            material.CullMode = CullMode.None;

            //material.Ambient = terrData.MaterialAmbient;
            //material.Diffuse = terrData.MaterialDiffuse;
            //material.Emissive = terrData.MaterialEmissive;
            //material.Specular = terrData.MaterialSpecular;
            //material.Power = terrData.MaterialPower;
            material.SetEffect(EffectManager.Instance.GetModelEffect(TerrainEffectFactory.Name));

            tileCol = x * 5;
            tileLat = 60 + (y - 13) * 5;

            float radtc = MathEx.Degree2Radian(tileCol);
            float radtl = MathEx.Degree2Radian(tileLat);

            topLen = PlanetEarth.GetTileWidth(MathEx.Degree2Radian(tileLat + 10), MathEx.Degree2Radian(10));
            bottomLen = PlanetEarth.GetTileWidth(radtl, MathEx.Degree2Radian(10));
            terrEdgeSize = 1025;


            //float hs = terrEdgeSize * 0.5f;
            //Matrix b1 = Matrix.Translation(-hs, 0, -hs);
            //Matrix facing = Matrix.Identity;
            //facing.Up = -PlanetEarth.GetNormal(radtc, radtl);
            //facing.Forward = PlanetEarth.GetTangentY(radtc, radtl);
            //facing.Right = -Vector3.Cross(facing.Up, facing.Forward);
            //Matrix b2 = Matrix.Translation(hs, 0, hs);

            Matrix trans = Matrix.Translation(0, 0, PlanetEarth.PlanetRadius);

            Matrix mlat = Matrix.RotationX(-radtl);
            Matrix mcol = Matrix.RotationY(radtc);

            Transformation = Matrix.RotationX(MathEx.PiOver2) * trans * mlat * mcol;//b1 * facing * b2 *
        }

        #region Resource实现
        public override int GetSize()
        {
            int size = 0;
            if (vtxBuffer != null)
            {
                size += vtxBuffer.Size;
            }

            for (int i = 0; i < indexBuffer.Length; i++)
            {
                if (indexBuffer[i] != null)
                {
                    size += indexBuffer[i].Size;
                }
            }
            return size;
        }

        protected override void load()
        {
            if (resLoc == null)
            {
                ResourceInterlock.EnterAtomicOp();
                try
                {
                    vtxDecl = factory.CreateVertexDeclaration(TerrainVertex.Elements);
                    vtxBuffer = factory.CreateVertexBuffer(4, vtxDecl, BufferUsage.WriteOnly);
                    TerrainVertex* vertices = (TerrainVertex*)vtxBuffer.Lock(LockMode.None);

                    vertices[0].Position = new Vector3(0, 0, 0);
                    vertices[2].Position = new Vector3(terrEdgeSize, 0, terrEdgeSize);
                    vertices[1].Position = new Vector3(terrEdgeSize, 0, 0);
                    vertices[3].Position = new Vector3(0, 0, terrEdgeSize);

                    vtxBuffer.Unlock();
                }
                finally
                {
                    ResourceInterlock.ExitAtomicOp();
                }

                noDataGeo = new GeomentryData(this);
                noDataGeo.VertexDeclaration = vtxDecl;

                noDataGeo.VertexSize = TerrainVertex.Size;
                noDataGeo.VertexBuffer = vtxBuffer;
                noDataGeo.PrimCount = 2;
                noDataGeo.VertexCount = 4;

                noDataGeo.PrimitiveType = RenderPrimitiveType.TriangleStrip;

                noDataGeo.BaseVertex = 0;

            }
            else
            {
                TDMPIO data = new TDMPIO();
                data.Load(resLoc);
                tileCol = data.Xllcorner;
                tileLat = data.Yllcorner;

                topLen = PlanetEarth.GetTileWidth(MathEx.Degree2Radian(tileLat + data.YSpan), MathEx.Degree2Radian(data.XSpan));
                bottomLen = PlanetEarth.GetTileWidth(MathEx.Degree2Radian(tileLat), MathEx.Degree2Radian(data.XSpan));


                MeshData meshData = new MeshData(renderSystem);


                terrEdgeSize = data.Width;
                float halfTerrSize = terrEdgeSize * 0.5f;

                int edgeVtxCount = terrEdgeSize;
                int vertexCount = edgeVtxCount * edgeVtxCount;


                #region 索引数据
                int blockEdgeLen = TerrainBlockSize - 1;
                int edgeLen = edgeVtxCount - 1;
                this.blockEdgeCount = edgeLen / blockEdgeLen;
                this.blockCount = MathEx.Sqr(blockEdgeCount);

                levelLengths = new int[LocalLodCount];
                cellSpan = new int[LocalLodCount];
                lodLevelThreshold = new float[LocalLodCount];

                levelPrimConut = new int[LocalLodCount];
                levelVertexCount = new int[LocalLodCount];

                for (int k = 0, levelLength = blockEdgeLen; k < LocalLodCount; k++, levelLength /= 2)
                {
                    int cellLength = blockEdgeLen / levelLength;


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

                #region 顶点数据

                ResourceInterlock.EnterAtomicOp();
                try
                {
                    vtxDecl = factory.CreateVertexDeclaration(TerrainVertex.Elements);

                    vtxBuffer = factory.CreateVertexBuffer(vertexCount, vtxDecl, BufferUsage.WriteOnly);
                    TerrainVertex* vertices = (TerrainVertex*)vtxBuffer.Lock(LockMode.None);

                    for (int i = 0; i < edgeVtxCount; i++)
                    {
                        float lerp = i / (float)terrEdgeSize;
                        float latCellWidth = MathEx.LinearInterpose(topLen, bottomLen, lerp) / (float)terrEdgeSize;
                        float latOfs = (1 - latCellWidth) * halfTerrSize;

                        for (int j = 0; j < edgeVtxCount; j++)
                        {
                            //float px = j * TerrainMeshManager.TerrainScale;
                            //float pz = i * TerrainMeshManager.TerrainScale;
                            vertices[i * edgeVtxCount + j].Position =
                                new Vector3(j * TerrainMeshManager.TerrainScale * latCellWidth + latOfs,
                                            data.Data[i * edgeVtxCount + j] * TerrainMeshManager.HeightScale - TerrainMeshManager.ZeroLevel,
                                            i * TerrainMeshManager.TerrainScale);
                            //  new Vector3(px,
                            //    ComputeTerrainHeight(px, pz, data.Data[i * edgeVtxCount + j], halfTerrSize, PlanetRadius),
                            //  pz);
                        }
                    }
                    BuildTerrainTree(vertices, blockEdgeCount);

                    vtxBuffer.Unlock();
                }
                finally
                {
                    ResourceInterlock.ExitAtomicOp();
                }

                #endregion

            }
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
        #endregion

        void BuildTerrainTree(TerrainVertex* vertices, int blockEdgeLen)
        {
            TerrainBlock[] blocks = new TerrainBlock[blockCount];

            float halfTerrSize = terrEdgeSize * 0.5f;

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

                    gd.BaseVertex = y * terrEdgeSize + x;

                    blocks[index].GeoData = gd;

                    for (int ii = 0; ii < TerrainBlockSize; ii++)
                    {
                        for (int jj = 0; jj < TerrainBlockSize; jj++)
                        {
                            int dmY = i * blockEdgeLen + ii;
                            int dmX = j * blockEdgeLen + jj;

                            center += vertices[dmY * terrEdgeSize + dmX].Position;
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

                            Vector3 vtxPos = vertices[dmY * terrEdgeSize + dmX].Position;

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
            rootNode = new TerrainTreeNode(new FastList<TerrainBlock>(blocks), (terrEdgeSize - 1) / 2, (terrEdgeSize - 1) / 2, 1, terrEdgeSize);

            BoundingSphere = rootNode.BoundingVolume;
            BoundingSphere.Center = Vector3.TransformSimple(BoundingSphere.Center, Transformation);
        }

        public void PrepareVisibleObjects(ICamera cam)
        {
            if (resLoc == null)
                return;

            if (State == ResourceState.Loaded)
            {
                opBuffer.Clear();

                Matrix invTrans;
                Matrix.Invert(ref Transformation, out invTrans);



                Frustum frus = cam.Frustum.Transform(invTrans);
                Vector3 camPos = Vector3.TransformSimple(cam.Position, invTrans);

                Vector3 c = rootNode.BoundingVolume.Center;
                //Vector3.TransformSimple(ref c, ref Transformation, out c);

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
                                //Vector3.TransformSimple(ref c, ref Transformation, out c);

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
                                //Vector3.TransformSimple(ref c, ref Transformation, out c);

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
                                    op.Geomentry.PrimCount = levelPrimConut[lodLevel];
                                    op.Geomentry.VertexCount = levelVertexCount[lodLevel];

                                    op.Transformation = Transformation;

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
                if (resLoc == null)
                {
                    opBuffer.Clear();
                    RenderOperation op;

                    op.Material = material;
                    op.Geomentry = noDataGeo;

                    op.Transformation = Transformation;

                    opBuffer.Add(op);
                    return opBuffer.Elements;
                }

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
