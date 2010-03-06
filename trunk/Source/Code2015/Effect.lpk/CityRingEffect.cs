﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;

namespace Apoc3D.Graphics.Effects
{
    public class CityRingEffectFactory : EffectFactory
    {
        static readonly string typeName = "CityRing";


        public static string Name
        {
            get { return typeName; }
        }



        RenderSystem device;

        public CityRingEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new CityRingEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class CityRingEffect : Effect
    {
        bool stateSetted;

        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        public unsafe CityRingEffect(RenderSystem rs)
            : base(false, StandardEffectFactory.Name)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("cityring.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("cityring.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);

        }

        

        protected override int begin()
        {
            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);
            pixShader.SetValue("lightDir", EffectParams.LightDir);

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
            vtxShader.SetValue("world", ref op.Transformation);

            CityOwnerRing ring = op.Geomentry.Sender as CityOwnerRing;
            pixShader.SetValue("weights", new Vector4(0.5f, 0.2f, 0.2f, 0.1f));

            Matrix colors = new Matrix();
            colors.SetRow(0, new Vector4(1, 0, 0, 1));
            colors.SetRow(1, new Vector4(0, 1, 0, 1));
            colors.SetRow(2, new Vector4(0, 0, 1, 1));
            colors.SetRow(3, new Vector4(1, 1, 0, 1));
            pixShader.SetValue("ownerColors", colors);


            if (!stateSetted)
            {
                ShaderSamplerState state = new ShaderSamplerState();
                state.AddressU = TextureAddressMode.Wrap;
                state.AddressV = TextureAddressMode.Wrap;
                state.AddressW = TextureAddressMode.Wrap;
                state.MinFilter = TextureFilter.Anisotropic;
                state.MagFilter = TextureFilter.Anisotropic;
                state.MipFilter = TextureFilter.Anisotropic;
                state.MaxAnisotropy = 8;
                state.MipMapLODBias = 0;

                //pixShader.SetValue("k_a", mat.Ambient);
                //pixShader.SetValue("k_d", mat.Diffuse);
                //pixShader.SetValue("k_s", mat.Specular);
                //pixShader.SetValue("k_e", mat.Emissive);
                //pixShader.SetValue("k_power", mat.Power);

                pixShader.SetSamplerState("texDif", ref state);

                ResourceHandle<Texture> clrTex = mat.GetTexture(0);
                pixShader.SetTexture("texDif", clrTex != null ? clrTex.Resource : null);

                stateSetted = true;
            }
        }


        protected override void Dispose(bool disposing)
        {

        }
    }
}