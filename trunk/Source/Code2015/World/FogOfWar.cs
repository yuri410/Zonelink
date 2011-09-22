using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics.Geometry;
using Code2015.EngineEx;
using Code2015.Effects;
using Apoc3D.Scene;
using Apoc3D;
using Apoc3D.Media;
using Apoc3D.Core;
using Apoc3D.MathLib;
using Apoc3D.Collections;

namespace Code2015.World
{
    unsafe class FogOfWar : StaticModelObject
    {
        const int Width = 128;
        const int Height = 64;
        const int VisiblityCap = 255;
        const int StdTargetValue = 2000;

        Sphere oceanSphere;

        int[][] targetValue;
        int[][] visiblity;
        Texture mask;

        Texture FogMask
        { get { return mask; } }

        public FogOfWar()
        {

            BoundingSphere.Radius = float.MaxValue;

            targetValue = new int[Height][];
            visiblity = new int[Height][];
            for (int i = 0; i < Height; i++)
            {
                visiblity[i] = new int[Width];
                targetValue[i] = new int[Width];
            }
            
        }
        public void InitailizeGraphics(RenderSystem rs)
        {

            mask = rs.ObjectFactory.CreateTexture(Width, Height, 1, TextureUsage.DynamicWriteOnly, ImagePixelFormat.Luminance8);

            Material[][] mats = new Material[1][];
            mats[0] = new Material[1];
            mats[0][0] = new Material(rs);
            mats[0][0].IsTransparent = true;
            mats[0][0].CullMode = CullMode.CounterClockwise;
            mats[0][0].ZEnabled = false;
            mats[0][0].ZWriteEnabled = false;
            mats[0][0].SetTexture(0, new ResourceHandle<Texture>(mask, true));
            mats[0][0].PriorityHint = RenderPriority.Last;
            mats[0][0].SetEffect(EffectManager.Instance.GetModelEffect(FogOfWarEffectFactory.Name));

            oceanSphere = new Sphere(rs, PlanetEarth.PlanetRadius + 100,
                PlanetEarth.ColTileCount * 4, PlanetEarth.LatTileCount * 4, mats);

            base.ModelL0 = oceanSphere;

        }
        public override bool IsSerializable
        {
            get { return false; }
        }

        public static void GetMapCoord(float lng, float lat, out int x, out int y)
        {
            float yspan = MathEx.PIf;

            y = (int)(((yspan * 0.5f - lat) / yspan) * Height);
            x = (int)(((lng + MathEx.PIf) / (2 * MathEx.PIf)) * Width);

            if (y < 0) y += Height;
            if (y >= Height) y -= Height;

            if (x < 0) x += Width;
            if (x >= Width) x -= Width;
        }

        public void LightArea(float lng, float lat, float r)
        {
            int x, y;
            GetMapCoord(lng, lat, out x, out y);

            float w = (((r + MathEx.PIf) / (2 * MathEx.PIf)) * Width - ((0 + MathEx.PIf) / (2 * MathEx.PIf)) * Width);

            int bound = (int)Math.Floor(w);

            for (int i = -bound; i < bound; i++)
            {
                for (int j = -bound; j < bound; j++)
                {
                    float rr = (float)Math.Sqrt(MathEx.Sqr(i) + MathEx.Sqr(j));

                    if (rr <= w*w)
                    {
                        targetValue[i + y][j + x] = StdTargetValue;
                    }
                }
            }
        }


        public override void Update(GameTime dt)
        {
            base.Update(dt);



            int minX = 0;
            int maxX = 0;

            int minY = 0;
            int maxY = 0;
            bool gotRect = false;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (visiblity[i][j] > 0)
                    {
                        if (i < minY)
                            minY = i;
                        if (j < minX)
                            minX = j;

                        if (i > maxY)
                            maxY = i;
                        if (j > maxX)
                            maxX = j;

                        gotRect = true;
                    }

                    int diff = targetValue[i][j] - visiblity[i][j];
                    if (Math.Abs(diff) > 4)
                    {
                        visiblity[i][j] += Math.Sign(diff) * 4;
                    }
                    else 
                    {
                        targetValue[i][j] = 0;
                    }

                }
            }

            if (gotRect)
            { 
                Rectangle rect = new Rectangle (minX, minY, maxX - minX+1, maxY - minY + 1);
                DataRectangle dr =  mask.Lock(0, LockMode.None, rect);

                byte* dst = (byte*)dr.Pointer;
                for (int i = 0; i < rect.Height; i++)
                {
                    for (int j = 0; j < rect.Width; j++)
                    {
                        int newVal = visiblity[i][j];
                        if (newVal > 0xff) newVal = 0xff;

                        dst[j] = (byte)newVal;
                    }
                    dst += dr.Pitch;
                }

                mask.Unlock(0);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                mask.Dispose();
            }
        }
    }
}
