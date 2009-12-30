using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;
using System.Windows.Forms;
using System.IO;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Apoc3D.MathLib;

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

            bool IsIn(int index, int x, int y, out Rectangle rect)
            {
                rect = GetRect(index);
                return rect.Contains(x, y);
            }
            Rectangle GetRect(int index)
            {
                switch (index)
                {
                    case 0:
                        return new Rectangle(0, 0,
                            unitWid, unitHgt);
                    case 1:
                        return new Rectangle(unitWid, 0,
                            unitWid, unitHgt);
                    case 2:
                        return new Rectangle(unitWid * 2, 0,
                            unitWid, unitHgt);
                    case 3:
                        return new Rectangle(0, unitHgt,
                            unitWid, unitHgt);
                    case 4:
                        return new Rectangle(unitWid, unitHgt,
                            unitWid, unitHgt);
                    case 5:
                        return new Rectangle(unitWid * 2, unitHgt,
                            unitWid, unitHgt);
                    case 6:
                        return new Rectangle(0, unitHgt * 2,
                            unitWid, unitHgt);
                    case 7:
                        return new Rectangle(unitWid, unitHgt * 2,
                            unitWid, unitHgt);
                    case 8:
                        return new Rectangle(unitWid * 2, unitHgt * 2,
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
                        Rectangle rect;
                        if (IsIn(i, x, y, out rect))
                        {
                            x -= rect.X;
                            y -= rect.Y;

                            return data[i][y * unitWid + x];
                        }
                    }
                    return 0;

                }
            }

            public float[] MainData { get { return data[4]; } }
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

                    if (minY < 0) minY = 23;
                    if (maxY > 72) maxY = 1;

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

                    if (passed)
                    {
                        int width = 0;
                        int height = 0;
                        float xSpan = 0;
                        float ySpan = 0;

                        float xllCorner = 0;
                        float yllCorner = 0;
                        int bits = 32;
                        bool parsed = false;

                        TDMPIO[] dataBlocks = new TDMPIO[9];
                        for (int i = 0; i < 9; i++)
                        {
                            TDMPIO d1 = new TDMPIO();
                            d1.Load(new DevFileLocation(files[i]));
                            if (!parsed)
                            {
                                width = d1.Width;
                                height = d1.Height;
                                xSpan = d1.XSpan;
                                ySpan = d1.YSpan;
                                xllCorner = d1.Xllcorner;
                                yllCorner = d1.Yllcorner;
                                bits = d1.Bits;
                                parsed = true;
                            }

                            dataBlocks[i] = d1;
                        }

                        float[] data = new float[width * height * 9];
                        width = width * 3;
                        height = height * 3;

                        DataGetter dg = new DataGetter(
                            dataBlocks[0].Data, dataBlocks[1].Data, dataBlocks[2].Data,
                            dataBlocks[3].Data, dataBlocks[4].Data, dataBlocks[5].Data,
                            dataBlocks[6].Data, dataBlocks[7].Data, dataBlocks[8].Data, width, height);

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                data[i * width + j] = dg[i, j];
                            }
                        }

                        #region 高斯模糊
                        int brushSize = 5;

                        float[][] weights = MathEx.ComputeGuassFilter2D((float)Math.Sqrt(0.4 * brushSize), brushSize);

                        #region Normalize
                        float maxValue = 0;
                        for (int i = 0; i < brushSize; i++)
                        {
                            for (int j = 0; j < brushSize; j++)
                            {
                                if (maxValue < weights[i][j])
                                {
                                    maxValue = weights[i][j];
                                }
                            }
                        }

                        if (maxValue != 0)
                        {
                            float scale = 1.0f / maxValue;

                            for (int i = 0; i < brushSize; i++)
                            {
                                for (int j = 0; j < brushSize; j++)
                                {
                                    maxValue *= scale;
                                }
                            }
                        }
                        #endregion

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                //data[i * width + j] += gaussFilter[j][i] * invHeightScale * delta * 0.8f;
                            }
                        }
                        #endregion
                        //string outPath = Path.Combine(dstDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                        //FileStream fs = new FileStream(outPath, FileMode.OpenOrCreate, FileAccess.Write);
                        //data.Save(fs);
                    }
                }
            }
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
