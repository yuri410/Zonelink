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
        const int Lod0Size = 32;
        const int Lod1Size = 8;


        OceanWaterData data0;
        OceanWaterData data1;

        RenderSystem renderSystem;

        RenderOperation[] opBuf0 = new RenderOperation[1];
        RenderOperation[] opBuf1 = new RenderOperation[1];


        public OceanWaterTile(RenderSystem rs, OceanWaterDataManager manager, int col, int lat)
            : base(false)
        {
            renderSystem = rs;

            data0 = manager.GetData(Lod0Size, lat);
            data1 = manager.GetData(Lod1Size, lat);

            opBuf0[0].Geomentry = data0.GeoData;
            opBuf1[0].Geomentry = data1.GeoData;
            
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            switch (level)
            {
                case 0:
                    return opBuf0;
                case 1:
                    return opBuf1;
                default:
                    return null;
            }
        }
        public override RenderOperation[] GetRenderOperation()
        {
            return GetRenderOperation(0);
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
