using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.GUI
{
    class Intro : UIComponent
    {
        const int FrameCount = 40;
        float delay = 6;

        int currentFrame;
        Texture[] frames = new Texture[FrameCount];
        Texture blackBg;



        public Intro(RenderSystem rs)
        {
            for (int i = 0; i < FrameCount; i++)
            {
                FileLocation fl = FileSystem.Instance.Locate(i.ToString("D2") + ".tex",GameFileLocs.Movie);

                frames[i] = UITextureManager.Instance.CreateInstance(fl);
            }
            FileLocation fl2 = FileSystem.Instance.Locate("bg_black.tex", GameFileLocs.GUI);
            blackBg = UITextureManager.Instance.CreateInstance(fl2);
        }

        public bool IsOver
        {
            get { return currentFrame >= FrameCount - 1 && delay < 0; }
        }

        public override void Render(Sprite sprite)
        {
            currentFrame++;
            if (currentFrame >= frames.Length)
            {
                if (delay > 3)
                {
                    sprite.Draw(frames[frames.Length - 1], 0, 0, ColorValue.White);
                    ColorValue color = ColorValue.White;
                    color.A = (byte)(byte.MaxValue * (1 - MathEx.Saturate((delay - 3) / 3)));
                    sprite.Draw(blackBg, 0, 0, color);
                }
                else
                {
                    ColorValue color = ColorValue.White;
                    color.A = (byte)(byte.MaxValue * MathEx.Saturate(delay / 3));
                    sprite.Draw(blackBg, 0, 0, color);
                }
                

                delay -= 0.04f;
            }
            else
            {
                sprite.Draw(frames[currentFrame], 0, 0, ColorValue.White);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                for (int i = 0; i < FrameCount; i++) 
                {
                    frames[i].Dispose();
                }
                frames = null;
            }

        }

    }
}

