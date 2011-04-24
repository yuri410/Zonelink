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
    class NIGColor : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        Texture overlay;
        Texture background;
        Texture chooseColor;

        Button okButton;


        float showPrg;
        NIGDialogState state;

        public NIGColor(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;

            FileLocation fl = FileSystem.Instance.Locate("nig_colordlg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_colordlg_color.tex", GameFileLocs.GUI);
            chooseColor = UITextureManager.Instance.CreateInstance(fl);
            
            fl = FileSystem.Instance.Locate("nig_colordlg_ok.tex", GameFileLocs.GUI);
            okButton = new Button();
            okButton.Image = UITextureManager.Instance.CreateInstance(fl);
            okButton.X = 765; 
            okButton.Y = 337;
            okButton.Width = okButton.Image.Width;
            okButton.Height = okButton.Image.Height;
            okButton.Enabled = true;
            okButton.IsValid = true;
            okButton.MouseClick += OkButton_Click;

            

            fl = FileSystem.Instance.Locate("bg_black.tex", GameFileLocs.GUI);
            overlay = UITextureManager.Instance.CreateInstance(fl);
            state = NIGDialogState.Hiding;
        }


        public void Show()
        {
            state = NIGDialogState.MovingIn;
        }
        public void Hide()
        {
            state = NIGDialogState.MovingOut;
        }


        public override int Order
        {
            get
            {
                return 95;
            }
        }

        public override bool HitTest(int x, int y)
        {
            if (state != NIGDialogState.Hiding)
            {
                return true;
            }
            return false;
        }

        public override void Update(Apoc3D.GameTime time)
        {
            if (state == NIGDialogState.MovingOut)
            {
                showPrg -= time.ElapsedGameTimeSeconds * 8;
                if (showPrg < 0)
                {
                    showPrg = 0;
                    state = NIGDialogState.Hiding;
                }
            }
            else if (state == NIGDialogState.MovingIn)
            {
                showPrg += time.ElapsedGameTimeSeconds * 8;
                if (showPrg > 1)
                {
                    showPrg = 1;
                    state = NIGDialogState.Showing;
                }
            }
            if (state == NIGDialogState.Showing)
            {
                okButton.Update(time);
            
            }
        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            if (state == NIGDialogState.Showing)
            {
                okButton.Update(time);
              
            }
        }


        void OkButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                Hide();
            }
        }

        public override void Render(Sprite sprite)
        {
            if (state != NIGDialogState.Hiding)
            {
                ColorValue overlayColor = ColorValue.Transparent;
                overlayColor.A = (byte)(165 * showPrg);

                sprite.Draw(overlay, 0, 0, overlayColor);


                int x = 461; 
                int y = 166;


                Matrix trans = Matrix.Translation(-background.Width / 2, -background.Height / 2, 0) *
                    Matrix.Scaling(showPrg * 0.8f + 0.2f, 1, 1) *
                    Matrix.Translation(x + background.Width / 2, y + background.Height / 2, 0);

                sprite.SetTransform(trans);
                sprite.Draw(background, 0, 0, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);

            }
            if (state == NIGDialogState.Showing)
            {                
                sprite.Draw(chooseColor, 673, 208, gameLogic.LocalHumanPlayer.SideColor);

                sprite.Draw(okButton.Image, okButton.X, okButton.Y, ColorValue.White);
                if (okButton.IsMouseOver)
                {
                    sprite.Draw(okButton.Image, okButton.X, okButton.Y - 4, ColorValue.White);
                }
            }
        }
    }
}
