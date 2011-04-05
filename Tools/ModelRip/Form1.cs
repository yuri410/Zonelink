using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using System.IO;
using Apoc3D;

namespace ModelRip
{
    public unsafe partial class Form1 : Form
    {

        public const int MdlId = 0;

        protected static readonly string EntityCountTag = "EntityCount";
        protected static readonly string EntityPrefix = "Ent";

        public const int MeshId = ((byte)'M' << 24) | ((byte)'E' << 16) | ((byte)'S' << 8) | ((byte)'H');

        protected static readonly string MaterialCountTag = "MaterialCount";
        protected static readonly string MaterialsTag = "Materials";

        protected static readonly string MaterialAnimationTag = "MaterialAnimation";
        protected static readonly string FaceCountTag = "FaceCount";
        protected static readonly string FacesTag = "Faces";
        //protected static readonly string VertexFormatTag = "VertexFormat";
        protected static readonly string VertexDeclTag = "VertexDeclaration";
        protected static readonly string VertexCountTag = "VertexCount";
        protected static readonly string VertexSizeTag = "VertexSize";

        protected static readonly string DataTag = "VertexData";

        protected static readonly string NameTag = "Name";
        static readonly string MaterialColorTag = "MaterialColor";
        static readonly string HasTextureTag = "HasTexture";
        static readonly string TextureTag = "Texture";

        /// <summary>
        ///  虚拟流，通常用来读取其他流之中的一段数据而不影响那个流。
        /// </summary>
        public class VirtualStream : Stream
        {
            Stream stream;

            long length;
            long baseOffset;

            bool isOutput;


            public Stream BaseStream
            {
                get { return stream; }
            }

            public VirtualStream(Stream stream)
            {
                isOutput = true;
                this.stream = stream;
                this.length = stream.Length;
                this.baseOffset = 0;
                stream.Position = 0;
            }
            public VirtualStream(Stream stream, long baseOffset)
            {
                isOutput = true;
                this.stream = stream;
                this.baseOffset = 0;
                stream.Position = baseOffset;
            }
            public VirtualStream(Stream stream, long baseOffset, long length)
            {
                stream.Position = baseOffset;

                this.stream = stream;
                this.length = length;
                this.baseOffset = baseOffset;
                stream.Position = baseOffset;
            }


            public bool IsOutput
            {
                get { return isOutput; }
            }
            public long BaseOffset
            {
                get { return baseOffset; }
            }
            public override bool CanRead
            {
                get { return stream.CanRead; }
            }
            public override bool CanSeek
            {
                get { return stream.CanSeek; }
            }
            public override bool CanWrite
            {
                get { return stream.CanWrite; }
            }
            public override bool CanTimeout
            {
                get { return stream.CanTimeout; }
            }

            public override void Flush()
            {
                stream.Flush();
            }

            public override long Length
            {
                get { return isOutput ? stream.Length : length; }
            }

            public long AbsolutePosition
            {
                get { return stream.Position; }
            }
            public override long Position
            {
                get
                {
                    return stream.Position - baseOffset;
                }
                set
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException();
                    if (value > Length)
                        throw new EndOfStreamException();
                    stream.Position = value + baseOffset;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (Position + count > length)
                {
                    count = (int)(length - Position);
                }
                if (count > 0)
                {
                    return stream.Read(buffer, offset, count);
                }
                return 0;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        if (offset > length)
                        {
                            offset = length;
                        }
                        if (offset < 0)
                        {
                            offset = 0;
                        }
                        break;
                    case SeekOrigin.Current:
                        if (stream.Position + offset > baseOffset + length)
                        {
                            offset = baseOffset + length - stream.Position;
                        }
                        if (stream.Position + offset < baseOffset)
                        {
                            offset = baseOffset - stream.Position;
                        }
                        break;
                    case SeekOrigin.End:
                        if (offset > 0)
                        {
                            offset = 0;
                        }
                        if (offset < -length)
                        {
                            offset = -length;
                        }
                        break;
                }
                return stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                stream.Write(buffer, offset, count);
                if (isOutput)
                    length += count;
            }
            public override void WriteByte(byte value)
            {
                stream.WriteByte(value);
                if (isOutput)
                    length++;
            }

            public override void Close() { }
        }
        public class ContentBinaryReader : BinaryReader
        {
            //bool closeStream = true;

            public ContentBinaryReader(FileStream fl)
                : this(fl, Encoding.Default)
            { }

            public ContentBinaryReader(Stream src)
                : base(src)
            { }

