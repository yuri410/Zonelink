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
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    /// <summary>
    ///  负责回收和创建粒子
    /// </summary>
    public class ParticleEmitter
    {
        float creationSpeed;

        public ParticleEmitter(float speed)
        {
            this.creationSpeed = speed;
        }

        public float CreationSpeed
        {
            get { return creationSpeed; }
        }

        Particle CreateParticle(Particle p)
        {
            p.Life = 5;
            p.Alpha = 1;
            p.Position = new Vector3();
            p.Velocity = new Vector3(
                (Randomizer.GetRandomSingle() - 0.5f) * 8,
                (Randomizer.GetRandomSingle() - 0.5f) * 8,
                (Randomizer.GetRandomSingle() - 0.5f) * 8);
            p.Velocity += new Vector3(18, 18, 0);

            return p;
        }
        Particle CreateParticle()
        {
            Particle p = new Particle();// ParticleManager.Instance.CreateParticle();
            return CreateParticle(p);
        }

        public virtual void Update(FastList<Particle> particles, float dt)
        {
            int count = (int)(60 * creationSpeed * dt);

            for (int i = 0; i < particles.Count && count > 0; i++)
            {
                //particles[i].ApplyMoment(new Vector3(0, 0.8f, 0));

                if (particles[i].Life <= float.Epsilon)
                {
                    particles[i] = CreateParticle();
                    count--;
                }
            }
            if (count > 0)
            {
                count = Math.Min(particles.Elements.Length - particles.Count, count);

                for (int i = 0; i < count; i++)
                {
                    particles.Add(CreateParticle());
                }
            }
        }
    }
}
