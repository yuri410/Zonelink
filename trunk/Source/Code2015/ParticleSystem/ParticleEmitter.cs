using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    /// <summary>
    ///  负责回收和创建粒子
    /// </summary>
    class ParticleEmitter
    {
        ParticleManager pmgr;
        int currentParticleCount;

        public ParticleEmitter(ParticleManager pm)
        {
            this.pmgr = pm;
        }

        public int CurrentCount
        {
            get { return currentParticleCount; }
        }

        Particle CreateParticle() 
        {
            return ParticleManager.Instance.CreateParticle();
        }



        public void Update(Particle[] particles, float dt)
        {
            for (int i = 0; i < currentParticleCount; i++)
            {
                particles[i].ApplyMoment(new Vector3(0, -0.5f, 0));

                if (particles[i].Life <= 0)
                {
                    ParticleManager.Instance.Retire(particles[i]);
                    particles[i] = CreateParticle();
                }
            }
        }
    }
}
