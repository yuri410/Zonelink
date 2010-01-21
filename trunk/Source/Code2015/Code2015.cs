using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;
using Code2015.World;
using X = Microsoft.Xna.Framework;
using XGS = Microsoft.Xna.Framework.GamerServices;
using XI = Microsoft.Xna.Framework.Input;
using XN = Microsoft.Xna.Framework.Net;

namespace Code2015
{
    /// <summary>
    ///  表示游戏。处理、分配各种操作例如渲染，逻辑等等。
    /// </summary>
    class Code2015 : IRenderWindowHandler
    {
        RenderSystem renderSys;

        List<TerrainTile> terrList = new List<TerrainTile>();
        FpsCamera camera;
        ReflectionCamera reflectionCamera;
        SceneRenderer renderer;
        RenderTarget reflectionRt;

        public Code2015(RenderSystem rs)
        {
            this.renderSys = rs;

        }

        #region IRenderWindowHandler 成员

        /// <summary>
        ///  处理游戏初始化操作，在游戏初始加载之前
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        ///  处理游戏初始加载资源工作
        /// </summary>
        public void Load()
        {
            ConfigurationManager.Initialize();
            ConfigurationManager.Instance.Register(new IniConfigurationFormat());

            EffectManager.Initialize(renderSys);
            EffectManager.Instance.RegisterModelEffectType(TerrainEffect513Factory.Name, new TerrainEffect513Factory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(TerrainEffect129Factory.Name, new TerrainEffect129Factory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(TerrainEffect33Factory.Name, new TerrainEffect33Factory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(WaterEffectFactory.Name, new WaterEffectFactory(renderSys));

            EffectManager.Instance.LoadEffects();

            TextureManager.Instance.Factory = renderSys.ObjectFactory;
            TerrainMaterialLibrary.Initialize(renderSys);

            FileLocation fl = FileSystem.Instance.Locate("terrainMaterial.ini", GameFileLocs.Config);

            TerrainMaterialLibrary.Instance.LoadTextureSet(fl);





            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctplSceneManager(PlanetEarth.PlanetRadius);
            sm.UseShadow = false;
            sm.PostRenderer = new DefaultPostRenderer();




            renderer = new SceneRenderer(renderSys, sm);
            //Viewport vp = renderSys.Viewport;
            //reflectionRt = renderSys.ObjectFactory.CreateRenderTarget(vp.Width, vp.Height, ImagePixelFormat.X8R8G8B8);





            camera = new FpsCamera(1);
            camera.Position = new Vector3(0, 0, -PlanetEarth.PlanetRadius - 1500);
            camera.NearPlane = 10;
            camera.FarPlane = 6000;
            camera.Mode = RenderMode.Final;
            //reflectionCamera = new ReflectionCamera(camera);
            //reflectionCamera.RenderTarget = reflectionRt;
            //WaterEffect.Reflection = reflectionRt;

            //renderer.RegisterCamera(reflectionCamera);
            renderer.RegisterCamera(camera);




            PlanetEarth earth = new PlanetEarth(renderSys);
            sm.SceneManager.AddObjectToScene(earth);

            OceanWater water = new OceanWater(renderSys);
            sm.SceneManager.AddObjectToScene(water);

            //camera.MoveSpeed = 50;
        }

        /// <summary>
        ///  处理游戏关闭时的释放资源工作
        /// </summary>
        public void Unload()
        {
        }

        /// <summary>
        ///  进行游戏逻辑帧中的处理
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            XI.KeyboardState state = XI.Keyboard.GetState();

            if (state.IsKeyDown(XI.Keys.W))
            {
                camera.MoveFront();
            }

            if (state.IsKeyDown(XI.Keys.A))
            {
                camera.MoveLeft();
            }
            if (state.IsKeyDown(XI.Keys.D))
            {
                camera.MoveRight();
            }
            if (state.IsKeyDown(XI.Keys.S))
            {
                camera.MoveBack();
            }

            if (state.IsKeyDown(XI.Keys.LeftShift))
            {
                camera.MoveSpeed = 50;
            }
            else
            {
                camera.MoveSpeed = 2;
            }

            if (state.IsKeyDown(XI.Keys.Space))
            {
                camera.MoveUp();
            }
            if (state.IsKeyDown(XI.Keys.LeftControl))
            {
                camera.MoveDown();
            }

            if (state.IsKeyDown(XI.Keys.Right))
            {
                camera.TurnRight();
            }
            if (state.IsKeyDown(XI.Keys.Left))
            {
                camera.TurnLeft();
            }
            if (state.IsKeyDown(XI.Keys.Up))
            {
                camera.TurnUp();
            }
            if (state.IsKeyDown(XI.Keys.Down))
            {
                camera.TurnDown();
            }

            renderer.Update(time);
        }

        /// <summary>
        ///  进行游戏图像渲染帧中的渲染操作
        /// </summary>
        public void Draw()
        {
            
            renderer.RenderScene();
        }

        #endregion
    }
}
