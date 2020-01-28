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
using AdvanceMath.Geometry2D;
using AdvanceMath;
namespace Physics2D.CollisionDetection 
{
    [Serializable]
    public sealed class CollisionPairParameters
    {
        /// <summary>
        /// (NOT USED BY PHYSICS2D)
        /// </summary>
        public bool TestEdgeonEdge;
        public bool DoRayTrace;
        public bool DoInnerRadiusTests;
        public float RayTraceVelocityTolerance; 
    }
	/// <summary>
	/// This class does the actaul finite detection. 
	/// </summary>
    [Serializable]
    public sealed class CollisionPair : CollidablePair
    {
        #region fields
        CollisionPartPair[,] collisionPartPairs;
        CollisionInfo bestCollisionInfo = null;
        Physics2D.Coefficients coefficients = null;
        int collisionLevel = 0;
        public bool IsValid = false;
        #endregion
        #region constructors
        public CollisionPair(ICollidableBody body1, ICollidableBody body2, CollisionPairParameters parameters)
            : base(body1, body2)
        {
            this.collidable1 = body1;
            this.collidable2 = body2;
            ICollidableBodyPart[] parts1 = body1.CollidableParts;
            ICollidableBodyPart[] parts2 = body2.CollidableParts;
            int partLength1 = parts1.Length;
            int partLength2 = parts2.Length;
            this.collisionPartPairs = new CollisionPartPair[partLength1, partLength2];
            if (partLength1 == 1 && 1 == partLength2)
            {
                //Dont do the extra checking needed for multiple part objects.
                this.collisionPartPairs[0, 0] = new CollisionPartPair(this, parts1[0], parts2[0], parameters);
            }
            else
            {
                //Makes sure only objects in the area where the bounding boxes are overlapping are checked.
                BoundingBox2D CollisionArea = BoundingBox2D.SmallestFrom2BoundingBox2Ds(body1.SweepBoundingBox2D, body2.SweepBoundingBox2D);
                bool[] parts2tests = new bool[partLength2];
                for (int pos2 = 0; pos2 < partLength2; ++pos2)
                {
                    parts2tests[pos2] =  CollisionArea.TestIntersection(parts2[pos2].BoundingBox2D);
                }
                for (int pos1 = 0; pos1 < partLength1; ++pos1)
                {
                    if ( CollisionArea.TestIntersection(parts1[pos1].BoundingBox2D))
                    {
                        for (int pos2 = 0; pos2 < partLength2; ++pos2)
                        {
                            if (parts2tests[pos2])
                            {
                                this.collisionPartPairs[pos1, pos2] = new CollisionPartPair(this, parts1[pos1], parts2[pos2], parameters);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region properties

        public Coefficients Coefficients
        {
            get
            {
                return coefficients;
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
            set
            {
                bestCollisionInfo = value;
            }
        }
        public int CollisionLevel
        {
            get
            {
                return collisionLevel;
            }
            set
            {
                collisionLevel = value;
            }
        }
        #endregion
        #region methods
        /// <summary>
        /// This calculates all the variables that have the word distance in them.
        /// </summary>
        public void CalcDistance()
		{
            foreach (CollisionPartPair pair in collisionPartPairs)
            {
                if (pair!=null&&pair.IsValid)
                {
                    pair.CalcDistance();
                }
            }
		}
		/// <summary>
		/// Tests the bounding radius to see if the objects are in each others bounding circle.
		/// </summary>
		/// <returns>true if the bounding circles overlap; otherwise false.</returns>
		public bool TestBoundingRadius()
		{
            bool returnvalue = false;
            foreach (CollisionPartPair pair in collisionPartPairs)
            {
                if (pair != null && pair.IsValid)
                {
                    returnvalue = pair.TestBoundingRadius() || returnvalue;
                }
            }
            return returnvalue;
		}
		/// <summary>
		/// Tests the BoundingBox2Des to see if the objects are in each others BoundingBox2D.
		/// </summary>
		/// <returns>true if the bounding boxes overlap; otherwise false.</returns>
		public bool TestBoundingBox2Ds()
		{
            if (collisionPartPairs.Length == 1)
            {
                return this.Collidable1.BoundingBox2D.TestIntersection(this.Collidable2.BoundingBox2D);
            }
            else
            {
                if (this.Collidable1.BoundingBox2D.TestIntersection(this.Collidable2.BoundingBox2D))
                {
                    bool returnvalue = false;
                    foreach (CollisionPartPair pair in collisionPartPairs)
                    {
                        if (pair != null && pair.IsValid)
                        {
                            returnvalue = pair.TestBoundingBox2Ds() || returnvalue;
                        }
                    }
                    return returnvalue;
                }
                return false;
            }
		}

		/// <summary>
		/// Gets the Relative velocity with angular calculations.
		/// </summary>
		public Vector2D GetRelativeVelocityAt(Vector2D pointRelativeToWorld)
		{
			return collidable1.GetVelocityAtWorld(pointRelativeToWorld)-collidable2.GetVelocityAtWorld(pointRelativeToWorld);
			//return collidable1.Current.GetVelocityAtWorld(pointRelativeToWorld)-collidable2.Current.GetVelocityAtWorld(pointRelativeToWorld);
		}
        /// <summary>
        /// Does the Final Collision detection
        /// </summary>
        /// <param name="dt">change in time.</param>
        /// <returns>true if they collid; otherwise false.</returns>
        public bool TestCollisions(float dt)
		{
            bestCollisionInfo = null;
            foreach (CollisionPartPair pair in collisionPartPairs)
            {
                if (pair!=null&&pair.IsValid)
                {
                    CollisionInfo info = pair.TestCollisions(dt);
                    if (info != null)
                    {
                        if (bestCollisionInfo == null)
                        {
                            bestCollisionInfo = info;
                            coefficients = pair.Coefficients;
                        }
                        else
                        {
                            if (bestCollisionInfo.Distance < info.Distance)
                            {
                                bestCollisionInfo = info;
                                coefficients = pair.Coefficients;
                            }
                        }
                    }
                }
            }
            IsValid = bestCollisionInfo != null;
            return IsValid;
		}
        /// <summary>
        /// Calculates the Collision Level.
        /// </summary>
		public void CalcCollisionLevel()
		{
			collisionLevel = Math.Min(collidable1.CollisionState.CollisionLevel,collidable2.CollisionState.CollisionLevel);
		}
		/// <summary>
		/// This is used to freeze the object whos side is closer to the gravity source or is farther in the diection of gravity.
		/// </summary>
		public void FreezeLowerLevel()
		{
            collidable1.CollisionState.Frozen = collidable1.CollisionState.CollisionLevel < collidable2.CollisionState.CollisionLevel;
            collidable2.CollisionState.Frozen = !collidable1.CollisionState.Frozen;

			/*if((!collidable1.CollisionState.Frozen)&&(!collidable2.CollisionState.Frozen))
			{
				if(collidable1.CollisionState.CollisionLevel!=collidable2.CollisionState.CollisionLevel)
				{
					if(collidable1.CollisionState.CollisionLevel<collidable2.CollisionState.CollisionLevel)
					{
						collidable1.CollisionState.Frozen = true;
					}
					else
					{
						collidable2.CollisionState.Frozen = true;
					}
				}
				else
				{
					if(IsValid)
					{
						Vector2D accel = this.Collidable1.Current.AccelerationDoToGravity+this.Collidable2.Current.AccelerationDoToGravity;
						int accelDirection = Math.Sign(Math.Round( BestCollisionInfo.CollisionNormal*accel,2));
						switch(accelDirection)
						{
							case 1:
								collidable1.CollisionState.Frozen = true;
								break;
							case -1:
								collidable2.CollisionState.Frozen = true;
								break;
							default:
								break;
						}
					}
				}
			}*/
		}
        #endregion
    }
}
