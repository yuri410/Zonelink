using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <summary>
        ///  0级LOD地形
        /// </summary>
        ResourceHandle<TerrainMesh> terrain;
        /// <summary>
        ///  1级LOD地形
        /// </summary>
        ResourceHandle<TerrainMesh> terrain1;
        ///// <summary>
        /////  2级LOD地形
        ///// </summary>
        //ResourceHandle<TerrainMesh> terrain2;
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
            terrain = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 0);
            terrain1 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 1);
            terrain1.Touch();
            //terrain2 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 2);
            //terrain2.Touch();
            //terrain3 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 3);

            Transformation = terrain1.GetWeakResource().Transformation;
            BoundingSphere = terrain1.GetWeakResource().BoundingSphere;

            terrain.GetWeakResource().ObjectSpaceChanged += TerrainMesh_ObjectSpaceChanged;
            terrain1.GetWeakResource().ObjectSpaceChanged += TerrainMesh_ObjectSpaceChanged;
        }


        void TerrainMesh_ObjectSpaceChanged(Matrix mat, BoundingSphere bs)
        {
            if (Transformation != mat)
            {
                Transformation = mat;
                RequiresUpdate = true;
            }
            if (BoundingSphere != bs)
            {
                BoundingSphere = bs;
                RequiresUpdate = true;
            }
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
            switch (level)
            {
                case 0:
                case 1:
                    if (terrain.State == ResourceState.Loaded)
                    {
                        ActiveTerrain = terrain;
                    }
                    else
                    {
                        terrain.Touch();
                    }
                    break;
                case 2:
                case 3:
                    if (terrain1.State == ResourceState.Loaded)
                    {
                        ActiveTerrain = terrain1;
                    }
                    else
                    {
                        terrain1.Touch();
                    }
                    break;
                 
                    //if (terrain2.State == ResourceState.Loaded)
                    //{
                    //    ActiveTerrain = terrain2;
                    //}
                    //else
                    //{
                    //    terrain2.Touch();
                    //}
                    //break;
                
                default:
                    ActiveTerrain = null;
                    break;
            }

            if (ActiveTerrain != null)
            {
                TerrainMesh tm = ActiveTerrain.Resource;

                if (tm.State == ResourceState.Loaded)
                {
                    if (Transformation != tm.Transformation)
                    {
                        Transformation = tm.Transformation;
                        RequiresUpdate = true;
                    }
                    if (BoundingSphere != tm.BoundingSphere)
                    {
                        BoundingSphere = tm.BoundingSphere;
                        RequiresUpdate = true;
                    }
                }

                tm.PrepareVisibleObjects(cam, level);
            }
        }

        public override void Update(GameTime dt)
        {

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
                terrain.GetWeakResource().ObjectSpaceChanged -= TerrainMesh_ObjectSpaceChanged;
                terrain1.GetWeakResource().ObjectSpaceChanged -= TerrainMesh_ObjectSpaceChanged;

                terrain.Dispose();
                terrain1.Dispose();
                //terrain2.Dispose();
            }
            terrain = null;
            terrain1 = null;
            //terrain2 = null;
        }
    }
}
