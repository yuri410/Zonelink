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
    enum TransferType
    {
        Default,
        Wood,
        Food,
        Oil
    }
    class TransferEffect : ParticleEffect
    {
        public TransferEffect(RenderSystem rs, TransferType type)
            : base(rs, 60)
        {
            Material.CullMode = CullMode.None;
            Material.ZEnabled = true;
            Material.ZWriteEnabled = false;
            Material.PriorityHint = RenderPriority.Third;
            Material.IsTransparent = true;

            //Material.Flags = MaterialFlags.BlendBright;
            Material.Ambient = new Color4F(1, 0.4f, 0.4f, 0.4f);
            Material.Diffuse = new Color4F(1, 1f, 1, 1);


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

            ParticleSize = 16f;
            Material.ZEnabled = false;
            Material.ZWriteEnabled = false;
        }


    }
    class TransferModifier : ParticleModifier
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
                particles[i].Alpha = MathEx.Saturate(particles[i].Life * 5);
                //Vector3 dir = particles[i].Velocity;
                //dir.Normalize();
                //particles[i].Velocity = Vector3.Zero;
                //particles[i].Velocity -= direction * (100 * dt);
                //particles[i].Velocity += tangent * (100 * dt);



                particles[i].Update(dt);
            }
        }
    }
    class TransferEmitter : ParticleEmitter
    {
        Vector3 targetPosition;

        Vector3 srcPosition;
        float distance;

        Vector3 currentPosition;
        Vector3 velocity;
        Vector3 noise;

        Vector3 direction;
        Vector3 tangent;

        bool isShutDown;

        public bool IsShutDown 
        {
            get { return isShutDown; }
            set { isShutDown = value; }
        }

        public bool IsVisible
        {
            get;
            set;
        }

        public TransferEmitter(Vector3 pos, Vector3 targetPos, Vector3 tangent)
            : base(3)
        {
            this.srcPosition = pos;
            this.targetPosition = targetPos;
            distance = Vector3.Distance(ref srcPosition, ref targetPosition);

            this.currentPosition = pos;
            this.direction = targetPosition - srcPosition;
            this.direction.Normalize();
            this.tangent = tangent;
            Reset();
        }

        void Reset()
        {
            currentPosition = srcPosition;
            noise = Vector3.Zero;
        }

        Particle CreateParticle()
        {
            Particle p = new Particle();

            return CreateParticle(p);
        }

        Particle CreateParticle(Particle p)
        {
            p.Life = .2f;
            p.Alpha = 1;
            p.Position = currentPosition;
            p.Velocity = velocity * 0.5f;

            return p;
        }


        public override void Update(FastList<Particle> particles, float dt)
        {
            Vector3 dist = targetPosition - currentPosition;


            float currentDist = Vector3.Dot(ref direction, ref dist);
            float distPer = MathEx.Saturate(currentDist / distance);

            if (IsVisible)
                isShutDown = false;            

            if (currentDist < 0 && !isShutDown)
            {
                if (!IsVisible)
                {
                    isShutDown = true;
                }
                Reset();
                return;
            }

            if (isShutDown) 
            {
                return;
            }

            dist.Normalize();

            noise += new Vector3(
                (Randomizer.GetRandomSingle() - 0.5f) * 50,
                (Randomizer.GetRandomSingle() - 0.5f) * 50,
                (Randomizer.GetRandomSingle() - 0.5f) * 50);
            velocity = 900 * direction;
            velocity += noise;
            velocity -= tangent * (300 * (float)Math.Cos(distPer * Math.PI));


            currentPosition += velocity * dt;


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
