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
    
    class TerrainEffect33 : TerrainEffect
    {
        public TerrainEffect33(RenderSystem renderSystem)
            : base(renderSystem, 33)
        {
        }
    }

    class TerrainEffect : ShadowedEffect
    {
        RenderSystem renderSystem;


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
             
        }

        protected override int begin()
        {
            if (mode == RenderMode.Depth)
            {
                renderSystem.BindShader(shdVtxShader);
                renderSystem.BindShader(shdPixShader);
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


                ShaderSamplerState state = new ShaderSamplerState();
                state.AddressU = TextureAddressMode.Wrap;
                state.AddressV = TextureAddressMode.Wrap;
                state.AddressW = TextureAddressMode.Wrap;
                state.MinFilter = TextureFilter.Anisotropic;
                state.MagFilter = TextureFilter.Anisotropic;
                state.MipFilter = TextureFilter.Linear;
                state.MaxAnisotropy = 8;
                state.MipMapLODBias = -1;


                pixShader.SetTexture("texColor", TerrainMaterialLibrary.Instance.GlobalColorTexture);
                pixShader.SetTexture("texDif", TerrainMaterialLibrary.Instance.GlobalIndexTexture);
                pixShader.SetTexture("texNorm", TerrainMaterialLibrary.Instance.GlobalNormalTexture);

                pixShader.SetSamplerState("texDif", ref state);
                pixShader.SetSamplerState("texColor", ref state);
                pixShader.SetSamplerState("texNorm", ref state);


                TerrainTexture tex;
                tex = TerrainMaterialLibrary.Instance.GetTexture("Snow");
                pixShader.SetTexture("texDet1", tex.Texture);
                pixShader.SetSamplerState("texDet1", ref state);
                tex = TerrainMaterialLibrary.Instance.GetTexture("Grass");
                pixShader.SetTexture("texDet2", tex.Texture);
                pixShader.SetSamplerState("texDet2", ref state);
                tex = TerrainMaterialLibrary.Instance.GetTexture("Sand");
                pixShader.SetTexture("texDet3", tex.Texture);
                pixShader.SetSamplerState("texDet3", ref state);
                tex = TerrainMaterialLibrary.Instance.GetTexture("Rock");
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
                    //pixShader.SetValue("k_s", mat.Specular);
                    //pixShader.SetValue("k_e", mat.Emissive);
                    //pixShader.SetValue("k_power", mat.Power);

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
