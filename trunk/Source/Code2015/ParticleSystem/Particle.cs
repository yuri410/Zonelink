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
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    public class Particle
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

            //if (Life < 0)
            //{
            //    ParticleManager.Instance.Retire(this);
            //}
        }
    }
}
