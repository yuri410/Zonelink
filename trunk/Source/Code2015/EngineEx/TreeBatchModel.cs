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

        public TreeModelData[] Plants;
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


        int resourceSize;

        ForestInfo info;

        public BoundingSphere BoundingVolume;
        public Matrix Transformation;

        public Matrix TreeOrientation;

        public TreeBatchModel(RenderSystem rs, ForestInfo info)
            : base(TreeBatchModelManager.Instance,
            info.Longitude.ToString() + "_" + info.Latitude.ToString() + "_" + info.Radius.ToString())
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

            int vtxCount = 0;
            int vtxOffset = 0;

            int plantCount = (int)(info.Amount * 0.05f);
           

            FastList<byte> vertices = new FastList<byte>(plantCount * 2500);
            Dictionary<Material, FastList<int>> indices = new Dictionary<Material, FastList<int>>();
            Dictionary<Material, int> partVtxCount = new Dictionary<Material, int>();



            byte[] vtxBldBuffer = new byte[TreeVertex.Size];

            for (int i = 0; i < plantCount; i++)
            {
                int idx = Randomizer.GetRandomInt(info.Plants.Length);

            Label_retry:
                float rnd1 = Randomizer.GetRandomSingle();
                float rnd2 = Randomizer.GetRandomSingle();

                float r = rnd1 * radr;
                float rr = PlanetEarth.GetTileArcLength(r);
                float rt = rnd2 * MathEx.PIf * 2;

                float rotCos = (float)Math.Cos(rt);
                float rotSin = (float)Math.Sin(rt);

                float alt = TerrainData.Instance.QueryHeight(radlng + r * rotCos, radlat + r * rotSin);

                if (alt < 0)
                    goto Label_retry;

                TreeOrientation = PlanetEarth.GetOrientation(radlng + r * rotCos, radlat + r * rotSin);
                TreeOrientation.TranslationValue = PlanetEarth.GetPosition(radlng + r * rotCos, radlat + r * rotSin,
                    PlanetEarth.PlanetRadius + alt * TerrainMeshManager.PostHeightScale);

                float instanceData = Randomizer.GetRandomSingle();

                float theta = instanceData * MathEx.PIf * 2;
                rotCos = (float)Math.Cos(theta);
                rotSin = (float)Math.Sin(theta);

                TreeModelData meshData = info.Plants[idx];

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

                        p.pos = p.pos * Game.TreeScale;

                        Vector3 pp;
                        Vector3.TransformSimple(ref p.pos, ref TreeOrientation, out pp);
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
            }

            // ============================================================================

            ObjectFactory fac = renderSys.ObjectFactory;
            vtxDecl = fac.CreateVertexDeclaration(TreeVertex.Elements);

            vtxBuffer = fac.CreateVertexBuffer(vtxCount, vtxDecl, BufferUsage.Static);

            vertices.Trim();
            vtxBuffer.SetData<byte>(vertices.Elements);

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

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion
    }
}
