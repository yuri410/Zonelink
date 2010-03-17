using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    class Particle
    {
        const float mass = 1;

        public Vector3 Position;
        public Vector3 Velocity;
        //public ColorValue Color;
        public float Alpha;
        //public float Size;
        //public float Rotation;
        public float Life;

        public void ApplyMoment(Vector3 m)
        {
            Velocity += m / mass;
        }

        public void Update(float dt)
        {
            Position += Velocity * dt;
        }
    }
}
