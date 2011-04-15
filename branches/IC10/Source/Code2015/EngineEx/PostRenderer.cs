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
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Code2015.Effects;
using Apoc3D.Graphics.Effects;
using Code2015.World;

namespace Code2015.EngineEx
{
    /// <summary>
    ///  后期效果渲染器
    /// </summary>
    public class GamePostRenderer : UnmanagedResource, IPostSceneRenderer
    {
        //const float BloomThreshold = 0.3f;
        const float BlurAmount = 3.5f;

        //const float BloomIntensity = 1;
        //const float BaseIntensity = 1;

        //const float BloomSaturation = 1;
        //const float BaseSaturation = 1;

        const int SampleCount = 5;

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
        RtsCamera camera;
        GuassBlurFilter guassFilter;


        RenderTarget nrmDepthBuffer;
        RenderTarget edgeResultBuffer;

        RenderTarget colorBuffer;
        RenderTarget blurredRt1;
        RenderTarget blurredRt2;


        Bloom bloomEff;
        Composite compEff;
        EdgeDetect edgeEff;

        GaussBlur gaussBlur;


        VertexDeclaration vtxDecl;

        IndexBuffer indexBuffer;
        VertexBuffer quad;
        VertexBuffer smallQuad;
        ObjectFactory factory;

        GeomentryData quadOp;
        GeomentryData smallQuadOp;

        public GamePostRenderer(RenderSystem rs, RtsCamera camera)
        {
            this.factory = rs.ObjectFactory;

            this.renderSys = rs;
            this.camera = camera;

            bloomEff = new Bloom(rs);
            compEff = new Composite(rs);
            gaussBlur = new GaussBlur(rs);
            edgeEff = new EdgeDetect(rs);

            vtxDecl = factory.CreateVertexDeclaration(RectVertex.Elements);

            LoadUnmanagedResources();
        }


        void DrawBigQuad()
        {
            renderSys.RenderSimple(quadOp);
        }
        void DrawSmallQuad()
        {
            renderSys.RenderSimple(smallQuadOp);
        }

        /// <summary>
        ///  见接口
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="screenTarget"></param>
        public void RenderFullScene(ISceneRenderer renderer, RenderTarget screenTarget, RenderMode mode)
        {
            renderer.RenderScene(nrmDepthBuffer, RenderMode.DeferredNormal);

            

            renderer.RenderScene(colorBuffer, RenderMode.Final);

            Viewport vp = renderSys.Viewport;

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
            sampler2.MagFilter = TextureFilter.Linear;
            sampler2.MinFilter = TextureFilter.Linear;

            #region 边缘合成
            renderSys.SetRenderTarget(0, edgeResultBuffer);
            edgeEff.Begin();

            edgeEff.SetSamplerStateDirect(0, ref sampler1);
            edgeEff.SetSamplerStateDirect(1, ref sampler1);

            edgeEff.SetTextureDirect(0, nrmDepthBuffer.GetColorBufferTexture());
            edgeEff.SetTextureDirect(1, colorBuffer.GetColorBufferTexture());

            Vector2 nrmBufSize = new Vector2(vp.Width, vp.Height);
            edgeEff.SetValue("normalBufferSize", ref nrmBufSize);

            DrawBigQuad();
            edgeEff.End();
            #endregion

            #region 高斯X
            renderSys.SetRenderTarget(0, blurredRt1);

            gaussBlur.Begin();

            gaussBlur.SetSamplerStateDirect(0, ref sampler1);
            gaussBlur.SetTextureDirect(0, edgeResultBuffer.GetColorBufferTexture());

            for (int i = 0; i < SampleCount; i++)
            {
                gaussBlur.SetValueDirect(i, ref guassFilter.SampleOffsetsX[i]);
                gaussBlur.SetValueDirect(i + 15, guassFilter.SampleWeights[i]);
            }

            DrawSmallQuad();

            gaussBlur.End();
            #endregion

            #region 高斯Y

            renderSys.SetRenderTarget(0, blurredRt2);
            gaussBlur.Begin();

            gaussBlur.SetSamplerStateDirect(0, ref sampler1);
            gaussBlur.SetTextureDirect(0, blurredRt1.GetColorBufferTexture());

            for (int i = 0; i < SampleCount; i++)
            {
                gaussBlur.SetValueDirect(i, ref guassFilter.SampleOffsetsY[i]);
                gaussBlur.SetValueDirect(i + 15, guassFilter.SampleWeights[i]);
            }
            DrawSmallQuad();

            gaussBlur.End();


            #endregion


            #region DOF合成

            renderSys.SetRenderTarget(0, screenTarget);

            compEff.Begin();

            compEff.SetSamplerStateDirect(0, ref sampler1);
            compEff.SetSamplerStateDirect(1, ref sampler2);
            compEff.SetSamplerStateDirect(2, ref sampler2);

            compEff.SetTextureDirect(0, edgeResultBuffer.GetColorBufferTexture());
            compEff.SetTextureDirect(1, blurredRt2.GetColorBufferTexture());
            compEff.SetTextureDirect(2, nrmDepthBuffer.GetColorBufferTexture());

            if (camera != null)
            {
                float focNear = (camera.Position.Length() - PlanetEarth.PlanetRadius) / camera.FarPlane;
                compEff.SetValue("FocusNear", focNear);
            }
            else 
            {
                compEff.SetValue("FocusNear", 0.3f);
            }

            renderSys.RenderStates.AlphaBlendEnable = true;
            renderSys.RenderStates.SourceBlend = Blend.SourceAlpha;
            renderSys.RenderStates.DestinationBlend = Blend.InverseSourceAlpha;
            renderSys.RenderStates.BlendOperation = BlendFunction.Add;

            DrawBigQuad();

            compEff.End();
            #endregion
        }

