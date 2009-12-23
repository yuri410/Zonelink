using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.MathLib;

namespace Code2015.World
{
    public class PlanetEarth : StaticModelObject
    {
        const int MaxInstance = 25;
        public const float PlanetRadius = 6371f;

        /// <summary>
        ///  计算地块弧长
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static float GetTileArcLength(float rad)
        {
            return rad * PlanetRadius;
        }
        /// <summary>
        ///  计算地块角度
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static float GetTileArcAngle(float len)
        {
            return len / PlanetRadius;
        }

        /// <summary>
        ///  计算弦长
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static float GetTileWidth(float lat, float span)
        {
            float r = GetLatRadius(lat);
            return (float)(2 * r * Math.Sin(span * 0.5));
            // (float)Math.Sqrt(2 * r * r * (1 - (float)Math.Cos(span)));
        }
        /// <summary>
        ///  计算弦长
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static float GetTileHeight(float span) 
        {
            return (float)(2 * PlanetRadius * Math.Sin(span * 0.5));
            // (float)Math.Sqrt(2 * PlanetRadius * PlanetRadius - 2 * PlanetRadius * PlanetRadius * (float)Math.Cos(span));
        }

        public static float GetLatRadius(float lat)
        {
            return PlanetRadius * (float)Math.Cos(lat);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="span">弧度制</param>
        /// <returns></returns>
        public static Vector3 GetInnerPosition(float x, float y, float span)
        {
            Vector3 n = GetNormal(x, y);
            float ir = (float)(PlanetRadius * Math.Cos(0.5 * span));
            return n * ir;
        }
        public static Vector3 GetPosition(float x, float y)
        {
            x = -x;
            // 微积分 球面参数方程
            Vector3 result;
            float py = (float)Math.Cos(y);
            float rr = PlanetRadius * py;

            result.Y = (float)Math.Sqrt(PlanetRadius * PlanetRadius - rr * rr);
            result.X = (float)Math.Cos(x) * rr;
            result.Z = (float)Math.Sin(x) * rr;


            if (float.IsNaN(result.Y))
                result.Y = 0;
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
            Vector3 up = GetNormal(x,y);
            Vector3 fwd = GetTangentY(x, y);

            Vector3 result = Vector3.Cross(up, fwd);            
            result.Normalize();

            return result;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
