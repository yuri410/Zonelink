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
    public class TreeEffectFactory : EffectFactory
    {
        static readonly string typeName = "Tree";


        public static string Name
        {
            get { return typeName; }
        }

        RenderSystem device;

        public TreeEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new TreeEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class TreeEffect : RigidEffect
    {
        bool stateSetted;

        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        float winding;
        int sign = 1;

        Texture noTexture;

        public unsafe TreeEffect(RenderSystem rs)
            : base(rs, TreeEffectFactory.Name, false)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("tree.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("tree.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);
          
            
            fl = FileSystem.Instance.Locate("tillingmark.tex", GameFileLocs.Texture);
            noTexture = TextureManager.Instance.CreateInstance(fl);// fac.CreateTexture(1, 1, 1, TextureUsage.Static, ImagePixelFormat.A8R8G8B8);

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
                pixShader.SetValue("i_s", EffectParams.LightSpecular);
                pixShader.SetValue("lightDir", EffectParams.LightDir);
                vtxShader.SetValue("viewPos", EffectParams.CurrentCamera.Position);
              
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


                winding += sign * 0.0033f;
                if (winding > 2 * MathEx.PIf)
                    winding -= 2 * MathEx.PIf;


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
                //if (winding > 1f)
                //{
                //    sign = -1;
                //}
                //else if (winding < 0f)
                //{
                //    sign = 1;
                //}
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
               
                Matrix lightPrjTrans;
                Matrix.Multiply(ref op.Transformation, ref EffectParams.DepthViewProj, out lightPrjTrans);

                vtxShader.SetValue("smTrans", lightPrjTrans);

                TreeBatchModel mdl = op.Sender as TreeBatchModel;
                if (mdl != null)
                {
                    vtxShader.SetValue("world", ref mdl.TreeOrientation);
                }
                else
                {
                    vtxShader.SetValue("world", ref op.Transformation);
                }

                if (!stateSetted)
                {

                    pixShader.SetValue("k_a", mat.Ambient);
                    pixShader.SetValue("k_d", mat.Diffuse);
                    pixShader.SetValue("k_s", mat.Specular);
                    pixShader.SetValue("k_e", mat.Emissive);
                    pixShader.SetValue("k_power", mat.Power);

                    ResourceHandle<Texture> clrTex = mat.GetTexture(0);
                    if (clrTex == null)
                    {
                        pixShader.SetTexture("texDif", null);
                    }
                    else
                    {
                        pixShader.SetTexture("texDif", clrTex);
                    }


                    Vector2 isVeg_wind = new Vector2();

                    if (mat.IsVegetation)
                    {
                        isVeg_wind.X = 100;// vtxShader.SetValue("isVeg_wind", new Vector4(100, 100, 100, 100));
                    }
                    isVeg_wind.Y = winding;
                    vtxShader.SetValue("isVeg_wind", ref isVeg_wind);


                    stateSetted = true;
                }
            }
        }
        

        protected override void Dispose(bool disposing)
        {
      
        }
    }
}
