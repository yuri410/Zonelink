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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.GUI
{
    /// <summary>
    ///  GUI纹理贴图管理器，所有GUI的纹理贴图应从此处创建
    /// </summary>
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
