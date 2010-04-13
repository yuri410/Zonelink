using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.World;

namespace Code2015.GUI
{
    class ResourceInfo : UIComponent
    {
        RenderSystem renderSys;
        Font font;

        ResInfoDisplay parent;

        IResourceObject resource;

        Texture background;
        ProgressBar amountBar;



        public ResourceInfo(ResInfoDisplay info, RenderSystem rs, IResourceObject res)
        {
            this.renderSys = rs;
            this.font = FontManager.Instance.GetFont("default");
            this.parent = info;
            this.resource = res;

            FileLocation fl = FileSystem.Instance.Locate("ig_resMeter.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);
            
            fl = FileSystem.Instance.Locate("ig_prgbar_cmp.tex", GameFileLocs.GUI);
            Texture prgBg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_prgbar_imp.tex", GameFileLocs.GUI);
            Texture prgBg1 = UITextureManager.Instance.CreateInstance(fl);

            amountBar = new ProgressBar();

            amountBar.Height = 18;
            amountBar.Width = 117;
            amountBar.ProgressImage = prgBg;
            amountBar.Background = prgBg1;

        }

        public override void Render(Sprite sprite)
        {
            Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(resource.Longitude), MathEx.Degree2Radian(resource.Latitude));

            Vector3 pos = resource.Position;
            Vector3 ppos = renderSys.Viewport.Project(pos - tangy * (resource.Radius + 5),
                parent.Projection, parent.View, Matrix.Identity);

            float dist = Vector3.Distance(ref pos, ref parent.CameraPosition);
            float d1 = 1 - MathEx.Saturate((dist - 1000) / 1500);
            float d2 = 1 - MathEx.Saturate((dist - 3000) / 1000);

            byte alpha = (byte)(d2 * byte.MaxValue);

            if (alpha > 5)
            {
                ColorValue modColor = new ColorValue(byte.MaxValue, byte.MaxValue, byte.MaxValue, alpha);

                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

                string title = resource.Type.ToString();
                Size strSize = font.MeasureString(title, 20, DrawTextFormat.Center);

                Rectangle rect = new Rectangle(scrnPos.X, scrnPos.Y, (int)(50 + 100 * d1), (int)(40 + 80 * d1));

                sprite.Draw(background, rect, modColor);

                font.DrawString(sprite, title, scrnPos.X + 1, scrnPos.Y + 1, 20, DrawTextFormat.Center, (int)(modColor.A << 24));
                font.DrawString(sprite, title, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, (int)modColor.PackedValue);

                amountBar.ModulateColor = modColor;
                amountBar.X = scrnPos.X;
                amountBar.Y = scrnPos.Y - 30;
                amountBar.Value = resource.AmountPer;
                amountBar.Render(sprite);
            }
        }

        public override void Update(GameTime time)
        {
        }
    }
}
