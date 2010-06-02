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
        //protected Vector3 cameraPosition;
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
            //cameraPosition = SoundManager.Instance.ListenerPosition;

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
           
            emitter.Position = new MXF.Vector3(position.X, position.Y, position.Z);
            //emitter.Forward = MXF.Vector3.Normalize(emitter.Position);
           
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
            float distance = Vector3.Distance(this.Position, SoundManager.Instance.ListenerPosition);
            if (distance < radius * 3)
            {
                instance.Add(soundEffectGame.Play(SoundEffectPart.Default, this));
            }
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
        public float Probability
        {
            get;
            set;
        }
        public AmbientSoundObject(SoundManager sm, ISoundObjectParent parent, SoundEffectGame sfx, float radius)
            : base(parent, sfx)
        {
            this.radius = radius;
            this.Probability = sfx.Probability;
        }

        public override void Update(GameTime dt)
        {
            if (Probability < 1)
            {
                float rnd = Randomizer.GetRandomSingle();
                if (rnd > Probability)
                {
                    return;
                }
            }

            base.Update(dt);

            float distance = Vector3.Distance(this.Position, SoundManager.Instance.ListenerPosition);

            if (distance <= radius * 3)
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

            float distance = Vector3.Distance(this.Position, SoundManager.Instance.ListenerPosition);
            if (distance < float.Epsilon)
                return;

            bool action = false;

            float r = radius * 4;
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
