using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Config;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;

namespace Plugin.GISTools
{
    class TextureColorCalculator : ConverterBase
    {
        IniSection iniSect;
        IniConfiguration ini;
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

                string dest = Path.Combine(path, "color.ini");
                ini = new IniConfiguration();

                iniSect = new IniSection("Textures");
                ini.Add(iniSect.Name, iniSect);

                for (int i = 0; i < files.Length; i++)
                {
                    Convert(new DevFileLocation(files[i]), null);

                    pd.Value = i;
                    Application.DoEvents();
                }
                pd.Close();
                pd.Dispose();

                ini.Save(dest);
            }
        }

        public unsafe override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            Stream stm = source.GetStream;

            Bitmap bmp = new Bitmap(stm);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            uint* src = (uint*)data.Scan0;

            ulong totalR = 0;
            ulong totalG = 0;
            ulong totalB = 0;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    uint clr = src[i * bmp.Width + j];

                    totalR += ((clr & 0x00ff000000) >> 16);
                    totalB += ((clr & 0x0000ff0000) >> 8);
                    totalB += clr & 0xff;
                }
            }
            totalR /= (ulong)(bmp.Width * bmp.Height);
            totalG /= (ulong)(bmp.Width * bmp.Height);
            totalB /= (ulong)(bmp.Width * bmp.Height);

            StringBuilder sb = new StringBuilder(50);
            sb.Append(totalR.ToString());
            sb.Append(", ");
            sb.Append(totalG.ToString());
            sb.Append(", ");
            sb.Append(totalB.ToString());

            iniSect.Add(source.Name, sb.ToString());

            stm.Close();
            bmp.Dispose();
        }

        public override string Name
        {
            get { return "纹理颜色计算器"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".png", ".bmp" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".ini" }; }
        }

        public override string SourceDesc
        {
            get { return "图像"; }
        }

        public override string DestDesc
        {
            get { return "配置文件"; }
        }
    }
}
