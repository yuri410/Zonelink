/*
-----------------------------------------------------------------------------
This source file is part of Apoc3D Engine

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
using Apoc3D.Graphics;
using Code2015.EngineEx;
using Code2015.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zonelink;
using System.IO;
using Zonelink.World;
using Zonelink.MathLib;
using Zonelink.Graphics;

namespace Apoc3D.Graphics
{
    public class GuassBlurFilter
    {
        public float BlurAmount;
        public int SampleCount;
        public float[] SampleWeights;
        public Vector2[] SampleOffsetsX;
        public Vector2[] SampleOffsetsY;


        public GuassBlurFilter(int sampleCount, float blurAmount, int mapWidth, int mapHeight)
        {
            this.SampleCount = sampleCount;
            this.BlurAmount = blurAmount;

            ComputeFilter(1 / (float)mapWidth, 0, out SampleWeights, out SampleOffsetsX);
            ComputeFilter(0, 1 / (float)mapHeight, out SampleWeights, out SampleOffsetsY);
        }


        void ComputeFilter(float dx, float dy, out float[] sampleWeights, out Vector2[] sampleOffsets)
        {
            // Create temporary arrays for computing our filter settings.
            sampleWeights = new float[SampleCount];
            sampleOffsets = new Vector2[SampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < SampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }
        }

        float ComputeGaussian(float n)
        {
            float theta = BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }

    }

    /// <summary>
    ///  定义 Shadow Mapping 所需要的阴影贴图
    /// </summary>
    public class ShadowMap : IDisposable
    {
        //struct RectVertex
        //{
        //    public Vector4 Position;

        //    public Vector2 TexCoord;

        //    static readonly VertexElement[] elements;

        //    static RectVertex()
        //    {
        //        elements = new VertexElement[2];
        //        elements[0] = new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position);
        //        elements[1] = new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0);
        //    }

        //    public static VertexElement[] Elements
        //    {
        //        get { return elements; }
        //    }


        //    public static int Size
        //    {
        //        get { return Vector4.SizeInBytes + Vector2.SizeInBytes; }
        //    }
        //}

        public const int ShadowMapLength = 512;

        GuassBlurFilter guassFilter;

        RenderTarget2D shadowRt;
        RenderTarget2D shadowRt2;
        //RenderTarget2D stdRenderTarget;
        Viewport stdVp;



        //VertexDeclaration vtxDecl;
        //IndexBuffer indexBuffer;
        //VertexBuffer smallQuad;
        //GeomentryData smallQuadOp;
//E:\Documents\ic11gd\Zonelink\ZonelinkContent\Effect\blurShd.fx
        Effect gaussBlur;
        public Matrix LightProjection;
        public Matrix ViewTransform;
        public Matrix ViewProj;

        Viewport smVp;
        Game1 renderSys;
        //ObjectFactory factory;

        public unsafe ShadowMap(Game1 dev)
        {
            this.renderSys = dev;
            //this.factory = dev.ObjectFactory;
            this.gaussBlur = dev.Content.Load<Effect>(Path.Combine(GameFileLocs.CEffect, "blurShd"));
            
            //this.vtxDecl = factory.CreateVertexDeclaration(RectVertex.Elements);

            loadUnmanagedResources();

            smVp.MinDepth = 0;
            smVp.MaxDepth = 1;
            smVp.Height = ShadowMapLength;
            smVp.Width = ShadowMapLength;
            smVp.X = 0;
            smVp.Y = 0;
        }



        public void End(SpriteBatch sprite)
        {
            GraphicsDevice device = renderSys.GraphicsDevice;
            #region 高斯X

            device.SetRenderTarget(shadowRt2);

            BlendState bs = new BlendState();
            
            sprite.Begin(SpriteSortMode.Immediate, bs, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, gaussBlur);
         
            gaussBlur.Parameters["tex"].SetValue(shadowRt);
            gaussBlur.Parameters["SampleOffsets"].SetValue(guassFilter.SampleOffsetsX);
            gaussBlur.Parameters["SampleWeights"].SetValue(guassFilter.SampleWeights);
            
            sprite.Draw(shadowRt, Vector2.Zero, Color.White);

            sprite.End();
            #endregion

            #region 高斯Y

            device.SetRenderTarget(shadowRt);

            sprite.Begin(SpriteSortMode.Immediate, bs, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, gaussBlur);
            
            
            gaussBlur.Parameters["tex"].SetValue(shadowRt2);
            gaussBlur.Parameters["SampleOffsets"].SetValue(guassFilter.SampleOffsetsY);
            gaussBlur.Parameters["SampleWeights"].SetValue(guassFilter.SampleWeights);

            sprite.Draw(shadowRt2, Vector2.Zero, Color.White);

            sprite.End();


            #endregion
            

            device.SetRenderTarget(null);

            device.Viewport = stdVp;

            
            //stdRenderTarget = null;
        }

        Matrix GetLightView(float longitude, float latitude, float h)
        {
            const float rotation = -MathHelper.Pi / 6f;
            const float yaw = -MathHelper.PiOver4;


            float p = h * 0.022f;

            Vector3 target = PlanetEarth.GetPosition(longitude - p * MathHelper.Pi / 90f, latitude);

            Vector3 axis = target;
            axis.Normalize();

            //float sign = latitude > 0 ? 1 : -1;
            Vector3 up = Vector3.UnitY;

            Vector3 rotAxis = axis;

            Vector3 yawAxis = Vector3.Cross(axis, up);
            yawAxis.Normalize();

            Quaternion rotTrans = Quaternion.CreateFromAxisAngle(rotAxis, rotation);
            axis = MathEx.Vec3TransformSimple(axis, Quaternion.CreateFromAxisAngle(yawAxis, yaw) * rotTrans);

            Vector3 position = target + axis * h * 35;
            return Matrix.CreateLookAt(position, target,
                MathEx.Vec3TransformSimple(up, rotTrans));

        }
      

        public void Begin(Vector3 lightDir, ICamera cam)
        {
            GraphicsDevice device = renderSys.GraphicsDevice;
            stdVp = device.Viewport;

            device.Viewport = smVp;


            device.SetRenderTarget(shadowRt);

            device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.White, 1, 0);

            float scale = cam.GetSMScale();

            Matrix.CreateOrthographic((float)ShadowMapLength * scale,
               (float)ShadowMapLength * scale,
               cam.NearPlane, cam.FarPlane, out LightProjection);

            //RtsCamera rtsCamera = (RtsCamera)cam;
            //float height = 2000;// cam.Position.Length() - PlanetEarth.PlanetRadius; // rtsCamera.Height;
            //float lng, lat;
            //PlanetEarth.GetCoord(cam.Position, out lng, out lat);

            //Matrix iv = Matrix.Invert (cam.ViewMatrix );
            ViewTransform = cam.GetSMTrans();
            // GetLightView(cam.Position, iv.Forward, iv.Up, iv.Right, cam.Position.Length() - PlanetEarth.PlanetRadius); // GetLightView(lng, lat, height);

            ViewProj = ViewTransform * LightProjection;
            EffectParameters.DepthViewProj = ViewProj;
        }

        public RenderTarget2D ShadowColorMap
        {
            get { return shadowRt; }
        }
        unsafe void loadUnmanagedResources()
        {
            shadowRt = new RenderTarget2D(renderSys.GraphicsDevice, ShadowMapLength, ShadowMapLength, false, SurfaceFormat.Rg32, DepthFormat.Depth24Stencil8);// G32R32F
            shadowRt2 = new RenderTarget2D(renderSys.GraphicsDevice, ShadowMapLength, ShadowMapLength, false, SurfaceFormat.Rg32, DepthFormat.Depth24Stencil8);
            
            guassFilter = new GuassBlurFilter(5, 2, ShadowMapLength, ShadowMapLength);

            gaussBlur.CurrentTechnique = gaussBlur.Techniques[0];
            //#region 建立小quad

            //float adj = -0.5f;
            //smallQuad = factory.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);
            //RectVertex* vdst = (RectVertex*)smallQuad.Lock(0, 0, LockMode.None);
            //vdst[0].Position = new Vector4(adj, adj, 0, 1);
            //vdst[0].TexCoord = new Vector2(0, 0);
            //vdst[1].Position = new Vector4(ShadowMapLength + adj, adj, 0, 1);
            //vdst[1].TexCoord = new Vector2(1, 0);
            //vdst[2].Position = new Vector4(adj, ShadowMapLength + adj, 0, 1);
            //vdst[2].TexCoord = new Vector2(0, 1);
            //vdst[3].Position = new Vector4(ShadowMapLength + adj, ShadowMapLength + adj, 0, 1);
            //vdst[3].TexCoord = new Vector2(1, 1);
            //smallQuad.Unlock();

            //#endregion

            //indexBuffer = factory.CreateIndexBuffer(IndexBufferType.Bit32, 6, BufferUsage.Static);
            //int* idst = (int*)indexBuffer.Lock(0, 0, LockMode.None);

            //idst[0] = 3;
            //idst[1] = 1;
            //idst[2] = 0;
            //idst[3] = 2;
            //idst[4] = 3;
            //idst[5] = 0;
            //indexBuffer.Unlock();

            //smallQuadOp = new GeomentryData();
            //smallQuadOp.BaseIndexStart = 0;
            //smallQuadOp.BaseVertex = 0;
            //smallQuadOp.IndexBuffer = indexBuffer;
            //smallQuadOp.PrimCount = 2;
            //smallQuadOp.PrimitiveType = RenderPrimitiveType.TriangleList;
            //smallQuadOp.VertexBuffer = smallQuad;
            //smallQuadOp.VertexCount = 4;
            //smallQuadOp.VertexDeclaration = vtxDecl;
            //smallQuadOp.VertexSize = RectVertex.Size;
        }

        protected void unloadUnmanagedResources()
        {
            shadowRt.Dispose();
            shadowRt2.Dispose();
        }

        public void Dispose()
        {
            //base.Dispose(disposing);
            unloadUnmanagedResources();
            
        }

    }
}
