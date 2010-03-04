using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.BalanceSystem;

namespace Code2015.World
{
    class CityLink : Entity
    {
        const float LinkBaseLength = 100;
        const float LinkWidthScale = 0.00065f;
        const float LinkHeightScale = 4 * 1f / LinkBaseLength;

        CityObject start;
        CityObject end;

        public CityLink(RenderSystem renderSys, CityObject a, CityObject b)
            : base(false)
        {
            start = a;
            end = b;

            FileLocation fl = FileSystem.Instance.Locate("track.mesh", GameFileLocs.Model);
            ModelL0 = new Model(ModelManager.Instance.CreateInstance(renderSys, fl));

            float dist = Vector3.Distance(a.Position, b.Position);

            ModelL0.CurrentAnimation = new NoAnimation(Matrix.RotationY (MathEx.PiOver2) *
                Matrix.Scaling(dist / LinkBaseLength, 1 + LinkHeightScale, 1 + LinkWidthScale * dist));
           

            float longitude = MathEx.Degree2Radian(0.5f * (a.Longitude + b.Longitude));
            float latitude = MathEx.Degree2Radian(0.5f * (a.Latitude + b.Latitude));

            Matrix ori = Matrix.Identity;
            ori.Right = Vector3.Normalize(a.Position - b.Position);
            ori.Up = PlanetEarth.GetNormal(longitude, latitude);
            ori.Forward = Vector3.Normalize(Vector3.Cross(ori.Up, ori.Right));

            Orientation = ori;

            Position = 0.5f * (a.Position + b.Position);
            
            BoundingSphere.Center = position;
            BoundingSphere.Radius = dist * 0.5f;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
