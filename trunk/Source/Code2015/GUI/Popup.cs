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
using Apoc3D.MathLib;

namespace Code2015.GUI
{
    class Popup : UIComponent
    {
        const float YShift = 100;
        int x;
        int y;


        float time;
        float duration;
        Texture texture;

        public Popup(RenderSystem rs, Texture tex, int x, int y, float duration)
        {
            this.x = x;
            this.y = y;
            this.texture = tex;
            this.duration = duration;
        }

        public bool IsFinished
        {
            get { return time > duration; }
        }


        public override void Render(Sprite sprite)
        {
            float alpha = 1 - MathEx.Saturate(time / duration);
            int ny = (int)(y - (1 - alpha) * YShift);

            ColorValue modClr = new ColorValue(1, 1, 1, alpha);
            //ColorValue modClr2 = new ColorValue(0, 0, 0, alpha);
            sprite.Draw(texture, x, ny, modClr);
            //font.DrawString(sprite, text, x + 1, ny + 1, 15, DrawTextFormat.Center | DrawTextFormat.VerticalCenter, (int)modClr2.PackedValue);
            //font.DrawString(sprite, text, x, ny, 15, DrawTextFormat.Center | DrawTextFormat.VerticalCenter, (int)modClr.PackedValue);
        }
        public override void Update(GameTime time)
        {
            this.time += time.ElapsedGameTimeSeconds;
        }

    }
    class Popup2 : UIComponent
    {
        const float YShift = 100;
        int x;
        int y;


        float time;
        float duration;
        Texture texture;

        public Popup2(RenderSystem rs, Texture tex, int x, int y, float duration)
        {
            this.x = x;
            this.y = y;
            this.texture = tex;
            this.duration = duration;
        }

        public bool IsFinished
        {
            get { return time > duration; }
        }


        public override void Render(Sprite sprite)
        {
            float d = 3 * time / duration;

            d = d - (float)Math.Truncate(d);

            float alpha = MathEx.Saturate(1 - d);


            ColorValue modClr = new ColorValue(1, 1, 1, alpha);

            float scale = 5 - 4 * alpha;
            sprite.SetTransform(Matrix.Translation(-texture.Width / 2, -texture.Height / 2, 0) * Matrix.Scaling(scale, scale, scale) * Matrix.Translation(x, y, 0));
            sprite.Draw(texture, 0, 0, modClr);
            sprite.SetTransform(Matrix.Identity);
        }
        public override void Update(GameTime time)
        {
            this.time += time.ElapsedGameTimeSeconds;
        }

    }
}
