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
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;

namespace Code2015.ParticleSystem
{
    class SmokeEffect : ParticleEffect
    {
        public SmokeEffect(RenderSystem rs)
                : base(rs, 300)
            {
                Material.CullMode = CullMode.None;
                Material.ZEnabled = true;
                Material.ZWriteEnabled = false;
                Material.PriorityHint = RenderPriority.Third;
                Material.IsTransparent = true;

                //Material.Flags = MaterialFlags.BlendBright;
                Material.Ambient = new Color4F(1, 0.4f, 0.4f, 0.4f);
                Material.Diffuse = new Color4F(1, 1f, 1, 1);


                string file = "smoke_01.tex";
                
                FileLocation fl = FileSystem.Instance.Locate(file, GameFileLocs.Texture);
                Material.SetTexture(0, TextureManager.Instance.CreateInstance(fl));
                Material.SetEffect(EffectManager.Instance.GetModelEffect(SmokeRDEffectFactory.Name));

                BoundingSphere.Radius = float.MaxValue;

                ParticleSize = 10f;
                Material.ZEnabled = true;
                Material.ZWriteEnabled = false;
            }
    }

    class SmokeModifier : ParticleModifier
    {
        public override void Update(FastList<Particle> particles, float dt)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                //Vector3 dir = new Vector3(
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale,
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale,
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale);
                
                particles[i].Velocity *= 0.999f;
                particles[i].Life -= dt;
                particles[i].Alpha = MathEx.Saturate(particles[i].Life * 0.33f);
                //Vector3 dir = particles[i].Velocity;
                //dir.Normalize();
                //particles[i].Velocity = Vector3.Zero;
                //particles[i].Velocity -= direction * (100 * dt);
                //particles[i].Velocity += tangent * (100 * dt);



                particles[i].Update(dt);
            }
        }
    }

    class SmokeEmitter : ParticleEmitter
    {
        Vector3 currentPosition;
        Vector3 up;
        Vector3 right;
        Vector3 front;

        public Vector3 Up 
        {
            get { return up; }
            set { up = value; }
        }
        public Vector3 Right
        {
            get { return right; }
            set { right = value; }
        }
        public Vector3 Front 
        {
            get { return front; }
            set { front = value; }
        }

        public Vector3 Position
        {
            get { return currentPosition; }
            set{currentPosition = value;}
        }

        Particle CreateParticle()
        {
            Particle p = new Particle();

            return CreateParticle(p);
        }

        Particle CreateParticle(Particle p)
        {
            const float MinHorizontalVelocity = -10;
            const float MaxHorizontalVelocity = -40;

            const float MinVerticalVelocity = 25;
            const float MaxVerticalVelocity = 50;

            //const float EndVelocity = 0.75f;

            //const float MinStartSize = 5;
            //const float MaxStartSize = 10;

            //const float MinEndSize = 50;
            //const float MaxEndSize = 200;


            float hoz = MinHorizontalVelocity + (MaxHorizontalVelocity - MinHorizontalVelocity) * Randomizer.GetRandomSingle();
            float hoz2 = MinHorizontalVelocity + (MaxHorizontalVelocity - MinHorizontalVelocity) * Randomizer.GetRandomSingle();
            float vert = MinVerticalVelocity + (MaxVerticalVelocity - MinVerticalVelocity) * Randomizer.GetRandomSingle();

            p.Life = 0.6f * (4 + Randomizer.GetRandomSingle());
            p.Alpha = 1;
            p.Position = currentPosition;
            p.Velocity = hoz * right + hoz2 * front + vert * up;           

            return p;
        }

        public SmokeEmitter()
            : base(1)
        {

        }
        public override void Update(FastList<Particle> particles, float dt)
        {

            int count = (int)(60 * CreationSpeed * dt);

            for (int i = 0; i < particles.Count && count > 0; i++)
            {
                //particles[i].ApplyMoment(new Vector3(0, 0.8f, 0));

                if (particles[i].Life <= float.Epsilon)
                {
                    particles[i] = CreateParticle(particles[i]);
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
