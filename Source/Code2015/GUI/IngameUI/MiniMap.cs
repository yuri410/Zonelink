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

        const int MapX = 51;
        const int MapY = 28;
        const int MapWidth = 325;
        const int MapHeight = 160;

        const float RotIn = -MathEx.PiOver2;

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        RtsCamera camera;

        Texture background;
        Texture compass;
        Texture cameraView;
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
            this.camera = scene.Camera;

            FileLocation fl = FileSystem.Instance.Locate("ig_map.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            switchButton = new Button();
            switchButton.X = 302;
            switchButton.Y = 554;
            switchButton.Width = 38;
            switchButton.Height = 21;

            switchButton.Enabled = true;
            switchButton.IsValid = true;

            fl = FileSystem.Instance.Locate("ig_normal.tex", GameFileLocs.GUI);
            switchButton.Image = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_hover.tex", GameFileLocs.GUI);
            switchButton.ImageMouseOver = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_click.tex", GameFileLocs.GUI);
            switchButton.ImageMouseDown = UITextureManager.Instance.CreateInstance(fl);

            switchButton.MouseClick += SwitchButton_MouseClick;


            fl = FileSystem.Instance.Locate("ig_map_guide.tex", GameFileLocs.GUI);
            compass = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_map_view.tex", GameFileLocs.GUI);
            cameraView = UITextureManager.Instance.CreateInstance(fl);

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
                return Control.IsInBounds(x, y, ref rect) || switchButton.HitTest(x, y);
            }
            Rectangle rect2 = new Rectangle(51, 688, 111, 38);
            return Control.IsInBounds(x, y, ref rect2);
        }
        
        public override void Render(Sprite sprite)
        {
            sprite.SetTransform(Matrix.RotationZ(-rot) * Matrix.Translation(0, PanelY + PanelHeight, 0));
            sprite.Draw(background, PanelX, -PanelHeight, ColorValue.White);

            if (switchButton.IsPressed)
            {
                sprite.Draw(switchButton.ImageMouseDown, switchButton.X, switchButton.Y - (PanelY + PanelHeight), switchButton.ModulateColor);
            }
            else if (switchButton.IsMouseOver)
            {
                sprite.Draw(switchButton.ImageMouseOver, switchButton.X, switchButton.Y - (PanelY + PanelHeight), switchButton.ModulateColor);
            }
            else
            {
                sprite.Draw(switchButton.Image, switchButton.X, switchButton.Y - (PanelY + PanelHeight), switchButton.ModulateColor);
            }

            {
                int cx;
                int cy;
                float yspan = (14.0f / 18.0f) * MathEx.PIf;

                cy = (int)(((yspan * 0.5f - MathEx.Degree2Radian(camera.Latitude)) / yspan) * MapHeight);
                cx = (int)(((MathEx.Degree2Radian(camera.Longitude) + MathEx.PIf) / (2 * MathEx.PIf)) * MapWidth);

                if (cy < 0) cy += MapHeight;
                if (cy >= MapHeight) cy -= MapHeight;

                if (cx < 0) cx += MapWidth;
                if (cx >= MapWidth) cx -= MapWidth;

                float ratio = 1;
                Rectangle rect = new Rectangle(cx - (int)(ratio * cameraView.Width / 2f), cy - (int)(ratio * cameraView.Height / 2), (int)(cameraView.Width * ratio), (int)(cameraView.Height * ratio));

                sprite.Draw(cameraView, rect, ColorValue.White);
            }
            sprite.SetTransform(Matrix.Identity);


            sprite.Draw(compass, -21, 666, ColorValue.White);
        }
        public override void UpdateInteract(GameTime time)
        {
            switchButton.Update(time);

            if (state == AnimState.Inside)
            {
                state = AnimState.Out;
            }
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
