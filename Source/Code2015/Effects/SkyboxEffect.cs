using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;

namespace Code2015.Effects
{
    public class SkyboxEffectFactory : EffectFactory
    {
        static readonly string typeName = "Skybox";

        public static string Name
        {
            get { return typeName; }
        }


        RenderSystem renderSystem;

        public SkyboxEffectFactory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new WaterEffect(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class SkyboxEffect : Effect
    {
        RenderSystem renderSystem;

        PixelShader pixShader;
        VertexShader vtxShader;

        public SkyboxEffect(RenderSystem renderSystem)
            : base(false, SkyboxEffectFactory.Name)
        {
            this.renderSystem = renderSystem;
        }

        protected override int begin()
        {
            renderSystem.BindShader(vtxShader);
            renderSystem.BindShader(pixShader);

            return 1;
        }

        protected override void end()
        {
            renderSystem.BindShader((VertexShader)null);
            renderSystem.BindShader((PixelShader)null);
        }

        public override void BeginPass(int passId)
        {
        }

        public override void EndPass()
        {

        }

        //public override void BeginShadowPass()
        //{
        //}

        //public override void EndShadowPass()
        //{
        //}

        public override void Setup(Material mat, ref RenderOperation op)
        {
            pixShader.SetTexture("texColor", mat.GetTexture(0));
            Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;

            vtxShader.SetValue("mvp", ref mvp);
        }

        //public override void SetupShadowPass(Material mat, ref RenderOperation op)
        //{
        //}

        protected override void Dispose(bool disposing)
        {
            vtxShader.Dispose();
            pixShader.Dispose();
        }
    }
}
