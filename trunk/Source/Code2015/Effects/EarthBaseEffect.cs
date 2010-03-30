using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.Effects
{
    public class EarthBaseEffectFactory : EffectFactory
    {
        static readonly string typeName = "EarthBase";


        public static string Name
        {
            get { return typeName; }
        }
        RenderSystem device;

        public EarthBaseEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new EarthBaseEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class EarthBaseEffect : Effect
    {
        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        public unsafe EarthBaseEffect(RenderSystem rs)
            : base(false, EarthBaseEffectFactory.Name)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("earthbase.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("earthbase.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);
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
            vtxShader.SetValue("mvp", 
                EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
