using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.EngineEx;

namespace Code2015.World
{
    /// <summary>
    ///  表示地球上的一个地形块
    /// </summary>
    class TerrainTile : SceneObject
    {
        ResourceHandle<TerrainMesh> terrain;
        //ResourceHandle<TerrainMesh> terrain3;

        ResourceHandle<TerrainMesh> activeTerrain;

        ResourceHandle<TerrainMesh> ActiveTerrain
        {
            get { return activeTerrain; }
            set { activeTerrain = value; }
        }

        public TerrainTile(RenderSystem rs, int col, int lat)
            : base(true)
        {
            terrain = TerrainMeshManager.Instance.CreateInstance(rs, col, lat);
            terrain.Touch();
            //terrain3 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 3);

            Transformation = terrain.GetWeakResource().Transformation;
            BoundingSphere = terrain.GetWeakResource().BoundingSphere;

            //Transformation = terrain0.GetWeakResource().Transformation;
            //BoundingSphere = terrain0.GetWeakResource().BoundingSphere;

            //terrain0.GetWeakResource().ObjectSpaceChanged += TerrainMesh_ObjectSpaceChanged;
            //terrain1.GetWeakResource().ObjectSpaceChanged += TerrainMesh_ObjectSpaceChanged;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ActiveTerrain != null)
                return ActiveTerrain.Resource.GetRenderOperation();
            return null;
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            if (ActiveTerrain != null)
                return ActiveTerrain.Resource.GetRenderOperation();
            return null;
        }

        public override void PrepareVisibleObjects(ICamera cam, int level)
        {
            if (terrain.State == ResourceState.Loaded)
            {
                ActiveTerrain = terrain;
            }
            else
            {
                terrain.Touch();
            }

            if (ActiveTerrain != null)
            {
                TerrainMesh tm = ActiveTerrain.Resource;

                tm.PrepareVisibleObjects(cam, level);
            }
        }

        public override void Update(GameTime dt)
        {
            BoundingSphere = terrain.GetWeakResource().BoundingSphere;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                terrain.Dispose();
                //terrain2.Dispose();
            }
            terrain = null;
            //terrain2 = null;
        }
    }
}
