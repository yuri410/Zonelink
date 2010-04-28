using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.World;

namespace Code2015.EngineEx
{
    struct ForestInfo
    {
        public float Longitude;
        public float Latitude;
        public float Radius;

        public float Amount;
    }

    class TreeBatchModel : Resource, IRenderable
    {
        struct TreeVertex
        {
            public Vector3 pos;
            public Vector3 n;
            public Vector3 tex1;

            public static int Size
            {
                get { return Vector3.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes; }
            }

            static VertexElement[] elements;

            static TreeVertex()
            {
                elements = new VertexElement[3];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
                elements[1] = new VertexElement(elements[0].Size, VertexElementFormat.Vector3, VertexElementUsage.Normal);
                elements[2] = new VertexElement(elements[1].Offset + elements[1].Size, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0);
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }

        }

        RenderSystem renderSys;

        VertexBuffer vtxBuffer;
        VertexDeclaration vtxDecl;
        IndexBuffer[] idxBuffer;
        Material[] materials;
        RenderOperation[] opBuf;

        VertexBuffer vtxBuffer2;
        IndexBuffer idxBuffer2;
        Material material2;
        RenderOperation[] opbuf2;


        int resourceSize;

        ForestInfo info;

        public BoundingSphere BoundingVolume;
        public Matrix Transformation;

        public Matrix TreeOrientation;

        public static string GetHashString(ForestInfo info)
        {
            return info.Longitude.ToString() + "_" + info.Latitude.ToString() + "_" + info.Radius.ToString();
        }

        public TreeBatchModel(RenderSystem rs, ForestInfo info)
            : base(TreeBatchModelManager.Instance,
            GetHashString(info))
        {
            this.info = info;
            this.renderSys = rs;

            float radlng = MathEx.Degree2Radian(info.Longitude);
            float radlat = MathEx.Degree2Radian(info.Latitude);

            Transformation = Matrix.Identity;// PlanetEarth.GetOrientation(radlng, radlat);
            //Transformation.TranslationValue = PlanetEarth.GetPosition(radlng, radlat);

            BoundingVolume.Center = PlanetEarth.GetPosition(radlng, radlat);
            BoundingVolume.Radius = PlanetEarth.GetTileArcLength(MathEx.Degree2Radian(info.Radius));
        }

        public override int GetSize()
        {
            return resourceSize;
        }

        protected unsafe override void load()
        {
            resourceSize = 0;

            float radlng = MathEx.Degree2Radian(info.Longitude);
            float radlat = MathEx.Degree2Radian(info.Latitude);
            float radr = MathEx.Degree2Radian(info.Radius) * 2;

            TreeOrientation = PlanetEarth.GetOrientation(radlng, radlat);
            TreeOrientation.TranslationValue = PlanetEarth.GetPosition(radlng, radlat, PlanetEarth.PlanetRadius);


            int vtxCount = 0;
            int vtxOffset = 0;
            int vtxCount2 = 0;
            int vtxOffset2 = 0;

            int plantCount = (int)(info.Amount * 0.05f);



            FastList<byte> vertices2 = new FastList<byte>(plantCount * 2500);
            FastList<int> indices2 = new FastList<int>();
            int partVtxCount2 = 0;
            Material trunkMat = null;

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
                        treeOrientation.TranslationValue = PlanetEarth.GetPosition(treeLng, treeLat,
                            PlanetEarth.PlanetRadius + alt * TerrainMeshManager.PostHeightScale);

                        float instanceData = Randomizer.GetRandomSingle();

                        float theta = instanceData * MathEx.PIf * 2;
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

                                p.pos = p.pos * (Game.TreeScale * (instanceData * 0.4f + 0.8f));

                                Vector3 pp;
                                Vector3.TransformSimple(ref p.pos, ref treeOrientation, out pp);
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

                                p.pos = p.pos * (Game.TreeScale * (instanceData * 0.4f + 0.8f));

                                Vector3 pp;
                                Vector3.TransformSimple(ref p.pos, ref treeOrientation, out pp);
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

            ObjectFactory fac = renderSys.ObjectFactory;
            vtxDecl = fac.CreateVertexDeclaration(TreeVertex.Elements);

            vtxBuffer = fac.CreateVertexBuffer(vtxCount, vtxDecl, BufferUsage.Static);

            vertices.Trim();
            vtxBuffer.SetData<byte>(vertices.Elements);

            vtxBuffer2 = fac.CreateVertexBuffer(vtxCount2, vtxDecl, BufferUsage.Static);
            vertices2.Trim();
            vtxBuffer2.SetData<byte>(vertices2.Elements);

            int partCount = indices.Count;

            idxBuffer = new IndexBuffer[partCount];
            materials = new Material[partCount];
            opBuf = new RenderOperation[partCount];

            int index = 0;
            int vtxSize = vtxDecl.GetVertexSize();

            foreach (KeyValuePair<Material, FastList<int>> e in indices)
            {
                FastList<int> list = e.Value;
                list.Trim();

                materials[index] = e.Key;
                idxBuffer[index] = fac.CreateIndexBuffer(IndexBufferType.Bit32, list.Count, BufferUsage.Static);

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

            idxBuffer2 = fac.CreateIndexBuffer(IndexBufferType.Bit32, indices2.Count, BufferUsage.Static);
            idxBuffer2.SetData<int>(indices2.Elements);
            opbuf2 = new RenderOperation[1];
            opBuf[index].Material = trunkMat;
            opbuf2[index].Geomentry = new GeomentryData();
            opbuf2[index].Geomentry.BaseIndexStart = 0;
            opbuf2[index].Geomentry.BaseVertex = 0;
            opbuf2[index].Geomentry.IndexBuffer = idxBuffer2;
            opbuf2[index].Geomentry.PrimCount = idxBuffer2.IndexCount / 3;
            opbuf2[index].Geomentry.PrimitiveType = RenderPrimitiveType.TriangleList;
            opbuf2[index].Geomentry.VertexBuffer = vtxBuffer2;
            opbuf2[index].Geomentry.VertexCount = partVtxCount2;
            opbuf2[index].Geomentry.VertexDeclaration = vtxDecl;
            opbuf2[index].Geomentry.VertexSize = vtxSize;
            opbuf2[index].Sender = this;
        }

        protected override void unload()
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
        public RenderOperation[] GetRenderOperation2()
        {
            if (State == ResourceState.Loaded)
            {
                for (int i = 0; i < opbuf2.Length; i++)
                {
                    opbuf2[i].Transformation = Matrix.Identity;
                }
                return opbuf2;
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
