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
using Code2015.EngineEx;
using Zonelink;
using Apoc3D.Scene;
using Microsoft.Xna.Framework;

namespace Code2015.World
{
    /// <summary>
    ///  表示地球上的一个地形块
    /// </summary>
    class TerrainTile : SceneObject
    {
        TerrainMesh terrain;
        //ResourceHandle<TerrainMesh> terrain3;

        TerrainMesh activeTerrain;

        TerrainMesh ActiveTerrain
        {
            get { return activeTerrain; }
            set { activeTerrain = value; }
        }

        public TerrainTile(Game1 rs, int col, int lat)
        {
            terrain = TerrainMeshManager.Instance.CreateInstance(rs, col, lat);
            activeTerrain = terrain;
            //terrain3 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 3);

            Transformation = terrain.Transformation;
            BoundingSphere = terrain.BoundingSphere;

            //Transformation = terrain0.GetWeakResource().Transformation;
            //BoundingSphere = terrain0.GetWeakResource().BoundingSphere;

            //terrain0.GetWeakResource().ObjectSpaceChanged += TerrainMesh_ObjectSpaceChanged;
            //terrain1.GetWeakResource().ObjectSpaceChanged += TerrainMesh_ObjectSpaceChanged;
        }

        
        public override void Render()
        {
            if (ActiveTerrain != null)
                ActiveTerrain.Render();
        }


        public override void Update(GameTime dt)
        {
            BoundingSphere = terrain.BoundingSphere;
        }



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                terrain.Dispose();
                //terrain2.Dispose();
            }
            terrain = null;
            //terrain2 = null;
        }
    }
}
