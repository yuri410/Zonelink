using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.Effects
{
    public class TerrainEffect513Factory : EffectFactory
    {
        static readonly string typeName = "Terrain513";


        public static string Name
        {
            get { return typeName; }
        }

        RenderSystem renderSystem;

        public TerrainEffect513Factory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new TerrainEffect513(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }
    public class TerrainEffect129Factory : EffectFactory
    {
        static readonly string typeName = "Terrain129";


        public static string Name
        {
            get { return typeName; }
        }

        RenderSystem renderSystem;

        public TerrainEffect129Factory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new TerrainEffect129(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }
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

    class TerrainEffect513 : TerrainEffect
    {
        public TerrainEffect513(RenderSystem renderSystem)
            : base(renderSystem, 513)
        {
        }
    }
    class TerrainEffect129 : TerrainEffect
    {
        public TerrainEffect129(RenderSystem renderSystem)
            : base(renderSystem, 129)
        {
        }
    }
    class TerrainEffect33 : TerrainEffect
    {
        public TerrainEffect33(RenderSystem renderSystem)
            : base(renderSystem, 33)
        {
        }
    }

    class TerrainEffect : Effect
    {
        RenderSystem renderSystem;


        PixelShader pixShader;
        VertexShader vtxShader;
        int terrSize;

        public TerrainEffect(RenderSystem renderSystem, int ts)
            : base(false, TerrainEffect513Factory.Name)
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
            renderSystem.BindShader(vtxShader);
            renderSystem.BindShader(pixShader);
            vtxShader.SetValue("terrSize", (float)terrSize);

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

        public override void BeginShadowPass()
        {
            throw new NotImplementedException();
        }

        public override void EndShadowPass()
        {
            throw new NotImplementedException();
        }

        public override void Setup(Material mat, ref RenderOperation op)
        {
            Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;

            vtxShader.SetValue("mvp", ref mvp);

            Matrix invWorld;
            Matrix.Invert(ref op.Transformation, out invWorld);
            vtxShader.SetValue("invWorld", ref invWorld);

            ShaderSamplerState state = new ShaderSamplerState();
            state.AddressU = TextureAddressMode.Wrap;
            state.AddressV = TextureAddressMode.Wrap;
            state.AddressW = TextureAddressMode.Wrap;
            state.MinFilter = TextureFilter.Anisotropic;
            state.MagFilter = TextureFilter.Anisotropic;
            state.MipFilter = TextureFilter.Anisotropic;
            state.MaxAnisotropy = 8;
            //state.MipMapLODBias = -2;

            pixShader.SetSamplerState("texDif", ref state);
            pixShader.SetTexture("texDif", mat.GetTexture(0));

            pixShader.SetTexture("texColor", TerrainMaterialLibrary.Instance.GlobalColorTexture);
            pixShader.SetSamplerState("texColor", ref state);


            TerrainTexture tex;
            tex = TerrainMaterialLibrary.Instance.GetTexture("Snow0041_5");
            pixShader.SetTextureDirect(0, tex.Texture);
            pixShader.SetSamplerStateDirect(0, ref state);
            tex = TerrainMaterialLibrary.Instance.GetTexture("Grass0027_13");
            pixShader.SetTextureDirect(1, tex.Texture);
            pixShader.SetSamplerStateDirect(1, ref state);
            tex = TerrainMaterialLibrary.Instance.GetTexture("Sand0068_2");
            pixShader.SetTextureDirect(2, tex.Texture);
            pixShader.SetSamplerStateDirect(2, ref state);
            tex = TerrainMaterialLibrary.Instance.GetTexture("RockLayered0023_2");
            pixShader.SetTextureDirect(3, tex.Texture);
            pixShader.SetSamplerStateDirect(3, ref state);


            state.AddressU = TextureAddressMode.Clamp;
            state.AddressV = TextureAddressMode.Clamp;
            state.AddressW = TextureAddressMode.Clamp;
            pixShader.SetSamplerState("texNrm", ref state);
            pixShader.SetTexture("texNrm", mat.GetTexture(1));

        }

        public override void SetupShadowPass(Material mat, ref RenderOperation op)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}
