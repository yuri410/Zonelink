using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.MathLib;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MXF = Microsoft.Xna.Framework;

namespace Code2015
{
    public enum SoundEffectPart
    {
        Default = 0,
        Fadein = 0,
        Medium = 1,
        Fadeout
    }
    public class SoundEffectGame
    {
        SoundEffect[][] soundEffects;
        AudioListener listener;

        public float Volume
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }
        public SoundType Type
        {
            get;
            private set;
        }
        public SoundControl Control
        {
            get;
            private set;
        }
        public bool IsLooped
        {
            get { return (Control & SoundControl.Loop) == SoundControl.Loop; }
        }

        public float Probability
        {
            get;
            set;
        }
        ContentManager contentManager;

        public SoundEffectGame(AudioListener listener, SoundManager sm, ConfigurationSection sect)
        {
            this.contentManager = sm.Content;
            this.listener = listener;


            string[] control = sect.GetStringArray("Control");
            for (int i = 0; i < control.Length; i++)
            {
                control[i] = control[i].ToLowerInvariant();
                switch (control[i])
                {
                    case "loop":
                        Control |= SoundControl.Loop;
                        break;
                    case "random":
                        Control |= SoundControl.Random;
                        break;
                    case "ambient":
                        Control |= SoundControl.Ambient;
                        break;
                }
            }
            Probability = sect.GetSingle("Probability", 1);
            Volume = sect.GetSingle("Volume", 1);
            Type = (SoundType)Enum.Parse(typeof(SoundType), sect.GetString("Type", SoundType.Normal.ToString()), true);
            //this.soundEffect = contentManager.Load<SoundEffect>(sect.Name);
            soundEffects = new SoundEffect[3][];

            if ((Control & SoundControl.Loop) == SoundControl.Loop)
            {
                string[] fadein = sect.GetStringArray("Fadein");
                soundEffects[(int)SoundEffectPart.Fadein] = new SoundEffect[fadein.Length];
                for (int index = 0; index < fadein.Length; index++)
                {
                    soundEffects[(int)SoundEffectPart.Fadein][0] = contentManager.Load<SoundEffect>(fadein[index]);
                }

                string[] mediumstring = sect.GetStringArray("Medium");
                soundEffects[(int)SoundEffectPart.Medium] = new SoundEffect[mediumstring.Length];
                for (int index = 0; index < mediumstring.Length; index++)
                {
                    soundEffects[(int)SoundEffectPart.Medium][index] = contentManager.Load<SoundEffect>(mediumstring[index]);
                }

                string[] fadeout = sect.GetStringArray("Fadeout");
                soundEffects[(int)SoundEffectPart.Fadeout] = new SoundEffect[fadeout.Length];
                for (int index = 0; index < fadein.Length; index++)
                {
                    soundEffects[(int)SoundEffectPart.Fadeout][0] = contentManager.Load<SoundEffect>(fadeout[index]);
                }
            }
            else
            {
                string[] sounds = sect.GetStringArray("Sounds");
                soundEffects[(int)SoundEffectPart.Fadein] = new SoundEffect[sounds.Length];
                for (int i = 0; i < sounds.Length; i++)
                {
                    soundEffects[0][i] = contentManager.Load<SoundEffect>(sounds[i]);
                }
                soundEffects[1] = soundEffects[0];
                soundEffects[2] = soundEffects[0];
            }
        }

        public void Update(SoundObject soundObj, SoundEffectInstance inst)
        {
            inst.Volume = soundObj.Volume;

            if (Type == SoundType.World || Type == SoundType.Global)
            {
                SoundEffect.DistanceScale = soundObj.Radius;
                inst.Apply3D(listener, soundObj.Emitter);
            }
        }

        public SoundEffectInstance Play(SoundEffectPart category, SoundObject soundObj)
        {
            switch (Type)
            {
                case SoundType.Normal:
                    return Play(category, soundObj.Volume);
                case SoundType.Player:
                    if (soundObj.Parent.IsLocalHumanPlayer)
                    {
                        return Play(category, soundObj.Volume);
                    }
                    return null;
                case SoundType.Global:
                case SoundType.World:
                    return Play3D(category, soundObj.Emitter);
            }
            throw new NotSupportedException();
        }

        //播放声音
        public SoundEffectInstance Play(SoundEffectPart category)
        {

            int idx = 0;
            if ((Control & SoundControl.Random) == SoundControl.Random)
            {
                idx = Randomizer.GetRandomInt(soundEffects[(int)category].Length);
            }
            return soundEffects[(int)category][idx].Play(Volume);

            //return soundEffect.Play();
        }
        public SoundEffectInstance Play(SoundEffectPart category, float volume)
        {

            int idx = 0;
            if ((Control & SoundControl.Random) == SoundControl.Random)
            {
                idx = Randomizer.GetRandomInt(soundEffects[(int)category].Length);
            }
            return soundEffects[(int)category][idx].Play(volume * Volume);

        }
        //pitch,pan（-1左声道，1右声道）默认值设为0；
        public SoundEffectInstance Play(SoundEffectPart category, float volume, float pitch, float pan, bool isLoop)
        {

            int idx = 0;
            if ((Control & SoundControl.Random) == SoundControl.Random)
            {
                idx = Randomizer.GetRandomInt(soundEffects[(int)category].Length);
            }
            return soundEffects[(int)category][idx].Play(volume * Volume, pitch, pan, isLoop);

        }
        //播放3D声音


        public SoundEffectInstance Play3D(SoundEffectPart category, AudioEmitter emitter)
        {
            int idx = 0;
            if ((Control & SoundControl.Random) == SoundControl.Random)
            {
                idx = Randomizer.GetRandomInt(soundEffects[(int)category].Length);
            }
            return Play(soundEffects[(int)category][idx], emitter);

        }

        public SoundEffectInstance Play3D(SoundEffectPart category, AudioEmitter emitter, float volume, float pitch, bool loop)
        {
            int idx = 0;
            if ((Control & SoundControl.Random) == SoundControl.Random)
            {
                idx = Randomizer.GetRandomInt(soundEffects[(int)category].Length);
            }
            return Play(soundEffects[(int)category][idx], emitter, volume * Volume, pitch, loop);
        }


        SoundEffectInstance Play(SoundEffect sf, AudioEmitter emitter)
        {
            unsafe
            {
                //MXF.Vector3* tempPointer1 = (MXF.Vector3*)(&listener);
                //this.Listener.Position = *tempPointer1;

                //MXF.Vector3* tempPointer2 = (MXF.Vector3*)(&emitter);
                //this.Emitter.Position = *tempPointer2;

                return sf.Play3D(listener, emitter);
            }
        }

        SoundEffectInstance Play(SoundEffect sf, AudioEmitter emitter, float volume, float pitch, bool loop)
        {
            unsafe
            {
                //MXF.Vector3* tempPointer1 = (MXF.Vector3*)(&listener);
                //this.Listener.Position = *tempPointer1;

                //MXF.Vector3* tempPointer2 = (MXF.Vector3*)(&emitter);
                //this.Emitter.Position = *tempPointer2;
                
                //this.Listener.Position = *tempPointer1;
                //this.Emitter.Position = *tempPointer2;
                return sf.Play3D(listener, emitter, volume * Volume, pitch, loop);
            }
        }



    }
}
