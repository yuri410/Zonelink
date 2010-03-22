using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.World;

namespace Code2015.GUI
{
    class CityInfoDisplay
    {
        GameScene scene;
        RenderSystem renderSys;
        RtsCamera camera;

        Font font;
        FastList<ProgressBar> prgBars = new FastList<ProgressBar>();

        Texture[] brackets;

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

            fl = FileSystem.Instance.Locate("ig_bracket.tex", GameFileLocs.GUI);
            brackets = new Texture[4];
            brackets[0] = UITextureManager.Instance.CreateInstance(fl);

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
            Matrix proj = camera.ProjectionMatrix;
            Matrix view = camera.ViewMatrix;

            int pidx = 0;
            for (int i = 0; i < scene.VisibleCityCount; i++)
            {
                CityObject cc = scene.GetVisibleCity(i);

                Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(cc.Longitude), MathEx.Degree2Radian(cc.Latitude));

                Vector3 ppos = renderSys.Viewport.Project(cc.Position - tangy * (CityStyleTable.CityRadius[(int)cc.Size] + 5),
                    proj, view, Matrix.Identity);
                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

                Size strSize = font.MeasureString(cc.Name, 20, DrawTextFormat.Center);

                //scrnPos.Y += strSize.Height;
                scrnPos.X -= strSize.Width / 2;

                font.DrawString(sprite, cc.Name, scrnPos.X + 1, scrnPos.Y + 1, 20, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                font.DrawString(sprite, cc.Name, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, -1);

                if (i < prgBars.Count && cc.IsCaptured)
                {
                    prgBars[pidx].X = scrnPos.X;
                    prgBars[pidx].Y = scrnPos.Y - 30;
                    prgBars[pidx].Value = cc.Satisfaction;
                    prgBars[pidx].Render(sprite);
                    pidx++;
                }


                if (cc.PluginCount > 0)
                {
                    Vector3 ppofs = new Vector3(0, 100, 0);
                    Matrix ctrans = PlanetEarth.GetOrientation(cc.Longitude, cc.Latitude);

                    for (int j = 0; j < cc.PluginCount; j++)
                    {
                        Matrix ptrans = cc.GetPluginTransform(j) * ctrans;
                        Vector3 plpos;
                        Vector3.TransformSimple(ref ppofs, ref ptrans, out plpos);

                        plpos = renderSys.Viewport.Project(plpos, proj, view, Matrix.Identity);

                        sprite.Draw(brackets[0], (int)plpos.X, (int)plpos.Y, ColorValue.White);
                    }
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
        FastList<ProgressBar> prgBars = new FastList<ProgressBar>();

        public ResInfoDisplay(GameScene scene, RenderSystem rs)
        {
            this.scene = scene;
            this.camera = scene.Camera;
            this.font = FontManager.Instance.GetFont("default");
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("ig_resMeter.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);
            
            
            fl = FileSystem.Instance.Locate("ig_prgbar_cmp.tex", GameFileLocs.GUI);
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
            int pidx = 0;
            Vector3 cpos = camera.Position;
            for (int i = 0; i < scene.VisibleResourceCount; i++)
            {
                IResourceObject res = scene.GetResourceObject(i);

                Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(res.Longitude), MathEx.Degree2Radian(res.Latitude));

                Vector3 pos = res.Position;
                Vector3 ppos = renderSys.Viewport.Project(pos - tangy * (res.Radius + 5),
                    camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                float dist = Vector3.Distance(ref pos, ref cpos);
                dist = 1 - MathEx.Saturate((dist-1000) / 1500);


                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

                string title = res.Type.ToString();
                Size strSize = font.MeasureString(title, 20, DrawTextFormat.Center);

                Rectangle rect = new Rectangle(scrnPos.X, scrnPos.Y, (int)(50 + 100 * dist), (int)(40 + 80 * dist));

                sprite.Draw(background, rect, ColorValue.White);

                font.DrawString(sprite, title, scrnPos.X + 1, scrnPos.Y + 1, 20, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                font.DrawString(sprite, title, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, -1);
              
                if (i < prgBars.Count)
                {
                    prgBars[pidx].X = scrnPos.X;
                    prgBars[pidx].Y = scrnPos.Y - 30;
                    prgBars[pidx].Value = res.AmountPer;
                    prgBars[pidx].Render(sprite);
                    pidx++;
                }
            }
        }
    }
}
