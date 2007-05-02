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
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    /// <summary>
    /// Represents a Single point.
    /// </summary>
    [Serializable]
    public sealed class Particle : Shape
    {
        /// <summary>
        /// Creates a new Particle Instance.
        /// </summary>
        public Particle()
            : this(1)
        {  }
        /// <summary>
        /// Creates a new Particle Instance.
        /// </summary>
        /// <param name="momentOfInertiaMultiplier">
        /// How hard it is to turn the shape. Depending on the construtor in the 
        /// Body this will be multiplied with the mass to determine the moment of inertia.
        /// </param>
        public Particle(Scalar momentOfInertiaMultiplier)
            : base(new Vector2D[] { Vector2D.Zero }, momentOfInertiaMultiplier)
        { }
        private Particle(Particle copy)
            : base(copy) { }
        public Vector2D Location { get { return vertexes[0]; } }
        public override bool CanGetIntersection
        {
            get { return false; }
        }
        public override bool CanGetDistance
        {
            get { return true; }
        }
        public override bool BroadPhaseDetectionOnly
        {
            get { return false; }
        }
        public override bool CanGetCustomIntersection
        {
            get { return false; }
        }

        protected override void CalcBoundingRectangle()
        {
            rect.Max = vertexes[0];
            rect.Min = vertexes[0];
        }
        public override bool TryGetIntersection(Vector2D vector, out IntersectionInfo info)
        {
            throw new NotSupportedException();
        }
        public override bool TryGetCustomIntersection(Body other, out object customIntersectionInfo)
        {
            throw new NotSupportedException();
        }
        public override void GetDistance(ref Vector2D point,out Scalar result)
        {
            Vector2D temp;
            Vector2D.Subtract(ref point, ref vertexes[0], out temp);
            Vector2D.GetMagnitude(ref temp, out result);
        }
        public override Shape Duplicate()
        {
            return new Particle(this);
        }
    }
}