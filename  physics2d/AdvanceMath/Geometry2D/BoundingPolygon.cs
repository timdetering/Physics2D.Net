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
using System.Runtime.InteropServices;
using AdvanceMath.Design;
namespace AdvanceMath.Geometry2D
{
    [Serializable]
    public sealed class BoundingPolygon
    {
        public static bool ContainsExclusive(Vector2D[] vertexes, Vector2D point)
        {
            bool result;
            ContainsExclusive(vertexes, ref point, out result);
            return result;
        }
        public static void ContainsExclusive(Vector2D[] vertexes, ref Vector2D point, out bool result)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length < 3) { throw new ArgumentOutOfRangeException("vertexes"); }
            int count = 0; //intersection count
            Vector2D v1 = vertexes[vertexes.Length - 1];
            Vector2D v2;
            Scalar temp;
            for (int index = 0; index < vertexes.Length; ++index, v1 = v2)
            {
                v2 = vertexes[index];
                bool t1 = (v1.Y <= point.Y);
                if (t1 ^ (v2.Y <= point.Y))
                {
                    temp = ((point.Y - v1.Y) * (v2.X - v1.X) - (point.X - v1.X) * (v2.Y - v1.Y));
                    if (t1) { if (temp > 0) { count++; } }
                    else { if (temp < 0) { count--; } }
                }
            }
            result = count != 0;
        }
        public static bool ContainsInclusive(Vector2D[] vertexes, Vector2D point)
        {
            bool result;
            ContainsInclusive(vertexes, ref point, out result);
            return result;
        }
        public static void ContainsInclusive(Vector2D[] vertexes, ref Vector2D point, out bool result)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length < 3) { throw new ArgumentOutOfRangeException("vertexes"); }
            int count = 0;    // the crossing count
            Vector2D v1 = vertexes[vertexes.Length - 1];
            Vector2D v2;
            for (int index = 0; index < vertexes.Length; index++, v1 = v2)
            {    
                v2 = vertexes[index];
                if (((v1.Y <= point.Y) ^ (v2.Y <= point.Y)) || 
                    (v1.Y == point.Y) || (v2.Y == point.Y))
                {
                    Scalar xIntersection = (v1.X + ((point.Y - v1.Y) / (v2.Y - v1.Y)) * (v2.X - v1.X));
                    if (point.X < xIntersection) // P.X < intersect
                    {
                        ++count;  
                    }
                    else if (xIntersection == point.X)
                    {
                        result = true;
                        return;
                    }
                }
            }
            result = (count & 1) != 0; //true if odd.
        }
        
        public static Scalar GetDistance(Vector2D[] vertexes, Vector2D point)
        {
            Scalar result;
            GetDistance(vertexes, ref point, out result);
            return result;
        }
        public static void GetDistance(Vector2D[] vertexes, ref Vector2D point, out Scalar result)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            result = -1;
            Scalar resultAbs = Scalar.MaxValue;
            Scalar other, otherABS;
            Vector2D v1, v2;
            v1 = vertexes[vertexes.Length - 1];
            for (int index = 0; index < vertexes.Length; ++index, v1 = v2)
            {
                v2 = vertexes[index];
                int index2 = (index + 1) % vertexes.Length;
                LineSegment.GetDistance(ref v1, ref v2, ref point, out other);
                otherABS = Math.Abs(other);
                if (otherABS < resultAbs)
                {
                    result = other;
                    resultAbs = otherABS;
                }
            }
        }


        Vector2D[] vertexes;
        public BoundingPolygon(Vector2D[] vertexes)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length < 3) { throw new ArgumentOutOfRangeException("vertexes"); }
            this.vertexes = vertexes;
        }
        public Vector2D[] Vertexes
        {
            get { return vertexes; }
        }

        public  Scalar GetDistance(Vector2D point)
        {
            Scalar result;
            GetDistance(vertexes, ref point, out result);
            return result;
        }
        public void GetDistance(ref Vector2D point, out Scalar result)
        {
            GetDistance(vertexes, ref point, out result);
        }

        public bool Contains(Vector2D point)
        {
            bool result;
            Contains(ref point, out result);
            return result;
        }
        public void Contains(ref Vector2D point, out bool result)
        {
            ContainsInclusive(vertexes,  ref point, out result);
        }
       
        public bool Contains(BoundingCircle circle)
        {
            bool result;
            Contains(ref circle, out result);
            return result;
        }
        public void Contains(ref BoundingCircle circle, out bool result)
        {
            Scalar distance;
            GetDistance(ref circle.Position, out distance);
            result = distance + circle.Radius <= 0;
        }

        public bool Contains(BoundingRectangle rect)
        {
            bool result;
            Contains(ref rect, out result);
            return result;
        }
        public void Contains(ref BoundingRectangle rect, out bool result)
        {
            Contains(rect.Corners(), out result);
        }

        public bool Contains(BoundingPolygon polygon)
        {
            bool result;
            Contains(ref polygon, out result);
            return result;
        }
        public void Contains(ref BoundingPolygon polygon, out bool result)
        {
            if (polygon == null) { throw new ArgumentNullException("polygon"); }
            Contains(polygon.vertexes, out result);
        }
        private void Contains(Vector2D[] otherVertexes, out bool result)
        {
            for (int index = 0; index < vertexes.Length; ++index)
            {
                ContainsExclusive(otherVertexes, ref vertexes[index], out result);
                if (result) { result = false; return; }
            }
            for (int index = 0; index < otherVertexes.Length; ++index)
            {
                ContainsInclusive(vertexes, ref otherVertexes[index], out result);
                if (!result)
                {
                    return;
                }
            }
            result = true;
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
            Intersects(ref circle, out result);
            return result;
        }
        public bool Intersects(BoundingPolygon polygon)
        {
            bool result;
            Intersects(ref polygon, out result);
            return result;
        }

        public void Intersects(ref Ray ray, out Scalar result)
        {
            result = -1;
            for (int index = 0; index < vertexes.Length; ++index)
            {
                int index2 = (index + 1) % vertexes.Length;
                Scalar temp;
                LineSegment.Intersects(ref vertexes[index], ref vertexes[index2], ref ray, out temp);
                if (temp >= 0 && (result == -1 || temp < result))
                {
                    result = temp;
                }
            }
        }
        public void Intersects(ref BoundingRectangle rect, out bool result)
        {
            Intersects(rect.Corners(), out result);
        }
        public void Intersects(ref BoundingCircle circle, out bool result)
        {
            result = false;
            for (int index = 0; index < vertexes.Length; ++index)
            {
                int index2 = (index + 1) % vertexes.Length;
                Scalar temp;
                LineSegment.GetDistance(ref vertexes[index], ref vertexes[index2], ref circle.Position, out temp);
                if (temp <= circle.Radius)
                {
                    result = true;
                    break;
                }
            }
        }
        public void Intersects(ref BoundingPolygon polygon, out bool result)
        {
            if (polygon == null) { throw new ArgumentNullException("polygon"); }
            Intersects(polygon.vertexes, out result);
        }
        private void Intersects(Vector2D[] otherVertexes, out bool result)
        {
            for (int index = 0; index < vertexes.Length; ++index)
            {
                ContainsInclusive(otherVertexes,  ref vertexes[index], out result);
                if (result) { return; }
            }
            for (int index = 0; index < otherVertexes.Length; ++index)
            {
                ContainsInclusive(vertexes,  ref otherVertexes[index], out result);
                if (result) { return; }
            }
            result = false;
        }
    }
}