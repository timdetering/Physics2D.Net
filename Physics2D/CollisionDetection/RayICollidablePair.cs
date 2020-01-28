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
namespace Physics2D.CollisionDetection
{
    [Serializable]
    public sealed class RayICollidableBodyPair
    {
        RaySegment2D raySegment;
        ICollidableBody collidable;
        RayICollidableBodyPartPair[] parts;
        public bool IsValid = false;
        Ray2DIntersectInfo bestIntersectInfo = null;
        public RayICollidableBodyPair(RaySegment2D raySegment, ICollidableBody collidable)
        {
            this.raySegment = raySegment;
            this.collidable = collidable;
            int length = collidable.CollidableParts.Length;
            parts = new RayICollidableBodyPartPair[length];
            for (int pos = 0; pos < length; ++pos)
            {
                parts[pos] = new RayICollidableBodyPartPair(raySegment, collidable.CollidableParts[pos]);
            }
        }
        private bool TestBoundingBox()
        {
            IsValid = IsValid || IntersectionTests2D.TestIntersection(raySegment, collidable.BoundingBox2D).Intersects;
            return IsValid;
        }
        private bool TestCircle2D()
        {
            IsValid = IsValid||IntersectionTests2D.TestIntersection(raySegment, collidable.ToCircle2D(),false).Intersects;
            return IsValid;
        }
        private bool TestPartsCircle2Ds()
        {
            if (IsValid)
            {
                IsValid = false;
                foreach (RayICollidableBodyPartPair partpair in parts)
                {
                    IsValid = partpair.TestCircle2D() || IsValid;
                }
            }
            return IsValid;
        }
        private bool TestPartsTestBoundingBox2Ds()
        {
            if (IsValid)
            {
                IsValid = false;
                foreach (RayICollidableBodyPartPair partpair in parts)
                {
                    IsValid = partpair.TestBoundingBox2D() || IsValid;
                }
            }
            return IsValid;
        }
        public bool TestIntersection()
        {
            IsValid = false;
            if (TestBoundingBox() && TestCircle2D() && TestPartsCircle2Ds())
            {
                IsValid = false;
                float mindistance = raySegment.Length;
                foreach (RayICollidableBodyPartPair partpair in parts)
                {
                    if (partpair.IntersectInfo.Intersects)// && mindistance > partpair.IntersectInfo.DistanceFromOrigin)
                    {
                        if (partpair.CollidablePart.UseCircleCollision)
                        {
                            if (mindistance > partpair.IntersectInfo.DistanceFromOrigin)
                            {
                                mindistance = partpair.IntersectInfo.DistanceFromOrigin;
                                bestIntersectInfo = partpair.IntersectInfo;
                                IsValid = true;
                            }
                        }
                        else if (partpair.TestPolygon2D() && mindistance > partpair.IntersectInfo.DistanceFromOrigin)
                        {
                            mindistance = partpair.IntersectInfo.DistanceFromOrigin;
                            bestIntersectInfo = partpair.IntersectInfo;
                            IsValid = true;
                        }
                    }
                }
            }
            return IsValid;
        }
        public Ray2DIntersectInfo BestIntersectInfo
        {
            get
            {
                return bestIntersectInfo;
            }
        }
        public ICollidableBody ICollidableBody
        {
            get
            {
                return collidable;
            }
        }
        public RaySegment2D RaySegment2D
        {
            get
            {
                return raySegment;
            }
        }
    }
}