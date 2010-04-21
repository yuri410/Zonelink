using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;

namespace Code2015.EngineEx
{
    unsafe struct PlantDensityData
    {
        public fixed float Density[PlantDensity.TypeCount];
    }
    unsafe class PlantDensity
    {
        static readonly string[] TableNames =
        {
            "plant0.raw", "plant1.raw", "plant2.raw", 
            "plant3.raw", "plant4.raw", "plant5.raw", 
            "plant6.raw", "plant7.raw", "plant8.raw"
        };

        static PlantDensity singleton;

        public static PlantDensity Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new PlantDensity();
                return singleton;
            }
        }

        const int Width = 1188;
        const int Height = Width / 2;

        public const int TypeCount = 8;

        byte[][] densityTable;

        private PlantDensity()
        {
            densityTable = new byte[TypeCount][];
            for (int i = 0; i < TypeCount; i++)
            {
                FileLocation fl = FileSystem.Instance.Locate(TableNames[i], GameFileLocs.Nature);

                ContentBinaryReader br = new ContentBinaryReader(fl);
                densityTable[i] = br.ReadBytes(Width * Height);

                br.Close();
            }
        }

        public PlantDensityData GetDensity(float longtiude, float latitude)
        {
            double yspan = Math.PI;

            int y = (int)(((yspan * 0.5 - latitude) / yspan) * Height);
            int x = (int)(((longtiude + Math.PI) / (2 * Math.PI)) * Width);

            if (y < 0) y += Height;
            if (y >= Height) y -= Height;

            if (x < 0) x += Width;
            if (x >= Width) x -= Width;

            int idx = y * Width + x;
            PlantDensityData result = new PlantDensityData();
            for (int i = 0; i < TypeCount; i++)
            {
                result.Density[i] = densityTable[i][idx] / 255f;
            }
            return result;
        }
    }

    struct TreeModelData
    {
        public Material[] Materials;
        public int[][] Indices;
        public int[] PartVtxCount;

        public byte[] VertexData;

        public int VertexCount;
    }

    class TreeModelLibrary : Singleton
    {
        static TreeModelLibrary singleton;
        
        public static TreeModelLibrary Instance
        {
            get { return singleton; }
        }

        public static void Initialize(RenderSystem rs)
        {
            singleton = new TreeModelLibrary(rs);
        }

        RenderSystem renderSys;
        //Dictionary<PlantCategory, FastList<TreeModelData>> categoryModels 
        //    = new Dictionary<PlantCategory, FastList<TreeModelData>>();

        //Dictionary<PlantType, FastList<TreeModelData>> typeModels 
        //    = new Dictionary<PlantType, FastList<TreeModelData>>();

        //Dictionary<PlantCategory, Dictionary<PlantType, FastList<TreeModelData>>> table 
        //    = new Dictionary<PlantCategory, Dictionary<PlantType, FastList<TreeModelData>>>();
        FastList<TreeModelData> tempList = new FastList<TreeModelData>();

        FastList<ModelMemoryData> loadedModels
            = new FastList<ModelMemoryData>();

        private unsafe TreeModelLibrary(RenderSystem rs)
        {
            renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("trees.xml", GameFileLocs.Config);
            Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);

            foreach (KeyValuePair<string, ConfigurationSection> s in conf)
            {
                ConfigurationSection sect = s.Value;
                TreeModelData mdl;

                string fileName = sect.GetString("Level0", string.Empty);
                FileLocation fl2 = FileSystem.Instance.Locate(fileName, GameFileLocs.Model);

                ModelMemoryData mdlData = new ModelMemoryData(rs, fl2);

                loadedModels.Add(mdlData);

                MeshData[] dataArr = mdlData.Entities;

                if (dataArr.Length == 1)
                {
                    MeshData data = dataArr[0];

                    Material[][] mtrls = data.Materials;

                    int partCount = mtrls.Length;
                    FastList<int>[] indices = new FastList<int>[partCount];
                    for (int i = 0; i < partCount; i++)
                        indices[i] = new FastList<int>();

                    mdl.Materials = new Material[partCount];
                    mdl.Indices = new int[partCount][];
                    mdl.PartVtxCount = new int[partCount];

                    MeshFace[] faces = data.Faces;

                    for (int i = 0; i < faces.Length; i++)
                    {
                        int matId = faces[i].MaterialIndex;
                        indices[matId].Add(faces[i].IndexA);
                        indices[matId].Add(faces[i].IndexB);
                        indices[matId].Add(faces[i].IndexC);
                    }


                    for (int i = 0; i < partCount; i++)
                    {
                        Material mtrl = mtrls[i][0];
                        mdl.Materials[i] = mtrl;

                        indices[i].Trim();
                        mdl.Indices[i] = indices[i].Elements;

                        int partVtxCount =0;

                        bool[] passed = new bool[data.VertexCount];

                        for (int j = 0; j < mdl.Indices[i].Length; j++) 
                        {
                            passed[indices[i][j]] = true;
                        }

                        for (int j = 0; j < data.VertexCount; j++)
                            if (passed[j])
                                partVtxCount++;

                        mdl.PartVtxCount[i] = partVtxCount;

                    }

                    mdl.VertexCount = data.VertexCount;
                    mdl.VertexData = new byte[data.VertexCount * data.VertexSize];
                    fixed (byte* dst = &mdl.VertexData[0])
                    {
                        Memory.Copy(data.Data.ToPointer(), dst, mdl.VertexData.Length);
                    }

                    tempList.Add(mdl);
                    //#region 添加到表中
                    //FastList<TreeModelData> mdlList;
                    //if (!categoryModels.TryGetValue(mdl.Category, out mdlList))
                    //{
                    //    mdlList = new FastList<TreeModelData>();
                    //    categoryModels.Add(mdl.Category, mdlList);
                    //}
                    //mdlList.Add(ref mdl);


                    //if (!typeModels.TryGetValue(mdl.Type, out mdlList))
                    //{
                    //    mdlList = new FastList<TreeModelData>();
                    //    typeModels.Add(mdl.Type, mdlList);
                    //}
                    //mdlList.Add(ref mdl);

                    //Dictionary <PlantType, FastList <TreeModelData>> typeTbl;
                    //if (!table.TryGetValue(mdl.Category, out typeTbl)) 
                    //{
                    //    typeTbl = new Dictionary<PlantType, FastList<TreeModelData>>();
                    //    table.Add(mdl.Category, typeTbl);
                    //}

                    //if (!typeTbl.TryGetValue(mdl.Type, out mdlList)) 
                    //{
                    //    mdlList = new FastList<TreeModelData>();
                    //    typeTbl.Add(mdl.Type, mdlList);
                    //}
                    //mdlList.Add(ref mdl);


                    //#endregion
                }

            }

            //foreach (KeyValuePair<PlantCategory, FastList<TreeModelData>> e in categoryModels)
            //{
            //    e.Value.Trim();
            //}
            //foreach (KeyValuePair<PlantType, FastList<TreeModelData>> e in typeModels)
            //{
            //    e.Value.Trim();
            //}
            //foreach (KeyValuePair<PlantCategory, Dictionary<PlantType, FastList<TreeModelData>>> e1 in table)
            //{
            //    Dictionary<PlantType, FastList<TreeModelData>> typeTbl = e1.Value;

            //    foreach (KeyValuePair<PlantType, FastList<TreeModelData>> e2 in typeTbl) 
            //    {
            //        e2.Value.Trim();
            //    }
            //}
            tempList.Trim();
        }

        public TreeModelData[] GetAll() 
        {
            return tempList.Elements;
        }
        //public TreeModelData[] Get(PlantCategory cate, PlantType type)
        //{
        //    Dictionary<PlantType, FastList<TreeModelData>> typeTable;
        //    if (table.TryGetValue(cate, out typeTable))
        //    {
        //        FastList<TreeModelData> result;
        //        if (typeTable.TryGetValue(type, out result))
        //        {
        //            return result.Elements;
        //        }
        //    }
        //    return null;
        //}

        //public TreeModelData[] GetCategory(PlantCategory cate)
        //{
        //    FastList<TreeModelData> result;
        //    if (categoryModels.TryGetValue(cate, out result)) 
        //    {
        //        return result.Elements;
        //    }
        //    return null;
        //}

        //public TreeModelData[] GetType(PlantType cate)
        //{
        //    FastList<TreeModelData> result;
        //    if (typeModels.TryGetValue(cate, out result))
        //    {
        //        return result.Elements;
        //    }
        //    return null;
        //}


        protected override void dispose()
        {
            //categoryModels.Clear();
            //typeModels.Clear();
        }
    }
}
