using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Core;

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

        private TerrainMeshManager() { }

        public const float TerrainScale = 1;

        public const float HeightScale = 5500;
        public const float ZeroLevel = 100;

    }
}
