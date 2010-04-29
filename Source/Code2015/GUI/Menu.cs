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
using Apoc3D.Graphics.Geometry;
using Code2015.Effects;
using Apoc3D.Core;

namespace Code2015.GUI
{

    /// <summary>
    ///  表示游戏菜单
    /// </summary>
    class Menu : UIComponent, IGameComponent
    {
        const float RotSpeed = 3;
        Vector3 RotAxis = new Vector3(2505.168f, 4325.199f, 4029.689f);
        float angle;
        float rotScale;

        class MenuScene : StaticModelObject
        {

            public MenuScene(RenderSystem rs)
            {
                FileLocation fl = FileSystem.Instance.Locate("start.mesh", GameFileLocs.Model);

                ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                BoundingSphere.Radius = PlanetEarth.PlanetRadius;
                Transformation = Matrix.Identity;

            }

            public override bool IsSerializable
            {
                get { return false; }
            }

            public override RenderOperation[] GetRenderOperation()
            {
                return base.GetRenderOperation();
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

                //x->z y->x z->y 
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
            public override float GetSMScale()
            {
                return 20;
            }
            public override Matrix GetSMTrans()
            {
                //Vector3 pos = new Vector3(-10799.082f, -3815.834f, 6951.33f);
                Vector3 pos = new Vector3(-5009.926f, 8066.071f, -16341.605f);//-6522.938f, 8066.071f, -12065.895f);// new Vector3(-3815.834f, 6951.33f, -10799.082f);
                Vector3 target = new Vector3(-2578.986f, 4845.344f, -2702.878f);// new Vector3(-3151.209f, 6214.899f, 325.246f);//-2702.878  -2578.986  4845.344


                return Matrix.LookAtRH(pos, target, Vector3.UnitY);
            }
        }
        class MenuWater : StaticModelObject
        {
            Sphere oceanSphere;

            public MenuWater(RenderSystem renderSys) 
            {

                Material[][] mats = new Material[1][];
                mats[0] = new Material[1];
                mats[0][0] = new Material(renderSys);
                
                FileLocation fl = FileSystem.Instance.Locate("WaterNormal.tex", GameFileLocs.Nature);
                ResourceHandle<Texture> map = TextureManager.Instance.CreateInstance(fl);
                mats[0][0].SetTexture(1, map);

                fl = FileSystem.Instance.Locate("WaterDudv.tex", GameFileLocs.Nature);
                map = TextureManager.Instance.CreateInstance(fl);
                mats[0][0].SetTexture(0, map);

                mats[0][0].SetEffect(EffectManager.Instance.GetModelEffect(MMWaterEffectFactory.Name));
                mats[0][0].IsTransparent = true;
                mats[0][0].ZWriteEnabled = false;
                mats[0][0].ZEnabled = true;
                mats[0][0].CullMode = CullMode.CounterClockwise;
                mats[0][0].PriorityHint = RenderPriority.Third;


                oceanSphere = new Sphere(renderSys, PlanetEarth.PlanetRadius + 15,
                    PlanetEarth.ColTileCount * 4, PlanetEarth.LatTileCount * 4, mats);

                base.ModelL0 = oceanSphere;

                BoundingSphere.Radius = PlanetEarth.PlanetRadius;
            }

            public override bool IsSerializable
            {
                get { return false; }
            }
        }

        RenderSystem renderSys;
        SceneRenderer renderer;
        MenuScene earth;
        MenuWater water;

        Intro intro;
        LoadingScreen loadScreen;
        ScoreScreen scoreScreen;
        CreditScreen credits;

        Code2015 game;
        MainMenu mainMenu;
        SelectScreen sideSelect;
        RenderTarget renderTarget;

        public UIComponent CurrentScreen
        {
            get;
            set;
        }
        public CreditScreen GetCredits() 
        {
            return credits;
        }
        public MainMenu GetMainMenu()
        {
            return mainMenu;
        }
        public SelectScreen GetSelectScreen()
        {
            return sideSelect;
        }

        public Texture Earth 
        {
            get { return renderTarget.GetColorBufferTexture(); }
        }

