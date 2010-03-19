using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Apoc3D.Core;

namespace Code2015.Effects
{
    public class ParticleRDEffectFactory : EffectFactory
    {
        static readonly string typeName = "Particle";


        public static string Name
        {
            get { return typeName; }
        }
        RenderSystem device;

        public ParticleRDEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new PaticleRDEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class PaticleRDEffect : Effect
    {
        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;
        Texture noTexture;

        public unsafe PaticleRDEffect(RenderSystem rs)
            : base(false, ParticleRDEffectFactory.Name)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("particle.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("particle.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("tillingmark.tex", GameFileLocs.Texture);
            noTexture = TextureManager.Instance.CreateInstance(fl);
        }


        protected override int begin()
        {
            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);

            return 1;
            //return effect.Begin(FX.DoNotSaveState | FX.DoNotSaveShaderState | FX.DoNotSaveSamplerState);
        }
        protected override void end()
        {
            //effect.End();
        }
        public override void BeginPass(int passId)
        {
            //effect.BeginPass(passId);
        }
        public override void EndPass()
        {
            //effect.EndPass();
        }

        public override void Setup(Material mat, ref RenderOperation op)
        {
            Matrix view = EffectParams.CurrentCamera.ViewMatrix;
            Vector3 tl = view.TranslationValue;
            view = Matrix.Identity;
            view.TranslationValue = tl;

            vtxShader.SetValue("mvp",
                view * EffectParams.CurrentCamera.ProjectionMatrix);

            vtxShader.SetValue("size", 1f);

            ResourceHandle<Texture> clrTex = mat.GetTexture(0);
            if (clrTex == null)
            {
                pixShader.SetTexture("texDif", noTexture);
            }
            else
            {
                pixShader.SetTexture("texDif", clrTex);
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
