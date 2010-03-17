using System;
using System.Collections.Generic;
using System.Text;

namespace Code2015.ParticleSystem
{
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

        public void Update(Particle[] particles, float dt)
        {

        }
    }
}
