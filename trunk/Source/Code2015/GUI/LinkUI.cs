using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;
using Apoc3D.Vfs;
using Apoc3D.Graphics.Animation;

namespace Code2015.GUI
{
    class LinkUI : UIComponent
    {
        class PreviewPath : StaticModelObject
        {

            public override bool IsSerializable
            {
                get { return false; }
            }

            public void SetModel0(Model mdl)
            {
                ModelL0 = mdl;
            }
            public override RenderOperation[] GetRenderOperation()
            {
                return base.GetRenderOperation();
            }
            public override RenderOperation[] GetRenderOperation(int level)
            {
                return base.GetRenderOperation(level);
            }
        }

        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        Map map;
        PathFinder pathFinder;
        

        CityObject hoverCity;
        PreviewPath staticObject;
        ModelData pathMdlData;

        float longitude;
        float latitude;
        Vector3 hoverPosition;
        Point hoverPoint;

        bool isDirty;


        public CityObject SelectedCity
        {
            get;
            set;
        }

        public CityObject HoverCity
        {
            get { return hoverCity; }
            set
            {
                if (object.ReferenceEquals(value, null))
                {
                    hoverCity = null;
                    return;
                }

                if (!object.ReferenceEquals(hoverCity, value))
                {
                    if (!object.ReferenceEquals(value, SelectedCity))
                    {
                        if (!object.ReferenceEquals(value, null) && !object.ReferenceEquals(SelectedCity, null))
                        {
                            hoverCity = value;
                            isDirty = true;
                        }
                    }
                }
            }
        }

        public Vector3 HoverPoint
        {
            get { return hoverPosition; }
            set
            {

                // 计算结果有错

                hoverPosition = value;
                PlanetEarth.GetCoord(value, out longitude, out latitude);

                int x, y;
                Map.GetMapCoord(longitude, latitude, out x, out y);

                if (hoverPoint.X != x || hoverPoint.Y != y)
                {
                    hoverPoint.X = x;
                    hoverPoint.Y = y;
                    isDirty = true;
                }
            }
        }

        void UpdatePath()
        {
            int sx, sy;
            Map.GetMapCoord(MathEx.Degree2Radian(SelectedCity.Longitude), MathEx.Degree2Radian(SelectedCity.Latitude), out sx, out sy);

            int tx, ty;
            if (!object.ReferenceEquals(hoverCity, null))
            {
                Map.GetMapCoord(MathEx.Degree2Radian(hoverCity.Longitude), MathEx.Degree2Radian(hoverCity.Latitude), out tx, out ty);
            }
            else
            {
                tx = hoverPoint.X;
                ty = hoverPoint.Y;
            }

            pathFinder.Reset();

            PathFinderResult result = pathFinder.FindPath(sx, sy, tx, ty);

            if (result != null && result.NodeCount >0)
            {
                Point[] pts = new Point[result.NodeCount];
                for (int i = 0; i < result.NodeCount; i++)
                {
                    pts[i] = result[i];
                }
                pathMdlData = PathBuilder.BuildModel(renderSys, map, pts);

                staticObject.SetModel0(new Model(new ResourceHandle<ModelData>(pathMdlData, true)));

            }
            else if (!object.ReferenceEquals(staticObject.ModelL0, null))
            {
                staticObject.SetModel0(null);
                pathMdlData.Dispose();
            }
            
        }


        public LinkUI(Code2015 game, Game parent, GameScene scene)
        {
            this.parent = parent;

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.map = parent.Map;

            this.pathFinder = map.PathFinder.CreatePathFinder();

            FileLocation fl = FileSystem.Instance.Locate("tree01_l0.mesh", GameFileLocs.Model);

            staticObject = new PreviewPath();

            staticObject.SetModel0(new Model(ModelManager.Instance.CreateInstance(renderSys, fl)));
            staticObject.ModelL0.CurrentAnimation = new NoAnimation(Matrix.Scaling(7, 7, 7));
            staticObject.BoundingSphere.Radius = float.MaxValue;

            scene.Scene.AddObjectToScene(staticObject);
        }

        public override void Update(GameTime time)
        {
            if (SelectedCity != null && isDirty && MouseInput.IsRightPressed)
            {
                UpdatePath();
                isDirty = false;
            }
        }
        public override void Render(Sprite sprite)
        {
            

        }
    }
}
