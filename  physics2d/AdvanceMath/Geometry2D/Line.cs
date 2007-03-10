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
    public struct Line
    {
        public Vector2D Normal;
        public Scalar D;
        public Line(Vector2D normal, Scalar d)
        {
            this.Normal = normal;
            this.D = d;
        }

        public void GetDistance(ref Vector2D point, out Scalar result)
        {
            Vector2D.Dot(ref point, ref Normal, out result);
            result += D;
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
        public bool Intersects(BoundingPolygon polygon)
        {
            bool result;
            Intersects(ref polygon, out result);
            return result;
        }

        public void Intersects(ref Ray ray, out Scalar result)
        {
            Scalar dir;
            Vector2D.Dot(ref Normal, ref ray.Direction, out dir);
            if (-dir > 0)
            {
                Scalar DistanceFromOrigin = Normal * ray.Origin + D;
                Vector2D.Dot(ref Normal, ref ray.Origin, out DistanceFromOrigin);
                DistanceFromOrigin = -((DistanceFromOrigin + D) / dir);
                if (DistanceFromOrigin >= 0)
                {
                    result = DistanceFromOrigin;
                    return;
                }
            }
            result = -1;
        }
        public void Intersects(ref BoundingRectangle box, out bool result)
        {
            Vector2D[] vertexes = box.Corners();
            Scalar distance;
            GetDistance(ref  vertexes[0], out distance);

            int sign = Math.Sign(distance);
            result = false;
            for (int index = 1; index < vertexes.Length; ++index)
            {
                GetDistance(ref  vertexes[index], out distance);

                if (Math.Sign(distance) != sign)
                {
                    result = true;
                    break;
                }
            }
        }
        public void Intersects(ref BoundingCircle circle, out bool result)
        {
            circle.Intersects(ref this, out result);
        }
        public void Intersects(ref BoundingPolygon polygon, out bool result)
        {
            Vector2D[] vertexes = polygon.Vertexes;
            Scalar distance;
            GetDistance(ref  vertexes[0], out distance);

            int sign = Math.Sign(distance);
            result = false;
            for (int index = 1; index < vertexes.Length; ++index)
            {
                GetDistance(ref  vertexes[index], out distance);

                if (Math.Sign(distance) != sign)
                {
                    result = true;
                    break;
                }
            }
        }
    }
}
