using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Scene;
using Apoc3D.MathLib;

namespace Code2015.World
{
    struct WaterVertex
    {
        public Vector3 Position;

        public float Index;

        public static int Size
        {
            get { return Vector3.SizeInBytes + sizeof(float); }
        }

        static readonly VertexElement[] elements;

        static WaterVertex()
        {
            elements = new VertexElement[2];
            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0);

        }

        public static VertexElement[] Elements
        {
            get { return elements; }
        }
    }

    class OceanWaterTile : SceneObject
    {

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
            get { return false; }
        }
    }
}
