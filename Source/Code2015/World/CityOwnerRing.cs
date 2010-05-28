using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.ParticleSystem;

namespace Code2015.World
{
    class CityOwnerRing : IRenderable
    {
        public const int MaxDots = 5;

        public static readonly Matrix WhiteMatrix =
            new Matrix(1, 1, 1, 1,
                       1, 1, 1, 1,
                       1, 1, 1, 1,
                       1, 1, 1, 1);

        CityObject parent;

        CityStyle style;

        int flashDuration;

        bool visible;
        FastList<Vector4> colorBuffer = new FastList<Vector4>(4);

        FastList<RenderOperation> dotOpBuffer = new FastList<RenderOperation>();
        ResourceEffect[] resr = new ResourceEffect[MaxDots];
        ResourceEmitter[] emitr = new ResourceEmitter[MaxDots];
        ValueSmoother red = new ValueSmoother(5);

        ResourceEffect[] resg = new ResourceEffect[MaxDots];
        ResourceEmitter[] emitg = new ResourceEmitter[MaxDots];
        ValueSmoother green = new ValueSmoother(5);

        ResourceEffect[] resy = new ResourceEffect[MaxDots];
        ResourceEmitter[] emity = new ResourceEmitter[MaxDots];
        ValueSmoother yellow = new ValueSmoother(5);

        public void Flash(int duration)
        {
            flashDuration = duration;
        }
        public CityOwnerRing(RenderSystem rs, CityObject obj, CityStyle style)
        {
            this.parent = obj;
            this.style = style;

            for (int i = 0; i < MaxDots; i++)
            {
                resr[i] = new ResourceEffect(rs, TransferType.Oil);
                emitr[i] = new ResourceEmitter(obj.Position + obj.Transformation.Up * 70, obj.Transformation.Forward, obj.Transformation.Right,
                    (0.6f * Randomizer.GetRandomSingle() + 0.7f) * CityStyleTable.CityRadius);
                emitr[i].IsVisible = true;

                resr[i].Emitter = emitr[i];
                resr[i].Modifier = new ResourceModifier();


                resg[i] = new ResourceEffect(rs, TransferType.Wood);
                emitg[i] = new ResourceEmitter(obj.Position + obj.Transformation.Up * 70, obj.Transformation.Forward, obj.Transformation.Right,
                    (0.6f * Randomizer.GetRandomSingle() + 0.7f) * CityStyleTable.CityRadius);
                emitg[i].IsVisible = true;

                resg[i].Emitter = emitg[i];
                resg[i].Modifier = new ResourceModifier();


                resy[i] = new ResourceEffect(rs, TransferType.Food);
                emity[i] = new ResourceEmitter(obj.Position + obj.Transformation.Up * 70, obj.Transformation.Forward, obj.Transformation.Right,
                    (0.6f * Randomizer.GetRandomSingle() + 0.7f) * CityStyleTable.CityRadius);

                emity[i].IsVisible = true;

                resy[i].Emitter = emity[i];
                resy[i].Modifier = new ResourceModifier();
            }

        }

        public Vector4 GetWeights()
        {
            if (colorBuffer.Count > 0)
            {
                CaptureState capture = parent.Capture;

                Vector4 result;

                result.X = colorBuffer.Elements[0].W;
                result.Y = colorBuffer.Count > 1 ? colorBuffer.Elements[1].W : 0;
                result.Z = colorBuffer.Count > 2 ? colorBuffer.Elements[2].W : 0;
                result.W = colorBuffer.Count > 3 ? colorBuffer.Elements[3].W : 0;

                return result;
            }
            return new Vector4(1, 0, 0, 0);
        }


