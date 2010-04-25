using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.World;

namespace Code2015.GUI
{
    class ResourceInfo : UIComponent
    {
        RenderSystem renderSys;

        ResInfoDisplay parent;

        IResourceObject resource;

        ProgressBar amountBar;
        Texture woodOverlay;

        public ResourceInfo(ResInfoDisplay info, RenderSystem rs, IResourceObject res)
        {
            this.renderSys = rs;

            this.parent = info;
            this.resource = res;

            string imp = res.Type == NaturalResourceType.Wood ? "ig_prgbar_wood_imp.tex" : "ig_prgbar_oil_imp.tex";
            string cmp = res.Type == NaturalResourceType.Wood ? "ig_prgbar_wood_cmp.tex" : "ig_prgbar_oil_cmp.tex";
            string text = res.Type == NaturalResourceType.Wood ? "ig_prgbar_wood.tex" : "ig_prgbar_oil.tex";
            string gray = "ig_prgbar_gray.tex";


            amountBar = new ProgressBar();

            amountBar.Height = 30;
            amountBar.Width = 150;
            FileLocation fl = FileSystem.Instance.Locate(cmp, GameFileLocs.GUI);
            amountBar.ProgressImage = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate(imp, GameFileLocs.GUI);
            amountBar.Background = UITextureManager.Instance.CreateInstance(fl);
            amountBar.LeftPadding = 7;
            amountBar.RightPadding = 9;

            fl = FileSystem.Instance.Locate(gray, GameFileLocs.GUI);
            amountBar.Medium = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate(text, GameFileLocs.GUI);
            woodOverlay = UITextureManager.Instance.CreateInstance(fl);
        }

        public override void Render(Sprite sprite)
        {
            Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(resource.Longitude), MathEx.Degree2Radian(resource.Latitude));

            Vector3 pos = resource.Position;
            Vector3 ppos = renderSys.Viewport.Project(pos - tangy * (resource.Radius + 5),
                parent.Projection, parent.View, Matrix.Identity);

            float dist = Vector3.Distance(ref pos, ref parent.CameraPosition);
            //float d1 = 1 - MathEx.Saturate((dist - 1000) / 1500);
            float d2 = 1 - MathEx.Saturate((dist - 2000) / 1000);

            byte alpha = (byte)(d2 * byte.MaxValue);

            if (alpha > 5)
            {
                ColorValue modColor = new ColorValue(byte.MaxValue, byte.MaxValue, byte.MaxValue, alpha);

                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);
                scrnPos.X -= amountBar.Width / 2;
                scrnPos.Y -= amountBar.Height / 2;

                amountBar.ModulateColor = modColor;
                amountBar.X = scrnPos.X;
                amountBar.Y = scrnPos.Y;
                amountBar.Value = resource.AmountPer;
                amountBar.Render(sprite);

                sprite.Draw(woodOverlay, scrnPos.X, scrnPos.Y, modColor);
            }
        }

        public override void Update(GameTime time)
        {
        }
    }
}
