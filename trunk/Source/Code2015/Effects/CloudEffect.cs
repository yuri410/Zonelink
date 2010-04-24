using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.Effects
{
    public class CloudEffectFactory : EffectFactory
    {
        static readonly string typeName = "Cloud";


        public static string Name
        {
            get { return typeName; }
        }



        RenderSystem device;

        public CloudEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new CloudEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class CloudEffect : ShadowedEffect
    {
        bool stateSetted;

        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        public unsafe CloudEffect(RenderSystem rs)
            : base(rs, TreeEffectFactory.Name, false)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("cloud.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("cloud.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);

        }

        protected override int begin()
        {

            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);
            pixShader.SetValue("i_a", EffectParams.LightAmbient);
            pixShader.SetValue("i_d", EffectParams.LightDiffuse);
            pixShader.SetValue("lightDir", EffectParams.LightDir);

            ShaderSamplerState state = new ShaderSamplerState();
            state.AddressU = TextureAddressMode.Wrap;
            state.AddressV = TextureAddressMode.Wrap;
            state.AddressW = TextureAddressMode.Wrap;
            state.MinFilter = TextureFilter.Anisotropic;
            state.MagFilter = TextureFilter.Anisotropic;
            state.MipFilter = TextureFilter.Linear;
            state.MaxAnisotropy = 8;
            state.MipMapLODBias = 0;

            pixShader.SetSamplerState("texDif", ref state);

            //if (winding > 1f)
            //{
            //    sign = -1;
            //}
            //else if (winding < 0f)
            //{
            //    sign = 1;
            //}

            stateSetted = false;
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
            Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;
            vtxShader.SetValue("mvp", ref mvp);

            if (!stateSetted)
            {
                pixShader.SetValue("k_a", mat.Ambient);
                pixShader.SetValue("k_d", mat.Diffuse);

                ResourceHandle<Texture> clrTex = mat.GetTexture(0);
                if (clrTex == null)
                {
                    pixShader.SetTexture("texDif", null);
                }
                else
                {
                    pixShader.SetTexture("texDif", clrTex);
                }

                stateSetted = true;
            }

        }


        protected override void Dispose(bool disposing)
        {

        }
    }
}
