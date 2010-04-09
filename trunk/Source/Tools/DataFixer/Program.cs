using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.World;
using Plugin.GISTools;
using System.Drawing.Imaging;
namespace DataFixer
{
    //class BitTable 
    //{
    //    int[] data;
    //    int size;

    //    public BitTable(int size)
    //    {
    //        this.size = size;

    //        data = new int[size];
    //    }

    //    public bool GetBit(int x)
    //    {
    //        int xx = x % 32;

    //        x = x / 32;

    //        int v = data[x];

    //        return ((v >> xx) & 1) != 0;
    //    }

    //    public void SetBit(int x, bool v)
    //    {
    //        int xx = x % 32;

    //        x = x / 32;

    //        //int vv = v ? 1 : 0;
    //        if (v)
    //        {
    //            data[x] |= (1 << xx);
    //        }
    //        else 
    //        {
    //            data[x] &= ~(1 << xx);
    //        }
    //    }


    //    public void Load(FileLocation fl)
    //    {
    //        ContentBinaryReader br = new ContentBinaryReader(fl);
    //        size = br.ReadInt32();
    //        data = new int[size];
    //        for (int i = 0; i < size; i++)
    //        {
    //            data[i] = br.ReadInt32();
    //        }
    //        br.Close();
    //    }

    //    public void Save(Stream stm)
    //    {
    //        ContentBinaryWriter bw = new ContentBinaryWriter(stm);

    //        bw.Write(size);
    //        for (int i = 0; i < size; i++)
    //        {
    //            bw.Write(data[i]);
    //        }

