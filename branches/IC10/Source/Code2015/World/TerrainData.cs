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
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.World
{
    public class TerrainData : Singleton
    {
        static TerrainData singleton;

        public static TerrainData Instance
        {
            get { return singleton; }
        }

        public static void Initialize()
        {
            singleton = new TerrainData();
        }


        const int Data1Len = 33;
        const int Data2Len = 17;

        const int DataWidth1 = Data1Len * 36;
        const int DataHeight1 = Data1Len * 14;

        const int DataWidth2 = Data2Len * 36;
        const int DataHeight2 = Data2Len * 14;

        //const int TileLength0 = 513;
        const int TileLength1 = Data1Len;
        const int TileLength2 = Data2Len;

        //ContentBinaryReader reader0;
        //ContentBinaryReader reader1;
        //ContentBinaryReader reader2;
        //bool[,] existData;

        ushort[] data0;
        ushort[] data1;



        object syncHelper = new object();


        void Transform(ref int tx, ref int ty)
        {
            //tx = tx * 2 + 1;
            //ty = ty * 2 + 5;
            tx = (tx - 1) / 2;
            ty = (ty - 5) / 2;
        }



        //long GetPosition1(int tx, int ty)
        //{
        //    return sizeof(ushort) * (TileLength1 * ty * DataWidth1 + tx * TileLength1);
        //}
        //long GetPosition2(int tx, int ty)
        //{
        //    return sizeof(ushort) * (TileLength2 * ty * DataWidth2 + tx * TileLength2);
        //}

        //public bool HasData(int tx, int ty)
        //{
        //    Transform(ref tx, ref ty);
        //    return existData[tx, ty];
        //}





        public float QueryHeight(float longtiude, float latitude)
        {

            double yspan = (14.0 / 18.0) * Math.PI;

            int y = (int)(((yspan * 0.5 - latitude) / yspan) * DataHeight1);
            int x = (int)(((longtiude + Math.PI) / (2 * Math.PI)) * DataWidth1);

            if (y < 0) y += DataHeight1;
            if (y >= DataHeight1) y -= DataHeight1;

            if (x < 0) x += DataWidth1;
            if (x >= DataWidth1) x -= DataWidth1;

            return data0[y * DataWidth1 + x] / 7.0f - TerrainMeshManager.PostZeroLevel;
            //lock (syncHelper)
            //{
            //    reader1.BaseStream.Position = sizeof(ushort) * (y * DataWidth1 + x);

            //    return (reader1.ReadUInt16() / 7.0f - TerrainMeshManager.PostZeroLevel);
            //}
        }
        //public float QueryHeight(float longtiude, float latitude)
        //{

        //    double yspan = (14.0 / 18.0) * Math.PI;

        //    int y = (int)(((yspan * 0.5 - latitude) / yspan) * DataHeight0);
        //    int x = (int)(((longtiude + Math.PI) / (2 * Math.PI)) * DataWidth0);

        //    if (y < 0) y += DataHeight0;
        //    if (y >= DataHeight0) y -= DataHeight0;

        //    if (x < 0) x += DataWidth0;
        //    if (x >= DataWidth0) x -= DataWidth0;

        //    lock (syncHelper)
        //    {
        //        reader0.BaseStream.Position = sizeof(ushort) * (y * DataWidth0 + x);

        //        return (reader0.ReadUInt16() / 7.0f - TerrainMeshManager.PostZeroLevel);
        //    }
        //}
        float[] GetData1(int tx, int ty)
        {
            Transform(ref tx, ref ty);

            float[] result = null;
            long colSpan = 0;
            int tileLen = 0;

            //ContentBinaryReader cbr = null;

            //cbr = reader2;
            long start = TileLength2 * ty * DataWidth2 + tx * TileLength2;
            colSpan = DataWidth2;
            tileLen = TileLength2;


            result = new float[tileLen * tileLen];
            for (int i = 0; i < tileLen; i++)
            {
                long rowStart = start + i * colSpan;
                for (int j = 0; j < tileLen; j++)
                {
                    result[i * tileLen + j] = data1[rowStart + j] / 7.0f;
                }
            }

            return result;
        }
        float[] GetData0(int tx, int ty)
        {
            Transform(ref tx, ref ty);

            float[] result = null;
            long colSpan = 0;
            int tileLen = 0;


            long start = TileLength1 * ty * DataWidth1 + tx * TileLength1;
            colSpan = DataWidth1;
            tileLen = TileLength1;


            result = new float[tileLen * tileLen];
            for (int i = 0; i < tileLen; i++)
            {
                long rowStart = start + i * colSpan;
                for (int j = 0; j < tileLen; j++)
                {
                    result[i * tileLen + j] = data0[rowStart + j] / 7.0f;
                }
            }

            return result;
        }

        public float[] GetData(int tx, int ty, int size)
        {
            if (size == 33) 
            {
                return GetData0(tx, ty);
            }
            return GetData1(tx, ty);
        }

        private TerrainData()
        {
            //FileLocation fl = FileSystem.Instance.Locate("terrain_l0.tdmp", GameFileLocs.Terrain);
            //reader0 = new ContentBinaryReader(fl);

            FileLocation fl = FileSystem.Instance.Locate("terrain_l2.tdmp", GameFileLocs.Terrain);
            ContentBinaryReader reader1 = new ContentBinaryReader(fl);

            data0 = new ushort[DataWidth1 * DataHeight1];
            for (int i = 0; i < DataHeight1; i++)
            {
                for (int j = 0; j < DataWidth1; j++)
                {
                    data0[i * DataWidth1 + j] = reader1.ReadUInt16();
                }
            }
            reader1.Close();



            fl = FileSystem.Instance.Locate("terrain_l3.tdmp", GameFileLocs.Terrain);
            ContentBinaryReader reader2 = new ContentBinaryReader(fl);

            data1 = new ushort[DataHeight2 * DataWidth2];
            for (int i = 0; i < DataHeight2; i++)
            {
                for (int j = 0; j < DataWidth2; j++)
                {
                    data1[i * DataWidth2 + j] = reader2.ReadUInt16();
                }
            }
            reader2.Close();



            //fl = FileSystem.Instance.Locate("flags.dat", GameFileLocs.Terrain);

            //existData = new bool[36, 14];
            //ContentBinaryReader br = new ContentBinaryReader(fl);
            //for (int i = 0; i < 36; i++)
            //{
            //    for (int j = 0; j < 14; j++)
            //    {
            //        existData[i, j] = br.ReadBoolean();
            //    }
            //}
            //br.Close();
        }


        protected override void dispose()
        {
        }
    }
}
