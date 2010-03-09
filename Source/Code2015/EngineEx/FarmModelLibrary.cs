using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class FarmModelLibrary : Singleton
    {
        static FarmModelLibrary singleton;

        public static FarmModelLibrary Instance
        {
            get { return singleton; }
        }

        public static void Initialize(RenderSystem rs)
        {
            singleton = new FarmModelLibrary(rs);
        }

        RenderSystem renderSys;

        FastList<ModelMemoryData> loadedModels
            = new FastList<ModelMemoryData>();



        

        private unsafe FarmModelLibrary(RenderSystem rs)
        {
            renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("farms.xml", GameFileLocs.Config);
            Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);

            foreach (KeyValuePair<string, ConfigurationSection> s in conf)
            {
                ConfigurationSection sect = s.Value;
                //TreeModelData mdl;

                //mdl.Category = (PlantCategory)Enum.Parse(typeof(PlantCategory), sect.GetString("Category", string.Empty));
                //mdl.Type = (PlantType)Enum.Parse(typeof(PlantType), sect.GetString("Type", string.Empty));

                //string fileName = sect.GetString("Level0", string.Empty);
                //FileLocation fl2 = FileSystem.Instance.Locate(fileName, GameFileLocs.Model);

                //ModelMemoryData mdlData = new ModelMemoryData(rs, fl2);

                //loadedModels.Add(mdlData);

                //MeshData[] dataArr = mdlData.Entities;

                //if (dataArr.Length == 1)
                //{
                //    MeshData data = dataArr[0];

                //    Material[][] mtrls = data.Materials;

                //    int partCount = mtrls.Length;
                //    FastList<int>[] indices = new FastList<int>[partCount];
                //    for (int i = 0; i < partCount; i++)
                //        indices[i] = new FastList<int>();

                //    mdl.Materials = new Material[partCount];
                //    mdl.Indices = new int[partCount][];
                //    mdl.PartVtxCount = new int[partCount];

                //    MeshFace[] faces = data.Faces;

                //    for (int i = 0; i < faces.Length; i++)
                //    {
                //        int matId = faces[i].MaterialIndex;
                //        indices[matId].Add(faces[i].IndexA);
                //        indices[matId].Add(faces[i].IndexB);
                //        indices[matId].Add(faces[i].IndexC);
                //    }


                //    for (int i = 0; i < partCount; i++)
                //    {
                //        Material mtrl = mtrls[i][0];
                //        mdl.Materials[i] = mtrl;

                //        indices[i].Trim();
                //        mdl.Indices[i] = indices[i].Elements;

                //        int partVtxCount =0;

                //        bool[] passed = new bool[data.VertexCount];

                //        for (int j = 0; j < mdl.Indices[i].Length; j++) 
                //        {
                //            passed[indices[i][j]] = true;
                //        }

                //        for (int j = 0; j < data.VertexCount; j++)
                //            if (passed[j])
                //                partVtxCount++;

                //        mdl.PartVtxCount[i] = partVtxCount;

                //    }

                //    mdl.VertexCount = data.VertexCount;
                //    mdl.VertexData = new byte[data.VertexCount * data.VertexSize];
                //    fixed (byte* dst = &mdl.VertexData[0])
                //    {
                //        Memory.Copy(data.Data.ToPointer(), dst, mdl.VertexData.Length);
                //    }


                //    #region 添加到表中
                //    FastList<TreeModelData> mdlList;
                //    if (!categoryModels.TryGetValue(mdl.Category, out mdlList))
                //    {
                //        mdlList = new FastList<TreeModelData>();
                //        categoryModels.Add(mdl.Category, mdlList);
                //    }
                //    mdlList.Add(ref mdl);


                //    if (!typeModels.TryGetValue(mdl.Type, out mdlList))
                //    {
                //        mdlList = new FastList<TreeModelData>();
                //        typeModels.Add(mdl.Type, mdlList);
                //    }
                //    mdlList.Add(ref mdl);

                //    Dictionary <PlantType, FastList <TreeModelData>> typeTbl;
                //    if (!table.TryGetValue(mdl.Category, out typeTbl)) 
                //    {
                //        typeTbl = new Dictionary<PlantType, FastList<TreeModelData>>();
                //        table.Add(mdl.Category, typeTbl);
                //    }

                //    if (!typeTbl.TryGetValue(mdl.Type, out mdlList)) 
                //    {
                //        mdlList = new FastList<TreeModelData>();
                //        typeTbl.Add(mdl.Type, mdlList);
                //    }
                //    mdlList.Add(ref mdl);


                //    #endregion
                //}
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
