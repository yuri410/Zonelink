using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class SharedBlockIndexData
    {
        IndexBuffer[] indexBuffer;

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

        public IndexBuffer[] IndexBuffers 
        {
            get { return indexBuffer; }
        }

        public int[] LevelLength 
        {
            get { return levelLengths; }
        }
        public int[] CellSpan 
        {
            get { return cellSpan; }
        }
        public float[] LodLevelThreshold 
        {
            get { return lodLevelThreshold; }
        }
        public int[] LevelPrimCount 
        {
            get { return levelPrimConut; }
        }
        public int[] LevelVertexCount 
        {
            get { return levelVertexCount; }
        }
        public int TerrainSize
        {
            get;
            private set;
        }
        public SharedBlockIndexData(RenderSystem rs, int terrSize) 
        {
            TerrainSize = terrSize;
        }
    }
    class SharedIndexData
    {
        public int TerrainSize
        {
            get;
            private set;
        }
        public SharedIndexData(RenderSystem rs, int terrSize)
        {
            TerrainSize = terrSize;
        }
    }
    class TerrainMeshManager : ResourceManager
    {
        static volatile TerrainMeshManager singleton;
        static volatile object syncHelper = new object();

        public static TerrainMeshManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncHelper)
                    {
                        if (singleton == null)
                        {
                            singleton = new TerrainMeshManager(1028576 * 80);
                        }
                    }
                }
                return singleton;
            }
        }

        public const float TerrainScale = 1;

        public const float HeightScale = 5500 * 0.01f;
        public const float ZeroLevel = 100;

        bool loaded;
        SharedBlockIndexData sharedIdxBuffer1025;
        SharedBlockIndexData sharedIdxBuffer257;
        SharedBlockIndexData sharedIdxBuffer65;

        private TerrainMeshManager() { }
        private TerrainMeshManager(int cacheSize)
            : base(cacheSize)
        {
        }

        public SharedBlockIndexData SharedIndexBuffer1025
        {
            get { return sharedIdxBuffer1025; }
        }
        public SharedBlockIndexData SharedIndexBuffer257
        {
            get { return sharedIdxBuffer257; }
        }
        public SharedBlockIndexData SharedIndexBuffer65
        {
            get { return sharedIdxBuffer65; }
        }
        public ResourceHandle<TerrainMesh> CreateInstance(RenderSystem rs, int x, int y, int lod)
        {
            if (!loaded) 
            {
                lock (syncHelper) 
                {
                    if (!loaded)
                    {
                        loaded = true;
                        sharedIdxBuffer1025 = new SharedBlockIndexData(rs, 1025);
                        sharedIdxBuffer257 = new SharedBlockIndexData(rs, 257);
                        sharedIdxBuffer65 = new SharedBlockIndexData(rs, 65);
                    }
                }
            }
            Resource retrived = base.Exists(TerrainMesh.GetHashString(x, y, lod));
            if (retrived == null)
            {
                TerrainMesh mdl = new TerrainMesh(rs, x, y, lod);
                retrived = mdl;
                base.NotifyResourceNew(mdl);
            }
            //else
            //{
            //    retrived.Use();
            //}
            return new ResourceHandle<TerrainMesh>((TerrainMesh)retrived);
        }
    }
}
