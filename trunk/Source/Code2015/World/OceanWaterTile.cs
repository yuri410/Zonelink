using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Scene;

namespace Code2015.World
{
    struct WaterVertex
    {

        public float Index;
    }

    class OceanWaterTile : SceneObject
    {
        VertexBuffer vertices;
        RenderSystem renderSystem;

        RenderOperation[] opBuf = new RenderOperation[1];

        public OceanWaterTile(RenderSystem rs)
            : base(false)
        {
            renderSystem = rs;


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
            get { throw new NotImplementedException(); }
        }
    }
}
