using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.World
{
    class CityLink : Entity
    {
        const float LinkBaseLength = 100;
        const float LinkWidthScale = 0.00065f;
        const float LinkHeightScale = 4 * 1f / LinkBaseLength;

        CityObject start;
        CityObject end;
        Model link_e;

        public CityLink(RenderSystem renderSys, CityObject a, CityObject b, Model road)
            : base(false)
        {
            start = a;
            end = b;

            //FileLocation fl = FileSystem.Instance.Locate("track.mesh", GameFileLocs.Model);
            ModelL0 = road;// new Model(ModelManager.Instance.CreateInstance(renderSys, fl));

            //ModelL0.CurrentAnimation = new NoAnimation(Matrix.RotationY(MathEx.PiOver2) *
            //    Matrix.Scaling(dist / LinkBaseLength, 1 + LinkHeightScale, 1 + LinkWidthScale * dist));



            Vector3 dir = b.Position - a.Position;

            dir.Normalize();

            Vector3 pa = a.Position + dir * CityStyleTable.CityRadius[(int)a.Size];
            Vector3 pb = b.Position - dir * CityStyleTable.CityRadius[(int)b.Size];


            float dist = Vector3.Distance(pa, pb);

            float longitude = MathEx.Degree2Radian(0.5f * (a.Longitude + b.Longitude));
            float latitude = MathEx.Degree2Radian(0.5f * (a.Latitude + b.Latitude));

            Matrix ori = Matrix.Identity;
            ori.Right = Vector3.Normalize(pa - pb);
            ori.Up = PlanetEarth.GetNormal(longitude, latitude);
            ori.Forward = Vector3.Normalize(Vector3.Cross(ori.Up, ori.Right));
            ori.TranslationValue = 0.5f * (pa + pb);




            FileLocation fl = FileSystem.Instance.Locate("link_e.mesh", GameFileLocs.Model);
            link_e = new Model(ModelManager.Instance.CreateInstance(renderSys, fl));
            link_e.CurrentAnimation =
                new NoAnimation(Matrix.Scaling(dist / LinkBaseLength, 1 + LinkHeightScale, 1 + LinkWidthScale * dist) * ori);

            orientation = Matrix.Identity;
            position = Vector3.Zero;// 0.5f * (pa + pb);

            BoundingSphere.Center = 0.5f * (pa + pb);
            BoundingSphere.Radius = dist * 0.5f;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        public override RenderOperation[] GetRenderOperation()
        {
            opBuffer.FastClear();
            //return base.GetRenderOperation();
            RenderOperation[] ops = ModelL0.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }
            ops = link_e.GetRenderOperation();
            if (ops != null) 
            {
                opBuffer.Add(ops);
            }
            opBuffer.Trim();
            return opBuffer.Elements;
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }
    }
}
