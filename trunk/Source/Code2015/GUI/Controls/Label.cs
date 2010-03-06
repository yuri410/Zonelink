using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;

namespace Apoc3D.GUI.Controls
{
    public class Label : TextControl
    {
        public Label(Apoc3D.Graphics.RenderSystem rs)
            : base(rs)
        {
        }


        public override string ToString()
        {
            return "{ Text= " + Text + " }";
        }

        public override void Render(Sprite sprite)
        {
            //Sprite.Transform = Matrix.Translation(X, Y, 0);
            base.Render(sprite);
        }
    }

    
}
