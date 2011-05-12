using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;
using Apoc3D.Graphics;

namespace Plugin.GISTools
{
    class TDmp32To12Converter : ConverterBase
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

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            TDMPIO srcData = new TDMPIO();
            srcData.Load(source);

            srcData.Bits = 12;
            srcData.Save(dest.GetStream);
        }

        public override string Name
        {
            get { return "32位至12位地形数据转换器"; }
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
            get { return "Terrain Displacement Map"; }
        }

        public override string DestDesc
        {
            get { return "Terrain Displacement Map"; }
        }
    }
}
