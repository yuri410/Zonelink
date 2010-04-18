using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015.GUI
{
    class MiniMap : UIComponent
    {
        const int PanelX = 0;
        const int PanelY = 530;
        const int PanelWidth = 364;
        const int PanelHeight = 190;
        const float PopBaseSpeed = 0.75f;

        const int MapX = 45;
        const int MapY = 29;
        const int MapWidth = 290;
        const int MapHeight = 125;

        const float RotIn = -MathEx.PiOver2;

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        Texture background;
        RoundButton switchButton;

        AnimState state;

        float rot = RotIn;

        public MiniMap(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.state = AnimState.Outside;

            FileLocation fl = FileSystem.Instance.Locate("ig_minimap.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            const int SWBRadius = 64;
            switchButton = new RoundButton();
            switchButton.X = -SWBRadius;
            switchButton.Y = Program.ScreenHeight - SWBRadius;
            switchButton.Radius = SWBRadius;
            switchButton.ResizeImage = true;
            switchButton.Enabled = true;
            switchButton.IsValid = true;

            fl = FileSystem.Instance.Locate("ig_circle1.tex", GameFileLocs.GUI);
            switchButton.Image = UITextureManager.Instance.CreateInstance(fl);
            switchButton.MouseClick += SwitchButton_MouseClick;
        }

        void SwitchButton_MouseClick(object sender, MouseButtonFlags btn)
        {
            if (state != AnimState.Inside)
            {
                state = AnimState.In;
            }
            else if (state != AnimState.Outside)
            {
                state = AnimState.Out;
            }
        }
        public override int Order
        {
            get { return 4; }
        }
        public override bool HitTest(int x, int y)
        {
            if (state == AnimState.Outside)
            {
                Rectangle rect = new Rectangle(PanelX, PanelY, PanelWidth, PanelHeight);
                return Control.IsInBounds(x, y, ref rect);
            }
            return switchButton.HitTest(x, y);
        }
        
        public override void Render(Sprite sprite)
        {
            sprite.SetTransform(Matrix.RotationZ(-rot) * Matrix.Translation(PanelX, PanelY + PanelHeight, 0));
            sprite.Draw(background, 0, -PanelHeight, ColorValue.White);

            sprite.SetTransform(Matrix.Identity);

            switchButton.Render(sprite);
        }
        public override void UpdateInteract(GameTime time)
        {
            switchButton.Update(time);
        }
        public override void Update(GameTime time)
        {
            const float StdRot = 0;
            if (state == AnimState.Out)
            {
                rot += (StdRot - rot + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (rot >= StdRot)
                {
                    rot = StdRot;
                    state = AnimState.Outside;
                }

            }
            else if (state == AnimState.In)
            {
                rot -= (StdRot - rot + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (rot < RotIn)
                {
                    rot = RotIn;
                    state = AnimState.Inside;                    
                }
            }

            if (state == AnimState.Outside)
            {
                //timeCounter -= time.ElapsedGameTimeSeconds;
                //if (timeCounter < 0)
                //{
                //    state = AnimState.In;
                //}
            }
        }
    }
}
