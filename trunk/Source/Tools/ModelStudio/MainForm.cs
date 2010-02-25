using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Vfs;
using DX = SlimDX.Direct3D9;

namespace ModelStudio
{
    public unsafe partial class MainForm : Form
    {
        class EditableModel : ModelBase<MeshData>, IDisposable
        {
            public EditableModel() { }

            protected override MeshData LoadMesh(BinaryDataReader data)
            {
                throw new NotSupportedException();
            }

            protected override BinaryDataWriter SaveMesh(MeshData mesh)
            {
                return mesh.Save();
            }

            protected override void unload()
            {

            }

        }

        DX.Direct3D d3d;
        DX.Device device;

        RenderSystem renderSys;
        public MainForm(RenderSystem rs)
        {
            InitializeComponent();
            renderSys = rs;

            
            d3d = new DX.Direct3D();
            
            DX.PresentParameters pm = new DX.PresentParameters();
            pm.Windowed = true;
            
            device = new DX.Device(d3d, 0, DX.DeviceType.Reference, this.Handle, DX.CreateFlags.None, pm);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {


        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Program.Viewer.CurrentModel = new Model(ModelManager.Instance.CreateInstance(renderSys, new FileLocation(openFileDialog1.FileName)));
            }
        }

        static VertexElementFormat Convert(DX.DeclarationType type)
        {
            switch (type)
            {
                case DX.DeclarationType.Color:
                    return VertexElementFormat.Color;
                case DX.DeclarationType.Dec3N:
                    return VertexElementFormat.Normalized101010;
                case DX.DeclarationType.Float1:
                    return VertexElementFormat.Single;
                case DX.DeclarationType.Float2:
                    return VertexElementFormat.Vector2;
                case DX.DeclarationType.Float3:
                    return VertexElementFormat.Vector3;
                case DX.DeclarationType.Float4:
                    return VertexElementFormat.Vector4;
                case DX.DeclarationType.HalfFour:
                    return VertexElementFormat.HalfVector4;
                case DX.DeclarationType.HalfTwo:
                    return VertexElementFormat.HalfVector2;
                case DX.DeclarationType.Short2:
                    return VertexElementFormat.Short2;
                case DX.DeclarationType.Short2N:
                    return VertexElementFormat.NormalizedShort2;
                case DX.DeclarationType.Short4:
                    return VertexElementFormat.Short4;
                case DX.DeclarationType.Short4N:
                    return VertexElementFormat.NormalizedShort4;
                case DX.DeclarationType.UByte4N:
                    return VertexElementFormat.Rgba32;
                case DX.DeclarationType.UDec3:
                    return VertexElementFormat.UInt101010;
                case DX.DeclarationType.UShort2N:
                    return VertexElementFormat.Rg32;
                case DX.DeclarationType.UShort4N:
                    return VertexElementFormat.Rgba64;
            }
            return VertexElementFormat.Unused;
        }
        static VertexElementUsage Convert(DX.DeclarationUsage usage)
        {
            switch (usage)
            {
                case DX.DeclarationUsage.Binormal:
                    return VertexElementUsage.Binormal;
                case DX.DeclarationUsage.BlendIndices:
                    return VertexElementUsage.BlendIndices;
                case DX.DeclarationUsage.BlendWeight:
                    return VertexElementUsage.BlendWeight;
                case DX.DeclarationUsage.Color:
                    return VertexElementUsage.Color;
                case DX.DeclarationUsage.Depth:
                    return VertexElementUsage.Depth;
                case DX.DeclarationUsage.Fog:
                    return VertexElementUsage.Fog;
                case DX.DeclarationUsage.Normal:
                    return VertexElementUsage.Normal;
                case DX.DeclarationUsage.PointSize:
                    return VertexElementUsage.PointSize;
                case DX.DeclarationUsage.Position:
                    return VertexElementUsage.Position;
                case DX.DeclarationUsage.PositionTransformed:
                    return VertexElementUsage.PositionTransformed;
                case DX.DeclarationUsage.Sample:
                    return VertexElementUsage.Sample;
                case DX.DeclarationUsage.Tangent:
                    return VertexElementUsage.Tangent;
                case DX.DeclarationUsage.TessellateFactor:
                    return VertexElementUsage.TessellateFactor;
                case DX.DeclarationUsage.TextureCoordinate:
                    return VertexElementUsage.TextureCoordinate;
            }
            return VertexElementUsage.TextureCoordinate;
        }

