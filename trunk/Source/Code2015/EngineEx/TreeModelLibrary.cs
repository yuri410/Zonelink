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
        Dictionary<PlantCategory, TreeModelData> categoryModels = new Dictionary<PlantCategory, TreeModelData>();
        Dictionary<PlantType, TreeModelData> typeModels = new Dictionary<PlantType, TreeModelData>();


        private unsafe TreeModelLibrary(RenderSystem rs)
        {
            renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("trees.xml", GameFileLocs.Config);
            Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);

            foreach (KeyValuePair<string, ConfigurationSection> s in conf)
            {
                ConfigurationSection sect = s.Value;
                TreeModelData mdl;

                mdl.Category = (PlantCategory)Enum.Parse(typeof(PlantCategory), sect.GetString("Category", string.Empty));
                mdl.Type = (PlantType)Enum.Parse(typeof(PlantType), sect.GetString("Type", string.Empty));

                string fileName = sect.GetString("File", string.Empty);
                FileLocation fl2 = new FileLocation(fileName);

                ModelMemoryData mdlData = new ModelMemoryData(rs, fl2);

                MeshData[] dataArr = mdlData.Entities;

                if (dataArr.Length == 1)
                {
                    MeshData data = dataArr[0];

                    Material[][] mtrls = data.Materials;

                    int partCount = mtrls.Length;
                    FastList<int>[] indices = new FastList<int>[partCount];

                    mdl.Materials = new Material[partCount];
                    mdl.Indices = new int[partCount][];


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
                    }

                    mdl.VertexCount = data.VertexCount;
                    mdl.VertexData = new byte[data.VertexCount * data.VertexSize];
                    fixed (byte* dst = &mdl.VertexData[0])
                    {
                        Memory.Copy(data.Data.ToPointer(), dst, mdl.VertexData.Length);
                    }


                }
            }
        }




        protected override void dispose()
        {
            categoryModels.Clear();
            typeModels.Clear();
        }
    }
}
