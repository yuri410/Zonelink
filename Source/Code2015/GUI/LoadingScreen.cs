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
        RenderSystem renderSys;
        Texture lds_ball;

        Texture background;
        Texture progressBarImp;
        Texture progressBarCmp;

        Font font;

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
           
            font = FontManager.Instance.GetFont("default");
        }

        public override void Render(Sprite sprite)
        {
          
            sprite.Draw(background, 0, 0, ColorValue.White);

            font.DrawString(sprite, "Loading", 0, 0, 34, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);


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
        }

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                renderSys = null;
                lds_ball.Dispose();

                lds_ball = null;
                background.Dispose();
                background = null;
                progressBarImp.Dispose();
                progressBarImp = null;

                progressBarCmp.Dispose();
                progressBarCmp = null;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion
    }
}
