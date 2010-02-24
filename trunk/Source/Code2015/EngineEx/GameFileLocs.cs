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
        public static FileLocateRule Nature
        {
            get;
            private set;
        }
        public static FileLocateRule Model
        {
            get;
            private set;
        }
        public static FileLocateRule Terrain
        {
            get;
            private set;
        }
        public static FileLocateRule TerrainNormal
        {
            get;
            private set;
        }
        public static FileLocateRule TerrainTexture
        {
            get;
            private set;
        }
        public static FileLocateRule Texture
        {
            get;
            private set;
        }
        public static FileLocateRule GUI
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
            pts[0].AddPath("terrainNormal.lpk");
            TerrainNormal = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("effect.lpk");
            Effect = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("configs");
            Config = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("terraintexture.lpk");
            TerrainTexture = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("nature.lpk");
            Nature = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("texture.lpk");
            Texture = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("gui.lpk");
            GUI = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("model.lpk");
            Model = new FileLocateRule(pts);
        }
    }
}
