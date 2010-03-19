using System;
using System.Collections.Generic;
using System.Text;
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
            return ParticleManager.Instance.CreateParticle();
        }

        public void Update(FastList<Particle> particles, float dt)
        {
            int count = (int)(creationSpeed * dt);

            for (int i = 0; i < particles.Count && count > 0; i++)
            {
                particles[i].ApplyMoment(new Vector3(0, -0.5f, 0));

                if (particles[i].Life <= 0)
                {
                    particles[i] = CreateParticle();
                    count--;
                }
            }
        }
    }
}
