using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide.Converters;
using System.Windows.Forms;
using Apoc3D.Ide;
using System.IO;
using SlimDX.Direct3D9;
using Apoc3D.Graphics;
using Apoc3D.Core;

namespace Plugin.Common.Converter
{
    class Model2XConverter : ConverterBase
    {
        Direct3D d3d;
        Device device;

        public override void ShowDialog(object sender, EventArgs e)
        {
            if (d3d == null)
            {
                d3d = new Direct3D();
                var pm = new SlimDX.Direct3D9.PresentParameters();
                pm.Windowed = true;
                device = new Device(d3d, 0, DeviceType.Reference, IntPtr.Zero, CreateFlags.FpuPreserve, pm);
            }

            string[] files;
            string path;
            if (ConvDlg.Show(Name, GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                ProgressDlg pd = new ProgressDlg(DevStringTable.Instance["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = files.Length;

                pd.Show();
                for (int i = 0; i < files.Length; i++)
                {
                    string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + ".x");

                    Convert(new DevFileLocation(files[i]), new DevFileLocation(dest));
                    pd.Value = i;
                }
                pd.Close();
                pd.Dispose();
            }
        }

        public override void Convert(Apoc3D.Vfs.ResourceLocation source, Apoc3D.Vfs.ResourceLocation dest)
        {
            ModelData mdlData = new ModelData();
            //Model model = new Model(new ResourceHandle<ModelData>(null, true));


        }

        public override string Name
        {
            get { return "Mesh to X(模型提取)"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".mesh" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".x" }; }
        }

        public override string SourceDesc
        {
            get { return "Apoc3D Engine Model"; }
        }

        public override string DestDesc
        {
            get { return "DirectX X File"; }
        }
    }
}
