using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;
using Apoc3D.Graphics;
using Apoc3D;
using Apoc3D.Graphics.Effects;
using Code2015.World;

namespace Code2015.EngineEx
{
    unsafe class Tail
    {
        struct Vertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;
            //public Vector3 Direction;

            static VertexElement[] elements;
            static Vertex()
            {
                elements = new VertexElement[2];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
                elements[1] = new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
                //elements[2] = new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1);
            }

            public static VertexElement[] Elements { get { return elements; } }
        }

        RenderSystem m_device;
        VertexBuffer m_vtxBuffer;
        IndexBuffer m_idxBuffer;
        Vertex[] m_localVertex;
        VertexDeclaration m_vtxDecl;

        Material tailMaterial;

        GeomentryData geoData;
        RenderOperation[] opBuffer = new RenderOperation[1];

        int m_maxLength;
        int m_currentLength;

        float m_generateInterval;
        float m_generateCoolDown;


        public static int getVertexStride() { return sizeof(Vertex); }

        public void Reset()
        {
            m_currentLength = 0;
        }

        public Tail(RenderSystem device, int maxLength,  float sementPerSec,  Material mtrl)
        {
            m_device = device;
            m_maxLength = maxLength;

            m_generateInterval = 1.0f / sementPerSec;


            m_generateCoolDown = m_generateInterval;
            m_localVertex = new Vertex[maxLength * 2];


            tailMaterial = mtrl;

            ObjectFactory fac= device.ObjectFactory;
            m_vtxDecl = fac.CreateVertexDeclaration(Vertex.Elements);

            m_vtxBuffer = fac.CreateVertexBuffer(maxLength * 2, m_vtxDecl, BufferUsage.Dynamic | BufferUsage.WriteOnly);
            m_idxBuffer = fac.CreateIndexBuffer(IndexBufferType.Bit16, maxLength * 2, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[maxLength * 2];
            for (int i = 0; i < maxLength * 2; i++) 
            {
                indices[i] = (ushort)i;
            }
            m_idxBuffer.SetData<ushort>(indices);

            geoData = new GeomentryData();
            opBuffer[0].Geomentry = geoData;
            opBuffer[0].Material = tailMaterial;            
            opBuffer[0].Transformation = Matrix.Identity;
            opBuffer[0].Sender = this;

            geoData.VertexSize = m_vtxDecl.GetVertexSize();            
            geoData.VertexCount = m_localVertex.Length;
            geoData.VertexBuffer = m_vtxBuffer;
            geoData.IndexBuffer = m_idxBuffer;
            geoData.VertexDeclaration = m_vtxDecl;
            geoData.PrimitiveType = RenderPrimitiveType.LineStrip;
        }

        public RenderOperation[] GetRenderOperation()
        {
            if (m_currentLength >= 2)
            {
                geoData.VertexCount = m_currentLength * 2;
                geoData.PrimCount = m_currentLength * 2 - 1;// m_currentLength * 2 - 2;
                return opBuffer;
            }
            return null;
        }
        public void Update(GameTime time, Vector3 newPos, Vector3 dir)
        {
            //m_currentTime += time.ElapsedGameTimeSeconds;
            m_generateCoolDown -= time.ElapsedGameTimeSeconds;


            if (m_generateCoolDown < 0)
            {
                BuildNewSegment(newPos, dir);

                m_generateCoolDown = m_generateInterval;
            }		
        }
        public void BuildNewSegment(Vector3 newPos, Vector3 dir) 
        {
            Vertex l;
            Vertex r;

            Vector3 tangent = Vector3.Cross(EffectParams.InvView.Forward, dir);

            l.Position = newPos + new Vector3(500, 0, 0);// +tangent * 500;
            r.Position = newPos - new Vector3(500, 0, 0);
            //l.Direction = dir;
            //r.Direction = dir;

            l.TexCoord = new Vector2(0, 1);
            r.TexCoord = new Vector2(1, 1);


            if (m_currentLength == m_maxLength)
            {
                for (int i = 2 * m_currentLength - 1; i >= 2; i--)
                {
                    m_localVertex[i] = m_localVertex[i - 2];
                }

                m_localVertex[0] = l;
                m_localVertex[1] = r;
            }
            else
            {
                m_currentLength++;
                if (m_currentLength > m_maxLength)
                    m_currentLength = m_maxLength;

                if (m_currentLength > 1)
                {
                    int numCurrentVtx = m_currentLength * 2;
                    for (int i = numCurrentVtx - 1; i >= 2; i--)
                    {
                        m_localVertex[i] = m_localVertex[i - 2];
                    }

                    for (int i = numCurrentVtx; i < (m_maxLength * 2); i++)
                    {
                        m_localVertex[i] = m_localVertex[numCurrentVtx - 1];
                    }
                }

                m_localVertex[0] = l;
                m_localVertex[1] = r;


            }
            float tatalLen = m_currentLength * 2;
            for (int i = 0; i < m_currentLength * 2; i += 2)
            {
                m_localVertex[i].TexCoord.Y = i / tatalLen;
                m_localVertex[i + 1].TexCoord.Y = i / tatalLen;
            }

            m_vtxBuffer.SetData<Vertex>(m_localVertex);            
        }

        public void Dispose()
        {
            m_vtxBuffer.Dispose();
            m_vtxDecl.Dispose();
        }
    }
}
