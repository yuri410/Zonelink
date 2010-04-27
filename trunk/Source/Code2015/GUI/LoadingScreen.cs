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
        Texture lds_ball;

        Texture background;
        Texture progressBarImp;
        Texture progressBarCmp;

        GameFont font;

        int curIndex;
        float displayCD;

        public float Progress
        {
            get;
            set;
        }

        public LoadingScreen(RenderSystem rs)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("lds_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("lds_prgcmp.tex", GameFileLocs.GUI);
            progressBarCmp = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("lds_prgimp.tex", GameFileLocs.GUI);
            progressBarImp = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("lds_ball.tex", GameFileLocs.GUI);
            lds_ball = UITextureManager.Instance.CreateInstance(fl);

            font = GameFontManager.Instance.F18;
        }

        public override void Render(Sprite sprite)
        {
          
            sprite.Draw(background, 0, 0, ColorValue.White);


            if (displayCD < ChangeTime)
            {
                font.DrawString(sprite, LoadingMessages[curIndex], -(int)displayCD * 2, 0, ColorValue.White);
                font.DrawString(sprite, LoadingMessages[(curIndex + 1) % LoadingMessages.Length], (int)displayCD * 2, 0, ColorValue.White);
            }
            else 
            {
                font.DrawString(sprite, LoadingMessages[curIndex], 0, 0, ColorValue.White);
            }

            sprite.Draw(progressBarImp, 15, 692, ColorValue.White);


            Rectangle srect = new Rectangle(0, 0, (int)(progressBarCmp.Width * Progress), progressBarCmp.Height);
            Rectangle drect = new Rectangle(15, 692, srect.Width, progressBarCmp.Height);


            sprite.Draw(progressBarCmp, drect, srect, ColorValue.White);
            int x = srect.Width + 15 - 60;
            ColorValue c = ColorValue.White;
            c.A = 189;
            sprite.Draw(lds_ball, x, 657, c);
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
                lds_ball.Dispose();

                background.Dispose();
                progressBarImp.Dispose();

                progressBarCmp.Dispose();
            }
            progressBarCmp = null;
            progressBarImp = null;
            background = null;

            renderSys = null;
            lds_ball = null;
        }

        #endregion
    }
}
