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
        ResourceHandle<TerrainMesh> terrain0;
        //ResourceHandle<TerrainMesh> terrain1;
        //ResourceHandle<TerrainMesh> terrain3;

        ResourceHandle<TerrainMesh> activeTerrain;

        ResourceHandle<TerrainMesh> ActiveTerrain
        {
            get { return activeTerrain; }
            set { activeTerrain = value; }
        }

        public TerrainTile(RenderSystem rs, int col, int lat)
            : base(true)
        {
            terrain0 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 17);
            terrain0.Touch();
            //terrain1 = TerrainMeshManager.Instance.CreateInstance(rs, col, lat, 17);
            //terrain1.Touch();
            Transformation = terrain0.GetWeakResource().Transformation;
            BoundingSphere = terrain0.GetWeakResource().BoundingSphere;

            //Transformation = terrain0.GetWeakResource().Transformation;
            //BoundingSphere = terrain0.GetWeakResource().BoundingSphere;

            //terrain0.GetWeakResource().ObjectSpaceChanged += TerrainMesh_ObjectSpaceChanged;
            //terrain1.GetWeakResource().ObjectSpaceChanged += TerrainMesh_ObjectSpaceChanged;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ActiveTerrain != null)
                return ActiveTerrain.Resource.GetRenderOperation();
            return null;
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            if (ActiveTerrain != null)
                return ActiveTerrain.Resource.GetRenderOperation();
            return null;
        }

        public override void PrepareVisibleObjects(ICamera cam, int level)
        {
            //if (level > 0)
            //{
            //    if (terrain1.State == ResourceState.Loaded)
            //    {
            //        ActiveTerrain = terrain1;
            //    }
            //    else
            //    {
            //        terrain1.Touch();
            //    }

            //    if (ActiveTerrain != null)
            //    {
            //        TerrainMesh tm = ActiveTerrain.Resource;

            //        tm.PrepareVisibleObjects(cam, level);
            //    }
            //}
            //else
            {
                if (terrain0.State == ResourceState.Loaded)
                {
                    ActiveTerrain = terrain0;
                }
                else
                {
                    terrain0.Touch();
                }

                if (ActiveTerrain != null)
                {
                    TerrainMesh tm = ActiveTerrain.Resource;

                    tm.PrepareVisibleObjects(cam, level);
                }
            }
        }

        public override void Update(GameTime dt)
        {
            BoundingSphere = terrain0.GetWeakResource().BoundingSphere;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                terrain0.Dispose();
                //terrain2.Dispose();
            }
            terrain0 = null;
            //terrain2 = null;
        }
    }
}
