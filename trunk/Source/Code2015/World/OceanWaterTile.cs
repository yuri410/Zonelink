using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;

namespace Code2015.World
{
    struct WaterVertex
    {
        public Vector3 Position;

        public Vector2 NormalCoord;

        public static int Size
        {
            get { return Vector3.SizeInBytes + Vector2.SizeInBytes; }
        }

        static readonly VertexElement[] elements;

        static WaterVertex()
        {
            elements = new VertexElement[2];
            elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);

        }

        public static VertexElement[] Elements
        {
            get { return elements; }
        }
    }

    class OceanWaterTile : SceneObject
    {
        const int Lod0Size = 8;

        OceanWaterData data0;

        RenderSystem renderSystem;

        RenderOperation[] opBuf = new RenderOperation[1];

        float tileCol;
        float tileLat;

        Material material;

        public OceanWaterTile(RenderSystem rs, OceanWaterDataManager manager, int @long, int lat)
            : base(false)
        {
            renderSystem = rs;

            PlanetEarth.TileCoord2CoordNew(@long, lat, out tileCol, out tileLat);

            material = new Material(rs);

            FileLocation fl = FileSystem.Instance.Locate("WaterNormal.tex", GameFileLocs.Nature);
            ResourceHandle<Texture> map = TextureManager.Instance.CreateInstance(fl);
            material.SetTexture(1, map);

            fl = FileSystem.Instance.Locate("WaterDudv.tex", GameFileLocs.Nature);
            map = TextureManager.Instance.CreateInstance(fl);
            material.SetTexture(0, map);

            material.SetEffect(EffectManager.Instance.GetModelEffect(WaterEffectFactory.Name));
            material.IsTransparent = true;
            material.ZWriteEnabled = false;
            material.ZEnabled = true;
            material.CullMode = CullMode.CounterClockwise;
            material.PriorityHint = RenderPriority.Third;

            data0 = manager.GetData(Lod0Size, tileLat);
            //data1 = manager.GetData(Lod1Size, tileLat);


            float radtc = MathEx.Degree2Radian(tileCol);
            float radtl = MathEx.Degree2Radian(tileLat);
            float rad5 = PlanetEarth.DefaultTileSpan * 0.5f;

            BoundingSphere.Center = PlanetEarth.GetPosition(radtc + rad5, radtl - rad5);
            BoundingSphere.Radius = PlanetEarth.GetTileHeight(rad5 * 2);

            Transformation = Matrix.RotationY(radtc);
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            //switch (level)
            //{
            //    case 0:
                    opBuf[0].Geomentry = data0.GeoData;
                    opBuf[0].Material = material;
                    opBuf[0].Transformation = Matrix.Identity;
                    if (opBuf[0].Sender == null)
                        opBuf[0].Sender = this;
                    return opBuf;
            //    case 1:
            //        opBuf[0].Geomentry = data1.GeoData;
            //        opBuf[0].Material = material;
            //        opBuf[0].Transformation = Matrix.Identity;
            //        if (opBuf[0].Sender == null)
            //            opBuf[0].Sender = this;
            //        return opBuf;
            //    default:
            //        opBuf[0].Geomentry = data2.GeoData;
            //        opBuf[0].Material = material;
            //        opBuf[0].Transformation = Matrix.Identity;
            //        if (opBuf[0].Sender == null)
            //            opBuf[0].Sender = this;
            //        return opBuf;
            //}
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
