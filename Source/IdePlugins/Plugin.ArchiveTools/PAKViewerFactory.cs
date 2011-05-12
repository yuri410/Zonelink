using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide;
using Apoc3D.Ide.Designers;
using Apoc3D.Vfs;

namespace Plugin.ArchiveTools
{
    class PAKViewerFactory : DesignerAbstractFactory
    {
        public override DocumentBase CreateInstance(ResourceLocation res)
        {
            return new PAKViewer(this, res);
        }

        public override Type CreationType
        {
            get { throw new NotImplementedException(); }
        }

        public override string Description
        {
            get { return DevStringTable.Instance["DOCS:PAKViewer"]; }
        }

        public override string[] Filters
        {
            get { return new string[] { PAKViewer.Extension }; }
        }
    }
}
