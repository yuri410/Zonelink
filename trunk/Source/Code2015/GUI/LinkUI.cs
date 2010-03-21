using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.World;
using Code2015.Logic;

namespace Code2015.GUI
{
    class LinkUI : UIComponent
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        Map map;

        CityObject hoverCity;

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
                isDirty = false;

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
                hoverPosition = value;
                PlanetEarth.GetCoord(value, out longitude, out latitude);

                int x, y;
                Map.GetMapCoord(longitude, latitude, out x, out y);

                if (hoverPoint.X != x && hoverPoint.Y != y)
                {
                    hoverPoint.X = x;
                    hoverPoint.Y = y;
                    isDirty = true;
                }
            }
        }

        void UpdatePath()
        {

        }


        public LinkUI(Code2015 game, Game parent, GameScene scene)
        {
            this.parent = parent;

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.map = parent.Map;
        }

        public override void Update(GameTime time)
        {
            if (SelectedCity != null && HoverCity != null && isDirty)
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
