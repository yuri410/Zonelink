using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide.Designers;
using Apoc3D.Vfs;
using Apoc3D.Ide;

namespace Plugin.ArchiveTools
{
    class LPKViewerFactory : DesignerAbstractFactory
    {
        public override DocumentBase CreateInstance(ResourceLocation res)
        {
            return new LPKViewer(this, res);
        }

        public override Type CreationType
        {
            get { throw new NotImplementedException(); }
        }

        public override string Description
        {
            get { return DevStringTable.Instance["DOCS:LPKViewer"]; }
        }

        public override string[] Filters
        {
            get { return new string[] { LPKViewer.Extension }; }
        }
    }
}
