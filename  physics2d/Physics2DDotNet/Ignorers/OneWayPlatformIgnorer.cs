#region MIT License
/*
 * Copyright (c) 2005-2008 Jonathan Mark Porter. http://physics2d.googlepages.com/
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
    /// this allows you to have platforms that are one way. like in platform games.
    /// </summary>
    public class OneWayPlatformIgnorer : Ignorer
    {
        Scalar depthAllowed;
        Matrix2x2 directionMatrix;
        public OneWayPlatformIgnorer(Vector2D allowedDirection, Scalar depthAllowed)
        {
            Vector2D.Normalize(ref allowedDirection, out allowedDirection);
            this.depthAllowed = depthAllowed;
            this.directionMatrix.m00 = allowedDirection.X;
            this.directionMatrix.m10 = allowedDirection.Y;
            this.directionMatrix.m01 = -directionMatrix.m10;
            this.directionMatrix.m11 = directionMatrix.m00;
        }
        public override bool BothNeeded
        {
            get { return true; }
        }
        protected override bool CanCollide(Body thisBody, Body otherBody, Ignorer other)
        {
            if (otherBody.IgnoresPhysicsLogics ||
                otherBody.IsBroadPhaseOnly)
            {
                return true;
            }
            Matrix2x3 m1, m2;
            Matrix2x3.Multiply(ref directionMatrix, ref thisBody.Matrices.ToWorld, out m1);
            Matrix2x3.Multiply(ref directionMatrix, ref otherBody.Matrices.ToWorld, out m2);
            BoundingRectangle r1, r2;
            thisBody.Shape.CalcBoundingRectangle(ref m1, out r1);
            otherBody.Shape.CalcBoundingRectangle(ref m2, out r2);
            return r1.Min.X + depthAllowed > r2.Max.X;
        }
    }
}