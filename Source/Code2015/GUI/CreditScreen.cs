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
using Apoc3D;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.GUI
{
    class CreditScreen : UIComponent
    {
        RenderSystem renderSys;
        Menu parent;

        Texture cursor;
        Texture bg;
        Texture list;
        Point mousePosition;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;
        float roll;
        public CreditScreen(RenderSystem rs, Menu parent)
        {
            this.renderSys = rs;
            this.parent = parent;

            FileLocation fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("crd_bg.tex", GameFileLocs.GUI);
            bg = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("crd_list.tex", GameFileLocs.GUI);
            list = UITextureManager.Instance.CreateInstance(fl);




            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);
        }

        public override void Render(Sprite sprite)
        {

            //parent.RenderLogo(sprite);
            sprite.Draw(bg, 0, 0, ColorValue.White);

            int panelHeight = 538;
            int h = (int)(roll * (list.Height + panelHeight));

            if (h < panelHeight)
            {
                Rectangle drect = new Rectangle(658, 46 + panelHeight - h, list.Width, h);
                Rectangle srect = new Rectangle(0, 0, list.Width, h);
                sprite.Draw(list, drect, srect, ColorValue.White);
            }
            else if (h > list.Height - panelHeight)
            {
                Rectangle drect = new Rectangle(658, 46, list.Width, list.Height * 2 - h + panelHeight);
                Rectangle srect = new Rectangle(0, h - panelHeight, list.Width, list.Height * 2 - h + panelHeight);
                sprite.Draw(list, drect, srect, ColorValue.White);
            }
            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
        }
        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;
            if (MouseInput.IsMouseUpLeft) 
            {
                parent.CurrentScreen = null;
            }

            roll += 0.1f * time.ElapsedGameTimeSeconds;
            if (roll > 1)
                roll = 0;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