            public ContentBinaryReader(Stream src, Encoding enc)
                : base(src, enc)
            { }

            //public bool AutoCloseStream
            //{
            //    get { return closeStream; }
            //    set { closeStream = value; }
            //}

            //public override void Close()
            //{
            //    base.Close();
            //}

            public void ReadMatrix(out Matrix mat)
            {
                mat.M11 = ReadSingle();
                mat.M12 = ReadSingle();
                mat.M13 = ReadSingle();
                mat.M14 = ReadSingle();
                mat.M21 = ReadSingle();
                mat.M22 = ReadSingle();
                mat.M23 = ReadSingle();
                mat.M24 = ReadSingle();
                mat.M31 = ReadSingle();
                mat.M32 = ReadSingle();
                mat.M33 = ReadSingle();
                mat.M34 = ReadSingle();
                mat.M41 = ReadSingle();
                mat.M42 = ReadSingle();
                mat.M43 = ReadSingle();
                mat.M44 = ReadSingle();
            }

            public string ReadStringUnicode()
            {
                int len = ReadInt32();
                char[] chars = new char[len];
                for (int i = 0; i < len; i++)
                {
                    chars[i] = (char)ReadUInt16();
                }
                //char[] chars = ReadChars(len);
                return new string(chars);
            }

            public Matrix ReadMatrix()
            {
                Matrix mat;
                mat.M11 = ReadSingle();
                mat.M12 = ReadSingle();
                mat.M13 = ReadSingle();
                mat.M14 = ReadSingle();
                mat.M21 = ReadSingle();
                mat.M22 = ReadSingle();
                mat.M23 = ReadSingle();
                mat.M24 = ReadSingle();
                mat.M31 = ReadSingle();
                mat.M32 = ReadSingle();
                mat.M33 = ReadSingle();
                mat.M34 = ReadSingle();
                mat.M41 = ReadSingle();
                mat.M42 = ReadSingle();
                mat.M43 = ReadSingle();
                mat.M44 = ReadSingle();
                return mat;
            }

            

            /// <summary>
            ///  读取一个BinaryDataReader数据块。
            /// </summary>
            /// <returns></returns>
            public BinaryDataReader ReadBinaryData()
            {
                int size = ReadInt32();

                VirtualStream vs = new VirtualStream(BaseStream, BaseStream.Position, size);
                return new BinaryDataReader(vs);
            }

            public void Close(bool closeBaseStream)
            {
                base.Dispose(closeBaseStream);
            }
        }
        /// <summary>
        ///  定义一种由若干“键”—“值”组成的集合的存储方式  的读取器
        ///  “键”为字符串，“值”为二进制数据块。
        ///  
        ///  意义：可以不按先后顺序将数据存储，可以方便增添或减少存储的项目。
        /// </summary>
        public unsafe class BinaryDataReader
        {
            /// <summary>
            ///  定义一个“键”—“值”的存储项
            /// </summary>
            struct Entry
            {
                public string name;
                public int offset;
                public int size;

                public Entry(string name, int offset, int size)
                {
                    this.name = name;
                    this.offset = offset;
                    this.size = size;
                }
            }


            int sectCount;
            Dictionary<string, Entry> positions;
            Stream stream;

            byte[] buffer;

            public BinaryDataReader(Stream stm)
            {
                stream = stm;
                buffer = new byte[sizeof(decimal)];

                ContentBinaryReader br = new ContentBinaryReader(stm, Encoding.Default);

                // 读出所有块
                sectCount = br.ReadInt32();
                positions = new Dictionary<string, Entry>(sectCount);

                for (int i = 0; i < sectCount; i++)
                {
                    string name = br.ReadStringUnicode();
                    int size = br.ReadInt32();

                    positions.Add(name, new Entry(name, (int)br.BaseStream.Position, size));

                    br.BaseStream.Position += size;
                }
                br.Close();
            }

            public bool Contains(string name)
            {
                return positions.ContainsKey(name);
            }

            public ContentBinaryReader TryGetData(string name)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {
                    return new ContentBinaryReader(new VirtualStream(stream, ent.offset, ent.size));
                }
                return null;
            }
            public ContentBinaryReader GetData(string name)
            {
                Entry ent = positions[name];
                return new ContentBinaryReader(new VirtualStream(stream, ent.offset, ent.size));
            }
            public Stream GetDataStream(string name)
            {
                Entry ent = positions[name];
                return new VirtualStream(stream, ent.offset, ent.size);
            }
            public int GetDataInt32(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(int));

