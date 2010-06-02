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

namespace Apoc3D.GUI.Controls
{
    public abstract class TextControl : Control
    {
        protected TextControl()
        {
            Text = string.Empty;
        }

        public Font Font
        {
            get;
            set;
        }

        public float FontSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control should automatically size to its contents.
        /// </summary>
        /// <value><c>true</c> if the control should automatically size to its contents; otherwise, <c>false</c>.</value>
        public bool AutoSize
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }


        public override void Update(GameTime dt)
        {
            // call the base method
            base.Update(dt);

            //if (AutoSize)
            //{
            //    // check if we need to autosize
            //    Size size = GetPreferredSize();
            //    if (size != Size.Empty)
            //    {
            //        // resize
            //        Size = size;
            //        //Width += Padding.Horizontal;
            //        //Height += Padding.Vertical;
            //    }
            //}
        }

        protected void DrawString(Sprite sprite,string str, int x, int y, int w, int h)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (Font != null)
                {
                    Font.DrawString(sprite, str, x, y, FontSize, DrawTextFormat.Center, (int)foreColor.PackedValue);
                }
                //if (d3dFont != null)
                //{
                //    Rectangle rect = new Rectangle(x, y, w, h);

                //    rect.Height -= padding.Vertical;
                //    rect.Width -= padding.Horizontal;
                //    rect.X += padding.Left;
                //    rect.Y += padding.Right;

                //    d3dFont.DrawString(Sprite, str, rect, GetDrawTextFormat(ContentAlign) | DrawTextFormat.WordBreak, foreColor.ToArgb());
                //}
            }
        }

        public override void Render(Sprite sprite)
        {
            base.Render(sprite);

            if (!string.IsNullOrEmpty(Text))
            {
                if (Font != null)
                {
                    Font.DrawString(sprite, Text, X, Y, FontSize, DrawTextFormat.Center, (int)foreColor.PackedValue);
                }
                //if (d3dFont != null)
                //{
                //    Rectangle rect = new Rectangle(0, 0, Width, Height);

                //    rect.Height -= padding.Vertical;
                //    rect.Width -= padding.Horizontal;
                //    rect.X += padding.Left;
                //    rect.Y += padding.Right;
                //    if (AutoSize)
                //    {
                //        d3dFont.DrawString(Sprite, Text, rect, GetDrawTextFormat(ContentAlign) | DrawTextFormat.SingleLine, foreColor.ToArgb());
                //    }
                //    else
                //    {
                //        d3dFont.DrawString(Sprite, Text, rect, GetDrawTextFormat(ContentAlign) | DrawTextFormat.WordBreak, foreColor.ToArgb());
                //    }
                //}
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
        }

    }
}
