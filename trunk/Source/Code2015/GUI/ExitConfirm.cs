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
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.GUI
{
    class ExitConfirm : UIComponent
    {
        bool passed = true;

        Texture tex;

        public bool IsShown { get { return !passed; } }
        public void Show()
        {
            passed = false;
        }

        public ExitConfirm()
        {
            FileLocation fl = FileSystem.Instance.Locate("ig_exitconfirm.tex", GameFileLocs.GUI);
            tex = UITextureManager.Instance.CreateInstance(fl);

        }
        public override void Render(Sprite sprite)
        {
            if (!passed)
            {
                sprite.Draw(tex, 0, 0, ColorValue.White);
            }
        }
        public override bool HitTest(int x, int y)
        {
            return !passed;
        }
        public override int Order
        {
            get
            {
                return 105;
            }
        }
        public override void Update(GameTime time)
        {
        }
        public override void UpdateInteract(GameTime time)
        {
            if (MouseInput.IsMouseUpLeft)
            {
                passed = true;
            }
        }
    }
}
