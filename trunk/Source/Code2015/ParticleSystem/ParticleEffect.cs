using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;
using Apoc3D.Graphics;

namespace Code2015.ParticleSystem
{
    class ParticleEffect : IRenderable
    {

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

        class Batch : IRenderable
        {
            const int MaxPerBatchCount = 120;

            VertexBuffer vertexBuffer;
            Vector4[] transform;
            ColorValue[] colors;
            VertexDeclaration vertexDecl;

            public Batch(RenderSystem rs, VertexDeclaration vtxDecl)
            {
                this.vertexDecl = vtxDecl;


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

        VertexDeclaration vtxDecl;
 
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