        public Matrix GetColorMatrix()
        {
            Matrix colors = WhiteMatrix;
            if (colorBuffer.Count > 0)
            {
                for (int i = 0; i < colorBuffer.Count; i++)
                {
                    Vector4 v = colorBuffer.Elements[i];
                    v.W = 1;
                    colors.SetRow(i, v);
                }
            }
            //if (parent.IsCaptured)
            //{
            //    if (flashDuration > 0)
            //    {
            //        float weight = 0.5f * ((float)Math.Sin(flashDuration * Math.PI * 0.1 - 0.5 * Math.PI) + 1);

            //        Color4F cc = new Color4F(parent.Owner.SideColor);
            //        Color4F modColor = new Color4F(1, weight + cc.Red, weight + cc.Green, weight + cc.Blue);

            //        colors.SetRow(0, modColor.ToVector4());
            //    }
            //    else
            //    {
            //        colors.SetRow(0, parent.Owner.SideColor.ToVector4());
            //    }
            //}
            //else
            //{
            //    if (parent.IsCapturing)
            //    {
            //        CaptureState capture = parent.Capture;

            //        Vector4 clr;
            //        if (capture.NewOwner1 != null)
            //        {
            //            clr = capture.NewOwner1.SideColor.ToVector4();
            //            colors.SetRow(0, clr);
            //        }
            //        if (capture.NewOwner2 != null)
            //        {
            //            clr = capture.NewOwner2.SideColor.ToVector4();
            //            colors.SetRow(1, clr);
            //        }
            //        if (capture.NewOwner3 != null)
            //        {
            //            clr = capture.NewOwner3.SideColor.ToVector4();
            //            colors.SetRow(2, clr);
            //        }

            //        if (capture.NewOwner4 != null)
            //        {
            //            clr = capture.NewOwner4.SideColor.ToVector4();
            //            colors.SetRow(3, clr);
            //        }
            //    }
            //}

            return colors;
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            colorBuffer.FastClear();

            if (!parent.IsCaptured)
            {
                if (parent.IsCapturing)
                {
                    CaptureState capture = parent.Capture;

                    Vector4 clr;
                    if (capture.NewOwner1 != null)
                    {
                        clr = capture.NewOwner1.SideColor.ToVector4();
                        clr.W = capture.CaputreProgress1;
                        colorBuffer.Add(ref clr);
                    }
                    if (capture.NewOwner2 != null)
                    {
                        clr = capture.NewOwner2.SideColor.ToVector4();
                        clr.W = capture.CaputreProgress2;
                        colorBuffer.Add(ref clr);
                    }
                    if (capture.NewOwner3 != null)
                    {
                        clr = capture.NewOwner3.SideColor.ToVector4();
                        clr.W = capture.CaputreProgress3;
                        colorBuffer.Add(ref clr);
                    }
                    if (capture.NewOwner4 != null)
                    {
                        clr = capture.NewOwner4.SideColor.ToVector4();
                        clr.W = capture.CaputreProgress4;
                        colorBuffer.Add(ref clr);
                    }
                }

                if (parent.City.IsRecovering)
                {
                    Vector4 clr = parent.City.CoolDownPlayer.SideColor.ToVector4();
                    clr.W = parent.City.RecoverCoolDown / CityGrade.GetRecoverCoolDown(parent.Size);
                    colorBuffer.Add(ref clr);
                }
            }
            else
            {
                if (flashDuration > 0)
                {
                    float weight = 0.5f * ((float)Math.Sin(flashDuration * Math.PI * 0.1 - 0.5 * Math.PI) + 1);

                    Color4F cc = new Color4F(parent.Owner.SideColor);
                    Vector4 modColor = new Vector4(1, weight + cc.Red, weight + cc.Green, weight + cc.Blue);

                    modColor.W = 1;
                    colorBuffer.Add(ref modColor);
                }
                else
                {
                    Vector4 v = parent.Owner.SideColor.ToVector4();
                    v.W = 1;
                    colorBuffer.Add(ref v);
                }
            }


            RenderOperation[] ops = style.Ring.GetRenderOperation();
            if (ops != null)
            {
                for (int i = 0; i < ops.Length; i++)
                {
                    ops[i].Sender = this;
                }
            }
            return ops;
        }

        public RenderOperation[] GetRenderOperation2()
        {
            dotOpBuffer.FastClear();

            if (parent.IsCaptured)
            {
                visible = true;

                for (int i = 0; i < MaxDots; i++)
                {
                    if (emitr[i].IsVisible)
                    {
                        RenderOperation[] ops = resr[i].GetRenderOperation();
                        if (ops != null)
                        {
                            dotOpBuffer.Add(ops);
                        }                        
                    }
                    else break;
                }
                for (int i = 0; i < MaxDots; i++)
                {
                    if (emitg[i].IsVisible)
                    {
                        RenderOperation[] ops = resg[i].GetRenderOperation();
                        if (ops != null)
                        {
                            dotOpBuffer.Add(ops);
                        }
                    }
                    else break;
                }
                for (int i = 0; i < MaxDots; i++)
                {
                    if (emity[i].IsVisible)
                    {
                        RenderOperation[] ops = resy[i].GetRenderOperation();
                        if (ops != null)
                        {
                            dotOpBuffer.Add(ops);
                        }
                    }
                    else break;
                }

                dotOpBuffer.TrimClear();
            }
            //if (emitter.IsVisible) 
            //{
            //    return restest.GetRenderOperation();
            //}
            return dotOpBuffer.Elements;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion

        public void Update(GameTime time)
        {
            flashDuration--;

            if (visible)
            {
                visible = false;

                City cc = parent.City;

                red.Add(cc.LocalHR.Current);
                green.Add(cc.LocalLR.Current);
                yellow.Add(cc.LocalFood.Current);

                int level = (int)(MaxDots * MathEx.Saturate(0.5f * red.Result / cc.LocalHR.StandardStorageBalance));

                for (int i = 0; i < level; i++)
                {
                    emitr[i].IsVisible = true;
                }
                for (int i = level; i < MaxDots; i++)
                {
                    emitr[i].IsVisible = false;
                }

                level = (int)(MaxDots * MathEx.Saturate(0.5f * green.Result / cc.LocalLR.StandardStorageBalance));

                for (int i = 0; i < level; i++)
                {
                    emitg[i].IsVisible = true;
                }
                for (int i = level; i < MaxDots; i++)
                {
                    emitg[i].IsVisible = false;
                }

                level = (int)(MaxDots * MathEx.Saturate(0.5f * yellow.Result / cc.LocalFood.StandardStorageBalance));

                for (int i = 0; i < level; i++)
                {
                    emity[i].IsVisible = true;
                }
                for (int i = level; i < MaxDots; i++)
                {
                    emity[i].IsVisible = false;
                }

                for (int i = 0; i < MaxDots; i++)
                {
                    resr[i].Update(time);
                    resg[i].Update(time);
                    resy[i].Update(time);
                }
            }
        }
    }
}
