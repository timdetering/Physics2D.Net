#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion




#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;


namespace AdvanceMath
{
    public static class MathHelper
    {
        #region consts
        public const Scalar E = (Scalar)System.Math.E;
        public const Scalar PI = (Scalar)System.Math.PI;
        public const Scalar TWO_PI = (Scalar)(System.Math.PI * 2);
        public const Scalar TWO_PI_INV = (Scalar)(1 / (System.Math.PI * 2));
        public const Scalar HALF_PI = (Scalar)(System.Math.PI / 2);
        public const Scalar HALF_THREE_PI = (Scalar)((2 * System.Math.PI) / 3);
        public const Scalar RADIANS_PER_DEGREE = (Scalar)(PI / 180.0);
        public const Scalar DEGREES_PER_RADIAN = (Scalar)(180.0f / PI);
        public const Scalar Tolerance = 0.000000001f;

        public const Scalar EPSILON = 1e-03f;

        internal static Scalar One = 1;
        internal static Scalar Two = 1;
        #endregion
        #region methods

        public static Scalar Lerp(Scalar left, Scalar right, Scalar amount)
        {
            return (right - left) * amount + left;
        }
        public static void Lerp(ref Scalar left, ref  Scalar right, ref  Scalar amount, out Scalar result)
        {
            result = (right - left) * amount + left;
        }

        public static Scalar Clamp(Scalar value, Scalar lower, Scalar upper)
        {
            return (value < lower) ? (lower) : ((value > upper) ? (upper) : (value));
        }
        public static void Clamp(ref Scalar value, ref Scalar lower, ref Scalar upper, out Scalar result)
        {
            result = (value < lower) ? (lower) : ((value > upper) ? (upper) : (value));
        }


        public static Scalar ClampAngle(Scalar radianAngle)
        {
            if (Math.Abs(radianAngle) <= PI) { return radianAngle; }
            return radianAngle - (Scalar)(Math.Truncate(radianAngle * TWO_PI_INV) + ((radianAngle < 0) ? (-1) : (1))) * TWO_PI;
        }
        [CLSCompliant(false)]
        public static void ClampAngle(ref Scalar radianAngle)
        {
            if (Math.Abs(radianAngle) <= PI) { return; }
            radianAngle -= (Scalar)(Math.Truncate(radianAngle * TWO_PI_INV) + ((radianAngle < 0) ? (-1) : (1))) * TWO_PI;
        }
        public static void ClampAngle(ref Scalar radianAngle, out Scalar result)
        {
            if (Math.Abs(radianAngle) <= PI) { result = radianAngle; return; }
            result = radianAngle - (Scalar)(Math.Truncate(radianAngle * TWO_PI_INV) + ((radianAngle < 0) ? (-1) : (1))) * TWO_PI;
        }

        public static Scalar AngleSubtract(Scalar radianAngle1, Scalar radianAngle2)
        {
            return ClampAngle(radianAngle1 - radianAngle2);
        }
        public static void AngleSubtract(ref Scalar radianAngle1,ref  Scalar radianAngle2,out Scalar result)
        {
            result = radianAngle1 - radianAngle2;
            ClampAngle(ref result);
        }

        /// <summary>
        /// Trys to Solve for x in the equation: (a * (x * x) + b * x + c == 0)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="plus">The result of (b + Math.Sqrt((b * b) - (4 * a * c))) / (2 * a)</param>
        /// <param name="minus">The result of (b - Math.Sqrt((b * b) - (4 * a * c))) / (2 * a)</param>
        /// <returns><see langword="false" /> if an error would have been thrown; otherwise <see langword="true" />.</returns>
        public static bool TrySolveQuadratic(Scalar a, Scalar b, Scalar c, out Scalar plus, out Scalar minus)
        {
            if (a == 0)
            {
                plus = -c / b;
                minus = plus;
                return true;
            }
            c = (b * b) - (4 * a * c);
            if (0 <= c)
            {
                b = -b;
                c = Sqrt(c);
                a = .5f / a;
                plus = ((b + c) * a);
                minus = ((b - c) * a);
                return true;
            }
            plus = 0;
            minus = 0;
            return false;
        }




        public static Scalar InvSqrt(Scalar number)
        {
            return 1 / Sqrt(number);
        }

