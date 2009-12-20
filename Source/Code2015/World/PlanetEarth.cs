using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.MathLib;

namespace Code2015.World
{
    class PlanetEarth : StaticModelObject
    {
        const int MaxInstance = 25;
        public const float PlanetRadius = 6371f;

        public static float GetTileLegth(float rad)
        {
            return (float)Math.Sin(rad) * PlanetRadius;
        }
        public static float GetTileArc(float len)
        {
            return (float)Math.Asin(len / PlanetRadius);
        }



        public static Vector3 GetPosition(float x, float y)
        {
            // 微积分 球面参数方程
            Vector3 result;
            float py = (float)Math.Cos(y);
            float rr = PlanetRadius * py;

            result.Y = (float)Math.Sqrt(PlanetRadius * PlanetRadius - py * PlanetRadius);
            result.X = (float)Math.Cos(x) * rr;
            result.Z = (float)Math.Sin(x) * rr;

            return result;
        }
        public static Vector3 GetNormal(float x, float y)
        {
            Vector3 result = GetPosition(x, y);
            result.Normalize();
            return result;
        }
        public static Vector3 GetTangentY(float x, float y)
        {
            Vector3 result = GetPosition(x, y + MathEx.PiOver2);
            result.Normalize();

            return result;
        }
        public static Vector3 GetTangentX(float x, float y)
        {
            Vector3 result = GetPosition(x + MathEx.PiOver2, y);
            result.Normalize();

            return result;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
