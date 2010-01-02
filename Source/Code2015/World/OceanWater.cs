using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics.Geometry;
using Apoc3D.Scene;
using Code2015.Effects;
using Code2015.EngineEx;

namespace Code2015.World
{
    class OceanWater : StaticModelObject
    {
        RenderSystem renderSys;
        Sphere oceanSphere;

        public override bool IsSerializable
        {
            get { return false; }
        }

        public OceanWater(RenderSystem rs)
            : base(false)
        {
            renderSys = rs;

            Material[][] mats = new Material[1][];
            mats[0] = new Material[1];
            mats[0][0] = new Material(renderSys);

            //mats[0][0].SetTexture(0, TerrainMaterialLibrary.Instance.GlobalIndexTexture);
            //mats[0][0].SetTexture(0, TerrainMaterialLibrary.Instance.GlobalIndexTexture);
            
            mats[0][0].SetEffect(EffectManager.Instance.GetModelEffect(WaterEffectFactory.Name));
            oceanSphere = new Sphere(rs, PlanetEarth.PlanetRadius + TerrainMeshManager.PostZeroLevel * 0.5f, 4 * PlanetEarth.ColTileCount, 4 * PlanetEarth.LatTileCount, mats);

            base.ModelL0 = oceanSphere;

            BoundingSphere.Radius = PlanetEarth.PlanetRadius;
        }
        public override RenderOperation[] GetRenderOperation()
        {
            return base.GetRenderOperation();
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            return base.GetRenderOperation(level);
        }
    }
}
