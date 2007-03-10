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
    [Serializable]
    public sealed class BoundingPolygon
    {
        private static bool Intersects(Vector2D[] vertexes1, Vector2D[] vertexes2)
        {
            for (int index = 0; index < vertexes1.Length; ++index)
            {
                bool broke = false;
                for (int index1 = 0; index1 < vertexes2.Length; ++index1)
                {
                    int index2 = (index1 + 1) % vertexes2.Length;
                    Scalar temp;
                    LineSegment.GetDistance(ref vertexes2[index], ref vertexes2[index2], ref vertexes1[index], out temp);
                    if (temp > 0)
                    {
                        broke = true;
                        break;
                    }
                }
                if (!broke)
                {
                    return true;
                }
            }
            return false;
        }

        Vector2D[] vertexes;
        public BoundingPolygon(Vector2D[] vertexes)
        {
            this.vertexes = vertexes;
        }
        public Vector2D[] Vertexes
        {
            get { return vertexes; }
        }

        public void GetDistance(ref Vector2D point, out Scalar result)
        {
            Scalar resultAbs = Scalar.MaxValue;
            result = -1;
            Scalar other, otherABS;
            for (int index = 0; index < vertexes.Length; ++index)
            {
                int index2 = (index + 1) % vertexes.Length;
                LineSegment.GetDistance(ref vertexes[index], ref vertexes[index2], ref point, out other);
                otherABS = Math.Abs(other);
                if (otherABS < resultAbs)
                {
                    result = other;
                    resultAbs = otherABS;
                }
            }
        }

        public bool Contains(Vector2D point)
        {
            Scalar distance;
            GetDistance(ref point, out distance);
            return distance <= 0;
        }
        public void Contains(ref Vector2D point, out bool result)
        {
            Scalar distance;
            GetDistance(ref point, out distance);
            result = distance <= 0;
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
            Vector2D[] corners = rect.Corners();
            result = Intersects(corners, vertexes) || Intersects(vertexes, corners);
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
            result = Intersects(polygon.vertexes, vertexes) || Intersects(vertexes, polygon.vertexes);
        }
    }
}