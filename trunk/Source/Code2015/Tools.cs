using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Apoc3D.MathLib;
using Apoc3D.Collections;

namespace Code2015
{
    static class Tools
    {
        static readonly string EntityCountTag = "EntityCount";
        static readonly string EntityPrefix = "Ent";

        static BinaryDataWriter SaveMesh(MeshData ent)
        {
            return ent.Save();
        }
        static MeshData LoadMesh(RenderSystem rs, BinaryDataReader data)
        {
            MeshData md = new MeshData(rs);
            md.Load(data);
            return md;
        }
        static MeshData[] ReadData(RenderSystem rs, BinaryDataReader data)
        {
            int entCount = data.GetDataInt32(EntityCountTag);
            MeshData[] entities = new MeshData[entCount];

            ContentBinaryReader br;
            for (int i = 0; i < entCount; i++)
            {
                br = data.GetData(EntityPrefix + i.ToString());
                BinaryDataReader meshData = br.ReadBinaryData();
                entities[i] = LoadMesh(rs, meshData);
                meshData.Close();
                br.Close();
            }
            return entities;
            //ModelAnimationFlags flags = (ModelAnimationFlags)data.GetDataInt32(AnimationFlagTag);
            //BinaryDataReader animData;

            //if ((flags & ModelAnimationFlags.EntityTransform) == ModelAnimationFlags.EntityTransform)
            //{
            //    br = data.GetData(AnimationTag + ModelAnimationFlags.EntityTransform.ToString());
            //    TransformAnimation transAnimDat = new TransformAnimation(entities.Length);

            //    animData = br.ReadBinaryData();
            //    transAnimDat.Load(animData);
            //    animData.Close();

            //    br.Close();

            //    transAnim = new TransformAnimationInstance(transAnimDat);
            //}
            //if ((flags & ModelAnimationFlags.Skin) == ModelAnimationFlags.Skin)
            //{

            //}

            //br = data.TryGetData(LodMeshTag);
            //if (br != null)
            //{
            //    BinaryDataReader meshData = br.ReadBinaryData();
            //    lodMesh = LoadMesh(meshData);

            //    meshData.Close();
            //    br.Close();
            //}

        }
        public unsafe static void ModelRecenter(RenderSystem rs, string srcDir)
        {
            FastList<MeshData[]> list = new FastList<MeshData[]>();
            const string OutDir = @"E:\Desktop\out";
            string[] files = Directory.GetFiles(srcDir, "*.mesh", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < files.Length; i++)
            {
                ContentBinaryReader br = new ContentBinaryReader(new FileLocation(files[i]));
                if (br.ReadInt32() == 0)
                {
                    BinaryDataReader data = br.ReadBinaryData();

                    MeshData[] ents = ReadData(rs, data);

                    for (int j = 0; j < ents.Length; j++)
                    {
                        if (ents[j].VertexSize == VertexPNT1.Size)
                        {
                            Vector3 center = Vector3.Zero;

                            VertexPNT1* pnt1 = (VertexPNT1*)ents[j].Data;
                            for (int k = 0; k < ents[j].VertexCount; k++)
                            {
                                center += pnt1[k].pos;
                            }
                            center /= (float)ents[j].VertexCount;

                            for (int k = 0; k < ents[j].VertexCount; k++)
                            {
                                pnt1[k].pos -= center;
                            }
                        }
                    }

                    data.Close();
                    list.Add(ents);
                    {
                        BinaryDataWriter outData = new BinaryDataWriter();
                        outData.AddEntry(EntityCountTag, ents.Length);

                        ContentBinaryWriter bw;
                        for (int j = 0; j < ents.Length; j++)
                        {
                            bw = outData.AddEntry(EntityPrefix + j.ToString());

                            BinaryDataWriter meshData = SaveMesh(ents[j]);
                            bw.Write(meshData);
                            meshData.Dispose();
                            bw.Close();
                        }

                        string dest = Path.Combine(OutDir, Path.GetFileName(files[i]));
                        bw = new ContentBinaryWriter(File.Open(dest, FileMode.OpenOrCreate, FileAccess.Write));

                        bw.Write(ModelData.MdlId);
                        bw.Write(outData);
                        outData.Dispose();

                        bw.Close();
                    }
                }

                br.Close();

            }
        }
    }
}
