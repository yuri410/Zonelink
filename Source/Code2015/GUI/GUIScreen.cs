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
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;

namespace Code2015.GUI
{
    class GUIScreen : UIComponent
    {
        FastList<UIComponent> subElements = new FastList<UIComponent>();

        protected void AddElement(UIComponent con)
        {
            subElements.Add(con);
        }


        public override bool HitTest(int x, int y)
        {
            return base.HitTest(x, y);
        }

        public override void Render(Sprite sprite)
        {
            for (int i = subElements.Count - 1; i >= 0; i--)
            {
                subElements[i].Render(sprite);

                sprite.SetTransform(Matrix.Identity);
            }
        }

        int Comparision(UIComponent a, UIComponent b) 
        {
            return b.Order.CompareTo(a.Order);
        }

        public override void Update(GameTime time)
        {
            subElements.Sort(Comparision);

            bool canInteract = true;
            for (int i = 0; i < subElements.Count; i++)
            {
                subElements[i].Update(time);

                if (canInteract && subElements[i].HitTest(MouseInput.X, MouseInput.Y))
                {
                    subElements[i].UpdateInteract(time);
                    canInteract = false;
                }
            }

        }
    }
}
