using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{

    /// <summary>
    ///  表示游戏菜单
    /// </summary>
    class Menu : UIComponent, IGameComponent
    {
        class MenuScene : StaticModelObject
        {

            const float RotSpeed = 3;
            Vector3 RotAxis = new Vector3(2505.168f, 4325.199f, 4029.689f);
            float angle;
            
            public MenuScene(RenderSystem rs)
            {
                FileLocation fl = FileSystem.Instance.Locate("start.mesh", GameFileLocs.Model);

                ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                BoundingSphere.Radius = PlanetEarth.PlanetRadius;
                Transformation = Matrix.Identity;
                RotAxis.Normalize();

            }

            public override bool IsSerializable
            {
                get { return false; }
            }

            public override RenderOperation[] GetRenderOperation()
            {
                Matrix rot = Matrix.RotationAxis(RotAxis, angle);
                RenderOperation[] ops = base.GetRenderOperation();

                if (ops != null)
                {
                    for (int i = 0; i < ops.Length; i++)
                    {
                        ops[i].Transformation *= rot;
                    }
                }
                return ops;
            }

            public override void Update(GameTime dt)
            {
                base.Update(dt);

                angle -= MathEx.Degree2Radian(RotSpeed * dt.ElapsedGameTimeSeconds);
            }
        }

        class MenuCamera : Camera
        {
            public MenuCamera(float aspect)
                : base(aspect)
            {
                FieldOfView = 35;
                Position = new Vector3(-5260.516f, 6214.899f, -15371.574f);
                NearPlane = 100;
                FarPlane = 25000;
            }
            public override void UpdateProjection()
            {
                float fovy = MathEx.Degree2Radian(23.5f);
                NearPlaneHeight = (float)(Math.Tan(fovy * 0.5f)) * NearPlane * 2;
                NearPlaneWidth = NearPlaneHeight * AspectRatio;

                Frustum.proj = Matrix.PerspectiveRH(NearPlaneWidth, NearPlaneHeight, NearPlane, FarPlane);

            }
            public override void Update(GameTime time)
            {
                //UpdateProjection();

                Vector3 target = new Vector3(-3151.209f, 6214.899f, 325.246f);

                base.Update(time);
                Frustum.view = Matrix.LookAtRH(Position, target, Vector3.UnitY);
                Frustum.Update();

                orientation = Quaternion.RotationMatrix(Frustum.view);

                Matrix m = Matrix.Invert(Frustum.view);
                front = m.Forward;// MathEx.GetMatrixFront(ref m);
                top = m.Up;// MathEx.GetMatrixUp(ref m);
                right = m.Right;// MathEx.GetMatrixRight(ref m);
            }
        }

        SceneRenderer renderer;



        Code2015 game;
        MainMenu mainMenu;
        SelectScreen sideSelect;

        Texture cursor;
        Point mousePosition;

        public UIComponent CurrentScreen
        {
            get;
            set;
        }
        public MainMenu GetMainMenu()
        {
            return mainMenu;
        }
        public SelectScreen GetSelectScreen()
        {
            return sideSelect;
        }

        public Menu(Code2015 game, RenderSystem rs)
        {
            this.game = game;
            this.mainMenu = new MainMenu(game, this);
            this.sideSelect = new SelectScreen(game, this);

            this.CurrentScreen = mainMenu;
            CreateScene(rs);
            FileLocation fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);
        }

        void CreateScene(RenderSystem rs)
        {
            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctreeSceneManager(new OctreeBox(PlanetEarth.PlanetRadius * 4f), PlanetEarth.PlanetRadius / 75f);
            sm.PostRenderer = new BloomPostRenderer(rs);
            sm.UseShadow = true;

            MenuCamera camera = new MenuCamera(Program.ScreenWidth / (float)Program.ScreenHeight);

            camera.RenderTarget = rs.GetRenderTarget(0);

            renderer = new SceneRenderer(rs, sm);
            renderer.RegisterCamera(camera);


            renderer.ClearScreen = false;
            MenuScene obj = new MenuScene(rs);
            renderer.SceneManager.AddObjectToScene(obj);
        }

        public void Render()
        {
            if (!game.IsIngame)
            {
                if (CurrentScreen == mainMenu)
                {
                    //renderer.RenderScene();
                }
            }
        }
        public void RenderCursor(Sprite sprite) 
        {
            if (!game.IsIngame)
            {
                if (CurrentScreen != null)
                {
                    sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
                }
            }
        }

        public override void Render(Sprite sprite)
        {
            if (!game.IsIngame)
            {
                if (CurrentScreen != null)
                {
                    CurrentScreen.Render(sprite);
                }
            }
        }
        public override void Update(GameTime time)
        {
            if (!game.IsIngame)
            {
                mousePosition.X = MouseInput.X;
                mousePosition.Y = MouseInput.Y; 
            
                renderer.Update(time);

                if (CurrentScreen != null)
                {
                    CurrentScreen.Update(time);

                    EffectParams.LightDir = -renderer.CurrentCamera.Front;
                }
            }
        }
    }

}
