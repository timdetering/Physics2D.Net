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
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    [Serializable]
    public sealed class Line : Shape
    {
        Scalar thicknessHalf;
        Scalar thickness;
        DistanceGrid grid;
        /// <summary>
        /// Creates a new Line Instance.
        /// </summary>
        /// <param name="vertexes">the vertexes that make up the shape of the Line</param>
        /// <param name="thickness">How thick the Line is.</param>
        /// <param name="gridSpacing">
        /// How large a grid cell is. Usualy you will want at least 2 cells between major vertexes.
        /// The smaller this is the better precision you get, but higher cost in memory. 
        /// The larger the less precision and if it's to high collision detection may fail completely.
        /// </param>
        public Line(Vector2D[] vertexes, Scalar thickness, Scalar gridSpacing)
            : this(vertexes, thickness, gridSpacing, 1)
        { }
        /// <summary>
        /// Creates a new Line Instance.
        /// </summary>
        /// <param name="vertexes">the vertexes that make up the shape of the Line</param>
        /// <param name="thickness">How thick the Line is.</param>
        /// <param name="gridSpacing">
        /// How large a grid cell is. Usualy you will want at least 2 cells between major vertexes.
        /// The smaller this is the better precision you get, but higher cost in memory. 
        /// The larger the less precision and if it's to high collision detection may fail completely.
        /// </param>
        /// <param name="momentOfInertiaMultiplier">
        /// How hard it is to turn the shape. Depending on the construtor in the 
        /// Body this will be multiplied with the mass to determine the moment of inertia.
        /// </param>
        public Line(Vector2D[] vertexes, Scalar thickness, Scalar gridSpacing, Scalar momentOfInertiaMultiplier)
            : base(vertexes, momentOfInertiaMultiplier)
        {
            if (thickness < 0) { throw new ArgumentOutOfRangeException("thickness", "must be equal or larger then zero"); }
            this.thickness = thickness;
            this.thicknessHalf = thickness * .5f;
            if (thickness > 0)
            {
                this.grid = new DistanceGrid(this, gridSpacing);
            }
        }
        private Line(Line copy)
            : base(copy)
        {
            this.thickness = copy.thickness;
            this.grid = copy.grid;
        }
        public Scalar Thickness
        {
            get { return thickness; }
        }
        public override bool CanGetIntersection
        {
            get { return grid != null; }
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

        public override void CalcBoundingRectangle()
        {
            BoundingRectangle.FromVectors(vertexes, out rect);
            rect.Max.X += thicknessHalf;
            rect.Max.Y += thicknessHalf;
            rect.Min.X -= thicknessHalf;
            rect.Min.Y -= thicknessHalf;
        }

        public override bool TryGetIntersection(Vector2D vector, out IntersectionInfo info)
        {
            if (grid == null)
            {
                throw new NotSupportedException();
            }
            Vector2D local;
            Vector2D.Transform(ref matrix2DInv.VertexMatrix, ref vector, out local);
            if (grid.TryGetIntersection(local, out info))
            {
                Vector2D.Transform(ref matrix2D.NormalMatrix, ref info.Normal, out info.Normal);
                info.Position = vector;
                return true;
            }
            return false;
        }
        public override bool TryGetCustomIntersection(Body other, out object customIntersectionInfo)
        {
            throw new NotSupportedException();
        }
        public override void GetDistance(ref Vector2D point, out Scalar result)
        {
            result = Scalar.MaxValue;
            Scalar other;
            for (int index = 0; index < vertexes.Length - 1; ++index)
            {
                LineSegment.GetDistance(ref vertexes[index], ref vertexes[index + 1], ref point, out other);
                if (other < result)
                {
                    result = other;
                }
            }
            result -= thicknessHalf;
        }
        public override Shape Duplicate()
        {
            return new Line(this);
        }






    }
}