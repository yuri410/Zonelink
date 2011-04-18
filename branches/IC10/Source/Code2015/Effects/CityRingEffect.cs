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

    class CityRingEffect : RigidEffect
    {
        bool stateSetted;

        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        public unsafe CityRingEffect(RenderSystem rs)
           : base(rs, CityRingEffectFactory.Name, false)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("cityring.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("cityring.cps", GameFileLocs.Effect);
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
                pixShader.SetSamplerState("hatch0", ref state2);
                pixShader.SetSamplerState("hatch1", ref state2);


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
                    if (clrTex != null)
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

                City sender = op.Sender as City;

                pixShader.SetValue("k_a", mat.Ambient);
                if (sender != null && sender.IsCaptured)
                {
                    Color4F color = new Color4F(sender.Owner.SideColor);
                    pixShader.SetValue("k_d", color);
                }
                else
                {
                    pixShader.SetValue("k_d", mat.Diffuse);
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

                    pixShader.SetSamplerState("texDif", ref state);

                    ResourceHandle<Texture> clrTex = mat.GetTexture(0);
                    if (clrTex != null)
                    {
                        pixShader.SetTexture("texDif", clrTex);
                    }

                    stateSetted = true;
                }
            }
        }


        protected override void Dispose(bool disposing)
        {

        }
    }
}
