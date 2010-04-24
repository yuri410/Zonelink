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
    public class GoalEffectFactory : EffectFactory
    {
        static readonly string typeName = "Goal";


        public static string Name
        {
            get { return typeName; }
        }



        RenderSystem device;

        public GoalEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new GoalEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class GoalEffect : Effect
    {
        bool stateSetted;

        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        Texture noTexture;

        public unsafe GoalEffect(RenderSystem rs)
            : base(false, GoalEffectFactory.Name)
        {
            FileLocation fl = FileSystem.Instance.Locate("tillingmark.tex", GameFileLocs.Texture);
            noTexture = TextureManager.Instance.CreateInstance(fl);

            this.renderSys = rs;

            fl = FileSystem.Instance.Locate("goal.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("goal.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);

        }


        protected override int begin()
        {
            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);
            pixShader.SetValue("i_a", EffectParams.LightAmbient);
            pixShader.SetValue("i_d", EffectParams.LightDiffuse);
            
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

        Matrix rot45 = Matrix.RotationX(MathEx.PiOver4);

        public override void Setup(Material mat, ref RenderOperation op)
        {
            Matrix world = rot45 * op.Transformation;
            Matrix mvp = world * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;
            
            vtxShader.SetValue("mvp", ref mvp);
            pixShader.SetValue("world", ref world);
            pixShader.SetValue("lightDir", EffectParams.InvView.Forward);

            if (!stateSetted)
            {
                ShaderSamplerState state = new ShaderSamplerState();
                state.AddressU = TextureAddressMode.Wrap;
                state.AddressV = TextureAddressMode.Wrap;
                state.AddressW = TextureAddressMode.Wrap;
                state.MinFilter = TextureFilter.Linear;
                state.MagFilter = TextureFilter.Linear;
                state.MipFilter = TextureFilter.Linear;
                state.MaxAnisotropy = 8;
                state.MipMapLODBias = -3;

                pixShader.SetValue("k_a", mat.Ambient);
                pixShader.SetValue("k_d", mat.Diffuse);
                //pixShader.SetValue("k_e", mat.Emissive);

                pixShader.SetSamplerState("texDif", ref state);

                ResourceHandle<Texture> clrTex = mat.GetTexture(0);
                if (clrTex == null)
                {
                    pixShader.SetTexture("texDif", noTexture);
                }
                else
                {
                    pixShader.SetTexture("texDif", clrTex);
                }
                clrTex = mat.GetTexture(1);
                if (clrTex == null)
                {
                    pixShader.SetTexture("texNrm", noTexture);
                }
                else
                {
                    pixShader.SetTexture("texNrm", clrTex);
                }

                stateSetted = true;
            }
        }

        

        protected override void Dispose(bool disposing)
        {
    

        }
    }
}
