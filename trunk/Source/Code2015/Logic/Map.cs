using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.Logic
{
    public class Map
    {
        public const int MapWidth = PathFinderManager.DW;
        public const int MapHeight = PathFinderManager.DH;

        SimulationRegion region;
        PathFinderManager pathFinder;


        public Map(SimulationRegion region)
        {
            this.region = region;

            FileLocation fl = FileSystem.Instance.Locate("grad.bit", GameFileLocs.Nature);

            BitTable gradMap = new BitTable(32);
            gradMap.Load(fl);

            pathFinder = new PathFinderManager(gradMap);
        }

        public PathFinderManager PathFinder
        {
            get { return pathFinder; }
        }

        public void BlockArea(float lng, float lat, float r)
        {
            
        }

        public static void GetMapCoord(float lng, float lat, out float x, out float y)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            y = ((yspan * 0.5f - lat) / yspan) * MapHeight;
            x = ((lng + MathEx.PIf) / (2 * MathEx.PIf)) * MapWidth;

            if (y < 0) y += MapHeight;
            if (y >= MapHeight) y -= MapHeight;

            if (x < 0) x += MapWidth;
            if (x >= MapWidth) x -= MapWidth;
        }

        public static void GetCoord(float x, float y, out float lng, out float lat)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            lat = yspan * 0.5f - y * yspan / (float)MapHeight;
            lng = x * MathEx.PIf * 2 / (float)MapWidth - MathEx.PIf;
        }

        public static void GetMapCoord(float lng, float lat, out int x, out int y)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            y = (int)(((yspan * 0.5f - lat) / yspan) * MapHeight);
            x = (int)(((lng + MathEx.PIf) / (2 * MathEx.PIf)) * MapWidth);

            if (y < 0) y += MapHeight;
            if (y >= MapHeight) y -= MapHeight;

            if (x < 0) x += MapWidth;
            if (x >= MapWidth) x -= MapWidth;
        }

        public static void GetCoord(int x, int y, out float lng, out float lat)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            lat = yspan * 0.5f - y * MathEx.PIf / (float)MapHeight;
            lng = x * MathEx.PIf * 2 / (float)MapWidth - MathEx.PIf;
        }
        
    }
}
