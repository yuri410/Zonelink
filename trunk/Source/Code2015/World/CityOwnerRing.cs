using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.Logic;

namespace Code2015.World
{
    class CityOwnerRing : IRenderable
    {

        public static readonly Matrix WhiteMatrix = 
            new Matrix(1, 1, 1, 1,
                       1, 1, 1, 1, 
                       1, 1, 1, 1,
                       1, 1, 1, 1);

        CityObject parent;

        CityStyle style;

        int flashDuration;

        FastList<Vector4> colorBuffer = new FastList<Vector4>(4);


        public void Flash(int duration)
        {
            flashDuration = duration;
        }
        public CityOwnerRing(CityObject obj, CityStyle style)
        {
            this.parent = obj;
            this.style = style;
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
            return new Vector4(1,0,0,0);
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


            RenderOperation[] ops = style.Ring[(int)parent.Size].GetRenderOperation();
            for (int i = 0; i < ops.Length; i++)
            {
                ops[i].Sender = this;
            }
            return ops;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion

        public void Update(GameTime time)
        {
            flashDuration--;

        }
    }
}
