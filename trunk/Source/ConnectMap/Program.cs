using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace ConnectMap
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectMap c = new ConnectMap();
            float[,] total = c.totalMapData;
            BinaryWriter bw = new BinaryWriter(File.Open(@"C:\Users\penser\Desktop\ushort.all.raw", FileMode.OpenOrCreate));

            for (int i = 0; i < ConnectMap.Height; i++)
            {
                for (int j = 0; j < ConnectMap.Width; j++)
                {
                    
                    bw.Write((ushort)(total[i, j]*7));
                    //Console.WriteLine(total[i,j]);

                    //Console.WriteLine("Over");
                }
            }
            bw.Close();
            Console.WriteLine("Over");
            Console.ReadLine();
        }
    }
    class ConnectMap
    {

        string file;
        //public float[] subMapData;//用于存储每个小图片的数组
        public int SubMapHAmount = 263169;//存储每张图片的高度值的数目
        public float[,][] totalsub;//用于存储总共图片数组的数组
        public const int Height = 7182;
        public const int Width = 18468;
        public int Amount = 432;//所有图片的数目
        public float[,] totalMapData;//用于存储所有图片的高度信息的数组

        public ConnectMap()
        {
            totalsub = new float[36,14][];
            totalMapData = new float[Height, Width];

            string dir = "C:\\Users\\penser\\Documents\\Visual Studio 2008\\Projects\\lrvbsvnicg\\Source\\Code2015\\bin\\x86\\Debug\\terrain.lpk\\";
            //int sub = 0;
            for (int i = 0; i < 36; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    int x = i * 2 + 1;
                    int y = j * 2 + 5;

                    file = dir + "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_0.tdmp";
                    TDMPIO td = new TDMPIO();
                    if (File.Exists(file))
                    {
                        td.Load(new FileLocation(file));
                        Console.WriteLine(i.ToString() + "_" + j.ToString());


                        totalsub[i, j] = td.Data;
                    }
                }
            }

            Console.WriteLine("down");
            for (int i = 0; i < 36; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    int x = i * 2 + 1;
                    int y = j * 2 + 5;

                    float[] temp = totalsub[i, j];
                    if (temp != null)
                    {
                        for (int iPerMap = 0; iPerMap < temp.Length; iPerMap++)
                        {
                            int xx = iPerMap % 513;
                            int yy = iPerMap / 513;
                            totalMapData[j * 513 + yy, i * 513 + xx] = temp[iPerMap];
                        }
                    }
                }
            }


            //for (int i = 0; i < Amount; i++)
            //{
            //    float[] temp = totalsub[i];
            //    for (int width = 0; width < 36; width++)
            //    {
            //        for (int height = 0; height < 12; height++)
            //        {
            //            for (int iPerMap = 0; iPerMap < temp.Length; iPerMap++)
            //                totalMapData[width * 513, height * 513] = temp[iPerMap];
            //        }
            //    }
            //}


        }
    }
}
