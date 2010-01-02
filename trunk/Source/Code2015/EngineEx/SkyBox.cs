using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.Effects;

namespace Code2015.Graphics
{
    /// <summary>
    ///  定义一个天空盒
    /// </summary>
    public class SkyBox : SceneObject, IConfigurable, IDisposable
    {
        struct SkyVertex
        {
            public Vector3 pos;
            public Vector3 texCoord;

            static readonly VertexElement[] elements;

            static SkyVertex()
            {
                elements = new VertexElement[2];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position);
                elements[1] = new VertexElement(1, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0);
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }
        }

        bool disposed;

        //Texture dayTex;
        //Texture nightTex;

        VertexBuffer box;
        IndexBuffer indexBuffer;

        VertexDeclaration vtxDecl;

        RenderSystem renderSystem;
        ObjectFactory factory;

        Effect skyEff;

        RenderOperation[] opList;

        public unsafe SkyBox(RenderSystem rs)
            : base(false)
        {
            renderSystem = rs;
            factory = rs.ObjectFactory;

            // sqrt(3)/3
            const float l = 1f / MathEx.Root3;

            vtxDecl = factory.CreateVertexDeclaration(SkyVertex.Elements);
            box = factory.CreateVertexBuffer(8, vtxDecl, BufferUsage.WriteOnly);

            SkyVertex* dst = (SkyVertex*)box.Lock(0, 0, LockMode.None);

            dst[0] = new SkyVertex { pos = new Vector3(-50f, -50f, -50f), texCoord = new Vector3(-l, -l, -l) };
            dst[1] = new SkyVertex { pos = new Vector3(50f, -50f, -50f), texCoord = new Vector3(l, -l, -l) };
            dst[2] = new SkyVertex { pos = new Vector3(-50f, -50f, 50f), texCoord = new Vector3(-l, -l, l) };
            dst[3] = new SkyVertex { pos = new Vector3(50f, -50f, 50f), texCoord = new Vector3(l, -l, l) };
            dst[4] = new SkyVertex { pos = new Vector3(-50f, 50f, -50f), texCoord = new Vector3(-l, l, -l) };
            dst[5] = new SkyVertex { pos = new Vector3(50f, 50f, -50f), texCoord = new Vector3(l, l, -l) };
            dst[6] = new SkyVertex { pos = new Vector3(-50f, 50f, 50f), texCoord = new Vector3(-l, l, l) };
            dst[7] = new SkyVertex { pos = new Vector3(50f, 50, 50f), texCoord = new Vector3(l, l, l) };

            box.Unlock();

            indexBuffer = factory.CreateIndexBuffer(IndexBufferType.Bit16, 36, BufferUsage.WriteOnly);

            ushort* ibDst = (ushort*)indexBuffer.Lock(0, 0, LockMode.None);

            ibDst[0] = 0; ibDst[1] = 1; ibDst[2] = 3;
            ibDst[3] = 0; ibDst[4] = 3; ibDst[5] = 2;

            ibDst[6] = 0; ibDst[7] = 4; ibDst[8] = 5;
            ibDst[9] = 0; ibDst[10] = 5; ibDst[11] = 1;

            ibDst[12] = 2; ibDst[13] = 6; ibDst[14] = 4;
            ibDst[15] = 2; ibDst[16] = 4; ibDst[17] = 0;

            ibDst[18] = 3; ibDst[19] = 7; ibDst[20] = 6;
            ibDst[21] = 3; ibDst[22] = 6; ibDst[23] = 2;

            ibDst[24] = 1; ibDst[25] = 5; ibDst[26] = 7;
            ibDst[27] = 1; ibDst[28] = 7; ibDst[29] = 3;

            ibDst[30] = 6; ibDst[31] = 7; ibDst[32] = 5;
            ibDst[33] = 6; ibDst[34] = 5; ibDst[35] = 4;

            indexBuffer.Unlock();

            skyEff = EffectManager.Instance.GetModelEffect(SkyboxEffectFactory.Name);

            opList = new RenderOperation[1];
            opList[0].Transformation = Matrix.Identity;
            opList[0].Material = new Material(renderSystem);
            opList[0].Material.ZEnabled = false;
            opList[0].Material.ZWriteEnabled = false;
            opList[0].Material.CullMode = CullMode.None;

            opList[0].Geomentry = new GeomentryData(this);
            opList[0].Geomentry.IndexBuffer = indexBuffer;
            opList[0].Geomentry.VertexBuffer = box;
            opList[0].Geomentry.VertexCount = 8;
            opList[0].Geomentry.VertexDeclaration = vtxDecl;
            opList[0].Geomentry.VertexSize = vtxDecl.GetVertexSize();
            opList[0].Geomentry.PrimitiveType = RenderPrimitiveType.TriangleList;
            opList[0].Geomentry.PrimCount = 12;

        }

        public float DayNightLerpParam
        {
            get;
            set;
        }


        public override void Update(GameTime time)
        {

        }

        public override RenderOperation[] GetRenderOperation()
        {
            return opList;
        }
        public override bool IsSerializable
        {
            get { return true; }
        }
        public override void Serialize(BinaryDataWriter data)
        {
            base.Serialize(data);
        }
        public override bool IntersectsSelectionRay(ref Ray ray)
        {
            return false;
        }


        /// <summary>
        ///  从ResourceLocation加载天空盒纹理
        /// </summary>
        /// <param name="dayTexture"></param>
        public void LoadTexture(FileLocation dayTexture)
        {
            Texture dayTex = TextureManager.Instance.CreateInstanceUnmanaged(dayTexture);
            opList[0].Material.SetTexture(0, new ResourceHandle<Texture>(dayTex));

            //if (dayTexture != null)
            //{
            //    if (dayTex != null)
            //    {
            //        dayTex.Dispose();
            //        dayTex = null;
            //    }
            //    this.dayTex = factory.CreateTexture(dayTexture, TextureUsage.Static);
            //}
            //if (nightTexture != null)
            //{
            //    if (nightTex != null)
            //    {
            //        nightTex.Dispose();
            //        nightTex = null;
            //    }
            //    this.nightTex = factory.CreateTexture(nightTexture, TextureUsage.Static);
            //}
        }

        //#region IConfigurable 成员

        //public void Parse(ConfigurationSection sect)
        //{
        //    string dayTexFile = sect.GetString("DayTexture", null);
        //    if (dayTexFile != null)
        //    {
        //        FileLocation fl = FileSystem.Instance.Locate(dayTexFile, FileLocateRules.Default);

        //        dayTex = factory.CreateTexture(fl, TextureUsage.Static);
        //    }

        //    string nightTexFile = sect.GetString("NightTexture", null);
        //    if (nightTexFile != null)
        //    {
        //        FileLocation fl = FileSystem.Instance.Locate(nightTexFile, FileLocateRules.Default);

        //        nightTex = factory.CreateTexture(fl, TextureUsage.Static);
        //    }
        //}

        //#endregion

        #region IDisposable 成员
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                opList[0].Material.Dispose();

                indexBuffer.Dispose();
                box.Dispose();
            }
        }


        #endregion
    }
}
