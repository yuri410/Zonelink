using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MapRepair
{
    class Program
    {
        static void Main(string[] args)
        {
            string url1 = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain\terrain_l1.tdmp";
            //string url2 = @"C:\Users\penser\Documents\Visual Studio 2008\Projects\lrvbsvnicg\Source\Code2015\bin\x86\Debug\terrain.lpk\terrain_l2.tdmp";

            BinaryReader br1 = new BinaryReader(File.Open(url1, FileMode.Open));

            int width = 36 * 129;
            int height = 14 * 129;

            ushort[,] data = new ushort[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    data[i, j] = br1.ReadUInt16();
                }
            }



            for (int i = 1; i < 14; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int I1 = 129 * i - 1;
                    int I2 = 129 * i;

                    int temp1 = data[I1, j];
                    int temp2 = data[I2, j];

                    data[I1, j] = (ushort)((temp1 + temp2) / 2);
                    data[I2, j] = (ushort)((temp1 + temp2) / 2);
                }
            }


            for (int i = 0; i < height; i++)
            {
                for (int j = 1; j < 36; j++)
                {
                    int J1 = 129 * j - 1;
                    int J2 = 129 * j;

                    int temp1 = data[i, J1];
                    int temp3 = data[i, J2];

                    data[i, J2] = (ushort)((temp1 + temp3) / 2);
                    data[i, J2] = (ushort)((temp1 + temp3) / 2);
                }
            }
            BinaryWriter bw = new BinaryWriter(File.Open(@"E:\Desktop\terrain_l1.tdmp", FileMode.OpenOrCreate));

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bw.Write(data[i, j]);
                }
            }
            bw.Close();

        }
    }
}
    
