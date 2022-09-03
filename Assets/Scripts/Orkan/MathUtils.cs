using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Orkan
{
    public static class MathUtils
    {
        public static float RotationSign(float a, float b)
        {
            a = Mod(a, 360.0f);
            b = Mod(b, 360.0f);
            return (a > b ? -1.0f : 1.0f) * (Math.Abs(b - a) > 180.0f ? -1.0f : 1.0f);
        }

        public static float Mod(this float a, float b) => a < 0 ? a % b + b : a % b;

        public static Vector2 UnitCircle(float angle1)
        {
            angle1 *= Deg2Rad;
            return new Vector2(
                Cos(angle1),
                Sin(angle1));
        }
        public static Vector3 UnitSphere(float angle1, float angle2)
        {
            angle1 *= Deg2Rad;
            angle2 *= Deg2Rad;
            return new Vector3(
                Cos(angle1) * Cos(angle1),
                Sin(angle1) * Cos(angle1),
                Sin(angle2));
        }
        public static Vector4 UnitHypersphere(float angle1, float angle2, float angle3)
        {
            angle1 *= Deg2Rad;
            angle2 *= Deg2Rad;
            angle3 *= Deg2Rad;
            return new Vector4(
                Cos(angle1) * Cos(angle1) * Cos(angle3),
                Sin(angle1) * Cos(angle1) * Cos(angle3),
                Sin(angle2) * Cos(angle3),
                Sin(angle3));
        }

    }
}
