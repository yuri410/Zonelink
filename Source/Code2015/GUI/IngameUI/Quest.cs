using System;
using System.Collections.Generic;
using System.Text;
using Code2015.World;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Apoc3D.MathLib;
using Code2015.EngineEx;

namespace Code2015.GUI.IngameUI
{
    class Quest : UIComponent 
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
  

        private Texture background;

        public override int Order
        {
            get
            {
                return 50;
            }
        }

        public Quest(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;

            FileLocation fl = FileSystem.Instance.Locate("nig_quest.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);
        }

        public override void Render(Sprite sprite)
        {
            base.Render(sprite);

            sprite.Draw(background, new Vector2(0, 613), ColorValue.White);
        }
    }
}
