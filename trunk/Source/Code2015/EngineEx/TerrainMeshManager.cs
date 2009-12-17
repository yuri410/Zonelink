using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
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
                            singleton = new TerrainMeshManager();
                        }
                    }
                }
                return singleton;
            }
        }


        public const float TerrainScale = 1;

        public const float HeightScale = 550;
        public const float ZeroLevel = 100;

        private TerrainMeshManager() { }
        private TerrainMeshManager(int cacheSize)
            : base(cacheSize)
        {
        }
        public ResourceHandle<TerrainMesh> CreateInstance(RenderSystem rs, int x, int y, int lod)
        {
            Resource retrived = base.Exists(TerrainMesh.GetHashString(x, y, lod));
            if (retrived == null)
            {
                TerrainMesh mdl = new TerrainMesh(rs, x, y, lod);
                retrived = mdl;
                base.NotifyResourceNew(mdl);
            }
            else
            {
                retrived.Use();
            }
            return new ResourceHandle<TerrainMesh>((TerrainMesh)retrived);
        }
    }
}
