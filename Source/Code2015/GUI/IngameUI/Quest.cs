using System;
using System.Collections.Generic;
using System.Text;
using Code2015.World;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Apoc3D.MathLib;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.GUI.IngameUI
{
    class Quest : UIComponent 
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;

        GameFontRuan fedge6;

        private Texture background;

        public override int Order
        {
            get
            {
                return 58;
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

            player = gamelogic.LocalHumanPlayer;

            fedge6 = GameFontManager.Instance.FRuanEdged8;
        }

        public override void Render(Sprite sprite)
        {
            base.Render(sprite);

            sprite.Draw(background, new Vector2(0, 613), ColorValue.White);

            string msg = "HELP 15 CITIES";

            fedge6.DrawString(sprite, msg, 130, 660, ColorValue.White);
            fedge6.DrawString(sprite, "\n " + player.Area.CityCount.ToString() + " / 15 )", 130, 660, ColorValue.Yellow);
          
        }
    }
}
