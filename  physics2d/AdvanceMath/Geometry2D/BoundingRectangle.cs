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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AdvanceMath.Design;
namespace AdvanceMath.Geometry2D
{
    [StructLayout(LayoutKind.Sequential, Size = BoundingRectangle.Size, Pack = 0), Serializable]
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<BoundingRectangle>))]
    [AdvBrowsableOrder("Max,Min")]
    public struct BoundingRectangle : IEquatable<BoundingRectangle>
    {
        public const int Size = Vector2D.Size * 2;

        /// <summary>
        /// Creates a new BoundingRectangle Instance from 2 Vector2Ds.
        /// </summary>
        /// <param name="first">the first Vector2D.</param>
        /// <param name="second">the second Vector2D.</param>
        /// <returns>a new BoundingRectangle</returns>
        /// <remarks>The Max and Min values are automatically determined.</remarks>
        public static BoundingRectangle FromVectors(Vector2D first, Vector2D second)
        {
            BoundingRectangle result;
            if (first.X > second.X)
            {
                result.Max.X = first.X;
                result.Min.X = second.X;
            }
            else
            {
                result.Max.X = second.X;
                result.Min.X = first.X;
            }
            if (first.Y > second.Y)
            {
                result.Max.Y = first.Y;
                result.Min.Y = second.Y;
            }
            else
            {
                result.Max.Y = second.Y;
                result.Min.Y = first.Y;
            }
            return result;
        }
        public static void FromVectors(ref Vector2D first, ref  Vector2D second, out BoundingRectangle result)
        {
            if (first.X > second.X)
            {
                result.Max.X = first.X;
                result.Min.X = second.X;
            }
            else
            {
                result.Max.X = second.X;
                result.Min.X = first.X;
            }
            if (first.Y > second.Y)
            {
                result.Max.Y = first.Y;
                result.Min.Y = second.Y;
            }
            else
            {
                result.Max.Y = second.Y;
                result.Min.Y = first.Y;
            }
        }
        /// <summary>
        /// Creates a new BoundingRectangle Instance from multiple Vector2Ds.
        /// </summary>
        /// <param name="vectors">the list of vectors</param>
        /// <returns>a new BoundingRectangle</returns>
        /// <remarks>The Max and Min values are automatically determined.</remarks>
        public static BoundingRectangle FromVectors(Vector2D[] vectors)
        {
            BoundingRectangle result;
            FromVectors(vectors, out result);
            return result;
        }
        public static void FromVectors(Vector2D[] vectors, out BoundingRectangle result)
        {
            if (vectors == null) { throw new ArgumentNullException("vectors"); }
            if (vectors.Length == 0) { throw new ArgumentOutOfRangeException("vectors"); }
            int length = vectors.Length;
            result.Max = vectors[0];
            result.Min = vectors[0];
            for (int pos = 1; pos < length; ++pos)
            {
                Vector2D current = vectors[pos];
                if (current.X > result.Max.X)
                {
                    result.Max.X = current.X;
                }
                else if (current.X < result.Min.X)
                {
                    result.Min.X = current.X;
                }
                if (current.Y > result.Max.Y)
                {
                    result.Max.Y = current.Y;
                }
                else if (vectors[pos].Y < result.Min.Y)
                {
                    result.Min.Y = current.Y;
                }
            }
        }
        /// <summary>
        /// Makes a BoundingRectangle that can contain the 2 BoundingRectangles passed.
        /// </summary>
        /// <param name="first">The First BoundingRectangle.</param>
        /// <param name="second">The Second BoundingRectangle.</param>
        /// <returns>The BoundingRectangle that can contain the 2 BoundingRectangles passed.</returns>
        public static BoundingRectangle FromUnion(BoundingRectangle first, BoundingRectangle second)
        {
            BoundingRectangle result;
            result.Max.X = MathHelper.Max(first.Max.X, second.Max.X);
            result.Max.Y = MathHelper.Max(first.Max.Y, second.Max.Y);
            result.Min.X = MathHelper.Min(first.Min.X, second.Min.X);
            result.Min.Y = MathHelper.Min(first.Min.Y, second.Min.Y);
            return result;
        }
        public static void FromUnion(ref BoundingRectangle first, ref BoundingRectangle second, out BoundingRectangle result)
        {
            result.Max.X = MathHelper.Max(first.Max.X, second.Max.X);
            result.Max.Y = MathHelper.Max(first.Max.Y, second.Max.Y);
            result.Min.X = MathHelper.Min(first.Min.X, second.Min.X);
            result.Min.Y = MathHelper.Min(first.Min.Y, second.Min.Y);
        }
        /// <summary>
        /// Makes a BoundingRectangle that contains the area where the BoundingRectangles Intersect.
        /// </summary>
        /// <param name="first">The First BoundingRectangle.</param>
        /// <param name="second">The Second BoundingRectangle.</param>
        /// <returns>The BoundingRectangle that can contain the 2 BoundingRectangles passed.</returns>
        public static BoundingRectangle FromIntersection(BoundingRectangle first, BoundingRectangle second)
        {
            BoundingRectangle result;
            result.Max.X = MathHelper.Min(first.Max.X, second.Max.X);
            result.Max.Y = MathHelper.Min(first.Max.Y, second.Max.Y);
            result.Min.X = MathHelper.Max(first.Min.X, second.Min.X);
            result.Min.Y = MathHelper.Max(first.Min.Y, second.Min.Y);
            return result;
        }
        public static void FromIntersection(ref BoundingRectangle first, ref BoundingRectangle second, out BoundingRectangle result)
        {
            result.Max.X = MathHelper.Min(first.Max.X, second.Max.X);
            result.Max.Y = MathHelper.Min(first.Max.Y, second.Max.Y);
            result.Min.X = MathHelper.Max(first.Min.X, second.Min.X);
            result.Min.Y = MathHelper.Max(first.Min.Y, second.Min.Y);
        }


        [AdvBrowsable]
        public Vector2D Max;
        [AdvBrowsable]
        public Vector2D Min;

        /// <summary>
        /// Creates a new BoundingRectangle Instance.
        /// </summary>
        /// <param name="maxX">The Upper Bound on the XAxis.</param>
        /// <param name="maxY">The Upper Bound on the YAxis.</param>
        /// <param name="minX">The Lower Bound on the XAxis.</param>
        /// <param name="minY">The Lower Bound on the YAxis.</param>
        public BoundingRectangle(Scalar maxX, Scalar maxY, Scalar minX, Scalar minY)
        {
            this.Max.X = maxX;
            this.Max.Y = maxY;
            this.Min.X = minX;
            this.Min.Y = minY;
        }
        /// <summary>
        /// Creates a new BoundingRectangle Instance from 2 Vector2Ds.
        /// </summary>
        /// <param name="max">The Upper Vector2D.</param>
        /// <param name="min">The Lower Vector2D.</param>
        [InstanceConstructor("Max,Min")]
        public BoundingRectangle(Vector2D max, Vector2D min)
        {
            this.Max = max;
            this.Min = min;
        }

        public Vector2D[] Corners()
        {
            return new Vector2D[4]
            {
                Max,
                new Vector2D(Min.X, Max.Y),
                Min,
                new Vector2D(Max.X, Min.Y),
            };
        }

        public void GetDistance(ref Vector2D point, out Scalar result)
        {
            Scalar xDistance = Math.Abs(point.X - ((Max.X + Min.X) * .5f)) - (Max.X - Min.X) * .5f;
            Scalar yDistance = Math.Abs(point.Y - ((Max.Y + Min.Y) * .5f)) - (Max.Y - Min.Y) * .5f;
            if (xDistance > 0 && yDistance > 0)
            {
                result = MathHelper.Sqrt(xDistance * xDistance + yDistance * yDistance);
            }
            else
            {
                result = Math.Max(xDistance, yDistance);
            }
        }

        public bool Contains(Vector2D point)
        {
            return
                point.X <= Max.X && point.X >= Min.X &&
                point.Y <= Max.Y && point.Y >= Min.Y;
        }
        public void Contains(ref Vector2D point, out bool result)
        {
            result =
                point.X <= Max.X && point.X >= Min.X &&
                point.Y <= Max.Y && point.Y >= Min.Y;
        }

        public Scalar Intersects(Ray ray)
        {
            Scalar result;
            Intersects(ref ray, out result);
            return result;
        }
        public bool Intersects(BoundingRectangle rect)
        {
            bool result;
            Intersects(ref rect, out result);
            return result;
        }
        public bool Intersects(BoundingCircle circle)
        {
            bool result;
            circle.Intersects(ref this, out result);
            return result;
        }
        public bool Intersects(Line line)
        {
            bool result;
            line.Intersects(ref this, out result);
            return result;
        }
        public bool Intersects(BoundingPolygon polygon)
        {
            bool result;
            polygon.Intersects(ref this, out result);
            return result;
        }

        public void Intersects(ref Ray ray, out Scalar result)
        {

            if (Contains(ray.Origin))
            {
                result = 0;
                return;
            }

            Scalar distance;
            Scalar intersectValue;
            result = -1;
            if (ray.Origin.X < Min.X && ray.Direction.X > 0)
            {
                distance = (Min.X - ray.Origin.X) / ray.Direction.X;
                if (distance > 0)
                {
                    intersectValue = ray.Origin.Y + ray.Direction.Y * distance;
                    if (intersectValue >= Min.Y && intersectValue <= Max.Y &&
                        (result == -1 || distance < result))
                    {
                        result = distance;
                    }
                }
            }
            if (ray.Origin.X > Max.X && ray.Direction.X < 0)
            {
                distance = (Max.X - ray.Origin.X) / ray.Direction.X;
                if (distance > 0)
                {
                    intersectValue = ray.Origin.Y + ray.Direction.Y * distance;
                    if (intersectValue >= Min.Y && intersectValue <= Max.Y &&
                        (result == -1 || distance < result))
                    {
                        result = distance;
                    }
                }
            }
            if (ray.Origin.Y < Min.Y && ray.Direction.Y > 0)
            {
                distance = (Min.Y - ray.Origin.Y) / ray.Direction.Y;
                if (distance > 0)
                {
                    intersectValue = ray.Origin.X + ray.Direction.X * distance;
                    if (intersectValue >= Min.X && intersectValue <= Max.X &&
                        (result == -1 || distance < result))
                    {
                        result = distance;
                    }
                }
            }
            if (ray.Origin.Y > Max.Y && ray.Direction.Y < 0)
            {
                distance = (Max.Y - ray.Origin.Y) / ray.Direction.Y;
                if (distance > 0)
                {
                    intersectValue = ray.Origin.X + ray.Direction.X * distance;
                    if (intersectValue >= Min.X && intersectValue <= Max.X &&
                        (result == -1 || distance < result))
                    {
                        result = distance;
                    }
                }
            }
        }
        public void Intersects(ref BoundingRectangle rect, out bool result)
        {
            result = !
                ((this.Min.X >= rect.Max.X) || (this.Max.X <= rect.Min.X) ||
                (rect.Min.Y >= this.Max.Y) || (rect.Max.Y <= this.Min.Y));
        }
        public void Intersects(ref BoundingCircle circle, out bool result)
        {
            circle.Intersects(ref this, out result);
        }
        public void Intersects(ref BoundingPolygon polygon, out bool result)
        {
            polygon.Intersects(ref this, out result);
        }
        public void Intersects(ref Line line, out bool result)
        {
            line.Intersects(ref this, out result);
        }

        public override string ToString()
        {
            return string.Format("{0} > {1}", Max, Min);
        }

        public override bool Equals(object obj)
        {
            return obj is BoundingRectangle && Equals((BoundingRectangle)obj);
        }
        public bool Equals(BoundingRectangle other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(BoundingRectangle rect1, BoundingRectangle rect2)
        {
            return Equals(ref rect1, ref rect2);
        }
        [CLSCompliant(false)]
        public static bool Equals(ref BoundingRectangle rect1, ref BoundingRectangle rect2)
        {
            return Vector2D.Equals(ref rect1.Min, ref rect2.Min) && Vector2D.Equals(ref rect1.Max, ref rect2.Max);
        }
        public override int GetHashCode()
        {
            return Min.GetHashCode() ^ Max.GetHashCode();
        }
        public static bool operator ==(BoundingRectangle rect1, BoundingRectangle rect2)
        {
            return Equals(ref rect1, ref rect2);
        }
        public static bool operator !=(BoundingRectangle rect1, BoundingRectangle rect2)
        {
            return !Equals(ref rect1, ref rect2);
        }

    }
}