using System;
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
        
        public OceanWaterData(RenderSystem rs, int size)
        {
            geoData = new GeomentryData(null);

            ObjectFactory fac = rs.ObjectFactory;


        }

        public GeomentryData GeoData
        {
            get { return geoData; }
        }


    }
}
