using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            public uint Normal;

            static VertexElement[] elements;

            static TerrainVertex()
            {
                elements = new VertexElement[2];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);

                elements[1] = new VertexElement(Vector3.SizeInBytes, VertexElementFormat.Color, VertexElementUsage.TextureCoordinate, 0);
            }

            public void SetNormal(float x, float y, float z)
            {
                Normal = ColorValue.PackValue(x, y, z, 1);
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }

            public static int Size
            {
                get { return Vector3.SizeInBytes + sizeof(uint); }
            }
        }

        FileLocation resLoc;

        /// <summary>
        ///  地形一条边上的顶点数
        /// </summary>
        int terrEdgeSize;


        VertexDeclaration vtxDecl;
        VertexBuffer vtxBuffer;

        IndexBuffer[] indexBuffer;

        /// <summary>
        ///  世界LOD级别
        /// </summary>
        int dataLevel;

        RenderSystem renderSystem;
        ObjectFactory factory;


        FastList<RenderOperation> opBuffer;

        Material material;

        public BoundingSphere BoundingSphere;
        public Matrix Transformation = Matrix.Identity;

        /// <summary>
        ///  表示地形是否是分块的
        /// </summary>
        bool isBlockTerrain;

        #region 分块地形的数据
        /// <summary>
        ///  地形块的数量
        /// </summary>
        int blockCount;

        /// <summary>
        ///  地形分块的在地形的一条边上的数量
        /// </summary>
        int blockEdgeCount;

        /// <summary>
        ///  不同的lod级别下一个地形分块的长度
        /// </summary>
        int[] levelLengths;

        /// <summary>
        ///  在不同lod级别下一个单元的跨度
        /// </summary>
        int[] cellSpan;

        /// <summary>
        ///  lod 权值
        /// </summary>
        float[] lodLevelThreshold;

        /// <summary>
        ///  不同的lod级别下一个地形分块的三角形数量
        /// </summary>
        int[] levelPrimConut;

        /// <summary>
        ///  不同的lod级别下一个地形分块的顶点数量
        /// </summary>
        int[] levelVertexCount;

        Queue<TerrainTreeNode> bfsQueue;
        TerrainTreeNode rootNode;
        #endregion

        /// <summary>
        ///  不是分块地形时，
        /// </summary>
        GeomentryData defGeometryData;


        /// <summary>
        ///  地形的上边（纬度较高的边）的长度
        /// </summary>
        float topLen;
        /// <summary>
        ///  地形的下边（纬度较低的边）的长度
        /// </summary>
        float bottomLen;
        /// <summary>
        ///  地形的侧边（沿经度方向）的长度
        /// </summary>
        float heightLen;

        float tileCol;
        float tileLat;

        /// <summary>
        ///  经度
        /// </summary>
        public float TileCol
        {
            get { return tileCol; }
        }
        /// <summary>
        ///  纬度
        /// </summary>
        public float TileLat
        {
            get { return tileLat; }
        }


        public static string GetHashString(int x, int y, int lod)
        {
            return "TM" + x.ToString("D2") + y.ToString("D2") + lod.ToString("D1");
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

            tileCol = x * 5 - 185;
            tileLat = 50 - y * 5;

            float radtc = MathEx.Degree2Radian(tileCol);
            float radtl = MathEx.Degree2Radian(tileLat);
            terrEdgeSize = 1025;

            UpdateTransformation(radtc, radtl, terrEdgeSize, 10);
        }

        /// <summary>
        ///  计算该地形的变换矩阵
        /// </summary>
        /// <param name="radtc">经度</param>
        /// <param name="radtl">纬度</param>
        /// <param name="terrSize">原始地形大小</param>
        /// <param name="span">边所占的度数</param>
        void UpdateTransformation(float radtc, float radtl, float terrSize, float span)
        {
            float rad10 = MathEx.Degree2Radian(span);
            float rad5 = MathEx.Degree2Radian(span * 0.5f);
            topLen = PlanetEarth.GetTileWidth(radtl + rad10, rad10);
            bottomLen = PlanetEarth.GetTileWidth(radtl, rad10);
            heightLen = PlanetEarth.GetTileHeight(rad10);

            float poscol = radtc + rad5;
            float poslat = radtl + rad5;

            terrSize--;
            float hs = terrSize * 0.5f;
            Matrix b1 = Matrix.Translation(-hs, 0, -hs);
            Matrix facing = Matrix.Identity;
            facing.Up = PlanetEarth.GetNormal(poscol, poslat);

            Vector3 v = Vector3.Cross(Vector3.UnitY, facing.Up);
            v.Normalize();
            facing.Right = v;// PlanetEarth.GetTangentX(poscol, poslat);

            v = Vector3.Cross(facing.Right, facing.Up);
            v.Normalize();

            facing.Forward = -v;// PlanetEarth.GetTangentY(poscol, poslat);            

            Matrix scaling = Matrix.Scaling(1, 1, heightLen / terrSize);

            Vector3 position = PlanetEarth.GetInnerPosition(poscol, poslat, rad10);
            Transformation = b1 * scaling * facing * Matrix.Translation(position);

            BoundingSphere.Center = position;
            BoundingSphere.Radius = terrSize * MathEx.Root2;
        }


        #region Resource实现
        public override int GetSize()
        {
            int size = 0;
            if (resLoc != null)
            {
                switch (dataLevel)
                {
                    case 0:
                        size += TerrainVertex.Size * 1025 * 1025;
                        size += sizeof(int) * (32 * 32) * 6 * LocalLodCount;
                        break;
                    case 1:
                        size += TerrainVertex.Size * 257 * 257;
                        size += sizeof(int) * (8 * 8) * 6 * LocalLodCount;
                        break;
                    case 2:
                        size += TerrainVertex.Size * 65 * 65;
                        size += sizeof(int) * (2 * 2) * 6 * LocalLodCount;
                        break;
                }
            }

            return size;
        }

        protected override void load()
        {
            if (resLoc == null)
                return;

            // 读取地形数据
            TDMPIO data = new TDMPIO();
            data.Load(resLoc);
            tileCol = (float)Math.Round(data.Xllcorner);
            tileLat = (float)Math.Round(data.Yllcorner);

            float radtc = MathEx.Degree2Radian(tileCol);
            float radtl = MathEx.Degree2Radian(tileLat);
            terrEdgeSize = data.Width;

            UpdateTransformation(radtc, radtl, terrEdgeSize, data.XSpan);

            int vertexCount = terrEdgeSize * terrEdgeSize;
            int terrEdgeLen = terrEdgeSize - 1;
            isBlockTerrain = terrEdgeSize >= TerrainBlockSize;

            #region 顶点数据

            vtxDecl = factory.CreateVertexDeclaration(TerrainVertex.Elements);

            vtxBuffer = factory.CreateVertexBuffer(vertexCount, vtxDecl, BufferUsage.WriteOnly);

            TerrainVertex[] vtxArray = new TerrainVertex[vertexCount];

            #region 计算局部坐标系下的地心坐标
            //float beta = 0.5f * (MathEx.PIf - MathEx.Degree2Radian(data.XSpan));

            //Vector3 localEarthCenter = new Vector3(terrEdgeLen * 0.5f, 0, terrEdgeLen * 0.5f);
            //localEarthCenter.Y = -(float)Math.Sin(beta) * PlanetRadius;

            Matrix invTrans = Matrix.Invert(Transformation);
            Vector3 localEarthCenter = Vector3.TransformSimple(Vector3.Zero, invTrans);
            #endregion

            #region 计算顶点坐标
            for (int i = 0; i < terrEdgeSize; i++)
            {
                float lerp = i / (float)(terrEdgeLen);
                float colCellWidth = MathEx.LinearInterpose(topLen, bottomLen, lerp) / (float)terrEdgeLen;
                float colOfs = (1 - colCellWidth) * terrEdgeLen * 0.5f;

                for (int j = 0; j < terrEdgeSize; j++)
                {
                    Vector3 pos = new Vector3(j * TerrainMeshManager.TerrainScale * colCellWidth + colOfs,
                                    0,
                                    i * TerrainMeshManager.TerrainScale);

                    // 计算海拔高度
                    float height = data.Data[i * terrEdgeSize + j] * TerrainMeshManager.HeightScale - TerrainMeshManager.ZeroLevel;

                    Vector3 worldPos;
                    Vector3.TransformSimple(ref pos, ref Transformation, out worldPos);

                    // 计算曲面偏移量
                    float delta = PlanetRadius - worldPos.Length();

                    Vector3 normal = pos - localEarthCenter;
                    normal.Normalize();
                    vtxArray[i * terrEdgeSize + j].Position = pos + normal * (height + delta);
                    vtxArray[i * terrEdgeSize + j].SetNormal(normal.X, normal.Y, normal.Z);

                }
            }
            #endregion

            #endregion

            if (isBlockTerrain)
            {
                #region 索引数据
                int blockEdgeLen = TerrainBlockSize - 1;
                this.blockEdgeCount = terrEdgeLen / blockEdgeLen;
                this.blockCount = MathEx.Sqr(blockEdgeCount);

                SharedBlockIndexData sharedData = TerrainMeshManager.Instance.GetSharedIndexData(terrEdgeSize);

                levelLengths = sharedData.LevelLength;
                cellSpan = sharedData.CellSpan;
                lodLevelThreshold = sharedData.LodLevelThreshold;
                levelPrimConut = sharedData.LevelPrimCount;
                levelVertexCount = sharedData.LevelVertexCount;
                indexBuffer = sharedData.IndexBuffers;
                
                #endregion

                BuildTerrainTree(vtxArray);
            }
            else
            {
                #region 索引数据
                this.blockEdgeCount = 1;
                this.blockCount = 1;

                levelLengths = new int[1];
                cellSpan = new int[1];
                lodLevelThreshold = new float[1];

                levelPrimConut = new int[1];
                levelVertexCount = new int[1];


                cellSpan[0] = 1;
                levelLengths[0] = terrEdgeLen;

                int indexCount = MathEx.Sqr(terrEdgeLen) * 2 * 3;

                levelPrimConut[0] = MathEx.Sqr(terrEdgeLen) * 2;
                levelVertexCount[0] = MathEx.Sqr(terrEdgeSize);

                indexBuffer[0] = factory.CreateIndexBuffer(IndexBufferType.Bit32, indexCount, BufferUsage.WriteOnly);

                int[] indexArray = new int[indexCount];

                for (int i = 0, index = 0; i < terrEdgeLen; i++)
                {
                    for (int j = 0; j < terrEdgeLen; j++)
                    {
                        int x = i;
                        int y = j;

                        indexArray[index++] = y * terrEdgeSize + x;
                        indexArray[index++] = y * terrEdgeSize + (x + 1);
                        indexArray[index++] = (y + 1) * terrEdgeSize + (x + 1);

                        indexArray[index++] = y * terrEdgeSize + x;
                        indexArray[index++] = (y + 1) * terrEdgeSize + (x + 1);
                        indexArray[index++] = (y + 1) * terrEdgeSize + x;
                    }
                }
                indexBuffer[0].SetData<int>(indexArray);
                #endregion

                #region 构造GeomentryData
                defGeometryData = new GeomentryData(this);
                defGeometryData.VertexDeclaration = vtxDecl;

                defGeometryData.VertexSize = TerrainVertex.Size;
                defGeometryData.VertexBuffer = vtxBuffer;
                defGeometryData.IndexBuffer = indexBuffer[0];
                defGeometryData.PrimCount = levelPrimConut[0];
                defGeometryData.VertexCount = levelVertexCount[0];

                defGeometryData.PrimitiveType = RenderPrimitiveType.TriangleList;

                defGeometryData.BaseVertex = 0;

                #endregion
            }
            vtxBuffer.SetData<TerrainVertex>(vtxArray);
        }

        protected override void unload()
        {
            if (!object.ReferenceEquals(vtxBuffer, null))
            {
                vtxBuffer.Dispose();
                vtxBuffer = null;
            }
            if (!object.ReferenceEquals(vtxDecl, null))
            {
                vtxDecl.Dispose();
                vtxDecl = null;
            }
            indexBuffer = null;
            if (rootNode != null)
            {
                rootNode.Dispose();
                rootNode = null;
            }
        }
        #endregion

        /// <summary>
        ///  构造地形树
        /// </summary>
        /// <param name="vertices">顶点数据</param>
        void BuildTerrainTree(TerrainVertex[] vertices)
        {
            // 地块边的长度，边定点数减1
            int blockEdgeLen = TerrainBlockSize - 1;
            TerrainBlock[] blocks = new TerrainBlock[blockCount];

            float halfTerrSize = terrEdgeSize * 0.5f;

            int index = 0;

            // 枚举每个地块
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
                    gd.PrimCount = levelPrimConut[0];
                    gd.VertexCount = levelVertexCount[0];

                    gd.PrimitiveType = RenderPrimitiveType.TriangleList;

                    int x = (j == 0) ? 0 : j * blockEdgeLen;
                    int y = (i == 0) ? 0 : i * blockEdgeLen;

                    gd.BaseVertex = y * terrEdgeSize + x;

                    blocks[index].GeoData = gd;

                    #region 计算包围球中心点
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

                    #endregion

                    #region 计算包围球半径

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

                    #endregion
                    index++;
                }
            }
            rootNode = new TerrainTreeNode(new FastList<TerrainBlock>(blocks), (terrEdgeSize - 1) / 2, (terrEdgeSize - 1) / 2, 1, terrEdgeSize);

            BoundingSphere = rootNode.BoundingVolume;
            BoundingSphere.Center = Vector3.TransformSimple(rootNode.BoundingVolume.Center, Transformation);
        }

        /// <summary>
        ///  准备特定lod级别下的可见物体
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="level"></param>
        public void PrepareVisibleObjects(ICamera cam, int level)
        {
            if (resLoc == null)
                return;

            if (State == ResourceState.Loaded)
            {
                opBuffer.Clear();

                if (isBlockTerrain)
                {
                    #region 分块地形的可见性测试
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

                                        int lodLevel = LocalLodCount - 1;

                                        for (int lod = 0; lod < LocalLodCount; lod++)
                                        {
                                            if (dist <= lodLevelThreshold[LocalLodCount - lod - 1])
                                            {
                                                lodLevel = lod;
                                                break;
                                            }
                                        }

                                        op.Geomentry.IndexBuffer = node.Block.IndexBuffers[lodLevel];
                                        op.Geomentry.PrimCount = levelPrimConut[lodLevel];
                                        op.Geomentry.VertexCount = levelVertexCount[lodLevel];

                                        op.Transformation = Matrix.Identity;

                                        opBuffer.Add(op);
                                    }
                                }
                            }

                        }
                    }
                    #endregion
                }
                else
                {
                    RenderOperation op;

                    op.Material = material;
                    op.Geomentry = defGeometryData;

                    op.Transformation = Matrix.Identity;

                    opBuffer.Add(op);
                }
            }
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (State == ResourceState.Loaded)
            {
                if (resLoc != null)
                {
                    return opBuffer.Elements;
                }
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
