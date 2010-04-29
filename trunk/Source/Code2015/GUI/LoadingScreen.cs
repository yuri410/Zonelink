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
        }

        public override void Render(Sprite sprite)
        {
          
            sprite.Draw(background, 0, 0, ColorValue.White);


            if (displayCD < ChangeTime)
            {
                const int MoveRange = 350;
                float rate = MathEx.Saturate((ChangeTime - displayCD) * 2);

                ColorValue color1 = ColorValue.White;
                ColorValue color2 = ColorValue.White;
                color1.A = (byte)(rate * byte.MaxValue);
                color2.A = (byte)((1 - rate) * byte.MaxValue);

                font.DrawString(sprite, LoadingMessages[curIndex], -(int)(rate * MoveRange) + 500, 25, color2);
                font.DrawString(sprite, LoadingMessages[(curIndex + 1) % LoadingMessages.Length], (int)((1 - rate) * MoveRange) + 500, 25, color1);
            }
            else
            {
                font.DrawString(sprite, LoadingMessages[curIndex], 500, 25, ColorValue.White);
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
