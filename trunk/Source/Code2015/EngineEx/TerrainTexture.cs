using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    public class TerrainTexture : Resource
    {
        Texture texture;
        FileLocation texLoc;

        public TerrainTexture(RenderSystem rs, FileLocation fl)
            : base(TerrainTextureManager.Instance, fl.Name)
        {
            texLoc = fl;
        }

        public override int GetSize()
        {
            if (texture != null)
                return texture.GetSize();
            return 0;
        }

        protected override void load()
        {
            texture = TextureManager.Instance.CreateInstance(texLoc);
        }

        protected override void unload()
        {
            texture.Dispose();
            texture = null;
        }


        public Texture Texture 
        {
            get { return texture; }
        }
    }
}
