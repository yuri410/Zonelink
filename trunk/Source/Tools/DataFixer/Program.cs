using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Plugin.GISTools;

namespace DataFixer
{
    class Program
    {
        static void Convert(string srcDir, string dstDir, string suffix)
        {
            for (int x = 1; x < 80; x += 2)
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
                        result.Data = new float[result.Width * result.Height];

                        int[] weightMap = new int[width * height];

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                result.Data[i * width + j] = dg[i + width, j + height];
                                weightMap[i * width + j] = 1;
                            }
                        }

                        for (int i = 0; i < height; i++)
                        {
                            result.Data[i * width] += dg[width - 1, i + height];
                            weightMap[i * width]++;

                            result.Data[i * width + height - 1] += dg[2 * width, i + height];
                            weightMap[i * width + height - 1]++; 
                        }

                        for (int j = 0; j < width; j++)
                        {
                            result.Data[j] += dg[j + width, height - 1];
                            weightMap[j]++;

                            result.Data[width * (height - 1) + j] += dg[j + width, 2 * height];
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
                            for (int j = 0; j < height; j++)
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

        }
    }
}