        public static Scalar Max(params Scalar[] vals)
        {
            if (vals == null) { throw new ArgumentNullException("vals"); }
            if (vals.Length == 0) { throw new ArgumentException("There must be at least one value to compare", "vals"); }
            Scalar max = vals[0];
            if (Scalar.IsNaN(max)) { return max; }
            for (int i = 1; i < vals.Length; i++)
            {
                Scalar val = vals[i];
                if (val > max) { max = val; }
                else if (Scalar.IsNaN(val)) { return val; }
            }
            return max;
        }
        public static Scalar Min(params Scalar[] vals)
        {
            if (vals == null)
            {
                throw new ArgumentNullException("vals");
            }
            if (vals.Length == 0)
            {
                throw new ArgumentException("There must be at least one value to compare", "vals");
            }
            Scalar min = vals[0];
            if (Scalar.IsNaN(min)) { return min; }
            for (int i = 1; i < vals.Length; i++)
            {
                Scalar val = vals[i];
                if (val < min) { min = val; }
                else if (Scalar.IsNaN(val)) { return val; }
            }
            return min;
        }

        public static bool PointInTri2D(Vector2D p, Vector2D a, Vector2D b, Vector2D c)
        {
            bool bClockwise = (((b - a) ^ (p - b)) >= 0);
            return !(((((c - b) ^ (p - c)) >= 0) ^ bClockwise) && ((((a - c) ^ (p - a)) >= 0) ^ bClockwise));
        }
        /// <summary>
        ///		Converts degrees to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Scalar DegreesToRadians(Scalar degrees)
        {
            return degrees * RADIANS_PER_DEGREE;
        }
        /// <summary>
        ///		Converts radians to degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Scalar RadiansToDegrees(Scalar radians)
        {
            return radians * DEGREES_PER_RADIAN;
        }

        #region System.Math Methods
        public static Scalar Abs(Scalar value) 
        {
            return Math.Abs(value); 
        }
        public static int Sign(Scalar value) { return Math.Sign(value); }
        public static Scalar Max(Scalar val1, Scalar val2)
        {
            if (val1 > val2) { return val1; }
            if (Scalar.IsNaN(val1)) { return val1; }
            return val2;
        }
        public static Scalar Min(Scalar val1, Scalar val2)
        {
            if (val1 < val2) { return val1; }
            if (Scalar.IsNaN(val1)) { return val1; }
            return val2;
        }
        public static Scalar Acos(Scalar d) { return (Scalar)Math.Acos(d); }
        public static Scalar Asin(Scalar d) { return (Scalar)Math.Asin(d); }
        public static Scalar Atan(Scalar d) { return (Scalar)Math.Atan(d); }
        public static Scalar Atan2(Scalar y, Scalar x) { return (Scalar)Math.Atan2(y, x); }
        public static Scalar Ceiling(Scalar a) { return (Scalar)Math.Ceiling(a); }
        public static Scalar Cos(Scalar d) { return (Scalar)Math.Cos(d); }
        public static Scalar Cosh(Scalar value) { return (Scalar)Math.Cosh(value); }
        public static Scalar Exp(Scalar d) { return (Scalar)Math.Exp(d); }
        public static Scalar Floor(Scalar d) { return (Scalar)Math.Floor(d); }
        public static Scalar IEEERemainder(Scalar x, Scalar y) { return (Scalar)Math.IEEERemainder(x, y); }
        public static Scalar Log(Scalar d) { return (Scalar)Math.Log(d); }
        public static Scalar Log(Scalar a, Scalar newBase) { return (Scalar)Math.Log(a, newBase); }
        public static Scalar Log10(Scalar d) { return (Scalar)Math.Log10(d); }
        public static Scalar Pow(Scalar x, Scalar y) { return (Scalar)Math.Pow(x, y); }
        public static Scalar Round(Scalar a) { return (Scalar)Math.Round(a); }
        public static Scalar Round(Scalar value, int digits) { return (Scalar)Math.Round(value, digits); }
        public static Scalar Round(Scalar value, MidpointRounding mode) { return (Scalar)Math.Round(value, mode); }
        public static Scalar Round(Scalar value, int digits, MidpointRounding mode) { return (Scalar)Math.Round(value, digits, mode); }
        public static Scalar Sin(Scalar a) { return (Scalar)Math.Sin(a); }
        public static Scalar Sinh(Scalar value) { return (Scalar)Math.Sinh(value); }
        public static Scalar Sqrt(Scalar d) { return (Scalar)Math.Sqrt(d); }
        public static Scalar Tan(Scalar a) { return (Scalar)Math.Tan(a); }
        public static Scalar Tanh(Scalar value) { return (Scalar)Math.Tanh(value); }
        public static Scalar Truncate(Scalar d) { return (Scalar)Math.Truncate(d); }
        #endregion


        #endregion
    }
}