                return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);
            }
            //[CLSCompliant(false)]
            public uint GetDataUInt32(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(uint));

                return (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
            }

            public short GetDataInt16(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(short));

                return (short)(buffer[0] | (buffer[1] << 8));
            }
            //[CLSCompliant(false)]
            public ushort GetDataUInt16(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(ushort));

                return (ushort)(buffer[0] | (buffer[1] << 8));
            }

            public long GetDataInt64(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(long));

                uint num = (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
                uint num2 = (uint)(buffer[4] | (buffer[5] << 8) | (buffer[6] << 16) | (buffer[7] << 24));
                return (long)((num2 << 32) | num);
            }
            //[CLSCompliant(false)]
            public ulong GetDataUInt64(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(ulong));

                uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
                return ((num2 << 32) | num);
            }

            public bool GetDataBool(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(bool));

                return (buffer[0] != 0);
            }

            public float GetDataSingle(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(float));

                uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                return *(((float*)&num));
            }
            public float GetDataDouble(string name)
            {
                Entry ent = positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(float));

                uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
                ulong num3 = (num2 << 32) | num;
                return *(((float*)&num3));

            }




            public int GetDataInt32(string name, int def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {//= positions[name];

                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(int));

                    return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);
                }
                return def;
            }
            public uint GetDataUInt32(string name, uint def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {
                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(uint));

                    return (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
                }
                return def;
            }

            public short GetDataInt16(string name, short def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {
                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(short));

                    return (short)(buffer[0] | (buffer[1] << 8));
                }
                return def;
            }
            public ushort GetDataUInt16(string name, ushort def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {

                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(ushort));

                    return (ushort)(buffer[0] | (buffer[1] << 8));
                }
                return def;
            }

            public long GetDataInt64(string name, long def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {
                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(long));

                    uint num = (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
                    uint num2 = (uint)(buffer[4] | (buffer[5] << 8) | (buffer[6] << 16) | (buffer[7] << 24));
                    return (long)((num2 << 32) | num);
                }
                return def;
            }
            public ulong GetDataUInt64(string name, ulong def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {
                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(ulong));

                    uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                    uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
                    return ((num2 << 32) | num);
                }
                return def;
            }

            public bool GetDataBool(string name, bool def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {
                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(bool));

                    return (buffer[0] != 0);
                }
                return def;
            }

            public float GetDataSingle(string name, float def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {
                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(float));

                    uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                    return *(((float*)&num));
                }
                return def;
            }
            public float GetDataDouble(string name, float def)
            {
                Entry ent;
                if (positions.TryGetValue(name, out ent))
                {
                    stream.Position = ent.offset;
                    stream.Read(buffer, 0, sizeof(float));

                    uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                    uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
                    ulong num3 = (num2 << 32) | num;
                    return *(((float*)&num3));
                }
                return def;
            }



            public void Close()
            {
                stream.Close();
            }


            public int GetChunkOffset(string name)
            {
                Entry ent = positions[name];
                return ent.offset;
            }
            public Stream BaseStream
            {
                get { return stream; }
            }
        }
        /// <summary>
        /// mesh的最小组成部分-三角形
        /// </summary>
        public struct MeshFace
        {
            #region Fields
            int a;
            int b;
            int c;

            int materialIdx;
            #endregion

            #region Constructors
            public MeshFace(int A, int B, int C)
            {
                a = A;
                b = B;
                c = C;
                materialIdx = -1;
            }
            public MeshFace(int A, int B, int C, int matId)
            {
                a = A;
                b = B;
                c = C;
                materialIdx = matId;
            }
            #endregion

            #region Properties
            public int IndexA
            {
                get { return a; }
                set { a = value; }
            }
            public int IndexB
            {
                get { return b; }
                set { b = value; }
            }
            public int IndexC
            {
                get { return c; }
                set { c = value; }
            }

            public int MaterialIndex
            {
                get { return materialIdx; }
                set { materialIdx = value; }
            }
            #endregion
        }

        const string OutputDir = @"E:\Desktop\model_dump";

        Direct3D d3d;
        Device device;

        struct MeshVertex 
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TexCoord;
        }
        struct Entity 
        {
            public ExtendedMaterial[] Materials;
            public MeshFace[] Faces;
            public int VertexCount;
            public byte[] VertexBuffer;
        }

        public Form1()
        {
            InitializeComponent();
            d3d = new Direct3D();

            PresentParameters pm = new PresentParameters();
            pm.Windowed = true;
            pm.SwapEffect = SwapEffect.Discard;
            
            device = new Device(d3d,0, DeviceType.Reference, panel1.Handle, CreateFlags.SoftwareVertexProcessing | CreateFlags.FpuPreserve, pm);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] files = openFileDialog1.FileNames;
                for (int i = 0; i < files.Length; i++)
                {
                    listBox1.Items.Add(files[i]);
                }
            }
        }

        ExtendedMaterial LoadMaterial(BinaryDataReader data)
        {

            Material d3dMtrl = new Material();
            ExtendedMaterial result = new ExtendedMaterial();
            ContentBinaryReader br;


            br = data.GetData(MaterialColorTag);
            Color4 ambient;
            ambient.Alpha = br.ReadSingle();
            ambient.Red = br.ReadSingle();
            ambient.Green = br.ReadSingle();
            ambient.Blue = br.ReadSingle();
            d3dMtrl.Ambient = ambient;

            Color4 diffuse;
            diffuse.Alpha = br.ReadSingle();
            diffuse.Red = br.ReadSingle();
            diffuse.Green = br.ReadSingle();
            diffuse.Blue = br.ReadSingle();
            d3dMtrl.Diffuse = diffuse;

            Color4 specular;
            specular.Alpha = br.ReadSingle();
            specular.Red = br.ReadSingle();
            specular.Green = br.ReadSingle();
            specular.Blue = br.ReadSingle();
            d3dMtrl.Specular = specular;

            Color4 emissive;
            emissive.Alpha = br.ReadSingle();
            emissive.Red = br.ReadSingle();
            emissive.Green = br.ReadSingle();
            emissive.Blue = br.ReadSingle();
            d3dMtrl.Emissive = emissive;

            d3dMtrl.Power = br.ReadSingle();

            br.Close();

            
            //br = data.GetData(EffectTag);
            //effectName = br.ReadStringUnicode();
            //if (effectName.Length == 0)
            //    effectName = "Standard";
            ////if (!string.IsNullOrEmpty(effectName))
            //Effect = LoadEffect(effectName);

            //br.Close();

            //br = data.GetData(IsTexEmbededTag);
            //for (int i = 0; i < MaxTexLayers; i++)
            //{
            //    texEmbeded[i] = br.ReadBoolean();
            //}
            //br.Close();
            result.MaterialD3D = d3dMtrl;
            
            const int MaxTexLayers = 16;
            bool[] hasTexture = new bool[MaxTexLayers];
            string[] textureFiles = new string[MaxTexLayers];

            br = data.GetData(HasTextureTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                hasTexture[i] = br.ReadBoolean();
            }
            br.Close();


            for (int i = 0; i < MaxTexLayers; i++)
            {
                if (hasTexture[i])
                {
                    br = data.GetData(TextureTag + i.ToString());

                    string fn = br.ReadStringUnicode();
                    textureFiles[i] = fn;
                    //textures[i] = LoadTexture(br, i);
                    br.Close();
                }
            }
            if (hasTexture[0])
                result.TextureFileName = textureFiles[0];
            else
                result.TextureFileName = string.Empty;
            return result;
        }
        Entity LoadMesh(BinaryDataReader data)
        {
            Entity result;
            int materialCount = data.GetDataInt32(MaterialCountTag);
            result.Materials = new ExtendedMaterial[materialCount];

            ContentBinaryReader br = data.GetData(MaterialsTag);
            for (int i = 0; i < materialCount; i++)
            {
                int frameCount = br.ReadInt32();

                {
                    BinaryDataReader matData = br.ReadBinaryData();
                    result.Materials[i] = LoadMaterial( matData);
                    matData.Close();
                }
            }
            br.Close();



            br = data.GetData(NameTag);
            Name = br.ReadStringUnicode();
            br.Close();

            #region 读取面
            int faceCount = data.GetDataInt32(FaceCountTag);
            result.Faces = new MeshFace[faceCount];

            br = data.GetData(FacesTag);
            for (int i = 0; i < faceCount; i++)
            {
                result.Faces[i].IndexA = br.ReadInt32();
                result.Faces[i].IndexB = br.ReadInt32();
                result.Faces[i].IndexC = br.ReadInt32();

                result.Faces[i].MaterialIndex = br.ReadInt32();
            }
            br.Close();
            #endregion

            int VertexSize = data.GetDataInt32(VertexSizeTag);
            if (VertexSize != 32)
                throw new NotSupportedException();

            result.VertexCount = data.GetDataInt32(VertexCountTag);

            br = data.GetData(DataTag);
            result.VertexBuffer = br.ReadBytes(VertexSize * result.VertexCount);
            br.Close();
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                string srcFile = (string)listBox1.Items[i];
                string srcFileName = Path.GetFileNameWithoutExtension(srcFile);

                FileStream fs = File.Open(srcFile, FileMode.Open, FileAccess.Read);
                ContentBinaryReader br = new ContentBinaryReader(fs);

                if (br.ReadInt32() == MdlId)
                {
                    BinaryDataReader data = br.ReadBinaryData();


                    int entCount = data.GetDataInt32(EntityCountTag);
                    Entity[] entities = new Entity[entCount];

                    ContentBinaryReader br2;
                    for (int j = 0; j < entCount; j++)
                    {
                        br2 = data.GetData(EntityPrefix + j.ToString());
                        BinaryDataReader meshData = br.ReadBinaryData();
                        entities[j] = LoadMesh(meshData);
                        meshData.Close();
                        br2.Close();
                    }


                    data.Close();


                    int totalFaceCount = 0;
                    int totalVertexCount = 0;
                    for (int j = 0; j < entCount; j++)
                    {
                        totalFaceCount += entities[i].Faces.Length;
                        totalVertexCount += entities[i].VertexCount;
                    }


                    

                    Mesh mesh = new Mesh(device, totalFaceCount, totalVertexCount, 
                        MeshFlags.Use32Bit | MeshFlags.SystemMemory, VertexFormat.PositionNormal | VertexFormat.Texture1);

                    #region 重组定点索引生成属性
                    List<ExtendedMaterial> materials = new List<ExtendedMaterial>();
                    List<MeshVertex> vertices = new List<MeshVertex>(totalVertexCount);
                    List<uint> indices = new List<uint>(totalFaceCount * 3);
                    List<uint> attribute = new List<uint>(totalFaceCount);

                    int idxOffset = 0;
                    int vtxOffset = 0;
                    int materialOffset = 0;
                    for (int j = 0; j < entCount; j++)
                    {
                        Entity ent = entities[j];

                        MeshVertex[] vtx = new MeshVertex[ent.VertexCount];
                        fixed (MeshVertex* dst = &vtx[0])
                        {
                            fixed (byte* src = &ent.VertexBuffer[0])
                            {
                                Memory.Copy(src, dst, ent.VertexBuffer.Length);
                            }
                        }
                        for (int k = 0; k < vtx.Length; k++)
                        {
                            vertices.Add(vtx[k]);
                        }
                        for (int k = 0; k < ent.Materials.Length; k++)
                        {
                            materials.Add(ent.Materials[k]);
                        }

                        for (int k = 0; k < ent.Faces.Length; k++)
                        {
                            int mid = ent.Faces[k].MaterialIndex + materialOffset;
                            if (mid < 0)
                                mid = 0;
                            attribute.Add((uint)mid);
                            indices.Add((uint)(ent.Faces[k].IndexA + vtxOffset));
                            indices.Add((uint)(ent.Faces[k].IndexB + vtxOffset));
                            indices.Add((uint)(ent.Faces[k].IndexC + vtxOffset));
                        }

                        idxOffset += ent.Faces.Length * 3;
                        vtxOffset += ent.VertexCount;
                        materialOffset += ent.Materials.Length;
                    }
                    #endregion

                    #region 存入Mesh

                    uint* idxPtr = (uint*)mesh.LockIndexBuffer(LockFlags.None).DataPointer;
                    MeshVertex* vtxPtr= ( MeshVertex*)mesh.LockVertexBuffer(LockFlags.None).DataPointer;
                    uint* attPtr = (uint*)mesh.LockAttributeBuffer(LockFlags.None).DataPointer;


                    for (int j = 0; j < totalVertexCount; j++)
                    {
                        vtxPtr[j] = vertices[j];
                    }

                    for (int j = 0; j < indices.Count; j++) 
                    {
                        idxPtr[j] = indices[j];
                    }
                    for (int j = 0; j < attribute.Count; j++) 
                    {
                        attPtr[j] = attribute[j];
                    }


                    mesh.UnlockIndexBuffer();
                    mesh.UnlockVertexBuffer();
                    mesh.UnlockAttributeBuffer();
                    #endregion

                    mesh.SetMaterials(materials.ToArray());
                    mesh.OptimizeInPlace(MeshOptimizeFlags.AttributeSort);

                    Mesh.ToXFile(mesh, Path.Combine(OutputDir, srcFileName), XFileFormat.Text);
                    mesh.Dispose();
                }
                else
                {
                    MessageBox.Show("error");
                }
                br.Close();
            }
        }
    }
}
