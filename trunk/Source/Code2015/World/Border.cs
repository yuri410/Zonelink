using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Scene;

namespace Code2015.World
{
    /// <summary>
    ///  表示边界
    /// </summary>
    class Border : StaticModelObject
    {

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
