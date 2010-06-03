/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;

namespace Code2015.World.Screen
{
    public class ScreenPhysicsObject
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

    public class ScreenRigidBody : ScreenPhysicsObject
    {
        float orientation;

        Vector2 position;
        Vector2 velocity;
        Vector2 force;
        float angularVel;

        float mass;
        float inertia;

        float radius;

        public ScreenRigidBody()
        {
            CollisionEnabled = true;

        }

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
        public bool CollisionEnabled
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
            im *= 0.1f;
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
