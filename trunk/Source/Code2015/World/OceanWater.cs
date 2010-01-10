using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
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
        OceanWaterTile[] waterTiles;
        OceanWaterDataManager dataMgr;

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

            dataMgr = new OceanWaterDataManager(rs);


            waterTiles = new OceanWaterTile[PlanetEarth.ColTileCount * PlanetEarth.LatTileCount];

            for (int i = 1, index = 0; i < PlanetEarth.ColTileCount * 2; i += 2)
            {
                for (int j = 1; j < PlanetEarth.LatTileCount * 2; j += 2)
                {
                    waterTiles[index++] = new OceanWaterTile(rs, dataMgr, i, j);
                }
            }

            oceanSphere = new Sphere(rs, PlanetEarth.PlanetRadius + TerrainMeshManager.PostZeroLevel * 0.5f, 
                PlanetEarth.ColTileCount * 4, PlanetEarth.LatTileCount * 4, mats);

            base.ModelL0 = oceanSphere;

            BoundingSphere.Radius = PlanetEarth.PlanetRadius;
        }

        public override void OnAddedToScene(object sender, SceneManagerBase sceneMgr)
        {
            base.OnAddedToScene(sender, sceneMgr);

            for (int i = 0; i < waterTiles.Length; i++)
            {
                sceneMgr.AddObjectToScene(waterTiles[i]);
            }
        }
        public override void OnRemovedFromScene(object sender, SceneManagerBase sceneMgr)
        {
            base.OnRemovedFromScene(sender, sceneMgr);
            for (int i = 0; i < waterTiles.Length; i++)
            {
                sceneMgr.RemoveObjectFromScene(waterTiles[i]);
            }
        }

        public override RenderOperation[] GetRenderOperation()
        {
            return base.GetRenderOperation();
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            return base.GetRenderOperation(level);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            waterTiles = null;
        }
    }
}
