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
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Code2015.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zonelink.World;
using Zonelink.MathLib;
using Zonelink;

namespace Code2015.EngineEx
{
    struct ForestInfo
    {
        public float Longitude;
        public float Latitude;
        public float Radius;

        public float Amount;
    }

    class TreeBatchModel
    {
        public const float TreeScale = GameScene.OldModelScale * 3.33f;


        struct TreeVertex
        {
            public Vector3 pos;
            public Vector3 n;
            public Vector3 tex1;

            public static int Size
            {
                get { return sizeof(float) * 3 + sizeof(float) * 3 + sizeof(float) * 3; }
            }

            static VertexElement[] elements;

            static TreeVertex()
            {
                elements = new VertexElement[3];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0);
                elements[1] = new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0);
                elements[2] = new VertexElement(elements[1].Offset + sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0);
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }

        }

        /// <summary>
        ///  定义具有位置、法向量和一组二维纹理坐标的顶点
        /// </summary>
        public struct VertexPNT1
        {
            static VertexElement[] elements;

            /// <summary>
            ///  获取或设置顶点的位置
            /// </summary>
            public Vector3 pos;

            /// <summary>
            ///  获取或设置顶点的法向量
            /// </summary>
            public Vector3 n;

            /// <summary>
            ///  获取或设置纹理坐标u
            /// </summary>
            public float u;

            /// <summary>
            ///  获取或设置纹理坐标v
            /// </summary>
            public float v;


            static VertexPNT1()
            {
                elements = new VertexElement[3];

                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0);
                elements[1] = new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0);
                elements[2] = new VertexElement(elements[1].Offset + sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            }

            /// <summary>
            ///  获取一个VertexElement数组，包含该顶点格式的元素
            /// </summary>
            public static VertexElement[] Elements
            {
                get { return elements; }
            }

            public unsafe static int Size
            {
                get { return sizeof(VertexPNT1); }
            }

            public override int GetHashCode()
            {
                return pos.GetHashCode() ^ n.GetHashCode() ^ u.GetHashCode() ^ v.GetHashCode();
            }


            /// <summary>
            ///  获取该顶点的System.String表达形式
            /// </summary>
            /// <returns>该顶点的System.String表达形式</returns>
            public override string ToString()
            {
                return "Pos: " + pos.ToString() + "N: " + n.ToString() + "uv: " + u.ToString() + "," + v.ToString();
            }
        }

        GraphicsDevice renderSys;

        VertexBuffer vtxBuffer;
        VertexDeclaration vtxDecl;
        IndexBuffer[] idxBuffer;
        Material[] materials;
        //RenderOperation[] opBuf;

        VertexBuffer vtxBuffer2;
        IndexBuffer idxBuffer2;
        Material material2;
        //RenderOperation[] opbuf2;


        int resourceSize;

        ForestInfo info;

        public BoundingSphere BoundingVolume;
        public Matrix Transformation;

        public Matrix TreeOrientation;

        public static string GetHashString(ForestInfo info)
        {
            return info.Longitude.ToString() + "_" + info.Latitude.ToString() + "_" + info.Radius.ToString();
        }

        public TreeBatchModel(GraphicsDevice rs, ForestInfo info)
        {
            this.info = info;
            this.renderSys = rs;

            float radlng = MathHelper.ToRadians(info.Longitude);
            float radlat = MathHelper.ToRadians(info.Latitude);

            Transformation = Matrix.Identity;// PlanetEarth.GetOrientation(radlng, radlat);
            //Transformation.TranslationValue = PlanetEarth.GetPosition(radlng, radlat);

            BoundingVolume.Center = PlanetEarth.GetPosition(radlng, radlat);
            BoundingVolume.Radius = PlanetEarth.GetTileArcLength(MathHelper.ToRadians(info.Radius));
        }

        unsafe void load()
        {
            resourceSize = 0;

            float radlng = MathHelper.ToRadians(info.Longitude);
            float radlat = MathHelper.ToRadians(info.Latitude);
            float radr = MathHelper.ToRadians(info.Radius) * 2;

            TreeOrientation = PlanetEarth.GetOrientation(radlng, radlat);
            TreeOrientation.Translation = PlanetEarth.GetPosition(radlng, radlat, PlanetEarth.PlanetRadius);


            int vtxCount = 0;
            int vtxOffset = 0;
            int vtxCount2 = 0;
            int vtxOffset2 = 0;

            int plantCount = (int)(info.Amount * 0.05f);



            FastList<byte> vertices2 = new FastList<byte>(plantCount * 2500);
            FastList<int> indices2 = new FastList<int>();
            int partVtxCount2 = 0;


            FastList<byte> vertices = new FastList<byte>(plantCount * 2500);
            Dictionary<Material, FastList<int>> indices = new Dictionary<Material, FastList<int>>();
            Dictionary<Material, int> partVtxCount = new Dictionary<Material, int>();

            plantCount = 0;

            byte[] vtxBldBuffer = new byte[TreeVertex.Size];

            const float AreaWidth = 0.036f;
            for (float blkLng = radlng - radr; blkLng < radlng + radr; blkLng += AreaWidth)
            {
                for (float blkLat = radlat - radr; blkLat < radlat + radr; blkLat += AreaWidth)
                {
                    float altblk = TerrainData.Instance.QueryHeight(blkLng, blkLat) - 100;

                    if (altblk < 0)
                        continue;

                    float density = PlantDensity.Instance.GetPlantDensity(blkLng, blkLat);

                    int count = (int)(density * 3);

                    for (int i = 0; i < count; i++)
                    {
                        float treeLng = blkLng + AreaWidth * Randomizer.GetRandomSingle();
                        float treeLat = blkLat + AreaWidth * Randomizer.GetRandomSingle();

                        float alt = TerrainData.Instance.QueryHeight(treeLng, treeLat);

                        PlantDensityData d = PlantDensity.Instance.GetDensity(treeLng, treeLat);
                        int idx = Randomizer.Random(d.Density, PlantDensity.TypeCount);
                       
                        TreeModelData[] mdls = TreeModelLibrary.Instance.Get(idx);
                        TreeModelData meshData = mdls[Randomizer.GetRandomInt(mdls.Length)];


                        Matrix treeOrientation = PlanetEarth.GetOrientation(treeLng, treeLat);
                        treeOrientation.Translation = PlanetEarth.GetPosition(treeLng, treeLat,
                            PlanetEarth.PlanetRadius + alt * TerrainMeshManager.PostHeightScale);

                        float instanceData = Randomizer.GetRandomSingle();

                        float theta = instanceData * MathHelper.Pi * 2;
                        float rotCos = (float)Math.Cos(theta);
                        float rotSin = (float)Math.Sin(theta);

                        #region 数据
                        resourceSize += meshData.VertexCount * TreeVertex.Size;
                        vtxCount += meshData.VertexCount;

                        fixed (byte* src = &meshData.VertexData[0])
                        {
                            VertexPNT1* ptr = (VertexPNT1*)src;

                            for (int j = 0; j < meshData.VertexCount; j++)
                            {
                                TreeVertex p;
                                p.pos.X = rotCos * ptr[j].pos.X - rotSin * ptr[j].pos.Z;
                                p.pos.Z = rotSin * ptr[j].pos.X + rotCos * ptr[j].pos.Z;
                                p.pos.Y = ptr[j].pos.Y;

                                p.pos = p.pos * (TreeScale * (instanceData * 0.4f + 0.8f));

                                Vector3 pp;
                                MathEx.Vec3TransformSimple(ref p.pos, ref treeOrientation, out pp);
                                p.pos = pp;

                                p.n.X = rotCos * ptr[j].n.X - rotSin * ptr[j].n.Z;
                                p.n.Z = rotSin * ptr[j].n.Z + rotCos * ptr[j].n.Z;
                                p.n.Y = ptr[j].n.Y;

                                p.tex1 = new Vector3(ptr[j].u, ptr[j].v, instanceData);

                                fixed (byte* dst = &vtxBldBuffer[0])
                                {
                                    Memory.Copy(&p, dst, TreeVertex.Size);
                                }
                                vertices.Add(vtxBldBuffer);
                            }
                        }

                        Material[] mtrls = meshData.Materials;
                        for (int k = 0; k < mtrls.Length; k++)
                        {
                            Material mtrl = mtrls[k];

                            FastList<int> idxData;
                            if (!indices.TryGetValue(mtrl, out idxData))
                            {
                                idxData = new FastList<int>(plantCount * 120);
                                indices.Add(mtrl, idxData);

                                partVtxCount.Add(mtrl, 0);
                            }

                            partVtxCount[mtrl] += meshData.PartVtxCount[k];

                            int[] meshIdx = meshData.Indices[k];

                            for (int j = 0; j < meshIdx.Length; j++)
                            {
                                idxData.Add(meshIdx[j] + vtxOffset);
                            }


                        }
                        vtxOffset += meshData.VertexCount;
                        #endregion

                        TreeModelData meshData2 = TreeModelLibrary.Instance.GetTrunk();
                        #region 树桩
                        resourceSize += meshData2.VertexCount * TreeVertex.Size;
                        vtxCount2 += meshData2.VertexCount;

                        fixed (byte* src = &meshData2.VertexData[0])
                        {
                            VertexPNT1* ptr = (VertexPNT1*)src;

                            for (int j = 0; j < meshData2.VertexCount; j++)
                            {
                                TreeVertex p;
                                p.pos.X = rotCos * ptr[j].pos.X - rotSin * ptr[j].pos.Z;
                                p.pos.Z = rotSin * ptr[j].pos.X + rotCos * ptr[j].pos.Z;
                                p.pos.Y = ptr[j].pos.Y;

                                p.pos = p.pos * (TreeScale * (instanceData * 0.4f + 0.8f));

                                Vector3 pp;
                                MathEx.Vec3TransformSimple(ref p.pos, ref treeOrientation, out pp);
                                p.pos = pp;

                                p.n.X = rotCos * ptr[j].n.X - rotSin * ptr[j].n.Z;
                                p.n.Z = rotSin * ptr[j].n.Z + rotCos * ptr[j].n.Z;
                                p.n.Y = ptr[j].n.Y;

                                p.tex1 = new Vector3(ptr[j].u, ptr[j].v, instanceData);

                                fixed (byte* dst = &vtxBldBuffer[0])
                                {
                                    Memory.Copy(&p, dst, TreeVertex.Size);
                                }
                                vertices2.Add(vtxBldBuffer);
                            }
                        }

                        material2 = meshData2.Materials[0];
                        partVtxCount2 += meshData2.PartVtxCount[0];

                        int[] meshIdx2 = meshData2.Indices[0];

                        for (int j = 0; j < meshIdx2.Length; j++)
                        {
                            indices2.Add(meshIdx2[j] + vtxOffset2);
                        }
                        vtxOffset2 += meshData2.VertexCount;
                        #endregion
                        plantCount++;
                    }
                }
            }



            // ============================================================================


            vtxDecl = new VertexDeclaration(TreeVertex.Elements);
            int vtxSize = TreeVertex.Size;

            vtxBuffer = new VertexBuffer(renderSys, vtxDecl, vtxCount, BufferUsage.WriteOnly);

            vertices.Trim();
            vtxBuffer.SetData<byte>(vertices.Elements);

            vtxBuffer2 = new VertexBuffer(renderSys, vtxDecl, vtxCount2, BufferUsage.WriteOnly);
            vertices2.Trim();
            vtxBuffer2.SetData<byte>(vertices2.Elements);

            int partCount = indices.Count;

            idxBuffer = new IndexBuffer[partCount];
            materials = new Material[partCount];
            //opBuf = new RenderOperation[partCount];

            int index = 0;

            foreach (KeyValuePair<Material, FastList<int>> e in indices)
            {
                FastList<int> list = e.Value;
                list.Trim();

                materials[index] = e.Key;
                idxBuffer[index] = new IndexBuffer(renderSys, IndexElementSize.ThirtyTwoBits, list.Count, BufferUsage.WriteOnly);

                idxBuffer[index].SetData<int>(list.Elements);

                resourceSize += sizeof(int) * list.Count;

                // ==============================================================================================

                opBuf[index].Material = e.Key;
                opBuf[index].Geomentry = new GeomentryData();
                opBuf[index].Geomentry.BaseIndexStart = 0;
                opBuf[index].Geomentry.BaseVertex = 0;
                opBuf[index].Geomentry.IndexBuffer = idxBuffer[index];
                opBuf[index].Geomentry.PrimCount = idxBuffer[index].IndexCount / 3;
                opBuf[index].Geomentry.PrimitiveType = RenderPrimitiveType.TriangleList;
                opBuf[index].Geomentry.VertexBuffer = vtxBuffer;
                opBuf[index].Geomentry.VertexCount = partVtxCount[e.Key];
                opBuf[index].Geomentry.VertexDeclaration = vtxDecl;
                opBuf[index].Geomentry.VertexSize = vtxSize;
                opBuf[index].Sender = this;
                index++;
            }

            indices2.Trim();

            idxBuffer2 = new IndexBuffer(renderSys, IndexElementSize.ThirtyTwoBits, indices2.Count, BufferUsage.WriteOnly);
            idxBuffer2.SetData<int>(indices2.Elements);
            opbuf2 = new RenderOperation[1];
            opbuf2[0].Material = material2;
            opbuf2[0].Geomentry = new GeomentryData();
            opbuf2[0].Geomentry.BaseIndexStart = 0;
            opbuf2[0].Geomentry.BaseVertex = 0;
            opbuf2[0].Geomentry.IndexBuffer = idxBuffer2;
            opbuf2[0].Geomentry.PrimCount = idxBuffer2.IndexCount / 3;
            opbuf2[0].Geomentry.PrimitiveType = RenderPrimitiveType.TriangleList;
            opbuf2[0].Geomentry.VertexBuffer = vtxBuffer2;
            opbuf2[0].Geomentry.VertexCount = partVtxCount2;
            opbuf2[0].Geomentry.VertexDeclaration = vtxDecl;
            opbuf2[0].Geomentry.VertexSize = vtxSize;
            opbuf2[0].Sender = this;
        }

        void unload()
        {
            if (vtxBuffer != null)
            {
                vtxBuffer.Dispose();

                vtxBuffer = null;
            }
            if (idxBuffer != null)
            {
                for (int i = 0; i < idxBuffer.Length; i++)
                {
                    if (idxBuffer[i] != null && !idxBuffer[i].Disposed)
                        idxBuffer[i].Dispose();
                }
                idxBuffer = null;
            }
            if (vtxDecl != null)
            {
                vtxDecl.Dispose();
                vtxDecl = null;
            }
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (State == ResourceState.Loaded)
            {
                for (int i = 0; i < opBuf.Length; i++)
                {
                    opBuf[i].Transformation = Matrix.Identity;
                }
                return opBuf;
            }
            return null;
        }
        //public RenderOperation[] GetRenderOperation2()
        //{
        //    if (State == ResourceState.Loaded)
        //    {
        //        for (int i = 0; i < opbuf2.Length; i++)
        //        {
        //            opbuf2[i].Transformation = Matrix.Identity;
        //        }
        //        return opbuf2;
        //    }
        //    return null;
        //}

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion
    }
}
