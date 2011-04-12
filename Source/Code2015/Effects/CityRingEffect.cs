﻿/*
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

    class CityRingEffect : Effect
    {
        public static readonly Matrix WhiteMatrix =
              new Matrix(1, 1, 1, 1,
                         1, 1, 1, 1,
                         1, 1, 1, 1,
                         1, 1, 1, 1);
        bool stateSetted;

        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;

        public unsafe CityRingEffect(RenderSystem rs)
            : base(false, CityRingEffectFactory.Name)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("cityring.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("cityring.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);

        }

        public override bool SupportsMode(RenderMode mode)
        {
            switch (mode)
            {
                case RenderMode.Depth:
                    return false;
                case RenderMode.Final:
                case RenderMode.Simple:
                case RenderMode.Wireframe:
                    return true;
            }
            return false;
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

            //object sdr = op.Sender;
            //bool passed = false;
            //if (sdr != null)
            //{
            //    CityOwnerRing ring = sdr as CityOwnerRing;

            //    if (ring != null)
            //    {
            //        pixShader.SetValue("weights", new Vector4(1, 0, 0, 0));

            //        pixShader.SetValue("ownerColors", ring.GetColorMatrix());
            //        passed = true;

            //    }
            //}
            //if (!passed)
            //{
                pixShader.SetValue("weights", new Vector4(0,0,0,0));

                pixShader.SetValue("ownerColors", WhiteMatrix);
            //}
            

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
