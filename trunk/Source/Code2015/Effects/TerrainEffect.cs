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

namespace Code2015.Effects
{
    public class TerrainEffect33Factory : EffectFactory
    {
        static readonly string typeName = "Terrain33";


        public static string Name
        {
            get { return typeName; }
        }

        RenderSystem renderSystem;

        public TerrainEffect33Factory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new TerrainEffect33(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }
    public class TerrainEffect17Factory : EffectFactory
    {
        static readonly string typeName = "Terrain17";


        public static string Name
        {
            get { return typeName; }
        }

        RenderSystem renderSystem;

        public TerrainEffect17Factory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new TerrainEffect17(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }
    
    class TerrainEffect33 : TerrainEffect
    {
        public TerrainEffect33(RenderSystem renderSystem)
            : base(renderSystem, 33)
        {
        }
    }

    class TerrainEffect17 : TerrainEffect
    {
        public TerrainEffect17(RenderSystem renderSystem)
            : base(renderSystem, 17)
        {
        }
    }
    class TerrainEffect : RigidEffect
    {
        public static Texture FogMask;

        RenderSystem renderSystem;

        PixelShader nrmPixShader;
        VertexShader nrmVtxShader;

        PixelShader pixShader;
        VertexShader vtxShader;

        int terrSize;
        bool stateSetted;

        public TerrainEffect(RenderSystem renderSystem, int ts)
            : base(renderSystem, TerrainEffect33Factory.Name, false)
        {
            this.terrSize = ts;
            this.renderSystem = renderSystem;

            FileLocation fl = FileSystem.Instance.Locate("terrain.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSystem, fl);


            fl = FileSystem.Instance.Locate("terrain.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSystem, fl);


            fl = FileSystem.Instance.Locate("terrainNormalGen.cvs", GameFileLocs.Effect);
            nrmVtxShader = LoadVertexShader(renderSystem, fl);

            fl = FileSystem.Instance.Locate("terrainNormalGen.cps", GameFileLocs.Effect);
            nrmPixShader = LoadPixelShader(renderSystem, fl);
        }

        public override bool SupportsMode(RenderMode mode)
        {
            //if (mode == RenderMode.Depth)
                //return false;
            
            return true;
        }

        protected override int begin()
        {
            if (mode == RenderMode.Depth)
            {
                renderSystem.BindShader(shdVtxShader);
                renderSystem.BindShader(shdPixShader);
            }
            else if (mode == RenderMode.DeferredNormal)
            {
                renderSystem.BindShader(nrmVtxShader);
                renderSystem.BindShader(nrmPixShader);


                nrmPixShader.SetTexture("texNorm", MaterialLibrary.Instance.GlobalBakedNormalTexture);
            }
            else
            {
                renderSystem.BindShader(vtxShader);
                renderSystem.BindShader(pixShader);

                vtxShader.SetValue("terrSize", (float)terrSize);
                pixShader.SetValue("i_a", EffectParams.LightAmbient);
                pixShader.SetValue("i_d", EffectParams.LightDiffuse);
                //pixShader.SetValue("i_s", EffectParams.LightSpecular);
                vtxShader.SetValue("lightDir", EffectParams.LightDir);
                //vtxShader.SetValue("viewPos", EffectParams.CurrentCamera.Position);

                pixShader.SetTexture("texShd", EffectParams.DepthMap[0]);
                pixShader.SetTexture("texFog", FogMask);

                ShaderSamplerState state = new ShaderSamplerState();
                state.AddressU = TextureAddressMode.Wrap;
                state.AddressV = TextureAddressMode.Wrap;
                state.AddressW = TextureAddressMode.Wrap;
                state.MinFilter = TextureFilter.Anisotropic;
                state.MagFilter = TextureFilter.Anisotropic;
                state.MipFilter = TextureFilter.Linear;
                state.MaxAnisotropy = 8;
                state.MipMapLODBias = 0;


                ShaderSamplerState state2 = new ShaderSamplerState();
                state2.AddressU = TextureAddressMode.Wrap;
                state2.AddressV = TextureAddressMode.Wrap;
                state2.AddressW = TextureAddressMode.Wrap;
                state2.MinFilter = TextureFilter.Anisotropic;
                state2.MagFilter = TextureFilter.Anisotropic;
                state2.MipFilter = TextureFilter.Linear;
                state2.MaxAnisotropy = 8;
                state2.MipMapLODBias = 0;

                pixShader.SetTexture("texDif", MaterialLibrary.Instance.GlobalIndexTexture);
                pixShader.SetTexture("texNorm", MaterialLibrary.Instance.GlobalNormalTexture);
                //pixShader.SetTexture("texEdge", MaterialLibrary.Instance.FadeEdge);


                pixShader.SetTexture("hatch0", MaterialLibrary.Instance.Hatch0);
                pixShader.SetTexture("hatch1", MaterialLibrary.Instance.Hatch1);
                pixShader.SetSamplerState("hatch0", ref state2);
                pixShader.SetSamplerState("hatch1", ref state2);

                pixShader.SetSamplerState("texDif", ref state);
                pixShader.SetSamplerState("texNorm", ref state);
                pixShader.SetSamplerState("texFog", ref state);

                TerrainTexture tex;
                tex = MaterialLibrary.Instance.GetTexture("Snow");
                pixShader.SetTexture("texDet1", tex.Texture);
                pixShader.SetSamplerState("texDet1", ref state);
                tex = MaterialLibrary.Instance.GetTexture("Grass");
                pixShader.SetTexture("texDet2", tex.Texture);
                pixShader.SetSamplerState("texDet2", ref state);
                tex = MaterialLibrary.Instance.GetTexture("Sand");
                pixShader.SetTexture("texDet3", tex.Texture);
                pixShader.SetSamplerState("texDet3", ref state);
                tex = MaterialLibrary.Instance.GetTexture("Rock");
                pixShader.SetTexture("texDet4", tex.Texture);
                pixShader.SetSamplerState("texDet4", ref state);



                state.AddressU = TextureAddressMode.Clamp;
                state.AddressV = TextureAddressMode.Clamp;
                state.AddressW = TextureAddressMode.Clamp;
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
            }
            stateSetted = false;
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

        //public override void BeginShadowPass()
        //{
        //    throw new NotImplementedException();
        //}

        //public override void EndShadowPass()
        //{
        //    throw new NotImplementedException();
        //}

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
                nrmPixShader.SetValue("view", EffectParams.CurrentCamera.ViewMatrix);

                Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;
                nrmVtxShader.SetValue("mvp", ref mvp);
            }
            else
            {
                Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;

                vtxShader.SetValue("mvp", ref mvp);
                vtxShader.SetValue("world", ref op.Transformation);

                Matrix lightPrjTrans;
                Matrix.Multiply(ref op.Transformation, ref EffectParams.DepthViewProj, out lightPrjTrans);

                vtxShader.SetValue("smTrans", lightPrjTrans);

                if (!stateSetted)
                {
                    pixShader.SetValue("k_a", mat.Ambient);
                    pixShader.SetValue("k_d", mat.Diffuse);

                    stateSetted = true;
                }
            }
        }

        //public override void SetupShadowPass(Material mat, ref RenderOperation op)
        //{
        //    throw new NotImplementedException();
        //}

        protected override void Dispose(bool disposing)
        {

        }
    }
}
