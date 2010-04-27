using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class MainMenu : UIComponent
    {
        class MenuCamera : Camera
        {
            public MenuCamera(float aspect)
                : base(aspect)
            {
                FieldOfView = 45;
                Position = new Vector3(-12784.912f, 6031.855f, -4521.323f);
                NearPlane = 100;
                FarPlane = 25000;
            }
            public override void UpdateProjection()
            {
                float fovy = MathEx.Degree2Radian(45);
                NearPlaneHeight = (float)(Math.Tan(fovy * 0.5f)) * NearPlane * 2;
                NearPlaneWidth = NearPlaneHeight * AspectRatio;

                Frustum.proj = Matrix.PerspectiveRH(NearPlaneWidth, NearPlaneHeight, NearPlane, FarPlane);

            }
            public override void Update(GameTime time)
            {
                Vector3 target = new Vector3(273.556f, 6031.855f, -2766.552f);

                base.Update(time);
                Frustum.view = Matrix.LookAtLH(Position, target, Vector3.UnitY);
                Frustum.Update();

                orientation = Quaternion.RotationMatrix(Frustum.view);

                Matrix m = Matrix.Invert(Frustum.view);
                front = m.Forward;// MathEx.GetMatrixFront(ref m);
                top = m.Up;// MathEx.GetMatrixUp(ref m);
                right = m.Right;// MathEx.GetMatrixRight(ref m);
            }
        }


        Code2015 game;
        Menu parent;

        SceneRenderer renderer;

        float fps;

        Texture credits;
        Texture exit;
        Texture help;
        Texture start;

        Texture credits_hover;
        Texture exit_hover;
        Texture help_hover;
        Texture start_hover;

        Texture credits_down;
        Texture exit_down;
        Texture help_down;
        Texture start_down;


        Texture background;
        Texture linkbg;

        Texture cursor;
        Point mousePosition;


        RoundButton startButton;
        RoundButton exitButton;
        RoundButton creditButton;
        RoundButton helpButton;

        public MainMenu(Code2015 game, Menu parent)
        {
            RenderSystem rs = game.RenderSystem;

            this.game = game;
            this.parent = parent;

            FileLocation fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("mm_btn_credits.tex", GameFileLocs.GUI);
            credits = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_quit.tex", GameFileLocs.GUI);
            exit = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_help.tex", GameFileLocs.GUI);
            help = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_single.tex", GameFileLocs.GUI);
            start = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("mm_btn_credits_hover.tex", GameFileLocs.GUI);
            credits_hover = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_quit_hover.tex", GameFileLocs.GUI);
            exit_hover = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_help_hover.tex", GameFileLocs.GUI);
            help_hover = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_single_hover.tex", GameFileLocs.GUI);
            start_hover = UITextureManager.Instance.CreateInstance(fl);



            fl = FileSystem.Instance.Locate("mm_btn_credits_down.tex", GameFileLocs.GUI);
            credits_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_quit_down.tex", GameFileLocs.GUI);
            exit_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_help_down.tex", GameFileLocs.GUI);
            help_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_single_down.tex", GameFileLocs.GUI);
            start_down = UITextureManager.Instance.CreateInstance(fl);



            fl = FileSystem.Instance.Locate("mm_start_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("mm_start_link.tex", GameFileLocs.GUI);
            linkbg = UITextureManager.Instance.CreateInstance(fl);

            #region 配置按钮
            startButton = new RoundButton();
            startButton.X = 663;
            startButton.Y = 48;
            startButton.Radius = 244 / 2;
            startButton.Enabled = true;
            startButton.IsValid = true;

            startButton.MouseClick += StartButton_Click;

            exitButton = new RoundButton();
            exitButton.X = 1061;
            exitButton.Y = 554;
            exitButton.Radius = 106 / 2;
            exitButton.Enabled = true;
            exitButton.IsValid = true;

            exitButton.MouseClick += ExitButton_Click;


            creditButton = new RoundButton();
            creditButton.X = 901;
            creditButton.Y = 357;
            creditButton.Radius = 138 / 2;
            creditButton.Enabled = true;
            creditButton.IsValid = true;


            helpButton = new RoundButton();
            helpButton.X = 1031;
            helpButton.Y = 182;
            helpButton.Radius = 138 / 2;
            helpButton.Enabled = true;
            helpButton.IsValid = true;
            #endregion
            CreateScene(rs);
        }
        void CreateScene(RenderSystem rs)
        {
            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctreeSceneManager(new OctreeBox(PlanetEarth.PlanetRadius * 4f), PlanetEarth.PlanetRadius / 75f);
            sm.PostRenderer = new BloomPostRenderer(rs);
            sm.UseShadow = true;

            FpsCamera camera = new FpsCamera(Program.ScreenWidth / (float)Program.ScreenHeight);

         
            
            camera.RenderTarget = rs.GetRenderTarget(0);

            renderer = new SceneRenderer(rs, sm);
            renderer.RegisterCamera(camera);
        }
        void ExitButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                game.Exit();
            }
        }
        void StartButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                parent.CurrentScreen = parent.GetSelectScreen();
                //GameCreationParameters gcp = new GameCreationParameters();
                //gcp.Player1 = new Player("test");
                //gcp.Player1.SideColor = ColorValue.Red;
                //game.StartNewGame(gcp);
            }
        }


        public void Render()
        {
            renderer.RenderScene();
        }

        public override void Render(Sprite sprite)
        {
            //sprite.SetTransform(Matrix.Identity);


            sprite.SetTransform(Matrix.Identity);

            sprite.Draw(background, 0, 0, ColorValue.White);
            sprite.Draw(linkbg, 0, 0, ColorValue.White);


            int x = 818 - 322 / 2;
            int y = 158 - 281 / 2;


            if (startButton.IsPressed)
            {
                sprite.Draw(start_down, x, y, ColorValue.White);
            }
            else if (startButton.IsMouseOver)
            {
                sprite.Draw(start_hover, x, y, ColorValue.White);
            }
            else
            {
                sprite.Draw(start, x, y, ColorValue.White);
            }

            x = 1107 - 192 / 2;
            y = 241 - 195 / 2;


            if (helpButton.IsPressed)
            {
                sprite.Draw(help_down, x, y, ColorValue.White);
            }
            else if (helpButton.IsMouseOver)
            {
                sprite.Draw(help_hover, x, y, ColorValue.White);
            }
            else
            {
                sprite.Draw(help, x, y, ColorValue.White);
            }

            x = 835;// -225 / 2;
            y = 336;// -180 / 2;

            if (creditButton.IsPressed)
            {
                sprite.Draw(credits_down, x, y, ColorValue.White);
            }
            else if (creditButton.IsMouseOver)
            {
                sprite.Draw(credits_hover, x, y, ColorValue.White);
            }
            else
            {
                sprite.Draw(credits, x, y, ColorValue.White);
            }

            x = 1129 - 160 / 2;
            y = 594 - 160 / 2;


            if (exitButton.IsPressed)
            {
                sprite.Draw(exit_down, x, y, ColorValue.White);
            }
            else if (exitButton.IsMouseOver)
            {
                sprite.Draw(exit_hover, x, y, ColorValue.White);
            }
            else
            {
                sprite.Draw(exit, x, y, ColorValue.White);
            }

            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);


        }
        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;
            fps = time.FramesPerSecond;


            startButton.Update(time);
            creditButton.Update(time);
            exitButton.Update(time);
            helpButton.Update(time);
            fps = time.FramesPerSecond;
        }
    }
}
