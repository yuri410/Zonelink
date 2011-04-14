/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.World;

namespace Code2015.EngineEx
{
    delegate void ObjectSpaceChangedHandler(Matrix matrix, BoundingSphere sphere);

    unsafe class TerrainMesh : Resource, IRenderable
    {
        const float PlanetRadius = PlanetEarth.PlanetRadius;

        struct TerrainVertex
        {
            public Vector3 Position;
            public float u;
            public float v;
            public float Index;


            static VertexElement[] elements;
            static int size = sizeof(TerrainVertex);
            static TerrainVertex()
            {
                elements = new VertexElement[3];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
                elements[1] = new VertexElement(elements[0].Size,
                    VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
                elements[2] = new VertexElement(elements[1].Size + elements[1].Offset,
                    VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1);
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }

            public static int Size
            {
                get { return size; }
            }
        }



        /// <summary>
        ///  地形一条边上的顶点数
        /// </summary>
        const int terrEdgeSize = 17;


        VertexDeclaration vtxDecl;
        VertexBuffer vtxBuffer;

        IndexBuffer indexBuffer;

        RenderSystem renderSystem;
        ObjectFactory factory;


        FastList<RenderOperation> opBuffer;

        Material material;

        public BoundingSphere BoundingSphere;
        public Matrix Transformation = Matrix.Identity;

        /// <summary>
        ///  
        /// </summary>
        GeomentryData defGeometryData;

        float tileCol;
        float tileLat;
        int tileX;
        int tileY;

        /// <summary>
        ///  经度
        /// </summary>
        public float TileCol
        {
            get { return tileCol; }
        }
        /// <summary>
        ///  纬度
        /// </summary>
        public float TileLat
        {
            get { return tileLat; }
        }

        public int TerrainSize
        {
            get { return terrEdgeSize; }
        }


        public event ObjectSpaceChangedHandler ObjectSpaceChanged;
        
        public static string GetHashString(int x, int y)
        {
            return "TM" + x.ToString("D2") + y.ToString("D2");
        }



        public TerrainMesh(RenderSystem rs, int x, int y)
            : base(TerrainMeshManager.Instance, GetHashString(x, y))
        {
            this.opBuffer = new FastList<RenderOperation>();

            this.tileX = x;
            this.tileY = y;

            renderSystem = rs;
            factory = rs.ObjectFactory;

            material = new Material(rs);
            material.CullMode = CullMode.None;


            material.Ambient = new Color4F(1, 0.5f, 0.5f, 0.5f);
            material.Diffuse = new Color4F(1f, 1f, 1f, 1f);
            material.Specular = new Color4F(0, 0, 0, 0);
            material.Power = 1;
            material.PriorityHint = RenderPriority.Second;

            PlanetEarth.TileCoord2CoordNew(x, y, out tileCol, out tileLat);

            // 估算包围球
            {
                float radtc = MathEx.Degree2Radian(tileCol);
                float radtl = MathEx.Degree2Radian(tileLat);
                float rad5 = PlanetEarth.DefaultTileSpan * 0.5f;

                BoundingSphere.Center = PlanetEarth.GetPosition(radtc + rad5, radtl - rad5);
                BoundingSphere.Radius = MathEx.Root2 * PlanetEarth.GetTileHeight(rad5 * 2);

                if (ObjectSpaceChanged != null)
                    ObjectSpaceChanged(Transformation, BoundingSphere);
            }
        }

        #region Resource实现
        public override int GetSize()
        {
            int size = 0;

            size += TerrainVertex.Size * 33 * 33;
            size += sizeof(int) * (2 * 2) * 6;

            return size;
        }

