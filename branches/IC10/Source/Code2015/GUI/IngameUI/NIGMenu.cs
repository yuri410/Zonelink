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
    public enum NIGDialogState 
    {
        MovingIn,
        MovingOut,
        Showing,
        Hiding
    }

    class NIGMenu : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        Texture overlay;
        Texture background;

        float showPrg;
        NIGDialogState state;



        Button resumeButton;
        Button restartButton;
        Button exitButton;

        public NIGMenu(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
         
            FileLocation fl = FileSystem.Instance.Locate("nig_m_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("bg_black.tex", GameFileLocs.GUI);
            overlay = UITextureManager.Instance.CreateInstance(fl);
            state = NIGDialogState.Hiding;

            fl = FileSystem.Instance.Locate("nig_m_btn_resume.tex", GameFileLocs.GUI);
            resumeButton = new Button();
            resumeButton.Image = UITextureManager.Instance.CreateInstance(fl);
            resumeButton.X = 566;
            resumeButton.Y = 211;
            resumeButton.Width = resumeButton.Image.Width;
            resumeButton.Height = resumeButton.Image.Height;
            resumeButton.Enabled = true;
            resumeButton.IsValid = true;
            resumeButton.MouseClick += ResumeButton_Click;

            fl = FileSystem.Instance.Locate("nig_m_btn_restart.tex", GameFileLocs.GUI);
            restartButton = new Button();
            restartButton.Image = UITextureManager.Instance.CreateInstance(fl);
            restartButton.X = 560;
            restartButton.Y = 276;
            restartButton.Width = restartButton.Image.Width;
            restartButton.Height = restartButton.Image.Height;
            restartButton.Enabled = true;
            restartButton.IsValid = true;
            restartButton.MouseClick += RestartButton_Click;

            fl = FileSystem.Instance.Locate("nig_m_btn_back.tex", GameFileLocs.GUI);
            exitButton = new Button();
            exitButton.Image = UITextureManager.Instance.CreateInstance(fl);
            exitButton.X = 582;
            exitButton.Y = 339;
            exitButton.Width = exitButton.Image.Width;
            exitButton.Height = exitButton.Image.Height;
            exitButton.Enabled = true;
            exitButton.IsValid = true;
            exitButton.MouseClick += ExitButton_Click;
        }

        public bool IsHidden 
        {
            get { return state == NIGDialogState.Hiding; }
        }

        public void Show()
        {
            state = NIGDialogState.MovingIn;
        }
        public void Hide()
        {
            state = NIGDialogState.MovingOut;
        }

        void ResumeButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                Hide();
            }
        }
        void RestartButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                Hide();
            }
        }
        void ExitButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                parent.Over();
                Hide();
            }
        }


        public override int Order
        {
            get
            {
                return 98;
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
                resumeButton.Update(time);
                restartButton.Update(time);
                exitButton.Update(time);
            }
        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            if (state == NIGDialogState.Showing)
            {
                resumeButton.UpdateInteract(time);
                restartButton.UpdateInteract(time);
                exitButton.UpdateInteract(time);
            }
        }

        public override void Render(Sprite sprite)
        {
            if (state != NIGDialogState.Hiding)
            {
                ColorValue overlayColor = ColorValue.Transparent;
                overlayColor.A = (byte)(165 * showPrg);

                sprite.Draw(overlay, 0, 0, overlayColor);


                int x = 451;
                int y = 157;

                Matrix trans = Matrix.Translation(-background.Width / 2, -background.Height / 2, 0) *
                    Matrix.Scaling(showPrg * 0.8f + 0.2f, 1, 1) *
                    Matrix.Translation(x + background.Width / 2, y + background.Height / 2, 0);

                sprite.SetTransform(trans);
                sprite.Draw(background, 0, 0, ColorValue.White);

                sprite.SetTransform(Matrix.Identity);


                sprite.Draw(resumeButton.Image, resumeButton.X, resumeButton.Y, ColorValue.White);
                if (resumeButton.IsMouseOver)
                {
                    sprite.Draw(resumeButton.Image, resumeButton.X, resumeButton.Y - 4, ColorValue.White);
                }

                sprite.Draw(restartButton.Image, restartButton.X, restartButton.Y, ColorValue.White);
                if (restartButton.IsMouseOver)
                {
                    sprite.Draw(restartButton.Image, restartButton.X, restartButton.Y - 4, ColorValue.White);
                }

                sprite.Draw(exitButton.Image, exitButton.X, exitButton.Y, ColorValue.White);
                if (exitButton.IsMouseOver)
                {
                    sprite.Draw(exitButton.Image, exitButton.X, exitButton.Y - 4, ColorValue.White);
                }
            }
        }
    }
}
