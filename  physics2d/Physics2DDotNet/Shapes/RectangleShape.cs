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
using System.Xml.Serialization;

namespace Physics2DDotNet
{
    /// <summary>
    /// A shape whose BoundingRectangle is manualy Set and will not change, unless manualy changed.
    /// It is meant for clipping and Area triggers. If you want a Rectangle Use Polygon.CreateRectangle
    /// </summary>
    [Serializable]
    public sealed class RectangleShape : Shape
    {
        public event EventHandler BoundingRectangleRequested;
        public RectangleShape()
            : base(Empty, Scalar.PositiveInfinity)
        { }
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
            get { return true; }
        }
        public override bool CanGetCustomIntersection
        {
            get { return false; }
        }
        public override bool CanGetDragInfo
        {
            get { return false; }
        }

        public void SetRectangle(BoundingRectangle rectangle)
        {
            this.rect = rectangle;
        }
        public override void ApplyMatrix(ref Matrix2D matrix) { }
        protected override void CalcBoundingRectangle()
        {
            if (BoundingRectangleRequested != null)
            {
                BoundingRectangleRequested(this, EventArgs.Empty);
            }
        }
        public override bool TryGetIntersection(Vector2D point, out IntersectionInfo info)
        {
            ContainmentType type;
            rect.Contains(ref point, out type);
            if (type == ContainmentType.Contains)
            {
                Scalar xDist, yDist;
                info.Position = point;
                xDist = Math.Min(rect.Min.X - point.X, point.X - rect.Min.X);
                yDist = Math.Min(rect.Min.Y - point.Y, point.Y - rect.Min.Y);
                if (xDist < yDist)
                {
                    info.Distance = xDist;
                    info.Normal.Y = 0;
                    info.Normal.X = (xDist < 0) ? (1) : (-1);
                }
                else
                {
                    info.Distance = yDist;
                    info.Normal.X = 0;
                    info.Normal.Y = (yDist < 0) ? (1) : (-1);
                }
                return true;
            }
            else
            {
                info = IntersectionInfo.Zero;
                return false;
            }
        }
        public override bool TryGetCustomIntersection(Body other, out object customIntersectionInfo)
        {
            throw new NotSupportedException();
        }
        public override void GetDistance(ref Vector2D point, out Scalar result)
        {
            rect.GetDistance(ref point, out result);
        }
        public override Shape Duplicate()
        {
            return new RectangleShape();
        }

        public override DragInfo GetDragInfo(Vector2D tangent)
        {
            throw new NotSupportedException();
        }

        public override bool CanGetCentroid
        {
            get { return false; }
        }
        public override bool CanGetArea
        {
            get { return false; }
        }
        public override bool CanGetInertia
        {
            get { return false; }
        }

        public override Vector2D GetCentroid()
        {
            throw new NotSupportedException();
        }
        public override Scalar GetArea()
        {
            throw new NotSupportedException();
        }
        public override Scalar GetInertia()
        {
            throw new NotSupportedException();
        }
    }
}