        protected unsafe override void loadUnmanagedResources()
        {
            Viewport vp = renderSys.Viewport;

            Size blmSize = new Size(vp.Width / 2, vp.Height / 2);
            Size scrnSize = new Size(vp.Width, vp.Height);

            blurredRt1 = factory.CreateRenderTarget(blmSize.Width, blmSize.Height, ImagePixelFormat.A8R8G8B8);
            blurredRt2 = factory.CreateRenderTarget(blmSize.Width, blmSize.Height, ImagePixelFormat.A8R8G8B8);
            colorBuffer = factory.CreateRenderTarget(scrnSize.Width, scrnSize.Height, ImagePixelFormat.A8R8G8B8);
            nrmDepthBuffer = factory.CreateRenderTarget(scrnSize.Width, scrnSize.Height, ImagePixelFormat.A8R8G8B8);
            edgeResultBuffer = factory.CreateRenderTarget(scrnSize.Width, scrnSize.Height, ImagePixelFormat.A8R8G8B8);

            #region 计算参数

            guassFilter = new GuassBlurFilter(SampleCount, BlurAmount, blmSize.Width, blmSize.Height);
            #endregion


            #region 建立屏幕quad
            quad = factory.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);

            float adj = -0.5f;

            RectVertex* vdst = (RectVertex*)quad.Lock(0, 0, LockMode.None);
            vdst[0].Position = new Vector4(adj, adj, 0, 1);
            vdst[0].TexCoord = new Vector2(0, 0);
            vdst[1].Position = new Vector4(scrnSize.Width + adj, adj, 0, 1);
            vdst[1].TexCoord = new Vector2(1, 0);
            vdst[2].Position = new Vector4(adj, scrnSize.Height + adj, 0, 1);
            vdst[2].TexCoord = new Vector2(0, 1);
            vdst[3].Position = new Vector4(scrnSize.Width + adj, scrnSize.Height + adj, 0, 1);
            vdst[3].TexCoord = new Vector2(1, 1);
            quad.Unlock();
            #endregion

            #region 建立小quad

            smallQuad = factory.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);
            vdst = (RectVertex*)smallQuad.Lock(0, 0, LockMode.None);
            vdst[0].Position = new Vector4(adj, adj, 0, 1);
            vdst[0].TexCoord = new Vector2(0, 0);
            vdst[1].Position = new Vector4(blmSize.Width + adj, adj, 0, 1);
            vdst[1].TexCoord = new Vector2(1, 0);
            vdst[2].Position = new Vector4(adj, blmSize.Height + adj, 0, 1);
            vdst[2].TexCoord = new Vector2(0, 1);
            vdst[3].Position = new Vector4(blmSize.Width + adj, blmSize.Height + adj, 0, 1);
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
            colorBuffer.Dispose();
            blurredRt1.Dispose();
            nrmDepthBuffer.Dispose();
            edgeResultBuffer.Dispose();

            indexBuffer.Dispose();
            quad.Dispose();
            smallQuad.Dispose();

            quadOp = null;
            smallQuadOp = null;

            quad = null;
            smallQuad = null;
            indexBuffer = null;

            colorBuffer = null;
            blurredRt1 = null;
            //colorTarget = null;
            //bloom = null;
        }
    }
}
