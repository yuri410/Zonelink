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
            : base(rs, 60)
        {
            ParticleSize = 20f;
            Material.ZEnabled = false;
            Material.ZWriteEnabled = false;
        }


    }
    class TransferModifier : ParticleModifier
    {
        //public TransferModifier(TransferEmitter emitter)
        //{
        //    this.emitter = emitter;
        //}

        public override void Update(FastList<Particle> particles, float dt)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                //Vector3 dir = new Vector3(
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale,
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale,
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale);

                
                particles[i].Life -= dt;
                particles[i].Alpha = MathEx.Saturate(particles[i].Life * 5);
                //Vector3 dir = particles[i].Velocity;
                //dir.Normalize();
                //particles[i].Velocity = Vector3.Zero;
                //particles[i].Velocity -= direction * (100 * dt);
                //particles[i].Velocity += tangent * (100 * dt);



                particles[i].Update(dt);
            }
        }
    }
    class TransferEmitter : ParticleEmitter
    {
        Vector3 targetPosition;

        Vector3 srcPosition;
        float distance;

        Vector3 currentPosition;
        Vector3 velocity;
        Vector3 noise;

        Vector3 direction;
        Vector3 tangent;

        public bool IsVisible
        {
            get;
            set;
        }

        public TransferEmitter(Vector3 pos, Vector3 targetPos, Vector3 tangent)
            : base(3)
        {
            this.srcPosition = pos;
            this.targetPosition = targetPos;
            distance = Vector3.Distance(ref srcPosition, ref targetPosition);

            this.currentPosition = pos;
            this.direction = targetPosition - srcPosition;
            this.direction.Normalize();
            this.tangent = tangent;
            Reset();
        }

        void Reset()
        {
            currentPosition = srcPosition;
            noise = Vector3.Zero;
        }

        Particle CreateParticle()
        {
            Particle p = new Particle();

            return CreateParticle(p);
        }

        Particle CreateParticle(Particle p)
        {
            p.Life = .2f;
            p.Alpha = 1;
            p.Position = currentPosition;
            p.Velocity = velocity * 0.5f;

            return p;
        }


        public override void Update(FastList<Particle> particles, float dt)
        {
            Vector3 dist = targetPosition - currentPosition;


            float currentDist = Vector3.Dot(ref direction, ref dist);
            float distPer = MathEx.Saturate(currentDist / distance);

            if (IsVisible && currentDist < 0)
            {
                Reset();
                //particles.FastClear();
                return;
            }

            dist.Normalize();

            noise += new Vector3(
                (Randomizer.GetRandomSingle() - 0.5f) * 25,
                (Randomizer.GetRandomSingle() - 0.5f) * 25,
                (Randomizer.GetRandomSingle() - 0.5f) * 25);
            velocity = 900 * direction;
            velocity += noise * dt;
            velocity -= tangent * (300 * (float)Math.Cos(distPer * Math.PI));


            currentPosition += velocity * dt;


            

            int count = (int)(60 * CreationSpeed * dt);

            for (int i = 0; i < particles.Count && count > 0; i++)
            {
                //particles[i].ApplyMoment(new Vector3(0, 0.8f, 0));

                if (particles[i].Life <= float.Epsilon)
                {
                    particles[i] = CreateParticle(particles[i]);
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
