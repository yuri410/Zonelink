using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    static class GameFileLocs
    {
        public static FileLocateRule Effect
        {
            get;
            private set;
        }
        public static FileLocateRule Terrain
        {
            get;
            private set;
        }
        public static FileLocateRule TerrainTexture
        {
            get;
            private set;
        }
        public static FileLocateRule Config
        {
            get;
            private set;
        }
        static GameFileLocs()
        {
            LocateCheckPoint[] pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("terrain.lpk");
            Terrain = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("effects");
            Effect = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("configs");
            Config = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("terraintexture");
            TerrainTexture = new FileLocateRule(pts);
        }
    }
}
