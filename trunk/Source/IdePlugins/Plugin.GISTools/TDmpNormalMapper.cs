using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Vfs;

namespace Plugin.GISTools
{
    class TDmpNormalMapper : ConverterBase
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

        public static Vector3[] Resize(Vector3[] inp, int width, int height, int tw, int th)
        {
            Vector3[] result = new Vector3[tw * th];
            float wzoom = width / (float)tw;
            float hzoom = height / (float)th;

            Vector3[] buffer = new Vector3[16];
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

                    Vector3 s = Vector3.Zero;
                    for (int ii = 0; ii < 4; ii++)
                    {
                        Vector3 a = Vector3.Zero;
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

        public unsafe override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            const int twid = 512;
            const int thgt = 512;

            const float HeightScale = 33;

            TDMPIO src = new TDMPIO();
            src.Load(source);

            float[] data = src.Data;// TDmpLodGen.Resize(src.Data, src.Width, src.Height, twid + 1, thgt + 1);
            int width = src.Width - 1;
            int height = src.Height - 1;
            Vector3[] norm1 = new Vector3[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int idx = i * (src.Width) + j;

                    Vector3 u;
                    u.Y = HeightScale * (data[idx] - data[idx + 1]);
                    u.X = 1; u.Z = 0;

                    Vector3 v;
                    v.Y = HeightScale * (data[idx] - data[idx + src.Width]);
                    v.X = 0; v.Z = 1;


                    Vector3 n;
                    Vector3.Cross(ref v, ref u, out n);
                    n.Normalize();

                    norm1[i * width + j] = n;
                }
            }

            width = twid;
            height = thgt;

            int level = 0;
            while (width > 16) 
            {
                Vector3[] norm2;
                if (src.Width - 1 == width && src.Height - 1 == height)
                    norm2 = norm1;
                else
                    norm2 = Resize(norm1, src.Width - 1, src.Height - 1, width, height);

                ColorValue[] nrmColor = new ColorValue[norm2.Length];
                for (int i = 0; i < norm2.Length; i++)
                {
                    nrmColor[i] = new ColorValue((uint)MathEx.Vector2ARGB(ref norm2[i]));
                }

                TextureData nrmMap;
                nrmMap.LevelCount = 1;
                nrmMap.Format = ImagePixelFormat.A8R8G8B8;
                nrmMap.Type = TextureType.Texture2D;
                nrmMap.ContentSize = Apoc3D.Media.PixelFormat.GetMemorySize(width, height, 1, ImagePixelFormat.A8R8G8B8);
                nrmMap.Levels = new TextureLevelData[1];
                nrmMap.Levels[0].Depth = 1;
                nrmMap.Levels[0].Height = width;
                nrmMap.Levels[0].Width = height;
                nrmMap.Levels[0].LevelSize = nrmMap.ContentSize;
                nrmMap.Levels[0].Content = new byte[nrmMap.ContentSize];

                fixed (ColorValue* srcp = &nrmColor[0])
                {
                    fixed (byte* dstp = &nrmMap.Levels[0].Content[0])
                    {
                        Memory.Copy(srcp, dstp, nrmMap.ContentSize);
                    }
                }
                
               
                FileLocation fl = dest as FileLocation;
                if (fl != null)
                {
                    string file = Path.Combine(Path.GetDirectoryName(fl.Path),
                        Path.GetFileNameWithoutExtension(fl.Path) + "_" + level.ToString());
                  
                    nrmMap.Save(File.Open(file + TextureData.Extension, FileMode.OpenOrCreate, FileAccess.Write));

                    OutPng(nrmColor, width, height, file + ".png");
                }

                width /= 4;
                height /= 4;
                level++;
            }
        }

        public static void OutPng(ColorValue[] data, int width, int height, string file)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            unsafe
            {
                uint* dst = (uint*)bmpData.Scan0;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        dst[i * width + j] = data[i * width + j].PackedValue;
                    }
                }
            }

            bmp.UnlockBits(bmpData);
            bmp.Save(file, ImageFormat.Png);
            bmp.Dispose();
        }
        public override string Name
        {
            get { return "地形法线贴图生成器"; }
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
            get { return "法线贴图"; }
        }
    }
}
