using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015.GUI
{
    class PieceContainer : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        //侧边栏图标
        Texture ico_box1;
        Texture ico_box2;
        Texture ico_box3;
        Texture ico_box4;
        Texture ico_exchange;

        public PieceContainer(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;


            //侧边栏
            FileLocation fl = FileSystem.Instance.Locate("ig_box1.tex", GameFileLocs.GUI);
            ico_box1 = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_box2.tex", GameFileLocs.GUI);
            ico_box2 = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_box3.tex", GameFileLocs.GUI);
            ico_box3 = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_box4.tex", GameFileLocs.GUI);
            ico_box4 = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_changeBox.tex", GameFileLocs.GUI);
            ico_exchange = UITextureManager.Instance.CreateInstance(fl);
        }

        public override int Order
        {
            get { return 2; }
        }
        public override bool HitTest(int x, int y)
        {
            return false;
        }
        public override void UpdateInteract(GameTime time)
        {
            
        }

        public override void Render(Sprite sprite)
        {
            sprite.Draw(ico_exchange, 1075, 521, ColorValue.White);
            sprite.Draw(ico_box1, 614, 606, ColorValue.White);
            sprite.Draw(ico_box2, 729, 606, ColorValue.White);
            sprite.Draw(ico_box3, 844, 606, ColorValue.White);
            sprite.Draw(ico_box4, 959, 606, ColorValue.White);
        }

        public override void Update(GameTime time)
        {

        }
    }
}
