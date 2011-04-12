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
using System.IO;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    public class TerrainMaterialLibrary : Singleton
    {
        static TerrainMaterialLibrary singleton;


        public static TerrainMaterialLibrary Instance
        {
            get { return singleton; }
        }

        public static void Initialize(RenderSystem device)
        {
            singleton = new TerrainMaterialLibrary(device);
        }

        protected struct Entry
        {
            public ResourceHandle<TerrainTexture> Map;
            public Color4F Color;
            public Color4F Ambient;
            public Color4F Diffuse;
            public Color4F Specular;

            public string Name;
        }

        RenderSystem renderSystem;
        Dictionary<string, Entry> detailedMaps;

        static string defaultMap = "default";

        public static string DefaultMap
        {
            get { return defaultMap; }
            private set { defaultMap = value; }
        }

        public ResourceHandle<Texture> GlobalBakedNormalTexture 
        {
            get;
            private set;
        }
        public ResourceHandle<Texture> GlobalIndexTexture
        {
            get;
            private set;
        }
        public ResourceHandle<Texture> GlobalColorTexture
        {
            get;
            private set;
        }
        public ResourceHandle<Texture> CliffColor
        {
            get;
            private set;
        }
        public ResourceHandle<Texture> GlobalNormalTexture
        {
            get;
            private set;
        }

        public void LoadTextureSet(FileLocation configLoc)
        {
            Configuration config = ConfigurationManager.Instance.CreateInstance(configLoc);

            ConfigurationSection sect = config["MaterialList"];

            ConfigurationSection.ValueCollection entries = sect.Values;

            foreach (string s in entries)
            {
                Entry entry;
                ConfigurationSection matSect = config[s];

                string fileName = matSect["DiffuseMap"];
                FileLocation fl = FileSystem.Instance.Locate(fileName, GameFileLocs.TerrainTexture);

                ResourceHandle<TerrainTexture> texture = TerrainTextureManager.Instance.CreateInstance(renderSystem, fl);
                texture.Touch();

                entry.Map = texture;
                float[] v = matSect.GetSingleArray("DiffuseMapColor");
                entry.Color = new Color4F((int)v[0], (int)v[1], (int)v[2]);

                v = matSect.GetSingleArray("Ambient");
                entry.Ambient = new Color4F(v[0], v[1], v[2], v[3]);

                v = matSect.GetSingleArray("Diffuse");
                entry.Diffuse = new Color4F(v[0], v[1], v[2], v[3]);

                v = matSect.GetSingleArray("Specular");
                entry.Specular = new Color4F(v[0], v[1], v[2], v[3]);

                entry.Name = s;

                detailedMaps.Add(s, entry);
            }

            FileLocation fl2 = FileSystem.Instance.Locate("index.tex", GameFileLocs.TerrainTexture);
            GlobalIndexTexture = TextureManager.Instance.CreateInstance(fl2);
            fl2 = FileSystem.Instance.Locate("planetClr.tex", GameFileLocs.TerrainTexture);
            GlobalColorTexture = TextureManager.Instance.CreateInstance(fl2);

            fl2 = FileSystem.Instance.Locate("normal.tex", GameFileLocs.TerrainTexture);
            GlobalNormalTexture = TextureManager.Instance.CreateInstance(fl2);

            fl2 = FileSystem.Instance.Locate("cliff.tex", GameFileLocs.TerrainTexture);
            CliffColor = TextureManager.Instance.CreateInstance(fl2);
          
            fl2 = FileSystem.Instance.Locate("normal_baked.tex", GameFileLocs.TerrainTexture);
            GlobalBakedNormalTexture = TextureManager.Instance.CreateInstance(fl2);

            string msg = "细节纹理库初始化完毕。加载了{0}种纹理。";

            EngineConsole.Instance.Write(string.Format(msg, detailedMaps.Count.ToString()), ConsoleMessageType.Information);
        }

        private TerrainMaterialLibrary(RenderSystem device)
        {
            this.renderSystem = device;
            this.detailedMaps = new Dictionary<string, Entry>(CaseInsensitiveStringComparer.Instance);
        }

        public Color4F GetAmbient(string name)
        {
            return detailedMaps[name].Ambient;
        }
        public Color4F GetDiffuse(string name)
        {
            return detailedMaps[name].Diffuse;
        }
        public Color4F GetSpecular(string name)
        {
            return detailedMaps[name].Specular;
        }
        public Color4F GetColor(string name) 
        {
            return detailedMaps[name].Color;
        }
        public ResourceHandle<TerrainTexture> GetTexture(string name)
        {
            return detailedMaps[name].Map;
        }

        public string[] GetNames()
        {
            string[] result = new string[detailedMaps.Count];

            int index = 0;
            foreach (KeyValuePair<string, Entry> e in detailedMaps)
            {
                result[index++] = e.Key;
            }
            return result;
        }

        protected override void dispose()
        {
            foreach (KeyValuePair<string, Entry> e in detailedMaps)
            {
                ResourceHandle<TerrainTexture> tex = e.Value.Map;
                tex.Dispose();
            }
            detailedMaps.Clear();

            singleton = null;
        }
    }
}
