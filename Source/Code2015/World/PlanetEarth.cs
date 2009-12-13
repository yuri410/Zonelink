using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Scene;

namespace Code2015.World
{
    class PlanetEarth : StaticModelObject
    {
        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
