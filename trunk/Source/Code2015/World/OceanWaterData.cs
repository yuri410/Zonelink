using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.EngineEx;

namespace Code2015.World
{
    class OceanWaterDataManager
    {

    }

    class OceanWaterData
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        GeomentryData geoData;

        VertexDeclaration vtxDecl;


        public OceanWaterData(RenderSystem rs, int size, float lat)
        {
            this.Size = size;

            geoData = new GeomentryData(null);

            ObjectFactory fac = rs.ObjectFactory;


            vtxDecl = fac.CreateVertexDeclaration(WaterVertex.Elements);

            int len = size - 1;
            int vertexCount = size * size;

            vertexBuffer = fac.CreateVertexBuffer(vertexCount, vtxDecl, BufferUsage.Static);

            float rad10 = MathEx.Degree2Radian(10);
            float radtl = MathEx.Degree2Radian(lat);
            float topLen = PlanetEarth.GetTileWidth(radtl + rad10, rad10);
            float bottomLen = PlanetEarth.GetTileWidth(radtl, rad10);
            float heightLen = PlanetEarth.GetTileHeight(rad10);

            Matrix scalingY = Matrix.Scaling(1, 1, heightLen / (float)len);

            WaterVertex[] vtxArray = new WaterVertex[vertexCount];

            // i为经度方向
            for (int i = 0; i < size; i++)
            {
                float lerp = i / (float)(size);
                float colCellScale = MathEx.LinearInterpose(topLen, bottomLen, lerp) / (float)len;
                float colOfs = (1 - colCellScale) * len * 0.5f;

                Matrix scalingX = Matrix.Scaling(colCellScale, 1, 1);
                scalingX *= Matrix.Translation(colOfs, 0, 0);

                // j为纬度方向
                for (int j = 0; j < size; j++)
                {
                    Vector3 pos = new Vector3(
                                 j * TerrainMeshManager.TerrainScale,// * colCellScale + colOfs,
                                 0,
                                 i * TerrainMeshManager.TerrainScale);
                }
            }
        }

        public int Size
        {
            get;
            private set;
        }
        public GeomentryData GeoData
        {
            get { return geoData; }
        }


    }
}
