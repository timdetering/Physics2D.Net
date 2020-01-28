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
using System.ComponentModel;
using System.Xml.Serialization;
namespace AdvanceMath.Geometry2D
{
    /// <summary>
    /// This class is used to represent a Geometry at a certain location with a certain position.
    /// </summary>
    [Serializable]
    //[TypeConverter(typeof(AdvanceSystem.ComponentModel.Polygon2DConverter))]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public sealed class Polygon2D : IGeometry2D, System.Runtime.Serialization.IDeserializationCallback
    {
        #region static methods
        public static Vector2D[] FromSquare(Scalar length)
        {
            Scalar innerRadius = length / 2;
            Scalar outerRadius = (Scalar)Math.Sqrt(2 * (innerRadius * innerRadius));
            Vector2D[] vertices = new Vector2D[4];
            vertices[0] = new Vector2D(innerRadius, innerRadius);
            vertices[1] = new Vector2D(-innerRadius, innerRadius);
            vertices[2] = new Vector2D(-innerRadius, -innerRadius);
            vertices[3] = new Vector2D(innerRadius, -innerRadius);
            return vertices;
        }
        public static Vector2D[] FromRectangle(Scalar length, Scalar width)
        {
            Scalar Ld2 = length / 2;
            Scalar Wd2 = width / 2;
            Scalar outerRadius = (Scalar)Math.Sqrt((Ld2) * (Ld2) + (Wd2) * (Wd2));
            Scalar innerRadius = MathAdv.Min(Ld2, Wd2);
            Vector2D[] vertices = new Vector2D[4];
            vertices[0] = new Vector2D(Wd2, Ld2);
            vertices[1] = new Vector2D(-Wd2, Ld2);
            vertices[2] = new Vector2D(-Wd2, -Ld2);
            vertices[3] = new Vector2D(Wd2, -Ld2);
            return vertices;
        }
        public static Vector2D[] FromNumberofSidesAndRadius(int NumberofSides, Scalar Radius)
        {
            if (NumberofSides < 3)
            {
                throw new System.ArgumentException("Too Few Number of Sides. There must be more then 2. ");
            }
            Vector2D[] vertices = new Vector2D[NumberofSides];
            Scalar angleinc = (Scalar)(Math.PI * 2) / ((Scalar)NumberofSides);
            for (int angle = 0; angle != NumberofSides; angle++)
            {
                vertices[angle] = Vector2D.FromLengthAndAngle(Radius, ((Scalar)angle) * angleinc);
            }
            return vertices;
        }

        public static float CalculateInertia(Vector2D[] A, float mass)
        {
            if (A.Length == 1) return 0.0f;

            float denom = 0.0f;
            float numer = 0.0f;

            for (int j = A.Length - 1, i = 0; i < A.Length; j = i, i++)
            {
                Vector2D P0 = A[j];
                Vector2D P1 = A[i];

                float a = MathAdv.Abs(P0 ^ P1);
                float b = (P1 * P1 + P1 * P0 + P0 * P0);

                denom += (a * b);
                numer += a;
            }
            float inertia = (mass / 6.0f) * (denom / numer);

            return inertia;
        }
        public static explicit operator Polygon2D(Vector2D[] vertices)
        {
            return new Polygon2D(vertices);

        }





        public static Scalar CalcBoundingRadius(Vector2D[] vertices)
        {
            Scalar rv = vertices[0].MagnitudeSq;
            int length = vertices.Length;
            for (int pos = 1; pos < length; ++pos)
            {
                rv = MathAdv.Max(rv, vertices[pos].MagnitudeSq);
            }
            return MathAdv.Sqrt(rv);
        }
        /// <summary>
        /// Calculates the area of a polygon.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <returns>The area.</returns>
        public static Scalar CalcArea(Vector2D[] vertices)
        {
            Scalar returnvalue = 0;
            int length = vertices.Length;
            int pos2;
            for (int pos1 = 0; pos1 < length; ++pos1)
            {
                pos2 = (pos1 + 1) % length;
                returnvalue += vertices[pos1] ^ vertices[pos2];
            }
            return (Scalar)Math.Abs(returnvalue * .5);
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
            return Vector2D.Translate(CalcCentroid(vertices), vertices);
        }
        public static Scalar CalcPerimeter(Vector2D[] vertices)
        {
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
        public static void CalcRadius(Vector2D[] vertices, out Scalar innerRadius, out Scalar outerRadius)
        {
            innerRadius = Scalar.MaxValue;
            outerRadius = 0;
            Scalar mag;
            Scalar vpoeMag;
            Vector2D edge;
            Vector2D edgeNormal;
            int length = vertices.Length;
            for (int pos = 0; pos < length; ++pos)
            {
                if (pos == length - 1)
                {
                    edge = vertices[pos] - vertices[0];
                }
                else
                {
                    edge = vertices[pos] - vertices[pos + 1];
                }
                edgeNormal = Vector2D.GetRightHandNormal(Vector2D.Normalize(edge));
                vpoeMag = ((vertices[pos] * edgeNormal) * edgeNormal).Magnitude;
                mag = vertices[pos].Magnitude;
                if (vpoeMag < innerRadius)
                {
                    innerRadius = vpoeMag;
                }
                if (mag < innerRadius)
                {
                    innerRadius = mag;
                }
                if (mag > outerRadius)
                {
                    outerRadius = mag;
                }
            }
        }

        #endregion
        #region fields
        /// <summary>
        /// The edges that make up the polygon.
        /// </summary>
        [NonSerialized]
        Edge2D[] edges;
        /// <summary>
        /// The vertices that make up the polygon.
        /// </summary>
        /// <remarks><seealso cref="http://en.wikipedia.org/wiki/Vertices"/></remarks>
        Vertex2D[] vertices;
        /// <summary>
        /// the Position of the center of the polygon.
        /// </summary>
        ALVector2D position;
        /// <summary>
        /// the bounding Radius of the polygon. 
        /// </summary>
        [NonSerialized]
        Scalar boundingRadius;
        [NonSerialized]
        Scalar innerRadius;
        /// <summary>
        /// The length of the 3 arrays in this class.
        /// </summary>
        [NonSerialized]
        int length;
        /// <summary>
        /// The basis of the polygon.
        /// </summary>
        //IBaseGeometry2D baseGeometry;
        /// <summary>
        /// True if the polygon has a 90 degree angle on one corner.
        /// </summary>
        [NonSerialized]
        bool isBoxlike = false;
        /// <summary>
        /// The last calculated bounding box of the Polygon2D.
        /// </summary>
        [NonSerialized]
        BoundingBox2D boundingBox2D;

        #endregion
        #region constructors
        public Polygon2D() : this(FromNumberofSidesAndRadius(3,1)) { }
        public Polygon2D(Vector2D[] baseVertices)
            : this(ALVector2D.Zero, baseVertices)
        {

        }
        [AdvanceSystem.ComponentModel.UTCConstructor]
        public Polygon2D(ALVector2D Position, Vector2D[] baseVertices)
        {
            Set(Position, baseVertices);
        }
        private void Set(ALVector2D Position, Vector2D[] baseVertices)
        {
            CalcRadius(baseVertices, out innerRadius, out boundingRadius);
            this.position = Position;
            this.length = baseVertices.Length;
            this.vertices = new Vertex2D[length];
            this.edges = new Edge2D[length];
            Matrix2D matrix = position.ToMatrix2D();
            for (int pos = 0; pos < length; ++pos)
            {
                this.vertices[pos] = new Vertex2D(baseVertices[pos], matrix);
            }
            int pos2;
            for (int pos1 = 0; pos1 < length; ++pos1)
            {
                pos2 = (pos1 + 1) % length;
                this.edges[pos1] = new Edge2D(vertices[pos1], vertices[pos2], matrix);
            }
            this.isBoxlike = (length == 4) && ((edges[0].Normal * edges[1].Normal == 0));
        }

        /// <summary>
        /// A Copy Constructor.
        /// </summary>
        /// <param name="copy">The Polygon2D to make a polygone of.</param>
        public Polygon2D(Polygon2D copy)
        {
            this.boundingBox2D = copy.boundingBox2D;
            this.boundingRadius = copy.boundingRadius;
            this.innerRadius = copy.innerRadius;
            this.position = copy.position;
            this.length = copy.length;
            this.vertices = new Vertex2D[length];
            this.edges = new Edge2D[length];
            for (int pos = 0; pos < length; ++pos)
            {
                this.vertices[pos] = new Vertex2D(copy.vertices[pos]);
            }
            int pos2;
            for (int pos1 = 0; pos1 < length; ++pos1)
            {
                pos2 = (pos1 + 1) % length;
                this.edges[pos1] = new Edge2D(copy.edges[pos1], vertices[pos1], vertices[pos2]);
            }
            this.isBoxlike = copy.isBoxlike;
        }
        #endregion
        #region properties
        /// <summary>
        /// Gets The Edges that make up the polygon.
        /// </summary>
        [Browsable(false),XmlIgnore()]
        public Edge2D[] Edges
        {
            get
            {
                return edges;
            }
        }
        /// <summary>
        /// Gets The Vertices that make up the polygon.
        /// </summary>
        /// <remarks><seealso cref="http://en.wikipedia.org/wiki/Vertices"/></remarks>
        [Browsable(false), XmlIgnore()]
        public Vertex2D[] Vertices
        {
            get
            {
                return vertices;
            }
        }
        /// <summary>
        /// Gets and Sets the Position of the polygon.
        /// </summary>
        /// <remarks>Set will change the Edges and Vertices</remarks>
        public ALVector2D Position
        {
            get
            {
                return position;
            }
            set
            {
                SetPosition(value, value.ToMatrix2D());
            }
        }
        [AdvanceSystem.ComponentModel.UTCConstructorParameter("baseVertices")]
        public Vector2D[] BaseVertices
        {
            get
            {
                return Vertex2D.OriginalPositionToVector2DArray(vertices);
            }
            set
            {
                Set(position, value);
            }
        }
        /// <summary>
        /// Gets the last calculated bounding box of the Polygon2D.
        /// </summary>
        [Browsable(false), XmlIgnore()]
        public BoundingBox2D BoundingBox2D
        {
            get
            {
                if (boundingBox2D == null)
                {
                    CalcBoundingBox2D();
                }
                return boundingBox2D;
            }
        }
        /// <summary>
        /// Gets the Bounding Radius.
        /// </summary>
        [Browsable(false), XmlIgnore()]
        public Scalar BoundingRadius
        {
            get
            {
                return boundingRadius;
            }
        }
        [Browsable(false), XmlIgnore()]
        public Scalar InnerRadius
        {
            get
            {
                return innerRadius;
            }
        }

        [Browsable(false), XmlIgnore()]
        public bool IsBoxlike
        {
            get { return isBoxlike; }
        }

        [Browsable(false), XmlIgnore()]
        public Scalar Area
        {
            get
            {
                return CalcArea(Vertex2D.OriginalPositionToVector2DArray(this.vertices));
            }
        }

        [Browsable(false), XmlIgnore()]
        public Scalar Perimeter
        {
            get
            {
                return CalcPerimeter(Vertex2D.OriginalPositionToVector2DArray(this.vertices));
            }
        }

        [Browsable(false), XmlIgnore()]
        public Vector2D Centroid
        {
            get
            {
                return CalcCentroid(Vertex2D.OriginalPositionToVector2DArray(this.vertices));
            }
        }
        #endregion
        #region methods
        public void CalcBoundingBox2D()
        {
            Scalar upperX = position.Linear.X;
            Scalar upperY = position.Linear.Y;
            Scalar lowerX = upperX;
            Scalar lowerY = upperY;
            Vector2D RelativeToWorld;
            for (int pos = 0; pos < length; ++pos)
            {
                RelativeToWorld = vertices[pos].Position;
                if (upperX < RelativeToWorld.X)
                {
                    upperX = RelativeToWorld.X;
                }
                else if (lowerX > RelativeToWorld.X)
                {
                    lowerX = RelativeToWorld.X;
                }
                if (upperY < RelativeToWorld.Y)
                {
                    upperY = RelativeToWorld.Y;
                }
                else if (lowerY > RelativeToWorld.Y)
                {
                    lowerY = RelativeToWorld.Y;
                }
            }
            boundingBox2D = new BoundingBox2D(upperX, upperY, lowerX, lowerY);
        }
        /// <summary>
        /// Sets the values of the current Polygon2D to that of another.
        /// </summary>
        /// <param name="copy">the copy with the values.</param>
        /// <remarks>
        /// This method is ment to set a Polygon2D that was created via the copy constructor. 
        /// Any other use will most likely result in exceptions being thrown.
        /// </remarks>
        public void Set(Polygon2D copy)
        {
            this.boundingBox2D = copy.boundingBox2D;
            this.position = copy.position;
            for (int pos = 0; pos < length; ++pos)
            {
                vertices[pos].Set(copy.vertices[pos]);
                edges[pos].Set(copy.edges[pos]);
            }
        }
        public void SetPosition(ALVector2D position, Matrix2D matrix)
        {
            this.position = position;
            int pos;
            for (pos = 0; pos < length; ++pos)
            {
                vertices[pos].Transform(matrix.VertexMatrix);
            }
            for (pos = 0; pos < length; ++pos)
            {
                edges[pos].Transform(matrix);
            }
        }
        /// <summary>
        /// Calculates the Distance of a point from the Polygon2D.
        /// </summary>
        /// <param name="pointRelativeToWorld">the point to calculate the distance of.</param>
        /// <returns>The Polygon2DDistanceInfo describing all the distance info.</returns>
        public Polygon2DDistanceInfo CalcDistanceInfo(Vector2D pointRelativeToWorld)
        {
            Edge2D ClosestEdge = null;
            Edge2DDistanceInfo tmpDistanceInfo = null;
            Edge2DDistanceInfo bestDistanceInfo = null;
            bool InsideEdgesChecked = true;

            bool test = true;
            Edge2D edge;
            for (int pos = 0; pos < length; ++pos)
            {
                edge = edges[pos];
                tmpDistanceInfo = edge.CalcDistanceInfo(pointRelativeToWorld);
                InsideEdgesChecked = InsideEdgesChecked && tmpDistanceInfo.BehindEdge2D;
                if (!test)
                {
                    if (tmpDistanceInfo.InEdgesVoronoiRegion)
                    {
                        test = tmpDistanceInfo.DistanceProjOnNormal >= bestDistanceInfo.DistanceProjOnNormal;
                    }
                    else
                    {
                        test = tmpDistanceInfo.DistanceProjOnNormal > bestDistanceInfo.DistanceProjOnNormal;
                    }
                    if (test)
                    {
                        ClosestEdge = edge;
                        bestDistanceInfo = tmpDistanceInfo;
                    }
                }
                else
                {
                    ClosestEdge = edge;
                    bestDistanceInfo = tmpDistanceInfo;
                }
                test = false;
            }
            Edge2DNormalInfo NormalInfo = ClosestEdge.CalcNormalInfo(bestDistanceInfo, false);
            return new Polygon2DDistanceInfo(ref pointRelativeToWorld, ClosestEdge, bestDistanceInfo, NormalInfo, ref InsideEdgesChecked);
        }
        /// <summary>
        /// Calculates the Distance of a point from the Polygon2D.
        /// </summary>
        /// <param name="pointRelativeToWorld">the point to calculate the distance of.</param>
        /// <param name="goodedges">An array of booleans that indicate which edges to check.</param>
        /// <returns>The Polygon2DDistanceInfo describing all the distance info.</returns>
        public Polygon2DDistanceInfo CalcDistanceInfo(Vector2D pointRelativeToWorld, bool[] goodedges)
        {
            Edge2D ClosestEdge = null;
            Edge2DDistanceInfo tmpDistanceInfo = null;
            Edge2DDistanceInfo bestDistanceInfo = null;
            bool InsideEdgesChecked = true;

            bool test = true;
            bool returnnull = true;
            Edge2D edge;
            for (int pos = 0; pos < length; ++pos)
            {
                if (goodedges[pos])
                {
                    edge = edges[pos];
                    tmpDistanceInfo = edge.CalcDistanceInfo(pointRelativeToWorld);
                    InsideEdgesChecked = InsideEdgesChecked && tmpDistanceInfo.BehindEdge2D;
                    if (!test)
                    {
                        if (tmpDistanceInfo.InEdgesVoronoiRegion)
                        {
                            test = tmpDistanceInfo.DistanceProjOnNormal >= bestDistanceInfo.DistanceProjOnNormal;
                        }
                        else
                        {
                            test = tmpDistanceInfo.DistanceProjOnNormal > bestDistanceInfo.DistanceProjOnNormal;
                        }
                        if (test)
                        {
                            ClosestEdge = edge;
                            bestDistanceInfo = tmpDistanceInfo;
                        }
                    }
                    else
                    {
                        ClosestEdge = edge;
                        bestDistanceInfo = tmpDistanceInfo;
                        returnnull = false;
                    }
                    test = false;
                }
            }
            if (returnnull)
            {
                return null;
            }
            Edge2DNormalInfo NormalInfo = ClosestEdge.CalcNormalInfo(bestDistanceInfo, false);
            return new Polygon2DDistanceInfo(ref pointRelativeToWorld, ClosestEdge, bestDistanceInfo, NormalInfo, ref InsideEdgesChecked);
        }
        /// <summary>
        /// Tests to see is a point is inside the Polygon2D using the Crossing Numbers algorimth.
        /// </summary>
        /// <param name="pointRelativeToWorld">The point to test.</param>
        /// <returns>true if the point is inside; otherwise false.</returns>
        public bool CrossingNumberPointInsideTest(Vector2D pointRelativeToWorld)
        {
            int NumberOfCrossings = 0;
            Vector2D lastPoint = this.edges[0].FirstVertex.Position;
            bool LastLessThenTest = lastPoint.Y <= pointRelativeToWorld.Y;
            bool LastGreaterThenTest = lastPoint.Y > pointRelativeToWorld.Y;

            Edge2D currentEdge;
            Vector2D currentPoint;

            bool currentGreaterThenTest;
            bool currentLessThenTest;
            bool UpwardCrossing;
            bool DownwardCrossing;

            Scalar vt;
            Scalar intersect;
            for (int Pos = 0; Pos != length; ++Pos)
            {
                currentEdge = edges[Pos];
                currentPoint = currentEdge.SecondVertex.Position;

                currentGreaterThenTest = currentPoint.Y > pointRelativeToWorld.Y;
                currentLessThenTest = currentPoint.Y <= pointRelativeToWorld.Y;

                UpwardCrossing = (LastLessThenTest) && (currentGreaterThenTest);
                DownwardCrossing = (LastGreaterThenTest) && (currentLessThenTest);
                if (UpwardCrossing || DownwardCrossing)
                {
                    vt = (currentPoint.Y - pointRelativeToWorld.Y) / (currentEdge.Edge.Y);
                    intersect = currentPoint.X - vt * (currentEdge.Edge.X);
                    if (pointRelativeToWorld.X < intersect)
                    {
                        ++NumberOfCrossings;
                    }
                }
                lastPoint = currentPoint;
                LastLessThenTest = currentLessThenTest;
                LastGreaterThenTest = currentGreaterThenTest;
            }
            return ((NumberOfCrossings & 1) == 1);
        }


        public bool TestIntersection(Vector2D point)
        {
            return CrossingNumberPointInsideTest(point);
        }
        public void Shift(Vector2D offset)
        {
            Vector2D[] vt = Vertex2D.OriginalPositionToVector2DArray(vertices);
            Matrix2D ma = this.Position.ToMatrix2D();
            vt = Vector2D.Translate(offset, vt);
            for (int pos = 0; pos < this.length; ++pos)
            {
                vertices[pos].OriginalPosition = vt[pos];
            }
            //baseGeometry = new BasePolygon(vt);

            boundingRadius = CalcBoundingRadius(vt); //baseGeometry.BoundingRadius;
            offset = ma.NormalMatrix * offset;
            ALVector2D temp = new ALVector2D(Position.Angular, Position.Linear - offset);
            SetPosition(temp, temp.ToMatrix2D());
        }

        #endregion


        public void OnDeserialization(object sender)
        {
            Vector2D[] ar = Vertex2D.OriginalPositionToVector2DArray(this.vertices);
            this.Set(position, ar);
        }

    }
    /// <summary>
    /// A Result class that stores the result of the CalcDistanceInfo methods in Polygon2D.
    /// </summary>
    public sealed class Polygon2DDistanceInfo
    {
        public readonly Vector2D PointRelativeToWorld;
        public readonly Edge2D ClosestEdge;
        public readonly Edge2DDistanceInfo DistanceInfo;
        public readonly Edge2DNormalInfo NormalInfo;
        public readonly bool InsideEdgesChecked;
        internal Polygon2DDistanceInfo(
            ref Vector2D PointRelativeToWorld,
            Edge2D ClosestEdge,
            Edge2DDistanceInfo DistanceInfo,
            Edge2DNormalInfo NormalInfo,
            ref bool InsideEdgesChecked
            )
        {
            this.PointRelativeToWorld = PointRelativeToWorld;
            this.ClosestEdge = ClosestEdge;
            this.DistanceInfo = DistanceInfo;
            this.NormalInfo = NormalInfo;
            this.InsideEdgesChecked = InsideEdgesChecked;
        }
    }
}
