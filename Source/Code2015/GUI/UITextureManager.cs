using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.GUI
{
    class UITextureManager
    {
        static UITextureManager singleton;

        public static UITextureManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new UITextureManager();
                }
                return singleton;
            }
        }

        Dictionary<string, Texture> loadedTextures;


        private UITextureManager()
        {
            loadedTextures = new Dictionary<string, Texture>(CaseInsensitiveStringComparer.Instance);
        }

        public Texture CreateInstance(FileLocation rl)
        {
            Texture result;
            if (!loadedTextures.TryGetValue(rl.Name, out result))
            {
                result = TextureManager.Instance.CreateInstanceUnmanaged(rl);
                loadedTextures.Add(rl.Name, result);
            }
            return result;
        }
    }
}
