using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.EngineEx
{
    public struct PathVertex
    {
        public Vector3 Position;
        public Vector3 N;
        public Vector3 Right;
        public Vector2 Tex1;

        public static int Size 
        {
            get { return Vector3.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes + Vector2.SizeInBytes; }
        }

        public static VertexElement[] Elements
        {
            get;
            private set;
        }

        static PathVertex()
        {
            Elements = new VertexElement[4];
            Elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
            Elements[1] = new VertexElement(Elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Normal);
            Elements[2] = new VertexElement(Elements[1].Size + Elements[1].Offset, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0);
            Elements[3] = new VertexElement(Elements[2].Size + Elements[2].Offset, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1);
           
        }
   
    }

    public unsafe static class PathBuilder
    {
        static Vector3[] positionBuffer = new Vector3[1000];

        static Vector3[] positionBuffer2 = new Vector3[1000];
        static Vector3[] nrmBuffer = new Vector3[1000];
        static Vector3[] rightBuffer = new Vector3[1000];


        const float PathWidth = 30f;

        public static ModelData BuildModel(RenderSystem rs, Map map, Point[] points)
        {
            MeshData surface = new MeshData(rs);
          
            int vertexLen = points.Length;
            int vertexCount = vertexLen * 4;

            // 计算世界坐标
            for (int i = 0; i < vertexLen; i++)
            {
                float lng;
                float lat;
                Map.GetCoord(points[i].X, points[i].Y, out lng, out lat);

                positionBuffer[i] = PlanetEarth.GetPosition(lng, lat, Math.Max(0, PlanetEarth.PlanetRadius + map.GetHeight(lng, lat)) + 5);
            }

            // 插值
            for (int i = 0; i < vertexLen; i++)
            {
                int last = i - 1;
                if (last < 0) last = 0;

                int next1 = i + 1;
                if (next1 >= vertexLen)
                    next1 = vertexLen - 1;

                int next2 = i + 2;
                if (next2 >= vertexLen)
                    next2 = vertexLen - 1;


                positionBuffer2[i * 2] =
                    MathEx.CatmullRom(positionBuffer[last], positionBuffer[i], positionBuffer[next1], positionBuffer[next2], 0);
                positionBuffer2[i * 2 + 1] =
                    MathEx.CatmullRom(positionBuffer[last], positionBuffer[i], positionBuffer[next1], positionBuffer[next2], 0.5f);
            }

            vertexLen *= 2;


            // 计算切线空间向量
            for (int i = 0; i < vertexLen - 1; i++)
            {
                Vector3 dir = positionBuffer2[i + 1] - positionBuffer2[i];
                dir.Normalize();

                Vector3 up = positionBuffer2[i];
                up.Normalize();

                Vector3.Cross(ref dir, ref up, out rightBuffer[i]);

                Vector3.Cross(ref rightBuffer[i], ref dir, out up);

                nrmBuffer[i] = up;
            }

            // 计算网格顶点
            PathVertex[] vertices = new PathVertex[vertexCount];
            float texV = 0;
            for (int i = 0; i < vertexLen; i++)
            {

                vertices[i * 2].N = nrmBuffer[i];
                vertices[i * 2].Right = rightBuffer[i];
                vertices[i * 2].Position = positionBuffer2[i] - rightBuffer[i] * PathWidth * 0.5f;

                vertices[i * 2 + 1].Position = positionBuffer2[i] + rightBuffer[i] * PathWidth * 0.5f;
                vertices[i * 2 + 1].N = nrmBuffer[i];
                vertices[i * 2 + 1].Right = rightBuffer[i];


                vertices[i * 2].Tex1 = new Vector2(0, texV);
                vertices[i * 2].Tex1 = new Vector2(1, texV);

                texV += 0.1f;
            }

            fixed (PathVertex* src = &vertices[0])
            {
                surface.SetData(src, PathVertex.Size * vertexCount);
            }
            surface.VertexCount = vertexCount;
            surface.VertexElements = PathVertex.Elements;
            surface.VertexSize = PathVertex.Size;

            MeshFace[] faces = new MeshFace[vertexCount - 2];
            for (int i = 0; i < faces.Length; i += 2)
            {
                faces[i].MaterialIndex = 0;
                faces[i].IndexA = i;
                faces[i].IndexB = i + 1;
                faces[i].IndexC = i + 2;

                faces[i + 1].MaterialIndex = 0;
                faces[i + 1].IndexA = i + 1;
                faces[i + 1].IndexB = i + 2;
                faces[i + 1].IndexC = i + 3;
            }
            surface.Faces = faces;
            surface.Materials = new Material[1][];
            surface.Materials[0] = new Material[1];

            Material surfMtrl = new Material(rs);
            surfMtrl.ZEnabled = true;
            surfMtrl.ZWriteEnabled = true;
            surfMtrl.IsTransparent = true;
            surfMtrl.PriorityHint = RenderPriority.Third;
            surfMtrl.Power = 4;
            surfMtrl.Ambient = new Color4F(1, 0.4f, 0.4f, 0.4f);
            surfMtrl.Diffuse = new Color4F(1, 1f, 1, 1);
            surfMtrl.Specular = new Color4F(1, 0.8f, 0.8f, 0.8f);
            surfMtrl.CullMode = CullMode.None;

            surfMtrl.SetEffect(EffectManager.Instance.GetModelEffect(StandardEffectFactory.Name));

            surface.MaterialAnimation = new MaterialAnimationInstance[]
            {
                new MaterialAnimationInstance(new MaterialAnimation(1, 1)) 
            };
            surface.Materials[0][0] = surfMtrl;
            
            // 侧栏








            Mesh surfaceMesh = new Mesh(rs, surface);
            ModelData result = new ModelData(rs, new Mesh[] { surfaceMesh });
            return result;
        }
    }
}
