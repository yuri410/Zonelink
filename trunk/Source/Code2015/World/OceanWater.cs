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
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
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

         
            dataMgr = new OceanWaterDataManager(rs);


            waterTiles = new OceanWaterTile[PlanetEarth.ColTileCount * PlanetEarth.LatTileCount];

            for (int i = 1, index = 0; i < PlanetEarth.ColTileCount * 2; i += 2)
            {
                for (int j = 1; j < PlanetEarth.LatTileCount * 2; j += 2)
                {
                    waterTiles[index++] = new OceanWaterTile(rs, dataMgr, i, j + PlanetEarth.LatTileStart);
                }
            }

            //base.ModelL0 = oceanSphere;

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
            return null; // base.GetRenderOperation();
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            return null;// base.GetRenderOperation(level);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            waterTiles = null;
        }
    }
}
