using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Scene;
using Code2015.World.Screen;
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;

namespace Code2015.World
{
    class CityGoalSite : IRenderable
    {
        const int SiteCount = 4;

        struct GoalSite
        {
            public bool HasPiece;
            public MdgType Type;
        }

        CityObject parent;
        CityStyle style;
        bool isRotating;
        float rotation;
        float actuallRotation;

        GoalSite[] sites = new GoalSite[SiteCount];

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        public CityGoalSite(CityObject obj, CityStyle style)
        {
            this.parent = obj;
            this.style = style;

            
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            opBuffer.TrimClear();
            for (int i = 0; i < SiteCount; i++)
            {
                if (sites[i].HasPiece)
                {
                    RenderOperation[] ops = style.MdgBracket[(int)sites[i].Type].GetRenderOperation();

                    opBuffer.Add(ops);
                }
            }
            return opBuffer.Elements;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion



        public unsafe void Rotate(int span)
        {
            GoalSite* newSites = stackalloc GoalSite[SiteCount];

            for (int i = 0; i < SiteCount; i++)
            {
                int n = (i + span) % SiteCount;

                newSites[n] = sites[i];
            }

            for (int i = 0; i < SiteCount; i++)
            {
                sites[i] = newSites[i];
            }
        }
        public void BeginRotate()
        {
            isRotating = true;
        }


        public void Rotating(float amount)
        {
            const float GlueThreshold = 0.1f;

            rotation = amount;

            if (parent.Size == UrbanSize.Large)
            {
                float s = Math.Sign(rotation);
                float rem = Math.Abs(rotation) % MathEx.PiOver4;

                if (rem < GlueThreshold)
                {
                    actuallRotation = rotation - s * rem;
                }
            }
            else
            {
                float s = Math.Sign(rotation);
                float rem = Math.Abs(rotation) % MathEx.PiOver2;

                if (rem < GlueThreshold)
                {
                    actuallRotation = rotation - s * rem;
                }
            }
        }
        public void EndRotate()
        {
            isRotating = false;

            if (parent.Size == UrbanSize.Large)
            {
                const float Pi8 = MathEx.PiOver4 * 0.5f;

                int sp = (int)(actuallRotation / Pi8);
                Rotate(sp);
            }
            else
            {
                const float Pi4 = MathEx.PiOver4;

                int sp = (int)(actuallRotation / Pi4);
                Rotate(sp);
            }
        }

    }
}
