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
using System;
using System.Collections.Generic;
using AdvanceMath.Geometry2D;
using AdvanceMath;

namespace Physics2D.CollisionDetection 
{
	/// <summary>
    /// This class does collision detetion between the 2 ICollidableBodyParts of 2 ICollidableBodys.
	/// </summary>
    [Serializable]
    public sealed class CollisionPartPair		
	{
		#region fields 
        /// <summary>
        /// The collidables this PartPair belongs to.
        /// </summary>
        CollisionPair pair;
        /// <summary>
        /// The first part.
        /// </summary>
        public ICollidableBodyPart part1;
        /// <summary>
        /// The second part.
        /// </summary>
        public ICollidableBodyPart part2;
        /// <summary>
        /// The type of collision.
        /// </summary>
		CollisionType collisionType;
        /// <summary>
        /// An array of collision points.
        /// </summary>
		List<CollisionPointInfo> pointInfos;
        /// <summary>
        /// The the difference in the positions of the 2 parts.
        /// </summary>
		Vector2D distance;
        /// <summary>
        /// The magnitude of the distance Vector2D.
        /// </summary>
		float distanceMag;
        /// <summary>
        /// The Normalized distance Vector2D.
        /// </summary>
		Vector2D distanceNorm;
        /// <summary>
        /// The CollisionInfo the detection logic has determined to be best.
        /// </summary>
		CollisionInfo bestCollisionInfo = null;
        /// <summary>
        /// The Polygon2D that describes the shape of the first Part.
        /// </summary>
		Polygon2D Polygon2DforPart1 = null;
        /// <summary>
        /// The Polygon2D that describes the shape of the second Part.
        /// </summary>
		Polygon2D Polygon2DforPart2 = null;
        /// <summary>
        /// States if the collidables is colliding. if it fails any test it is set to false.
        /// </summary>
        public bool IsValid = true;
        /// <summary>
        /// states if the position between the first and second parts have been switched.
        /// </summary>
		bool switched = false;
        CollisionPairParameters parameters;
        bool DoBoxlike;
		#endregion
		#region constructors
        /// <summary>
        /// Creates a new CollisionPartPair instance.
        /// </summary>
        /// <param name="collidables">The collidables this PartPair belongs to.</param>
        /// <param name="part1">The first part.</param>
        /// <param name="part2">The second part</param>
        public CollisionPartPair(CollisionPair pair, ICollidableBodyPart part1, ICollidableBodyPart part2, CollisionPairParameters parameters)
		{
            this.parameters = parameters;
            this.pair = pair;
            this.part1 = part1;
            this.part2 = part2;
			if(this.part1.UseCircleCollision^this.part2.UseCircleCollision)
			{
				collisionType = CollisionType.PolygonCircle;
				if(this.part2.UseCircleCollision)
				{
					this.switched = true;
                    this.part1 = part2;
                    this.part2 = part1;
				}
				this.Polygon2DforPart2 = this.part2.Polygon2D;
			}
			else
			{
				if(this.part1.UseCircleCollision)
				{
					collisionType = CollisionType.CircleCircle;
				}
				else
				{
					collisionType = CollisionType.PolygonPolygon;
					this.Polygon2DforPart1 = this.part1.Polygon2D;
					this.Polygon2DforPart2 = this.part2.Polygon2D;
				}
			}
            this.IsValid = true;
		}
		#endregion
		#region properties
        /// <summary>
        /// Gets the Relative Linear Velocity of the 2 parts.
        /// </summary>
		public  Vector2D RelativeLinearVelocity
		{
			get
			{
				return pair.Collidable1.Current.Velocity.Linear - pair.Collidable2.Current.Velocity.Linear;
			}
		}
		/// <summary>
		/// Gets the CollisionInfo that descibes what is the best point of contact for use in collision reaction.
		/// </summary>
		public CollisionInfo BestCollisionInfo
		{
			get
			{
				return bestCollisionInfo;
			}
		}
		/// <summary>
        /// The Merged Coefficients of the 2 objects.
		/// </summary>
        public Coefficients Coefficients
		{
			get
			{
				return Coefficients.Merge(this.part1.Coefficients, this.part2.Coefficients);
			}
		}
        /// <summary>
        /// Gets The maximum possible depth for a collision.
        /// </summary>
		public float MaxDepth
		{
			get
			{
                return Math.Max(this.part1.BaseGeometry.InnerRadius, this.part2.BaseGeometry.InnerRadius);
			}
		}
		/// <summary>
		/// Gets the Vector2D that descibes the difference of the 2 parts positions.
		/// </summary>
        public Vector2D  Distance
		{
			get
			{
				return distance;
			}
		}
        /// <summary>
        /// The Normalized Distance Vector2D.
        /// </summary>
		public Vector2D  DistanceNorm
		{
			get
			{
				return distanceNorm;
			}
		}
        /// <summary>
        /// The Magnitude of the Distance Vector.
        /// </summary>
		public float  DistanceMag
		{
			get
			{
				return distanceMag;
			}
		}
		#endregion
		#region methods
        /// <summary>
        /// Calculates the Distance values.
        /// </summary>
		public void CalcDistance()
		{
			distance = part1.Position.Linear-part2.Position.Linear;
            distanceNorm = Vector2D.Normalize(distance, out distanceMag);
		}
        /// <summary>
        /// Tests the 
        /// </summary>
        /// <returns></returns>
		public bool TestBoundingRadius()
		{
            bool returnvalue = (distanceMag-(part1.BaseGeometry.BoundingRadius+part2.BaseGeometry.BoundingRadius)<Physics.CollisionTolerance );
            if (!returnvalue)
            {
                IsValid = false;
            }
            return returnvalue;
		}
		public bool TestBoundingBox2Ds()
		{
            BoundingBox2D body1bb = part1.SweepBoundingBox2D;
            BoundingBox2D body2bb = part2.SweepBoundingBox2D;
            bool returnvalue = body1bb.TestIntersection(body2bb);
            if (!returnvalue)
            {
                IsValid = false;
            }
            return returnvalue;
		}
		private CollisionInfo CalcCollisionInfoNew(float dt)
		{
			CollisionInfo returnvalue = null;
			Vector2D CollisionNormal;
			Vector2D RelativeVelocity;
			bool setFirst = false;
			bool distanceTest = true;
			float maxDepth = MaxDepth;
			foreach(CollisionPointInfo pointinfo in pointInfos)
			{
				if(pointinfo.Distance<Physics.CollisionTolerance)//&&-pointinfo.Distance<MaxDepth)
				{
					distanceTest = true;
					if(setFirst)
					{
						if(pointinfo.IsEdgonEdge)
						{
							distanceTest = returnvalue.Distance<=pointinfo.Distance;
						}
						else
						{
							distanceTest = returnvalue.Distance<pointinfo.Distance;
						}
					}
					if(distanceTest)
					{
						RelativeVelocity = pair.GetRelativeVelocityAt(pointinfo.WorldPoint);
						CollisionNormal = pointinfo.CollisionNormal;
						if(!pointinfo.VertexIsICollidableBody1)
						{
							CollisionNormal = -CollisionNormal;
						}
						float RelativeVelocityAlongNormal = RelativeVelocity*CollisionNormal;
						if(RelativeVelocityAlongNormal < -0.006)
						{
							//if(Math.Abs(RelativeVelocityAlongNormal*dt)+maxDepth>Math.Abs(pointinfo.DistanceProjOnNormal))
							//{
								if(CollisionNormal*CollisionNormal != 1)
								{
									CollisionNormal = Vector2D.Normalize(CollisionNormal);
								}
								setFirst = true;
								returnvalue = new CollisionInfo(pointinfo.WorldPoint,CollisionNormal,pointinfo.Distance);
							//}
						}
					}
				}
			}
			return returnvalue;
		}
		private CollisionInfo CalcCollisionInfo(float dt)
		{
			CollisionInfo returnvalue = null;
			Vector2D CollisionNormal;
			Vector2D RelativeVelocity;
			bool setFirst = false;
			bool distanceTest = true;
			float maxDepth = MaxDepth;
			foreach(CollisionPointInfo pointinfo in pointInfos)
			{
				if(pointinfo.Distance<Physics.CollisionTolerance)//&&-pointinfo.Distance<MaxDepth*2)
				{
					distanceTest = true;
					if(setFirst)
					{
						if(pointinfo.IsEdgonEdge)
						{
							distanceTest = returnvalue.Distance<=pointinfo.Distance;
						}
						else
						{
							distanceTest = returnvalue.Distance<pointinfo.Distance;
						}
					}
					if(distanceTest)
					{
                        RelativeVelocity = pair.GetRelativeVelocityAt(pointinfo.WorldPoint);
						CollisionNormal = pointinfo.CollisionNormal;
						if((!pointinfo.VertexIsICollidableBody1)^switched)
						{
							CollisionNormal = -CollisionNormal;
						}
						float RelativeVelocityAlongNormal = RelativeVelocity*CollisionNormal;
						if(RelativeVelocityAlongNormal < -0.006)
						{
                            if ((!DoBoxlike) || (Math.Abs(RelativeVelocityAlongNormal * dt) + maxDepth > -pointinfo.DistanceProjOnNormal))
                            {
                                setFirst = true;
                                returnvalue = new CollisionInfo(pointinfo.WorldPoint, CollisionNormal, pointinfo.Distance);
                               
                            }
						}
					}
				}
			}
			return returnvalue;
		}
		private CollisionPointInfo GetPointInfo(Vertex2D vertex,Polygon2D geometry,bool vertexIsICollidableBody1,bool[] goodedges)
		{
            Polygon2DDistanceInfo GDI;
            if (goodedges == null)
            {
                GDI = geometry.CalcDistanceInfo(vertex.Position);
            }
            else
            {
                GDI = geometry.CalcDistanceInfo(vertex.Position, goodedges);
            }
            if (GDI == null || !GDI.DistanceInfo.InEdgesVoronoiRegion)
			{
				return null;
			}
			//CollisionPointInfo possibleCollisions= baseGeometry.CalcDistance(,goodedges,false);
			CollisionPointInfo returnvalue = new CollisionPointInfo();
			returnvalue.InEdgesVoronoiRegion = GDI.DistanceInfo.InEdgesVoronoiRegion;
            returnvalue.ClosestEdge = GDI.ClosestEdge;
			returnvalue.CollisionNormal = GDI.NormalInfo.Normal;
			returnvalue.Distance = GDI.NormalInfo.Distance;
			returnvalue.DistanceSq = GDI.DistanceInfo.DistanceSq;
			returnvalue.InsideEdgesChecked = GDI.InsideEdgesChecked;
			returnvalue.IsEdgonEdge = false;
			returnvalue.DistanceProjOnNormal = GDI.DistanceInfo.DistanceProjOnNormal;
			returnvalue.DistanceProjOnTangant = GDI.DistanceInfo.DistanceProjOnTangant;
			returnvalue.WorldPoint = GDI.PointRelativeToWorld;
			if(returnvalue.InEdgesVoronoiRegion)//  baseGeometry.CalcDistance(vertex.RelativeToWorld,false,out edge,out normal,out dist))
			{
				returnvalue.VertexIsICollidableBody1 = vertexIsICollidableBody1;
				return returnvalue;
				//return new CollisionPointInfo(edge,normal,vertex.RelativeToWorld,dist,vertexIsICollidableBody1,false);
			}
			return null;
		}
		/// <summary>
		/// Does the Detection between 2 circles.
		/// </summary>
		/// <returns></returns>
		private CollisionInfo CalcCircleCircle()
		{
			// this one is easy.
            Vector2D CollisionPoint = this.part1.Position.Linear - distanceNorm * (this.part1.BaseGeometry.BoundingRadius);
			Vector2D CollisionNormal = distanceNorm;
            Vector2D RelativeVelocity = pair.GetRelativeVelocityAt(CollisionPoint);
			float RelativeVelocityAlongNormal = RelativeVelocity*CollisionNormal;
			if(RelativeVelocityAlongNormal<0)
			{
				float distance = distanceMag-(part1.BaseGeometry.BoundingRadius+part2.BaseGeometry.BoundingRadius);
				return new CollisionInfo(CollisionPoint,CollisionNormal,distance);
            }
			else
			{
				return null;
			}
		}
        private CollisionInfo CalcCircleCircleInnerRadius()
        {
            // this one is easy.
            Vector2D CollisionPoint = this.part1.Position.Linear - distanceNorm * (this.part1.BaseGeometry.InnerRadius);
            Vector2D CollisionNormal = distanceNorm;
            Vector2D RelativeVelocity = pair.GetRelativeVelocityAt(CollisionPoint);
            float RelativeVelocityAlongNormal = RelativeVelocity * CollisionNormal;
            if (RelativeVelocityAlongNormal < 0)
            {
                float distance = distanceMag - (part1.BaseGeometry.InnerRadius + part2.BaseGeometry.InnerRadius);
                return new CollisionInfo(CollisionPoint, CollisionNormal, distance);
            }
            else
            {
                return null;
            }
        }
		/// <summary>
		/// Does the Detection between A polygon and a circle
		/// </summary>
		/// <returns></returns>
		private CollisionInfo CalcPolygonCircle(float dt)
		{
            Polygon2DDistanceInfo GDistanceInfo = Polygon2DforPart2.CalcDistanceInfo(this.part1.Position.Linear);
			if(GDistanceInfo == null)
			{
				return null;
			}
			if(GDistanceInfo.NormalInfo.Distance>part1.BaseGeometry.BoundingRadius)
			{
				return null;
			}
            Vector2D RelativeVelocity;
            float RelativeVelocityProjOnNormal;
            if (GDistanceInfo.NormalInfo.Distance < 0)
            {
                if (switched)
                {
                    RelativeVelocity = -pair.GetRelativeVelocityAt(this.part1.Position.Linear);
                }
                else
                {
                    RelativeVelocity = pair.GetRelativeVelocityAt(this.part1.Position.Linear);
                }
                RelativeVelocityProjOnNormal = RelativeVelocity * distanceNorm;
                if (RelativeVelocityProjOnNormal >= 0)
                {
                    return null;
                }
                if (switched)
                {
                    return new CollisionInfo(this.part1.Position.Linear, -distanceNorm, GDistanceInfo.NormalInfo.Distance);
                }
                else
                {
                    return new CollisionInfo(this.part1.Position.Linear, distanceNorm, GDistanceInfo.NormalInfo.Distance);
                }
            }
			Vector2D Normal = GDistanceInfo.NormalInfo.Normal.Normalized;
			Vector2D pointRelativeToPart1 = Normal*(-part1.BaseGeometry.BoundingRadius);
			Vector2D pointRelativeToWorld = pointRelativeToPart1 + this.part1.Position.Linear;
			Edge2DDistanceInfo EDistanceInfo = GDistanceInfo.ClosestEdge.CalcDistanceInfo(pointRelativeToWorld);
			if(!EDistanceInfo.BehindEdge2D)
			{
				//return null;
			}
			Edge2DNormalInfo NormalInfo = GDistanceInfo.ClosestEdge.CalcNormalInfo(EDistanceInfo,true);

			if(switched)
			{
				RelativeVelocity = -pair.GetRelativeVelocityAt(pointRelativeToWorld);
			}
			else
			{
				RelativeVelocity = pair.GetRelativeVelocityAt(pointRelativeToWorld);
			}
			RelativeVelocityProjOnNormal = RelativeVelocity*Normal ;
			if(RelativeVelocityProjOnNormal>=0)
			{
				return null;
			}
            if (switched)
            {
                return new CollisionInfo(pointRelativeToWorld, -Normal, NormalInfo.Distance);
            }
            else
            {
                return new CollisionInfo(pointRelativeToWorld, Normal, NormalInfo.Distance);
            }
		}
        /// <summary>
        /// Does the Detection between 2 geometries.
        /// </summary>
        /// <returns>the best CollisionInfo</returns>
		private CollisionInfo CalcPolygonPolygon(float dt)
		{
            BoundingBox2D targetArea = BoundingBox2D.SmallestFrom2BoundingBox2Ds(part1.BoundingBox2D, part2.BoundingBox2D);
            Vertex2D[] Vertices1 = Polygon2DforPart1.Vertices;
            Vertex2D[] Vertices2 = Polygon2DforPart2.Vertices;

            int Vertices1Length = Vertices1.Length;
            int Vertices2Length = Vertices2.Length;

            List<CollisionPointInfo> newpointInfos = new List<CollisionPointInfo>(Vertices1Length + Vertices2Length);
			CollisionPointInfo tmp;
            VertexEdgeContactDecider2D vertexEdgeCollisionDecider = null;
            DoBoxlike = Polygon2DforPart1.IsBoxlike && Polygon2DforPart2.IsBoxlike;
            if (DoBoxlike)
            {
                vertexEdgeCollisionDecider = new VertexEdgeContactDecider2D(Polygon2DforPart1, Polygon2DforPart2);
            }
			// populates the newpointInfos info with points that are contacting a edge.
			bool[] goodedgesforvertex = null;
            for (int pos = 0; pos < Vertices1Length; ++pos)
			{
                if (BoundingBox2D.TestIntersection(targetArea, Vertices1[pos].Position))
                {
                    if (vertexEdgeCollisionDecider != null)
                    {
                        goodedgesforvertex = vertexEdgeCollisionDecider.PossibleVertexEdgeCollisionsFor1[pos];
                    }
                    tmp = GetPointInfo(Vertices1[pos], Polygon2DforPart2, true, goodedgesforvertex);
                    if (tmp != null)
                    {
                        newpointInfos.Add(tmp);
                    }
                }
			}
            for (int pos = 0; pos < Vertices2Length; ++pos)
			{
                if (BoundingBox2D.TestIntersection(targetArea, Vertices2[pos].Position))
                {
                    if (vertexEdgeCollisionDecider != null)
                    {
                        goodedgesforvertex = vertexEdgeCollisionDecider.PossibleVertexEdgeCollisionsFor2[pos];
                    }
                    tmp = GetPointInfo(Vertices2[pos], Polygon2DforPart1, false, goodedgesforvertex);
                    if (tmp != null)
                    {
                        newpointInfos.Add(tmp);
                    }
                }
			}
            /*if (false)//DetectEdgeEdge)
            {
                //This is where it chacks to see if we have edge on edge collisions;
                List<CollisionPointInfo> edgepoints = new List<CollisionPointInfo>();
                for (int pos1 = 0; pos1 != newpointInfos.Count; ++pos1)
                {
                    CollisionPointInfo point1 = (CollisionPointInfo)newpointInfos[pos1];
                    for (int pos2 = pos1 + 1; pos2 != newpointInfos.Count; ++pos2)
                    {
                        CollisionPointInfo point2 = (CollisionPointInfo)newpointInfos[pos2];

                        float Ndot = point1.CollisionNormal * point2.CollisionNormal;
                        //checks to see if the edges are very close to Parallel
                        if (Math.Abs(Ndot) > 0.9)
                        {
                            // Checks to see if the 2 points are very close to the same distance from the edge.
                            if (Math.Abs(point1.Distance - point2.Distance) < Physics.CollisionTolerance)
                            {
                                Vector2D newWorldPoint = (point1.WorldPoint - point2.WorldPoint) * .5f + point2.WorldPoint;
                                CollisionPointInfo newpoint = point1;//new CollisionPointInfo(point1.Edge,point1.CollisionNormal,newWorldPoint,point1.Distance,point1.VertexIsICollidableBody1,true);
                                newpoint.WorldPoint = newWorldPoint;
                                newpoint.IsEdgonEdge = true;
                                newpoint.InEdgesVoronoiRegion = true;
                                edgepoints.Add(newpoint);
                                newpointInfos.RemoveAt(pos2);
                                newpointInfos.RemoveAt(pos1);
                                --pos1;
                                break;
                            }
                        }
                    }
                }
                newpointInfos.AddRange(edgepoints);
            }*/
			pointInfos = newpointInfos;
			return CalcCollisionInfo(dt);
		}
        
