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
    /// </summary>
    public class OneWayPlatformIgnorer : Ignorer
    {
        Vector2D allowedDirection;
        public OneWayPlatformIgnorer(Vector2D allowedDirection)
        {
            this.allowedDirection = allowedDirection.Normalized;
        }
        public override bool BothNeeded
        {
            get { return true; }
        }
        protected override bool CanCollide(Body thisBody, Body otherBody, Ignorer other)
        {
            if (otherBody.IgnoresPhysicsLogics ||
                otherBody.Shape.BroadPhaseDetectionOnly)
            {
                return true;
            }
            Matrix2x2 result;
            result.m00 = allowedDirection.X;
            result.m10 = allowedDirection.Y;
            result.m01 = -result.m10;
            result.m11 = result.m00;

            Matrix2x3 m1 = result * thisBody.Matrices.ToWorld;
            Matrix2x3 m2 = result * otherBody.Matrices.ToWorld;
            BoundingRectangle r1;
            thisBody.Shape.CalcBoundingRectangle(ref m1, out r1);
            BoundingRectangle r2;
            otherBody.Shape.CalcBoundingRectangle(ref m2, out r2);

            return (r1.Max.X + r2.Max.X) * .5f > r2.Max.X;
        }
    }
}