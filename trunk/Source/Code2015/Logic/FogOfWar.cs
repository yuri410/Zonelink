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

namespace Code2015.Logic
{
    /// <summary>
    ///  Implements FogOfWar feature, dividing the battle field to a 2D array 
    ///  whose element means the visibility of each quad location on the planet.
    /// </summary>
    unsafe class FogOfWar
    {
        /// <summary>
        ///  A filter used to influence an area of visibility
        ///  Each location is weighted
        /// </summary>
        class Filter2DResult
        {
            public float[][] Filter;

            public Filter2DResult(int size, float fallOutInner)
            {
               
                Filter = new float[size][];
                for (int i = 0; i < size; i++)
                {
                    Filter[i] = new float[size];
                }

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        float r = (float)Math.Sqrt(i * i + j * j);
                        Filter[i][j] = 1 - MathEx.Saturate((r - fallOutInner) / (size - fallOutInner));
                    }
                }
            }

        }
        const int Width = 128;
        const int Height = 64;
        const int VisiblityCap = 255;
        const int StdTargetValue = 2000;

        /// <summary>
        ///  The amount of time needed to darken the cell
        /// </summary>
        int[][] cooldown;
        /// <summary>
        ///  the value that the cell should close to
        /// </summary>
        int[][] targetValue;
        /// <summary>
        ///  how well the cell is in sight
        /// </summary>
        int[][] visiblity;
        /// <summary>
        ///  the luminance result, can work in a range of 0-255
        /// </summary>
        int[][] result;
        Texture mask;
        Filter2DResult[] gaussFilter = new Filter2DResult[50];

        bool fogAllOut = false;

        /// <summary>
        ///  Gets the global luminance texture
        /// </summary>
        Texture FogMask
        { get { return mask; } }

        public FogOfWar()
        {

            //BoundingSphere.Radius = float.MaxValue;

            targetValue = new int[Height][];
            visiblity = new int[Height][];
            cooldown = new int[Height][];
            result = new int[Height][];
            for (int i = 0; i < Height; i++)
            {
                visiblity[i] = new int[Width];
                targetValue[i] = new int[Width];
                cooldown[i] = new int[Width];
                result[i] = new int[Width];
            }
            
        }
        public void InitailizeGraphics(RenderSystem rs)
        {

            mask = rs.ObjectFactory.CreateTexture(Width, Height, 1, TextureUsage.DynamicWriteOnly, ImagePixelFormat.Luminance8);

            //Material[][] mats = new Material[1][];
            //mats[0] = new Material[1];
            //mats[0][0] = new Material(rs);
            //mats[0][0].IsTransparent = true;
            //mats[0][0].CullMode = CullMode.CounterClockwise;
            //mats[0][0].ZEnabled = false;
            //mats[0][0].ZWriteEnabled = false;
            //mats[0][0].SetTexture(0, new ResourceHandle<Texture>(mask, true));
            //mats[0][0].PriorityHint = RenderPriority.Last;
            //mats[0][0].SetEffect(EffectManager.Instance.GetModelEffect(FogOfWarEffectFactory.Name));

            //oceanSphere = new Sphere(rs, PlanetEarth.PlanetRadius + 100,
            //    PlanetEarth.ColTileCount * 4, PlanetEarth.LatTileCount * 4, mats);

            //base.ModelL0 = oceanSphere;

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
        public void RevealAll()
        { 

        }
        /// <summary>
        ///  Gets the visibility at a given location
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public float GetVisibility(float lng, float lat)
        {
            int x, y;
            GetMapCoord(lng, lat, out x, out y);
            return result[y][x] / 255.0f;
        }
        /// <summary>
        ///  Light up an round area
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="r"></param>
        public void LightArea(float lng, float lat, float r)
        {
            int x, y;
            GetMapCoord(lng, lat, out x, out y);

            float w = (((r + MathEx.PIf) / (2 * MathEx.PIf)) * Width - ((0 + MathEx.PIf) / (2 * MathEx.PIf)) * Width);

            int bound = (int)Math.Floor(w);

            float[][] weights = null;
            int size = bound + 1;

            if (size < gaussFilter.Length)
            {
                if (gaussFilter[size] == null)
                    gaussFilter[size] = new Filter2DResult(size, size * 0.75f);// new GuassBlurFilter(size, 0.75f, 10, 10);
                weights = gaussFilter[size].Filter;
               

            }

            for (int i = -bound; i <= bound; i++)
            {
                for (int j = -bound; j <= bound; j++)
                {

                    targetValue[i + y][j + x] = Math.Min(targetValue[i + y][j + x] + (int)((weights[Math.Abs(i)][Math.Abs(j)]) * 0xff), 0xff);
                    cooldown[i + y][j + x] = StdTargetValue;
                   
                }
            }
        }

        /// <summary>
        ///  Update the visibility
        ///  
        /// <remarks>
        ///  Each cell has 4 int value, cooldown, targetValue, visiblity and result.
        ///  
        /// </remarks>
        /// </summary>
        /// <param name="dt"></param>
        public void Update(GameTime dt)
        {
            //base.Update(dt);

            TerrainEffect.FogMask = mask;

            int minX = 0;
            int maxX = 0;

            int minY = 0;
            int maxY = 0;
            bool gotRect = false;

            
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (result[i][j] > 0)
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

                    if (fogAllOut)
                    {
                        if (result[i][j]<0xff)
                            result[i][j] += 4;
                    }
                    else 
                    {
                        if (cooldown[i][j] > 0)
                        {
                            cooldown[i][j] -= 4;
                        }
                        else { cooldown[i][j] = 0; }
                        
                        int diff = targetValue[i][j] - visiblity[i][j];
                        if (diff > 4 || diff < -4)
                        {
                            visiblity[i][j] += Math.Sign(diff) * 4;
                        }
                        else
                        {
                            visiblity[i][j] = targetValue[i][j];
                            targetValue[i][j] = 0;
                        }

                        if (cooldown[i][j] > 255)
                        {
                            result[i][j] = visiblity[i][j];
                        }
                        else 
                        {
                            result[i][j] = visiblity[i][j] * cooldown[i][j] / 255;
                        }
                        
                    }
                    
                }
            }

            // upload to texture
            if (gotRect)
            { 
                Rectangle rect = new Rectangle (minX, minY, maxX - minX+1, maxY - minY + 1);
                DataRectangle dr =  mask.Lock(0, LockMode.None, rect);

                byte* dst = (byte*)dr.Pointer;
                for (int i = 0; i < rect.Height; i++)
                {
                    for (int j = 0; j < rect.Width; j++)
                    {
                        int newVal = result[i][j];
                        if (newVal > 0xff) newVal = 0xff;

                        dst[j] = (byte)newVal;
                    }
                    dst += dr.Pitch;
                }

                mask.Unlock(0);
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    if (disposing)
        //    {
        //        mask.Dispose();
        //    }
        //}
    }
}
