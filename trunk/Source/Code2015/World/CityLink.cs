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

        CityObject start;
        CityObject end;

        public CityLink(RenderSystem renderSys, CityObject a, CityObject b)
            : base(false)
        {
            start = a;
            end = b;

            FileLocation fl = FileSystem.Instance.Locate("link.mesh", GameFileLocs.Model);
            ModelL0 = new Model(ModelManager.Instance.CreateInstance(renderSys, fl));

            float dist = Vector3.Distance(a.Position, b.Position);
            ModelL0.CurrentAnimation = new NoAnimation(Matrix.Scaling(dist / LinkBaseLength, 1, 1));


            float longitude = MathEx.Degree2Radian(0.5f * (a.Longitude + b.Longitude));
            float latitude = MathEx.Degree2Radian(0.5f * (a.Latitude + b.Latitude));
            Orientation = PlanetEarth.GetOrientation(longitude, latitude);

            Position = PlanetEarth.GetPosition(longitude, latitude);// 0.5f * (start.Position + end.Position);

        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
