using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.Effects
{
    public class TerrainEffectFactory : EffectFactory
    {
        static readonly string typeName = "Terrain";


        public static string Name
        {
            get { return typeName; }
        }



        RenderSystem renderSystem;

        public TerrainEffectFactory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new TerrainEffect(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class TerrainEffect : Effect
    {
        RenderSystem renderSystem;


        PixelShader pixShader;
        VertexShader vtxShader;

        public TerrainEffect(RenderSystem renderSystem)
            : base(false, TerrainEffectFactory.Name)
        {
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

            //ShaderSamplerState state = new ShaderSamplerState();
            //state.AddressU = TextureAddressMode.Mirror;
            //state.AddressV = TextureAddressMode.Mirror;
            //state.AddressW = TextureAddressMode.Mirror;
            //state.MinFilter = TextureFilter.Linear;
            //state.MagFilter = TextureFilter.Linear;
            //state.MipFilter = TextureFilter.Linear;
            //pixShader.SetSamplerState("texDif", ref state);
            pixShader.SetTexture("texDif", mat.GetTexture(0));
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
