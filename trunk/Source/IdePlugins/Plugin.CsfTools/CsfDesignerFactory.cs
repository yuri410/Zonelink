using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide;
using Apoc3D.Ide.Designers;
using Apoc3D.Vfs;

namespace Plugin.CsfTools
{
    public class CsfDesignerFactory : DesignerAbstractFactory
    {
        public override DocumentBase CreateInstance(ResourceLocation res)
        {
            return new CsfDesigner(this, res);
        }

        public override string Description
        {
            get { return DevStringTable.Instance["DOCS:CSFDESC"]; }
        }

        public override string[] Filters
        {
            get { return new string[] { CsfDesigner.Extension }; }
        }



        public override Type CreationType
        {
            get { return typeof(CsfDesigner); }
        }

    }
}
