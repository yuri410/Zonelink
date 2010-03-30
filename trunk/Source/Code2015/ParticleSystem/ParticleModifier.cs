using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    public class ParticleModifier
    {
        public virtual void Update(FastList<Particle> particles, float dt)
        {
            for (int i = 0; i < particles.Count; i++) 
            {
                particles[i].Update(dt);
                particles[i].Life -= dt;
                particles[i].Alpha = MathEx.Saturate(particles[i].Life / 5f);
            }
        }
    }
}
