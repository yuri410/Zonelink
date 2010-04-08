using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Code2015.Effects;

namespace Code2015.EngineEx
{
    /// <summary>
    ///  后期效果渲染器
    /// </summary>
    public class BloomPostRenderer : UnmanagedResource, IPostSceneRenderer
    {
        struct RectVertex
        {
            public Vector4 Position;

            public Vector2 TexCoord;

            static readonly VertexElement[] elements;

            static RectVertex()
            {
                elements = new VertexElement[2];
                elements[0] = new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.PositionTransformed);
                elements[1] = new VertexElement(Vector4.SizeInBytes, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
            }

            public static VertexElement[] Elements
            {
                get { return elements; }
            }


            public static int Size
            {
                get { return Vector4.SizeInBytes + Vector2.SizeInBytes; }
            }
        }

        RenderSystem renderSys;

        RenderTarget clrRt;
        RenderTarget blmRt;

        Bloom bloomEff;
        Composite compEff;

        GaussBlurX gaussXBlur;
        GaussBlurY gaussYBlur;


        VertexDeclaration vtxDecl;

        IndexBuffer indexBuffer;
        VertexBuffer quad;
        VertexBuffer smallQuad;
        ObjectFactory factory;

        GeomentryData quadOp;
        GeomentryData smallQuadOp;
        //Sprite spr;

        //Effect LoadEffect(string fileName) 
        //{
        //    FileLocation fl = FileSystem.Instance.Locate(FileSystem.CombinePath(Paths.Effects, fileName), FileLocateRules.Default);
        //    ContentStreamReader sr = new ContentStreamReader(fl);
        //    string code = sr.ReadToEnd();
        //    string err;
        //    Effect effect = Effect.FromString(device, code, null, IncludeHandler.Instance, null, ShaderFlags.OptimizationLevel3, null, out err);
        //    sr.Close();

        //    return effect;
        //}

        public BloomPostRenderer(RenderSystem rs)
        {
            this.factory = rs.ObjectFactory;

            this.renderSys = rs;

            bloomEff = new Bloom(rs);
            compEff = new Composite(rs);
            gaussXBlur = new GaussBlurX(rs);
            gaussYBlur = new GaussBlurY(rs);

            vtxDecl = factory.CreateVertexDeclaration(RectVertex.Elements);

            LoadUnmanagedResources();
        }

        void DrawBigQuad()
        {
            //renderSys.SetStreamSource(0, quad, 0, RectVertex.Size);
            //renderSys.VertexFormat = RectVertex.Format;
            //renderSys.Indices = indexBuffer;
            //renderSys.VertexDeclaration = vtxDecl;

            //renderSys.DrawIndexedPrimitives(RenderPrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            renderSys.RenderSimple(quadOp);
        }
        void DrawSmallQuad()
        {
            //renderSys.SetStreamSource(0, smallQuad, 0, RectVertex.Size);
            //renderSys.VertexFormat = RectVertex.Format;
            //renderSys.VertexDeclaration = vtxDecl;
            //renderSys.Indices = indexBuffer;
            //renderSys.DrawIndexedPrimitives(RenderPrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            renderSys.RenderSimple(smallQuadOp);
        }

        /// <summary>
        ///  见接口
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="screenTarget"></param>
        public void RenderFullScene(ISceneRenderer renderer, RenderTarget screenTarget, RenderMode mode)
        {
            renderer.RenderScene(clrRt, RenderMode.Final);

            ShaderSamplerState sampler1;
            sampler1.AddressU = TextureAddressMode.Clamp;
            sampler1.AddressV = TextureAddressMode.Clamp;
            sampler1.AddressW = TextureAddressMode.Clamp;
            sampler1.BorderColor = ColorValue.Transparent;
            sampler1.MagFilter = TextureFilter.Point;
            sampler1.MaxAnisotropy = 0;
            sampler1.MaxMipLevel = 0;
            sampler1.MinFilter = TextureFilter.Point;
            sampler1.MipFilter = TextureFilter.None;
            sampler1.MipMapLODBias = 0;

            ShaderSamplerState sampler2 = sampler1;
            sampler2.AddressU = TextureAddressMode.Clamp;
            sampler2.AddressV = TextureAddressMode.Clamp;
            sampler2.AddressW = TextureAddressMode.Clamp;
            sampler2.BorderColor = ColorValue.Transparent;
            sampler2.MagFilter = TextureFilter.Point;
            sampler2.MaxAnisotropy = 0;
            sampler2.MaxMipLevel = 0;
            sampler2.MinFilter = TextureFilter.Point;
            sampler2.MipFilter = TextureFilter.None;
            sampler2.MipMapLODBias = 0;

            renderSys.SetRenderTarget(0, null);
            #region 分离高光
            renderSys.SetRenderTarget(0, blmRt);

            bloomEff.Begin();
            bloomEff.SetTexture("tex", clrRt.GetColorBufferTexture());

            DrawSmallQuad();

            bloomEff.End();
            #endregion

            renderSys.SetRenderTarget(0, null);
            #region 高斯X
            gaussXBlur.Begin();
            gaussXBlur.SetTexture("tex", blmRt.GetColorBufferTexture());

            DrawSmallQuad();

            gaussXBlur.End();
            #endregion
            renderSys.SetRenderTarget(0, null);
            #region 高斯Y
            gaussYBlur.Begin();
            gaussYBlur.SetTexture("tex", blmRt.GetColorBufferTexture());

            DrawSmallQuad();

            gaussYBlur.End();


            #endregion



            renderSys.SetRenderTarget(0, null);
            #region 合成


            renderSys.SetRenderTarget(0, screenTarget);

            compEff.Begin();
            compEff.SetSamplerStateDirect(0, ref sampler1);
            compEff.SetSamplerStateDirect(1, ref sampler2);

            compEff.SetTextureDirect(0, clrRt.GetColorBufferTexture());
            compEff.SetTextureDirect(1, blmRt.GetColorBufferTexture());

            DrawBigQuad();

            compEff.End();
            #endregion
        }

