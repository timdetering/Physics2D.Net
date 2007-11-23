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
using System.Collections.ObjectModel;

using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    /// <summary>
    /// A Ray Segment is a Ray that has a length. It can be used to represent 
    /// lasers or very fast projectiles.
    /// </summary>
    [Serializable]
    public struct RaySegment
    {
        public Ray RayInstance;
        public Scalar Length;
        public RaySegment(Ray ray, Scalar length)
        {
            this.RayInstance = ray;
            this.Length = length;
        }
        public RaySegment(Vector2D origin,Vector2D direction, Scalar length)
        {
            this.RayInstance.Origin = origin;
            this.RayInstance.Direction = direction;
            this.Length = length;
        }
    }
    /// <summary>
    /// The information of an intersection with another shape. 
    /// </summary>
    [Serializable]
    public class RaySegmentIntersectionInfo
    {
        Scalar[] distances;
        public RaySegmentIntersectionInfo(Scalar[] distances)
        {
            this.distances = distances;
        }
        /// <summary>
        /// An collection of distances away from the Ray Segments. 
        /// The indexes match up with the Segments in the Ray Segments class. 
        /// A negative value means there was no intersection. 
        /// </summary>
        public ReadOnlyCollection<Scalar> Distances
        {
            get { return new ReadOnlyCollection<Scalar>(distances); }
        }
    }

    /// <summary>
    /// A shape that holds multiple Ray Segments and generates custom collision events 
    /// for when they intersect something. The Sequential Impulses Solver does not 
    /// handle collisions with this shape.
    /// </summary>
    [Serializable]
    public class RaySegments : Shape
    {
        RaySegment[] originalSegments;
        RaySegment[] segments;

        public RaySegments(params RaySegment[] segments)
            : base(Empty, 1)
        {
            if (segments == null) { throw new ArgumentNullException("segments"); }
            this.originalSegments = segments;
            this.segments = (RaySegment[])segments.Clone();
        }
        protected RaySegments(RaySegments copy):base(copy)
        {
            this.originalSegments = copy.originalSegments;
            this.segments = (RaySegment[])copy.segments.Clone();
        }
        
        public RaySegment[] OriginalSegments
        {
            get { return originalSegments; }
        }
        public RaySegment[] Segments
        {
            get { return segments; }
        }
        public override bool CanGetIntersection
        {
            get { return false; }
        }
        public override bool CanGetDistance
        {
            get { return false; }
        }
        public override bool CanGetCustomIntersection
        {
            get { return true; }
        }
        public override bool BroadPhaseDetectionOnly
        {
            get { return false; }
        }


        bool TryGetCustomIntersection(Circle circle, out RaySegmentIntersectionInfo info)
        {
            bool intersects = false;
            Scalar[] result = new Scalar[segments.Length];
            BoundingCircle bounding = new BoundingCircle(Vector2D.Zero, circle.Radius);
            Matrix2D matrixInv = circle.MatrixInv;
            Matrix2D matrix = circle.Matrix;
            for (int index = 0; index < segments.Length; ++index)
            {
                RaySegment segment = segments[index];
                Ray ray2;
                Vector2D.Transform(ref matrixInv.VertexMatrix,ref segment.RayInstance.Origin,out ray2.Origin);
                Vector2D.Transform(ref matrixInv.NormalMatrix,ref segment.RayInstance.Direction,out ray2.Direction);
                Scalar temp;
                bounding.Intersects(ref ray2, out temp);
                if (temp >= 0)
                {

                    Vector2D point = ray2.Origin + ray2.Direction * temp;
                    Vector2D.Transform(ref matrix.VertexMatrix, ref point, out point);
                    Vector2D.Distance(ref point, ref segment.RayInstance.Origin, out temp);
                    if (temp <= segment.Length)
                    {
                        intersects = true;
                        result[index] = temp;
                        continue;
                    }
                }
                result[index] = -1;
            }
            if (intersects)
            {
                info = new RaySegmentIntersectionInfo(result);
            }
            else
            {
                info = null;
            }
            return intersects;
        }
        bool TryGetCustomIntersection(Polygon polygon, out RaySegmentIntersectionInfo info)
        {
            bool intersects = false;
            Scalar temp;
            Scalar[] result = new Scalar[segments.Length];
            BoundingPolygon poly = new BoundingPolygon(polygon.Vertices);
            BoundingRectangle rect = polygon.Rectangle;
            for (int index = 0; index < segments.Length; ++index)
            {
                RaySegment segment = segments[index];
                rect.Intersects(ref segment.RayInstance, out temp); 
                if (temp >= 0 && temp <= segment.Length)
                {
                    poly.Intersects(ref segment.RayInstance, out temp); 
                    if (temp < 0 || temp > segment.Length)
                    {
                        result[index] = -1;
                    }
                    else
                    {
                        result[index] = temp;
                        intersects = true;
                    }
                }
                else
                {
                    result[index] = -1;
                }
            }
            if (intersects)
            {
                info = new RaySegmentIntersectionInfo(result);
            }
            else
            {
                info = null;
            }
            return intersects;
        }
        bool TryGetCustomIntersection(MultipartPolygon polygon, out RaySegmentIntersectionInfo info)
        {
            bool intersects = false;
            Scalar[] result = new Scalar[segments.Length];
            Scalar temp;
            Vector2D[][] polygons = polygon.Polygons;
            for (int index = 0; index < segments.Length; ++index)
            {
                result[index] = -1;
            }
            for (int polyIndex = 0; polyIndex < polygons.Length; ++polyIndex)
            {
                BoundingPolygon poly = new BoundingPolygon(polygons[polyIndex]);
                for (int index = 0; index < segments.Length; ++index)
                {
                    RaySegment segment = segments[index];
                    rect.Intersects(ref segment.RayInstance, out temp);
                    if (temp >= 0 && temp <= segment.Length)
                    {
                        poly.Intersects(ref segment.RayInstance, out temp);
                        if (temp >= 0 && temp <= segment.Length)
                        {
                            if (result[index] == -1 || temp < result[index])
                            {
                                result[index] = temp;
                            }
                            intersects = true;
                        }
                    }
                }
            }
            if (intersects)
            {
                info = new RaySegmentIntersectionInfo(result);
            }
            else
            {
                info = null;
            }
            return intersects;
        }
        public override bool TryGetCustomIntersection(Body other, out object customIntersectionInfo)
        {
            bool intersects = false;
            RaySegmentIntersectionInfo info = null;
            Polygon polygon;
            MultipartPolygon multiPolygon;
            Circle circle;
            if ((polygon = other.Shape as Polygon) != null)
            {
                intersects = TryGetCustomIntersection(polygon, out info);
            }
            else if ((multiPolygon = other.Shape as MultipartPolygon) != null)
            {
                intersects = TryGetCustomIntersection(multiPolygon, out info);
            }
            else if ((circle = other.Shape as Circle) != null)
            {
                intersects = TryGetCustomIntersection(circle, out info);
            }
            customIntersectionInfo = info;
            return intersects;
        }
        public override void ApplyMatrix(ref Matrix2D matrix)
        {
            base.ApplyMatrix(ref matrix);
            for (int index = 0; index < segments.Length; ++index)
            {
                Vector2D.Transform(ref matrix.VertexMatrix,ref originalSegments[index].RayInstance.Origin, out segments[index].RayInstance.Origin);
                Vector2D.Transform(ref matrix.NormalMatrix,ref originalSegments[index].RayInstance.Direction, out segments[index].RayInstance.Direction);
            }
        }
        public override void GetDistance(ref Vector2D point, out Scalar result)
        {
            throw new NotSupportedException();
        }
        public override bool TryGetIntersection(Vector2D point, out IntersectionInfo info)
        {
            throw new NotSupportedException();
        }
        protected override void CalcBoundingRectangle()
        {
            Scalar x1, y1, x2, y2;
            RaySegment segment = segments[0];
            x1 = segment.RayInstance.Origin.X;
            y1 = segment.RayInstance.Origin.Y;
            x2 = segment.RayInstance.Direction.X * segment.Length + x1;
            y2 = segment.RayInstance.Direction.Y * segment.Length + y1;
            if (x1 > x2)
            {
                rect.Min.X = x2;
                rect.Max.X = x1;
            }
            else
            {
                rect.Min.X = x1;
                rect.Max.X = x2;
            }
            if (y1 > y2)
            {
                rect.Min.Y = y2;
                rect.Max.Y = y1;
            }
            else
            {
                rect.Min.Y = y1;
                rect.Max.Y = y2;
            }

            for (int index = 1; index < segments.Length; ++index)
            {
                segment = segments[index];
                x1 = segment.RayInstance.Origin.X;
                y1 = segment.RayInstance.Origin.Y;


                if (x1 > rect.Max.X)
                {
                    rect.Max.X = x1;
                }
                else if (x1 < rect.Min.X)
                {
                    rect.Min.X = x1;
                }
                if (y1 > rect.Max.Y)
                {
                    rect.Max.Y = y1;
                }
                else if (y1 < rect.Min.Y)
                {
                    rect.Min.Y = y1;
                }

                x2 = segment.RayInstance.Direction.X * segment.Length + x1;
                y2 = segment.RayInstance.Direction.Y * segment.Length + y1;

                if (x2 > rect.Max.X)
                {
                    rect.Max.X = x2;
                }
                else if (x2 < rect.Min.X)
                {
                    rect.Min.X = x2;
                }
                if (y2 > rect.Max.Y)
                {
                    rect.Max.Y = y2;
                }
                else if (y2 < rect.Min.Y)
                {
                    rect.Min.Y = y2;
                }
            }
        }
        public override Shape Duplicate()
        {
            return new RaySegments(this);
        }
    }
}