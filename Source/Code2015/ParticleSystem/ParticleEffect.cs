using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;

namespace Code2015.ParticleSystem
{
    class ParticleEffect : IRenderable, IUpdatable
    {
        Vertex[] dataBuffer;
        struct Vertex
        {
            public Vector3 Position;
            public float Alpha;

            static VertexElement[] elements;

            static Vertex()
            {
                elements = new VertexElement[2]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(1, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0)
                };
            }

            public static int Size
            {
                get { return Vector3.SizeInBytes; }
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }

        }

        VertexBuffer vtxBuffer;

        VertexDeclaration vtxDecl;
        Particle[] particles;

        Material material;

        ParticleEmitter emitter;
        ParticleModifier modifier;

        GeomentryData geoData;
        RenderOperation[] renderOp;


        public ParticleEffect(RenderSystem rs, int particleCount)
        {
            ObjectFactory fac = rs.ObjectFactory;

            vtxDecl = fac.CreateVertexDeclaration(Vertex.Elements);

            particles = new Particle[particleCount];
            dataBuffer = new Vertex[particleCount * 4];
            vtxBuffer = fac.CreateVertexBuffer(particleCount, vtxDecl, BufferUsage.Dynamic);

            material = new Material(rs);
            material.CullMode = CullMode.None;
            material.ZEnabled = true;
            material.ZWriteEnabled = false;
            material.PriorityHint = RenderPriority.Third;
            material.IsTransparent = true;
            material.Ambient = new Color4F(1, 0.4f, 0.4f, 0.4f);
            material.Diffuse = new Color4F(1, 1f, 1, 1);
            

            geoData = new GeomentryData();
            geoData.VertexBuffer = vtxBuffer;
            renderOp = new RenderOperation[1];

            renderOp[0].Geomentry = geoData;
            renderOp[0].Material = material;
            
        }

        public ParticleEmitter Emitter 
        {
            get { return emitter; }
            set { emitter = value; }
        }
        public ParticleModifier Modifier 
        {
            get { return modifier; }
            set { modifier = value; }
        }

        public Material Material
        {
            get { return material; }
        }



        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (emitter != null)
            {
                geoData.VertexCount = emitter.CurrentCount * 4;
                geoData.BaseVertex = 0;
                geoData.BaseIndexStart = 0;
                geoData.PrimitiveType = RenderPrimitiveType.TriangleList;
                geoData.VertexSize = Vertex.Size;

                renderOp[0].Transformation = Matrix.Identity;

                return renderOp;
            }
            return null;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            if (level < 2)
                return GetRenderOperation();
            return null;
        }

        #endregion

        

        #region IUpdatable 成员

        public void Update(GameTime time)
        {
            float dt = time.ElapsedGameTimeSeconds;

            if (emitter != null)
            {
                emitter.Update(particles, dt);

                if (modifier != null)
                {
                    modifier.Update(particles, emitter.CurrentCount, dt);
                }

                for (int i = 0; i < emitter.CurrentCount; i++)
                {
                    int idx = i * 4;
                    dataBuffer[idx].Position = particles[i].Position;
                    dataBuffer[idx].Alpha = particles[i].Alpha;

                    idx++;
                    dataBuffer[idx].Position = particles[i].Position;
                    dataBuffer[idx].Alpha = particles[i].Alpha;

                    idx++;
                    dataBuffer[idx].Position = particles[i].Position;
                    dataBuffer[idx].Alpha = particles[i].Alpha;

                    idx++;
                    dataBuffer[idx].Position = particles[i].Position;
                    dataBuffer[idx].Alpha = particles[i].Alpha;
                }
                vtxBuffer.SetData(dataBuffer);

            }
        }

        #endregion
    }
}
