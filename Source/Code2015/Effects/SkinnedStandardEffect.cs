/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
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
    public class SkinnedStandardEffectFactory : EffectFactory
    {
        static readonly string typeName = "SkinnedStandard";


        public static string Name
        {
            get { return typeName; }
        }



        RenderSystem device;

        public SkinnedStandardEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new SkinnedStandardEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class SkinnedStandardEffect : RigidEffect
    {
        bool stateSetted;

        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        VertexShader skShdVtxShader;   


        Texture noTexture;

        public unsafe SkinnedStandardEffect(RenderSystem rs)
            : base(rs, SkinnedStandardEffectFactory.Name, false)
        {
            FileLocation fl = FileSystem.Instance.Locate("tillingmark.tex", GameFileLocs.Texture);
            noTexture = TextureManager.Instance.CreateInstance(fl);// fac.CreateTexture(1, 1, 1, TextureUsage.Static, ImagePixelFormat.A8R8G8B8);


            this.renderSys = rs;

            fl = FileSystem.Instance.Locate("skinnedstandard.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("skinnedstandard.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("skinnedShadowMap.cvs", GameFileLocs.Effect);
            skShdVtxShader = LoadVertexShader(renderSys, fl);
        }

        #region Instance

        //protected override int beginInst()
        //{
        //    stateSetted = false;
        //    return effectInst.Begin(FX.DoNotSaveState | FX.DoNotSaveShaderState | FX.DoNotSaveSamplerState);
        //}
        //protected override void endInst()
        //{
        //    effectInst.End();
        //}
        //public override void BeginPassInst(int passId)
        //{
        //    effectInst.BeginPass(passId);
        //}
        //public override void EndPassInst()
        //{
        //    effectInst.EndPass();
        //}
        #endregion

        protected override int begin()
        {
            if (mode == RenderMode.Depth)
            {
                renderSys.BindShader(skShdVtxShader);
                renderSys.BindShader(shdPixShader);
            }
            else
            {
                renderSys.BindShader(vtxShader);
                renderSys.BindShader(pixShader);

                pixShader.SetValue("i_a", EffectParams.LightAmbient);
                pixShader.SetValue("i_d", EffectParams.LightDiffuse);
                pixShader.SetValue("i_s", EffectParams.LightSpecular);
                pixShader.SetValue("lightDir", EffectParams.LightDir);
                vtxShader.SetValue("viewPos", EffectParams.CurrentCamera.Position);
                

                ShaderSamplerState state = new ShaderSamplerState();
                state.AddressU = TextureAddressMode.Border;
                state.AddressV = TextureAddressMode.Border;
                state.AddressW = TextureAddressMode.Border;
                state.MinFilter = TextureFilter.Linear;
                state.MagFilter = TextureFilter.Linear;
                state.MipFilter = TextureFilter.None;
                state.BorderColor = ColorValue.White;
                state.MaxAnisotropy = 0;
                state.MipMapLODBias = 0;

                pixShader.SetSamplerState("texShd", ref state);
                pixShader.SetTexture("texShd", EffectParams.DepthMap[0]);
            }

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
            if (mode == RenderMode.Depth)
            {
                Matrix lightPrjTrans;
                Matrix.Multiply(ref op.Transformation, ref EffectParams.DepthViewProj, out lightPrjTrans);
                skShdVtxShader.SetValue("mvp", ref lightPrjTrans);
                if (op.BoneTransforms != null)
                {
                    skShdVtxShader.SetValue("Bones", op.BoneTransforms);
                }
            }
            else
            {
                Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;

                vtxShader.SetValue("mvp", ref mvp);
                vtxShader.SetValue("world", ref op.Transformation);

                Matrix lightPrjTrans;
                Matrix.Multiply(ref op.Transformation, ref EffectParams.DepthViewProj, out lightPrjTrans);

                vtxShader.SetValue("smTrans", lightPrjTrans);

                if (op.BoneTransforms != null)
                {
                    vtxShader.SetValue("Bones", op.BoneTransforms);
                }

                if (!stateSetted)
                {
                    ShaderSamplerState state = new ShaderSamplerState();
                    state.AddressU = TextureAddressMode.Wrap;
                    state.AddressV = TextureAddressMode.Wrap;
                    state.AddressW = TextureAddressMode.Wrap;
                    state.MinFilter = TextureFilter.Anisotropic;
                    state.MagFilter = TextureFilter.Anisotropic;
                    state.MipFilter = TextureFilter.Linear;
                    state.MaxAnisotropy = 8;
                    state.MipMapLODBias = -1;


                    pixShader.SetValue("k_a", mat.Ambient);
                    pixShader.SetValue("k_d", mat.Diffuse);
                    pixShader.SetValue("k_s", mat.Specular);
                    pixShader.SetValue("k_e", mat.Emissive);
                    pixShader.SetValue("k_power", mat.Power);

                   
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

                    if (mat.IsVegetation)
                    {
                        vtxShader.SetValue("isVegetation", new Vector4(100, 100, 100, 100));
                    }
                    else
                    {
                        vtxShader.SetValue("isVegetation", new Vector4());
                    }
                    stateSetted = true;
                }
            }
        }

        //public override void SetupInstancing(Material mat)
        //{
        //    if (!stateSetted)
        //    {
        //        Light light = EffectParams.Atmosphere.Light;
        //        Vector3 lightDir = light.Direction;
        //        effectInst.SetValue(tlParamLa, light.Ambient);
        //        effectInst.SetValue(tlParamLd, light.Diffuse);
        //        effectInst.SetValue(tlParamLs, light.Specular);
        //        effectInst.SetValue(tlParamLdir, new float[3] { lightDir.X, lightDir.Y, lightDir.Z });

        //        Vector3 pos = EffectParams.CurrentCamera.Position;
        //        effectInst.SetValue(tlParamVpos, new float[3] { pos.X, pos.Y, pos.Z });

        //        effectInst.SetTexture(shadowMapParam, EffectParams.ShadowMap.ShadowColorMap);

        //        stateSetted = true;
        //    }
        //    effectInst.SetValue(tlParamKa, mat.mat.Ambient);
        //    effectInst.SetValue(tlParamKd, mat.mat.Diffuse);
        //    effectInst.SetValue(tlParamKs, mat.mat.Specular);
        //    effectInst.SetValue(tlParamKe, mat.mat.Emissive);

        //    effectInst.SetValue(tlParamPwr, mat.mat.Power);

        //    GameTexture clrTex = mat.GetTexture(0);
        //    if (clrTex == null)
        //    {
        //        effectInst.SetTexture(tlParamClrMap, noTexture);
        //    }
        //    else
        //    {
        //        effectInst.SetTexture(tlParamClrMap, clrTex.GetTexture);
        //    }

        //    effectInst.SetValue<Matrix>(tlParamVP, EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix);

        //    effectInst.SetValue(fogColorParam, new Color4(EffectParams.Atmosphere.FogColor));
        //    effectInst.SetValue(fogDensityParam, EffectParams.Atmosphere.FogDensity);

        //    effectInst.SetValue(shadowMapTransform, EffectParams.ShadowMap.ViewProj);

        //    effectInst.CommitChanges();
        //}

        //public override void SetupShadowPass(Material mat, ref RenderOperation op)
        //{
        //    Texture clrTex = mat.GetTexture(0);
        //    //if (clrTex == null)
        //    //{
        //    //    shadowMapGen.SetTexture(tlParamClrMap, noTexture);
        //    //}
        //    //else
        //    //{
        //    //    shadowMapGen.SetTexture(tlParamClrMap, clrTex.GetTexture);
        //    //}

        //    //shadowMapGen.SetValue(tlParamMVP, op.Transformation * EffectParams.ShadowMap.ViewProj);

        //    //shadowMapGen.CommitChanges();
        //}

        //public override void BeginShadowPass()
        //{
        //    //shadowMapGen.Begin(FX.DoNotSaveState | FX.DoNotSaveShaderState | FX.DoNotSaveSamplerState);
        //    //shadowMapGen.BeginPass(0);
        //}
        //public override void EndShadowPass()
        //{
        //    //shadowMapGen.EndPass();
        //    //shadowMapGen.End();
        //}

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    effect.Dispose();

            //    tlParamLa.Dispose();
            //    tlParamLd.Dispose();
            //    tlParamLs.Dispose();
            //    tlParamKa.Dispose();
            //    tlParamKd.Dispose();
            //    tlParamKs.Dispose();
            //    tlParamPwr.Dispose();
            //    tlParamLdir.Dispose();

            //    //tlParamClrMap.Dispose();
            //    tlParamWorldT.Dispose();

            //    tlParamVpos.Dispose();
            //    tlParamVP.Dispose();
            //    tlParamMVP.Dispose();

            //    shadowMapParam.Dispose();
            //    shadowMapTransform.Dispose();

            //    shadowMapGen.Dispose();
            //}
            //effect = null;
            //tlParamLa = null;
            //tlParamLd = null;
            //tlParamLs = null;
            //tlParamKa = null;
            //tlParamKd = null;
            //tlParamKs = null;
            //tlParamPwr = null;
            //tlParamLdir = null;

            ////tlParamClrMap = null;
            //tlParamWorldT = null;

            //tlParamVpos = null;
            //tlParamVP = null;
            //tlParamMVP = null;

            //shadowMapParam = null;
            //shadowMapTransform = null;

            //shadowMapGen = null;
        }
    }
}
