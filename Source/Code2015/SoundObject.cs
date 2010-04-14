using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MXF = Microsoft.Xna.Framework;

namespace Code2015
{
    /// <summary>
    ///  提供SndObject所需信息
    /// </summary>
    public interface ISoundObjectParent
    {
        bool IsLocalHumanPlayer { get; }
    }


    public abstract class SoundObject : IUpdatable
    {
        ISoundObjectParent parent;

        AudioEmitter emitter;


        protected float radius;

        protected SoundEffectGame soundEffectGame;
        protected SoundEffectInstance currentInstance;
        protected Vector3 cameraPosition;
        Vector3 position;

        public float Radius 
        {
            get { return radius; }
        }
        public AudioEmitter Emitter
        {
            get { return emitter; }
        }
        public ISoundObjectParent Parent
        {
            get { return parent; }
        }

        protected SoundObject(ISoundObjectParent parent, SoundEffectGame sfx)
        {
            this.parent = parent;
            this.soundEffectGame = sfx;
            this.emitter = new AudioEmitter();
            this.Volume = sfx.Volume;
        }

        public float Volume
        {
            get;
            protected set;
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        #region IUpdatable 成员

        public virtual void Update(GameTime dt)
        {
            cameraPosition = SoundManager.Instance.ListenerPosition;

            //float distance = Vector3.Distance(this.Position, cameraPosition);

            //float vol = MathEx.LinearInterpose(minVolume, maxVolume, distance / radius);
            //Volume = vol;

            if (currentInstance == null || currentInstance.State == SoundState.Stopped)
            {
                currentInstance = null;
            }
            if (currentInstance != null)
            {
                soundEffectGame.Update(this, currentInstance);
            }
           
            emitter.Position = new Microsoft.Xna.Framework.Vector3(position.X, position.Y, position.Z);
        }

        #endregion
    }

    public class NormalSoundObject : SoundObject
    {
        FastList<SoundEffectInstance> instance = new FastList<SoundEffectInstance>();

        public NormalSoundObject(SoundManager sm, ISoundObjectParent parent, SoundEffectGame sfx)
            : base(parent, sfx)
        {
            this.radius = 1;
        }

        public void Fire()
        {
            instance.Add(soundEffectGame.Play(SoundEffectPart.Default, this));
        }
        public void Fire(float volume)
        {
            Volume = volume;

            instance.Add(soundEffectGame.Play(SoundEffectPart.Default, this));
        }

        public override void Update(GameTime dt)
        {
            for (int i = 0; i < instance.Count; i++)
            {
                if (instance[i].State == SoundState.Stopped)
                {
                    instance.RemoveAt(i);
                }
                else
                {
                    soundEffectGame.Update(this, instance[i]);
                }
            }
        }
    }
    public class Normal3DSoundObject : SoundObject
    {
        FastList<SoundEffectInstance> instance = new FastList<SoundEffectInstance>();

        public Normal3DSoundObject(SoundManager sm, ISoundObjectParent parent, SoundEffectGame sfx, float radius)
            : base(parent, sfx)
        {
            this.radius = radius;
        }


        public void Fire()
        {
            instance.Add(soundEffectGame.Play(SoundEffectPart.Default, this));
        }
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            for (int i = 0; i < instance.Count; i++)
            {
                if (instance[i].State == SoundState.Stopped)
                {
                    instance.RemoveAt(i);
                }
                else 
                {
                    soundEffectGame.Update(this, instance[i]);
                }
            }
        }
    }
    public class AmbientSoundObject : SoundObject
    {

        public AmbientSoundObject(SoundManager sm, ISoundObjectParent parent, SoundEffectGame sfx, float radius)
            : base(parent, sfx)
        {
            this.radius = radius;
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            float distance = Vector3.Distance(this.Position, cameraPosition);

            if (distance <= radius)
            {
                if (currentInstance == null)
                {
                    currentInstance = soundEffectGame.Play(SoundEffectPart.Default, this);
                }
            }
        }
    }

    public class AmbientLoopSoundObject : SoundObject
    {
        struct PlayOp
        {
            public SoundEffectPart Part;
        }

        float lastDistance;

        FastList<PlayOp> playList = new FastList<PlayOp>();


        public AmbientLoopSoundObject(SoundManager sm, ISoundObjectParent parent, SoundEffectGame sfx, float radius)
            : base(parent, sfx)
        {
            this.lastDistance = float.MaxValue;

            this.radius = radius;
        }


        void AddToPlayList(SoundEffectPart category)
        {
            PlayOp op;
            op.Part = category;
            playList.Add(ref op);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            float distance = Vector3.Distance(this.Position, cameraPosition);
            if (distance < float.Epsilon)
                return;

            bool action = false;

            float r = radius * 2;
            if (distance <= r)
            {
                if (lastDistance > r)
                {
                    AddToPlayList(SoundEffectPart.Fadein);
                    action = true;
                    // just in
                }
            }
            else
            {
                if (lastDistance <= r)
                {
                    AddToPlayList(SoundEffectPart.Fadeout);
                    action = true;
                    // just out
                }
            }

            if (!action && distance <= r)
            {
                if (playList.Count == 0)
                {
                    AddToPlayList(SoundEffectPart.Medium);
                }
            }


            if (currentInstance == null)
            {
                if (playList.Count > 0)
                {
                    currentInstance = soundEffectGame.Play(playList[0].Part, this);

                    playList.RemoveAt(0);
                }
            }

            lastDistance = distance;
        }
    }
}
