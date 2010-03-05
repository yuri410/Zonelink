using System;
using System.Collections.Generic;
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

        public object Tag
        {
            get;
            set;
        }

    }

    class ScreenRigidBody : ScreenPhysicsObject
    {
        float orientation;

        Vector2 position;
        Vector2 velocity;
        Vector2 force;
        float angularVel;

        float mass;
        float inertia;

        float radius;

        public float Inertia
        {
            get { return inertia; }
        }

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
        public bool IsColliding
        {
            get;
            set;
        }

        public Vector2 Force
        {
            get { return force; }
            set { force = value; }
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

        public void Integrate(float dt)
        {
            IsColliding = false;

            orientation += angularVel * dt;

            velocity += force / mass;
            force = new Vector2();

            position += velocity * dt;

            velocity -= velocity * LinearDamp * dt;
            angularVel -= angularVel * AngularDamp * dt;
        }

        public void ApplyImpulse(Vector2 im, Vector2 pos)
        {
            velocity += im / mass;

            Vector2 r = pos - position;
            angularVel -= Vector2.Cross(r, im) / inertia;
        }

        public void AppluCentralImpulse(Vector2 im)
        {
            velocity += im / mass;
        }
    }
}
