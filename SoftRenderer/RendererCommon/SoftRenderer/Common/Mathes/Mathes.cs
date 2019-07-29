﻿// jave.lin 2019.07.14

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SoftRenderer.Common.Mathes
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("2D向量")]
    public struct Vector2 : IEquatable<Vector2>
    {
        private static readonly Random ran = new Random();
        public static readonly Vector2 zero = new Vector2(0, 0);
        public static readonly Vector2 nan = new Vector2(float.NaN, float.NaN);
        //https://blog.csdn.net/linjf520/article/details/78718520
        public static bool OnLeft(Vector2 refV, Vector2 refL)
        {
            return refV.Cross(refL) < 0;
        }
        /*
         * https://blog.csdn.net/linjf520/article/details/78718520
        PxQ=px * qy - py * qx
        */
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
        public static bool AnyNan(Vector2 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y);
        }
        public static Vector2 Ran(float s)
        {
            var result = new Vector2(ran.Next(-360, 360), ran.Next(-360, 360));
            result.Normalize();
            return result * s;
        }
        public float x, y;
        public float length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
        }
        public Vector2 normalized
        {
            get
            {
                var result = this;
                result.Normalize();
                return result;
            }
        }
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
        }
        public Vector2(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
        }
        public float Dot(Vector2 other)
        {
            return x * other.x + y * other.y;
        }
        public void Normalize()
        {
            var invLen = 1 / length;
            x *= invLen;
            y *= invLen;
        }
        public float Cross(Vector2 other)
        {
            return Cross(this, other);
        }
        public bool OnLeft(Vector2 other)
        {
            return OnLeft(this, other);
        }
        public bool AnyNaN()
        {
            return AnyNan(this);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }
        public static Vector2 operator *(Vector2 a, float s)
        {
            return new Vector2(a.x * s, a.y * s);
        }
        public static Vector2 operator *(float s, Vector2 a)
        {
            return new Vector2(a.x * s, a.y * s);
        }
        public static Vector2 operator /(Vector2 a, float s)
        {
            return new Vector2(a.x / s, a.y / s);
        }
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.x != b.x || a.y != b.y;
        }
        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y, 0);
        }
        public static implicit operator Vector2(Vector4 v)
        {
            return new Vector2(v.x, v.y);
        }
        public bool Equals(Vector2 other)
        {
            return x == other.x && y == other.y;
        }
        public override string ToString()
        {
            return $"x:{x}, y:{y}";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("3D向量")]
    public struct Vector3 : IEquatable<Vector3>
    {
        public static readonly Vector3 nan          = new Vector3(float.NaN, float.NaN, float.NaN);
        public static readonly Vector3 zero         = new Vector3(0, 0, 0);
        public static readonly Vector3 one          = new Vector3(1, 1, 1);
        public static readonly Vector3 right        = new Vector3(1, 0, 0);
        public static readonly Vector3 up           = new Vector3(0, 1, 0);
        public static readonly Vector3 forward      = new Vector3(0, 0, 1);
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3 operator *(Vector3 a, float s)
        {
            return new Vector3(a.x * s, a.y * s, a.z * s);
        }
        public static Vector3 operator *(float s, Vector3 a)
        {
            return new Vector3(a.x * s, a.y * s, a.z * s);
        }
        public static Vector3 operator /(Vector3 a, float s)
        {
            return new Vector3(a.x / s, a.y / s, a.z / s);
        }
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.y;
        }
        public static bool operator ==(Vector3 a, Vector4 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(Vector3 a, Vector4 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.y;
        }
        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
        public static implicit operator Vector3(Vector4 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
        public static bool AnyNaN(Vector3 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
        }
        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.Dot(b);
        }
        public static Vector3 Reflect(Vector3 i, Vector3 n)
        {
            return i - 2 * n.Dot(i) * n;
        }
        public float x, y, z;
        public float length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }
        public Vector3 normalized
        {
            get
            {
                var result = this;
                result.Normalize();
                return result;
            }
        }
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }
        public Vector3(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = 0;
        }
        public bool AnyNaN()
        {
            return AnyNaN(this);
        }
        public float Dot(Vector3 other)
        {
            return x * other.x + y * other.y + z * other.z;
        }
        public void Normalize()
        {
            var invLen = 1 / length;
            x *= invLen;
            y *= invLen;
            z *= invLen;
        }
        /*
        U x V = <
        (Ux,Uy,Uz) x (Vx, Vy, Vz)=
        Uy * Vz - Uz * Vy ,
        Uz * Vx - Ux * Vz ,
        Ux * Vy - Uy * Vx>
        */
        public Vector3 Cross(Vector3 a)
        {
            return new Vector3(
               y * a.z - z * a.y,
               z * a.x - x * a.z,
               x * a.y - y * a.x
                );
        }

        public bool Equals(Vector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }
        public override string ToString()
        {
            return $"x:{x}, y:{y}, z:{z}";
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("4D向量")]
    public struct Vector4 : IEquatable<Vector4>
    {
        public static readonly Vector4 one = new Vector4(1, 1, 1, 1);
        public static readonly Vector4 zero = new Vector4(0, 0, 0, 0);
        public static readonly Vector4 zeroPos = new Vector4(0, 0, 0, 1);

        public float x, y, z, w;
        public float length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
        }
        public Vector4 normalized
        {
            get
            {
                var result = this;
                result.Normalize();
                return result;
            }
        }
        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public Vector4(Vector4 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }
        public Vector4(Vector2 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = 0;
            this.w = 1;
        }
        public Vector4(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = 1;
        }
        public float Dot(Vector4 other)
        {
            return x * other.x + y * other.y + z * other.z;
        }
        public void Normalize()
        {
            var invLen = 1 / length;
            x *= invLen;
            y *= invLen;
            z *= invLen;
        }
        /*
        U x V = <
        (Ux,Uy,Uz) x (Vx, Vy, Vz)=
        Uy * Vz - Uz * Vy ,
        Uz * Vx - Ux * Vz ,
        Ux * Vy - Uy * Vx>
        */
        public Vector4 Cross(Vector4 a)
        {
            return new Vector4(
               y * a.z - z * a.y,
               z * a.x - x * a.z,
               x * a.y - y * a.x,
               w
                );
        }
        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w);
        }
        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w);
        }
        public static Vector4 operator *(Vector4 a, float s)
        {
            return new Vector4(a.x * s, a.y * s, a.z * s, a.w);
        }
        public static Vector4 operator *(float s, Vector4 a)
        {
            return new Vector4(a.x * s, a.y * s, a.z * s, a.w);
        }
        public static Vector4 operator +(Vector4 a, float s)
        {
            return new Vector4(a.x + s, a.y + s, a.z + s, a.w);
        }
        public static Vector4 operator /(Vector4 a, float s)
        {
            var inv = 1f / s;
            return new Vector4(a.x * inv, a.y * inv, a.z * inv, 1);
        }
        public static bool operator ==(Vector4 a, Vector4 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
        }
        public static bool operator !=(Vector4 a, Vector4 b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.y || a.w != b.w;
        }
        public static implicit operator Vector4(Vector3 v)
        {
            return new Vector4(v);
        }
        public static implicit operator Vector4(Vector2 v)
        {
            return new Vector4(v);
        }
        public static implicit operator Vector4(ColorNormalized c)
        {
            return new Vector4(c.r, c.g, c.b, c.a);
        }
        public bool Equals(Vector4 other)
        {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }
        public override string ToString()
        {
            return $"x:{x}, y:{y}, z:{z}, w:{w}";
        }
    }

    [Description("数学库")]
    public class Mathf
    {
        public static Vector2 Min(Vector2 v1, Vector2 v2)
        {
            return new Vector2(
                Min(v1.x, v2.x),
                Min(v1.y, v2.y)
                );
        }
        public static Vector3 Min(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                Min(v1.x, v2.x),
                Min(v1.y, v2.y),
                Min(v1.z, v2.z)
                );
        }
        public static Vector4 Min(Vector4 v1, Vector4 v2)
        {
            return new Vector4(
                Min(v1.x, v2.x),
                Min(v1.y, v2.y),
                Min(v1.z, v2.z),
                Min(v1.w, v2.w)
                );
        }
        public static Vector2 Max(Vector2 v1, Vector2 v2)
        {
            return new Vector2(
                Max(v1.x, v2.x),
                Max(v1.y, v2.y)
                );
        }
        public static Vector3 Max(Vector3 v1, Vector3 v2)
        {
            return new Vector3(
                Max(v1.x, v2.x),
                Max(v1.y, v2.y),
                Max(v1.z, v2.z)
                );
        }
        public static Vector4 Max(Vector4 v1, Vector4 v2)
        {
            return new Vector4(
                Max(v1.x, v2.x),
                Max(v1.y, v2.y),
                Max(v1.z, v2.z),
                Max(v1.w, v2.w)
                );
        }
        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }
        public static float Clamp(float v, float min, float max)
        {
            if (v < min) v = min;
            if (v > max) v = max;
            return v;
        }
        public static int Clamp(int v, int min, int max)
        {
            if (v < min) v = min;
            if (v > max) v = max;
            return v;
        }
        public static Vector2 Clamp(Vector2 v, float min, float max)
        {
            return new Vector2(
                Clamp(v.x, min, max),
                Clamp(v.y, min, max)
                );
        }
        public static Vector3 Clamp(Vector3 v, float min, float max)
        {
            return new Vector3(
                Clamp(v.x, min, max),
                Clamp(v.y, min, max),
                Clamp(v.z, min, max)
                );
        }
        public static Vector4 Clamp(Vector4 v, float min, float max)
        {
            return new Vector4(
                Clamp(v.x, min, max),
                Clamp(v.y, min, max),
                Clamp(v.z, min, max),
                Clamp(v.w, min, max)
                );
        }
        public static ColorNormalized Clamp(ColorNormalized c, float min = 0, float max = 1)
        {
            c.r = Clamp(c.r, min, max);
            c.g = Clamp(c.g, min, max);
            c.b = Clamp(c.b, min, max);
            c.a = Clamp(c.a, min, max);
            return c;
        }
        public static int Lerp(int from, int to, float t) => (int)((1 - t) * from + t * to);
        public static float Lerp(float from, float to, float t) => (1 - t) * from + t * to;
        public static Vector2 Lerp(Vector2 from, Vector2 to, float t) =>  from * (1 - t) + to * t;
        public static Vector3 Lerp(Vector3 from, Vector3 to, float t) =>  from * (1 - t) + to * t;
        public static Vector4 Lerp(Vector4 from, Vector4 to, float t) => from * (1 - t) + to * t;
        public static ColorNormalized Lerp(ColorNormalized from, ColorNormalized to, float t) => from * (1 - t) + to * t;
        public static Matrix4x4 Lerp(Matrix4x4 from, Matrix4x4 to, float t) => from * (1 - t) + to * t;

        public static int Lerp(int from, int to, float t, float tt) => (int)(tt * from + t * to);
        public static float Lerp(float from, float to, float t, float tt) => tt * from + t * to;
        public static Vector2 Lerp(Vector2 from, Vector2 to, float t, float tt) => from * tt + to * t;
        public static Vector3 Lerp(Vector3 from, Vector3 to, float t, float tt) => from * tt + to * t;
        public static Vector4 Lerp(Vector4 from, Vector4 to, float t, float tt) => from * tt + to * t;
        public static ColorNormalized Lerp(ColorNormalized from, ColorNormalized to, float t, float tt) => from * tt + to * t;
        public static Matrix4x4 Lerp(Matrix4x4 from, Matrix4x4 to, float t, float tt) => from * tt + to * t;
    }

    // jave.lin 2019.07.17
    // 下面我们将使用OpenGL的主列矩阵
    // http://www.songho.ca/opengl/gl_matrix.html
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("4x4矩阵")]
    public struct Matrix4x4
    {
        public const float DEG2RAD = (float)Math.PI / 180f;

        //public static readonly Matrix4x4 IDENTITY_MAT = new Matrix4x4(
        //    1, 0, 0, 0,
        //    0, 1, 0, 0,
        //    0, 0, 1, 0,
        //    0, 0, 0, 1
        //    );
        public static Matrix4x4 GetMat(bool identity = true)
        {
            var result = new Matrix4x4();
            result.m = new float[16];
            if (identity) result.Identity();
            return result;
        }

        public static Vector3 MulVec(Matrix4x4 mat, Vector3 v)
        {
            return mat.MulVec(v);
        }
        public static Vector3 MulPos(Matrix4x4 mat, Vector3 v)
        {
            return mat.MulPos(v);
        }
        public static Vector4 MulPos4(Matrix4x4 mat, Vector4 v)
        {
            return mat.MulPos(v);
        }
        public static Vector4 MulPos(Matrix4x4 mat, Vector4 v)
        {
            return mat.MulPos(v);
        }
        // right是被乘，被left变换
        public static Matrix4x4 MulMat(Matrix4x4 left, Matrix4x4 right)
        {
            return left.MulMat(right);
        }
        public static Matrix4x4 GenScaleMat(float sx, float sy, float sz)
        {
            return new Matrix4x4(
            sx, 0, 0, 0,
            0, sy, 0, 0,
            0, 0, sz, 0,
            0, 0, 0, 1
            );
        }
        public static Matrix4x4 GenXRotateMat(float degree)
        {
            var radian = DEG2RAD * degree;
            var c = (float)Math.Cos(radian);
            var s = (float)Math.Sin(radian);
            return new Matrix4x4(
            1, 0, 0, 0,
            0, c, -s, 0,
            0, s, c, 0,
            0, 0, 0, 1
            );
        }
        public static Matrix4x4 GenYRotateMat(float degree)
        {
            var radian = DEG2RAD * degree;
            var c = (float)Math.Cos(radian);
            var s = (float)Math.Sin(radian);
            return new Matrix4x4(
            c, 0, s, 0,
            0, 1, 0, 0,
            -s, 0, c, 0,
            0, 0, 0, 1
            );
        }
        public static Matrix4x4 GenZRotateMat(float degree)
        {
            var radian = DEG2RAD * degree;
            var c = (float)Math.Cos(radian);
            var s = (float)Math.Sin(radian);
            return new Matrix4x4(
            c, -s, 0, 0,
            s, c, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
            );
        }
        public static Matrix4x4 GenRotateAxisMat(float degree, float ax, float ay, float az)
        {
            var radian = DEG2RAD * degree;
            var c = (float)Math.Cos(radian);
            var s = (float)Math.Sin(radian);
            var omc = 1 - c;

            var m00 = ax * ax * omc + c;
            var m10 = ax * ay * omc + az * s;
            var m20 = ax * az * omc - ay * s;

            var m01 = ax * ay * omc - az * s;
            var m11 = ay * ay * omc + c;
            var m21 = ay * az * omc + ax * s;

            var m02 = ax * az * omc + ay * s;
            var m12 = ay * az * omc - ax * s;
            var m22 = az * az * omc + c;

            return new Matrix4x4(
            m00, m01, m02, 0,
            m10, m11, m12, 0,
            m20, m21, m22, 0,
            0, 0, 0, 1
            );
        }
        public static Matrix4x4 GenEulerMat(float dx, float dy, float dz)
        {
            // Roll ➔ Yaw ➔ Pitch XYZ = X * Y * Z
            var mat = GenZRotateMat(dz);
            mat = Matrix4x4.MulMat(GenYRotateMat(dy), mat);
            return Matrix4x4.MulMat(GenXRotateMat(dx), mat);
        }
        public static Matrix4x4 GenTranslateMat(float tx, float ty, float tz)
        {
            return new Matrix4x4(
            1, 0, 0, tx,
            0, 1, 0, ty,
            0, 0, 1, tz,
            0, 0, 0, 1
            );
        }
        public static Matrix4x4 GenTRS(Vector3 translate, Vector3 euler, Vector3 scale)
        {
            //var mat = GenScaleMat(scale.x, scale.y, scale.z);
            //mat = Matrix4x4.MulMat(GenEulerMat(euler.x, euler.y, euler.z), mat);
            //return Matrix4x4.MulMat(GenTranslateMat(translate.x, translate.y, translate.z), mat);
            var mat = GenEulerMat(euler.x, euler.y, euler.z);
            mat = Matrix4x4.MulMat(GenScaleMat(scale.x, scale.y, scale.z), mat);
            return Matrix4x4.MulMat(GenTranslateMat(translate.x, translate.y, translate.z), mat);
        }
        /*
     http://www.songho.ca/opengl/gl_matrix.html#example2
     * */
        public static Matrix4x4 GenFrustum(
            float l, float r,
            float b, float t,
            float n, float f)
        {
            Matrix4x4 mat = new Matrix4x4();
            mat.m = new float[16];
            mat[0] = 2 * n / (r - l);
            mat[5] = 2 * n / (t - b);
            mat[8] = (r + l) / (r - l);
            mat[9] = (t + b) / (t - b);
            mat[10] = -(f + n) / (f - n);
            mat[11] = -1;
            mat[14] = -(2 * f * n) / (f - n);
            mat[15] = 0;
            return mat;
        }
        public static Matrix4x4 GenFrustum(
            float fovY, float aspect, float front, float back)
        {
            float tangent = (float)Math.Tan(fovY / 2 * DEG2RAD); // tangent of half fovY
            float height = front * tangent;         // half height of near plane
            float width = height * aspect;          // half width of near plane

            // params: left, right, bottom, top, near, far
            return GenFrustum(-width, width, -height, height, front, back);
        }
        public static Matrix4x4 GenOrthoFrustum(
            float l, float r, float b, float t, float n, float f)
        {
            Matrix4x4 mat = new Matrix4x4();
            mat.m = new float[16];
            mat[0] = 2 / (r - l);
            mat[5] = 2 / (t - b);
            mat[10] = -2 / (f - n);
            mat[12] = -(r + l) / (r - l);
            mat[13] = -(t + b) / (t - b);
            mat[14] = -(f + n) / (f - n);
            return mat;
        }
        public static Matrix4x4 GenOrthoFrustum(
            float aspect, float size, float n, float f)
        {
            var b = -size;
            var t = size;
            var w = aspect * size;
            var l = -w;
            var r = w;
            return GenOrthoFrustum(l, r, b, t, n, f);
        }
        public static Matrix4x4 operator +(Matrix4x4 left, Matrix4x4 right)
        {
            for (int i = 0; i < right.m.Length; i++) right.m[i] += left.m[i];
            return right;
        }
        public static Matrix4x4 operator -(Matrix4x4 left, Matrix4x4 right)
        {
            for (int i = 0; i < right.m.Length; i++) right.m[i] -= left.m[i];
            return right;
        }
        public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
        {
            return MulMat(left, right);
        }
        public static Matrix4x4 operator +(Matrix4x4 a, float v)
        {
            for (int i = 0; i < a.m.Length; i++) a.m[i] += v;
            return a;
        }
        public static Matrix4x4 operator -(Matrix4x4 a, float v)
        {
            for (int i = 0; i < a.m.Length; i++) a.m[i] -= v;
            return a;
        }
        public static Matrix4x4 operator *(Matrix4x4 a, float v)
        {
            for (int i = 0; i < a.m.Length; i++) a.m[i] *= v;
            return a;
        }
        public static Matrix4x4 operator /(Matrix4x4 a, float v)
        {
            var inv = 1 / v;
            for (int i = 0; i < a.m.Length; i++) a.m[i] *= inv;
            return a;
        }
        public static Vector3 operator *(Matrix4x4 left, Vector3 v)
        {
            return MulVec(left, v);
        }
        public static Vector4 operator *(Matrix4x4 left, Vector4 pos)
        {
            return MulPos(left, pos);
        }
        /*
         * http://www.songho.ca/opengl/gl_camera.html#lookat
         * These 3 basis vectors, left vector, 
         * up vector and forward vector are used to construct the rotation matrix MR of lookAt, 
         * however, the rotation matrix must be inverted. 
         */
        public static Matrix4x4 GenLookAt(Vector3 eye, Vector3 target, Vector3 upDir)
        {
            // compute the forward vector from target to eye
            Vector3 forward = eye - target;
            forward.Normalize();                 // make unit length

            // compute the left vector
            Vector3 left = upDir.Cross(forward); // cross product
            left.Normalize();

            // recompute the orthonormal up vector
            Vector3 up = forward.Cross(left);    // cross product

            // init 4x4 matrix
            Matrix4x4 matrix = new Matrix4x4();
            matrix.m = new float[16];
            matrix.Identity();

            // set rotation part, inverse rotation matrix: M^-1 = M^T for Euclidean transform
            matrix[0] = left.x;
            matrix[4] = left.y;
            matrix[8] = left.z;
            matrix[1] = up.x;
            matrix[5] = up.y;
            matrix[9] = up.z;
            matrix[2] = forward.x;
            matrix[6] = forward.y;
            matrix[10] = forward.z;

            // set translation part
            matrix[12] = -left.x * eye.x - left.y * eye.y - left.z * eye.z;
            matrix[13] = -up.x * eye.x - up.y * eye.y - up.z * eye.z;
            matrix[14] = -forward.x * eye.x - forward.y * eye.y - forward.z * eye.z;

            //matrix[15] = 1;

            return matrix;
        }

        // 后面可将这儿优化成指针方式
        public float[] m;
        public float this[int index] { get => m[index]; set => m[index] = value; }
        /*
        OpenGL
        Column Major:
        |m00|m01|m02|m03|   |m0 |m4 |m8 |m12|   |Xx|Yx|Zx|Tx|
        |m10|m11|m12|m13| = |m1 |m5 |m9 |m13| = |Xy|Yy|Zy|Ty|
        |m20|m21|m22|m23|   |m2 |m6 |m10|m14|   |Xz|Yz|Zz|Tz|
        |m30|m31|m32|m33|   |m3 |m7 |m11|m15|   |Xw|Yw|Zw|w |

            以下都使用：右乘法则，就是被乘数放右边的意思

            Mat * Vec
            Mat行 * Vec列
            |Xx|Yx|Zx|                 
            |Xy|Yy|Zy|* |Vx|Vy|Vz|= |Xx*Vx+Yx*Vy+Zx*Vz|Xy*Vx+Yy*Vy+Zy*Vz|Xz*Vx+Yz*Vy+Zz*Vz|
            |Xz|Yz|Zz|   

            Mat * Pos
            Mat行 * Pos列
            |Xx|Yx|Zx|Tx|                 
            |Xy|Yy|Zy|Ty|* |Px|Py|Pz|1 |= |Xx*Px+Yx*Py+Zx*Pz+Tx*Pw|Xy*Px+Yy*Py+Zy*Pz+Ty*Pw|Xz*Px+Yz*Py+Zz*Pz+Tz*Pw|Xw*Px+Yw*Py+Zw*Pz+w*1|
            |Xz|Yz|Zz|Tz| 
            |Xw|Yw|Zw|w |

            Mat1 * Mat2
            Mat1行 * Mat2列
            |m00|m01|m02|m03|   |n00|n01|n02|n03|   |m00*n00+m01*n10+m02*n20+m03*n30|m00*n01+m01*n11+m02*n21+m03*n31|m00*n02+m01*n12+m02*n22+m03*n32|m00*n03+m01*n13+m02*n23+m03*n33|
            |m10|m11|m12|m13| x |n10|n11|n12|n13| = |m10*n00+m11*n10+m12*n20+m13*n30|m10*n01+m11*n11+m12*n21+m13*n31|m10*n02+m11*n12+m12*n22+m13*n32|m10*n03+m11*n13+m12*n23+m13*n33|
            |m20|m21|m22|m23|   |n20|n21|n22|n23|   |m20*n00+m21*n10+m22*n20+m23*n30|m20*n01+m21*n11+m22*n21+m23*n31|m20*n02+m21*n12+m22*n22+m23*n32|m20*n03+m21*n13+m22*n23+m23*n33|
            |m30|m31|m32|m33|   |n30|n31|n32|n33|   |m30*n00+m31*n10+m32*n20+m33*n30|m30*n01+m31*n11+m32*n21+m33*n31|m30*n02+m31*n12+m32*n22+m33*n32|m30*n03+m31*n13+m32*n23+m33*n33|

        Dx
        Row Major:
        |m00|m01|m02|m03|   |m0 |m1 |m2 |m3 |   |Xx|Xy|Xz|Xw|
        |m10|m11|m12|m13| = |m4 |m5 |m6 |m7 | = |Yx|Yy|Yz|Yw|
        |m20|m21|m22|m23|   |m8 |m9 |m10|m11|   |Zx|Zy|Zz|Zw|
        |m30|m31|m32|m33|   |m12|m13|m14|m15|   |Tx|Ty|Tz|w |

        Row Major没写过，不知道，大概和OpenGL差不多吧，可能就左乘右乘的区别吧？

        * */
        //public Matrix4x4()
        //{
        //}
        public Matrix4x4(float[] m)
        {
            this.m = new float[16];
            var len = m.Length;
            for (int i = 0; i < len; i++)
            {
                this.m[i] = m[i];
            }
        }
        public Matrix4x4(Vector4 col1, Vector4 col2, Vector4 col3, Vector4 col4)
        {
            this.m = new float[16];
            m[0]    = col1.x;
            m[1]    = col1.y;
            m[2]    = col1.z;
            m[3]    = col1.w;

            m[4]    = col2.x;
            m[5]    = col2.y;
            m[6]    = col2.z;
            m[7]    = col2.w;

            m[8]    = col3.x;
            m[9]    = col3.y;
            m[10]   = col3.z;
            m[11]   = col3.w;

            m[12]   = col4.x;
            m[13]   = col4.y;
            m[14]   = col4.z;
            m[15]   = col4.w;
        }
        public Matrix4x4(Matrix4x4 mat)
        {
            this.m = new float[16];
            var len = m.Length;
            for (int i = 0; i < len; i++)
            {
                this.m[i] = mat.m[i];
            }
        }
        public Matrix4x4(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            m = new float[16];
            m[0] = m00; m[4] = m01; m[8] = m02; m[12] = m03;
            m[1] = m10; m[5] = m11; m[9] = m12; m[13] = m13;
            m[2] = m20; m[6] = m21; m[10] = m22; m[14] = m23;
            m[3] = m30; m[7] = m31; m[11] = m32; m[15] = m33;
        }
        public void CopyTo(float[] m)
        {
            var len = m.Length;
            for (int i = 0; i < len; i++)
            {
                m[i] = this.m[i];
            }
        }
        public void CopyTo(Matrix4x4 mat)
        {
            var m = mat.m;
            var len = m.Length;
            for (int i = 0; i < len; i++)
            {
                m[i] = this.m[i];
            }
        }
        public void CopyFrom(float[] m)
        {
            var len = m.Length;
            for (int i = 0; i < len; i++)
            {
                this.m[i] = m[i];
            }
        }
        public void CopyFrom(Matrix4x4 mat)
        {
            var m = mat.m;
            var len = m.Length;
            for (int i = 0; i < len; i++)
            {
                this.m[i] = m[i];
            }
        }
        public Vector4 GetCol(int idx)
        {
            idx = idx * 4;

            return new Vector4(
                m[idx],
                m[idx + 1],
                m[idx + 2],
                m[idx + 3]
            );
        }
        public void SetCol(int idx, Vector4 col)
        {
            //if (idx < 0 || idx > 3) return;
            idx = idx * 4;

            m[idx    ] = col.x;
            m[idx + 1] = col.y;
            m[idx + 2] = col.z;
            m[idx + 3] = col.w;
        }
        public Vector4 GetRow(int idx)
        {
            return new Vector4(
                m[idx],      
                m[idx + 4],
                m[idx + 8],
                m[idx + 12]
            );
        }
        public void SetRow(int idx, Vector4 row)
        {
            //if (idx < 0 || idx > 3) return;

            m[idx]      = row.x;
            m[idx + 4]  = row.y;
            m[idx + 8]  = row.z;
            m[idx + 12] = row.w;
        }
        public void Identity()
        {
            m[0] = m[5] = m[10] = m[15] = 1;
            m[1] = m[2] = m[3] = m[4] = m[6] = m[7] = m[8] = m[9] = m[11] = m[12] = m[13] = m[14] = 0;
        }
        /*
            Mat * Vec
            Mat行 * Vec列
            |Xx|Yx|Zx|                 
            |Xy|Yy|Zy|* |Vx|Vy|Vz|= |Xx*Vx+Yx*Vy+Zx*Vz|Xy*Vx+Yy*Vy+Zy*Vz|Xz*Vx+Yz*Vy+Zz*Vz|
            |Xz|Yz|Zz|   
         * */
        public Vector3 MulVec(Vector3 v)
        {
            return new Vector3(
                m[0] * v.x + m[4] * v.y + m[8] * v.z,
                m[1] * v.x + m[5] * v.y + m[9] * v.z,
                m[2] * v.x + m[6] * v.y + m[10] * v.z
                );
        }
        /*
            Mat * Pos
            Mat行 * Pos列
            |Xx|Yx|Zx|Tx|                 
            |Xy|Yy|Zy|Ty|* |Px|Py|Pz|1 |= |Xx*Px+Yx*Py+Zx*Pz+Tx*Pw|Xy*Px+Yy*Py+Zy*Pz+Ty*Pw|Xz*Px+Yz*Py+Zz*Pz+Tz*Pw|Xw*Px+Yw*Py+Zw*Pz+w*1|
            |Xz|Yz|Zz|Tz| 
            |Xw|Yw|Zw|w |
         * */
        //public Vector3 MulPos(Vector3 v)
        //{
        //    return new Vector3(
        //        m[0] * v.x + m[4] * v.y + m[8] * v.z + m[12],
        //        m[1] * v.x + m[5] * v.y + m[9] * v.z + m[13],
        //        m[2] * v.x + m[6] * v.y + m[10] * v.z + m[14]
        //        );
        //}
        public Vector4 MulPos(Vector4 v)
        {
            return new Vector4(
                m[0] * v.x + m[4] * v.y + m[8] * v.z + m[12] * v.w,
                m[1] * v.x + m[5] * v.y + m[9] * v.z + m[13] * v.w,
                m[2] * v.x + m[6] * v.y + m[10] * v.z + m[14] * v.w,
                m[3] * v.x + m[7] * v.y + m[11] * v.z + m[15] * v.w
                );
        }
        /*
            Mat1 * Mat2
            Mat1行 * Mat2列
            |m00|m01|m02|m03|   |n00|n01|n02|n03|   |m00*n00+m01*n10+m02*n20+m03*n30|m00*n01+m01*n11+m02*n21+m03*n31|m00*n02+m01*n12+m02*n22+m03*n32|m00*n03+m01*n13+m02*n23+m03*n33|
            |m10|m11|m12|m13| x |n10|n11|n12|n13| = |m10*n00+m11*n10+m12*n20+m13*n30|m10*n01+m11*n11+m12*n21+m13*n31|m10*n02+m11*n12+m12*n22+m13*n32|m10*n03+m11*n13+m12*n23+m13*n33|
            |m20|m21|m22|m23|   |n20|n21|n22|n23|   |m20*n00+m21*n10+m22*n20+m23*n30|m20*n01+m21*n11+m22*n21+m23*n31|m20*n02+m21*n12+m22*n22+m23*n32|m20*n03+m21*n13+m22*n23+m23*n33|
            |m30|m31|m32|m33|   |n30|n31|n32|n33|   |m30*n00+m31*n10+m32*n20+m33*n30|m30*n01+m31*n11+m32*n21+m33*n31|m30*n02+m31*n12+m32*n22+m33*n32|m30*n03+m31*n13+m32*n23+m33*n33|
         * */
        public Matrix4x4 MulMat(Matrix4x4 right)
        {
            //return new Matrix4x4(
            //    m[0] * right[0] + m[4] * right[1] + m[8] * right[2] + m[12] * right[3], m[1] * right[0] + m[5] * right[1] + m[9] * right[2] + m[13] * right[3], m[2] * right[0] + m[6] * right[1] + m[10] * right[2] + m[14] * right[3], m[3] * right[0] + m[7] * right[1] + m[11] * right[2] + m[15] * right[3],
            //    m[0] * right[4] + m[4] * right[5] + m[8] * right[6] + m[12] * right[7], m[1] * right[4] + m[5] * right[5] + m[9] * right[6] + m[13] * right[7], m[2] * right[4] + m[6] * right[5] + m[10] * right[6] + m[14] * right[7], m[3] * right[4] + m[7] * right[5] + m[11] * right[6] + m[15] * right[7],
            //    m[0] * right[8] + m[4] * right[9] + m[8] * right[10] + m[12] * right[11], m[1] * right[8] + m[5] * right[9] + m[9] * right[10] + m[13] * right[11], m[2] * right[8] + m[6] * right[9] + m[10] * right[10] + m[14] * right[11], m[3] * right[8] + m[7] * right[9] + m[11] * right[10] + m[15] * right[11],
            //    m[0] * right[12] + m[4] * right[13] + m[8] * right[14] + m[12] * right[15], m[1] * right[12] + m[5] * right[13] + m[9] * right[14] + m[13] * right[15], m[2] * right[12] + m[6] * right[13] + m[10] * right[14] + m[14] * right[15], m[3] * right[12] + m[7] * right[13] + m[11] * right[14] + m[15] * right[15]);
            return new Matrix4x4(
                m[0] * right[0] + m[4] * right[1] + m[8] * right[2] + m[12] * right[3], m[0] * right[4] + m[4] * right[5] + m[8] * right[6] + m[12] * right[7], m[0] * right[8] + m[4] * right[9] + m[8] * right[10] + m[12] * right[11], m[0] * right[12] + m[4] * right[13] + m[8] * right[14] + m[12] * right[15],
                m[1] * right[0] + m[5] * right[1] + m[9] * right[2] + m[13] * right[3], m[1] * right[4] + m[5] * right[5] + m[9] * right[6] + m[13] * right[7], m[1] * right[8] + m[5] * right[9] + m[9] * right[10] + m[13] * right[11], m[1] * right[12] + m[5] * right[13] + m[9] * right[14] + m[13] * right[15],
                m[2] * right[0] + m[6] * right[1] + m[10] * right[2] + m[14] * right[3], m[2] * right[4] + m[6] * right[5] + m[10] * right[6] + m[14] * right[7], m[2] * right[8] + m[6] * right[9] + m[10] * right[10] + m[14] * right[11], m[2] * right[12] + m[6] * right[13] + m[10] * right[14] + m[14] * right[15],
                m[3] * right[0] + m[7] * right[1] + m[11] * right[2] + m[15] * right[3], m[3] * right[4] + m[7] * right[5] + m[11] * right[6] + m[15] * right[7], m[3] * right[8] + m[7] * right[9] + m[11] * right[10] + m[15] * right[11], m[3] * right[12] + m[7] * right[13] + m[11] * right[14] + m[15] * right[15]
                );
        }
        public Matrix4x4 MulMat(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            return new Matrix4x4(
                m[0] * m00 + m[4] * m10 + m[8] * m20 + m[12] * m30, m[1] * m00 + m[5] * m10 + m[9] * m20 + m[13] * m30, m[2] * m00 + m[6] * m10 + m[10] * m20 + m[14] * m30, m[3] * m00 + m[7] * m10 + m[11] * m20 + m[15] * m30,
                m[0] * m01 + m[4] * m11 + m[8] * m21 + m[12] * m31, m[1] * m01 + m[5] * m11 + m[9] * m21 + m[13] * m31, m[2] * m01 + m[6] * m11 + m[10] * m21 + m[14] * m31, m[3] * m01 + m[7] * m11 + m[11] * m21 + m[15] * m31,
                m[0] * m02 + m[4] * m12 + m[8] * m22 + m[12] * m32, m[1] * m02 + m[5] * m12 + m[9] * m22 + m[13] * m32, m[2] * m02 + m[6] * m12 + m[10] * m22 + m[14] * m32, m[3] * m02 + m[7] * m12 + m[11] * m22 + m[15] * m32,
                m[0] * m03 + m[4] * m13 + m[8] * m23 + m[12] * m33, m[1] * m03 + m[5] * m13 + m[9] * m23 + m[13] * m33, m[2] * m03 + m[6] * m13 + m[10] * m23 + m[14] * m33, m[3] * m03 + m[7] * m13 + m[11] * m23 + m[15] * m33);
        }
        public Matrix4x4 Scale(float sx, float sy, float sz)
        {
            return GenScaleMat(sx, sy, sz).MulMat(this);
        }
        public Matrix4x4 Scale(Vector3 s)
        {
            return Scale(s.x, s.y, s.z);
        }
        /*
        http://www.songho.ca/opengl/gl_anglestoaxes.html
        Pitch: Rotation about X-axis
        Yaw: Rotation about Y-axis
        Roll: Rotation about Z-axis
        Roll ➔ Yaw ➔ Pitch XYZ = X * Y * Z
         * */
        public Matrix4x4 RotateX(float degree)
        {
            return GenXRotateMat(degree).MulMat(this);
        }
        public Matrix4x4 RotateY(float degree)
        {
            return GenYRotateMat(degree).MulMat(this);
        }
        public Matrix4x4 RotateZ(float degree)
        {
            return GenZRotateMat(degree).MulMat(this);
        }
        public Matrix4x4 Pitch(float degree) => RotateX(degree);
        public Matrix4x4 Yaw(float degree) => RotateY(degree);
        public Matrix4x4 Roll(float degree) => RotateZ(degree);
        public Matrix4x4 Euler(float dx, float dy, float dz)
        {
            // Roll ➔ Yaw ➔ Pitch XYZ = X * Y * Z
            return Matrix4x4.MulMat(GenEulerMat(dx, dy, dz), this);
        }
        public Matrix4x4 Euler(Vector3 euler)
        {
            // Roll ➔ Yaw ➔ Pitch XYZ = X * Y * Z
            return Euler(euler.x, euler.y, euler.z);
        }
        public Matrix4x4 RotateAxis(float degree, float ax, float ay, float az)
        {
            return Matrix4x4.MulMat(GenRotateAxisMat(degree, ax, ay, az), this);
        }
        public Matrix4x4 RotateAxis(float degree, Vector3 axis)
        {
            return RotateAxis(degree, axis.x, axis.y, axis.z);
        }
        public Matrix4x4 RotateAxis(Vector4 axis)
        {
            return RotateAxis(axis.z, axis.x, axis.y, axis.z);
        }
        public Matrix4x4 Translate(float tx, float ty, float tz)
        {
            return GenTranslateMat(tx, ty, tz).MulMat(this);
        }
        public Matrix4x4 Translate(Vector3 t)
        {
            return Translate(t.x, t.y, t.z);
        }
        public Matrix4x4 TRS(Vector3 t, Vector3 r, Vector3 s)
        {
            return Matrix4x4.MulMat(GenTRS(t, r, s), this);
        }
        public override string ToString()
        {
            return
                $"m[0]:{PS(m[0])},\t\t m[4]:{PS(m[4])},\t\t m[8]: {PS(m[8])},\t\t m[12]:{PS(m[12])}\n" +
                $"m[1]:{PS(m[1])},\t\t m[5]:{PS(m[5])},\t\t m[9]: {PS(m[9])},\t\t m[13]:{PS(m[13])}\n" +
                $"m[2]:{PS(m[2])},\t\t m[6]:{PS(m[6])},\t\t m[10]:{PS(m[10])},\t\t m[14]:{PS(m[14])}\n" +
                $"m[3]:{PS(m[3])},\t\t m[7]:{PS(m[7])},\t\t m[11]:{PS(m[11])},\t\t m[15]:{PS(m[15])}";
        }
        public string dump { get => ToString(); }
        public static string PS(float v)
        {
            return v.ToString("000.0000");
        }
    }
}