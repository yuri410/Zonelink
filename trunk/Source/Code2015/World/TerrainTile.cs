using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Apoc3D.MathLib;

namespace Code2015.World
{
    class TerrainTile : SceneObject
    {
        ResourceHandle<TerrainMesh> terrain;

        public TerrainTile(RenderSystem rs, int col, int lat)
            : base(true)
        {
            terrain = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 0);
            BoundingSphere.Radius = float.MaxValue;           
        }

        public override RenderOperation[] GetRenderOperation()
        {
            return terrain.Resource.GetRenderOperation();
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        public override void PrepareVisibleObjects(ICamera cam)
        {
            terrain.Resource.PrepareVisibleObjects(cam);
        }

        public override void Update(float dt)
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
