using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015.GUI
{
    class CityInfoDisplay
    {
        GameScene scene;
        RenderSystem renderSys;
        RtsCamera camera;

        Font font;

        public CityInfoDisplay(GameScene scene, RenderSystem rs)
        {
            this.scene = scene;
            this.camera = scene.Camera;
            this.font = FontManager.Instance.GetFont("def");
            this.renderSys = rs;
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
            }
        }
    }

    class ResInfoDisplay
    {
        GameScene scene;
        RenderSystem renderSys;
        RtsCamera camera;

        Font font;

        public ResInfoDisplay(GameScene scene, RenderSystem rs)
        {
            this.scene = scene;
            this.camera = scene.Camera;
            this.font = FontManager.Instance.GetFont("def");
            this.renderSys = rs;
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

                font.DrawString(sprite, title, scrnPos.X + 1, scrnPos.Y + 1, 20, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                font.DrawString(sprite, title, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, -1);
            }
        }
    }
}
