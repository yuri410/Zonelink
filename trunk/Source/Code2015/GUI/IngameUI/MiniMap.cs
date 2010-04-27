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
using Code2015.BalanceSystem;

namespace Code2015.GUI
{
    class MiniMap : UIComponent
    {
        const int PanelX = -48;
        const int PanelY = 547;
        const int PanelWidth = 388;
        const int PanelHeight = 179;

        const float PopBaseSpeed = 0.75f;

        const int MapX = 7;
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
        Button switchButton;

        #region 内容显示
        Texture cameraView;

        Texture redDot;
        Texture greenDot;
        Texture blueDot;
        Texture yellowDot;

        Texture whiteDot;

        Texture redRing;
        Texture greenRing;
        Texture blueRing;
        Texture yellowRing;
        Dictionary<City, Point> positionBuffer = new Dictionary<City, Point>();
        #endregion

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

            #region 内容显示
            fl = FileSystem.Instance.Locate("ig_map_view.tex", GameFileLocs.GUI);
            cameraView = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_mp_redpoint.tex", GameFileLocs.GUI);
            redDot = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_greenpoint.tex", GameFileLocs.GUI);
            greenDot = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_yellowpoint.tex", GameFileLocs.GUI);
            yellowDot = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_bluepoint.tex", GameFileLocs.GUI);
            blueDot = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_whitepoint.tex", GameFileLocs.GUI);
            whiteDot = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_mp_red.tex", GameFileLocs.GUI);
            redRing  = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_green.tex", GameFileLocs.GUI);
            greenRing = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_yellow.tex", GameFileLocs.GUI);
            yellowRing = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_blue.tex", GameFileLocs.GUI);
            blueRing = UITextureManager.Instance.CreateInstance(fl);

            #endregion

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

        Point GetPosition(float lng, float lat) 
        {
            int cx;
            int cy;
            float yspan = MathEx.PIf;

            cy = (int)(((yspan * 0.5f - lat) / yspan) * MapHeight);
            cx = (int)(((lng + MathEx.PIf) / (2 * MathEx.PIf)) * MapWidth);

            if (cy < 0) cy += MapHeight;
            if (cy >= MapHeight) cy -= MapHeight;

            if (cx < 0) cx += MapWidth;
            if (cx >= MapWidth) cx -= MapWidth;
            return new Point(cx, cy);
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

            SimulationWorld slgWorld = gameLogic.SLGWorld;

            for (int i = 0; i < slgWorld.CityCount; i++)
            {
                City cc = slgWorld.GetCity(i);

                if (!cc.IsHomeCity)
                {
                    Texture dot;
                    if (cc.IsCaptured)
                    {
                        ColorValue color = cc.Owner.SideColor;
                        if (color == ColorValue.Red)
                        {
                            dot = redDot;
                        }
                        else if (color == ColorValue.Green)
                        {
                            dot = greenDot;
                        }
                        else if (color == ColorValue.Blue)
                        {
                            dot = blueDot;
                        }
                        else
                        {
                            dot = yellowDot;
                        }
                    }
                    else
                    {
                        dot = whiteDot;
                    }

                    Point pt;
                    if (!positionBuffer.TryGetValue(cc, out pt))
                    {
                        positionBuffer.Add(cc, GetPosition(MathEx.Degree2Radian(cc.Longitude), MathEx.Degree2Radian(cc.Latitude)));
                    }
                    sprite.Draw(dot, pt.X - 7, pt.Y - 7, ColorValue.White);
                }
                else
                {
                    Texture ring;

                    if (cc.IsCaptured)
                    {
                        ColorValue color = cc.Owner.SideColor;
                        if (color == ColorValue.Red)
                        {
                            ring = redRing;
                        }
                        else if (color == ColorValue.Green)
                        {
                            ring = greenRing;
                        }
                        else if (color == ColorValue.Blue)
                        {
                            ring = blueRing;
                        }
                        else
                        {
                            ring = yellowRing;
                        }

                        Point pt;
                        if (!positionBuffer.TryGetValue(cc, out pt))
                        {
                            positionBuffer.Add(cc, GetPosition(MathEx.Degree2Radian(cc.Longitude), MathEx.Degree2Radian(cc.Latitude)));
                        }
                        sprite.Draw(ring, pt.X - 12, pt.Y - 12, ColorValue.White);
                    }

                }
            }

            {
                Point pt = GetPosition(camera.Longitude, camera.Latitude);

                float ratio = camera.Height / 60f;
                Rectangle rect = new Rectangle(
                    pt.X + MapX - (int)(cameraView.Width * ratio * 0.5f),
                    pt.Y + MapY - (int)(cameraView.Height * ratio * 0.5f), (int)(cameraView.Width * ratio), (int)(cameraView.Height * ratio));
                rect.Y -= PanelHeight;

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
            if (state == AnimState.Outside)
            {
                Rectangle rect2 = new Rectangle(MapX, PanelY + MapY, MapWidth, MapHeight);
                if (MouseInput.IsLeftPressed && Control.IsInBounds(MouseInput.X, MouseInput.Y, ref rect2))
                {
                    int x = MouseInput.X - MapX;
                    int y = MouseInput.Y - MapY - PanelY;

                    float yspan = MathEx.PIf;

                    camera.Latitude = yspan * 0.5f - y * yspan / (float)MapHeight;
                    camera.Longitude = x * MathEx.PIf * 2 / (float)MapWidth - MathEx.PIf;
                }
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

        }
    }
}
