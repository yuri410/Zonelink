using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.Effects
{
    public abstract class PostEffect
    {
        protected PixelShader pixShader;
        protected VertexShader vtxShader;

        RenderSystem renderSys;

        protected PostEffect(RenderSystem rs)
        {
            renderSys = rs;
        }

        public void SetTexture(string name, Texture tex)
        {
            pixShader.SetTexture(name, tex);
        }

        public void Begin()
        {
            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);
        }

        public void End()
        {
            renderSys.BindShader((VertexShader)null);
            renderSys.BindShader((PixelShader)null);
        }

        #region Loading Shaders
        protected void LoadVertexShader(RenderSystem rs, ResourceLocation vs)
        {
            ObjectFactory fac = rs.ObjectFactory;

            vtxShader = fac.CreateVertexShader(vs);

        }
        protected void LoadPixelShader(RenderSystem rs, ResourceLocation vs)
        {
            ObjectFactory fac = rs.ObjectFactory;

            pixShader = fac.CreatePixelShader(vs);
        }
        #endregion
    }
}
