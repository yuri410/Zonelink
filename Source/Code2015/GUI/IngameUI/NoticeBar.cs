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

namespace Code2015.GUI
{
    class NoticeBar : UIComponent
    {
        const int PanelX = -2;
        const int PanelY = 30;
        const int PanelWidth = 635;
        const int PanelHeight = 70;
        const float PopBaseSpeed = 20;

        const float DisplayDuration = 10;

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;

        AnimState state;

        float cx;

        Texture background;
        float timeCounter;

        

        public NoticeBar(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.player = parent.HumanPlayer;

            FileLocation fl = FileSystem.Instance.Locate("ig_notice.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);


        }
        public override int Order
        {
            get { return 6; }
        }
        public override bool HitTest(int x, int y)
        {
            return false;
        }
        public override void Render(Sprite sprite)
        {
            if (state != AnimState.Inside)
            {
                sprite.Draw(background, (int)cx, PanelY, ColorValue.White);
            }
        }

        public override void Update(GameTime time)
        {
            if (state == AnimState.Out)
            {
                cx += (PanelWidth + PanelX - cx + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (cx >= PanelX)
                {
                    cx = PanelX;
                    state = AnimState.Outside;
                }

            }
            else if (state == AnimState.In)
            {
                cx -= (PanelWidth + PanelX - cx + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (cx < -PanelWidth)
                {
                    cx = -PanelWidth;
                    state = AnimState.Inside;
                    timeCounter = DisplayDuration;
                }
            }

            if (state == AnimState.Outside)
            {
                timeCounter -= time.ElapsedGameTimeSeconds;
                if (timeCounter < 0)
                {
                    state = AnimState.In;
                }
            }
        }
    }
}
