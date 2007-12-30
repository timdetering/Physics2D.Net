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
using AdvanceMath;
using AdvanceMath.Geometry2D;


namespace Physics2DDotNet.Ignorers
{
    /// <summary>
    /// this allows you to have platforms that are one way.
    /// Both the object going though and the platform must have this assigned.
    /// </summary>
    public class OneWayPlatformIgnorer : Ignorer
    {
        Vector2D allowedDirection;
        bool isPlatform;
        Body parent;
        public OneWayPlatformIgnorer(Body parent, Vector2D allowedDirection, bool isPlatform)
        {
            this.parent = parent;
            this.allowedDirection = allowedDirection.Normalized;
            this.isPlatform = isPlatform;
        }
        public override bool BothNeeded
        {
            get { return false; }
        }
        protected override bool CanCollide(Ignorer other)
        {
            OneWayPlatformIgnorer o = other as OneWayPlatformIgnorer;
            if (o != null)
            {
                if (isPlatform ^ o.isPlatform)
                {
                    if (isPlatform)
                    {
                        return CanCollide(allowedDirection, parent, o.parent);
                    }
                    else
                    {
                        return CanCollide(allowedDirection, o.parent, parent);
                    }
                }
            }
            return true;
        }
        private static bool CanCollide(Vector2D allowedDirection, Body b1, Body b2)
        {
            Matrix2x2 result;
            result.m00 = allowedDirection.X;
            result.m10 = allowedDirection.Y;
            result.m01 = -result.m10;
            result.m11 = result.m00;

            Matrix2x3 m1 = result * b1.Matrices.ToWorld;
            Matrix2x3 m2 = result * b2.Matrices.ToWorld;
            BoundingRectangle r1;
            b1.Shape.CalcBoundingRectangle(ref m1, out r1);
            BoundingRectangle r2;
            b2.Shape.CalcBoundingRectangle(ref m2, out r2);

            return (r1.Max.X + r2.Max.X) * .5f > r2.Max.X;
        }
    }
}