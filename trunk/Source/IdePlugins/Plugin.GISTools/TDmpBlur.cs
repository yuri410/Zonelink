using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using System.Drawing.Imaging;

namespace Plugin.GISTools
{
    class TDmpBlur : ConverterBase
    {
        class DataGetter
        {
            float[][] data;

            int unitWid, unitHgt;

            public DataGetter(float[] d1, float[] d2, float[] d3,
                float[] d4, float[] d5, float[] d6,
                float[] d7, float[] d8, float[] d9, int uw, int uh)
            {
                this.unitHgt = uh;
                this.unitWid = uw;

                data = new float[9][];
                data[0] = d1;
                data[1] = d2;
                data[2] = d3;
                data[3] = d4;
                data[4] = d5;
                data[5] = d6;
                data[6] = d7;
                data[7] = d8;
                data[8] = d9;
            }

            bool IsIn(int index, int x, int y, out Apoc3D.MathLib.Rectangle rect)
            {
                rect = GetRect(index);
                return rect.Contains(x, y);
            }
            Apoc3D.MathLib.Rectangle GetRect(int index)
            {
                switch (index)
                {
                    case 0:
                        return new Apoc3D.MathLib.Rectangle(0, 0,
                            unitWid, unitHgt);
                    case 1:
                        return new Apoc3D.MathLib.Rectangle(unitWid, 0,
                            unitWid, unitHgt);
                    case 2:
                        return new Apoc3D.MathLib.Rectangle(unitWid * 2, 0,
                            unitWid, unitHgt);
                    case 3:
                        return new Apoc3D.MathLib.Rectangle(0, unitHgt,
                            unitWid, unitHgt);
                    case 4:
                        return new Apoc3D.MathLib.Rectangle(unitWid, unitHgt,
                            unitWid, unitHgt);
                    case 5:
                        return new Apoc3D.MathLib.Rectangle(unitWid * 2, unitHgt,
                            unitWid, unitHgt);
                    case 6:
                        return new Apoc3D.MathLib.Rectangle(0, unitHgt * 2,
                            unitWid, unitHgt);
                    case 7:
                        return new Apoc3D.MathLib.Rectangle(unitWid, unitHgt * 2,
                            unitWid, unitHgt);
                    case 8:
                        return new Apoc3D.MathLib.Rectangle(unitWid * 2, unitHgt * 2,
                            unitWid, unitHgt);

                }
                throw new ArgumentOutOfRangeException("index");
            }

            public float this[int y, int x]
            {
                get
                {
                    for (int i = 0; i < 9; i++)
                    {
                        Apoc3D.MathLib.Rectangle rect;
                        if (IsIn(i, x, y, out rect))
                        {
                            if (data[i] == null)
                                break;
                            x -= rect.X;
                            y -= rect.Y;

                            return data[i][y * unitWid + x];
                        }
                    }
                    return 0;

                }
            }

            public float[] MainData { get { return data[4]; } set { data[4] = value; } }
        }

        string srcDir;
        string dstDir;

        public override void ShowDialog(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "选择输入目录";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                srcDir = dlg.SelectedPath;
                dlg = new FolderBrowserDialog();
                dlg.Description = "选择输出目录";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    dstDir = dlg.SelectedPath;
                    Convert();
                }
            }
        }

        void Convert()
        {
            for (int x = 1; x < 72; x += 2)
            {
                for (int y = 1; y < 24; y += 2)
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
                    files[0] = Path.Combine(srcDir, "tile_" + minX.ToString("D2") + "_" + minY.ToString("D2") + ".tdmp");
                    files[1] = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + minY.ToString("D2") + ".tdmp");
                    files[2] = Path.Combine(srcDir, "tile_" + maxX.ToString("D2") + "_" + minY.ToString("D2") + ".tdmp");

                    files[3] = Path.Combine(srcDir, "tile_" + minX.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                    files[4] = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                    files[5] = Path.Combine(srcDir, "tile_" + maxX.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");

                    files[6] = Path.Combine(srcDir, "tile_" + minX.ToString("D2") + "_" + maxY.ToString("D2") + ".tdmp");
                    files[7] = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + maxY.ToString("D2") + ".tdmp");
                    files[8] = Path.Combine(srcDir, "tile_" + maxX.ToString("D2") + "_" + maxY.ToString("D2") + ".tdmp");

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

                        #region 高斯模糊
                        int brushSize = 5;

                        float[] weights = MathEx.ComputeGuassFilter1D((float)Math.Sqrt(0.4 * brushSize), brushSize);

                        #region Normalize
                        float maxValue = 0;
                        for (int i = 0; i < brushSize; i++)
                        {
                            maxValue += weights[i];
                        }

                        if (maxValue != 0)
                        {
                            float scale = 1.0f / maxValue;

                            for (int i = 0; i < brushSize; i++)
                            {
                                weights[i] *= scale;
                            }
                        }

                        #endregion

                        int mid = brushSize / 2;

                        #region Y方向模糊
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                float val = 0;
                                for (int ii = 0; ii < brushSize; ii++)
                                {
                                    val += weights[ii] * dg[i + ii - mid + width, j + height];
                                }
                                databy[i * width + j] = val;
                            }
                        }
                        #endregion

                        float[] original = dg.MainData;
                        float[] databx = new float[width * height];
                        dg.MainData = databy;

                        #region X方向模糊
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                float val = 0;
                                for (int ii = 0; ii < brushSize; ii++)
                                {
                                    val += weights[ii] * dg[i + width, j + ii - mid + height];
                                }
                                databx[i * width + j] = val;
                            }
                        }
                        #endregion

                        #endregion

                        string fileName = Path.Combine(dstDir, Path.GetFileName(files[4]) );
                        string fileNameImg = Path.Combine(dstDir, Path.GetFileName(files[4]) + "s.png");
                        string fileNameImg2 = Path.Combine(dstDir, Path.GetFileName(files[4]) + "d.png");

                        TDMPIO result = new TDMPIO();
                        result.Data = databx;
                        result.Bits = 32;
                        result.Height = height;
                        result.Width = width;
                        result.XSpan = dataBlocks[4].XSpan;
                        result.YSpan = dataBlocks[4].YSpan;
                        result.Xllcorner = dataBlocks[4].Xllcorner;
                        result.Yllcorner = dataBlocks[4].Yllcorner;

                        result.Save(File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write));

                        #region 预览图
                        OutPng(databx, width, height, fileNameImg);
                        OutPng(original, width, height, fileNameImg2);
                        #endregion
                    }
                }
            }
        }


        void OutPng(float [] data, int width, int height, string file)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                uint* dst = (uint*)bmpData.Scan0;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int val = (int)(255 * data[i * width + j]);
                        if (val < 0)
                            val = 0;
                        if (val > 255)
                            val = 255;
                        dst[i * width + j] = ColorValue.PackValue(val, val, val, 255);
                    }
                }
            }

            bmp.UnlockBits(bmpData);
            bmp.Save(file, ImageFormat.Png);
            bmp.Dispose();
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return "高程数据模糊工具"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".tdmp" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".tdmp" }; }
        }

        public override string SourceDesc
        {
            get { return "高程数据"; }
        }

        public override string DestDesc
        {
            get { return "高程数据"; }
        }
    }
}
