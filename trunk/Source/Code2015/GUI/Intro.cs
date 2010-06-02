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

