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
