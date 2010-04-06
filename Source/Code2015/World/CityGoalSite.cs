using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Scene;

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

        public CityGoalSite(CityObject obj, CityStyle style)
        {
            this.parent = obj;
            this.style = style;
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            throw new NotImplementedException();
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

            if (Size == UrbanSize.Large)
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

            if (Size == UrbanSize.Large)
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
