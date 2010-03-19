using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;
using Apoc3D.Collections;

namespace Code2015.ParticleSystem
{
    class ParticleModifier
    {
        public void Update(FastList<Particle> particles, float dt)
        {
            for (int i = 0; i < particles.Count; i++) 
            {
                particles[i].Update(dt);
                particles[i].Life -= dt;
                particles[i].Alpha = MathEx.Saturate(particles[i].Life);
            }
        }
    }
}
