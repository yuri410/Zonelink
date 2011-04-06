﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Zonelink.MathLib
{
    class MathEx
    {
        public const float Root2 = 1.141f;

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to apply.</param>
        /// <returns>The transformed <see cref="Vector4"/>.</returns>
        public static Vector3 Vec3TransformSimple(Vector3 vector, Quaternion rotation)
        {
            Vector3 result;
            float x = rotation.X + rotation.X;
            float y = rotation.Y + rotation.Y;
            float z = rotation.Z + rotation.Z;
            float wx = rotation.W * x;
            float wy = rotation.W * y;
            float wz = rotation.W * z;
            float xx = rotation.X * x;
            float xy = rotation.X * y;
            float xz = rotation.X * z;
            float yy = rotation.Y * y;
            float yz = rotation.Y * z;
            float zz = rotation.Z * z;

            result.X = ((vector.X * ((1.0f - yy) - zz)) + (vector.Y * (xy - wz))) + (vector.Z * (xz + wy));
            result.Y = ((vector.X * (xy + wz)) + (vector.Y * ((1.0f - xx) - zz))) + (vector.Z * (yz - wx));
            result.Z = ((vector.X * (xz - wy)) + (vector.Y * (yz + wx))) + (vector.Z * ((1.0f - xx) - yy));

            return result;
        }

        public static int Sqr(int p) { return p * p; }
        public static float Sqr(float p)
        {
            return p * p;
        }


        /// <summary>
        /// 线形插值
        /// </summary>
        /// <param name="f1">第一个值</param>
        /// <param name="f2">第二个值</param>
        /// <param name="amount">插值系数</param>
        /// <returns></returns>
        public static float LinearInterpose(float f1, float f2, float amount)
        {
            return (f2 - f1) * amount + f1;
        }

        public static Vector3 LinearInterpose(Vector3 f1, Vector3 f2, float amount)
        {
            return (f2 - f1) * amount + f1;
        }
    }
}