        protected unsafe override void loadUnmanagedResources()
        {
            Viewport vp = renderSys.Viewport;

            Size blmSize = new Size(512, 512);
            Size scrnSize = new Size(vp.Width, vp.Height);


            blmRt = factory.CreateRenderTarget(blmSize.Width, blmSize.Width, ImagePixelFormat.A8R8G8B8);
            clrRt = factory.CreateRenderTarget(scrnSize.Width, scrnSize.Height, ImagePixelFormat.A8R8G8B8);

            #region 建立屏幕quad
            quad = factory.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);

            RectVertex* vdst = (RectVertex*)quad.Lock(0, 0, LockMode.None);
            vdst[0].Position = new Vector4(0, 0, 0, 1);
            vdst[0].TexCoord = new Vector2(0, 0);
            vdst[1].Position = new Vector4(scrnSize.Width, 0, 0, 1);
            vdst[1].TexCoord = new Vector2(1, 0);
            vdst[2].Position = new Vector4(0, scrnSize.Height, 0, 1);
            vdst[2].TexCoord = new Vector2(0, 1);
            vdst[3].Position = new Vector4(scrnSize.Width, scrnSize.Height, 0, 1);
            vdst[3].TexCoord = new Vector2(1, 1);
            quad.Unlock();
            #endregion

            #region 建立小quad

            smallQuad = factory.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);
            vdst = (RectVertex*)smallQuad.Lock(0, 0, LockMode.None);
            vdst[0].Position = new Vector4(0, 0, 0, 1);
            vdst[0].TexCoord = new Vector2(0, 0);
            vdst[1].Position = new Vector4(blmSize.Width, 0, 0, 1);
            vdst[1].TexCoord = new Vector2(1, 0);
            vdst[2].Position = new Vector4(0, blmSize.Height, 0, 1);
            vdst[2].TexCoord = new Vector2(0, 1);
            vdst[3].Position = new Vector4(blmSize.Width, blmSize.Height, 0, 1);
            vdst[3].TexCoord = new Vector2(1, 1);
            smallQuad.Unlock();

            #endregion

            indexBuffer = factory.CreateIndexBuffer(IndexBufferType.Bit32, 6, BufferUsage.Static);
            int* idst = (int*)indexBuffer.Lock(0, 0, LockMode.None);

            idst[0] = 3;
            idst[1] = 1;
            idst[2] = 0;
            idst[3] = 2;
            idst[4] = 3;
            idst[5] = 0;
            indexBuffer.Unlock();

            quadOp = new GeomentryData();
            quadOp.BaseIndexStart = 0;
            quadOp.BaseVertex = 0;
            quadOp.IndexBuffer = indexBuffer;
            quadOp.PrimCount = 2;
            quadOp.PrimitiveType = RenderPrimitiveType.TriangleList;
            quadOp.VertexBuffer = quad;
            quadOp.VertexCount = 4;
            quadOp.VertexDeclaration = vtxDecl;
            quadOp.VertexSize = RectVertex.Size;

            smallQuadOp = new GeomentryData();
            smallQuadOp.BaseIndexStart = 0;
            smallQuadOp.BaseVertex = 0;
            smallQuadOp.IndexBuffer = indexBuffer;
            smallQuadOp.PrimCount = 2;
            smallQuadOp.PrimitiveType = RenderPrimitiveType.TriangleList;
            smallQuadOp.VertexBuffer = smallQuad;
            smallQuadOp.VertexCount = 4;
            smallQuadOp.VertexDeclaration = vtxDecl;
            smallQuadOp.VertexSize = RectVertex.Size;
        }


        protected override void unloadUnmanagedResources()
        {
            clrRt.Dispose();
            blmRt.Dispose();

            indexBuffer.Dispose();
            quad.Dispose();
            smallQuad.Dispose();

            quadOp = null;
            smallQuadOp = null;

            quad = null;
            smallQuad = null;
            indexBuffer = null;

            clrRt = null;
            blmRt = null;
            //colorTarget = null;
            //bloom = null;
        }
    }
}