        /// <summary>
        /// Detects collision between 2 Geometries while treating each vertex as a RaySegment2D with its origin being its location last time step and its end at tis position during this timestep.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>the best CollisionInfo</returns>
        private CollisionInfo CalcPolygonPolygonRayTrace(float dt)//RayTrace
        {
            /*BoundingBox2D targetArea = BoundingBox2D.SmallestFrom2BoundingBox2Ds(part1.BoundingBox2D, part2.BoundingBox2D);
            Vertex2D[] Vertices = Polygon2DforPart1.Vertices;
            int length = Vertices.Length;
            List<Vertex2D> Vertices1 = new List<Vertex2D>(length);
            List<Vertex2D> OldVertices1 = new List<Vertex2D>(length);
            for (int pos = 0; pos < length; ++pos)
            {
                if (BoundingBox2D.TestIntersection(targetArea, Vertices[pos].relativeToWorld))
                {
                    Vertices1.Add(Vertices[pos]);
                    OldVertices1.Add(part1.InitialPolygon2D.Vertices[pos]);
                }
            }
            Vertices = Polygon2DforPart2.Vertices;
            length = Vertices.Length;
            List<Vertex2D> Vertices2 = new List<Vertex2D>(length);
            List<Vertex2D> OldVertices2 = new List<Vertex2D>(length);
            for (int pos = 0; pos < length; ++pos)
            {
                if (BoundingBox2D.TestIntersection(targetArea, Vertices[pos].relativeToWorld))
                {
                    Vertices2.Add(Vertices[pos]);
                    OldVertices2.Add(part2.InitialPolygon2D.Vertices[pos]);
                }
            }*/
            Edge2D goodEdge = null;
            Vertex2D goodVertex = null;
            Ray2DIntersectInfo goodInfo = null;
            float goodTimeAt = 1;
            Vertex2D[] Vertices1 = Polygon2DforPart1.Vertices;
            Vertex2D[] OldVertices1 = part1.InitialPolygon2D.Vertices;
            int length = Vertices1.Length;
            bool isCollidable1 = false;
            for (int pos = 0; pos < length; ++pos)
            {
                RaySegment2D segment = RaySegment2D.From2Points(OldVertices1[pos].Position, Vertices1[pos].Position);
                if (segment != null)
                {
                    foreach (Edge2D edge in Polygon2DforPart2.Edges)
                    {
                        Ray2DIntersectInfo info = IntersectionTests2D.TestIntersection(segment, edge);
                        if (info.Intersects && info.DistanceFromOrigin < segment.Length)
                        {
                            float timesince = info.DistanceFromOrigin / segment.Length;
                            if (timesince < goodTimeAt)
                            {
                                goodEdge = edge;
                                goodVertex = Vertices1[pos];
                                goodInfo = info;
                                isCollidable1 = true;
                            }
                        }
                    }
                }
            }
            Vertex2D[] Vertices2 = Polygon2DforPart2.Vertices;
            Vertex2D[] OldVertices2 = part2.InitialPolygon2D.Vertices;
            length = Vertices2.Length;
            for (int pos = 0; pos < length; ++pos)
            {
                RaySegment2D segment = RaySegment2D.From2Points(OldVertices2[pos].Position, Vertices2[pos].Position);
                if (segment != null)
                {
                    foreach (Edge2D edge in Polygon2DforPart1.Edges)
                    {
                        Ray2DIntersectInfo info = IntersectionTests2D.TestIntersection(segment, edge);
                        if (info.Intersects && info.DistanceFromOrigin < segment.Length)
                        {
                            float timesince = info.DistanceFromOrigin / segment.Length;
                            if (timesince < goodTimeAt)
                            {
                                goodEdge = edge;
                                goodVertex = Vertices2[pos];
                                goodInfo = info;
                                isCollidable1 = false;
                            }
                        }
                    }
                }
            }
            if (goodEdge != null)
            {
                float distance = goodEdge.Normal * (goodVertex.Position - goodEdge.FirstVertex.Position);
                if (isCollidable1)
                {
                    return new CollisionInfo(goodVertex.Position, goodEdge.Normal, distance);
                }
                else
                {
                    return new CollisionInfo(goodVertex.Position, -goodEdge.Normal, distance);
                }
            }
            return null;
        }
		/// <summary>
		/// Runs the tests on the collidables to see if they are colliding.
		/// </summary>
		/// <param name="dt"></param>
        public CollisionInfo TestCollisions(float dt)
		{
			bestCollisionInfo = null;
            if (parameters.DoInnerRadiusTests)
            {
                bestCollisionInfo = CalcCircleCircleInnerRadius();
            }
            else
            {
                switch (collisionType)
                {
                    case CollisionType.CircleCircle:
                        bestCollisionInfo = CalcCircleCircle();
                        break;
                    case CollisionType.PolygonCircle:
                        bestCollisionInfo = CalcPolygonCircle(dt);
                        break;
                    case CollisionType.PolygonPolygon:
                        if (parameters.DoRayTrace && (this.RelativeLinearVelocity * this.DistanceNorm) > parameters.RayTraceVelocityTolerance)
                        {
                            bestCollisionInfo = CalcPolygonPolygonRayTrace(dt);
                        }
                        else
                        {
                            bestCollisionInfo = CalcPolygonPolygon(dt);
                        }
                        break;
                    default:
                        throw new Exception("What The Hell??: unknown collisionType");
                }
            }
			IsValid = bestCollisionInfo != null;
            return bestCollisionInfo;
		}
		#endregion
	}
}
