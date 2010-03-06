
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Code2015.GUI;

namespace Apoc3D.GUI.Controls
{
    public class ImageControl : Control
    {
        Texture image;


        public ImageControl()
        {
        }

        public Texture Image
        {
            get { return image; }
            set { image = value; }
        }

        public override void Update(GameTime dt)
        {                     
            base.Update(dt);
        }

        public override void Render(Sprite sprite)
        {
            //Sprite.Transform = Matrix.Translation(X, Y, 0);
            base.Render(sprite);

            if (image != null)
            {
                sprite.Draw(image, X, Y, modColor);
            }
        }


        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                //UITextureManager.Instance.DestoryInstance(image);

            }

            image = null;
        }

    }
}
