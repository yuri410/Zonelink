using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Code2015
{
    public class SoundObject
    {
        SoundEffectGame soundEffectGame;
        ContentManager contentManager;

        public SoundEffectGame SoundEffectGame
        {
            get;
            set;
        }
        public SoundObject()
        { 
        
        }
        public SoundObject(ContentManager cm,string name)
        {
            contentManager = cm;
            
        }
    }
}
