using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    class ParticleModifier
    {
        public void Update(Particle[] particles, int count, float dt)
        {
            for (int i = 0; i < count; i++) 
            {
                particles[i].Update(dt);
                particles[i].Life -= dt;
                particles[i].Alpha = MathEx.Saturate(particles[i].Life);
            }
        }
    }
}
