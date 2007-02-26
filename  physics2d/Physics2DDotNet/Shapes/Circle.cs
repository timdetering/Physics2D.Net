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



using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;

using AdvanceMath;
using Physics2DDotNet.Math2D;
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
namespace Physics2DDotNet
{
    /// <summary>
    /// A Circle
    /// </summary>
    [Serializable]
    public sealed class Circle : Shape
    {
        private static Vector2D Zero = Vector2D.Zero;
        #region fields
        private Scalar radius;
        private Vector2D position; 
        #endregion
        #region constructors

        /// <summary>
        /// Creates a new Circle Instance.
        /// </summary>
        /// <param name="radius">how large the circle is.</param>
        /// <param name="vertexCount">
        /// The number or vertex that will be generated along the perimeter of the circle. 
        /// This is for collision detection.
        /// </param>
        public Circle(Scalar radius, int vertexCount)
            : this(radius, vertexCount, InertiaOfSolidCylinder(radius))
        { }
        /// <summary>
        /// Creates a new Circle Instance.
        /// </summary>
        /// <param name="radius">how large the circle is.</param>
        /// <param name="vertexCount">
        /// The number or vertex that will be generated along the perimeter of the circle. 
        /// This is for collision detection.
        /// </param>
        /// <param name="momentOfInertiaMultiplier">
        /// How hard it is to turn the shape. Depending on the construtor in the 
        /// Body this will be multiplied with the mass to determine the moment of inertia.
        /// </param>
        public Circle(Scalar radius, int vertexCount, Scalar momentOfInertiaMultiplier)
            : base(CreateCircle(radius, vertexCount))
        {
            if (radius <= 0) { throw new ArgumentOutOfRangeException("radius", "must be larger then zero"); }
            if (momentOfInertiaMultiplier < 0) { throw new ArgumentOutOfRangeException("momentofInertiaMultiplier", "must be larger then zero"); }
            this.radius = radius;
            this.inertiaMultiplier = momentOfInertiaMultiplier;
        }
        private Circle(Circle copy)
            : base(copy)
        {
            this.position = copy.position;
            this.radius = copy.radius;
        } 
        #endregion
        #region properties
        /// <summary>
        /// the distance from the position where the circle ends.
        /// </summary>
        public Scalar Radius
        {
            get { return radius; }
        }
        /// <summary>
        /// the cenrter of the circle.
        /// </summary>
        public Vector2D Position
        {
            get { return position; }
        }
        public override bool CanGetIntersection
        {
            get { return true; }
        }
        #endregion
        #region methods
        public override void CalcBoundingBox2D()
        {
            boundingBox = new BoundingBox2D(
                position.X + radius,
                position.Y + radius,
                position.X - radius,
                position.Y - radius);
        }
        public override void Set(Shape shape)
        {
            Circle other = shape as Circle;
            if (other == null) { throw new ArgumentException("the parameter must be a shape", "shape"); }
            this.position = other.position;
        }
        public override Scalar GetDistance(Vector2D vector)
        {
            Vector2D.Subtract(ref vector, ref position, out vector);
            Scalar result;
            Vector2D.GetMagnitude(ref vector, out result);
            return result - radius;
        }
        public override bool TryGetIntersection(Vector2D vector, out IntersectionInfo info)
        {
            Scalar result;
            Vector2D normal;
            Vector2D.Subtract(ref vector, ref position, out normal);
            Vector2D.Normalize(ref normal, out result, out normal);
            result -= radius;
            if (result <= 0)
            {
                info = new IntersectionInfo(vector, normal, result);
                return true;
            }
            info = null;
            return false;
        }
        public override void ApplyMatrix(ref Matrix2D matrix)
        {
            Vector2D.Transform(ref matrix.VertexMatrix, ref Zero, out position);
            base.ApplyMatrix(ref matrix);
        }
        public override Shape Duplicate()
        {
            return new Circle(this);
        } 
        #endregion
    }
}