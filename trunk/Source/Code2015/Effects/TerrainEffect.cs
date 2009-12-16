using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics;

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

        public TerrainEffect(RenderSystem renderSystem)
            : base(false, TerrainEffectFactory.Name)
        {

        }

        protected override int begin()
        {
            throw new NotImplementedException();
        }

        protected override void end()
        {
            throw new NotImplementedException();
        }

        public override void BeginPass(int passId)
        {
            throw new NotImplementedException();
        }

        public override void EndPass()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void SetupShadowPass(Material mat, ref RenderOperation op)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
