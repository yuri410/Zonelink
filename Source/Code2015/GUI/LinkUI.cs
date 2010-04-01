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
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class LinkUI : UIComponent
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        InGameUI igui;

        Map map;
        PathFinder pathFinder;
        Player player;

        CityObject hoverCity;
        

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
        }

        void Link() 
        {
            if (SelectedCity != null && hoverCity != null && SelectedCity != hoverCity)
            {
                if (!hoverCity.IsPlayerCapturing(player) && !hoverCity.IsCaptured)
                {
                    hoverCity.Capture.SetCapture(player, SelectedCity.City);

                    City a = SelectedCity.City;
                    City b = hoverCity.City;

                    CityLinkObject link = new CityLinkObject(renderSys, SelectedCity, hoverCity);
                    scene.Scene.AddObjectToScene(link);
                }
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
