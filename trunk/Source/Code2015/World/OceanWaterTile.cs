using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;

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

        float tileCol;
        float tileLat;


        public OceanWaterTile(RenderSystem rs, OceanWaterDataManager manager, int @long, int lat)
            : base(false)
        {
            renderSystem = rs;

            PlanetEarth.TileCoord2Coord(@long, lat, out tileCol, out tileLat);

            data0 = manager.GetData(Lod0Size, tileLat);
            data1 = manager.GetData(Lod1Size, tileLat);

            opBuf0[0].Geomentry = data0.GeoData;
            opBuf1[0].Geomentry = data1.GeoData;

            float radtc = MathEx.Degree2Radian(tileCol);
            float radtl = MathEx.Degree2Radian(tileLat);
            float rad5 = MathEx.Degree2Radian(5);

            BoundingSphere.Center = PlanetEarth.GetPosition(radtc + rad5, radtl - rad5);
            BoundingSphere.Radius = PlanetEarth.GetTileHeight(rad5 * 2);

            Transformation = Matrix.RotationY(radtc);
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
