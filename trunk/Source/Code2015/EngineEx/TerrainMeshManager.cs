using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class SharedIndexData
    {
        IndexBuffer indexBuffer;

        public IndexBuffer Index
        {
            get { return indexBuffer; }
        }
        public int TerrainSize
        {
            get;
            private set;
        }
        public SharedIndexData(RenderSystem rs, int terrSize)
        {
            TerrainSize = terrSize;

            ObjectFactory factory = rs.ObjectFactory;

            int primCount = MathEx.Sqr(terrSize) * 2;
            int indexCount = primCount * 3;
            indexBuffer = factory.CreateIndexBuffer(IndexBufferType.Bit32, indexCount, BufferUsage.WriteOnly);
            int[] indexArray = new int[indexCount];

            int idx = 0;
            for (int i = 0; i < terrSize - 1; i++)
            {
                int remi = i % 2;

                for (int j = 0; j < terrSize - 1; j++)
                {
                    int remj = j % 2;
                    if (remi == remj)
                    {
                        indexArray[idx++] = i * terrSize + j;
                        indexArray[idx++] = i * terrSize + (j + 1);
                        indexArray[idx++] = (i + 1) * terrSize + j;


                        indexArray[idx++] = i * terrSize + (j + 1);
                        indexArray[idx++] = (i + 1) * terrSize + (j + 1);
                        indexArray[idx++] = (i + 1) * terrSize + j;
                    }
                    else
                    {
                        indexArray[idx++] = i * terrSize + j;
                        indexArray[idx++] = (i + 1) * terrSize + (j + 1);
                        indexArray[idx++] = i * terrSize + (j + 1);

                        indexArray[idx++] = i * terrSize + j;
                        indexArray[idx++] = (i + 1) * terrSize + j;
                        indexArray[idx++] = (i + 1) * terrSize + (j + 1);
                    }
                }
            }
            indexBuffer.SetData<int>(indexArray);
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
                            singleton = new TerrainMeshManager(1048576 * 100);
                        }
                    }
                }
                return singleton;
            }
        }

        //public const float TerrainScale = 1;

        //public const float HeightScale = 5500;
        public const float PostZeroLevel = 1600;

        public const float PostHeightScale = 0.0534f;

        bool loaded;
        RenderSystem renderSystem;
        //Dictionary<int, SharedBlockIndexData> sharedIBCache = new Dictionary<int, SharedBlockIndexData>();
        SharedIndexData index33;

        private TerrainMeshManager() { }
        private TerrainMeshManager(int cacheSize)
            : base(cacheSize)
        {
        }
        public SharedIndexData GetIndexData() 
        {
            return index33;
        }
        //public SharedBlockIndexData GetSharedIndexData(int terrEdgeSize) 
        //{
        //    SharedBlockIndexData result;
        //    if (!sharedIBCache.TryGetValue(terrEdgeSize, out result ))
        //    {
        //        result = new SharedBlockIndexData(renderSystem, terrEdgeSize);
        //        sharedIBCache.Add(terrEdgeSize, result);
        //    }
        //    return result;
        //}
        public ResourceHandle<TerrainMesh> CreateInstance(RenderSystem rs, int x, int y)
        {
            if (!loaded)
            {
                lock (syncHelper)
                {
                    if (!loaded)
                    {
                        loaded = true;
                        renderSystem = rs;

                        index33 = new SharedIndexData(rs, 33);
                        //SharedBlockIndexData sharedIdxBuffer1025 = new SharedBlockIndexData(rs, 513);
                        //SharedBlockIndexData sharedIdxBuffer257 = new SharedBlockIndexData(rs, 129);
                        //SharedBlockIndexData sharedIdxBuffer65 = new SharedBlockIndexData(rs, 33);
                        //sharedIBCache.Add(513, sharedIdxBuffer1025);
                        //sharedIBCache.Add(129, sharedIdxBuffer257);
                        //sharedIBCache.Add(33, sharedIdxBuffer65);
                    }
                }
            }
            Resource retrived = base.Exists(TerrainMesh.GetHashString(x, y));
            if (retrived == null)
            {
                TerrainMesh mdl = new TerrainMesh(rs, x, y);
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
