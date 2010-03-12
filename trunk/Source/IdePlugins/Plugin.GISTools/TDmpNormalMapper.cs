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
                    //string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + ".tdmp");

                    Convert(new DevFileLocation(files[i]), null);

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
        public static float[] Resize(float[] inp, int width, int height, int tw, int th)
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

        public unsafe override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            const int TL = 513;
            const int DW = TL * 36;
            const int DH = TL * 14;
            const int NL = TL - 1;
            const int HeightScale = 33;

            ContentBinaryReader br = new ContentBinaryReader(source);
            float[] data = new float[DH * DW];
            for (int i = 0; i < DH; i++)
            {
                for (int j = 0; j < DW; j++)
                {
                    data[i * DW + j] = (br.ReadUInt16() / 7.0f)/7000f;
                    //data[i * DW + j] = br.ReadSingle();
                }
            }
            br.Close();

            // resample
            //data = Resize(data, DW, DH, DW, DH);

            //ContentBinaryWriter bw = new ContentBinaryWriter(File.Open(@"E:\Desktop\float.raw", FileMode.OpenOrCreate));
            //for (int i = 0; i < DH; i++)
            //    for (int j = 0; j < DW; j++)
            //        bw.Write(data[i * DW + j]);
            //bw.Close();

            //ContentBinaryWriter bw2 = new ContentBinaryWriter(File.Open(@"E:\Desktop\test.raw", FileMode.OpenOrCreate));
            //for (int i = 0; i < DH; i++)
            //    for (int j = 0; j < DW; j++)
            //        bw2.Write((ushort)(data[i * DW + j] * 7));
            //bw2.Close();

            for (int tx = 1; tx < 72; tx += 2)
            {
                for (int ty = 5; ty < 33; ty += 2)
                {
                    string onm = @"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug\terrainNormal.lpk\tile_" + tx.ToString("D2") + "_" + ty.ToString("D2") + "_0.tex";

                    if (!File.Exists(onm))
                        continue;

                    int baseI =  TL * (ty - 5) / 2;
                    int baseJ = TL * ((tx - 1) / 2);

                    float[] result = new float[TL * TL];
                    for (int i = 0; i < TL; i++)
                    {
                        
                        for (int j = 0; j < TL; j++)
                        {     

                            result[i * TL + j] = data[(baseI + i) * DW + baseJ + j];
                        }
                    }

                    Vector3[] norm1 = new Vector3[NL * NL];

                    for (int i = 0; i < NL; i++)
                    {
                        for (int j = 0; j < NL; j++)
                        {
                            int idx = i * (TL) + j;

                            Vector3 u;
                            u.Y = HeightScale * (result[idx] - result[idx + 1]);
                            u.X = 1; u.Z = 0;

                            Vector3 v;
                            v.Y = HeightScale * (result[idx] - result[idx + TL]);
                            v.X = 0; v.Z = 1;


                            Vector3 n;
                            Vector3.Cross(ref v, ref u, out n);
                            n.Normalize();

                            float tmp = n.Z;
                            n.Z = n.Y;
                            n.Y = tmp;

                            norm1[i * NL + j] = n;
                        }
                    }

                    ColorValue[] nrmColor = new ColorValue[norm1.Length];
                    for (int i = 0; i < norm1.Length; i++)
                    {
                        nrmColor[i] = new ColorValue((uint)MathEx.Vector2ARGB(ref norm1[i]));
                    }
                    OutPng(nrmColor, NL, NL, @"E:\Desktop\out\tile_" + tx.ToString("D2") + "_" + ty.ToString("D2") + "_0.png");

                }
            }


            //return;
            //const int twid = 512;
            //const int thgt = 512;

            //const float HeightScale = 33;

            //TDMPIO src = new TDMPIO();
            //src.Load(source);

            //float[] data = src.Data;// TDmpLodGen.Resize(src.Data, src.Width, src.Height, twid + 1, thgt + 1);
            //int width = src.Width - 1;
            //int height = src.Height - 1;
            //Vector3[] norm1 = new Vector3[width * height];

            //for (int i = 0; i < height; i++)
            //{
            //    for (int j = 0; j < width; j++)
            //    {
            //        int idx = i * (src.Width) + j;

            //        Vector3 u;
            //        u.Y = HeightScale * (data[idx] - data[idx + 1]);
            //        u.X = 1; u.Z = 0;

            //        Vector3 v;
            //        v.Y = HeightScale * (data[idx] - data[idx + src.Width]);
            //        v.X = 0; v.Z = 1;


            //        Vector3 n;
            //        Vector3.Cross(ref v, ref u, out n);
            //        n.Normalize();

            //        norm1[i * width + j] = n;
            //    }
            //}

            //width = twid;
            //height = thgt;

            //int level = 0;
            //while (width > 16) 
            //{
            //    Vector3[] norm2;
            //    if (src.Width - 1 == width && src.Height - 1 == height)
            //        norm2 = norm1;
            //    else
            //        norm2 = Resize(norm1, src.Width - 1, src.Height - 1, width, height);

            //    ColorValue[] nrmColor = new ColorValue[norm2.Length];
            //    for (int i = 0; i < norm2.Length; i++)
            //    {
            //        nrmColor[i] = new ColorValue((uint)MathEx.Vector2ARGB(ref norm2[i]));
            //    }

            //    TextureData nrmMap;
            //    nrmMap.LevelCount = 1;
            //    nrmMap.Format = ImagePixelFormat.A8R8G8B8;
            //    nrmMap.Type = TextureType.Texture2D;
            //    nrmMap.ContentSize = Apoc3D.Media.PixelFormat.GetMemorySize(width, height, 1, ImagePixelFormat.A8R8G8B8);
            //    nrmMap.Levels = new TextureLevelData[1];
            //    nrmMap.Levels[0].Depth = 1;
            //    nrmMap.Levels[0].Height = width;
            //    nrmMap.Levels[0].Width = height;
            //    nrmMap.Levels[0].LevelSize = nrmMap.ContentSize;
            //    nrmMap.Levels[0].Content = new byte[nrmMap.ContentSize];

            //    fixed (ColorValue* srcp = &nrmColor[0])
            //    {
            //        fixed (byte* dstp = &nrmMap.Levels[0].Content[0])
            //        {
            //            Memory.Copy(srcp, dstp, nrmMap.ContentSize);
            //        }
            //    }


            //    FileLocation fl = dest as FileLocation;
            //    if (fl != null)
            //    {
            //        string file = Path.Combine(Path.GetDirectoryName(fl.Path),
            //            Path.GetFileNameWithoutExtension(fl.Path) + "_" + level.ToString());

            //        nrmMap.Save(File.Open(file + TextureData.Extension, FileMode.OpenOrCreate, FileAccess.Write));

            //        OutPng(nrmColor, width, height, file + ".png");
            //    }

            //    width /= 4;
            //    height /= 4;
            //    level++;
            //}
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
