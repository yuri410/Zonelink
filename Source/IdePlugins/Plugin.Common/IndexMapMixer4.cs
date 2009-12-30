using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;
using Apoc3D.Graphics;
using System.Drawing;
using System.Drawing.Imaging;

namespace Plugin.Common
{
    unsafe class IndexMapMixer4 : ConverterBase
    {
        const int ConCount = 4;
        const uint ConMax = 0xFF;
        struct Pixel
        {
            public fixed uint Data[ConCount];

            public uint this[int i]
            {
                get { fixed (uint* s = Data) return s[i]; }
                set { fixed (uint* s = Data) s[i] = value; }
            }

            public uint Total
            {
                get
                {
                    fixed (uint* s = Data)
                    {
                        uint result = 0;
                        for (int i = 0; i < ConCount; i++)
                        {
                            result += s[i];
                        }
                        return result;
                    }
                }
            }

            public void Normalize()
            {
                uint t = Total;


                fixed (uint* s = Data)
                {
                    if (t == 0)
                    {
                        for (int i = 0; i < ConCount; i++)
                        {
                            s[i] = ConMax;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ConCount; i++)
                        {
                            s[i] = (s[i] * ConMax) / t;
                        }
                    }
                }
            }
        }

        int componIndex;
        Pixel[] result;
        int width;
        int height;

        public override void ShowDialog(object sender, EventArgs e)
        {
            string[] files;
            string path;
            if (ConvDlg.Show("转换器", GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                ProgressDlg pd = new ProgressDlg(DevStringTable.Instance["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = files.Length;

                pd.Show();
                for (int i = 0; i < files.Length; i++)
                {
                    componIndex = i;

                    Convert(new DevFileLocation(files[i]), null);

                    pd.Value = i;
                    Application.DoEvents();
                }
                pd.Close();
                pd.Dispose();


                string dest = Path.Combine(path, "index.png");

                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                uint* dst = (uint*)data.Scan0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        result[i * width + j].Normalize();

                        ulong val = 0;
                        for (int k = 0; k < ConCount; k++)
                        {
                            val <<= 8;
                            //if (result[i * width + j][k] > ConMax)
                            //{
                            //    throw new Exception();
                            //}

                            val |= (result[i * width + j][k] & ConMax);
                        }
                        dst[i * width + j] = (uint)val;
                    }
                }
                bmp.Save(dest, ImageFormat.Png);

                bmp.Dispose();

                result = null;
            }
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            Bitmap bmp = new Bitmap(source.GetStream);
            int width = bmp.Width;
            int height = bmp.Height;
            this.width = width;
            this.height = height;

            if (result == null)
            {
                result = new Pixel[bmp.Width * bmp.Height];
            }

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            uint* src = (uint*)data.Scan0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    uint color = src[i * width + j];
                    uint alpha = 0xff & (color >> 24);

                    result[i * width + j][componIndex] += alpha;
                }
            }

            bmp.UnlockBits(data);

            bmp.Dispose();
        }


        public override string Name
        {
            get { return "索引混合器4通道"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".png", ".bmp", ".tga", ".tif" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".png" }; }
        }

        public override string SourceDesc
        {
            get { return "索引图片"; }
        }

        public override string DestDesc
        {
            get { return "png"; }
        }
    }
}
