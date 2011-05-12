using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;

namespace Plugin.ArchiveTools
{
    class PakArchivePacker : ConverterBase
    {
        public override void ShowDialog(object sender, EventArgs e)
        {
            PakPackerFrom frm = new PakPackerFrom();
            frm.ShowDialog();
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return "Pak Packing Tool"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".*" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { PAKViewer.Extension }; }
        }

        public override string SourceDesc
        {
            get { return "All Files"; }
        }

        public override string DestDesc
        {
            get { return "Package File"; }
        }
    }
}
