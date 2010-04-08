using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Code2015
{
    public class SoundEffectGame
    {
        AudioEmitter emitter;
        AudioListener listener;
        ContentManager contentManager;
        SoundEffect effect;
        ContentManager cm;

        public AudioEmitter Emitter
        {
            get { return emitter; }
            set { emitter = value; }
        }

        public AudioListener Listener
        {
            get { return listener; }
            set { listener = value; }
        }


        public SoundEffectGame(IServiceProvider service, string name)
        {
            emitter = new AudioEmitter();
            listener = new AudioListener();
            cm = new ContentManager(service);
            cm.RootDirectory = "Sounds";
            effect = cm.Load<SoundEffect>(name);
        }
        //private void ReadXML()
        //{
        //    FileLocation fl = FileSystem.Instance.Locate("", GameFileLocs.Config);
        //    Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);
        //    foreach (KeyValuePair<string, ConfigurationSection> c in conf)
        //    {
        //        ConfigurationSection sect = c.Value;
        //        string name = sect.Name;
        //        switch (name)
        //        { 

        //        }
        //    }

        //} 
        public void Play()
        {
            effect.Play();
        }

    }
}
