using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MapRepair
{
    class Program
    {
        static void Main(string[] args)
        {
            string url1 = @"C:\Users\penser\Documents\Visual Studio 2008\Projects\lrvbsvnicg\Source\Code2015\bin\x86\Debug\terrain.lpk\terrain_l1.tdmp";
            string url2 = @"C:\Users\penser\Documents\Visual Studio 2008\Projects\lrvbsvnicg\Source\Code2015\bin\x86\Debug\terrain.lpk\terrain_l2.tdmp";

            BinaryReader br1 = new BinaryReader(File.Open(url1, FileMode.Open));
            BinaryReader br2 = new BinaryReader(File.Open(url2, FileMode.Open));

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



            for (int i = 0; i < 14; i++)
            {
                int temp1 = 0;
                int temp2 = 0;
                int temp3 = 0;

                for (int j = 0; j < 36; j++)
                {
                    int I1 = 129 * i;
                    int I2 = 129 * i + 1;
                    int J1 = 129 * j;
                    int J2 = 129 * j + 1;

                    temp1 = data[I1, J1];
                    temp2 = data[I2, J1];
                    temp3 = data[I1, J2];

                    data[I2, J1] = (ushort)((temp1 + temp2) / 2);

                    data[I1, J2] = (ushort)((temp1 + temp3) / 2);
                }
            }

            BinaryWriter bw = new BinaryWriter(File.Open(@"C:\Users\penser\Desktop\FileMap129.raw", FileMode.OpenOrCreate));
          
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
    
