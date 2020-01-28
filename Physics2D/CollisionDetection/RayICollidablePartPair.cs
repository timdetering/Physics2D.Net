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
    public sealed class RayICollidableBodyPartPair
    {
        Ray2D ray;
        ICollidableBodyPart part;
        Ray2DIntersectInfo info = null;
        public RayICollidableBodyPartPair(Ray2D ray, ICollidableBodyPart part)
        {
            this.ray = ray;
            this.part = part;
        }
        public bool TestBoundingBox2D()
        {
            if (info == null || info.Intersects)
            {
                info = IntersectionTests2D.TestIntersection(ray, part.BoundingBox2D);
            }
            return info.Intersects;
        }
        public bool TestCircle2D()
        {
            if (info == null || info.Intersects)
            {
                if (part.UseCircleCollision)
                {
                    info = IntersectionTests2D.TestIntersection(ray, new Circle2D(part.BaseGeometry.BoundingRadius, part.Position.Linear), true);
                }
                else
                {
                    info = IntersectionTests2D.TestIntersection(ray, new Circle2D(part.BaseGeometry.BoundingRadius, part.Position.Linear), false);
                }
            }
            return info.Intersects;
        }
        public bool TestPolygon2D()
        {
            if (info == null || info.Intersects)
            {
                info = IntersectionTests2D.TestIntersection(ray, part.Polygon2D.Edges);
            }
            return info.Intersects;
        }
        public Ray2DIntersectInfo IntersectInfo
        {
            get
            {
                return info;
            }
        }
        public ICollidableBodyPart CollidablePart
        {
            get
            {
                return part;
            }
        }
        public Ray2D Ray2D
        {
            get
            {
                return ray;
            }
        }
    }
}