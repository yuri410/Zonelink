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

        const float CloudHeight = 750;

        public LightningStrom(RenderSystem rs, Disaster disaster)
            : base(false)
        {
            this.disaster = disaster;
            float radlng = MathEx.Degree2Radian(disaster.Longitude);
            float radlat = MathEx.Degree2Radian(disaster.Latitude);

            int cloudCount = (int)(disaster.Damage * disaster.Radius);
            for (int i = 0; i < cloudCount; i++)
            {
                Cloud cld = new Cloud(rs);

                float rnd1 = Randomizer.GetRandomSingle();
                float rnd2 = Randomizer.GetRandomSingle();

                float r = rnd1 * disaster.Radius;
                float rr = PlanetEarth.GetTileArcLength(r);
                float rt = rnd2 * MathEx.PIf * 2;

                float rotCos = (float)Math.Cos(rt);
                float rotSin = (float)Math.Sin(rt);

                float cldLng = radlng + r * rotCos;
                float cldLat = radlat + r * rotSin;
                float alt = TerrainData.Instance.QueryHeight(cldLng, cldLat) + CloudHeight;

                Matrix trans = PlanetEarth.GetOrientation(cldLng, cldLat);
                trans.TranslationValue = PlanetEarth.GetPosition(cldLng, cldLat, alt);

                cld.Transform = trans;

                clouds.Add(cld);
            }
        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
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
