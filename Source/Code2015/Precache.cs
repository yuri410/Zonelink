using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015
{
    class Precache
    {
        static FastList<ResourceHandle<Texture>> textureBuffer = new FastList<ResourceHandle<Texture>>();
        static FastList<ResourceHandle<ModelData>> modelBuffer = new FastList<ResourceHandle<ModelData>>();

        public static void Initialize(RenderSystem rs)
        {
            string[] files = FileSystem.Instance.SearchFile("*.tex");

            for (int i = 0; i < files.Length; i++)
            {
                textureBuffer.Add(TextureManager.Instance.CreateInstance(new FileLocation(files[i])));
            }
            files = FileSystem.Instance.SearchFile("*.mesh");

            for (int i = 0; i < files.Length; i++)
            {
                modelBuffer.Add(ModelManager.Instance.CreateInstance(rs, new FileLocation(files[i])));
            }            
        }

        public static void Cache()
        {
            for (int i = 0; i < textureBuffer.Count; i++)
            {
                textureBuffer[i].Touch();
            }
            for (int i = 0; i < modelBuffer.Count; i++) 
            {
                modelBuffer[i].Touch();
            }
        }
    }
}
