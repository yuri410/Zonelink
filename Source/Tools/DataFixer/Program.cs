using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Plugin.GISTools;
using Apoc3D.MathLib;

namespace DataFixer
{
    class Program
    {
        static void Scan2(string srcDir)
        {
           
            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {
                    string file = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_3" + ".tdmp");

                    if (File.Exists(file))
                    {
                        TDMPIO d1 = new TDMPIO();
                        d1.Load(new DevFileLocation(file));

                        Point coord;
                        coord.X = (int)Math.Truncate(d1.Xllcorner);
                        coord.Y = (int)Math.Truncate(d1.Yllcorner);

                        int col = x * 5 - 185;
                        int lat = 50 - y * 5;

                        if (col != coord.X || lat != coord.Y)
                        {
                            Console.WriteLine("Err: " + Path.GetFileNameWithoutExtension(file));
                            d1.Xllcorner = col;
                            d1.Yllcorner = lat;

                            Stream stm = File.Open(file, FileMode.Open);
                            stm.SetLength(0);
                            d1.Save(stm);
                        }

                        
                    }
                }
            }


        }
        static void Scan(string srcDir)
        {
            Dictionary<Point, List<string>> table = new Dictionary<Point, List<string>>(500);

            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {
                    string file = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_0" + ".tdmp");

                    if (File.Exists(file))
                    {
                        TDMPIO d1 = new TDMPIO();
                        d1.Load(new DevFileLocation(file));

                        Point coord;
                        coord.X = (int)Math.Truncate(d1.Xllcorner);
                        coord.Y = (int)Math.Truncate(d1.Yllcorner);

                        List<string> list;
                        if (!table.TryGetValue(coord, out list))
                        {
                            list = new List<string>();
                            table.Add(coord, list);
                        }
                        list.Add(file);
                    }
                }
            }

