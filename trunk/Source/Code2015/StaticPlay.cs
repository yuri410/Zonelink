using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Code2015
{
    
    public static class StaticPlay
    {
        enum State
        {
            stoped,playing, fadein, fadeout
        }
        
        const float standard = 0.3f;

        static ContentManager cm;
        //static Song start;
        static Song background;
        //static Song end;
        //static Song next;

        static float current;
        static State currentstate;
        public static void Init(IServiceProvider service)
        {
            cm = new ContentManager(service, "Sounds");
            //start = cm.Load<Song>("Parpollon2");
            background = cm.Load<Song>("The islands");
            //end = cm.Load<Song>("end");

            MediaPlayer.Volume = standard;
        }
        public static void PlayStart()
        {
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = true;

            MediaPlayer.Play(background);
            //next = start;
            //currentstate = State.fadeout;
        }
        //public static void PlayBackGround()
        //{
        //    MediaPlayer.Stop();
        //    MediaPlayer.IsRepeating = true;
        //    MediaPlayer.Play(background);
            
        //    //next = background;
        //    //currentstate = State.fadeout;
        //}

        //public static void PlayEnd()
        //{
        //    next = end;
        //    currentstate = State.fadeout;
        //}
        //public static void Stop()
        //{
        //    next = null;
        //    currentstate = State.fadeout;
        //}

        //public static void Update(GameTime time)
        //{
        //    switch (currentstate)
        //    { 
        //        case State.stoped:
        //            if (next != null)
        //            {
        //                MediaPlayer.Play(next);
        //                currentstate = State.fadein;
        //                next = null;
        //            }
        //            break;
        //        case State.fadein:
                    
        //            current += 0.05f;

        //            if (current > standard)
        //            {
        //                current = standard;
        //                currentstate = State.playing;
        //            }
        //            break;
        //        case State.fadeout:
        //            current -= 0.05f;
        //            if (current <= 0)
        //            {
        //                current = 0;
        //                currentstate = State.stoped;
        //            }
        //            break;
        //        case State.playing:
                   
        //            break;

        //    }
        //}
    }
}
