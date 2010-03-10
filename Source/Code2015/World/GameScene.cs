using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015.World
{
    class GameScene
    {
        RenderSystem renderSys;

        List<TerrainTile> terrList = new List<TerrainTile>();
        RtsCamera camera;
        SceneRenderer renderer;

        FastList<CityObject> visibleList = new FastList<CityObject>();

        public RtsCamera Camera
        {
            get { return camera; }
        }

        public SceneManagerBase Scene
        {
            get { return renderer.SceneManager; }
        }

        public GameScene(RenderSystem rs)
        {
            renderSys = rs;

            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctplSceneManager(PlanetEarth.PlanetRadius);
            sm.UseShadow = false;
            sm.PostRenderer = new DefaultPostRenderer();




            renderer = new SceneRenderer(renderSys, sm);

            Viewport vp = rs.Viewport;
            float aspectRatio = vp.Width / (float)vp.Height;

            camera = new RtsCamera(65, aspectRatio);
            camera.NearPlane = 10;
            camera.FarPlane = 8000;
            camera.Mode = RenderMode.Final;
            camera.Longitude = MathEx.Degree2Radian(114);
            camera.Latitude = MathEx.Degree2Radian(35);

            renderer.RegisterCamera(camera);



            PlanetEarth earth = new PlanetEarth(renderSys);
            sm.SceneManager.AddObjectToScene(earth);

            OceanWater water = new OceanWater(renderSys);
            sm.SceneManager.AddObjectToScene(water);

            Atmosphere atmos = new Atmosphere(renderSys);
            sm.SceneManager.AddObjectToScene(atmos);
        }

        internal void City_Visible(CityObject obj) 
        {
            visibleList.Add(obj);
        }

        public int VisibleCityCount 
        {
            get { return visibleList.Count; }
        }
        public CityObject GetVisibleCity(int i)
        {
            return visibleList[i];
        }


        public void Update(GameTime time)
        {
            visibleList.Clear();
            //XI.KeyboardState state = XI.Keyboard.GetState();

            //if (state.IsKeyDown(XI.Keys.W))
            //{
            //    camera.MoveFront();
            //}

            //if (state.IsKeyDown(XI.Keys.A))
            //{
            //    camera.MoveLeft();
            //}
            //if (state.IsKeyDown(XI.Keys.D))
            //{
            //    camera.MoveRight();
            //}
            //if (state.IsKeyDown(XI.Keys.S))
            //{
            //    camera.MoveBack();
            //}


            //if (state.IsKeyDown(XI.Keys.Space))
            //{
            //    camera.Height++;
            //}
            //if (state.IsKeyDown(XI.Keys.LeftControl))
            //{
            //    camera.Height--;
            //}

            //if (state.IsKeyDown(XI.Keys.Right))
            //{
            //    camera.TurnRight();
            //}
            //if (state.IsKeyDown(XI.Keys.Left))
            //{
            //    camera.TurnLeft();
            //}
            //if (state.IsKeyDown(XI.Keys.Up))
            //{

            //}
            //if (state.IsKeyDown(XI.Keys.Down))
            //{
            //}

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
