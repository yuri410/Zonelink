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

        public ControlDirection Direction
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
