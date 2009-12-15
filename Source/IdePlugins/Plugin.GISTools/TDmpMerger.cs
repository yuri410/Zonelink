using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;
using System.Windows.Forms;
using System.IO;

namespace Plugin.GISTools
{
    class TDmpMerger : ConverterBase
    {
        string srcDir;

        public override void ShowDialog(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "选择输入目录";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                srcDir = dlg.SelectedPath;
                Convert();
            }
        }

        void Convert()
        {
            for (int x = 0; x < 80; x+=2) 
            {
                for (int y = 0; y < 36; y += 2)
                {
                    string file1 = Path.Combine(srcDir, "srtm_" + x.ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                    string file2 = Path.Combine(srcDir, "tile_" + (x + 1).ToString("D2") + "_" + y.ToString("D2") + ".tdmp");
                    string file3 = Path.Combine(srcDir, "tile_" + x.ToString("D2") + "_" + (y + 1).ToString("D2") + ".tdmp");
                    string file4 = Path.Combine(srcDir, "tile_" + (x + 1).ToString("D2") + "_" + (y + 1).ToString("D2") + ".tdmp");

                    bool ex1 = File.Exists(file1);
                    bool ex2 = File.Exists(file2);
                    bool ex3 = File.Exists(file3);
                    bool ex4 = File.Exists(file4);
                    
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