            Dictionary<Point, List<string>>.ValueCollection vals = table.Values;
            foreach (List<string> lst in vals)
            {
                if (lst.Count > 1)
                {
                    Console.Write("Collision: ");
                    for (int i = 0; i < lst.Count; i++)
                    {
                        Console.Write(Path.GetFileNameWithoutExtension(lst[i]));
                        if (i < lst.Count - 1)
                        {
                            Console.Write(", ");
                        }
                    }
                    Console.WriteLine();
                }
            }
            Console.ReadKey();
        }
        static void Convert(string srcDir, string dstDir, string suffix)
        {
            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {
                    int minX = x - 2;
                    int maxX = x + 2;
                    int minY = y - 2;
                    int maxY = y + 2;

                    if (minX < 0) minX = 71;
                    if (maxX > 72) maxX = 1;

                    //if (minY < 0) minY = 23;
                    //if (maxY > 72) maxY = 1;

                    string[] files = new string[9];
                    files[0] = Path.Combine(srcDir, "tile_" + minX.ToString("D2") + "_" + minY.ToString("D2") + suffix + ".tdmp");
                    files[1] = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + minY.ToString("D2") + suffix + ".tdmp");
                    files[2] = Path.Combine(srcDir, "tile_" + maxX.ToString("D2") + "_" + minY.ToString("D2") + suffix + ".tdmp");

                    files[3] = Path.Combine(srcDir, "tile_" + minX.ToString("D2") + "_" + y.ToString("D2") + suffix + ".tdmp");
                    files[4] = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + suffix + ".tdmp");
                    files[5] = Path.Combine(srcDir, "tile_" + maxX.ToString("D2") + "_" + y.ToString("D2") + suffix + ".tdmp");

                    files[6] = Path.Combine(srcDir, "tile_" + minX.ToString("D2") + "_" + maxY.ToString("D2") + suffix + ".tdmp");
                    files[7] = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + maxY.ToString("D2") + suffix + ".tdmp");
                    files[8] = Path.Combine(srcDir, "tile_" + maxX.ToString("D2") + "_" + maxY.ToString("D2") + suffix + ".tdmp");

                    bool[] exists = new bool[9];
                    bool passed = false;
                    for (int i = 0; i < files.Length; i++)
                    {
                        exists[i] = File.Exists(files[i]);
                        passed |= exists[i];
                    }
                    passed &= exists[4];

                    if (passed)
                    {
                        int width = 0;
                        int height = 0;

                        int bits = 32;
                        bool parsed = false;

                        TDMPIO[] dataBlocks = new TDMPIO[9];
                        for (int i = 0; i < 9; i++)
                        {
                            if (exists[i])
                            {
                                TDMPIO d1 = new TDMPIO();
                                d1.Load(new DevFileLocation(files[i]));
                                if (!parsed)
                                {
                                    width = d1.Width;
                                    height = d1.Height;

                                    bits = d1.Bits;
                                    parsed = true;
                                }

                                dataBlocks[i] = d1;
                            }
                        }

                        float[] databy = new float[width * height];

                        DataGetter dg = new DataGetter(
                            dataBlocks[0] == null ? null : dataBlocks[0].Data,
                            dataBlocks[1] == null ? null : dataBlocks[1].Data,
                            dataBlocks[2] == null ? null : dataBlocks[2].Data,
                            dataBlocks[3] == null ? null : dataBlocks[3].Data,
                            dataBlocks[4] == null ? null : dataBlocks[4].Data,
                            dataBlocks[5] == null ? null : dataBlocks[5].Data,
                            dataBlocks[6] == null ? null : dataBlocks[6].Data,
                            dataBlocks[7] == null ? null : dataBlocks[7].Data,
                            dataBlocks[8] == null ? null : dataBlocks[8].Data, width, height);


                        string fileName = Path.Combine(dstDir, Path.GetFileName(files[4]));
                      
                        TDMPIO result = new TDMPIO();
                        result.Bits = 16;
                        result.Height = height;
                        result.Width = width;
                        result.XSpan = dataBlocks[4].XSpan;
                        result.YSpan = dataBlocks[4].YSpan;
                        result.Xllcorner = dataBlocks[4].Xllcorner;
                        result.Yllcorner = dataBlocks[4].Yllcorner;
                        result.Data = new float[width * height];

                        int[] weightMap = new int[width * height];

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                result.Data[i * width + j] = dg[i + height, j + width];
                                weightMap[i * width + j] = 1;
                            }
                        }

                        for (int i = 0; i < height; i++)
                        {
                            result.Data[i * width] += dg[i + height, width - 1];
                            weightMap[i * width]++;

                            result.Data[i * width + height - 1] += dg[i + height, 2 * width];
                            weightMap[i * width + height - 1]++; 
                        }

                        for (int j = 0; j < width; j++)
                        {
                            result.Data[j] += dg[height - 1, j + width];
                            weightMap[j]++;

                            result.Data[width * (height - 1) + j] += dg[2 * height, j + width];
                            weightMap[width * (height - 1) + j]++;
                        }


                        //result.Data[0] += dg[width - 1, height - 1];
                        //weightMap[0]++;

                        //result.Data[width - 1] += dg[2 * width, height - 1];
                        //weightMap[width - 1]++;

                        //result.Data[width * (height - 1)] += dg[width - 1, 2 * height];
                        //weightMap[width * (height - 1) - 1]++;

                        //result.Data[width * height - 1] += dg[2 * width, 2 * height];
                        //weightMap[width * height - 1]++;

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                int idx = i * width + j;
                                result.Data[idx] /= (float)weightMap[idx];
                            }
                        }

                        result.Save(File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write));
                        Console.Write('#');
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            const string SrcDir = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk";
            const string DstDir = @"E:\Desktop\out";

            Console.WriteLine("Terrain data fix");

            Console.WriteLine("Process Lod 0...");
            Convert(SrcDir, DstDir, "_0");

            Console.WriteLine("Process Lod 1..");
            Convert(SrcDir, DstDir, "_1");

            Console.WriteLine("Process Lod 2..");
            Convert(SrcDir, DstDir, "_2");

            Console.WriteLine("Process Lod 3..");
            Convert(SrcDir, DstDir, "_3");
            Console.WriteLine("Done.");
            Console.ReadKey();
            //Scan2(SrcDir);
            //Console.ReadKey();
        }
    }
}
