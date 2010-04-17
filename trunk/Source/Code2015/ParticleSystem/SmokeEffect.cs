using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;

namespace Code2015.ParticleSystem
{
    class SmokeEffect : ParticleEffect
    {
        public SmokeEffect(RenderSystem rs)
                : base(rs, 60)
            {
                Material.CullMode = CullMode.None;
                Material.ZEnabled = true;
                Material.ZWriteEnabled = false;
                Material.PriorityHint = RenderPriority.Third;
                Material.IsTransparent = true;

                //Material.Flags = MaterialFlags.BlendBright;
                Material.Ambient = new Color4F(1, 0.4f, 0.4f, 0.4f);
                Material.Diffuse = new Color4F(1, 1f, 1, 1);


                string file = "smoke_01.tex";
                
                FileLocation fl = FileSystem.Instance.Locate(file, GameFileLocs.Texture);
                Material.SetTexture(0, TextureManager.Instance.CreateInstance(fl));
                Material.SetEffect(EffectManager.Instance.GetModelEffect(ParticleRDEffectFactory.Name));

                BoundingSphere.Radius = float.MaxValue;

                ParticleSize = 20f;
                Material.ZEnabled = false;
                Material.ZWriteEnabled = false;
            }
    }

    class SmokeModifier : ParticleModifier
    {
        public override void Update(FastList<Particle> particles, float dt)
        {
            base.Update(particles, dt);
        }
    }

    class SmokeEmitter : ParticleEmitter
    {
        public SmokeEmitter()
            : base(10)
        {

        }
        public override void Update(FastList<Particle> particles, float dt)
        {
            base.Update(particles, dt);
        }
    }
}
