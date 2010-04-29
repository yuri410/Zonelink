using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    public static class GameFileLocs
    {
        public static FileLocateRule Default
        {
            get;
            private set;
        }
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
        public static FileLocateRule Earth
        {
            get;
            private set;
        }
        public static FileLocateRule Config
        {
            get;
            private set;
        }
        public static FileLocateRule Sound
        {
            get;
            private set;
        }
        public static FileLocateRule Movie
        {
            get;
            private set;
        }
        public static FileLocateRule Help
        {
            get;
            private set;
        }
        static GameFileLocs()
        {
            LocateCheckPoint[] pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("terrain");
            Terrain = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("terrainNormal");
            TerrainNormal = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("effect");
            Effect = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("configs");
            Config = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("terraintexture");
            TerrainTexture = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("nature");
            Nature = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("texture");
            Texture = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("gui");
            GUI = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("model");
            Model = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("earth");
            Earth = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("sounds");
            Sound = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("movie");
            Movie = new FileLocateRule(pts);
            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath("help");
            Help = new FileLocateRule(pts);

            pts = new LocateCheckPoint[1];
            pts[0] = new LocateCheckPoint();
            pts[0].AddPath(string.Empty);
            Default = new FileLocateRule(pts);
        }
    }
}
