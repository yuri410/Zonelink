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
using Apoc3D.Vfs;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D;

namespace Code2015.GUI
{
    class LoadingOverlay : UIComponent, IDisposable
    {
        float alpha;
        float dir;
        Texture overlay34;
        Code2015 parent; 
        float perdelay;
        public LoadingOverlay(Code2015 parent, Texture overlay34)
        {
            this.parent = parent;
            this.overlay34 = overlay34;
        }

        public void In() { dir = 1; }
        public void Out(int preDelay)
        {
            dir = -1; this.perdelay = preDelay;
        }
        public override void Render(Sprite sprite)
        {
            ColorValue color = ColorValue.White;
            color.A = (byte)(byte.MaxValue * MathEx.Saturate(alpha));
            sprite.Draw(overlay34, 0, 0, color);
        }

        public override void Update(GameTime time)
        {
            

            if (perdelay > 0)
            {
                perdelay -= time.ElapsedGameTimeSeconds;
                return;
            }

            if (parent.IsIngame && parent.CurrentGame.IsLoaded && alpha > 0)
            {
                Out(0);
            }

            alpha += dir * time.ElapsedGameTimeSeconds;
            
            if (alpha > 1)
            {
                alpha = 1;
                dir = 0;
            }
            else if (alpha < 0)
            {
                alpha = 0;
                dir = 0;
            }
        }
    }
}
