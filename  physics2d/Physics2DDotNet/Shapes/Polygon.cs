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
    [Serializable]
    public sealed class Polygon : Shape
    {
        #region static methods
        /// <summary>
        /// creates vertexes that describe a Rectangle.
        /// </summary>
        /// <param name="length">The length of the Rectangle</param>
        /// <param name="width"></param>
        /// <returns>array of vectors the describe a rectangle</returns>
        public static Vector2D[] CreateRectangle(Scalar length, Scalar width)
        {
            Scalar Ld2 = length / 2;
            Scalar Wd2 = width / 2;
            return new Vector2D[4]
            {
                new Vector2D(Wd2, Ld2),
                new Vector2D(-Wd2, Ld2),
                new Vector2D(-Wd2, -Ld2),
                new Vector2D(Wd2, -Ld2)
            };
        }
        /// <summary>
        /// makes sure the distance between 2 vertexes is under the length passed, by adding vertexes between them.
        /// </summary>
        /// <param name="vertexes">the original vertexes.</param>
        /// <param name="maxLength">the maximum distance allowed between 2 vertexes</param>
        /// <returns>The new vertexes.</returns>
        public static Vector2D[] Subdivide(Vector2D[] vertexes, Scalar maxLength)
        {
            return Subdivide(vertexes, maxLength, true);
        }
        /// <summary>
        /// makes sure the distance between 2 vertexes is under the length passed, by adding vertexes between them.
        /// </summary>
        /// <param name="vertexes">the original vertexes.</param>
        /// <param name="maxLength">the maximum distance allowed between 2 vertexes</param>
        /// <param name="loop">if it should check the distance between the first and last vertex.</param>
        /// <returns>The new vertexes.</returns>
        public static Vector2D[] Subdivide(Vector2D[] vertexes, Scalar maxLength, bool loop)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length < 2) { throw new ArgumentOutOfRangeException("vertexes"); }
            if (maxLength <= 0) { throw new ArgumentOutOfRangeException("maxLength"); }

            LinkedList<Vector2D> list = new LinkedList<Vector2D>(vertexes);

            LinkedListNode<Vector2D> node = list.First;
            while (node != null)
            {
                Vector2D line;
                if (node.Next == null)
                {
                    if (!loop) { break; }
                    line = list.First.Value - node.Value;
                }
                else
                {
                    line = node.Next.Value - node.Value;
                }
                Scalar mag;
                Vector2D.GetMagnitude(ref line, out mag);
                if (mag > maxLength)
                {
                    int count = (int)MathHelper.Ceiling(mag / maxLength);
                    mag = mag / (mag * count);
                    Vector2D.Multiply(ref line, ref mag, out line);
                    for (int pos = 1; pos < count; ++pos)
                    {
                        node = list.AddAfter(node, line + node.Value);
                    }
                }
                node = node.Next;
            }
            Vector2D[] result = new Vector2D[list.Count];


            list.CopyTo(result, 0);
            return result;
        }
        /// <summary>
        /// Calculates the area of a polygon.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The area.</returns>
        public static Scalar CalcArea(Vector2D[] vertices)
        {
            if (vertices == null) { throw new ArgumentNullException("vertices"); }
            if (vertices.Length < 3) { throw new ArgumentOutOfRangeException("vertices", "There must be at least 3 vertices"); }
            Scalar returnvalue = 0;
            int length = vertices.Length;
            for (int pos1 = 0; pos1 < length; ++pos1)
            {
                int pos2 = (pos1 + 1) % length;
                returnvalue += vertices[pos1] ^ vertices[pos2];
            }
            return MathHelper.Abs(returnvalue * .5f);
        }
        /// <summary>
        /// Calculates the Centroid of a polygon.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The Centroid of a polygon.</returns>
        /// <remarks>
        /// This is Also known as Center of Gravity/Mass.
        /// </remarks>
        public static Vector2D CalcCentroid(Vector2D[] vertices)
        {
            if (vertices == null) { throw new ArgumentNullException("vertices"); }
            if (vertices.Length < 3) { throw new ArgumentOutOfRangeException("vertices", "There must be at least 3 vertices"); }
            Vector2D returnvalue = Vector2D.Zero;
            int length = vertices.Length;
            int pos2;
            for (int pos1 = 0; pos1 != length; ++pos1)
            {
                pos2 = (pos1 + 1) % length;
                returnvalue += ((vertices[pos1] + vertices[pos2]) * (vertices[pos1] ^ vertices[pos2]));
            }
            return returnvalue * (1 / (CalcArea(vertices) * 6));
        }
        /// <summary>
        /// repositions the polygon so the Centroid is the origin.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The vertices of the polygon with the Centroid as the Origin.</returns>
        public static Vector2D[] MakeCentroidOrigin(Vector2D[] vertices)
        {
            if (vertices == null) { throw new ArgumentNullException("vertices"); }
            if (vertices.Length < 3) { throw new ArgumentOutOfRangeException("vertices", "There must be at least 3 vertices"); }
            Vector2D centroid = CalcCentroid(vertices);
            return OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(vertices, ref centroid, Vector2D.Subtract);
        }
        public static Scalar CalcPerimeter(Vector2D[] vertices)
        {
            if (vertices == null) { throw new ArgumentNullException("vertices"); }
            if (vertices.Length < 3) { throw new ArgumentOutOfRangeException("vertices", "There must be at least 3 vertices"); }
            Scalar returnvalue = 0;
            int length = vertices.Length;
            int pos2;
            for (int pos1 = 0; pos1 != length; ++pos1)
            {
                pos2 = (pos1 + 1) % length;
                returnvalue += (vertices[pos1] - vertices[pos2]).Magnitude;
            }
            return returnvalue;
        } 
        #endregion
        #region fields
        private DistanceGrid grid; 
        #endregion
        #region constructors
        /// <summary>
        /// Creates a new Polygon Instance.
        /// </summary>
        /// <param name="vertexes">the vertexes that make up the shape of the Polygon</param>
        /// <param name="gridSpacing">
        /// How large a grid cell is. Usualy you will want at least 2 cells between major vertexes.
        /// The smaller this is the better precision you get, but higher cost in memory. 
        /// The larger the less precision and if it's to high collision detection may fail completely.
        public Polygon(Vector2D[] vertexes, Scalar gridSpacing)
            : this(vertexes, gridSpacing, InertiaOfPolygon(vertexes)) { }
        /// <summary>
        /// Creates a new Polygon Instance.
        /// </summary>
        /// <param name="vertexes">the vertexes that make up the shape of the Polygon</param>
        /// <param name="gridSpacing">
        /// How large a grid cell is. Usualy you will want at least 2 cells between major vertexes.
        /// The smaller this is the better precision you get, but higher cost in memory. 
        /// The larger the less precision and if it's to high collision detection may fail completely.
        /// </param>
        /// <param name="momentOfInertiaMultiplier">
        /// How hard it is to turn the shape. Depending on the construtor in the 
        /// Body this will be multiplied with the mass to determine the moment of inertia.
        /// </param>
        public Polygon(Vector2D[] vertexes, Scalar gridSpacing, Scalar momentOfInertiaMultiplier)
            : base(vertexes)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length < 3) { throw new ArgumentException("too few", "vertexes"); }
            if (momentOfInertiaMultiplier <= 0) { throw new ArgumentOutOfRangeException("momentofInertiaMultiplier"); }
            if (gridSpacing <= 0) { throw new ArgumentOutOfRangeException("gridSpacing"); }
            this.grid = new DistanceGrid(this, gridSpacing);
            this.inertiaMultiplier = momentOfInertiaMultiplier;
        }
        private Polygon(Polygon copy)
            : base(copy)
        {
            this.grid = copy.grid;
        }
        
        #endregion
        #region properties
        public override bool CanGetIntersection
        {
            get { return true; }
        } 
        #endregion
        #region methods
        public override void CalcBoundingBox2D()
        {
            BoundingBox2D.FromVectors(vertexes, out boundingBox);
        }
        public override Scalar GetDistance(Vector2D vector)
        {
            Scalar resultAbs = Scalar.MaxValue;
            Scalar result = Scalar.MaxValue;
            Scalar other, otherABS;

            for (int index = 0; index < vertexes.Length; ++index)
            {
                int index2 = (index + 1) % vertexes.Length;
                GetDistanceEdge(ref vector, ref vertexes[index], ref vertexes[index2], out other);
                otherABS = MathHelper.Abs(other);
                if (otherABS < resultAbs)
                {
                    result = other;
                    resultAbs = otherABS;
                }
            }
            return result;
        }
        public override bool TryGetIntersection(Vector2D vector, out IntersectionInfo info)
        {
            Vector2D local;
            Vector2D.Transform(ref matrix2DInv.VertexMatrix, ref vector, out local);
            if (grid.TryGetIntersection(local, out info))
            {
                Vector2D.Transform(ref matrix2D.NormalMatrix, ref info.normal, out info.normal);
                info.position = vector;
                return true;
            }
            return false;
        }
        public override Shape Duplicate()
        {
            return new Polygon(this);
        } 
        #endregion
    }
}