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


        public TerrainTile(RenderSystem rs, int col, int lat)
            : base(true)
        {
            terrain = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 0);
            terrain1 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 1);
            terrain2 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 2);
            BoundingSphere.Radius = float.MaxValue;

            Transformation = Matrix.Identity;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            return terrain.Resource.GetRenderOperation();
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            //switch (level)
            //{
                //case 0:
                    return terrain.Resource.GetRenderOperation();
            //    case 1:
            //        return terrain1.Resource.GetRenderOperation();
            //    case 2:
            //        return terrain2.Resource.GetRenderOperation();
            //}
            //return null;
            
        }

        public override void PrepareVisibleObjects(ICamera cam, int level)
        {
            //switch (level)
            //{
            //    case 0:
                    terrain.Resource.PrepareVisibleObjects(cam, level);
            //        break;
            //    case 1:
            //        terrain1.Resource.PrepareVisibleObjects(cam, level);
            //        break;
            //    case 2:
            //        terrain2.Resource.PrepareVisibleObjects(cam, level);
            //        break;
            //}
        }

        public override void Update(GameTime dt)
        {
            TerrainMesh tm = terrain.Resource;
            Transformation = tm.Transformation;
            BoundingSphere = tm.BoundingSphere;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
