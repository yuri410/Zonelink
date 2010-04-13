using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Apoc3D.MathLib;
using MXF = Microsoft.Xna.Framework;
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
        ContentManager contentManager;
        public SoundEffectGame(SoundManager  sm,string name)
        {
            contentManager = sm.MakeContentManager();
            soundEffect = contentManager.Load<SoundEffect>(name);
            this.Name = name;
            this.Listener = new AudioListener();
            this.Emitter = new AudioEmitter();
        }

        //播放声音
        public SoundEffectInstance Play()
        {
            return soundEffect.Play();
        }
        public SoundEffectInstance Play(float volume)
        {
            return soundEffect.Play(volume);
        }
        //pitch,pan（-1左声道，1右声道）默认值设为0；
        public SoundEffectInstance Play(float volume,float pitch,float pan,bool isLoop)
        {
            return soundEffect.Play(volume, pitch, pan, isLoop);
        }
        //播放3D声音
       
        public SoundEffectInstance Play(Vector3 listener, Vector3 emitter)
        {
            unsafe
            {
                MXF.Vector3* tempPointer1 = (MXF.Vector3*)(&listener);
                this.Listener.Position = *tempPointer1;

                MXF.Vector3* tempPointer2 = (MXF.Vector3*)(&emitter);
                this.Emitter.Position = *tempPointer2;
               
                return soundEffect.Play3D(this.Listener, this.Emitter);
            }
        }
    
        public SoundEffectInstance Play(Vector3 listener, Vector3 emitter, float volume, float pitch, bool loop)
        {
            unsafe
            {
                MXF.Vector3* tempPointer1 = (MXF.Vector3*)(&listener);
                this.Listener.Position = *tempPointer1;

                MXF.Vector3* tempPointer2 = (MXF.Vector3*)(&emitter);
                this.Emitter.Position = *tempPointer2;

                this.Listener.Position = *tempPointer1;
                this.Emitter.Position = *tempPointer2;
                return soundEffect.Play3D(this.Listener, this.Emitter, volume, pitch, loop);
            }
        }
        
        
        
    }
}
