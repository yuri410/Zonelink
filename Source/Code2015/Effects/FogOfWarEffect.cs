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
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Apoc3D.Core;

namespace Code2015.Effects
{
    public class FogOfWarEffectFactory : EffectFactory
    {
        static readonly string typeName = "FogOfWarEffect";


        public static string Name
        {
            get { return typeName; }
        }



        RenderSystem renderSystem;

        public FogOfWarEffectFactory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new FogOfWarEffect(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class FogOfWarEffect : Effect
    {
        RenderSystem renderSystem;

        PixelShader pixShader;
        VertexShader vtxShader;

        public FogOfWarEffect(RenderSystem renderSystem)
            : base(false, WaterEffectFactory.Name)
        {
            this.renderSystem = renderSystem;

            FileLocation fl = FileSystem.Instance.Locate("fogofwar.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSystem, fl);

            fl = FileSystem.Instance.Locate("fogofwar.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSystem, fl);


        }




        protected override int begin()
        {
            if (mode == RenderMode.DeferredNormal)
            {
                
            }
            else
            {
                renderSystem.BindShader(vtxShader);
                renderSystem.BindShader(pixShader);
            }


            return 1;
        }

        protected override void end()
        {
            renderSystem.BindShader((VertexShader)null);
            renderSystem.BindShader((PixelShader)null);
        }

        public override void BeginPass(int passId)
        {
        }

        public override void EndPass()
        {
        }

        public override bool SupportsMode(RenderMode mode)
        {
            switch (mode)
            {
                case RenderMode.Depth:
                    return false;
                case RenderMode.Final:
                    return true;
            }
            return false;
        }

        public override void Setup(Material mat, ref RenderOperation op)
        {
            if (mode == RenderMode.DeferredNormal)
            {
               
            }
            else
            {
                Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;

                vtxShader.SetValue("mvp", ref mvp);

              
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
                    if (clrTex == null)
                    {
                        
                    }
                    else
                    {
                        pixShader.SetTexture("texDif", clrTex);
                    }



                }

            }
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}
