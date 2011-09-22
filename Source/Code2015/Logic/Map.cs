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
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015.Logic
{
    class Map
    {
        public const int MapWidth = PathFinderManager.DW;
        public const int MapHeight = PathFinderManager.DH;
        public const int HeightMapWidth = 64 * 36;
        public const int HeightMapHeight = 64 * 14;

        BattleField region;
        PathFinderManager pathFinder;
        BitTable gradMap;

        ushort[][] heightData;

        public Map(BattleField region)
        {
            this.region = region;

            FileLocation fl = FileSystem.Instance.Locate("grad.bit", GameFileLocs.Nature);

            gradMap = new BitTable(32);
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
            int x, y;
            GetMapCoord(lng, lat, out x, out y);

            int w = (int)(((r + MathEx.PIf) / (2 * MathEx.PIf)) * MapWidth - ((0 + MathEx.PIf) / (2 * MathEx.PIf)) * MapWidth);

            for (int i = -w; i < w; i++) 
            {
                for (int j = -w; j < w; j++)
                {
                    float rr = (float)Math.Sqrt(MathEx.Sqr(i) + MathEx.Sqr(j));

                    if (rr <= w)
                    {
                        gradMap.SetBit((i + y) * MapWidth + (j + x), true);
                    }
                }
            }
        }

        
        //public float GetHeight(Point pt)
        //{

        //    return TerrainMeshManager.PostHeightScale * (heightData[pt.Y][pt.X] / 7f - TerrainMeshManager.PostZeroLevel);

        //}
        public Matrix GetTangentSpaceMatrix(Vector2 pa, Vector2 pb)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            float y1 = ((yspan * 0.5f - pa.Y) / yspan) * HeightMapHeight;
            float x1 = ((pa.X + MathEx.PIf) / (2 * MathEx.PIf)) * HeightMapWidth;

            if (y1 < 0) y1 += HeightMapHeight;
            if (y1 >= HeightMapHeight) y1 -= HeightMapHeight;

            if (x1 < 0) x1 += HeightMapWidth;
            if (x1 >= HeightMapWidth) x1 -= HeightMapWidth;


            float y2 = ((yspan * 0.5f - pb.Y) / yspan) * HeightMapHeight;
            float x2 = ((pb.X + MathEx.PIf) / (2 * MathEx.PIf)) * HeightMapWidth;

            if (y2 < 0) y2 += HeightMapHeight;
            if (y2 >= HeightMapHeight) y2 -= HeightMapHeight;

            if (x2 < 0) x2 += HeightMapWidth;
            if (x2 >= HeightMapWidth) x2 -= HeightMapWidth;

            float h1 = TerrainMeshManager.PostHeightScale * (heightData[(int)y1][(int)x1] / 7f - TerrainMeshManager.PostZeroLevel);
            float h2 = TerrainMeshManager.PostHeightScale * (heightData[(int)y2][(int)x2] / 7f - TerrainMeshManager.PostZeroLevel);

            Vector3 p1 = PlanetEarth.GetPosition(pa.X, pa.Y, PlanetEarth.PlanetRadius + h1);
            Vector3 p2 = PlanetEarth.GetPosition(pb.X, pb.Y, PlanetEarth.PlanetRadius + h2);

            Vector3 dir = p2 - p1;
            dir.Normalize();

            Vector3 n = p1;
            n.Normalize();

            Vector3 bi = Vector3.Cross(dir, n);
            bi.Normalize();

            n = Vector3.Cross(dir, bi);
            n.Normalize();

            Matrix result = Matrix.Identity;
            result.Right = bi;
            result.Up = n;
            result.Forward = -dir;
            return result;
        }

        public float GetHeight(float lng, float lat)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            float y = ((yspan * 0.5f - lat) / yspan) * HeightMapHeight;
            float x = ((lng + MathEx.PIf) / (2 * MathEx.PIf)) * HeightMapWidth;

            if (y < 0) y += HeightMapHeight;
            if (y >= HeightMapHeight) y -= HeightMapHeight;

            if (x < 0) x += HeightMapWidth;
            if (x >= HeightMapWidth) x -= HeightMapWidth;

            //int xx = (int)Math.Truncate(x);
            //int yy = (int)Math.Truncate(y);

            //float xlerp = x - xx;
            //float ylerp = y - yy;


            //float v1 = xx < HeightMapWidth - 1 ? MathEx.LinearInterpose(heightData[yy][xx], heightData[yy][xx + 1], xlerp) : heightData[yy][xx];
            //float v2 = yy < HeightMapHeight - 1 ? MathEx.LinearInterpose(heightData[yy][xx], heightData[yy + 1][xx], ylerp) : heightData[yy][xx];

            return TerrainMeshManager.PostHeightScale * (heightData[(int)y][(int)x] / 7f - TerrainMeshManager.PostZeroLevel);

            //return TerrainMeshManager.PostHeightScale * ((MathEx.LinearInterpose(v1, v2, xlerp * ylerp)) / 7f - TerrainMeshManager.PostZeroLevel);
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

            lat = yspan * 0.5f - y * yspan / (float)MapHeight;
            lng = x * MathEx.PIf * 2 / (float)MapWidth - MathEx.PIf;
        }
        
    }
}
