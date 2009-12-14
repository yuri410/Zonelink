using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;

namespace Plugin.ArchiveTools
{
    class LpkArchivePacker : ConverterBase
    {       
        public override void ShowDialog(object sender, EventArgs e)
        {
            PackerFrom frm = new PackerFrom();
            frm.ShowDialog();
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return "Lpk打包工具"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".*" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { LPKViewer.Extension }; }
        }

        public override string SourceDesc
        {
            get { return "所有文件"; }
        }

        public override string DestDesc
        {
            get { return "Lpk文件包"; }
        }
    }
}
