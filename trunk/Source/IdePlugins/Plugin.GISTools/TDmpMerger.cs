using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;

namespace Plugin.GISTools
{
    class TDmpMerger : ConverterBase
    {
        class DataGetter
        {
            float[] data1;
            float[] data2;
            float[] data3;
            float[] data4;

            int unitWid, unitHgt;
            
            public DataGetter(float[] d1, float[] d2, float[] d3, float[] d4, int uw, int uh)
            {
                this.unitHgt = uh;
                this.unitWid = uw;

                data1 = d1;
                data2 = d2;
                data3 = d3;
                data4 = d4;
            }


            public float this[int y, int x]
            {
                get
                {
                    if (y < unitHgt)
                    {
                        if (x < unitWid)
                        {
                            if (data1 == null)
                                return 0;
                            return data1[y * unitWid + x];
                        }
                        if (data2 == null)
                            return 0;
                        return data2[y * unitWid + x - unitWid];
                    }

                    if (x < unitWid)
                    {
                        if (data3 == null)
                            return 0;
                        return data3[(y - unitHgt) * unitWid + x];
                    }

                    if (data4 == null)
                        return 0;
                    return data4[(y - unitHgt) * unitWid + x - unitWid];
                }
            }
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
            for (int x = 1; x < 80; x += 2)
            {
                for (int y = 1; y < 36; y += 2)
                {
                    string file1 = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                    string file2 = Path.Combine(srcDir, "tile_" + (x + 1).ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                    string file3 = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + (y + 1).ToString("D2") + ".tdmp");
                    string file4 = Path.Combine(srcDir, "tile_" + (x + 1).ToString("D2") + "_" + (y + 1).ToString("D2") + ".tdmp");

                    bool ex1 = File.Exists(file1);
                    bool ex2 = File.Exists(file2);
                    bool ex3 = File.Exists(file3);
                    bool ex4 = File.Exists(file4);

                    if (ex1 || ex2 || ex3 || ex4)
                    {
                        int width = 0;
                        int height = 0;
                        float xSpan = 0;
                        float ySpan = 0;

                        float xllCorner = 0;
                        float yllCorner = 0;
                        int bits = 32;
                        bool parsed = false;

                        TDMPIO d1 = new TDMPIO();
                        if (ex1)
                        {
                            d1.Load(new DevFileLocation(file1));

                            if (!parsed)
                            {
                                width = d1.Width;
                                height = d1.Height;
                                xSpan = d1.XSpan;
                                ySpan = d1.YSpan;
                                xllCorner = d1.Xllcorner;
                                yllCorner = d1.Yllcorner;
                                bits = d1.Bits;
                            }
                        }

                        TDMPIO d2 = new TDMPIO();
                        if (ex2)
                        {
                            d2.Load(new DevFileLocation(file2));
                            if (!parsed)
                            {
                                width = d2.Width;
                                height = d2.Height;
                                xSpan = d2.XSpan;
                                ySpan = d2.YSpan;
                                xllCorner = d2.Xllcorner - xSpan;
                                yllCorner = d2.Yllcorner;
                                bits = d2.Bits;
                            }
                        }

                        TDMPIO d3 = new TDMPIO();
                        if (ex3)
                        {
                            d3.Load(new DevFileLocation(file3));
                            if (!parsed)
                            {
                                width = d3.Width;
                                height = d3.Height;
                                xSpan = d3.XSpan;
                                ySpan = d3.YSpan;
                                xllCorner = d3.Xllcorner;
                                yllCorner = d3.Yllcorner - ySpan;
                                bits = d3.Bits;
                            }
                        }

                        TDMPIO d4 = new TDMPIO();
                        if (ex4)
                        {
                            d4.Load(new DevFileLocation(file4));
                            if (!parsed)
                            {
                                width = d4.Width;
                                height = d4.Height;
                                xSpan = d4.XSpan;
                                ySpan = d4.YSpan;
                                xllCorner = d4.Xllcorner - xSpan;
                                yllCorner = d4.Yllcorner - ySpan;
                                bits = d4.Bits;
                            }
                        }

                        TDMPIO data = new TDMPIO();
                        data.Data = new float[width * height * 4];
                        data.Width = width * 2;
                        data.Height = height * 2;
                        data.Xllcorner = xllCorner;
                        data.Yllcorner = yllCorner;
                        data.XSpan = xSpan * 2;
                        data.YSpan = ySpan * 2;
                        data.Bits = bits;

                        DataGetter dg = new DataGetter(d1.Data, d2.Data, d3.Data, d4.Data, width, height);
                        for (int i = 0; i < data.Height; i++)
                        {
                            for (int j = 0; j < data.Width; j++)
                            {
                                data.Data[i * data.Width + j] = dg[i, j];
                            }
                        }

                        string outPath = Path.Combine(dstDir, "tile_" + x.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                        FileStream fs = new FileStream(outPath, FileMode.OpenOrCreate, FileAccess.Write);
                        data.Save(fs);
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
            get { return "高程数据合并工具"; }
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
            get { return "地形位移图"; }
        }

        public override string DestDesc
        {
            get { return "地形位移图"; }
        }
    }
}
