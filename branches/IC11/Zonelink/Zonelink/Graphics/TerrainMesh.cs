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
using Code2015.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zonelink.World;
using Zonelink.MathLib;
using Zonelink;

namespace Code2015.EngineEx
{
    delegate void ObjectSpaceChangedHandler(Matrix matrix, BoundingSphere sphere);

    unsafe class TerrainMesh
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
                
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0);
                elements[1] = new VertexElement(sizeof(float) * 3,
                    VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
                elements[2] = new VertexElement(sizeof(float) * 2 + elements[1].Offset,
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
        const int terrEdgeSize = 33;

        Game1 game;

        VertexDeclaration vtxDecl;
        VertexBuffer vtxBuffer;

        IndexBuffer indexBuffer;



        public BoundingSphere BoundingSphere;
        public Matrix Transformation = Matrix.Identity;



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


        public static string GetHashString(int x, int y)
        {
            return "TM" + x.ToString("D2") + y.ToString("D2");
        }



        public TerrainMesh(Game1 rs, int x, int y)
        {
            this.game = rs;

            this.tileX = x;
            this.tileY = y;

            //renderSystem = rs;
            //factory = rs.ObjectFactory;

            //material = new Material(rs);
            //material.CullMode = CullMode.None;


            //material.Ambient = new Color4F(1, 0.5f, 0.5f, 0.5f);
            //material.Diffuse = new Color4F(1f, 1f, 1f, 1f);
            //material.Specular = new Color4F(0, 0, 0, 0);
            //material.Power = 1;
            //material.PriorityHint = RenderPriority.Second;

            PlanetEarth.TileCoord2CoordNew(x, y, out tileCol, out tileLat);

            // 估算包围球
            {
                float radtc = MathHelper.ToRadians(tileCol);
                float radtl = MathHelper.ToRadians(tileLat);
                float rad5 = PlanetEarth.DefaultTileSpan * 0.5f;

                BoundingSphere.Center = PlanetEarth.GetPosition(radtc + rad5, radtl - rad5);
                BoundingSphere.Radius = MathEx.Root2 * PlanetEarth.GetTileHeight(rad5 * 2);
            }

            load();
        }

        #region Resource实现
        


        void load()
        {
            // 读取地形数据
            float[] data = TerrainData.Instance.GetData(tileX, tileY);

            float radtc = MathHelper.ToRadians(tileCol);
            float radtl = MathHelper.ToRadians(tileLat);

            float radSpan = MathHelper.ToRadians(10);

            int vertexCount = terrEdgeSize * terrEdgeSize;
            int terrEdgeLen = terrEdgeSize - 1;



            #region 顶点数据

            vtxDecl = new VertexDeclaration(TerrainVertex.Elements);

            vtxBuffer = new VertexBuffer(game.GraphicsDevice, vtxDecl, vertexCount, BufferUsage.WriteOnly);
            
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

                    curCol += MathHelper.Pi;
                    curLat -= MathHelper.ToRadians(10);

                    vtxArray[index].u = 0.5f * curCol / MathHelper.Pi;
                    vtxArray[index].v = (-curLat + MathHelper.PiOver2) / MathHelper.Pi;
                }
            }
            #endregion

            #endregion

            #region 索引数据
            SharedIndexData sindexData = TerrainMeshManager.Instance.GetIndexData();
            indexBuffer = sindexData.Index;
            #endregion

            //#region 构造GeomentryData
            //defGeometryData = new GeomentryData();
            //defGeometryData.VertexDeclaration = vtxDecl;

            //defGeometryData.VertexSize = TerrainVertex.Size;
            //defGeometryData.VertexBuffer = vtxBuffer;
            //defGeometryData.IndexBuffer = indexBuffer;
            //defGeometryData.PrimCount = indexBuffer.IndexCount / 3;
            //defGeometryData.VertexCount = terrEdgeSize * terrEdgeSize;

            //defGeometryData.PrimitiveType = RenderPrimitiveType.TriangleList;

            //defGeometryData.BaseVertex = 0;

            //#endregion

            vtxBuffer.SetData<TerrainVertex>(vtxArray);
        }

        void unload()
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


        public void Render() 
        {
            game.GraphicsDevice.SetVertexBuffer(vtxBuffer);
            game.GraphicsDevice.Indices = indexBuffer;
            game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, terrEdgeSize * terrEdgeSize, 0, indexBuffer.IndexCount / 3);
        }

        public void Dispose()
        {
            unload();
        }

    }
}
