using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;
using Code2015.World.Screen;
using Code2015.BalanceSystem;

namespace Code2015.GUI
{
    class PieceContainerOverlay : UIComponent
    {
        Texture containers_conver;
        public PieceContainerOverlay(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            FileLocation fl = FileSystem.Instance.Locate("ig_box_cover.tex", GameFileLocs.GUI);
            containers_conver = UITextureManager.Instance.CreateInstance(fl);

        }

        public override int Order
        {
            get
            {
                return 4;
            }
        }

        public override void Render(Sprite sprite)
        {
            //sprite.Draw(ico_exchange, 1075, 521, ColorValue.White);
            sprite.Draw(containers_conver, 702, 626, ColorValue.White);

        }

    }
    class PieceContainer : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        GameFont f14;

        //侧边栏图标
        Texture containers;

        Texture[] ico_exchange;

        int count1;
        int count2;
        int count3;
        int count4;

        int exCounter;

        int currentEx;
        MdgResource exInside;

        GoalIcons icons;
        MdgResourceManager resources;





        public PieceContainer(Code2015 game, Game parent, GameScene scene, GameState gamelogic, GoalIcons icons)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.icons = icons;
            this.resources = icons.Manager;

            //侧边栏
            FileLocation fl = FileSystem.Instance.Locate("ig_box.tex", GameFileLocs.GUI);
            containers = UITextureManager.Instance.CreateInstance(fl);

            f14 = GameFontManager.Instance.F14;

            ico_exchange = new Texture[4];

            fl = FileSystem.Instance.Locate("ig_changeBox.tex", GameFileLocs.GUI);
            ico_exchange[0] = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_changeBox.tex", GameFileLocs.GUI);
            ico_exchange[1] = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_changeBox.tex", GameFileLocs.GUI);
            ico_exchange[2] = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_changeBox.tex", GameFileLocs.GUI);
            ico_exchange[3] = UITextureManager.Instance.CreateInstance(fl);


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
            sprite.Draw(containers, 702, 626, ColorValue.White);

            sprite.Draw(ico_exchange[currentEx], 1155, 598, ColorValue.White);

            exInside.Render(sprite);

            f14.DrawString(sprite, count1.ToString(), 775, 704, ColorValue.White);
            f14.DrawString(sprite, count2.ToString(), 882, 704, ColorValue.White);
            f14.DrawString(sprite, count3.ToString(), 984, 704, ColorValue.White);
            f14.DrawString(sprite, count4.ToString(), 1084, 704, ColorValue.White);

            
        }

        public override void Update(GameTime time)
        {
            count1 = 0;
            for (int i = 0; i < resources.GetResourceCount(MdgType.Hunger); i++)
            {
                if (resources.GetResource(MdgType.Hunger, i).IsInBox)
                {
                    count1++;
                }
            }

            count2 = 0;
            for (int i = 0; i < resources.GetResourceCount(MdgType.Education); i++)
            {
                if (resources.GetResource(MdgType.Education, i).IsInBox)
                {
                    count2++;
                }
            }
            for (int i = 0; i < resources.GetResourceCount(MdgType.GenderEquality); i++)
            {
                if (resources.GetResource(MdgType.GenderEquality, i).IsInBox)
                {
                    count2++;
                }
            } 
            
            count3 = 0;
            for (int i = 0; i < resources.GetResourceCount(MdgType.Diseases); i++)
            {
                if (resources.GetResource(MdgType.Diseases, i).IsInBox)
                {
                    count3++;
                }
            }
            for (int i = 0; i < resources.GetResourceCount(MdgType.MaternalHealth); i++)
            {
                if (resources.GetResource(MdgType.MaternalHealth, i).IsInBox)
                {
                    count3++;
                }
            }
            for (int i = 0; i < resources.GetResourceCount(MdgType.ChildMortality); i++)
            {
                if (resources.GetResource(MdgType.ChildMortality, i).IsInBox)
                {
                    count3++;
                }
            }

            count4 = 0;
            for (int i = 0; i < resources.GetResourceCount(MdgType.Environment); i++)
            {
                if (resources.GetResource(MdgType.Environment, i).IsInBox)
                {
                    count4++;
                }
            }

            exCounter++;
            if (exCounter > 600) 
            {
                currentEx = Randomizer.GetRandomInt(ico_exchange.Length);
                exCounter = 0;

                Vector2 position = new Vector2(1219, 668);
                exInside = new MdgResource(icons.Manager, icons.PhysicsWorld,
                    (MdgType)Randomizer.GetRandomInt((int)MdgType.Count - 1), position, 0);

            }
        }
    }
}
