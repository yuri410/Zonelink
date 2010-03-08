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


    struct TreeModelData
    {
        public PlantType Type;
        public PlantCategory Category;

        public Material[] Materials;
        public int[][] Indices;
        public int[] PartVtxCount;

        public byte[] VertexData;

        public int VertexCount;

        public int VertexSize
        {
            get { return VertexPNT1.Size; }
        }
    }

    struct ForestInfo 
    {
        public float Longitude;
        public float Latitude;
        public float Radius;

        public float Amount;

        public PlantType Type;
        public PlantCategory Category;

        public TreeModelData[] BigPlants;
        public TreeModelData[] SmallPlants;
    }

    class TreeBatchModel : Resource, IRenderable
    {
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
            float radr = MathEx.Degree2Radian(info.Radius);

            int vtxCount = 0;
            int vtxOffset = 0;
            int vtxSizeTotal = 0;

            int plantCount = (int)(info.Amount * 0.4f);
            int treeCount = 0;
            int smvCount = 0;
            switch (info.Category)
            {
                case PlantCategory.Bush:
                case PlantCategory.Forest:
                    treeCount = (int)(plantCount * 0.6f);
                    smvCount = (int)(plantCount * 0.4f);
                    break;
            }

            FastList<byte> vertices = new FastList<byte>(plantCount * 2500);
            Dictionary<Material, FastList<int>> indices = new Dictionary<Material, FastList<int>>();
            Dictionary<Material, int> partVtxCount = new Dictionary<Material, int>();

           
            for (int i = 0; i < plantCount; i++)
            {
              
                int idx = Randomizer.GetRandomInt(info.BigPlants.Length);

                float rx = 2 * (Randomizer.GetRandomSingle() - 0.5f) * radr;
                float ry = 2 * (Randomizer.GetRandomSingle() - 0.5f) * radr;

                Matrix trans =// Matrix.Scaling(CityObjectTRAdjust.Scaler, CityObjectTRAdjust.Scaler, CityObjectTRAdjust.Scaler) *
                   PlanetEarth.GetOrientation(radlng + rx, radlat + ry);
                trans.TranslationValue = PlanetEarth.GetPosition(radlng + rx, radlng + ry,PlanetEarth.PlanetRadius + 100);


                TreeModelData meshData = info.BigPlants[idx];

                int vtxDataSize = meshData.VertexCount * meshData.VertexSize;
                vtxCount += meshData.VertexCount;
                vtxSizeTotal += vtxDataSize;

                fixed (byte* src = &meshData.VertexData[0])
                {
                    VertexPNT1* ptr = (VertexPNT1*)src;

                    for (int j = 0; j < meshData.VertexCount; j++)
                    {
                        Vector3 p = ptr[j].pos;
                        Vector3.TransformSimple(ref p, ref trans, out ptr[j].pos);
                    }
                }
                vertices.Add(meshData.VertexData);

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
            resourceSize += vtxSizeTotal;

            //for (int i = 0; i < smvCount; i++)
            //{
            //    int idx = Randomizer.GetRandomInt(info.SmallPlants.Length);
            //}


            // ============================================================================

            ObjectFactory fac = renderSys.ObjectFactory;
            vtxDecl = fac.CreateVertexDeclaration(VertexPNT1.Elements);

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
