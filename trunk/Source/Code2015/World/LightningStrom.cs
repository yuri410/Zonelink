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
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
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
                cld.Apply3D();
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
            if (disaster.IsOver)
            {
                bool passed = true ;

                for (int i = 0; i < clouds.Count; i++)
                {
                    if (clouds[i].IsActive)
                    {
                        passed = false;
                        break;
                    }
                }
                if (sceMgr != null && passed)
                {
                    sceMgr.RemoveObjectFromScene(this);
                }
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
