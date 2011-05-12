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
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Apoc3D.Graphics.Effects;
using Code2015.EngineEx;
using Code2015.Effects;

namespace Code2015.ParticleSystem
{
    class ResourceEffect : ParticleEffect
    {
        public ResourceEffect(RenderSystem rs, TransferType type)
            : base(rs, 60)
        {
            Material.CullMode = CullMode.None;
            Material.ZEnabled = false;
            Material.ZWriteEnabled = false;
            Material.PriorityHint = RenderPriority.Third;
            Material.IsTransparent = true;

            //Material.Flags = MaterialFlags.BlendBright;



            string file = "link_p_def.tex";
            switch (type) 
            {
                case TransferType.Food:
                    file = "link_p_yellow.tex";
                    break;
                case TransferType.Oil:
                    file = "link_p_red.tex";
                    break;
                case TransferType.Wood:
                    file = "link_p_green.tex";
                    break;
            }
            FileLocation fl = FileSystem.Instance.Locate(file, GameFileLocs.Texture);
            Material.SetTexture(0, TextureManager.Instance.CreateInstance(fl));
            Material.SetEffect(EffectManager.Instance.GetModelEffect(ParticleRDEffectFactory.Name));

            BoundingSphere.Radius = float.MaxValue;

            ParticleSize = 12;
            Material.ZEnabled = false;
            Material.ZWriteEnabled = false;
        }


    }
    class ResourceModifier : ParticleModifier
    {
        //public TransferModifier(TransferEmitter emitter)
        //{
        //    this.emitter = emitter;
        //}

        public override void Update(FastList<Particle> particles, float dt)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                //Vector3 dir = new Vector3(
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale,
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale,
                //      (Randomizer.GetRandomSingle() - 0.5f) * NoiseScale);

                
                particles[i].Life -= dt;
                particles[i].Alpha = MathEx.Saturate(particles[i].Life * 3);
                //Vector3 dir = particles[i].Velocity;
                //dir.Normalize();
                //particles[i].Velocity = Vector3.Zero;
                //particles[i].Velocity -= direction * (100 * dt);
                //particles[i].Velocity += tangent * (100 * dt);



                particles[i].Update(dt);
            }
        }
    }
    class ResourceEmitter : ParticleEmitter
    {
        Vector3 tanx;
        Vector3 tany;
        Vector3 centerPos;
        float radius;
        float angle;

        Vector3 currentPosition;
        float speedMod;


        public bool IsVisible
        {
            get;
            set;
        }

        public ResourceEmitter(Vector3 pos, Vector3 tanx, Vector3 tany, float r)
            : base(3)
        {
            this.tanx = tanx;
            this.tany = tany;
            this.centerPos = pos;
            this.radius = r;
            this.angle = Randomizer.GetRandomSingle() * MathEx.PIf * 2;

            speedMod = (Randomizer.GetRandomSingle() * 0.5f + 0.75f);
            Reset();
        }

        void Reset()
        {
            currentPosition = centerPos + (float)Math.Sin(angle) * tany + (float)Math.Cos(angle) * tanx;

            //noise = Vector3.Zero;
        }

        Particle CreateParticle()
        {
            Particle p = new Particle();

            return CreateParticle(p);
        }

        Particle CreateParticle(Particle p)
        {
            p.Life = .2f;
            p.Alpha = 0.6f;
            p.Position = currentPosition;
            //p.Velocity = velocity * 0.5f;

            return p;
        }


        public override void Update(FastList<Particle> particles, float dt)
        {
            //Vector3 dist = targetPosition - currentPosition;


            //float currentDist = Vector3.Dot(ref direction, ref dist);
            //float distPer = MathEx.Saturate(currentDist / distance);

            //if (!IsVisible)
            //{
            //    return;
            //}

            //if (IsVisible)
            //{
            //    blend += dt;
            //}
            //else 
            //{
            //    blend -= dt;
            //}
            //    isShutDown = false;


            angle += dt * 1.3f * speedMod;
            currentPosition = centerPos + radius * (((float)Math.Sin(-angle) * tany + (float)Math.Cos(-angle) * tanx) * MathEx.Root2 * 0.5f);


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
