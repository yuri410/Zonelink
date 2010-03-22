using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.EngineEx;
using Code2015.World;
using Apoc3D.Vfs;
using Code2015.GUI.Controls;
using Apoc3D.Collections;

namespace Code2015.GUI
{
    class CityInfoDisplay
    {
        GameScene scene;
        RenderSystem renderSys;
        RtsCamera camera;

        Font font;
        FastList<ProgressBar> prgBars = new FastList<ProgressBar>();

        public CityInfoDisplay(GameScene scene, RenderSystem rs)
        {
            this.scene = scene;
            this.camera = scene.Camera;
            this.font = FontManager.Instance.GetFont("default");
            this.renderSys = rs;


            FileLocation fl = FileSystem.Instance.Locate("ig_prgbar_cmp.tex", GameFileLocs.GUI);
            Texture prgBg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_prgbar_imp.tex", GameFileLocs.GUI);
            Texture prgBg1 = UITextureManager.Instance.CreateInstance(fl);

            for (int i = 0; i < 25; i++)
            {
                ProgressBar bar = new ProgressBar();
               
                bar.Height = 18;
                bar.Width = 117;
                bar.ProgressImage = prgBg;
                bar.Background = prgBg1;
                prgBars.Add(bar);
            }
        }

        public void Render(Sprite sprite)
        {
            for (int i = 0; i < scene.VisibleCityCount; i++)
            {
                CityObject cc = scene.GetVisibleCity(i);

                Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(cc.Longitude), MathEx.Degree2Radian(cc.Latitude));

                Vector3 ppos = renderSys.Viewport.Project(cc.Position - tangy * (CityStyleTable.CityRadius[(int)cc.Size] + 5),
                    camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

                Size strSize = font.MeasureString(cc.Name, 20, DrawTextFormat.Center);

                //scrnPos.Y += strSize.Height;
                scrnPos.X -= strSize.Width / 2;

                font.DrawString(sprite, cc.Name, scrnPos.X + 1, scrnPos.Y + 1, 20, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                font.DrawString(sprite, cc.Name, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, -1);

                if (i < prgBars.Count)
                {
                    prgBars[i].X = scrnPos.X;
                    prgBars[i].X = scrnPos.Y - 30;
                    prgBars[i].Render(sprite);
                }
            }
        }
    }

    class ResInfoDisplay
    {
        GameScene scene;
        RenderSystem renderSys;
        RtsCamera camera;

        Font font;
        Texture background;

        public ResInfoDisplay(GameScene scene, RenderSystem rs)
        {
            this.scene = scene;
            this.camera = scene.Camera;
            this.font = FontManager.Instance.GetFont("default");
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("ig_resMeter.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);
        }

        public void Render(Sprite sprite)
        {
            
            for (int i = 0; i < scene.VisibleResourceCount; i++)
            {
                IResourceObject res = scene.GetResourceObject(i);

                Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(res.Longitude), MathEx.Degree2Radian(res.Latitude));

                Vector3 ppos = renderSys.Viewport.Project(res.Position - tangy * (res.Radius + 5),
                    camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

                string title = res.Type.ToString();
                Size strSize = font.MeasureString(title, 20, DrawTextFormat.Center);


                sprite.Draw(background, scrnPos.X, scrnPos.Y, ColorValue.White);

                font.DrawString(sprite, title, scrnPos.X + 1, scrnPos.Y + 1, 20, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                font.DrawString(sprite, title, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, -1);
            }
        }
    }
}
