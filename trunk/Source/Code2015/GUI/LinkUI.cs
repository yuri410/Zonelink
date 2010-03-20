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
                hoverCity = null;
                if (!object.ReferenceEquals(hoverCity, value))
                {
                    if (!object.ReferenceEquals(hoverCity, SelectedCity))
                    {
                        if (!object.ReferenceEquals(hoverCity, null) && !object.ReferenceEquals(SelectedCity, null))
                        {
                            hoverCity = value;
                        }
                    }
                }
            }
        }

        public Point HoverPoint
        {
            get;
            private set;
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
            base.Update(time);
        }
        public override void Render(Sprite sprite)
        {
            base.Render(sprite);
        }
    }
}
