using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Code2015
{
    public class SoundEffectGame
    {
        SoundEffect soundEffect;
        public AudioListener Listener
        {
            get;
            set;
        }
        public AudioEmitter Emitter
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public SoundEffectGame(ContentManager cm,string name)
        {
            soundEffect = cm.Load<SoundEffect>(name);
            this.Name = name;
            this.Listener = new AudioListener();
            this.Emitter = new AudioEmitter();
        }

        //播放声音
        public SoundEffectInstance Play()
        {
            return soundEffect.Play();
        }
        //播放3D声音
        public SoundEffectInstance Play(Vector3 listener, Vector3 emitter)
        {
            this.Listener.Position = listener;
            this.Emitter.Position = emitter;
            return soundEffect.Play3D(this.Listener, this.Emitter);
        }
        public SoundEffectInstance Play(Vector3 listener, Vector3 emitter, float volume, float pitch, bool loop)
        {
            this.Listener.Position = listener;           
            this.Emitter.Position = emitter;
            return soundEffect.Play3D(this.Listener, this.Emitter, volume, pitch, loop);
        }


    }
}
