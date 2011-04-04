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
using Apoc3D.Config;
using System.IO;
using Zonelink;
using Apoc3D.Collections;
using Microsoft.Xna.Framework.Graphics;


namespace Code2015.EngineEx
{
    unsafe struct PlantDensityData
    {
        public fixed float Density[PlantDensity.TypeCount];

        public bool IsZero
        {
            get
            {
                fixed (float* d = Density)
                {
                    for (int i = 0; i < PlantDensity.TypeCount; i++)
                    {
                        if (d[i] > float.Epsilon)
                            return false;
                    }
                    return true;
                }
            }
        }
    }
    unsafe class PlantDensity
    {
        static readonly string[] TableNames =
        {
            "plant0.raw", "plant1.raw", "plant2.raw", 
            "plant3.raw", "plant4.raw", "plant5.raw", 
            "plant6.raw", "plant7.raw", "plant8.raw"
        };
        static readonly string DensityFile = "density.raw";

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
        byte[] density;

        private PlantDensity()
        {
            string parentpath = Path.Combine(Game1.ContentDir, "Nature");

            densityTable = new byte[TypeCount][];
            for (int i = 0; i < TypeCount; i++)
            {
                string path = Path.Combine(parentpath, TableNames[i]);

                FileStream fl = File.Open(path, FileMode.Open, FileAccess.Read);

                BinaryReader br = new BinaryReader(fl);
                densityTable[i] = br.ReadBytes(Width * Height);

                br.Close();
            }

            string path2 = Path.Combine(parentpath, DensityFile);

            //density = new byte[Width * Height];
            FileStream fl2 = File.Open(path2, FileMode.Open, FileAccess.Read);
            BinaryReader br2 = new BinaryReader(fl2);
            density = br2.ReadBytes(Width * Height);
            br2.Close();
        }

        public float GetPlantDensity(float longtiude, float latitude)
        {
            const double yspan = Math.PI;

            int y = (int)(((yspan * 0.5 - latitude) / yspan) * Height);
            int x = (int)(((longtiude + Math.PI) / (2 * Math.PI)) * Width);

            if (y < 0) y += Height;
            if (y >= Height) y -= Height;

            if (x < 0) x += Width;
            if (x >= Width) x -= Width;

            int idx = y * Width + x;

            return density[idx] / 255f;
        }
        public PlantDensityData GetDensity(float longtiude, float latitude)
        {
            const double yspan = Math.PI;

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
                //if (result.Density[i] < 0.1f)
                //    result.Density[i] = 0.1f;
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

        

        Game1 game;
        TreeModelData trunk;
        FastList<TreeModelData>[] typedList = new FastList<TreeModelData>[PlantDensity.TypeCount];
        FastList<ModelMemoryData> loadedModels
            = new FastList<ModelMemoryData>();

        unsafe void BuildTrunk(Game1 rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("shuzhuang.mesh", GameFileLocs.Model);
            ModelMemoryData mdlData2 = new ModelMemoryData(rs, fl);
            loadedModels.Add(mdlData2);
            MeshData[] dataArr2 = mdlData2.Entities;
            if (dataArr2.Length == 1)
            {
                TreeModelData mdl;

                
                
                
                MeshData data = dataArr2[0];

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

                    int partVtxCount = 0;

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
                trunk = mdl;
            }
        }

        private unsafe TreeModelLibrary(Game1 rs)
        {
            game = rs;

            FileLocation fl = FileSystem.Instance.Locate("trees.xml", GameFileLocs.Config);
            Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);

            foreach (KeyValuePair<string, ConfigurationSection> s in conf)
            {
                ConfigurationSection sect = s.Value;
                TreeModelData mdl;

                int type = sect.GetInt("Type", 0);
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

                        int partVtxCount = 0;

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

                    if (typedList[type] == null)
                    {
                        typedList[type] = new FastList<TreeModelData>();
                    }

                    typedList[type].Add(mdl);


                }
                BuildTrunk(rs);
            }




            for (int i = 0; i < typedList.Length; i++)
            {
                typedList[i].Trim();
            }
        }

        //public TreeModelData[] GetAll() 
        //{
        //    return tempList.Elements;
        //}
        public TreeModelData GetTrunk()
        {
            return trunk;
        }
        public TreeModelData[] Get(int typeId)
        {
            return typedList[typeId].Elements;
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
