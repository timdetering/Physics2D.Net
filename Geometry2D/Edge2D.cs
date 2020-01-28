#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;

using AdvanceMath;
namespace AdvanceMath.Geometry2D
{

	/// <summary>
	/// This class describes a Edge of a BasePolygon.
	/// </summary>
    [Serializable]
    public sealed class Edge2D
    {

        #region fields
        /// <summary>
        /// The First Vertex2D of that creates the Edge. 
        /// </summary>
        Vertex2D first;
        /// <summary>
        /// The Second Vertex2D of that creates the Edge. 
        /// </summary>
        Vertex2D second;
        /// <summary>
        /// The Vector2D that represents the Edge.
        /// </summary>
        Vector2D edge;
        /// <summary>
        /// The Normalized Vector2D that represents the Edge's direction.
        /// </summary>
        Vector2D tangent;
        /// <summary>
        /// The Normal that is perpendicular to Edge and points out of the polygon.
        /// </summary>
        Vector2D normal;
        /// <summary>
        /// The Magnitude of the Edge Vector2D.
        /// </summary>
        //Scalar magnitude;
        /// <summary>
        /// The inverse of the Magnitude.
        /// </summary>
        //Scalar magnitudeInv;

        OriginalEdge2DInfo original;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a new Edge2D Instance on the Heap.
        /// </summary>
        /// <param name="first">The First Vertex2D of that creates the Edge.</param>
        /// <param name="second">The Second Vertex2D of that creates the Edge.</param>
        public Edge2D(Vertex2D first, Vertex2D second, Matrix2D matrix)
        {
            this.first = first;
            this.second = second;
            this.edge = this.first.OriginalPosition - this.second.OriginalPosition;
            Scalar magnitude = this.edge.Magnitude;
            if (magnitude > MathAdv.Tolerance)
            {
                this.tangent = this.edge * (1 / magnitude);
                this.normal = Vector2D.GetRightHandNormal(this.tangent);
            }
            else
            {
                this.tangent = Vector2D.Zero;
                this.normal = Vector2D.Zero;
            }
            this.original = new OriginalEdge2DInfo(this.edge, this.tangent, this.normal, magnitude);
            this.Transform(matrix);
        }
        /// <summary>
        /// A Copy Constructor. 
        /// </summary>
        /// <param name="copy">The Edge2D to be Copied.</param>
        /// <param name="first">A Copy of The First Vertex2D of that creates the Edge.</param>
        /// <param name="second">A Copy of The Second Vertex2D of that creates the Edge.</param>
        public Edge2D(Edge2D copy, Vertex2D first, Vertex2D second)
        {
            this.first = first;
            this.second = second;
            this.edge = copy.edge;
            //this.magnitude = copy.magnitude;
            //this.magnitudeInv = copy.magnitudeInv;
            this.tangent = copy.tangent;
            this.normal = copy.normal;
            this.original = copy.original;
        }
        #endregion
        #region properties
        /// <summary>
        /// Gets The First Vertex2D of that creates the Edge. 
        /// </summary>
        public Vertex2D FirstVertex
        {
            get
            {
                return first;
            }
        }
        /// <summary>
        /// Gets The Second Vertex2D of that creates the Edge. 
        /// </summary>
        public Vertex2D SecondVertex
        {
            get
            {
                return second;
            }
        }
        /// <summary>
        /// Gets the Vector2D that represents the Edge.
        /// </summary>
        public Vector2D Edge
        {
            get
            {
                return edge;
            }
        }
        /// <summary>
        /// Gets the Normalized Vector2D that represents the Edge's direction.
        /// </summary>
        public Vector2D NormalizedEdge
        {
            get
            {
                return tangent;
            }
        }
        /// <summary>
        /// Gets the Normal that is perpendicular to Edge and points out of the polygon.
        /// </summary>
        public Vector2D Normal
        {
            get
            {
                return normal;
            }
        }
        /// <summary>
        /// Gets the Magnitude of the Edge Vector2D.
        /// </summary>
        public Scalar Magnitude
        {
            get
            {
                return original.Magnitude;
            }
        }
        /// <summary>
        /// Get the inverse of the Magnitude.
        /// </summary>
        /*public Scalar MagnitudeInv
        {
            get
            {
                return magnitudeInv;
            }
        }*/
        #endregion
        #region methods
        /// <summary>
        /// Sets the values in the instance of Edge2D to another instances values.
        /// </summary>
        /// <param name="copy">A Edge2D to set this values to.</param>
        /// <remarks>This is ment to be used with a Edge2D created via the copy constructor. (not all values are set)</remarks>
        internal void Set(Edge2D copy)
        {
            this.edge = copy.edge;
            this.tangent = copy.tangent;
            this.normal = copy.normal;
        }
        /// <summary>
        /// Transforms the Edge2D by the given Matrix2D
        /// </summary>
        /// <param name="matrix"></param>
        public void Transform(Matrix2D matrix)
        {
            this.tangent = matrix.NormalMatrix * this.original.Tangent;
            this.normal = matrix.NormalMatrix * this.original.Normal;
            this.edge = matrix.VertexMatrix * this.original.Edge;
        }
        /// <summary>
        /// Test 2 See if 2 Edges Intersect;
        /// </summary>
        /// <param name="first">The First Edge.</param>
        /// <param name="second">The Second Edge.</param>
        /// <returns>true if they intersect; otherwise false.</returns>
        public static bool TestIntersection(Edge2D first, Edge2D second)
        {
            Vector2D Dist = second.FirstVertex.Position - first.FirstVertex.Position;
            Vector2D FirstEdge = first.edge;
            Vector2D SecondEdge = second.edge;
            //Scalar rn = (Dist.Y)*(SecondEdge.X)-(Dist.X)*(SecondEdge.Y);
            Scalar rn = SecondEdge ^ Dist;
            //Scalar sn = (Dist.Y)*(FirstEdge.X)-(Dist.X)*(FirstEdge.Y);
            Scalar sn = FirstEdge ^ Dist;
            //Scalar multiplyer = 1/((FirstEdge.X)*(SecondEdge.Y)-(FirstEdge.Y)*(SecondEdge.X));
            Scalar multiplyer = 1 / (FirstEdge ^ SecondEdge);
            Scalar r = rn * multiplyer;
            Scalar s = sn * multiplyer;
            //Console.WriteLine("s: "+s+" r: "+r+" multiplyer: "+multiplyer);
            return s <= 1 && 0 <= s && r <= 1 && 0 <= r;
        }
        /// <summary>
        /// Calculates the distances of the point.
        /// </summary>
        /// <param name="pointRelativeToWorld">The point whos Distances are to be calculated </param>
        /// <returns>An Edge2DDistanceInfo containing the calculated Distances.</returns>
        public Edge2DDistanceInfo CalcDistanceInfo(Vector2D pointRelativeToWorld)
        {
            Edge2DDistanceInfo rv = new Edge2DDistanceInfo();
            Vector2D FromSecond = pointRelativeToWorld - second.Position;
            rv.DistanceProjOnNormal = FromSecond * normal;
            rv.DistanceProjOnTangant = FromSecond * tangent;
            rv.BehindEdge2D = rv.DistanceProjOnNormal < 0;
            if (rv.DistanceProjOnTangant < 0)
            {
                rv.DistanceSq = rv.DistanceProjOnTangant * rv.DistanceProjOnTangant + rv.DistanceProjOnNormal * rv.DistanceProjOnNormal;
                rv.InEdgesVoronoiRegion = false;
            }
            else
            {
                if (rv.DistanceProjOnTangant > original.Magnitude)
                {
                    rv.DistanceProjOnTangant -= original.Magnitude;
                    rv.InEdgesVoronoiRegion = false;
                    rv.DistanceSq = rv.DistanceProjOnTangant * rv.DistanceProjOnTangant + rv.DistanceProjOnNormal * rv.DistanceProjOnNormal;
                }
                else
                {
                    rv.DistanceSq = rv.DistanceProjOnNormal * rv.DistanceProjOnNormal;
                    rv.DistanceProjOnTangant = 0;
                    rv.InEdgesVoronoiRegion = true;
                }
            }
            return rv;
        }
        /// <summary>
        /// Calculates the normal and the actual distance.
        /// </summary>
        /// <param name="info">The Edge2DDistanceInfo containing the precalculated Distances.</param>
        /// <param name="useEdgesNormal">If true the method will just use the Edges normal instead of calculating it.</param>
        /// <returns>An Edge2DNormalInfo containing the calculated Distance and Normal.</returns>
        public Edge2DNormalInfo CalcNormalInfo(Edge2DDistanceInfo info, bool useEdgesNormal)
        {

            Scalar dist = (Scalar)Math.Sqrt(info.DistanceSq);
            bool flipsign = info.DistanceProjOnNormal < 0;
            Vector2D normal2;
            if (!info.InEdgesVoronoiRegion && !useEdgesNormal)
            {
                Scalar distInv = 1 / dist;
                if (flipsign)
                {
                    normal2 = ((-info.DistanceProjOnNormal) * distInv) * this.normal + (info.DistanceProjOnTangant * distInv) * this.tangent;
                }
                else
                {
                    normal2 = (info.DistanceProjOnNormal * distInv) * this.normal + (info.DistanceProjOnTangant * distInv) * this.tangent;
                }
            }
            else
            {
                normal2 = this.normal;
            }
            if (flipsign)
            {
                dist = -dist;
            }
            return new Edge2DNormalInfo(ref dist, ref normal2);
        }
        /// <summary>
        /// Converts the Edge2D to a Line2D that can describe it.
        /// </summary>
        /// <returns>A Line2D that describes the Edge2Ds Edge.</returns>
        public Line2D ToLine2D()
        {
            return new Line2D(normal, first.Position * normal);
        }
        #endregion
        #region subClasses
        sealed class OriginalEdge2DInfo
        {
            public readonly Vector2D Edge;
            public readonly Vector2D Tangent;
            public readonly Vector2D Normal;
            public readonly Scalar Magnitude;
            public OriginalEdge2DInfo(Vector2D Edge, Vector2D Tangent, Vector2D Normal, Scalar Magnitude)
            {
                this.Edge = Edge;
                this.Tangent = Tangent;
                this.Normal = Normal;
                this.Magnitude = Magnitude;
            }
        }
        #endregion
    }
    /// <summary>
    /// Used to Store distance calculations Results from the <see cref="Edge2D.CalcDistanceInfo"/> method.
    /// </summary>
    [Serializable]
    public sealed class Edge2DDistanceInfo
    {
        /// <summary>
        /// The Total Distance from the Edge2D Squared.
        /// </summary>
        public Scalar DistanceSq;
        /// <summary>
        /// The Result of Projecting the Distance Vector2D onto the Edge2D's Normal.
        /// </summary>
        public Scalar DistanceProjOnNormal;
        /// <summary>
        /// The Result of Projecting the Distance Vector2D onto the Edge2D's Tangent (NormalizedEdge).
        /// </summary>
        public Scalar DistanceProjOnTangant;
        /// <summary>
        /// Is true if the Point is between the 2 Vertex2D that Create the Edge2D.
        /// </summary>
        public bool InEdgesVoronoiRegion;
        /// <summary>
        /// Is true if the Point is behind the Edge2D.
        /// </summary>
        public bool BehindEdge2D;
        /// <summary>
        /// Creates a new Edge2DDistanceInfo Instance.
        /// </summary>
        internal Edge2DDistanceInfo() { }
    }
    /// <summary>
    /// Used to Store distance calculations Results from the <see cref="Edge2D.CalcNormalInfo"/> method.
    /// </summary>
    [Serializable]
    public sealed class Edge2DNormalInfo
    {
        /// <summary>
        /// The Total Distance from the Edge2D.
        /// </summary>
        public readonly Scalar Distance;
        /// <summary>
        /// The Normal from the Edge2D pointing towards the point.
        /// </summary>
        /// <remarks>Depending on the parameters passed to the Method this can be different then Edge2D.Normal.</remarks>
        public readonly Vector2D Normal;
        /// <summary>
        /// Creates a new Edge2DNormalInfo Instance on the Heap.
        /// </summary>
        /// <param name="Distance">The Total Distance from the Edge2D.</param>
        /// <param name="Normal">The Normal from the Edge2D pointing towards the point.</param>
        internal Edge2DNormalInfo(
            ref Scalar Distance,
            ref Vector2D Normal)
        {
            this.Distance = Distance;
            this.Normal = Normal;
        }
    }
}