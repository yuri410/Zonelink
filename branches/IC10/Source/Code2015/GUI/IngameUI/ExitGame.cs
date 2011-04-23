using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI.IngameUI
{
    class ExitGame : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        NIGMenu nigMenu;

        Button exitButton;
        bool isButtonClicked;

        public bool IsExitClicked
        {
            get
            {
                return isButtonClicked;
            }
          
        }

        public override int Order
        {
            get
            {
                return 99;
            }
        }

        

        public ExitGame(Code2015 game, Game parent, GameScene scene, GameState gamelogic, NIGMenu nigMenu)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;
            this.nigMenu = nigMenu;
            exitButton = new Button();

            FileLocation fl = FileSystem.Instance.Locate("nig_esc.tex", GameFileLocs.GUI);
            exitButton.Image = UITextureManager.Instance.CreateInstance(fl);
            exitButton.X = 0;
            exitButton.Y = 0;
            exitButton.Width = exitButton.Image.Width;
            exitButton.Height = exitButton.Image.Height;
            exitButton.Enabled = true;
            exitButton.IsValid = true;

            isButtonClicked = false;

            exitButton.MouseClick += ExitButton_Pressed;
            

        }


        public override bool HitTest(int x, int y)
        {
            return base.HitTest(x, y);
        }


        public override void Render(Sprite sprite)
        {
            base.Render(sprite);

            if (exitButton.IsMouseOver)
            {
                sprite.Draw(exitButton.Image, new Rectangle(-13, 0, exitButton.Image.Width, exitButton.Image.Height),
                                            ColorValue.Yellow);
            }
            else
            {
                sprite.Draw(exitButton.Image, new Rectangle(-13, 0, exitButton.Image.Width, exitButton.Image.Height),
                                            ColorValue.White);
            }
            
        }

        public override void Update(Apoc3D.GameTime time)
        {
            base.Update(time);

            exitButton.Update(time);
        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            base.UpdateInteract(time);
            exitButton.UpdateInteract(time);
        }

        void ExitButton_Pressed(object sender, MouseButtonFlags btn)
        {
            nigMenu.Show();
        }

      
    }
}
