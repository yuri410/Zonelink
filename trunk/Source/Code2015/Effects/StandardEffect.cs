using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics;

namespace Code2015.Effects
{
    public class StandardEffectFactory : EffectFactory
    {
        static readonly string typeName = "Standard";

        public static string Name
        {
            get { return typeName; }
        }


        RenderSystem renderSystem;

        public StandardEffectFactory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new WaterEffect(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }
    class StandardEffect : Effect
    {
        public StandardEffect()
            : base(false, StandardEffectFactory.Name)
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

        public override void Setup(Material mat, ref RenderOperation op)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
