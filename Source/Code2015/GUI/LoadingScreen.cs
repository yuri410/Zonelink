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
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;


namespace Code2015.GUI
{
    class LoadingScreen : UIComponent, IDisposable
    {
        const float ChangeTime = 0.5f;

        const float DisplayTime = 2;
        static string[] LoadingMessages =
        {
            "LOADING CITY NETWORK",
            "SEARCHING AVAILABLE RESOURCES",
            "RESEARCHING STARVING PEOPLE",
            "ANALYSING HEALTH CARE LEVEL",
            "TESTING AIR POLUTION",
            "CALCULATING CITY'S MAJOR PROBLEM"
        };

        

        RenderSystem renderSys;
        Menu parent;
        Texture background;
        Texture rotico;

        Texture[] progressBar;

        GameFont font;

        int curIndex;
        float displayCD;

        public float Progress
        {
            get;
            set;
        }

        public LoadingScreen(Menu parent, RenderSystem rs)
        {
            this.renderSys = rs;
            this.parent = parent;

            FileLocation fl = FileSystem.Instance.Locate("mm_start_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);
           

            progressBar = new Texture[14];
            for (int i = 0; i < 14; i++)
            {
                fl = FileSystem.Instance.Locate("lds_prg" + i.ToString() + ".tex", GameFileLocs.GUI);
                progressBar[i] = UITextureManager.Instance.CreateInstance(fl);
            }

            font = GameFontManager.Instance.F18;

            fl = FileSystem.Instance.Locate("goal1half_0.tex", GameFileLocs.GUI);
            rotico = UITextureManager.Instance.CreateInstance(fl);

        }

        public override void Render(Sprite sprite)
        {
          
            sprite.Draw(background, 0, 0, ColorValue.White);

            Size size = font.MeasureString(LoadingMessages[curIndex]);
            if (displayCD < ChangeTime)
            {
                const int MoveRange = 350;
                float rate = MathEx.Saturate((ChangeTime - displayCD) * 2);

                ColorValue color1 = ColorValue.White;
                ColorValue color2 = ColorValue.White;
                color1.A = (byte)(rate * byte.MaxValue);
                color2.A = (byte)((1 - rate) * byte.MaxValue);

                string nextMsg = LoadingMessages[(curIndex + 1) % LoadingMessages.Length];
                Size size2 = font.MeasureString(nextMsg);

                font.DrawString(sprite, LoadingMessages[curIndex], -(int)(rate * MoveRange) + (Program.ScreenWidth - size.Width) / 2, 25, color2);
                font.DrawString(sprite, nextMsg, (int)((1 - rate) * MoveRange) + (Program.ScreenWidth - size2.Width) / 2, 25, color1);
            }
            else
            {
                font.DrawString(sprite, LoadingMessages[curIndex], (Program.ScreenWidth - size.Width) / 2, 25, ColorValue.White);
            }

            sprite.Draw(parent.Earth, 0, 0, ColorValue.White);




            float p = Progress * (progressBar.Length - 1);
            int index = (int)Math.Truncate (p);


            if (index < progressBar.Length - 2)
            {
                ColorValue color = ColorValue.White;
                int alpha = (int)(MathEx.Saturate(p - index) * byte.MaxValue);
                color.A = (byte)alpha;
                sprite.Draw(progressBar[index + 1], 360, 101, color);

                color = ColorValue.White;
                alpha = (int)((1 - MathEx.Saturate(p - index)) * byte.MaxValue);
                color.A = (byte)alpha;
                sprite.Draw(progressBar[index], 360, 101, color);
            }
            else
            {
                sprite.Draw(progressBar[index], 360, 101, ColorValue.White);
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            displayCD -= time.ElapsedGameTimeSeconds;
            if (displayCD < 0)
            {
                displayCD = DisplayTime;
                curIndex++;
                if (curIndex >= LoadingMessages.Length)
                    curIndex = 0;
            }
        }

        #region IDisposable 成员

        //public bool Disposed
        //{
        //    get;
        //    private set;
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //lds_ball.Dispose();

                background.Dispose();
                //progressBarImp.Dispose();

                //progressBarCmp.Dispose();
            }
            //progressBarCmp = null;
            //progressBarImp = null;
            background = null;

            renderSys = null;
            //lds_ball = null;
        }

        #endregion
    }
}