    //        bw.Close();
    //    }
    //}


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
                        int lat = 95 - y * 5;

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
                            if (dg.HasValue(i + height, width - 1))
                            {
                                result.Data[i * width] += dg[i + height, width - 1];
                                weightMap[i * width]++;
                            }
                            if (dg.HasValue(i + height, 2 * width))
                            {
                                result.Data[i * width + height - 1] += dg[i + height, 2 * width];
                                weightMap[i * width + height - 1]++;
                            }
                        }

                        for (int j = 0; j < width; j++)
                        {
                            if (dg.HasValue(height - 1, j + width))
                            {
                                result.Data[j] += dg[height - 1, j + width];
                                weightMap[j]++;
                            }
                            if (dg.HasValue(2 * height, j + width))
                            {
                                result.Data[width * (height - 1) + j] += dg[2 * height, j + width];
                                weightMap[width * (height - 1) + j]++;
                            }
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
        static void EdgeMix()
        {
            const string SrcDir = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk";
            const string DstDir = @"E:\Desktop\out";

            Console.WriteLine("Terrain data fix");

            Console.WriteLine("Process Lod 0...");
            Convert(SrcDir, DstDir, "_0");
            Console.WriteLine();
            Console.WriteLine("Process Lod 1..");
            Convert(SrcDir, DstDir, "_1");
            Console.WriteLine();
            Console.WriteLine("Process Lod 2..");
            Convert(SrcDir, DstDir, "_2");
            Console.WriteLine();
            Console.WriteLine("Process Lod 3..");
            Convert(SrcDir, DstDir, "_3");
            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadKey();

        }

        static void SrtmBath(string srcDir, string bathy)
        {
            const string tmpDir = @"E:\Desktop\tmp\";

            const int bathWidth = 18433;
            const int bathHeight = 9217;
            byte[] bathyData;

            BinaryReader br = new BinaryReader(File.Open(bathy, FileMode.Open));
            bathyData = br.ReadBytes(bathWidth * bathHeight);
            br.Close();

            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {
                    string file = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_0" + ".tdmp");
                    if (File.Exists(file))
                    {
                        string file2 = Path.Combine(tmpDir, "tile_" + x.ToString("D2") + "_" + (y + 6).ToString("D2") + "_0" + ".tdmp");
                        File.Copy(file, file2);
                    }
                }
            }


            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {
                    string file = Path.Combine(tmpDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_0" + ".tdmp");

                    int startX = (x - 1) * 256;
                    int startY = (y - 1) * 256;// +1536;

                    if (y > 3 && y < 33)
                    {
                        if (File.Exists(file))
                        {
                            TDMPIO d1 = new TDMPIO();
                            d1.Load(new DevFileLocation(file));
                            d1.XSpan *= 2;
                            d1.YSpan *= 2;
                            PlanetEarth.TileCoord2CoordNew(x, y, out d1.Xllcorner, out d1.Yllcorner);

                            float[] data = d1.Data;

                            for (int i = 0; i < d1.Height; i++)
                            {
                                for (int j = 0; j < d1.Width; j++)
                                {
                                    int idx = i * d1.Height + j;
                                    data[idx] *= 5000;

                                    data[idx] += 1500;
                                    data[idx] -= (0xff - bathyData[(startY + i) * bathWidth + startX + j]) * (1500f / 256f);

                                    //data[idx] /= 7000;
                                }
                            }

                            Stream sout = File.Open(Path.Combine(@"E:\Desktop\out\", Path.GetFileNameWithoutExtension(file) + ".tdmp"), FileMode.OpenOrCreate);
                            d1.Save(sout);




                            for (int i = 0; i < d1.Height; i++)
                            {
                                for (int j = 0; j < d1.Width; j++)
                                {
                                    int idx = i * d1.Height + j;
                                    data[idx] /= 7000;
                                }
                            }
                            TDmpBlur.OutPng(data, d1.Width, d1.Height, Path.Combine(@"E:\Desktop\out\",
                                Path.GetFileNameWithoutExtension(file) + ".png"));
                        }
                        else
                        {
                            TDMPIO d2 = new TDMPIO();
                            d2.Width = 513;
                            d2.Height = 513;
                            d2.XSpan = 10;
                            d2.YSpan = 10;
                            d2.Bits = 16;

                            PlanetEarth.TileCoord2CoordNew(x, y, out d2.Xllcorner, out d2.Yllcorner);

                            d2.Data = new float[513 * 513];

                            for (int i = 0; i < d2.Height; i++)
                            {
                                for (int j = 0; j < d2.Width; j++)
                                {
                                    int idx = i * d2.Height + j;

                                    d2.Data[idx] = 1600;
                                    d2.Data[idx] -= (0xff - bathyData[(startY + i) * bathWidth + startX + j]) * (1500f / 256f);
                                }
                            }

                            Stream sout = File.Open(Path.Combine(@"E:\Desktop\out\", Path.GetFileNameWithoutExtension(file) + ".tdmp"), FileMode.OpenOrCreate);
                            d2.Save(sout);



                            for (int i = 0; i < d2.Height; i++)
                            {
                                for (int j = 0; j < d2.Width; j++)
                                {
                                    int idx = i * d2.Height + j;
                                    d2.Data[idx] /= 7000;
                                }
                            }
                            TDmpBlur.OutPng(d2.Data, d2.Width, d2.Height, Path.Combine(@"E:\Desktop\out\",
                                Path.GetFileNameWithoutExtension(file) + ".png"));
                        }
                    }
                }
            }
        }

        static void ElevationBias()
        {
            const string SrcDir = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk";
            const string OutDir = @"E:\Desktop\out";
            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {
                    string file = Path.Combine(SrcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_0" + ".tdmp");
                    string file2 = Path.Combine(OutDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                    if (File.Exists(file))
                    {
                        TDMPIO d1 = new TDMPIO();
                        d1.Load(new DevFileLocation(file));

                        for (int i = 0; i < d1.Height; i++)
                        {
                            for (int j = 0; j < d1.Width; j++)
                            {
                                int idx = i * d1.Height + j;
                                d1.Data[idx] += 45; ;
                            }
                        }
                        Stream sout = File.Open(file2, FileMode.OpenOrCreate);
                        d1.Save(sout);
                    }
                }
            }
        }

        static void DoubleSize()
        {
            const string SrcDir = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk";
            const string OutDir = @"E:\Desktop\out";
            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {
                    string file = Path.Combine(SrcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_3.tdmp");
                    string file2 = Path.Combine(OutDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_3.tdmp");
                    if (File.Exists(file))
                    {
                        TDMPIO d1 = new TDMPIO();
                        d1.Load(new FileLocation(file));

                        d1.XSpan *= 2;
                        d1.YSpan *= 2;

                        Stream sout = File.Open(file2, FileMode.OpenOrCreate);
                        d1.Save(sout);
                    }
                }
            }
        }

        static void Simplify()
        {
            const string SrcDir = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk";
            const string OutDir = @"E:\Desktop\out";

            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {

                    string file = Path.Combine(SrcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_0" + ".tdmp");
                    //string file2 = Path.Combine(OutDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                    if (File.Exists(file))
                    {
                        bool passed = false;
                        TDMPIO d1 = new TDMPIO();
                        d1.Load(new DevFileLocation(file));

                        for (int i = 0; i < d1.Height; i++)
                        {
                            for (int j = 0; j < d1.Width; j++)
                            {
                                int idx = i * d1.Height + j;
                                if (d1.Data[idx] > 500)
                                {
                                    passed = true;
                                }
                            }
                        }

                        if (!passed) 
                        {
                            Console.WriteLine(file);
                            File.Delete(file);
                            file = Path.Combine(SrcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_1" + ".tdmp");
                            File.Delete(file);
                            file = Path.Combine(SrcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_2" + ".tdmp");
                            File.Delete(file);
                            file = Path.Combine(SrcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_3" + ".tdmp");
                            File.Delete(file);

                        }
                    }
                }
            }
        }

        static void BuildFlag()
        {
            bool[,] flags = new bool[36, 14];

            for (int x = 1, i = 0; x < 72; x += 2, i++)
            {
                for (int y = 5, j = 0; y < 33; y += 2, j++)
                {
                    string file = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk\tile_" + x.ToString("D2") + "_" + y.ToString("D2") + "_0.tdmp";

                    flags[i, j] = File.Exists(file);
                }
            }

            ContentBinaryWriter bw = new ContentBinaryWriter(File.Open(@"E:\Desktop\flags.dat", FileMode.OpenOrCreate));

            for (int i = 0; i < 36; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    bw.Write(flags[i, j]);
                }
            }

            bw.Close();
        }
        static void BuildLandArea() 
        {
            FileLocation fl = new FileLocation(@"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk\terrain_l0.tdmp");
            ContentBinaryReader br = new ContentBinaryReader(fl);

            const int DW = 36 * 513;
            const int DH = 14 * 513;

            ushort[,] data = new ushort[DH, DW];

            for (int i = 0; i < DH; i++)
            {
                for (int j = 0; j < DW; j++)
                {
                    ushort v = br.ReadUInt16();
                    data[i, j] = (v > (ushort)(1640 * 7)) ? (ushort)0 : ushort.MaxValue;// br.ReadUInt16();
                }
            }
            ContentBinaryWriter bw = new ContentBinaryWriter(File.Open(@"E:\Desktop\land.raw", FileMode.OpenOrCreate));
            for (int i = 0; i < DH; i++)
            {
                for (int j = 0; j < DW; j++)
                {
                    bw.Write(data[i, j]);
                }
            }
            bw.Close();

        }

        static void BuildLandAreaHeight()
        {
            FileLocation fl = new FileLocation(@"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain\terrain_l0.tdmp");
            ContentBinaryReader br = new ContentBinaryReader(fl);

            const int DW = 36 * 513;
            const int DH = 14 * 513;

            ushort[,] data = new ushort[DH, DW];

            for (int i = 0; i < DH; i++)
            {
                for (int j = 0; j < DW; j++)
                {
                    ushort v = br.ReadUInt16();
                    data[i, j] = (v > (ushort)(1600 * 7)) ? v : (ushort)0;
                }
            }
            ContentBinaryWriter bw = new ContentBinaryWriter(File.Open(@"E:\Desktop\land513.raw", FileMode.OpenOrCreate));
            for (int i = 0; i < DH; i++)
            {
                for (int j = 0; j < DW; j++)
                {
                    bw.Write(data[i, j]);
                }
            }
            bw.Close();

        }

        static void BuildBitMap()
        {
            FileLocation fl = new FileLocation(@"E:\Desktop\terrain_l2副本.raw");
            ContentBinaryReader br = new ContentBinaryReader(fl);

            const int DW = 36 * 32;
            const int DH = 14 * 32;

            ushort[,] data = new ushort[DH, DW];

            for (int i = 0; i < DH; i++)
            {
                for (int j = 0; j < DW; j++)
                {
                    ushort v = br.ReadUInt16();
                    data[i, j] = Math.Max(v, (ushort)(1640*7));// br.ReadUInt16();
                }
            }

            br.Close();

            int[,] dir = new int[DH, DW];

            for (int i = 0; i < DH - 1; i++) 
            {
                for (int j = 0; j < DW - 1; j++) 
                {
                    int dx = data[i + 1, j] - data[i, j];
                    int dy = data[i, j + 1] - data[i, j];
                    dir[i, j] = Math.Abs(dx) + Math.Abs(dy);
                }
            }
            for (int i = 0; i < DH - 1; i++)
            {
                int j = DW - 1;
                int dx = data[i + 1, j] - data[i, j];
                int dy = data[i, 0] - data[i, j];
                dir[i, j] = Math.Abs(dx) + Math.Abs(dy);
            }

            ContentBinaryWriter bw = new ContentBinaryWriter(File.Open(@"E:\Desktop\grad.raw", FileMode.OpenOrCreate));
            for (int i = 0; i < DH; i++)
            {
                for (int j = 0; j < DW; j++)
                {
                    if (dir[i, j] > ushort.MaxValue)
                        dir[i, j] = ushort.MaxValue;

                    bw.Write((ushort)dir[i, j]);
                }
            }
            bw.Close();
        }

        static void Build3() 
        {
            FileLocation fl = new FileLocation(@"E:\Desktop\pass32.raw");
            ContentBinaryReader br = new ContentBinaryReader(fl);

            const int DW = 36 * 32;
            const int DH = 14 * 32;


            Apoc3D.BitTable b = new Apoc3D.BitTable(DW * DH);
            for (int i = 0; i < DH; i++)
            {
                for (int j = 0; j < DW; j++)
                {
                    ushort v = br.ReadUInt16();

                    b.SetBit(i * DW + j, v > 32767);
                }
            }


            br.Close();
            b.Save(File.Open(@"E:\Desktop\aa.bit", FileMode.OpenOrCreate));
        }

        static unsafe void MergeAlpha(string path1, string path2)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(path1);
            System.Drawing.Bitmap alpha = new System.Drawing.Bitmap(path2);
            System.Drawing.Bitmap res = new System.Drawing.Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);

            BitmapData data1 = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData data2 = alpha.LockBits(new System.Drawing.Rectangle(0, 0, alpha.Width, alpha.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData data3 = res.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int* d1 = (int*)data1.Scan0;
            int* d2 = (int*)data2.Scan0;
            int* d3 = (int*)data3.Scan0;

            unchecked
            {
                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        *d3++ = ((*d2++ & 0x00ff0000) << 8) | (*d1++ & 0x00ffffff);
                    }
                }
            }


            bmp.UnlockBits(data1);
            alpha.UnlockBits(data2);

            res.Save(@"E:\Desktop\sss.png");
        }

        static void Main(string[] args)
        {
            MergeAlpha(@"E:\Desktop\新建文件夹\hospital ground.png", @"E:\Desktop\新建文件夹\edu groud副本.jpg");
            //FileSystem.Instance.AddWorkingDir(@"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug");
            //TerrainData.Initialize();
            //BuildBitMap();
            //float radlng = MathEx.Degree2Radian(133.678894f);
            //float radlat = MathEx.Degree2Radian(43.090955f);
            //Console.WriteLine(TerrainData.Instance.QueryHeight(radlng, radlat));
            //BuildLandAreaHeight();
            //BuildFlag();
            //Scan2(@"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk");
            Console.ReadKey();

            //const string SrcDir = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrain.lpk";

            //SrtmBath(SrcDir, @"E:\Desktop\bathy副本.raw");
            //Scan2(SrcDir);
            //Console.ReadKey();
        }
    }
}
