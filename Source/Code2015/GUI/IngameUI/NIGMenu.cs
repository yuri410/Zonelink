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
    enum NIGDialogState 
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


            resumeButton = new Button();


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
        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            
        }

        public override void Render(Sprite sprite)
        {
            if (state != NIGDialogState.Hiding)
            {
                ColorValue overlayColor = ColorValue.Transparent;
                overlayColor.A = (byte)(165 * showPrg);

                sprite.Draw(overlay, 0, 0, overlayColor);


                int x = 1280 / 2;
                int y = 720 / 2;

                Matrix trans = Matrix.Translation(-background.Width / 2, -background.Height / 2, 0) *
                    Matrix.Scaling(showPrg * 0.8f + 0.2f, 1, 1) *
                    Matrix.Translation(x, y, 0);

                sprite.SetTransform(trans);
                sprite.Draw(background, 0, 0, ColorValue.White);

                sprite.SetTransform(Matrix.Identity);
            }
        }
    }
}
