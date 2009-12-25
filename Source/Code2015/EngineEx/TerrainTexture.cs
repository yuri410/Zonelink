using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Core;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class TerrainTexture : Resource
    {
        Texture texture;

        public TerrainTexture(RenderSystem rs, ResourceLocation rl)
            : base(TerrainTextureManager.Instance, rl.Name)
        {
        }

        public override int GetSize()
        {
            throw new NotImplementedException();
        }

        protected override void load()
        {
            throw new NotImplementedException();
        }

        protected override void unload()
        {
            throw new NotImplementedException();
        }
    }
}
