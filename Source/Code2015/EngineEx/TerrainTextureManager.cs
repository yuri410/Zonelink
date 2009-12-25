using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class TerrainTextureManager : ResourceManager
    {
        static volatile TerrainTextureManager singleton;
        static volatile object syncHelper = new object();

        public static TerrainTextureManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncHelper)
                    {
                        if (singleton == null)
                        {
                            singleton = new TerrainTextureManager(1028576 * 20);
                        }
                    }
                }
                return singleton;
            }
        }


        private TerrainTextureManager(int cacheSize)
            : base(cacheSize)
        {
        }

        public ResourceHandle<TerrainTexture> CreateInstance(RenderSystem rs, ResourceLocation rl)
        {
            Resource retrived = base.Exists(rl.Name);
            if (retrived == null)
            {
                TerrainTexture mdl = new TerrainTexture(rs, rl);
                retrived = mdl;
                base.NotifyResourceNew(mdl);
            }
            //else
            //{
            //    retrived.Use();
            //}
            return new ResourceHandle<TerrainTexture>((TerrainTexture)retrived);
        }
    }
}
