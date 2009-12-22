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

        /// <summary>
        ///  计算地块弦长
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static float GetTileLength(float rad)
        {
            return (float)Math.Sin(rad) * PlanetRadius;
        }
        /// <summary>
        ///  计算地块角度
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static float GetTileArc(float len)
        {
            return (float)Math.Asin(len / PlanetRadius);
        }


        public static float GetTileWidth(float lat, float span)
        {
            float r = GetLatRadius(lat);
            return (float)Math.Sqrt(2 * r * r * (1 - (float)Math.Cos(span)));
        }
        public static float GetTileHeight(float span) 
        {
            return (float)Math.Sqrt(2 * PlanetRadius * PlanetRadius - 2 * PlanetRadius * PlanetRadius * (float)Math.Cos(span));
        }

        public static float GetLatRadius(float lat)
        {
            return PlanetRadius * (float)Math.Cos(lat);
        }
        public static Vector3 GetPosition(float x, float y)
        {
            // 微积分 球面参数方程
            Vector3 result;
            float py = (float)Math.Cos(y);
            float rr = PlanetRadius * py;

            result.Y = (float)Math.Sqrt(PlanetRadius * PlanetRadius - rr * rr);
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
        //public static Vector3 GetTangentX(float x, float y)
        //{
        //    Vector3 result = GetPosition(x + MathEx.PiOver2, y);
        //    result.Normalize();

        //    return result;
        //}

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
