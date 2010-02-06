using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    class ParticleEffect : IRenderable
    {
        const int MaxPerBatchCount = 120;

        struct Vertex
        {
            public Vector2 Position;

            static VertexElement[] elements;

            static Vertex()
            {
                elements = new VertexElement[1] { new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0) };
            }

            public static int Size
            {
                get { return Vector2.SizeInBytes; }
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }

        }

        VertexBuffer vertexBuffer;
        VertexDeclaration vtxDecl;
        Vector4 transform;
 
        public ParticleEffect(RenderSystem rs)
        {
            ObjectFactory fac = rs.ObjectFactory;

            vtxDecl = fac.CreateVertexDeclaration(Vertex.Elements);


        }

       
        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
