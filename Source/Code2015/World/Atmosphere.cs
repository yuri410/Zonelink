using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Scene;

namespace Code2015.World
{
    class Atmosphere : SceneObject
    {
        public Atmosphere()
            : base(false)
        {

        }


        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime dt)
        {
            throw new NotImplementedException();
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
