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
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.GUI
{

    class ResourceMeasure
    {
        ValueSmoother amount = new ValueSmoother(10);
        NaturalResource current;

        public NaturalResource Current
        {
            get { return current; }
            set
            {
                if (!object.ReferenceEquals(current, value))
                {
                    current = value;
                    amount.Clear();
                    if (value != null)
                    {
                        amount.Add(current.CurrentAmount);
                    }
                }
            }
        }

        public void Render(Sprite sprite, Font font)
        {
            if (current != null)
            {
                switch (current.Type)
                {
                    //case NaturalResourceType.Food:
                    //    font.DrawString(sprite, "Farm", 457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                    //    break;
                    case NaturalResourceType.Petro:
                        font.DrawString(sprite, "Oil Field", 457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                        break;
                    case NaturalResourceType.Wood:
                        font.DrawString(sprite, "Forest", 457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                        break;
                }

                font.DrawString(sprite, "Total: " + amount.Result.ToString(),
                              470, 635, 13, DrawTextFormat.Left, (int)ColorValue.Black.PackedValue);
            }
        }

        public void Update(GameTime time)
        {
            if (current != null)
            {
                amount.Add(current.CurrentAmount);


            }
        }
    }
}
