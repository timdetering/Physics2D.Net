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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;

using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    /// <summary>
    /// A abstract class used to define the Shape of a Body.
    /// </summary>
    [Serializable]
    public abstract class Shape : IDuplicateable<Shape>
    {
        #region static methods
        public static Scalar InertiaOfCylindricalShell(Scalar radius)
        {
            return radius * radius;
        }
        public static Scalar InertiaOfHollowCylinder(Scalar innerRadius, Scalar outerRadius)
        {
            return .5f * (innerRadius * innerRadius + outerRadius * outerRadius);
        }
        public static Scalar InertiaOfSolidCylinder(Scalar radius)
        {
            return .5f * (radius * radius);
        }
        public static Scalar InertiaOfRectangle(Scalar length, Scalar width)
        {
            return (1f / 12f) * (length * length + width * width);
        }
        public static Scalar InertiaOfSquare(Scalar sideLength)
        {
            return (1f / 6f) * (sideLength * sideLength);
        }
        public static Scalar InertiaOfPolygon(Vector2D[] vertexes)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length == 0) { throw new ArgumentOutOfRangeException("vertexes"); }
            if (vertexes.Length == 1) { return 0; }

            Scalar denom = 0.0f;
            Scalar numer = 0.0f;

            for (int j = vertexes.Length - 1, i = 0; i < vertexes.Length; j = i, i++)
            {
                Scalar a, b, c;
                Vector2D P0 = vertexes[j];
                Vector2D P1 = vertexes[i];
                Vector2D.Dot(ref P1, ref P1, out a);
                Vector2D.Dot(ref P1, ref P0, out b);
                Vector2D.Dot(ref P0, ref P0, out c);
                a += b + c;
                Vector2D.ZCross(ref P0, ref P1, out b);
                b = MathHelper.Abs(b);
                denom += (b * a);
                numer += b;
            }
            return denom / (numer * 6);
        }
        public static Vector2D[] CreateCircle(Scalar radius, int vertexCount)
        {
            if (radius <= 0) { throw new ArgumentOutOfRangeException("radius", "Must be greater then zero."); }
            if (vertexCount < 3) { throw new ArgumentOutOfRangeException("vertexCount", "Must be equal or greater then 3"); }
            Vector2D[] result = new Vector2D[vertexCount];
            Scalar angleIncrement = MathHelper.TWO_PI / vertexCount;
            for (int index = 0; index < vertexCount; ++index)
            {
                Scalar angle = angleIncrement * index;
                Vector2D.FromLengthAndAngle(ref radius, ref angle, out result[index]);
            }
            return result;
        }
        protected static void GetDistanceEdge(ref Vector2D vector, ref Vector2D point1, ref Vector2D point2, out Scalar distance)
        {
            Scalar edgeLength, nProj, tProj;
            Vector2D tangent, normal, local;

            Vector2D.Subtract(ref vector, ref point2, out local);
            Vector2D.Subtract(ref point1, ref point2, out tangent);
            Vector2D.Normalize(ref tangent, out edgeLength, out tangent);
            Vector2D.GetRightHandNormal(ref tangent, out normal);
            Vector2D.Dot(ref local, ref normal, out nProj);
            Vector2D.Dot(ref local, ref tangent, out tProj);
            if (tProj < 0)
            {
                distance = MathHelper.Sqrt(tProj * tProj + nProj * nProj);
                if (nProj < 0)
                {
                    distance = -distance;
                }
            }
            else if (tProj > edgeLength)
            {
                tProj -= edgeLength;
                distance = MathHelper.Sqrt(tProj * tProj + nProj * nProj);
                if (nProj < 0)
                {
                    distance = -distance;
                }
            }
            else
            {
                distance = nProj;
            }
        }
        #endregion
        #region fields
        object tag;
        protected Matrix2D matrix2D;
        protected Matrix2D matrix2DInv;
        protected BoundingBox2D boundingBox;
        protected Scalar inertiaMultiplier;
        protected Vector2D[] originalVertexes;
        protected Vector2D[] vertexes;
        private Body parent; 
        #endregion
        #region constructors
        protected Shape(Vector2D[] vertexes)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            this.matrix2D = Matrix2D.Identity;
            this.matrix2DInv = Matrix2D.Identity;
            this.originalVertexes = vertexes;
            this.vertexes = (Vector2D[])vertexes.Clone();
        }
        protected Shape(Shape copy)
        {
            this.matrix2D = copy.matrix2D;
            this.matrix2DInv = copy.matrix2DInv;
            this.inertiaMultiplier = copy.inertiaMultiplier;
            this.boundingBox = copy.boundingBox;
            if (copy.tag is ICloneable)
            {
                this.tag = ((ICloneable)copy.tag).Clone();
            }
            else
            {
                this.tag = copy.tag;
            }
            this.originalVertexes = copy.originalVertexes;
            this.vertexes = (Vector2D[])copy.vertexes.Clone();
        } 
        #endregion
        #region properties
        public Body Parent
        {
            get { return parent; }
        }
        public Scalar MomentofInertiaMultiplier
        {
            get { return inertiaMultiplier; }
        }
        public abstract bool CanGetIntersection { get;}
        public BoundingBox2D BoundingBox2D
        {
            get { return boundingBox; }
        }
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public Matrix2D Matrix
        {
            get { return matrix2D; }
        }
        public Matrix2D MatrixInv
        {
            get { return matrix2DInv; }
        }
        public Vector2D[] OriginalVertices
        {
            get { return originalVertexes; }
        }
        public Vector2D[] Vertices
        {
            get { return vertexes; }
        } 
        #endregion
        #region methods
        public abstract void CalcBoundingBox2D();
        public virtual void ApplyMatrix(ref Matrix2D matrix)
        {
            this.matrix2D = matrix;
            Matrix2D.Invert(ref matrix, out matrix2DInv);
            OperationHelper.ArrayRefOp<Matrix3x3, Vector2D, Vector2D>(
                ref matrix.VertexMatrix,
                this.originalVertexes,
                this.vertexes,
                Vector2D.Transform);
        }
        public virtual void UpdateTime(Scalar dt) { }
        public virtual void Set(Shape other)
        {
            if (other == null) { throw new ArgumentNullException("other"); }
            other.vertexes.CopyTo(this.vertexes, 0);
        }
        public abstract bool TryGetIntersection(Vector2D vector, out IntersectionInfo info);
        public abstract Scalar GetDistance(Vector2D vector);
        public abstract Shape Duplicate();

        internal void OnAdded(Body parent)
        {
            if (this.parent != null) { throw new InvalidOperationException("must be orphan"); }
            this.parent = parent;
        }
        internal void OnRemoved()
        {
            this.parent = null;
        } 
        #endregion
    }
    [Serializable]
    public sealed class BoundingBox2DShape : Shape
    {
        public void SetBoundingBox(BoundingBox2D boundingBox)
        {
            this.boundingBox = boundingBox;
        }
        public override bool CanGetIntersection
        {
            get { return false; }
        }
        public BoundingBox2DShape()
            : base(new Vector2D[0])
        {

        }
        public override void ApplyMatrix(ref Matrix2D matrix){}
        public override void CalcBoundingBox2D() {}
        public override bool TryGetIntersection(Vector2D vector, out IntersectionInfo info)
        {
            throw new NotSupportedException();
        }
        public override float GetDistance(Vector2D vector)
        {
            throw new NotSupportedException();
        }
        public override Shape Duplicate()
        {
            return new BoundingBox2DShape();
        }
    }

}