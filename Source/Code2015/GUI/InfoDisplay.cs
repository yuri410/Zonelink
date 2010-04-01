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
            for (int i = 0; i < scene.VisibleCityCount; i++)
            {
                CityObject cc = scene.GetVisibleCity(i);

                CityInfo info;

                if (!cityTable.TryGetValue(cc, out info))
                {
                    info = new CityInfo(this, renderSys, cc, player);
                    cityTable.Add(cc, info);
                }

                info.Update(time);
            }

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
        Dictionary<IResourceObject, ResourceInfo> resTable;
        GameScene scene;
        RenderSystem renderSys;
        RtsCamera camera;



        public Matrix Projection;
        public Matrix View;

        public Vector3 CameraPosition;

        public ResInfoDisplay(GameScene scene, RenderSystem rs)
        {
            this.scene = scene;
            this.camera = scene.Camera;
            this.renderSys = rs;
            this.resTable = new Dictionary<IResourceObject, ResourceInfo>();
        }

        public void Render(Sprite sprite)
        {
            Projection = camera.ProjectionMatrix;
            View = camera.ViewMatrix;
            CameraPosition = camera.Position;

            for (int i = 0; i < scene.VisibleResourceCount; i++)
            {
                IResourceObject res = scene.GetResourceObject(i);

                ResourceInfo info;

                if (!resTable.TryGetValue(res, out info))
                {
                    info = new ResourceInfo(this, renderSys, res);
                    resTable.Add(res, info);
                }

                info.Render(sprite);
            }
        }

        public void Update(GameTime time)
        {
            for (int i = 0; i < scene.VisibleResourceCount; i++)
            {
                IResourceObject res = scene.GetResourceObject(i);

                ResourceInfo info;

                if (!resTable.TryGetValue(res, out info))
                {
                    info = new ResourceInfo(this, renderSys, res);
                    resTable.Add(res, info);
                }

                info.Update(time);
            }
        }
    }
}
