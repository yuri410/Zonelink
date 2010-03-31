using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;
using Code2015.BalanceSystem;

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
        InGameUI igui;

        Map map;
        PathFinder pathFinder;
        Player player;

        CityObject hoverCity;
        
        PreviewPath staticObject;
        ModelData pathMdlData;

        float longitude;
        float latitude;
        Vector3 hoverPosition;
        Point hoverPoint;

        bool isDirty;
        bool isLinking;


        
        #region 属性

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
                if (value==null)
                {
                    hoverCity = null;
                    return;
                }

                if (hoverCity != value)
                {
                    if (SelectedCity != value)
                    {
                        if (SelectedCity != null)
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
        #endregion

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


        public LinkUI(Code2015 game, Game parent, GameScene scene, InGameUI igui)
        {
            this.parent = parent;
            this.igui = igui;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.map = parent.Map;
            this.player = parent.HumanPlayer;

            this.pathFinder = map.PathFinder.CreatePathFinder();

            staticObject = new PreviewPath();
            staticObject.BoundingSphere.Radius = float.MaxValue;

            scene.Scene.AddObjectToScene(staticObject);
        }

        void Link() 
        {
            if (SelectedCity != null && hoverCity != null && SelectedCity != hoverCity)
            {
                hoverCity.Capture.SetCapture(player, SelectedCity.City);

                City a = SelectedCity.City;
                City b = hoverCity.City;


                CityLinkObject link = new CityLinkObject(renderSys, SelectedCity, hoverCity, staticObject.ModelL0);
                staticObject.SetModel0(null);
                scene.Scene.AddObjectToScene(link);
            }
            isLinking = false;
        }
        public void Interact(GameTime time)
        {

            if (MouseInput.IsMouseDownLeft)
            {
                SelectedCity = igui.MouseHoverCity;
            }
            if (MouseInput.IsMouseUpLeft && isLinking)
            {
                Link();
            }
            else if (MouseInput.IsLeftPressed && MouseInput.IsMouseMoving)
            {
                if (!isLinking)
                {
                    isLinking = MouseInput.DX > 5 || MouseInput.DY > 5;
                }
                else
                {
                    HoverCity = igui.MouseHoverCity;
                    BoundingSphere earthSphere = new BoundingSphere(new Vector3(), PlanetEarth.PlanetRadius);

                    Vector3 intersect;
                    if (BoundingSphere.Intersects(earthSphere, igui.SelectionRay, out intersect))
                    {
                        HoverPoint = intersect;
                    }

                }
            }
        }
        public override void Update(GameTime time)
        {
            if (SelectedCity != null && isDirty)
            {
                UpdatePath();
                isDirty = false;
            }
        }

        public override void Render(Sprite sprite)
        {
            if (isLinking) 
            {
                
            }

        }
    }
}
