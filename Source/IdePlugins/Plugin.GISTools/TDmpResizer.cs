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
    class TDmpResizer : ConverterBase
    {
        const int ResizeSize = 513;

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

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            TDMPIO src = new TDMPIO();
            src.Load(source);

            float[] rsd = TDmpLodGen.Resize(src.Data, src.Width, src.Height, ResizeSize, ResizeSize);

            TDMPIO dst = new TDMPIO();
            dst.Width = ResizeSize;
            dst.Height = ResizeSize;
            dst.Bits = src.Bits;
            dst.Xllcorner = src.Xllcorner;
            dst.Yllcorner = src.Yllcorner;
            dst.XSpan = src.XSpan;
            dst.YSpan = src.YSpan;

            dst.Data = rsd;
            dst.Save(dest.GetStream);
        }

        public override string Name
        {
            get { return "高程数据调整工具"; }
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
