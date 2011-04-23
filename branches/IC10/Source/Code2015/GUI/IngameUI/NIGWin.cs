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
    class NIGWin : UIComponent
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

        public NIGWin(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
         
            FileLocation fl = FileSystem.Instance.Locate("nig_result_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);
          
            fl = FileSystem.Instance.Locate("bg_black.tex", GameFileLocs.GUI);
            overlay = UITextureManager.Instance.CreateInstance(fl);
            state = NIGDialogState.Hiding;
        }
        public void Show()
        {
            state = NIGDialogState.Showing;
        }
        public void Hide()
        {
            state = NIGDialogState.Hiding;
        }

        public override int Order
        {
            get
            {
                return 97;
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
            if (state == NIGDialogState.Hiding)
            {
                showPrg -= time.ElapsedGameTimeSeconds;
                if (showPrg < 0)
                {
                    showPrg = 0;
                }
            }
            else if (state == NIGDialogState.Showing)
            {
                showPrg += time.ElapsedGameTimeSeconds;
                if (showPrg > 1)
                {
                    showPrg = 1;
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
                overlayColor.A = (byte)(byte.MaxValue * showPrg);

                sprite.Draw(overlay, 0, 0, overlayColor);


            }
        }
    }
}
