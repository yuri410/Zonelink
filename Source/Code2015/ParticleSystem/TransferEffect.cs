using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    class TransferEffect : ParticleEffect
    {
        public TransferEffect(RenderSystem rs)
            : base(rs, 10)
        {
        }


    }
    class TransferModifier : ParticleModifier
    {
        Vector3 targetPosition;

        public float NoiseScale = 1;

        public Vector3 TargetPosition
        {
            get { return targetPosition; }
            set { targetPosition = value; }
        }

        public TransferModifier(Vector3 targetPos) 
        {
            TargetPosition = targetPos;
        }

        public override void Update(FastList<Particle> particles, float dt)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Vector3 dir = new Vector3();
                      //(Randomizer.GetRandomSingle() - 0.5f) * NoiseScale,
                      //(Randomizer.GetRandomSingle() - 0.5f) * NoiseScale,
                      //(Randomizer.GetRandomSingle() - 0.5f) * NoiseScale);

                Vector3 dist = targetPosition - particles[i].Position;
                particles[i].Life = dist.LengthSquared() < 100 ? -1 : 1;

                dir += dist;

                dir.Normalize();

                particles[i].Velocity = dir;
                particles[i].Update(dt);
            }
        }
    }
    class TransferEmitter : ParticleEmitter 
    {

        Vector3 srcPosition;


        public TransferEmitter(Vector3 pos)
            : base(10)
        {
            srcPosition = pos;
        }



        Particle CreateParticle()
        {
            Particle p = ParticleManager.Instance.CreateParticle();
            p.Life = 100;
            p.Alpha = 1;
            p.Position = srcPosition;

            return p;
        }


        public override void Update(FastList<Particle> particles, float dt)
        {
            int count = (int)(60 * CreationSpeed * dt);

            for (int i = 0; i < particles.Count && count > 0; i++)
            {
                //particles[i].ApplyMoment(new Vector3(0, 0.8f, 0));
                
                if (particles[i].Life <= float.Epsilon)
                {
                    particles[i] = CreateParticle();
                    count--;
                }
            }
            if (count > 0)
            {
                count = Math.Min(particles.Elements.Length - particles.Count, count);

                for (int i = 0; i < count; i++)
                {
                    particles.Add(CreateParticle());
                }
            }
        }
    }
}
