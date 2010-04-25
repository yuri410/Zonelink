using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    class LightningStrom : SceneObject
    {
        Disaster disaster;
        FastList<Cloud> clouds = new FastList<Cloud>();

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        SceneManagerBase sceMgr;

        const float CloudHeight = 7500;

        public LightningStrom(RenderSystem rs, Disaster disaster)
            : base(false)
        {
            this.disaster = disaster;
            float radlng = MathEx.Degree2Radian(disaster.Longitude);
            float radlat = MathEx.Degree2Radian(disaster.Latitude);
            float radr = MathEx.Degree2Radian(disaster.Radius);

            int cloudCount = (int)(disaster.Damage * radr * 0.3f);
            for (int i = 0; i < cloudCount; i++)
            {
                Cloud cld = new Cloud(rs, Randomizer.GetRandomSingle() * disaster.Duration);

                float rnd1 = Randomizer.GetRandomSingle();
                float rnd2 = Randomizer.GetRandomSingle();

                float r = rnd1 * radr;
                float rr = PlanetEarth.GetTileArcLength(r);
                float rt = rnd2 * MathEx.PIf * 2;

                float rotCos = (float)Math.Cos(rt);
                float rotSin = (float)Math.Sin(rt);

                float cldLng = radlng + r * rotCos;
                float cldLat = radlat + r * rotSin;
                float alt = TerrainData.Instance.QueryHeight(cldLng, cldLat);
                if (alt < 0)
                    alt = 0;
                alt += CloudHeight;

                Matrix trans = PlanetEarth.GetOrientation(cldLng, cldLat);
                trans.TranslationValue = PlanetEarth.GetPosition(cldLng, cldLat, PlanetEarth.PlanetRadius + alt * TerrainMeshManager.PostHeightScale);

                cld.Transform = trans;

                clouds.Add(cld);
            }

            Transformation = Matrix.Identity;

            this.BoundingSphere.Center = PlanetEarth.GetPosition(radlng, radlat);
            this.BoundingSphere.Radius = PlanetEarth.GetTileHeight(radr);
        }

        public override RenderOperation[] GetRenderOperation()
        {
            opBuffer.Clear();

            for (int i = 0; i < clouds.Count; i++)
            {
                RenderOperation[] ops = clouds[i].GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }

                ops = clouds[i].GetRenderOperation2();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }

            }

            return opBuffer.Elements;
        }

        public override void Update(GameTime dt)
        {
            for (int i = 0; i < clouds.Count; i++)
            {
                clouds[i].Update(dt);
            }
            if (sceMgr != null && disaster.IsOver)
            {
                sceMgr.RemoveObjectFromScene(this);
            }
        }

        public override bool IsSerializable
        {
            get { return false; }
        }

        public override void OnAddedToScene(object sender, SceneManagerBase sceneMgr)
        {
            this.sceMgr = sceneMgr;
        }


    }
}
