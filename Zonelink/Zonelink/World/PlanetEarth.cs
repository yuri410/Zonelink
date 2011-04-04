using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Zonelink.World
{
    class PlanetEarth
    {
        #region 工具
        /// <summary>
        ///  表示地球的半径
        /// </summary>
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
        /// <param name="len">弧长</param>
        /// <returns></returns>
        public static float GetTileArcAngle(float len)
        {
            return len / PlanetRadius;
        }

        /// <summary>
        ///  计算纬度方向的弦长
        /// </summary>
        /// <param name="lagt">纬度</param>
        /// <param name="span">经度圆心角</param>
        /// <returns></returns>
        public static float GetTileWidth(float lat, float span)
        {
            float r = GetLatRadius(lat);
            return (float)(2 * r * Math.Sin(span * 0.5));
            // (float)Math.Sqrt(2 * r * r * (1 - (float)Math.Cos(span)));
        }
        /// <summary>
        ///  计算经度方向的弦长
        /// </summary>
        /// <param name="span">纬度圆心角</param>
        /// <returns></returns>
        public static float GetTileHeight(float span)
        {
            return (float)(2 * PlanetRadius * Math.Sin(span * 0.5));
        }

        /// <summary>
        ///  计算纬度截面园的半径
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        public static float GetLatRadius(float lat)
        {
            return PlanetRadius * (float)Math.Cos(lat);
        }
        /// <summary>
        ///  计算弦上点的坐标
        /// </summary>
        /// <param name="x">经度</param>
        /// <param name="y">纬度</param>
        /// <param name="span">经度圆心角，弧度制</param>
        /// <returns></returns>
        public static Vector3 GetInnerPosition(float x, float y, float span)
        {
            Vector3 n = GetNormal(x, y);
            float ir = (float)(PlanetRadius * Math.Cos(0.5 * span));
            return n * ir;
        }

        ///// <summary>
        /////  角度为角度制
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="col"></param>
        ///// <param name="lat"></param>
        //public static void TileCoord2Coord(float x, float y, out float col, out float lat)
        //{
        //    col = x * 5 - 185;
        //    lat = 50 - y * 5;

        //}
        /// <summary>
        ///  角度为角度制
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="col"></param>
        /// <param name="lat"></param>
        public static void TileCoord2CoordNew(float x, float y, out float col, out float lat)
        {
            col = x * 5 - 185;
            lat = 95 - y * 5;

        }

        public static void GetCoord(Vector3 p, out float col, out float lat)
        {
            p.Normalize();

            lat = (float)Math.Asin(p.Y);

            col = (float)Math.Atan2(-p.Z, p.X);
        }

        public static Matrix GetOrientation(float x, float y)
        {
            Matrix Transformation = Matrix.Identity;

            Transformation.Up = PlanetEarth.GetNormal(x, y);
            Transformation.Right = PlanetEarth.GetTangentX(x, y);
            Transformation.Forward = -PlanetEarth.GetTangentY(x, y);
            return Transformation;
        }


        /// <summary>
        ///  计算球面上的点的坐标
        /// </summary>
        /// <param name="x">经度</param>
        /// <param name="y">纬度</param>
        /// <returns></returns>
        public static Vector3 GetPosition(float x, float y)
        {
            x = -x;
            // 微积分 球面参数方程
            Vector3 result;
            float py = (float)Math.Cos(y);
            float rr = PlanetRadius * py;

            result.Y = (float)Math.Sqrt(PlanetRadius * PlanetRadius - rr * rr);
            if (y < 0)
                result.Y = -result.Y;

            result.X = (float)Math.Cos(x) * rr;
            result.Z = (float)Math.Sin(x) * rr;


            if (float.IsNaN(result.Y))
                result.Y = 0;
            return result;
        }
        public static Vector3 GetPosition(float x, float y, float r)
        {
            x = -x;
            // 微积分 球面参数方程
            Vector3 result;
            float py = (float)Math.Cos(y);
            float rr = r * py;

            result.Y = (float)Math.Sqrt(r * r - rr * rr);
            if (y < 0)
                result.Y = -result.Y;

            result.X = (float)Math.Cos(x) * rr;
            result.Z = (float)Math.Sin(x) * rr;


            if (float.IsNaN(result.Y))
                result.Y = 0;
            return result;
        }
        /// <summary>
        ///  计算球面上一点的法向量
        /// </summary>
        /// <param name="x">经度</param>
        /// <param name="y">纬度</param>
        /// <returns></returns>
        public static Vector3 GetNormal(float x, float y)
        {
            Vector3 result = GetPosition(x, y);
            result.Normalize();
            return result;
        }
        /// <summary>
        ///  计算球面上一点沿经度方向的切向量
        /// </summary>
        /// <param name="x">经度</param>
        /// <param name="y">纬度</param>
        /// <returns></returns>
        public static Vector3 GetTangentY(float x, float y)
        {
            Vector3 result = GetPosition(x, y + MathHelper.PiOver2);
            result.Normalize();

            return result;
        }
        /// <summary>
        ///  计算球面上一点沿纬度方向的切向量
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector3 GetTangentX(float x, float y)
        {
            Vector3 up = GetNormal(x, y);
            Vector3 fwd = GetTangentY(x, y);

            Vector3 result = Vector3.Cross(up, fwd);
            result.Normalize();

            return result;
        }
        #endregion
    }
}
