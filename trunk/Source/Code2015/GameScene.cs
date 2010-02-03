using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.EngineEx;
using Code2015.World;
using XI = Microsoft.Xna.Framework.Input;

namespace Code2015
{
    class GameScene
    {
        RenderSystem renderSys;

        List<TerrainTile> terrList = new List<TerrainTile>();
        RtsCamera camera;
        ReflectionCamera reflectionCamera;
        SceneRenderer renderer;
        RenderTarget reflectionRt;

        public GameScene(RenderSystem rs)
        {
            renderSys = rs;

            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctplSceneManager(PlanetEarth.PlanetRadius);
            sm.UseShadow = false;
            sm.PostRenderer = new DefaultPostRenderer();




            renderer = new SceneRenderer(renderSys, sm);


            camera = new RtsCamera(65, 1);
            camera.NearPlane = 10;
            camera.FarPlane = 6000;
            camera.Mode = RenderMode.Final;

            renderer.RegisterCamera(camera);



            PlanetEarth earth = new PlanetEarth(renderSys);
            sm.SceneManager.AddObjectToScene(earth);

            OceanWater water = new OceanWater(renderSys);
            sm.SceneManager.AddObjectToScene(water);

        }

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

            //if (state.IsKeyDown(XI.Keys.LeftShift))
            //{
            //    camera.MoveSpeed = 50;
            //}
            //else
            //{
            //    camera.MoveSpeed = 2;
            //}

            if (state.IsKeyDown(XI.Keys.Space))
            {
                camera.Height++;
            }
            if (state.IsKeyDown(XI.Keys.LeftControl))
            {
                camera.Height--;
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

            }
            if (state.IsKeyDown(XI.Keys.Down))
            {
            }

            renderer.Update(time);
        }

        public void RenderScene()
        {
            renderer.RenderScene();
        }

        public void ActivateVisibles()
        {

        }
    }
}
