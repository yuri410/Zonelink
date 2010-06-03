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
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.MathLib;

namespace Apoc3D.GUI.Controls
{
    public class Button : TextControl
    {
        public Button()
        {

        }

        protected override bool IsInBounds(int x, int y)
        {
            Rectangle rect = new Rectangle(X, Y, Width, Height);
            return IsInBounds(x, y, ref rect);
        }

        public bool IsValid
        {
            get;
            set;
        }

        public Texture ImageDisabled
        {
            get;
            set;
        }
        public Texture ImageInvalid
        {
            get;
            set;
        }
        public Texture ImageMouseDown
        {
            get;
            set;
        }
        public Texture ImageMouseOver
        {
            get;
            set;
        }
        public Texture Image
        {
            get;
            set;
        }

        
        public override void Render(Sprite sprite)
        {
            //spr.Transform = Matrix.Translation((int)(bounds.X + (bounds.Width - lblWidth) * 0.5f), (int)(bounds.Y + (bounds.Height - lblHeight) * 0.5f), 0);
            if (IsValid)
            {
                if (Enabled)
                {
                    if (IsPressed)
                    {
                        if (ImageMouseDown != null)
                            sprite.Draw(ImageMouseDown, X, Y, modColor);
                    }
                    else if (IsMouseOver)
                    {
                        if (ImageMouseOver != null)
                            sprite.Draw(ImageMouseOver, X, Y, modColor);
                    }
                    else
                    {
                        if (Image != null)
                            sprite.Draw(Image, X, Y, modColor);
                    }
                }
                else
                {
                    if (ImageDisabled != null)
                        sprite.Draw(ImageDisabled, X, Y, modColor);
                }
            }
            else
            {
                if (ImageInvalid != null)
                    sprite.Draw(ImageInvalid, X, Y, modColor);
            }

            //if (IsPressed)
            //{
            //    Sprite.Transform = Matrix.Translation(X + 1, Y + 1, 0);
            //}
            base.Render(sprite);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (disposing)
            //{
                //if (Image != null)
                //    Image.Dispose();
                //if (ImageDisabled != null)
                //    ImageDisabled.Dispose();
                //if (ImageInvalid != null)
                //    ImageInvalid.Dispose();
                //if (ImageMouseDown != null)
                //    ImageMouseDown.Dispose();
                //if (ImageMouseOver != null)
                //    ImageMouseOver.Dispose();
            //}
            Image = null;
            ImageDisabled = null;
            ImageInvalid = null;
            ImageMouseDown = null;
            ImageMouseOver = null;
        }

    }
}
