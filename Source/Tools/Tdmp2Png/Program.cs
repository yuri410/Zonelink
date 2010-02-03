using System;
using System.Collections.Generic;
using System.Text;
using Plugin.GISTools;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using System.IO;

namespace Tdmp2Png
{
    class Program
    {
        static void Main(string[] args)
        {
            const string SrcDir = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk";

            const string DstDir = @"E:\Desktop\out";

            string fileName;
            do
            {
                fileName = Console.ReadLine();
                string pngPath = Path.Combine(DstDir, Path.GetFileNameWithoutExtension(fileName) + ".png");

                fileName = Path.Combine(SrcDir, fileName);

                if (File.Exists(fileName))
                {
                    TDMPIO d1 = new TDMPIO();
                    d1.Load(new DevFileLocation(fileName));
                    TDmpBlur.OutPng(d1.Data, d1.Width, d1.Height, pngPath);

                    Console.WriteLine("OK");
                }
                else
                {
                    Console.WriteLine("* File not exist.");
                }
            }
            while (fileName != "exit");
           
        }
    }
}
