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
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;

namespace Code2015.GUI.Controls
{
    enum ControlDirection 
    {
        Hoz,
        Vertical
    }

    class ProgressBar : Control
    {
        public int LeftPadding
        {
            get;
            set;
        }
        public int RightPadding
        {
            get;
            set;
        }
        public float Value
        {
            get;
            set;
        }
        public float MediumValue
        {
            get;
            set;
        }
        public ControlDirection Direction
        {
            get;
            set;
        }
        public Texture Medium
        {
            get;
            set;
        }
        public Texture Background
        {
            get;
            set;
        }
        public Texture ProgressImage
        {
            get;
            set;
        }

        public override void Render(Sprite sprite)
        {
            if (Background != null)
            {
                sprite.Draw(Background, X, Y, modColor);
            }

            if (Medium != null)
            {
                if (Direction == ControlDirection.Hoz)
                {
                    Rectangle rect = new Rectangle(X + LeftPadding, Y, Width - LeftPadding - RightPadding, Height);

                    rect.Width = (int)(rect.Width * MediumValue);
                    Rectangle srect = new Rectangle(LeftPadding, 0, rect.Width, Height);

                    sprite.Draw(Medium, rect, srect, modColor);
                }
            }

            if (ProgressImage != null)
            {
                if (Direction == ControlDirection.Hoz)
                {

                    Rectangle rect = new Rectangle(X + LeftPadding, Y, Width - LeftPadding - RightPadding, Height);

                    rect.Width = (int)(rect.Width * Value);
                    Rectangle srect = new Rectangle(LeftPadding, 0, rect.Width, Height);

                    sprite.Draw(ProgressImage, rect, srect, modColor);
                }
                else
                {
                    Rectangle rect = new Rectangle(X, Y, Width, Height);
                    rect.Height = (int)(rect.Height * Value);
                    Rectangle srect = new Rectangle(0, 0, Width, rect.Height);

                    sprite.Draw(ProgressImage, rect, srect, modColor);
                }
            }

            base.Render(sprite);
        }

        //public override void Update(GameTime dt)
        //{

        //    base.Update(dt);
        //}
    }
}
