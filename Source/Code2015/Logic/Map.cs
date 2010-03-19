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
        public const int HeightMapWidth = MapWidth * 2;
        public const int HeightMapHeight = MapHeight * 2;

        SimulationRegion region;
        PathFinderManager pathFinder;

        ushort[][] heightData;

        public Map(SimulationRegion region)
        {
            this.region = region;

            FileLocation fl = FileSystem.Instance.Locate("grad.bit", GameFileLocs.Nature);

            BitTable gradMap = new BitTable(32);
            gradMap.Load(fl);

            pathFinder = new PathFinderManager(gradMap);

            fl = FileSystem.Instance.Locate("mapheight.raw", GameFileLocs.Nature);

            heightData = new ushort[HeightMapHeight][];
            ContentBinaryReader br = new ContentBinaryReader(fl);
            for (int i = 0; i < HeightMapHeight; i++)
            {
                heightData[i] = new ushort[HeightMapWidth];
                for (int j = 0; j < HeightMapWidth; j++) 
                {
                    heightData[i][j] = br.ReadUInt16();
                }
            }
            br.Close();
        }

        public PathFinderManager PathFinder
        {
            get { return pathFinder; }
        }

        public void BlockArea(float lng, float lat, float r)
        {
            
        }

        public float GetHeightBilinear(float lng, float lat)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            float y = ((yspan * 0.5f - lat) / yspan) * HeightMapHeight;
            float x = ((lng + MathEx.PIf) / (2 * MathEx.PIf)) * HeightMapWidth;

            if (y < 0) y += HeightMapHeight;
            if (y >= HeightMapHeight) y -= HeightMapHeight;

            if (x < 0) x += HeightMapWidth;
            if (x >= HeightMapWidth) x -= HeightMapWidth;

            int xx = (int)Math.Truncate(x);
            int yy = (int)Math.Truncate(y);

            float xlerp = x - xx;
            float ylerp = y - yy;


            float v1 = xx < HeightMapWidth - 1 ? MathEx.LinearInterpose(heightData[yy][xx], heightData[yy][xx + 1], xlerp) : heightData[yy][xx];
            float v2 = yy < HeightMapHeight - 1 ? MathEx.LinearInterpose(heightData[yy][xx], heightData[yy + 1][xx], ylerp) : heightData[yy][xx];


            return TerrainMeshManager.PostHeightScale * ((MathEx.LinearInterpose(v1, v2, xlerp * ylerp)) / 7f - TerrainMeshManager.PostZeroLevel);
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
