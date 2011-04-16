using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI.IngameUI
{
    class HUD : UIComponent
    {
       

        public HUD(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;
            this.f6 = GameFontManager.Instance.FRuanEdged6;

            FileLocation fl = FileSystem.Instance.Locate("bubble_256x64.tex", GameFileLocs.GUI);
            cityBubbleTex = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("bubble_64.tex", GameFileLocs.GUI);
            harvesterBubbleTex = UITextureManager.Instance.CreateInstance(fl);


        }

        public override int Order
        {
            get
            {
                return 5;
            }
        }


        public override bool HitTest(int x, int y)
        {
            return false;
        }


        public override void Render(Sprite sprite)
        {
           
        }

        public override void Update(GameTime time)
        {
            base.Update(time);


        }


    }
}
