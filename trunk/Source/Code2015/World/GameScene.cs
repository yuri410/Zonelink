using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
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
        FastList<IResourceObject> visibleResource = new FastList<IResourceObject>();

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
            sm.UseShadow = true;
            sm.PostRenderer = new BloomPostRenderer(renderSys);




            renderer = new SceneRenderer(renderSys, sm);

            Viewport vp = rs.Viewport;
            float aspectRatio = vp.Width / (float)vp.Height;

            camera = new RtsCamera(45, aspectRatio);
            camera.NearPlane = 20;
            camera.FarPlane = 11000;
            camera.Mode = RenderMode.Final;
            camera.RenderTarget = renderSys.GetRenderTarget(0);

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

        internal void Resource_Visible(IResourceObject obj)
        {
            visibleResource.Add(obj);
        }
        public int VisibleCityCount 
        {
            get { return visibleList.Count; }
        }
        public CityObject GetVisibleCity(int i)
        {
            return visibleList[i];
        }

        public int VisibleResourceCount
        {
            get { return visibleResource.Count; }
        }
        public IResourceObject GetResourceObject(int i) 
        {
            return visibleResource[i];
        }


        public void Update(GameTime time)
        {
            EffectParams.InvView = Matrix.Invert(camera.ViewMatrix);
            //Matrix view = camera.ViewMatrix;

            //view = Matrix.RotationY(MathEx.PiOver4) * view;
            Matrix view = Matrix.RotationY(-MathEx.PIf / 6) * EffectParams.InvView;
            EffectParams.LightDir = -view.Forward;

            renderer.Update(time);
        }

        public void RenderScene()
        {
            visibleList.Clear();
            visibleResource.Clear();



            renderer.RenderScene();
        }

        public void ActivateVisibles()
        {

        }
    }
}
