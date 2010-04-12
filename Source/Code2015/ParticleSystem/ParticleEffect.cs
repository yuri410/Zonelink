using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;

namespace Code2015.ParticleSystem
{
    public abstract class ParticleEffect : SceneObject
    {
        Vertex[] dataBuffer;
        struct Vertex
        {
            public Vector3 Position;
            public Vector2 AlphaIdx;

            static VertexElement[] elements;

            static Vertex()
            {
                elements = new VertexElement[2]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(Vector3.SizeInBytes, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                };
            }

            public static int Size
            {
                get { return Vector3.SizeInBytes + Vector2.SizeInBytes; }
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }

        }

        VertexBuffer vtxBuffer;
        IndexBuffer idxBuffer;

        VertexDeclaration vtxDecl;
        FastList<Particle> particles;

        Material material;

        ParticleEmitter emitter;
        ParticleModifier modifier;

        GeomentryData geoData;
        RenderOperation[] renderOp;


        public ParticleEffect(RenderSystem rs, int particleCount)
            : base(false)
        {
            ObjectFactory fac = rs.ObjectFactory;

            vtxDecl = fac.CreateVertexDeclaration(Vertex.Elements);

            particles = new FastList<Particle>(particleCount);
            dataBuffer = new Vertex[particleCount * 4];
            vtxBuffer = fac.CreateVertexBuffer(dataBuffer.Length, vtxDecl, BufferUsage.Dynamic);

            idxBuffer = fac.CreateIndexBuffer(IndexBufferType.Bit16, particleCount * 6, BufferUsage.Static);

            ushort[] indices = new ushort[particleCount * 6];
            for (int i = 0; i < particleCount; i++)
            {
                int ii = i * 6;
                int idx = i * 4;
                indices[ii] = (ushort)idx;
                indices[ii + 1] = (ushort)(idx + 1);
                indices[ii + 2] = (ushort)(idx + 2);
                indices[ii + 3] = (ushort)idx;
                indices[ii + 4] = (ushort)(idx + 2);
                indices[ii + 5] = (ushort)(idx + 3);
            }
            idxBuffer.SetData<ushort>(indices);

            geoData = new GeomentryData();
            geoData.VertexBuffer = vtxBuffer;
            geoData.VertexDeclaration = vtxDecl;
            geoData.VertexSize = Vertex.Size;
            geoData.IndexBuffer = idxBuffer;
            material = new Material(rs);

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

        public float ParticleSize
        {
            get;
            protected set;
        }

        #region IRenderable 成员

        public override RenderOperation[] GetRenderOperation()
        {
            if (emitter != null)
            {
                geoData.VertexCount = particles.Count * 4;
                geoData.PrimCount = particles.Count * 2;

                geoData.BaseVertex = 0;
                geoData.BaseIndexStart = 0;
                geoData.PrimitiveType = RenderPrimitiveType.TriangleList;
              
                renderOp[0].Transformation = Matrix.Identity;
                renderOp[0].Sender = this;
                return renderOp;
            }
            return null;
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            if (level < 2)
                return GetRenderOperation();
            return null;
        }

        #endregion

        

        #region IUpdatable 成员

        public override void Update(GameTime time)
        {
            float dt = time.ElapsedGameTimeSeconds;

            if (emitter != null)
            {
                emitter.Update(particles, dt);

                if (modifier != null)
                {
                    modifier.Update(particles, dt);
                }

                for (int i = 0; i < particles.Count; i++)
                {
                    int idx = i * 4;
                    dataBuffer[idx].Position = particles[i].Position;
                    dataBuffer[idx].AlphaIdx = new Vector2(particles[i].Alpha, 0);

                    idx++;
                    dataBuffer[idx].Position = particles[i].Position;
                    dataBuffer[idx].AlphaIdx = new Vector2(particles[i].Alpha, 1);

                    idx++;
                    dataBuffer[idx].Position = particles[i].Position;
                    dataBuffer[idx].AlphaIdx = new Vector2(particles[i].Alpha, 2);

                    idx++;
                    dataBuffer[idx].Position = particles[i].Position;
                    dataBuffer[idx].AlphaIdx = new Vector2(particles[i].Alpha, 3);
                }
                vtxBuffer.SetData(dataBuffer);

            }
        }

        #endregion

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
