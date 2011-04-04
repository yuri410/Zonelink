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
using Apoc3D.Graphics;
using Zonelink;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Code2015.EngineEx
{
    public class TerrainMaterialLibrary
    {
        protected struct Entry
        {
            public Texture2D Map;
            public Color Color;
            public Color Ambient;
            public Color Diffuse;
            public Color Specular;

            public string Name;
        }

        Game1 game;
        Dictionary<string, Entry> detailedMaps;

        static string defaultMap = "default";

        public static string DefaultMap
        {
            get { return defaultMap; }
            private set { defaultMap = value; }
        }

        public Texture2D GlobalIndexTexture
        {
            get;
            private set;
        }
        public Texture2D GlobalColorTexture
        {
            get;
            private set;
        }
        public Texture2D CliffColor
        {
            get;
            private set;
        }
        public Texture2D GlobalNormalTexture
        {
            get;
            private set;
        }

        public void LoadTextureSet(string configLoc)
        {
            GameConfiguration config = new GameConfiguration(configLoc);
            ConfigurationSection sect = config["MaterialList"];

            ConfigurationSection.ValueCollection entries = sect.Values;

            foreach (string s in entries)
            {
                Entry entry;
                ConfigurationSection matSect = config[s];

                string fileName = matSect["DiffuseMap"];

                Texture2D texture = game.Content.Load<Texture2D>(Path.Combine(GameFileLocs.TerrainTexture, fileName));
                
                entry.Map = texture;
                float[] v = matSect.GetSingleArray("DiffuseMapColor");
                entry.Color = new Color((int)v[0], (int)v[1], (int)v[2]);

                v = matSect.GetSingleArray("Ambient");
                entry.Ambient = new Color(v[0], v[1], v[2], v[3]);

                v = matSect.GetSingleArray("Diffuse");
                entry.Diffuse = new Color(v[0], v[1], v[2], v[3]);

                v = matSect.GetSingleArray("Specular");
                entry.Specular = new Color(v[0], v[1], v[2], v[3]);

                entry.Name = s;

                detailedMaps.Add(s, entry);
            }

            string name = Path.Combine(GameFileLocs.TerrainTexture, "index.tex");
            GlobalIndexTexture = game.Content.Load<Texture2D>(name);

            name = Path.Combine(GameFileLocs.TerrainTexture, "planetClr.tex");
            GlobalColorTexture = game.Content.Load<Texture2D>(name);

            name = Path.Combine(GameFileLocs.TerrainTexture, "normal.tex");
            GlobalNormalTexture = game.Content.Load<Texture2D>(name);

            name = Path.Combine(GameFileLocs.TerrainTexture, "cliff.tex");
            CliffColor = game.Content.Load<Texture2D>(name);


        }

        private TerrainMaterialLibrary(Game1 device)
        {
            this.game = device;
            this.detailedMaps = new Dictionary<string, Entry>(CaseInsensitiveStringComparer.Instance);
        }

        public Color GetAmbient(string name)
        {
            return detailedMaps[name].Ambient;
        }
        public Color GetDiffuse(string name)
        {
            return detailedMaps[name].Diffuse;
        }
        public Color GetSpecular(string name)
        {
            return detailedMaps[name].Specular;
        }
        public Color GetColor(string name) 
        {
            return detailedMaps[name].Color;
        }
        public Texture2D GetTexture(string name)
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


    }
}
