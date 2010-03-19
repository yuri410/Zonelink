using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.Graphics;
using Apoc3D;

namespace Code2015.World
{
    class LightningStrom : SceneObject
    {
        public LightningStrom()
            : base(false)
        {

        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime dt)
        {

        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
