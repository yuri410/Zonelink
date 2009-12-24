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
        /// <summary>
        ///  2级LOD地形
        /// </summary>
        ResourceHandle<TerrainMesh> terrain2;

        ResourceHandle<TerrainMesh> activeTerrain;

        ResourceHandle<TerrainMesh> ActiveTerrain
        {
            get { return activeTerrain; }
            set
            {
                activeTerrain = value;
                if (activeTerrain != null)
                {
                    TerrainMesh tm = activeTerrain.Resource;
                    Transformation = tm.Transformation;
                    RequiresUpdate = true;
                    BoundingSphere = tm.BoundingSphere;
                }
            }
        }
        public TerrainTile(RenderSystem rs, int col, int lat)
            : base(true)
        {
            terrain = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 0);
            terrain1 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 1);
            terrain2 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 2);
            BoundingSphere.Radius = 0;

            Transformation = new Matrix();
        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ActiveTerrain != null)
                return ActiveTerrain.Resource.GetRenderOperation();
            return null;
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            switch (level)
            {
                case 0:
                    ActiveTerrain = terrain;
                    break;
                case 1:
                    ActiveTerrain = terrain1;
                    break;
                case 2:
                    ActiveTerrain = terrain2;
                    break;
                default:
                    ActiveTerrain = null;
                    break;
            }
            if (ActiveTerrain != null)
                return ActiveTerrain.Resource.GetRenderOperation();
            return null;
            
        }

        public override void PrepareVisibleObjects(ICamera cam, int level)
        {
            switch (level)
            {
                case 0:
                    ActiveTerrain = terrain;
                    break;
                case 1:
                    ActiveTerrain = terrain1;
                    break;
                case 2:
                    ActiveTerrain = terrain2;
                    break;
                default:
                    ActiveTerrain = null;
                    break;
            }
            if (ActiveTerrain != null)
                ActiveTerrain.Resource.PrepareVisibleObjects(cam, level);
        }

        public override void Update(GameTime dt)
        {
           
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
