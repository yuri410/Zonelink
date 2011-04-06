using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Zonelink
{
    class GameFileLocs
    {
        public static readonly string Configs = "Configs";
        public static readonly string Nature = Path.Combine(Game1.ContentDir, "Nature");
        public static readonly string Model = Path.Combine(Game1.ContentDir, "Model");
        public static readonly string Terrain = Path.Combine(Game1.ContentDir, "Terrain");
        public static readonly string TerrainTexture = Path.Combine(Game1.ContentDir, "TerrainTexture");

        public static readonly string Texture = Path.Combine(Game1.ContentDir, "Texture");



        public static readonly string CNature = "Nature";
        public static readonly string CTerrain = "Terrain";
        public static readonly string CTerrainTexture = "TerrainTexture";

        public static readonly string CTexture = "Texture";


    }
}
