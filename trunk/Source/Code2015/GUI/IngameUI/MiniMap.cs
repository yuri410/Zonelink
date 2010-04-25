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
        const int PanelX = -48;
        const int PanelY = 547;
        const int PanelWidth = 388;
        const int PanelHeight = 179;

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
        Texture compass;
        Button switchButton;

        AnimState state;

        float rot;

        public MiniMap(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.state = AnimState.Outside;

            FileLocation fl = FileSystem.Instance.Locate("ig_map.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            const int SWBRadius = 79 / 2;
            switchButton = new Button();
            switchButton.X = 302;
            switchButton.Y = 720 - 554;
            switchButton.Width = 38;
            switchButton.Height = 21;

            switchButton.Enabled = true;
            switchButton.IsValid = true;

            switchButton.Image = UITextureManager.Instance.CreateInstance(fl);
            switchButton.MouseClick += SwitchButton_MouseClick;


            fl = FileSystem.Instance.Locate("ig_map_guide.tex", GameFileLocs.GUI);
            compass = UITextureManager.Instance.CreateInstance(fl);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                background.Dispose();
                background = null;

            }

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
            sprite.SetTransform(Matrix.RotationZ(-rot) * Matrix.Translation(0, PanelY + PanelHeight, 0));
            sprite.Draw(background, PanelX, -PanelHeight, ColorValue.White);
            
            sprite.SetTransform(Matrix.Identity);

            switchButton.Render(sprite);
            sprite.Draw(compass, -21, 666, ColorValue.White);
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
