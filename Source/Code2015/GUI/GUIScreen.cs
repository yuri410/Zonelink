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
