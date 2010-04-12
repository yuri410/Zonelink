﻿using System;
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

        const int DataWidth1 = 129 * 36;
        const int DataHeight1 = 129 * 14;

        const int DataWidth2 = 33 * 36;
        const int DataHeight2 = 33 * 14;

        //const int TileLength0 = 513;
        const int TileLength1 = 129;
        const int TileLength2 = 33;

        //ContentBinaryReader reader0;
        ContentBinaryReader reader1;
        ContentBinaryReader reader2;
        //bool[,] existData;

        object syncHelper = new object();


        void Transform(ref int tx, ref int ty)
        {
            //tx = tx * 2 + 1;
            //ty = ty * 2 + 5;
            tx = (tx - 1) / 2;
            ty = (ty - 5) / 2;
        }



        long GetPosition1(int tx, int ty)
        {
            return sizeof(ushort) * (TileLength1 * ty * DataWidth1 + tx * TileLength1);
        }
        long GetPosition2(int tx, int ty)
        {
            return sizeof(ushort) * (TileLength2 * ty * DataWidth2 + tx * TileLength2);
        }

        //public bool HasData(int tx, int ty)
        //{
        //    Transform(ref tx, ref ty);
        //    return existData[tx, ty];
        //}


        // 使用地形1级

        public void GetSlopTangentMatrix(Vector2 p, Vector2 upper, Vector2 right, out Matrix trans)
        {

            throw new NotImplementedException();
        }



        public float QueryHeight(float longtiude, float latitude)
        {

            double yspan = (14.0 / 18.0) * Math.PI;

            int y = (int)(((yspan * 0.5 - latitude) / yspan) * DataHeight1);
            int x = (int)(((longtiude + Math.PI) / (2 * Math.PI)) * DataWidth1);

            if (y < 0) y += DataHeight1;
            if (y >= DataHeight1) y -= DataHeight1;

            if (x < 0) x += DataWidth1;
            if (x >= DataWidth1) x -= DataWidth1;

            lock (syncHelper)
            {
                reader1.BaseStream.Position = sizeof(ushort) * (y * DataWidth1 + x);

                return (reader1.ReadUInt16() / 7.0f - TerrainMeshManager.PostZeroLevel);
            }
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

        public float[] GetData(int tx, int ty)
        {
            Transform(ref tx, ref ty);

            float[] result = null;
            long colSpan = 0;
            long start = 0;
            int tileLen = 0;

            ContentBinaryReader cbr = null;

            cbr = reader2;
            start = GetPosition2(tx, ty);
            colSpan = DataWidth2 * sizeof(ushort);
            tileLen = TileLength2;


            result = new float[tileLen * tileLen];
            for (int i = 0; i < tileLen; i++)
            {
                lock (syncHelper)
                {
                    cbr.BaseStream.Position = start + i * colSpan;

                    for (int j = 0; j < tileLen; j++)
                    {
                        result[i * tileLen + j] = cbr.ReadUInt16() / 7.0f;
                    }
                }
            }

            return result;
        }

        private TerrainData()
        {
            //FileLocation fl = FileSystem.Instance.Locate("terrain_l0.tdmp", GameFileLocs.Terrain);
            //reader0 = new ContentBinaryReader(fl);

            FileLocation fl = FileSystem.Instance.Locate("terrain_l1.tdmp", GameFileLocs.Terrain);
            reader1 = new ContentBinaryReader(fl);

            fl = FileSystem.Instance.Locate("terrain_l2.tdmp", GameFileLocs.Terrain);
            reader2 = new ContentBinaryReader(fl);

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
            reader1.Close();
            reader2.Close();
        }
    }
}
