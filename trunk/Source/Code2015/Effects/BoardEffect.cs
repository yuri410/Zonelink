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
using Code2015.World;

namespace Code2015.Effects
{
    public class BoardEffectFactory : EffectFactory
    {
        static readonly string typeName = "Board";


        public static string Name
        {
            get { return typeName; }
        }



        RenderSystem device;

        public BoardEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new BoardEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class BoardEffect : RigidEffect
    {
        bool stateSetted;

        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        Texture fontTexture;
        Texture noTexture;

        public unsafe BoardEffect(RenderSystem rs)
            : base(rs, BoardEffectFactory.Name, false)
        {
            FileLocation fl = FileSystem.Instance.Locate("tillingmark.tex", GameFileLocs.Texture);
            noTexture = TextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("boardfont.tex", GameFileLocs.Texture);
            fontTexture = TextureManager.Instance.CreateInstance(fl);

            this.renderSys = rs;

            fl = FileSystem.Instance.Locate("board.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("board.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);
        }

        protected override int begin()
        {
            if (mode == RenderMode.Depth)
            {
                renderSys.BindShader(shdVtxShader);
                renderSys.BindShader(shdPixShader);
            }
            else if (mode == RenderMode.DeferredNormal) 
            {
                renderSys.BindShader(nrmGenPShader);
                renderSys.BindShader(nrmGenVShader);
            }
            else
            {
                renderSys.BindShader(vtxShader);
                renderSys.BindShader(pixShader);

                pixShader.SetValue("i_a", EffectParams.LightAmbient);
                pixShader.SetValue("i_d", EffectParams.LightDiffuse);
                pixShader.SetValue("lightDir", EffectParams.LightDir);


                ShaderSamplerState state2 = new ShaderSamplerState();
                state2.AddressU = TextureAddressMode.Wrap;
                state2.AddressV = TextureAddressMode.Wrap;
                state2.AddressW = TextureAddressMode.Wrap;
                state2.MinFilter = TextureFilter.Anisotropic;
                state2.MagFilter = TextureFilter.Anisotropic;
                state2.MipFilter = TextureFilter.Linear;
                state2.MaxAnisotropy = 8;
                state2.MipMapLODBias = 0;

                pixShader.SetTexture("hatch0", MaterialLibrary.Instance.Hatch0);
                pixShader.SetTexture("hatch1", MaterialLibrary.Instance.Hatch1);
                pixShader.SetTexture("texSymbol", fontTexture);
                pixShader.SetSamplerState("hatch0", ref state2);
                pixShader.SetSamplerState("hatch1", ref state2);
                pixShader.SetSamplerState("texSymbol", ref state2);


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
                shdVtxShader.SetValue("mvp", ref lightPrjTrans);
            }
            else if (mode == RenderMode.DeferredNormal) 
            {
                Matrix worldView = op.Transformation * EffectParams.CurrentCamera.ViewMatrix;
                Matrix mvp = worldView * EffectParams.CurrentCamera.ProjectionMatrix;
                nrmGenVShader.SetValue("mvp", ref mvp);
                nrmGenVShader.SetValue("worldView", ref worldView);

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
                    state.MipMapLODBias = 0;

                    nrmGenPShader.SetSamplerState("texDif", ref state);

                    ResourceHandle<Texture> clrTex = mat.GetTexture(0);
                    if (clrTex == null)
                    {
                        nrmGenPShader.SetTexture("texDif", noTexture);
                    }
                    else
                    {
                        nrmGenPShader.SetTexture("texDif", clrTex);
                    }
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

                IResourceObject res = op.Sender as IResourceObject;
                if (res != null)
                {
                    pixShader.SetValue("amount", new Vector2(res.AmountPer, res.MaxValue));
                }
                else                 
                {
                    pixShader.SetValue("amount", new Vector2(123, 456));
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
                    state.MipMapLODBias = 0;


                    pixShader.SetValue("k_a", mat.Ambient);
                    pixShader.SetValue("k_d", mat.Diffuse);


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
