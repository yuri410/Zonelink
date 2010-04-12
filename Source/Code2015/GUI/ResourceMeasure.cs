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
