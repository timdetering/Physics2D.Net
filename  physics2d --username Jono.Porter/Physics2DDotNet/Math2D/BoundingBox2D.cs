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
using System.Collections.Generic;
using AdvanceMath;
using AdvanceMath.Design;
using System.Runtime.InteropServices;
namespace Physics2DDotNet.Math2D
{
    /// <summary>
    /// This class is used to descibe a the smallest box that can contain a Polygon2D object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = BoundingBox2D.Size, Pack = 0), Serializable]
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<BoundingBox2D>))]
    [AdvBrowsableOrder("Upper,Lower")]
    public struct BoundingBox2D
    {

        public const int Size = Vector2D.Size * 2;
        /// <summary>
        /// Creates a new BoundingBox2D Instance from 2 Vector2Ds.
        /// </summary>
        /// <param name="first">the first Vector2D.</param>
        /// <param name="second">the second Vector2D.</param>
        /// <returns>a new BoundingBox2D</returns>
        /// <remarks>The Upper and Lower values are automatically determined.</remarks>
        public static BoundingBox2D FromVectors(Vector2D first, Vector2D second)
        {
            BoundingBox2D result;
            if (first.X > second.X)
            {
                result.Upper.X = first.X;
                result.Lower.X = second.X;
            }
            else
            {
                result.Upper.X = second.X;
                result.Lower.X = first.X;
            }
            if (first.Y > second.Y)
            {
                result.Upper.Y = first.Y;
                result.Lower.Y = second.Y;
            }
            else
            {
                result.Upper.Y = second.Y;
                result.Lower.Y = first.Y;
            }
            return result;
        }
        public static void FromVectors(ref Vector2D first, ref  Vector2D second, out BoundingBox2D result)
        {
            if (first.X > second.X)
            {
                result.Upper.X = first.X;
                result.Lower.X = second.X;
            }
            else
            {
                result.Upper.X = second.X;
                result.Lower.X = first.X;
            }
            if (first.Y > second.Y)
            {
                result.Upper.Y = first.Y;
                result.Lower.Y = second.Y;
            }
            else
            {
                result.Upper.Y = second.Y;
                result.Lower.Y = first.Y;
            }
        }
        /// <summary>
        /// Creates a new BoundingBox2D Instance from multiple Vector2Ds.
        /// </summary>
        /// <param name="vectors">the list of vectors</param>
        /// <returns>a new BoundingBox2D</returns>
        /// <remarks>The Upper and Lower values are automatically determined.</remarks>
        public static BoundingBox2D FromVectors(Vector2D[] vectors)
        {
            BoundingBox2D result;
            FromVectors(vectors, out result); 
            return result;
        }
        public static void FromVectors(Vector2D[] vectors, out BoundingBox2D result)
        {
            if (vectors == null) { throw new ArgumentNullException("vectors"); }
            if (vectors.Length == 0) { throw new ArgumentOutOfRangeException("vectors"); }
            int length = vectors.Length;
            result.Upper = vectors[0];
            result.Lower = vectors[0];
            for (int pos = 1; pos < length; ++pos)
            {
                Vector2D current = vectors[pos];
                if (current.X > result.Upper.X)
                {
                    result.Upper.X = current.X;
                }
                else if (current.X < result.Lower.X)
                {
                    result.Lower.X = current.X;
                }
                if (current.Y > result.Upper.Y)
                {
                    result.Upper.Y = current.Y;
                }
                else if (vectors[pos].Y < result.Lower.Y)
                {
                    result.Lower.Y = current.Y;
                }
            }
        }
        /// <summary>
        /// Makes a BoundingBox2D that can contain the 2 BoundingBox2Ds passed.
        /// </summary>
        /// <param name="first">The First BoundingBox2D.</param>
        /// <param name="second">The Second BoundingBox2D.</param>
        /// <returns>The BoundingBox2D that can contain the 2 BoundingBox2Ds passed.</returns>
        public static BoundingBox2D FromUnion(BoundingBox2D first, BoundingBox2D second)
        {
            BoundingBox2D result;
            result.Upper.X = MathHelper.Max(first.Upper.X, second.Upper.X);
            result.Upper.Y = MathHelper.Max(first.Upper.Y, second.Upper.Y);
            result.Lower.X = MathHelper.Min(first.Lower.X, second.Lower.X);
            result.Lower.Y = MathHelper.Min(first.Lower.Y, second.Lower.Y);
            return result;
        }
        public static void FromUnion(ref BoundingBox2D first, ref BoundingBox2D second, out BoundingBox2D result)
        {
            result.Upper.X = MathHelper.Max(first.Upper.X, second.Upper.X);
            result.Upper.Y = MathHelper.Max(first.Upper.Y, second.Upper.Y);
            result.Lower.X = MathHelper.Min(first.Lower.X, second.Lower.X);
            result.Lower.Y = MathHelper.Min(first.Lower.Y, second.Lower.Y);
        }
        /// <summary>
        /// Makes a BoundingBox2D that contains the area where the BoundingBox2Ds Intersect.
        /// </summary>
        /// <param name="first">The First BoundingBox2D.</param>
        /// <param name="second">The Second BoundingBox2D.</param>
        /// <returns>The BoundingBox2D that can contain the 2 BoundingBox2Ds passed.</returns>
        public static BoundingBox2D FromIntersection(BoundingBox2D first, BoundingBox2D second)
        {
            BoundingBox2D result;
            result.Upper.X = MathHelper.Min(first.Upper.X, second.Upper.X);
            result.Upper.Y = MathHelper.Min(first.Upper.Y, second.Upper.Y);
            result.Lower.X = MathHelper.Max(first.Lower.X, second.Lower.X);
            result.Lower.Y = MathHelper.Max(first.Lower.Y, second.Lower.Y);
            return result;
        }
        public static void FromIntersection(ref BoundingBox2D first, ref BoundingBox2D second, out BoundingBox2D result)
        {
            result.Upper.X = MathHelper.Min(first.Upper.X, second.Upper.X);
            result.Upper.Y = MathHelper.Min(first.Upper.Y, second.Upper.Y);
            result.Lower.X = MathHelper.Max(first.Lower.X, second.Lower.X);
            result.Lower.Y = MathHelper.Max(first.Lower.Y, second.Lower.Y);
        }
        /// <summary>
        /// Tests the Interection between a BoundingBox2D and a point.
        /// </summary>
        /// <param name="box">The BoundingBox2D.</param>
        /// <param name="point">the point.</param>
        /// <returns>true if the point is inside the BoundingBox2D; otherwise false.</returns>
        public static bool TestIntersection(BoundingBox2D box, Vector2D point)
        {
            return
                box.Upper.X >= point.X && box.Lower.X <= point.X &&
                box.Upper.Y >= point.Y && box.Lower.Y <= point.Y;
        }
        [CLSCompliant(false)]
        public static bool TestIntersection(ref BoundingBox2D box, ref  Vector2D point)
        {
            return
                box.Upper.X >= point.X && box.Lower.X <= point.X &&
                box.Upper.Y >= point.Y && box.Lower.Y <= point.Y;
        }
        public static bool TestIntersection(BoundingBox2D box1, BoundingBox2D box2)
        {
            return !
                ((box1.Lower.X >= box2.Upper.X) || (box1.Upper.X <= box2.Lower.X) ||
                (box2.Lower.Y >= box1.Upper.Y) || (box2.Upper.Y <= box1.Lower.Y));
        }
        [CLSCompliant(false)]
        public static bool TestIntersection(ref BoundingBox2D box1, ref BoundingBox2D box2)
        {
            return !
                ((box1.Lower.X >= box2.Upper.X) || (box1.Upper.X <= box2.Lower.X) ||
                (box2.Lower.Y >= box1.Upper.Y) || (box2.Upper.Y <= box1.Lower.Y));
        }

        /// <summary>
        /// The Upper Bound.
        /// </summary>
        [AdvBrowsable]
        public Vector2D Upper;
        /// <summary>
        /// The Lower Bound.
        /// </summary>
        [AdvBrowsable]
        public Vector2D Lower;
        /// <summary>
        /// Creates a new BoundingBox2D Instance.
        /// </summary>
        /// <param name="upperX">The Upper Bound on the XAxis.</param>
        /// <param name="upperY">The Upper Bound on the YAxis.</param>
        /// <param name="lowerX">The Lower Bound on the XAxis.</param>
        /// <param name="lowerY">The Lower Bound on the YAxis.</param>
        public BoundingBox2D(Scalar upperX, Scalar upperY, Scalar lowerX, Scalar lowerY)
        {
            this.Upper.X = upperX;
            this.Upper.Y = upperY;
            this.Lower.X = lowerX;
            this.Lower.Y = lowerY;
        }
        /// <summary>
        /// Creates a new BoundingBox2D Instance from 2 Vector2Ds.
        /// </summary>
        /// <param name="Upper">The Upper Vector2D.</param>
        /// <param name="Lower">The Lower Vector2D.</param>
        [InstanceConstructor("Upper,Lower")]
        public BoundingBox2D(Vector2D upper, Vector2D lower)
        {
            this.Upper = upper;
            this.Lower = lower;
        }
        public Vector2D[] Corners
        {
            get
            {
                Vector2D[] returnvalue = new Vector2D[4];
                returnvalue[0] = Upper;
                returnvalue[1] = new Vector2D(Lower.X, Upper.Y);
                returnvalue[2] = Lower;
                returnvalue[3] = new Vector2D(Upper.X, Lower.Y);
                return returnvalue;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} > {1}",Upper,Lower);
        }
    }
}
