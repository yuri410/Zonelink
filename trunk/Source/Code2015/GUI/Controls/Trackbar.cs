using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.GUI;

namespace Apoc3D.GUI.Controls
{
    public class Trackbar : Control
    {
        int value;


        //int sLeft;
        //int sRight;

        Label lbl;

        Size sliderSize;
        //Size textBgSize;

        //void ComputeSlideRange()
        //{
        //    sLeft = sliderSize.Width / 2;
        //    sRight = Width - sLeft;

        //}

        //public Texture TextBackGround
        //{
        //    get;
        //    set;
        //}
        public Texture BackgroundImage
        {
            get;
            set;
        }
        public Texture SliderImage
        {
            get;
            set;
        }

        //public Size TextBackSize
        //{
        //    get { return textBgSize; }
        //    set { textBgSize = value; }
        //}

        public Size SliderSize
        {
            get { return sliderSize; }
            set { sliderSize = value; }
        }


        public Trackbar()
        {
            lbl = new Label();
            lbl.AutoSize = false;

            //Rectangle rect = Bounds;
            //rect.Height -= padding.Vertical;
            //rect.Width -= padding.Horizontal;
            //rect.X += padding.Left;
            //rect.Y += padding.Right;
        }

        public int Value
        {
            get { return value; }
            set
            {
                lbl.Text = value.ToString();
                this.value = value;
            }
        }
        public int Step
        {
            get;
            set;
        }
        public int Maximum
        {
            get;
            set;
        }
        public int Minimum
        {
            get;
            set;
        }

        public override void Render(Sprite sprite)
        {
            //sprite.Transform = Matrix.Translation(X, Y, 0);
            sprite.Draw(BackgroundImage, X, Y, modColor);
            base.Render(sprite);

            int sliderX = X + sliderSize.Width / 2 + (int)((2 * Width / 3 - sliderSize.Width) * ((double)value / (double)Maximum));


            //int sliderX = X + padding.Left + sliderSize.Width / 2 + (int)((2 * Width / 3 - padding.Right - sliderSize.Width) * ((double)value / (double)Maximum));
            //sprite.Transform = Matrix.Translation(
            //    sliderX,
            //    Y, 0);
            sprite.Draw(SliderImage, sliderX, Y, modColor);

            //Sprite.Transform = Matrix.Translation(X + Width / 3, Y, 0);

            lbl.Render(sprite);
        }
        public override void Update(GameTime dt)
        {
            base.Update(dt);
            lbl.X = X + 2 * Width / 3;
            lbl.Y = Y;
            lbl.ModulateColor = this.modColor;
        }


        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (BackgroundImage != null)
                {
                    BackgroundImage.Dispose();
                }
                if (SliderImage != null)
                {
                    SliderImage.Dispose();
                }
            }
            BackgroundImage = null;
            SliderImage = null;

        }

        protected override void OnMouseMove(MouseButtonFlags btn)
        {
            base.OnMouseMove(btn);

            if (IsPressed)
            {

            }
        }



        //protected override void Apply()
        //{
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        if (lbl != null)
        //        {
        //            lbl.Dispose();
        //        }

        //        lbl = new Texture(device, 1, 1, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

        //        DataRectangle rect = lbl.LockRectangle(0, LockFlags.None);
        //        rect.Data.Write<uint>(0xff000000);

        //        lbl.UnlockRectangle(0);
        //    }
        //    else
        //        if (useGDIp)
        //        {
        //            TextFormatFlags fmt = CreateTextFormat(RightToLeft.No, contentAlignment, false);
        //            Bitmap tmp = DrawString(text, font, fmt, textBgSize.Width, textBgSize.Height);

        //            if (lbl != null)
        //                lbl.Dispose();

        //            lbl = Utils.Bitmap2Texture(device, tmp, Usage.None, Pool.Managed);// Texture.FromBitmap(d3d, tmp, Usage.None, Pool.Managed);

        //            tmp.Dispose();
        //        }
        //        else
        //        {
        //            StringFormat format = CreateStringFormat(false, RightToLeft.No, contentAlignment, false, useMnemonic);

        //            //SizeF ms = MeasureString(format);
        //            int lblWidth = textBgSize.Width;// ms.Width;
        //            int lblHeight = textBgSize.Height;// ms.Height;

        //            Bitmap tmp = DrawString(text, font, format, lblWidth, lblHeight); // new Bitmap((int)width, (int)height);

        //            if (lbl != null)
        //                lbl.Dispose();

        //            lbl = Utils.Bitmap2Texture(device, tmp, Usage.None, Pool.Managed);// Texture.FromBitmap(d3d, tmp, Usage.None, Pool.Managed);
        //            tmp.Dispose();
        //        }

        //    requireUpdate = false;
        //}

        //public override void OnPaint(Sprite spr)
        //{
        //    if (requireUpdate)
        //        Apply();


        //    spr.Transform = Matrix.Translation(
        //        bounds.X + sLeft + (int)((bounds.Width - textBgSize.Width - sliderSize.Width) * ((double)value / (double)max)),
        //        bounds.Y, 0);

        //    spr.Draw(slider, -1);


        //    spr.Transform = Matrix.Translation(bounds.Width - textBgSize.Width, bounds.Y, 0);
        //    spr.Draw(textBackGround, -1);
        //    spr.Draw(lbl, -1);

            


        //}


        //#region IDisposable Members

        //public void Dispose()
        //{
        //    if (!disposed)
        //    {

        //        disposed = true;
        //    }
        //    else
        //        throw new ObjectDisposedException(ToString());
        //}

        //#endregion
    }
}
