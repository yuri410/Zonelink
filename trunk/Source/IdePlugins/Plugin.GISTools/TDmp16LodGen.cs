using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace Plugin.GISTools
{
    class TDmp16LodGen : ConverterBase
    {
        public override void ShowDialog(object sender, EventArgs e)
        {
            string[] files;
            string path;
            if (ConvDlg.Show("", GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                ProgressDlg pd = new ProgressDlg(DevStringTable.Instance["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = files.Length;

                pd.Show();
                for (int i = 0; i < files.Length; i++)
                {
                    string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + ".tdmp");

                    Convert(new DevFileLocation(files[i]), new DevFileLocation(dest));

                    pd.Value = i;
                    Application.DoEvents();
                }
                pd.Close();
                pd.Dispose();
            }
        }

        static float[] Resize(float[] inp, int width, int height, int tw, int th) 
        {
            float[] result = new float[tw * th];
            float wzoom = width / (float)tw;
            float hzoom = height / (float)th;

            float[] buffer = new float[16];
            float[] afu = new float[4];
            float[] afv = new float[4];

            for (int i = 0; i < th; i++)
            {
                float srcy = (i + 0.5f) * hzoom - 0.5f;
                int y0 = (int)srcy; if (y0 > srcy) --y0;

                for (int j = 0; j < tw; j++)
                {
                    float srcx = (j + 0.5f) * wzoom - 0.5f;

                    int x0 = (int)srcx; if (x0 > srcx) --x0;

                    float fv = srcx - x0;
                    float fu = srcy - y0;

                    for (int ii = 0; ii < 4; ii++)
                    {
                        for (int jj = 0; jj < 4; jj++)
                        {
                            int x = x0 + jj - 1;
                            int y = y0 + ii - 1;

                            if (x < 0) x = 0;
                            if (y < 0) y = 0;
                            if (x >= width) x = width - 1;
                            if (y >= height) y = height - 1;

                            buffer[ii * 4 + jj] = inp[y * width + x];
                        }
                    }

                    afu[0] = MathEx.Sinc(1 + fu);
                    afu[1] = MathEx.Sinc(fu);
                    afu[2] = MathEx.Sinc(1 - fu);
                    afu[3] = MathEx.Sinc(2 - fu);
                    afv[0] = MathEx.Sinc(1 + fv);
                    afv[1] = MathEx.Sinc(fv);
                    afv[2] = MathEx.Sinc(1 - fv);
                    afv[3] = MathEx.Sinc(2 - fv);

                    float s = 0;
                    for (int ii = 0; ii < 4; ii++)
                    {
                        float a = 0;
                        for (int jj = 0; jj < 4; jj++)
                        {
                            a += afu[jj] * buffer[ii * 4 + jj];
                        }
                        s += a * afv[ii];
                    }

                    result[i * th + j] = s;
                }
            }
            return result;
        }


        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            FileLocation fl = source as FileLocation;

            if (fl != null)
            {
                string path = fl.Path;

                string dir = Path.GetDirectoryName(path);
                string fn = Path.GetFileNameWithoutExtension(path);


                TDMP16IO srcData = new TDMP16IO();
                srcData.Load(source);

                float[] demData = Half.ConvertToSingle(srcData.Data);

                int tw, th;

                int level = 0;

                FileStream fs = new FileStream(Path.Combine(dir, fn + "_" + level++.ToString() + ".tdmp"), FileMode.OpenOrCreate, FileAccess.Write);
                srcData.Save(fs);


                do
                {
                    tw = srcData.Width / 2 + 1;
                    th = srcData.Height / 2 + 1;

                    demData = Resize(demData, srcData.Width, srcData.Height, tw, th);

                    TDMP16IO result;
                    result.Width = tw;
                    result.Height = th;
                    result.XSpan = srcData.XSpan;
                    result.YSpan = srcData.YSpan;
                    result.Xllcorner = srcData.Xllcorner;
                    result.Yllcorner = srcData.Yllcorner;

                    result.Data = Half.ConvertToHalf(demData);

                    fs = new FileStream(Path.Combine(dir, fn + "_" + level++.ToString() + ".tdmp"), FileMode.OpenOrCreate, FileAccess.Write);
                    result.Save(fs);
                }
                while (tw > 4 && th > 4);
            }
        }

        public override string Name
        {
            get { return "16位LOD地形数据生成器"; }
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
