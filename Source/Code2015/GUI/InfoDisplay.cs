using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class CityInfoDisplay : UIComponent
    {
        Dictionary<CityObject, CityInfo> cityTable;
        GameScene scene;
        RtsCamera camera;
        Player player;
        RenderSystem renderSys;

        FastList<Popup> popUps = new FastList<Popup>();


        public Matrix Projection;
        public Matrix View;

        public Vector3 CameraPosition;

        public CityInfoDisplay(GameScene scene, RenderSystem rs, Player player)
        {
            this.scene = scene;
            this.cityTable = new Dictionary<CityObject, CityInfo>();
            this.renderSys = rs;
            this.player = player;
            this.camera = scene.Camera;
        }

        public override void Render(Sprite sprite)
        {
            Projection = camera.ProjectionMatrix;
            View = camera.ViewMatrix;
            CameraPosition = camera.Position;

            for (int i = 0; i < scene.VisibleCityCount; i++)
            {
                CityObject cc = scene.GetVisibleCity(i);

                CityInfo info;

                if (!cityTable.TryGetValue(cc, out info))
                {
                    info = new CityInfo(this, renderSys, cc, player);
                    cityTable.Add(cc, info);
                }

                info.Render(sprite);
            }


            for (int i = 0; i < popUps.Count; i++)
            {
                popUps[i].Render(sprite);
            }
        }

        public override void Update(GameTime time)
        {
            for (int i = popUps.Count - 1; i >= 0; i--)
            {
                if (popUps[i].IsFinished)
                {
                    popUps.RemoveAt(i);
                }
                else
                {
                    popUps[i].Update(time);
                }
            }
        }

        public void AddPopup(Popup p)
        {
            popUps.Add(p);
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

        public void Update(GameTime time)
        {
            
        }
    }
}