        public static void BuildFromMesh(DX.Mesh mesh, MeshData<Material> data, Material[][] mats)
        {
            void* src = mesh.LockVertexBuffer(DX.LockFlags.None).DataPointer.ToPointer();

            byte[] buffer = new byte[mesh.VertexCount * mesh.BytesPerVertex];

            fixed (byte* dst = &buffer[0])
            {
                Memory.Copy(src, dst, buffer.Length);
                data.SetData(dst, buffer.Length);
            }

            mesh.UnlockVertexBuffer();

            data.Materials = mats;
            data.MaterialAnimation = new MaterialAnimationInstance[mats.Length]; //{ new MaterialAnimationInstance(matAnimData) };
            for (int i = 0; i < mats.Length; i++)
            {
                MaterialAnimation matAnimData = new MaterialAnimation(mats[i].Length, 0.025f);
                data.MaterialAnimation[i] = new MaterialAnimationInstance(matAnimData);
            }
            data.VertexSize = mesh.BytesPerVertex;
            data.VertexCount = mesh.VertexCount;

            DX.VertexElement[] elements = DX.D3DX.DeclaratorFromFVF(mesh.VertexFormat);

            data.VertexElements = new VertexElement[elements.Length];
            for (int i = 0; i < elements.Length - 1; i++)
            {
                data.VertexElements[i] = new VertexElement(elements[i].Offset, Convert(elements[i].Type), Convert(elements[i].Usage), elements[i].UsageIndex);
                //data.VertexElements [i]= new VertexElement (elements[i].Offset,
                //data.VertexElements[i].Index = elements[i].UsageIndex;
                //data.VertexElements[i].Offset = elements[i].Offset;
                //data.VertexElements[i].s
            }
            //Array.Copy(elements, data.VertexElements, elements.Length);

            int faceCount = mesh.FaceCount;

            data.Faces = new MeshFace[faceCount];

            uint* ab = (uint*)mesh.LockAttributeBuffer(DX.LockFlags.ReadOnly).DataPointer.ToPointer();

            if ((mesh.CreationOptions & DX.MeshFlags.Use32Bit) == DX.MeshFlags.Use32Bit)
            {
                uint* ib = (uint*)mesh.LockIndexBuffer(DX.LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < faceCount; i++)
                {
                    int idxId = i * 3;

                    data.Faces[i] = new MeshFace((int)ib[idxId], (int)ib[idxId + 1], (int)ib[idxId + 2], (int)ab[i]);
                }
                mesh.UnlockIndexBuffer();
            }
            else
            {
                ushort* ib = (ushort*)mesh.LockIndexBuffer(DX.LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < faceCount; i++)
                {
                    int idxId = i * 3;

                    data.Faces[i] = new MeshFace(ib[idxId], ib[idxId + 1], ib[idxId + 2], (int)ab[i]);
                }
                mesh.UnlockIndexBuffer();
            }

            mesh.UnlockAttributeBuffer();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                DX.Mesh mesh = DX.Mesh.FromFile(device, openFileDialog2.FileName, DX.MeshFlags.Managed);

                DX.ExtendedMaterial[] mats = mesh.GetMaterials();
                Material[][] outMats = new Material[mats.Length][];
                for (int i = 0; i < mats.Length; i++)
                {
                    outMats[i] = new Material[1];
                    outMats[i][0] = new Material(renderSys);


                    SlimDX.Color4 clr = mats[i].MaterialD3D.Ambient;
                    outMats[i][0].Ambient = new Color4F(clr.Alpha, clr.Red, clr.Green, clr.Blue);

                    clr = mats[i].MaterialD3D.Diffuse;
                    outMats[i][0].Diffuse = new Color4F(clr.Alpha, clr.Red, clr.Green, clr.Blue);

                    clr = mats[i].MaterialD3D.Specular;
                    outMats[i][0].Specular = new Color4F(clr.Alpha, clr.Red, clr.Green, clr.Blue);

                    clr = mats[i].MaterialD3D.Emissive;
                    outMats[i][0].Emissive = new Color4F(clr.Alpha, clr.Red, clr.Green, clr.Blue);

                    outMats[i][0].Power = mats[i].MaterialD3D.Power;
                    outMats[i][0].ZEnabled = true;
                    outMats[i][0].ZWriteEnabled = true;
                    outMats[i][0].CullMode = CullMode.None;
                    outMats[i][0].SetTextureFile(0, Path.GetFileNameWithoutExtension(mats[i].TextureFileName) + ".tex");
                }

                MeshData data = new MeshData(renderSys);


                string name = Path.GetFileNameWithoutExtension(openFileDialog2.FileName);
                data.Name = name;
                data.Materials = outMats;

                BuildFromMesh(mesh, data, outMats);


                EditableModel outMdl = new EditableModel();

                //mesh.Dispose();

                outMdl.Entities = new MeshData[] { data };

                //TransformAnimation transAnim = new TransformAnimation(1);
                //transAnim.Nodes[0].Transforms = new Matrix[1] { Matrix.Identity };

                //outMdl.SetTransformAnimInst(new TransformAnimationInstance(transAnim));
                string dest = Path.Combine(Path.GetDirectoryName(openFileDialog2.FileName), name + ".mesh");
                EditableModel.ToFile(outMdl, dest);

                Program.Viewer.CurrentModel = new Model(ModelManager.Instance.CreateInstance(renderSys, new FileLocation(dest)));

                //outMdl.Dispose();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Model cm = Program.Viewer.CurrentModel;
            if (cm != null) 
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
                {
                    ModelData data = cm.GetData();
                    ModelData.ToFile(data, saveFileDialog1.FileName);
                }
            }
        }


    }
}