        public Menu(Code2015 game, RenderSystem rs)
        {
            this.game = game;
            this.mainMenu = new MainMenu(game, this);
            this.sideSelect = new SelectScreen(game, this);
            this.renderSys = rs;

            CreateScene(rs);
            this.loadScreen = new LoadingScreen(this, rs);
            this.intro = new Intro(rs);
            this.credits = new CreditScreen(rs, this);

            this.CurrentScreen = mainMenu;
        }

        void CreateScene(RenderSystem rs)
        {
            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctreeSceneManager(new OctreeBox(PlanetEarth.PlanetRadius * 4f), PlanetEarth.PlanetRadius / 75f);
            sm.PostRenderer = new BloomPostRenderer(rs);
            sm.UseShadow = true;

            MenuCamera camera = new MenuCamera(Program.ScreenWidth / (float)Program.ScreenHeight);

            renderTarget = rs.ObjectFactory.CreateRenderTarget(Program.ScreenWidth, Program.ScreenHeight, Apoc3D.Media.ImagePixelFormat.A8R8G8B8);

            camera.RenderTarget = renderTarget;
            renderer = new SceneRenderer(rs, sm);
            renderer.ClearColor = ColorValue.TransparentWhite;
            renderer.RegisterCamera(camera);


            renderer.ClearScreen = true;

            earth = new MenuScene(rs);
            renderer.SceneManager.AddObjectToScene(earth);

            water = new MenuWater(rs);
            renderer.SceneManager.AddObjectToScene(water);
          
            
            RotAxis.Normalize();
        }

        public void Render()
        {
            
            if (!game.IsIngame)
            {
                if (CurrentScreen == mainMenu || CurrentScreen == loadScreen || CurrentScreen == scoreScreen)
                {
                    EffectParams.LightDir = -renderer.CurrentCamera.Front;

                    renderer.RenderScene();
                }
            }
            else 
            {
                if (!game.CurrentGame.IsLoaded)
                {
                    EffectParams.LightDir = -renderer.CurrentCamera.Front;

                    renderer.RenderScene();
                }
            }
        }


        void ShowScore(ScoreEntry[] entries)
        {
            scoreScreen = new ScoreScreen(game, this);
            for (int i = 0; i < entries.Length; i++)
            {
                scoreScreen.Add(entries[i]);
            }
            CurrentScreen = scoreScreen;
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
            else
            {
                if (!game.CurrentGame.IsLoaded && loadScreen != null)
                {
                    loadScreen.Progress = game.CurrentGame.LoadingProgress;
                    loadScreen.Render(sprite);
                }
            }

            if (intro != null)
            {
                intro.Render(sprite);
            }
        }
        void UpdateScene(GameTime time)
        {
            #region 地球
            if (angle > MathEx.Degree2Radian(140) && angle < MathEx.Degree2Radian(250))
            {
                rotScale += 0.01f;

                if (rotScale > 3.5f)
                    rotScale = 3.5f;
            }
            else if (rotScale < 1)
            {
                rotScale += 0.01f;
            }
            else
            {
                rotScale -= 0.01f;
                //if (rotScale < 1)
                //    rotScale = 1;
            }
            angle += MathEx.Degree2Radian(RotSpeed * time.ElapsedGameTimeSeconds) * rotScale;

            if (angle > MathEx.PIf * 2)
                angle -= MathEx.PIf * 2;


            Matrix rot = Matrix.RotationAxis(RotAxis, -angle);
            earth.Transformation = rot;
            water.Transformation = rot;


            renderer.Update(time);

            #endregion
        }
        public override void Update(GameTime time)
        {
            if (intro != null)
            {
                intro.Update(time);
                if (intro.IsOver)
                {
                    intro.Dispose();
                    intro = null;
                }
            }

            if (!game.IsIngame)
            {
                if (CurrentScreen != null)
                {
                    UpdateScene(time);
                    CurrentScreen.Update(time);
                }
            }
            else
            {
                if (!game.CurrentGame.IsLoaded && loadScreen != null)
                {
                    UpdateScene(time);
                    loadScreen.Update(time);
                }
                if (game.CurrentGame.IsOver)
                {
                    if (scoreScreen == null)
                    {
                        ShowScore(game.CurrentGame.ResultScore);
                        game.Back();
                    }
                }
            }
        }
    }

}
