using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    class CityLinkableMark : IRenderable
    {
        CityObject start;
        CityObject[] targets;

        Model[] linkArrow;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        public void SetCity(CityObject start, CityObject[] targets)
        {
            this.start = start;
            this.targets = targets;


            for (int i = 0; i < targets.Length; i++)
            {
                CityObject end = targets[i];

                Vector3 dir = end.Position - start.Position;

                dir.Normalize();

                Vector3 pa = start.Position + dir * CityStyleTable.CityRadius;
                Vector3 pb = end.Position - dir * CityStyleTable.CityRadius;


                float dist = Vector3.Distance(pa, pb);

                float longitude = MathEx.Degree2Radian(0.5f * (start.Longitude + end.Longitude));
                float latitude = MathEx.Degree2Radian(0.5f * (start.Latitude + end.Latitude));

                Matrix ori = Matrix.Identity;
                ori.Right = Vector3.Normalize(pa - pb);
                ori.Up = PlanetEarth.GetNormal(longitude, latitude);
                ori.Forward = Vector3.Normalize(Vector3.Cross(ori.Up, ori.Right));
                ori.TranslationValue = 0.5f * (pa + pb);


                linkArrow[i].CurrentAnimation =
                   new NoAnimation(Matrix.Scaling(dist / CityLinkObject.LinkBaseLength, 1 + CityLinkObject.LinkHeightScale, 1 + CityLinkObject.LinkWidthScale * dist) * ori);

            }
        }

        public CityLinkableMark(RenderSystem rs, CityObject start, CityObject[] targets)
        {
            this.linkArrow = new Model[4];

            FileLocation fl = FileSystem.Instance.Locate("link_e.mesh", GameFileLocs.Model);
            for (int i = 0; i < linkArrow.Length; i++)
            {
                linkArrow[i] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            }

            SetCity(start, targets);
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            opBuffer.FastClear();
            for (int i = 0; i < targets.Length; i++)
            {
                RenderOperation[] ops = linkArrow[i].GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }
            }

            opBuffer.Trim();
            return opBuffer.Elements;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion

        public void Update(GameTime time)
        {

        }
    }
   
}
