using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.Graphics;
using Apoc3D;

namespace Code2015.World
{
    class TerrainBlock : SceneObject
    {
        public TerrainBlock()
            : base(true)
        { }

        public override void PrepareVisibleObjects(ICamera cam)
        {
            base.PrepareVisibleObjects(cam);
        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        public override bool IsSerializable
        {
            get { throw new NotImplementedException(); }
        }
    }
}
