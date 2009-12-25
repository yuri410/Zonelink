using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class TerrainTextureLibrary : Singleton
    {
        static TerrainTextureLibrary singleton;


        public static TerrainTextureLibrary Instance
        {
            get { return singleton; }
        }

        public static void Initialize(RenderSystem device)
        {
            singleton = new TerrainTextureLibrary(device);
        }

        protected struct Entry
        {
            public ResourceHandle<TerrainTexture> Map;

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

        public void LoadTextureSet(FileLocation configLoc)
        {
            Configuration config = ConfigurationManager.Instance.CreateInstance(configLoc);

            ConfigurationSection sect = config["DetailedMapsList"];

            ConfigurationSection.ValueCollection entries = sect.Values;

            foreach (string s in entries)
            {
                Entry entry;
                ConfigurationSection texSect = config[s];

                string fileName = texSect["Map"];
                FileLocation fl = FileSystem.Instance.Locate(fileName, GameFileLocs.TerrainTexture);

                entry.Map = TerrainTextureManager.Instance.CreateInstance(renderSystem, fl);

                entry.Name = s;

                detailedMaps.Add(s, entry);
            }

            string msg = "细节纹理库初始化完毕。加载了{0}种纹理。";

            EngineConsole.Instance.Write(string.Format(msg, detailedMaps.Count.ToString()), ConsoleMessageType.Information);
        }

        private TerrainTextureLibrary(RenderSystem device)
        {
            this.renderSystem = device;
            this.detailedMaps = new Dictionary<string, Entry>(CaseInsensitiveStringComparer.Instance);
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
