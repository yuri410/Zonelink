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
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015
{
    class Precache
    {
        static FastList<ResourceHandle<Texture>> textureBuffer = new FastList<ResourceHandle<Texture>>();
        static FastList<ResourceHandle<ModelData>> modelBuffer = new FastList<ResourceHandle<ModelData>>();
        static FastList<ResourceHandle<TerrainMesh>> terrainMesh = new FastList<ResourceHandle<TerrainMesh>>();

        public static void Cache(RenderSystem rs)
        {
            string[] files = FileSystem.Instance.SearchFile("texture\\*.tex");

            for (int i = 0; i < files.Length; i++)
            {
                ResourceHandle<Texture> tex = TextureManager.Instance.CreateInstance(new FileLocation(files[i]));
                textureBuffer.Add(tex);
            }
            files = FileSystem.Instance.SearchFile("model\\*.mesh");

            for (int i = 0; i < files.Length; i++)
            {
                ResourceHandle<ModelData> mdl = ModelManager.Instance.CreateInstance(rs, new FileLocation(files[i]));
                modelBuffer.Add(mdl);
            }      
            for (int i = 0; i < textureBuffer.Count; i++)
            {
                textureBuffer[i].Touch();
            }
            for (int i = 0; i < modelBuffer.Count; i++) 
            {
                modelBuffer[i].Touch();
            }

            for (int i = 1; i < PlanetEarth.ColTileCount * 2; i += 2)
            {
                for (int j = 1; j < PlanetEarth.LatTileCount * 2; j += 2)
                {
                    ResourceHandle<TerrainMesh> mesh = TerrainMeshManager.Instance.CreateInstance(rs, i, j + PlanetEarth.LatTileStart);
                    mesh.Touch();
                    terrainMesh.Add(mesh);
                }
            }
        }
    }
}
