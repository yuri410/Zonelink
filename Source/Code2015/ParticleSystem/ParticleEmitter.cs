using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    /// <summary>
    ///  负责回收和创建粒子
    /// </summary>
    public class ParticleEmitter
    {
        float creationSpeed;

        public ParticleEmitter(float speed)
        {
            this.creationSpeed = speed;
        }

        public float CreationSpeed
        {
            get { return creationSpeed; }
        }

        Particle CreateParticle()
        {
            Particle p = ParticleManager.Instance.CreateParticle();
            p.Life = 9;
            p.Alpha = 1;
            p.Position = new Vector3();
            p.Velocity = new Vector3(
                (Randomizer.GetRandomSingle() - 0.5f) * 8,
                (Randomizer.GetRandomSingle() - 0.5f) * 8,
                (Randomizer.GetRandomSingle() - 0.5f) * 8);
            p.Velocity += new Vector3(18, 18, 0);

            return p;
        }

        public void Update(FastList<Particle> particles, float dt)
        {
            int count = (int)(60 * creationSpeed * dt);

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
