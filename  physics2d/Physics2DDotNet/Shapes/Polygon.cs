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
using System.Collections.Generic;

using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    [Serializable]
    public sealed class Polygon : Shape
    {
        #region static methods
        /// <summary>
        /// Takes a 2D Boolean array with a true value representing a collidable pixel and converts 
        /// it to an array of vertex that surrounds that bitmap. The bitmap should be a single piece 
        /// if there are many pieces it will only return the geometry of the first piece it comes 
        /// across. Make sure there are at least 3 pixels in a piece otherwise an exception will be 
        /// thrown (it wont be a polygon). 
        /// </summary>
        /// <param name="bitmap">a bitmap to be converted. true means its collidable.</param>
        /// <returns>A Vector2D[] representing the bitmap.</returns>
        public static Vector2D[] CreateFromBitmap(bool[,] bitmap)
        {
            return CreateFromBitmap(new ArrayBitmap(bitmap));
        }
        public static Vector2D[] CreateFromBitmap(IBitmap bitmap)
        {
            if (bitmap == null) { throw new ArgumentNullException("bitmap"); }
            if (bitmap.Width < 2 || bitmap.Height < 2) { throw new ArgumentOutOfRangeException("bitmap"); }
            return BitmapHelper.CreateFromBitmap(bitmap);
        }

        /// <summary>
        /// creates vertexes that describe a Rectangle.
        /// </summary>
        /// <param name="height">The length of the Rectangle</param>
        /// <param name="width"></param>
        /// <returns>array of vectors the describe a rectangle</returns>
        public static Vector2D[] CreateRectangle(Scalar height, Scalar width)
        {
            if (height <= 0) { throw new ArgumentOutOfRangeException("height", "must be greater then 0"); }
            if (width <= 0) { throw new ArgumentOutOfRangeException("width", "must be greater then 0"); }
            Scalar Ld2 = height * .5f;
            Scalar Wd2 = width * .5f;
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
            if (maxLength <= 0) { throw new ArgumentOutOfRangeException("maxLength", "must be greater then zero"); }

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
                    mag = 1f / count;
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
        /// Reduces a Polygon's number of vertexes.
        /// </summary>
        /// <param name="vertexes">The Polygon to reduce.</param>
        /// <returns>The reduced vertexes.</returns>
        public static Vector2D[] Reduce(Vector2D[] vertexes)
        {
            return Reduce(vertexes, 0);
        }
        /// <summary>
        /// Reduces a Polygon's number of vertexes.
        /// </summary>
        /// <param name="vertexes">The Polygon to reduce.</param>
        /// <param name="areaTolerance">
        /// The amount the removal of a vertex is allowed to change the area of the polygon.
        /// (Setting this value to 0 will reverse what the Subdivide method does) 
        /// </param>
        /// <returns>The reduced vertexes.</returns>
        public static Vector2D[] Reduce(Vector2D[] vertexes, Scalar areaTolerance)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length < 2) { throw new ArgumentOutOfRangeException("vertexes"); }
            if (areaTolerance < 0) { throw new ArgumentOutOfRangeException("areaTolerance","must be equal to or greater then zero."); }
            List<Vector2D> result = new List<Vector2D>(vertexes.Length);
            Vector2D v1, v2, v3;
            Scalar old1, old2, new1;
            v1 = vertexes[vertexes.Length - 2];
            v2 = vertexes[vertexes.Length - 1];
            areaTolerance = areaTolerance * 2;
            for (int index = 0; index < vertexes.Length; ++index, v2 = v3)
            {
                if (index == vertexes.Length - 1)
                {
                    if (result.Count == 0) { throw new ArgumentOutOfRangeException("areaTolerance", "The Tolerance is too high!"); }
                    v3 = result[0];
                }
                else { v3 = vertexes[index]; }
                Vector2D.ZCross(ref v1, ref v2, out old1);
                Vector2D.ZCross(ref v2, ref v3, out old2);
                Vector2D.ZCross(ref v1, ref v3, out new1);
                if (Math.Abs(new1 - (old1 + old2)) > areaTolerance)
                {
                    result.Add(v2);
                    v1 = v2;
                }
            }
            return result.ToArray();
        }
        /// <summary>
        /// Calculates the area of a polygon.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The area.</returns>
        public static Scalar GetArea(Vector2D[] vertices)
        {
            Scalar result;
            BoundingPolygon.GetArea(vertices,out result);
            return result;
        }
        /// <summary>
        /// Calculates the Centroid of a polygon.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The Centroid of a polygon.</returns>
        /// <remarks>
        /// This is Also known as Center of Gravity/Mass.
        /// </remarks>
        public static Vector2D GetCentroid(Vector2D[] vertices)
        {
            Vector2D result;
            BoundingPolygon.GetCentroid(vertices, out result);
            return result;
        }
        /// <summary>
        /// repositions the polygon so the Centroid is the origin.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The vertices of the polygon with the Centroid as the Origin.</returns>
        public static Vector2D[] MakeCentroidOrigin(Vector2D[] vertices)
        {
            Vector2D centroid;
            BoundingPolygon.GetCentroid(vertices, out centroid);
            return OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(vertices, ref centroid, Vector2D.Subtract);
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
            : base(vertexes, momentOfInertiaMultiplier)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length < 3) { throw new ArgumentException("too few", "vertexes"); }
            if (gridSpacing <= 0) { throw new ArgumentOutOfRangeException("gridSpacing"); }
            this.grid = new DistanceGrid(this, gridSpacing);
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
        #endregion
        #region methods
        protected override void CalcBoundingRectangle()
        {
            BoundingRectangle.FromVectors(vertexes, out rect);
        }
        public override void GetDistance(ref Vector2D point,out Scalar result)
        {
            BoundingPolygon.GetDistance(vertexes, ref point, out result);
        }
        public override bool TryGetIntersection(Vector2D point, out IntersectionInfo info)
        {
            Vector2D local;
            Vector2D.Transform(ref matrix2DInv.VertexMatrix, ref point, out local);
            if (grid.TryGetIntersection(local, out info))
            {
                Vector2D.Transform(ref matrix2D.NormalMatrix, ref info.Normal, out info.Normal);
                info.Position = point;
                return true;
            }
            return false;
        }
        public override bool TryGetCustomIntersection(Body other, out object customIntersectionInfo)
        {
            throw new NotSupportedException();
        }
        public override Shape Duplicate()
        {
            return new Polygon(this);
        } 
        #endregion
    }
}