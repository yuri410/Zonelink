using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;

namespace Code2015.World.Screen
{
    class ScreenPhysicsObject
    {
        float elasity;
        float friction;

        float linearDamp;
        float angularDamp;

        public float Elasity
        {
            get { return elasity; }
            set { elasity = value; }
        }

        public float Friction
        {
            get { return friction; }
            set { friction = value; }
        }
        public float LinearDamp 
        {
            get { return linearDamp; }
            set { linearDamp = value; }
        }

        public float AngularDamp
        {
            get { return angularDamp; }
            set { angularDamp = value; }
        }
    }

    class ScreenRigidBody
    {
        float orientation;

        Vector2 position;
        Vector2 velocity;
        float angularVel;

        float mass;
        float inertia;

        float radius;

        public float Radius
        {
            get { return radius; }
            set 
            {
                radius = value;
                inertia = 4 * mass * radius * radius;
            }
        }
        public float Mass
        {
            get { return mass; }
            set
            {
                mass = value;
                inertia = 4 * mass * radius * radius;
            }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public float AngularVelocity
        {
            get { return angularVel; }
            set { angularVel = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        public void Update(GameTime time)
        {
            float dt = time.ElapsedRealTime;

            orientation += angularVel * dt;

            position += velocity * dt;
        }

        public void ApplyImpulse(Vector2 im, Vector2 pos)
        {
            velocity += im / mass;

            Vector2 r = pos - position;
            angularVel += MathEx.Vec2Cross(r, im) / inertia;
        }

        public void AppluCentralImpulse(Vector2 im)
        {
            velocity += im / mass;
        }
    }
}
