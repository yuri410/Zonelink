using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D;
using Code2015.World;
using Apoc3D.MathLib;

namespace Code2015.GUI
{
    class LinkUI : UIComponent
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;


        public CityObject SelectedCity
        {
            get;
            private set;
        }

        public CityObject HoverCity
        {
            get;
            private set;
        }

        public Point HoverPoint
        {
            get;
            private set;
        }

        public LinkUI(Code2015 game, Game parent, GameScene scene)
        {
            this.parent = parent;

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
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
