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
#region ORGE LGPL License
/*
-----------------------------------------------------------------------------
This source file is part of OGRE
    (Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org/

Copyright (c) 2000-2005-2006 The OGRE Team
Also see acknowledgements in Readme.html

This program is free software; you can redistribute it and/or modify it under
the terms of the GNU Lesser General Public License as published by the Free Software
Foundation; either version 2 of the License, or (at your option) any later
version.

This program is distributed in the hope that it will be useful, but WITHOUT
ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with
this program; if not, write to the Free Software Foundation, Inc., 59 Temple
Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/lesser.txt.
-----------------------------------------------------------------------------
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
namespace AdvanceMath.Geometry2D
{
    /// <summary>
    /// A static class full of methods for intersection tests.
    /// </summary>
    /// <remarks>
    /// all code written for Physics 2D.
    /// </remarks>
    public partial class IntersectionTests2D
    {
        private IntersectionTests2D() { }
        public static Ray2DIntersectInfo TestIntersection(Ray2D ray, Edge2D[] edges)
        {
            Ray2DIntersectInfo returnvalue = null;
            foreach (Edge2D edge in edges)
            {
                Ray2DIntersectInfo info = TestIntersection(ray, edge);
                if (info.Intersects && (returnvalue == null || returnvalue.DistanceFromOrigin > info.DistanceFromOrigin))
                {
                    returnvalue = info;
                }
            }
            if (returnvalue == null)
            {
                returnvalue = new Ray2DIntersectInfo(false, 0);
            }
            return returnvalue;
        }
        public static Ray2DIntersectInfo TestIntersectionOLD(Ray2D ray, Edge2D[] edges)
        {
            List<Ray2DIntersectInfo> results = new List<Ray2DIntersectInfo>();
            foreach (Edge2D edge in edges)
            {
                results.Add(TestIntersection(ray, edge));
            }
            int lenght = edges.Length;
            Ray2DIntersectInfo returnvalue = null;
            foreach (Ray2DIntersectInfo info in results)
            {
                if (info.Intersects && (returnvalue == null || returnvalue.DistanceFromOrigin > info.DistanceFromOrigin))
                {
                    returnvalue = info;
                }
            }
            if (returnvalue == null)
            {
                return results[0];
            }
            return returnvalue;
        }
        public static Ray2DIntersectInfo TestIntersection(Ray2D ray, Edge2D edge)
        {
            Scalar dir = edge.Normal * ray.Direction;
            if (Math.Abs(dir) < MathAdv.Tolerance)
            {
                return new Ray2DIntersectInfo(false, 0);
            }
            else
            {
                Vector2D originDiff = ray.Origin - edge.SecondVertex.Position;
                Scalar actualDistance = edge.Normal * originDiff;
                Scalar DistanceFromOrigin = -(actualDistance / dir);
                if (DistanceFromOrigin >= 0)
                {
                    Vector2D intersectPos = originDiff + ray.Direction * DistanceFromOrigin;
                    Scalar distanceFromSecond = intersectPos * edge.NormalizedEdge;
                    return new Ray2DIntersectInfo(distanceFromSecond >= 0 && distanceFromSecond <= edge.Magnitude, DistanceFromOrigin);
                }
                else
                {
                    return new Ray2DIntersectInfo(false, 0);
                }
            }
        }
    }
    /// <summary>
    /// a static class full of methods for intersection tests.
    /// </summary>
    /// <remarks>
    /// all code adapted from ORGE Ogre.Math.intersects method (LGPL) 
    ///	http://www.ogre3d.org/ 
    /// </remarks>
    public partial class IntersectionTests2D
    {
        public static Ray2DIntersectInfo TestIntersection(Ray2D ray, Line2D line)
        {
            Scalar dir = line.Normal * ray.Direction;
            if (-dir <= 0)
            {
                return new Ray2DIntersectInfo(false, 0);
            }
            else
            {
                Scalar actualDistance = line.Normal * ray.Origin + line.NDistance;
                Scalar DistanceFromOrigin = -(actualDistance / dir);
                return new Ray2DIntersectInfo(DistanceFromOrigin >= 0, DistanceFromOrigin);
            }
        }
        public static Ray2DIntersectInfo TestIntersection(Ray2D ray, Line2D[] lines, bool normalIsOutside)
        {
            //std::list<line>::const_iterator lineit, lineitend;
            //lineitend = lines.end();
            bool allInside = true;
            //Ray2DIntersectInfo possibleCollisions;
            bool retfirst = false;
            Scalar retsecond = 0.0f;

            // derive side
            // NB we don't pass directly since that would require line::Side in 
            // interface, which results in recursive includes since Math is so fundamental
            LineSide outside = normalIsOutside ? LineSide.Positive : LineSide.Negitive;

            foreach (Line2D line in lines)
            {

                // is origin outside?
                if (Line2D.CalcLineSide(line, ray.Origin) == outside)
                {
                    allInside = false;
                    // Test single line
                    Ray2DIntersectInfo lineRes = TestIntersection(ray, line);
                    if (lineRes.Intersects)
                    {
                        // Ok, we intersected
                        retfirst = true;
                        // Use the most distant result since convex volume
                        retsecond = Math.Max(retsecond, lineRes.DistanceFromOrigin);
                    }
                }
            }
            if (allInside)
            {
                // Intersecting at 0 distance since inside the volume!
                retfirst = true;
                retsecond = 0.0f;
            }
            return new Ray2DIntersectInfo(retfirst, retsecond);
        }
        public static Ray2DIntersectInfo TestIntersection(Ray2D ray, Circle2D circle, bool discardInside)
        {
            Vector2D rayDirection = ray.Direction;
            Vector2D rayOriginRelativeToCircle2D = ray.Origin - circle.Center;
            Scalar radiusSq = circle.Radius;
            radiusSq *= radiusSq;
            Scalar MagSq = rayOriginRelativeToCircle2D.MagnitudeSq;
            // Check origin inside first
            if ((MagSq <= radiusSq) && discardInside)
            {
                return new Ray2DIntersectInfo(true, 0);
            }
            Scalar a = rayDirection.MagnitudeSq;
            Scalar b = 2 * rayOriginRelativeToCircle2D * rayDirection;
            Scalar c = MagSq - radiusSq;
            Scalar minus;
            Scalar plus;
            if (MathAdv.TrySolveQuadratic(a, b, c, out plus, out minus))
            {
                if (minus < 0)
                {
                    if (plus > 0)
                    {
                        return new Ray2DIntersectInfo(true, plus);
                    }
                    else
                    {
                        return new Ray2DIntersectInfo(false, 0);
                    }
                }
                else
                {

                    return new Ray2DIntersectInfo(true, minus);
                }
            }
            else
            {
                return new Ray2DIntersectInfo(false, 0);
            }

        }
        public static Ray2DIntersectInfo TestIntersection(Ray2D ray, BoundingBox2D box)
        {
            Vector2D rayorig = ray.Origin;


            if (BoundingBox2D.TestIntersection(box, rayorig))
            {
                return new Ray2DIntersectInfo(true, 0);
            }

            Scalar DistanceFromOrigin = 0.0f;
            Scalar tmpDistance;
            bool Intersects = false;
            Vector2D Intersectspoint;
            Vector2D Lower = box.Lower;
            Vector2D Upper = box.Upper;
            Vector2D raydir = ray.Direction;

            // Check each face in turn, only check closest 3
            // Min x
            if (rayorig.X < Lower.X && raydir.X > 0)
            {
                tmpDistance = (Lower.X - rayorig.X) / raydir.X;
                if (tmpDistance > 0)
                {
                    // Substitute t back into raySegment and check bounds and dist
                    Intersectspoint = rayorig + raydir * tmpDistance;
                    if (Intersectspoint.Y >= Lower.Y && Intersectspoint.Y <= Upper.Y &&
                        //Intersectspoint.Z >= Lower.Z && Intersectspoint.Z <= Upper.Z &&
                        (!Intersects || tmpDistance < DistanceFromOrigin))
                    {
                        Intersects = true;
                        DistanceFromOrigin = tmpDistance;
                    }
                }
            }
            // Max x
            if (rayorig.X > Upper.X && raydir.X < 0)
            {
                tmpDistance = (Upper.X - rayorig.X) / raydir.X;
                if (tmpDistance > 0)
                {
                    // Substitute t back into raySegment and check bounds and dist
                    Intersectspoint = rayorig + raydir * tmpDistance;
                    if (Intersectspoint.Y >= Lower.Y && Intersectspoint.Y <= Upper.Y &&
                       // Intersectspoint.Z >= Lower.Z && Intersectspoint.Z <= Upper.Z &&
                        (!Intersects || tmpDistance < DistanceFromOrigin))
                    {
                        Intersects = true;
                        DistanceFromOrigin = tmpDistance;
                    }
                }
            }
            // Min y
            if (rayorig.Y < Lower.Y && raydir.Y > 0)
            {
                tmpDistance = (Lower.Y - rayorig.Y) / raydir.Y;
                if (tmpDistance > 0)
                {
                    // Substitute t back into raySegment and check bounds and dist
                    Intersectspoint = rayorig + raydir * tmpDistance;
                    if (Intersectspoint.X >= Lower.X && Intersectspoint.X <= Upper.X &&
                        //Intersectspoint.Z >= Lower.Z && Intersectspoint.Z <= Upper.Z &&
                        (!Intersects || tmpDistance < DistanceFromOrigin))
                    {
                        Intersects = true;
                        DistanceFromOrigin = tmpDistance;
                    }
                }
            }
            // Max y
            if (rayorig.Y > Upper.Y && raydir.Y < 0)
            {
                tmpDistance = (Upper.Y - rayorig.Y) / raydir.Y;
                if (tmpDistance > 0)
                {
                    // Substitute t back into raySegment and check bounds and dist
                    Intersectspoint = rayorig + raydir * tmpDistance;
                    if (Intersectspoint.X >= Lower.X && Intersectspoint.X <= Upper.X &&
                        //Intersectspoint.Z >= Lower.Z && Intersectspoint.Z <= Upper.Z &&
                        (!Intersects || tmpDistance < DistanceFromOrigin))
                    {
                        Intersects = true;
                        DistanceFromOrigin = tmpDistance;
                    }
                }
            }
            /*
            // Min z
            if (rayorig.Z < Lower.Z && raydir.Z > 0)
            {
                tmpDistance = (Lower.Z - rayorig.Z) / raydir.Z;
                if (tmpDistance > 0)
                {
                    // Substitute t back into raySegment and check bounds and dist
                    Intersectspoint = rayorig + raydir * tmpDistance;
                    if (Intersectspoint.X >= Lower.X && Intersectspoint.X <= Upper.X &&
                        Intersectspoint.Y >= Lower.Y && Intersectspoint.Y <= Upper.Y &&
                        (!Intersects || tmpDistance < DistanceFromOrigin))
                    {
                        Intersects = true;
                        DistanceFromOrigin = tmpDistance;
                    }
                }
            }
            // Max z
            if (rayorig.Z > Upper.Z && raydir.Z < 0)
            {
                tmpDistance = (Upper.Z - rayorig.Z) / raydir.Z;
                if (tmpDistance > 0)
                {
                    // Substitute t back into raySegment and check bounds and dist
                    Intersectspoint = rayorig + raydir * tmpDistance;
                    if (Intersectspoint.X >= Lower.X && Intersectspoint.X <= Upper.X &&
                        Intersectspoint.Y >= Lower.Y && Intersectspoint.Y <= Upper.Y &&
                        (!Intersects || tmpDistance < DistanceFromOrigin))
                    {
                        Intersects = true;
                        DistanceFromOrigin = tmpDistance;
                    }
                }
            }*/
            return new Ray2DIntersectInfo(Intersects, DistanceFromOrigin);
        }
        public static bool TestIntersection(Circle2D circle, BoundingBox2D box)
        {
            //if (box.isNull()) return false;

            // Use splitting lines
            Vector2D center = circle.Center;
            Scalar radius = circle.Radius;
            Vector2D min = box.Lower;
            Vector2D max = box.Upper;

            // just test facing lines, early fail if circle is totally outside
            if (center.X < min.X &&
                min.X - center.X > radius)
            {
                return false;
            }
            if (center.X > max.X &&
                center.X - max.X > radius)
            {
                return false;
            }

            if (center.Y < min.Y &&
                min.Y - center.Y > radius)
            {
                return false;
            }
            if (center.Y > max.Y &&
                center.Y - max.Y > radius)
            {
                return false;
            }

            /*if (center.Z < min.Z &&
                min.Z - center.Z > radius)
            {
                return false;
            }
            if (center.Z > max.Z &&
                center.Z - max.Z > radius)
            {
                return false;
            }*/

            // Must intersect
            return true;
        }
        public static bool TestIntersection(Line2D line, BoundingBox2D box)
        {
            // Get corners of the box
            Vector2D[] pCorners = box.Corners;


            // Test which side of the line the corners are
            // Intersection occurs when at least one corner is on the 
            // opposite side to another

            LineSide lastSide = Line2D.CalcLineSide(line, pCorners[0]);
            for (int corner = 1; corner < 8; ++corner)
            {
                if (Line2D.CalcLineSide(line, pCorners[corner]) != lastSide)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool TestIntersection(Circle2D circle, Line2D line)
        {
            return (Math.Abs(line.Normal * circle.Center) <= circle.Radius);
        }
    }
}