        protected override void load()
        {
            // 读取地形数据
            float[] data = TerrainData.Instance.GetData(tileX, tileY);

            float radtc = MathEx.Degree2Radian(tileCol);
            float radtl = MathEx.Degree2Radian(tileLat);

            float radSpan = MathEx.Degree2Radian(10);

            int vertexCount = terrEdgeSize * terrEdgeSize;
            int terrEdgeLen = terrEdgeSize - 1;

            material.SetEffect(EffectManager.Instance.GetModelEffect(TerrainEffect33Factory.Name));

            #region 顶点数据

            vtxDecl = factory.CreateVertexDeclaration(TerrainVertex.Elements);

            vtxBuffer = factory.CreateVertexBuffer(vertexCount, vtxDecl, BufferUsage.WriteOnly);

            TerrainVertex[] vtxArray = new TerrainVertex[vertexCount];


            float cellAngle = radSpan / (float)terrEdgeLen;
            #region 计算顶点坐标

            // i为纬度方向
            for (int i = 0; i < terrEdgeSize; i++)
            {
                // j为经度方向
                for (int j = 0; j < terrEdgeSize; j++)
                {
                    Vector3 pos = PlanetEarth.GetPosition(radtc + j * cellAngle, radtl - i * cellAngle);

                    int index = i * terrEdgeSize + j;

                    // 计算海拔高度
                    float height = (data[index] - TerrainMeshManager.PostZeroLevel) * TerrainMeshManager.PostHeightScale;

                    //if (height > 0)
                    //{
                    //    height = (height - 0) * TerrainMeshManager.PostHeightScale;
                    //}
                    //else
                    //{
                    //    height *= TerrainMeshManager.PostHeightScale;
                    //    height -= 10;
                    //    //if (height < -30)
                    //    //    height = -30;
                    //}

                    Vector3 normal = pos;
                    normal.Normalize();
                    vtxArray[index].Position = pos + normal * height;


                    vtxArray[index].Index = index;
                    float curCol = radtc + j * cellAngle;
                    float curLat = radSpan + radtl - i * cellAngle;

                    curCol += MathEx.PIf;
                    curLat -= MathEx.Degree2Radian(10);

                    vtxArray[index].u = 0.5f * curCol / MathEx.PIf;
                    vtxArray[index].v = (-curLat + MathEx.PiOver2) / MathEx.PIf;
                }
            }
            #endregion

            #endregion

            #region 索引数据
            SharedIndexData sindexData = TerrainMeshManager.Instance.GetIndexData();
            indexBuffer = sindexData.Index;
            #endregion

            #region 构造GeomentryData
            defGeometryData = new GeomentryData();
            defGeometryData.VertexDeclaration = vtxDecl;

            defGeometryData.VertexSize = TerrainVertex.Size;
            defGeometryData.VertexBuffer = vtxBuffer;
            defGeometryData.IndexBuffer = indexBuffer;
            defGeometryData.PrimCount = indexBuffer.IndexCount / 3;
            defGeometryData.VertexCount = terrEdgeSize * terrEdgeSize;

            defGeometryData.PrimitiveType = RenderPrimitiveType.TriangleList;

            defGeometryData.BaseVertex = 0;

            #endregion

            vtxBuffer.SetData<TerrainVertex>(vtxArray);
        }

        protected override void unload()
        {
            if (!object.ReferenceEquals(vtxBuffer, null))
            {
                vtxBuffer.Dispose();
                vtxBuffer = null;
            }
            if (!object.ReferenceEquals(vtxDecl, null))
            {
                vtxDecl.Dispose();
                vtxDecl = null;
            }
            indexBuffer = null;

        }
        #endregion

        /// <summary>
        ///  准备特定lod级别下的可见物体
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="level"></param>
        public void PrepareVisibleObjects(ICamera cam, int level)
        {
            if (State == ResourceState.Loaded)
            {
                opBuffer.Clear();

                RenderOperation op;

                op.Material = material;
                op.Geomentry = defGeometryData;
                op.BoneTransforms = null;
                op.Transformation = Matrix.Identity;
                op.Sender = this;
                opBuffer.Add(op);
            }
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (State == ResourceState.Loaded)
            {
                return opBuffer.Elements;
            }
            return null;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion
    }
}
