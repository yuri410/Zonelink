using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;

namespace Code2015.GUI.Controls
{
    class ProgressBar : Control
    {

        public float Value
        {
            get;
            set;
        }

        public int PaddingX
        {
            get;
            set;
        }
        public int PaddingY
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
                sprite.Draw(Background, X, Y, ColorValue.White);
            }

            if (ProgressImage != null)
            {
                Rectangle rect = new Rectangle(X + PaddingX, Y + PaddingY, Width - PaddingX * 2, Height - PaddingY * 2);
                rect.Width = (int)(rect.Width * Value);

                sprite.Draw(ProgressImage, rect, ColorValue.White);
            }


            base.Render(sprite);
        }

        //public override void Update(GameTime dt)
        //{

        //    base.Update(dt);
        //}
    }
}
