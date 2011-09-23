/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
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

        FastList<City> visibleList = new FastList<City>();
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

            Viewport vp = rs.Viewport;
            float aspectRatio = vp.Width / (float)vp.Height;
            camera = new RtsCamera(42.5f, aspectRatio);


            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctplSceneManager(PlanetEarth.PlanetRadius);
            sm.UseShadow = true;
            sm.PostRenderer = new GamePostRenderer(renderSys, camera);




            renderer = new SceneRenderer(renderSys, sm);
            renderer.ClearColor = ColorValue.White;


            camera.NearPlane = 20;
            camera.FarPlane = 6000;
            camera.Mode = RenderMode.Final;
            camera.RenderTarget = renderSys.GetRenderTarget(0);

            renderer.RegisterCamera(camera);



            PlanetEarth earth = new PlanetEarth(renderSys);
            sm.SceneManager.AddObjectToScene(earth);

            OceanWater water = new OceanWater(renderSys);
            sm.SceneManager.AddObjectToScene(water);

            //Atmosphere atmos = new Atmosphere(renderSys);
            //sm.SceneManager.AddObjectToScene(atmos);

        }

        internal void City_Visible(City obj) 
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
        public City GetVisibleCity(int i)
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
            renderer.ActivateVisibles();
        }
    }
}
