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
using Code2015.EngineEx;
using Code2015.World;
using Zonelink;
using System.IO;
using Microsoft.Xna.Framework;
using Zonelink.World;
using Zonelink.MathLib;

namespace Code2015.Logic
{
    public class Map
    {
        public const int MapWidth = PathFinderManager.DW;
        public const int MapHeight = PathFinderManager.DH;
        public const int HeightMapWidth = 64 * 36;
        public const int HeightMapHeight = 64 * 14;

        
        PathFinderManager pathFinder;
        bool[][] gradMap;
        private BattleField region;

        ushort[][] heightData;

        public Map(BattleField field)
        {
            this.region = field;

            ReadMap();
            pathFinder = new PathFinderManager(gradMap);
        
            ReadHeightMap();         
        }

        public PathFinderManager PathFinder
        {
            get { return pathFinder; }
        }

        public void BlockArea(float lng, float lat, float r)
        {
            int x, y;
            GetMapCoord(lng, lat, out x, out y);

            int w = (int)(((r + MathHelper.Pi) / (2 * MathHelper.Pi)) * MapWidth - ((0 + MathHelper.Pi) / (2 * MathHelper.Pi)) * MapWidth);

            for (int i = -w; i < w; i++) 
            {
                for (int j = -w; j < w; j++)
                {
                    float rr = (float)Math.Sqrt(MathEx.Sqr(i - y) + MathEx.Sqr(j - x));

                    if (rr <= w)
                    {
                        // gradMap.SetBit((i + y) * MapWidth + (j + x), true);
                        gradMap[i + y][j + x] = true;
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
            float yspan = (14.0f / 18.0f) * MathHelper.Pi;

            float y1 = ((yspan * 0.5f - pa.Y) / yspan) * HeightMapHeight;
            float x1 = ((pa.X + MathHelper.Pi) / (2 * MathHelper.Pi)) * HeightMapWidth;

            if (y1 < 0) y1 += HeightMapHeight;
            if (y1 >= HeightMapHeight) y1 -= HeightMapHeight;

            if (x1 < 0) x1 += HeightMapWidth;
            if (x1 >= HeightMapWidth) x1 -= HeightMapWidth;


            float y2 = ((yspan * 0.5f - pb.Y) / yspan) * HeightMapHeight;
            float x2 = ((pb.X + MathHelper.Pi) / (2 * MathHelper.Pi)) * HeightMapWidth;

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
            float yspan = (14.0f / 18.0f) * MathHelper.Pi;

            float y = ((yspan * 0.5f - lat) / yspan) * HeightMapHeight;
            float x = ((lng + MathHelper.Pi) / (2 * MathHelper.Pi)) * HeightMapWidth;

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
            float yspan = (14.0f / 18.0f) * MathHelper.Pi;

            y = ((yspan * 0.5f - lat) / yspan) * MapHeight;
            x = ((lng + MathHelper.Pi) / (2 * MathHelper.Pi)) * MapWidth;

            if (y < 0) y += MapHeight;
            if (y >= MapHeight) y -= MapHeight;

            if (x < 0) x += MapWidth;
            if (x >= MapWidth) x -= MapWidth;
        }

        public static void GetCoord(float x, float y, out float lng, out float lat)
        {
            float yspan = (14.0f / 18.0f) * MathHelper.Pi;

            lat = yspan * 0.5f - y * yspan / (float)MapHeight;
            lng = x * MathHelper.Pi * 2 / (float)MapWidth - MathHelper.Pi;
        }

        public static void GetMapCoord(float lng, float lat, out int x, out int y)
        {
            float yspan = (14.0f / 18.0f) * MathHelper.Pi;

            y = (int)(((yspan * 0.5f - lat) / yspan) * MapHeight);
            x = (int)(((lng + MathHelper.Pi) / (2 * MathHelper.Pi)) * MapWidth);

            if (y < 0) y += MapHeight;
            if (y >= MapHeight) y -= MapHeight;

            if (x < 0) x += MapWidth;
            if (x >= MapWidth) x -= MapWidth;
        }

        public static void GetCoord(int x, int y, out float lng, out float lat)
        {
            float yspan = (14.0f / 18.0f) * MathHelper.Pi;

            lat = yspan * 0.5f - y * yspan / (float)MapHeight;
            lng = x * MathHelper.Pi * 2 / (float)MapWidth - MathHelper.Pi;
        }

        private void ReadMap()
        {
            string path = Path.Combine(GameFileLocs.Nature, "grad.raw");
            FileStream fl = File.Open(path, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fl);

            this.gradMap = new bool[MapWidth][];

            for (int i = 0; i < MapWidth; i++)
            {
                this.gradMap[i] = new bool[MapHeight];
                for (int j = 0; j < MapHeight; j++)
                {
                    byte b= br.ReadByte();
                    gradMap[i][j] = b > 127;
                }
            }

            br.Close();
        }

        private void ReadHeightMap()
        {
            string path = Path.Combine(GameFileLocs.Nature, "mapheight.raw");
            FileStream fl = File.Open(path, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fl);

            heightData = new ushort[HeightMapHeight][];  

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

        
    }
}
