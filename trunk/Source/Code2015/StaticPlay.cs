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
        
        const float standard = 0.15f;

        static ContentManager cm;
        //static Song start;
        static Song background;
        //static Song end;
        //static Song next;

        //static float current;
        //static State currentstate;
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
