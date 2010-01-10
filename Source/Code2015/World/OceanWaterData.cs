﻿using System;
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
        struct HashNode
        {
            public int Size;
            public float Latitude;

            public HashNode(int size, float lat)
            {
                Size = size;
                Latitude = lat;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        Dictionary<HashNode, OceanWaterData> dataTable;

        RenderSystem renderSystem;

        public OceanWaterDataManager(RenderSystem rs)
        {
            renderSystem = rs;
            dataTable = new Dictionary<HashNode, OceanWaterData>();
        }


        public OceanWaterData GetData(int size, float lat)
        {
            HashNode node = new HashNode(size, lat);
            OceanWaterData result;
            if (!dataTable.TryGetValue(node, out result))
            {
                result = new OceanWaterData(renderSystem, size, lat);
                dataTable.Add(node, result);
            }
            return result;
        }
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

            float rad10 = PlanetEarth.DefaultTileSpan;
            float radtl = MathEx.Degree2Radian(lat);

            #region 顶点数据
            WaterVertex[] vtxArray = new WaterVertex[vertexCount];

            float cellAngle = rad10 / (float)len;

            // i为经度方向
            for (int i = 0; i < size; i++)
            {
                // j为纬度方向
                for (int j = 0; j < size; j++)
                {
                    Vector3 pos = PlanetEarth.GetPosition(j * cellAngle, radtl - i * cellAngle);

                    int index = i * size + j;

                    vtxArray[index].Position = pos;
                    vtxArray[index].Index = index;
                }
            }
            vertexBuffer.SetData<WaterVertex>(vtxArray);
            #endregion

            #region 索引数据
            int indexCount = MathEx.Sqr(len) * 2 * 3;

            int[] indexArray = new int[indexCount];
            indexBuffer = fac.CreateIndexBuffer(IndexBufferType.Bit32, indexCount, BufferUsage.WriteOnly);

            for (int i = 0, index = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    int x = i;
                    int y = j;

                    indexArray[index++] = y * size + x;
                    indexArray[index++] = y * size + (x + 1);
                    indexArray[index++] = (y + 1) * size + (x + 1);

                    indexArray[index++] = y * size + x;
                    indexArray[index++] = (y + 1) * size + (x + 1);
                    indexArray[index++] = (y + 1) * size + x;
                }
            }
            indexBuffer.SetData<int>(indexArray);
            #endregion

            #region 构造GeomentryData
            geoData = new GeomentryData(null);
            geoData.VertexDeclaration = vtxDecl;

            geoData.VertexSize = WaterVertex.Size;
            geoData.VertexBuffer = vertexBuffer;
            geoData.IndexBuffer = indexBuffer;
            geoData.PrimCount = MathEx.Sqr(len) * 2;
            geoData.VertexCount = MathEx.Sqr(size);

            geoData.PrimitiveType = RenderPrimitiveType.TriangleList;

            geoData.BaseVertex = 0;

            #endregion
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
