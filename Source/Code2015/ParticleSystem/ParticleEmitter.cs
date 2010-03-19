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
    class ParticleEmitter
    {
        ParticleManager pmgr;
        float creationSpeed;

        public ParticleEmitter(ParticleManager pm, float speed)
        {
            this.pmgr = pm;
            this.creationSpeed = speed;
        }

        public float CreationSpeed
        {
            get { return creationSpeed; }
        }

        Particle CreateParticle()
        {
            Particle p = ParticleManager.Instance.CreateParticle();
            p.Life = 5;
            p.Alpha = 1;
            p.Position = new Vector3();
            p.Velocity = new Vector3(Randomizer.GetRandomSingle() * 3, Randomizer.GetRandomSingle() * 3, Randomizer.GetRandomSingle() * 3);
            return p;
        }

        public void Update(FastList<Particle> particles, float dt)
        {
            int count = (int)(creationSpeed * dt);

            for (int i = 0; i < particles.Count && count > 0; i++)
            {
                particles[i].ApplyMoment(new Vector3(0, -0.5f, 0));

                if (particles[i].Life <= float.Epsilon)
                {
                    particles[i] = CreateParticle();
                    count--;
                }
            }
        }
    }
}
