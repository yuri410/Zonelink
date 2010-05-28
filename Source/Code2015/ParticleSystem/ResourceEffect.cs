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
            Material.ZEnabled = true;
            Material.ZWriteEnabled = false;
            Material.PriorityHint = RenderPriority.Third;
            Material.IsTransparent = true;

            //Material.Flags = MaterialFlags.BlendBright;
            Material.Ambient = new Color4F(0.5f, 0.4f, 0.4f, 0.4f);
            Material.Diffuse = new Color4F(0.5f, 1f, 1, 1);


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

            ParticleSize = 4;
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
    class ResourceEmitter : ParticleEmitter
    {
        Vector3 tanx;
        Vector3 tany;
        Vector3 centerPos;
        float radius;
        float angle;
        float blend;
        Vector3 currentPosition;



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
            p.Alpha = 1;
            p.Position = currentPosition;
            //p.Velocity = velocity * 0.5f;

            return p;
        }


        public override void Update(FastList<Particle> particles, float dt)
        {
            //Vector3 dist = targetPosition - currentPosition;


            //float currentDist = Vector3.Dot(ref direction, ref dist);
            //float distPer = MathEx.Saturate(currentDist / distance);

            if (!IsVisible && blend <= 0)
            {
                return;
            }

            if (IsVisible)
            {
                blend += dt;
            }
            else 
            {
                blend -= dt;
            }
            //    isShutDown = false;

            //if (currentDist < 0 && !isShutDown)
            //{
            //    if (!IsVisible)
            //    {
            //        isShutDown = true;
            //    }
            //    Reset();
            //    return;
            //}

            //if (isShutDown) 
            //{
            //    return;
            //}

            //dist.Normalize();

            //noise += new Vector3(
            //    (Randomizer.GetRandomSingle() - 0.5f) * 50,
            //    (Randomizer.GetRandomSingle() - 0.5f) * 50,
            //    (Randomizer.GetRandomSingle() - 0.5f) * 50);
            //velocity = 900 * direction;
            //velocity += noise;
            //velocity -= tangent * (300 * (float)Math.Cos(distPer * Math.PI));

            angle += dt;
            currentPosition = centerPos + (float)Math.Sin(angle) * tany + (float)Math.Cos(angle) * tanx;


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
