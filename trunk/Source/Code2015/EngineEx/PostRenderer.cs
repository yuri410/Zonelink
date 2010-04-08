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
                elements[1] = new VertexElement(1, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
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
            RenderStateManager states = renderSys.RenderStates;

            renderer.RenderScene(clrRt, RenderMode.Final);

            states.CullMode = CullMode.None;

            #region 分离高光
            renderSys.SetRenderTarget(0, blmRt);

            bloomEff.Begin();
            bloomEff.SetTexture("rt", clrRt.GetColorBufferTexture());

            DrawSmallQuad();

            bloomEff.End();
            #endregion

            #region 高斯X
            gaussXBlur.Begin();
            gaussXBlur.SetTexture("rt", blmRt.GetColorBufferTexture());

            DrawSmallQuad();

            gaussXBlur.End();
            #endregion

            #region 高斯Y
            gaussYBlur.Begin();
            gaussYBlur.SetTexture("rt", blmRt.GetColorBufferTexture());

            DrawSmallQuad();

            gaussYBlur.End();
            

            #endregion

            #region 合成


            renderSys.SetRenderTarget(0, screenTarget);


            //device.VertexShader = null;
            //device.PixelShader = null;

            //spr.Transform = Matrix.Identity;

            //spr.Begin(SpriteFlags.DoNotSaveState);
            //spr.Draw(colorTarget, -1);
            //spr.End();

            //states.AlphaBlendEnable = true;
            //states.BlendFunction = BlendFunction.Add;

            //states.DestinationBlend = Blend.One;
            //states.DestinationBlendAlpha = Blend.One;
            //states.SourceBlend = Blend.One;
            //states.SourceBlendAlpha = Blend.One;



            compEff.Begin();
            compEff.SetTexture("rt", clrRt.GetColorBufferTexture());
            compEff.SetTexture("blmRt", blmRt.GetColorBufferTexture());

            DrawBigQuad();

            compEff.End();


            #endregion

        }

        protected unsafe override void loadUnmanagedResources()
        {
            RenderTarget s = renderSys.GetRenderTarget(0);

            Size blmSize = new Size(512, 512);
            Size scrnSize = new Size(s.Width, s.Height);

            clrRt = factory.CreateRenderTarget(scrnSize.Width, scrnSize.Height, ImagePixelFormat.A8R8G8B8);
            blmRt = factory.CreateRenderTarget(blmSize.Width, blmSize.Width, ImagePixelFormat.A8R8G8B8);

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

            idst[0] = 0;
            idst[1] = 1;
            idst[2] = 3;
            idst[3] = 0;
            idst[4] = 2;
            idst[5] = 3;
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
