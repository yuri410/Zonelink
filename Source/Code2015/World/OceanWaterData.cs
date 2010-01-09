﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;

namespace Code2015.World
{
    class OceanWaterData
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        GeomentryData geoData;

        VertexDeclaration vtxDecl;


        public OceanWaterData(RenderSystem rs, int size)
        {
            this.Size = size;

            geoData = new GeomentryData(null);

            ObjectFactory fac = rs.ObjectFactory;

            int vertexCount = size * size;
            vertexBuffer = fac.CreateVertexBuffer(vertexCount, vtxDecl, BufferUsage.Static);
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
