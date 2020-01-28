#region LGPL License
/*
Axiom Game Engine Library
Copyright (C) 2003  Axiom Project Team

The overall design, and a majority of the core engine and rendering code 
contained within this library is a derivative of the open source Object Oriented 
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.  
Many thanks to the OGRE team for maintaining such a high quality project.

The math library included in this project, in addition to being a derivative of
the works of Ogre, also include derivative work of the free portion of the 
Wild Magic mathematics source code that is distributed with the excellent
book Game Engine Design.
http://www.Wild-magic.com/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Runtime.CompilerServices;


namespace AdvanceMath
{

    public sealed partial class MathAdv
    {
        public const Scalar E = (Scalar)Math.E;
        public const Scalar PI = (Scalar)Math.PI;
        public const Scalar TWO_PI = (Scalar)(Math.PI * 2);
        public const Scalar HALF_PI = (Scalar)(Math.PI / 2);
        public const Scalar HALF_THREE_PI = (Scalar)((2 * Math.PI) / 3);


        public static Scalar InvSqrt(Scalar number)
        {
            return 1 / Sqrt(number);
        }

        public static Scalar Max(params Scalar[] vals)
        {
            if (vals == null)
            {
                throw new ArgumentNullException("vals");
            }
            if (vals.Length == 0)
            {
                throw new ArgumentException("There must be at least one value to compare", "vals");
            }
            Scalar max = vals[0];
            if (Scalar.IsNaN(max)) { return max; }
            for (int i = 1; i < vals.Length; i++)
            {
                Scalar val = vals[i];
                if (val > max) { max = val; }
                else if (Scalar.IsNaN(val)) { return val; }
            }
            return max;
        }
        public static Scalar Min(params Scalar[] vals)
        {
            if (vals == null)
            {
                throw new ArgumentNullException("vals");
            }
            if (vals.Length == 0)
            {
                throw new ArgumentException("There must be at least one value to compare", "vals");
            }
            Scalar min = vals[0];
            if (Scalar.IsNaN(min)) { return min; }
            for (int i = 1; i < vals.Length; i++)
            {
                Scalar val = vals[i];
                if (val < min) { min = val; }
                else if (Scalar.IsNaN(val)) { return val; }
            }
            return min;
        }

        public static bool PointInTri2D(Vector2D p, Vector2D a, Vector2D b, Vector2D c)
        {
            bool bClockwise = (((b - a) ^ (p - b)) >= 0);
            return !(((((c - b) ^ (p - c)) >= 0) ^ bClockwise) && ((((a - c) ^ (p - a)) >= 0) ^ bClockwise));
        }
        #region System.Math Methods
        public static Scalar Abs(Scalar value) { return Math.Abs(value); }
        public static int Sign(Scalar value) { return Math.Sign(value); }
        public static Scalar Max(Scalar val1, Scalar val2)
        {
            if (val1 > val2) { return val1; }
            if (Scalar.IsNaN(val1)) { return val1; }
            return val2;
        }
        public static Scalar Min(Scalar val1, Scalar val2)
        {
            if (val1 < val2) { return val1; }
            if (Scalar.IsNaN(val1)) { return val1; }
            return val2;
        }




        public static Scalar Acos(Scalar d) { return (Scalar)Math.Acos(d); }
        public static Scalar Asin(Scalar d) { return (Scalar)Math.Asin(d); }
        public static Scalar Atan(Scalar d) { return (Scalar)Math.Atan(d); }
        public static Scalar Atan2(Scalar y, Scalar x) { return (Scalar)Math.Atan2(y, x); }
        public static Scalar Ceiling(Scalar a) { return (Scalar)Math.Ceiling(a); }
        public static Scalar Cos(Scalar d) { return (Scalar)Math.Cos(d); }
        public static Scalar Cosh(Scalar value) { return (Scalar)Math.Cosh(value); }
        public static Scalar Exp(Scalar d) { return (Scalar)Math.Exp(d); }
        public static Scalar Floor(Scalar d) { return (Scalar)Math.Floor(d); }
        public static Scalar IEEERemainder(Scalar x, Scalar y) { return (Scalar)Math.IEEERemainder(x, y); }
        public static Scalar Log(Scalar d) { return (Scalar)Math.Log(d); }
        public static Scalar Log(Scalar a, Scalar newBase) { return (Scalar)Math.Log(a, newBase); }
        public static Scalar Log10(Scalar d) { return (Scalar)Math.Log10(d); }
        public static Scalar Pow(Scalar x, Scalar y) { return (Scalar)Math.Pow(x, y); }
        public static Scalar Round(Scalar a) { return (Scalar)Math.Round(a); }
        public static Scalar Round(Scalar value, int digits) { return (Scalar)Math.Round(value, digits); }
        public static Scalar Round(Scalar value, MidpointRounding mode) { return (Scalar)Math.Round(value, mode); }
        public static Scalar Round(Scalar value, int digits, MidpointRounding mode) { return (Scalar)Math.Round(value, digits, mode); }
        public static Scalar Sin(Scalar a) { return (Scalar)Math.Sin(a); }
        public static Scalar Sinh(Scalar value) { return (Scalar)Math.Sinh(value); }
        public static Scalar Sqrt(Scalar d) { return (Scalar)Math.Sqrt(d); }
        public static Scalar Tan(Scalar a) { return (Scalar)Math.Tan(a); }
        public static Scalar Tanh(Scalar value) { return (Scalar)Math.Tanh(value); }
        public static Scalar Truncate(Scalar d) { return (Scalar)Math.Truncate(d); } 
        #endregion
    }


    /// <summary>
    /// This is a class which exposes static methods for various common math functions.  Currently,
    /// the methods simply wrap the methods of the System.Math class (with the exception of a few added extras).
    /// This is in case the implementation needs to be swapped out with a faster C++ implementation, if
    /// deemed that the System.Math methods are not up to far speed wise.
    /// </summary>
    /// TODO: Add overloads for all methods for all instrinsic data types (i.e. Scalar, short, etc).
    public sealed partial class MathAdv //Axiom Stuff
    {
        /// <summary>
        ///		Empty private constructor.  This class has nothing but static methods/properties, so a public default
        ///		constructor should not be created by the compiler.  This prevents instance of this class from being
        ///		created.
        /// </summary>
        private MathAdv() { }
        static Random random = new Random();
        #region Constant


        public const Scalar RADIANS_PER_DEGREE = (Scalar)(PI / 180.0);
        public const Scalar DEGREES_PER_RADIAN = (Scalar)(180.0f / PI);

        #endregion
        #region Static Methods


        /// <summary>
        ///		Converts degrees to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Scalar DegreesToRadians(Scalar degrees)
        {
            return degrees * RADIANS_PER_DEGREE;
        }
        /// <summary>
        ///		Converts radians to degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Scalar RadiansToDegrees(Scalar radians)
        {
            return radians * DEGREES_PER_RADIAN;
        }
        /// <summary>
        ///		Calculate a face normal, including the w component which is the offset from the origin.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector4D CalculateFaceNormal(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            Vector3D normal = CalculateBasicFaceNormal(v1, v2, v3);

            // Now set up the w (distance of tri from origin
            return new Vector4D(normal.X, normal.Y, normal.Z, -(normal*v1));
        }
        /// <summary>
        ///		Calculate a face normal, no w-information.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3D CalculateBasicFaceNormal(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            return Vector3D.Normalize((v2 - v1) ^ (v3 - v1));
        }

        /// <summary>
        ///    Calculates the tangent space vector for a given set of positions / texture coords.
        /// </summary>
        /// <remarks>
        ///    Adapted from bump mapping tutorials at:
        ///    http://www.paulsprojects.net/tutorials/simplebump/simplebump.html
        ///    author : paul.baker@univ.ox.ac.uk
        /// </remarks>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        /// <param name="position3"></param>
        /// <param name="u1"></param>
        /// <param name="v1"></param>
        /// <param name="u2"></param>
        /// <param name="v2"></param>
        /// <param name="u3"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3D CalculateTangentSpaceVector(
            Vector3D position1, Vector3D position2, Vector3D position3, Scalar u1, Scalar v1, Scalar u2, Scalar v2, Scalar u3, Scalar v3)
        {

            // side0 is the vector along one side of the triangle of vertices passed in, 
            // and side1 is the vector along another side. Taking the cross product of these returns the normal.
            Vector3D side0 = position1 - position2;
            Vector3D side1 = position3 - position1;
            // Calculate face normal
            Vector3D normal = Vector3D.Normalize(side1^side0);
            
            // Now we use a formula to calculate the tangent. 
            Scalar deltaV0 = v1 - v2;
            Scalar deltaV1 = v3 - v1;
            Vector3D tangent = Vector3D.Normalize(deltaV1 * side0 - deltaV0 * side1);


            // Calculate binormal
            Scalar deltaU0 = u1 - u2;
            Scalar deltaU1 = u3 - u1;
            Vector3D binormal = Vector3D.Normalize(deltaU1 * side0 - deltaU0 * side1);


            // Now, we take the cross product of the tangents to get a vector which 
            // should point in the same direction as our normal calculated above. 
            // If it points in the opposite direction (the dot product between the normals is less than zero), 
            // then we need to reverse the s and t tangents. 
            // This is because the triangle has been mirrored when going from tangent space to object space.
            // reverse tangents if necessary.
            Vector3D tangentCross = tangent^binormal;
            if ((tangentCross*normal) < 0.0f)
            {
                tangent = -tangent;
                binormal = -binormal;
            }

            return tangent;
        }

        public static bool FloatEqual(Scalar a, Scalar b)
        {
            return FloatEqual(a, b, .00001f);
        }
        public static bool FloatEqual(Scalar a, Scalar b, Scalar tolerance)
        {
            if (Math.Abs(b - a) <= tolerance)
            {
                return true;
            }

            return false;
        }

        public static Scalar RangeRandom(Scalar min, Scalar max)
        {
            return (max - min) * UnitRandom() + min;
        }
        public static Scalar UnitRandom()
        {
            return (Scalar)random.Next(Int32.MaxValue) / (Scalar)Int32.MaxValue;
        }
        public static Scalar SymmetricRandom()
        {
            return 2.0f * UnitRandom() - 1.0f;
        }
        public static bool PointInTri2D(Scalar px, Scalar py, Scalar ax, Scalar ay, Scalar bx, Scalar by, Scalar cx, Scalar cy)
        {
            Scalar v1x, v2x, v1y, v2y;
            bool bClockwise;

            v1x = bx - ax;
            v1y = by - ay;

            v2x = px - bx;
            v2y = py - by;

            bClockwise = (v1x * v2y - v1y * v2x >= 0.0);

            v1x = cx - bx;
            v1y = cy - by;

            v2x = px - cx;
            v2y = py - cy;

            if ((v1x * v2y - v1y * v2x >= 0.0) != bClockwise)
                return false;

            v1x = ax - cx;
            v1y = ay - cy;

            v2x = px - ax;
            v2y = py - ay;

            if ((v1x * v2y - v1y * v2x >= 0.0) != bClockwise)
                return false;

            return true;
        }
        /*
        #region Intersection Methods

        /// <summary>
        ///    Tests an intersection between a ray and a box.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="box"></param>
        /// <returns>A Pair object containing whether the intersection occurred, and the distance between the 2 objects.</returns>
        public static IntersectResult Intersects(Ray ray, AxisAlignedBox box)
        {
            if (box.IsNull)
            {
                return new IntersectResult(false, 0);
            }

            Scalar lowt = 0.0f;
            Scalar t;
            bool hit = false;
            Vector3D hitPoint;
            Vector3D min = box.Minimum;
            Vector3D max = box.Maximum;

            // check origin inside first
            if (ray.origin > min && ray.origin < max)
            {
                return new IntersectResult(true, 0.0f);
            }

            // check each face in turn, only check closest 3

            // Min X
            if (ray.origin.X < min.X && ray.direction.X > 0)
            {
                t = (min.X - ray.origin.X) / ray.direction.X;

                if (t > 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.Y >= min.Y && hitPoint.Y <= max.Y &&
                        hitPoint.Z >= min.Z && hitPoint.Z <= max.Z &&
                        (!hit || t < lowt))
                    {

                        hit = true;
                        lowt = t;
                    }
                }
            }

            // Max X
            if (ray.origin.X > max.X && ray.direction.X < 0)
            {
                t = (max.X - ray.origin.X) / ray.direction.X;

                if (t > 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.Y >= min.Y && hitPoint.Y <= max.Y &&
                        hitPoint.Z >= min.Z && hitPoint.Z <= max.Z &&
                        (!hit || t < lowt))
                    {

                        hit = true;
                        lowt = t;
                    }
                }
            }

            // Min Y
            if (ray.origin.Y < min.Y && ray.direction.Y > 0)
            {
                t = (min.Y - ray.origin.Y) / ray.direction.Y;

                if (t > 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.X >= min.X && hitPoint.X <= max.X &&
                        hitPoint.Z >= min.Z && hitPoint.Z <= max.Z &&
                        (!hit || t < lowt))
                    {

                        hit = true;
                        lowt = t;
                    }
                }
            }

            // Max Y
            if (ray.origin.Y > max.Y && ray.direction.Y < 0)
            {
                t = (max.Y - ray.origin.Y) / ray.direction.Y;

                if (t > 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.X >= min.X && hitPoint.X <= max.X &&
                        hitPoint.Z >= min.Z && hitPoint.Z <= max.Z &&
                        (!hit || t < lowt))
                    {

                        hit = true;
                        lowt = t;
                    }
                }
            }

            // Min Z
            if (ray.origin.Z < min.Z && ray.direction.Z > 0)
            {
                t = (min.Z - ray.origin.Z) / ray.direction.Z;

                if (t > 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.X >= min.X && hitPoint.X <= max.X &&
                        hitPoint.Y >= min.Y && hitPoint.Y <= max.Y &&
                        (!hit || t < lowt))
                    {

                        hit = true;
                        lowt = t;
                    }
                }
            }

            // Max Z
            if (ray.origin.Z > max.Z && ray.direction.Z < 0)
            {
                t = (max.Z - ray.origin.Z) / ray.direction.Z;

                if (t > 0)
                {
                    // substitue t back into ray and check bounds and distance
                    hitPoint = ray.origin + ray.direction * t;

                    if (hitPoint.X >= min.X && hitPoint.X <= max.X &&
                        hitPoint.Y >= min.Y && hitPoint.Y <= max.Y &&
                        (!hit || t < lowt))
                    {

                        hit = true;
                        lowt = t;
                    }
                }
            }

            return new IntersectResult(hit, lowt);
        }


        /// <summary>
        ///    Tests an intersection between two boxes.
        /// </summary>
        /// <param name="boxA">
        ///    The primary box.
        /// </param>
        /// <param name="boxB">
        ///    The box to test intersection with boxA.
        /// </param>
        /// <returns>
        ///    <list type="bullet">
        ///        <item>
        ///            <description>None - There was no intersection between the 2 boxes.</description>
        ///        </item>
        ///        <item>
        ///            <description>Contained - boxA is fully within boxB.</description>
        ///         </item>
        ///        <item>
        ///            <description>Contains - boxB is fully within boxA.</description>
        ///         </item>
        ///        <item>
        ///            <description>Partial - boxA is partially intersecting with boxB.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// Submitted by: romout
        public static Intersection Intersects(AxisAlignedBox boxA, AxisAlignedBox boxB)
        {
            // grab the max and mix vectors for both boxes for comparison
            Vector3D minA = boxA.Minimum;
            Vector3D maxA = boxA.Maximum;
            Vector3D minB = boxB.Minimum;
            Vector3D maxB = boxB.Maximum;

            if ((minB.X < minA.X) &&
                (maxB.X > maxA.X) &&
                (minB.Y < minA.Y) &&
                (maxB.Y > maxA.Y) &&
                (minB.Z < minA.Z) &&
                (maxB.Z > maxA.Z))
            {

                // boxA is within boxB
                return Intersection.Contained;
            }

            if ((minB.X > minA.X) &&
                (maxB.X < maxA.X) &&
                (minB.Y > minA.Y) &&
                (maxB.Y < maxA.Y) &&
                (minB.Z > minA.Z) &&
                (maxB.Z < maxA.Z))
            {

                // boxB is within boxA
                return Intersection.Contains;
            }

            if ((minB.X > maxA.X) ||
                (minB.Y > maxA.Y) ||
                (minB.Z > maxA.Z) ||
                (maxB.X < minA.X) ||
                (maxB.Y < minA.Y) ||
                (maxB.Z < minA.Z))
            {

                // not interesting at all
                return Intersection.None;
            }

            // if we got this far, they are partially intersecting
            return Intersection.Partial;
        }


        public static IntersectResult Intersects(Ray ray, Sphere sphere)
        {
            return Intersects(ray, sphere, false);
        }

        /// <summary>
        ///		Ray/Sphere intersection test.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="sphere"></param>
        /// <param name="discardInside"></param>
        /// <returns>Struct that contains a bool (hit?) and distance.</returns>
        public static IntersectResult Intersects(Ray ray, Sphere sphere, bool discardInside)
        {
            Vector3D rayDir = ray.Direction;
            //Adjust ray origin relative to sphere center
            Vector3D rayOrig = ray.Origin - sphere.Center;
            Scalar radius = sphere.Radius;

            // check origin inside first
            if ((rayOrig.LengthSquared <= radius * radius) && discardInside)
            {
                return new IntersectResult(true, 0);
            }

            // mmm...sweet quadratics
            // Build coeffs which can be used with std quadratic solver
            // ie t = (-b +/- sqrt(b*b* + 4ac)) / 2a
            Scalar a = rayDir.Dot(rayDir);
            Scalar b = 2 * rayOrig.Dot(rayDir);
            Scalar c = rayOrig.Dot(rayOrig) - (radius * radius);

            // calc determinant
            Scalar d = (b * b) - (4 * a * c);

            if (d < 0)
            {
                // no intersection
                return new IntersectResult(false, 0);
            }
            else
            {
                // BTW, if d=0 there is one intersection, if d > 0 there are 2
                // But we only want the closest one, so that's ok, just use the 
                // '-' version of the solver
                Scalar t = (-b - MathUtil.Sqrt(d)) / (2 * a);

                if (t < 0)
                {
                    t = (-b + MathUtil.Sqrt(d)) / (2 * a);
                }

                return new IntersectResult(true, t);
            }
        }

        /// <summary>
        ///		Ray/Plane intersection test.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="plane"></param>
        /// <returns>Struct that contains a bool (hit?) and distance.</returns>
        public static IntersectResult Intersects(Ray ray, Plane plane)
        {
            Scalar denom = plane.Normal.Dot(ray.Direction);

            if (MathUtil.Abs(denom) < Scalar.Epsilon)
            {
                // Parellel
                return new IntersectResult(false, 0);
            }
            else
            {
                Scalar nom = plane.Normal.Dot(ray.Origin) + plane.D;
                Scalar t = -(nom / denom);
                return new IntersectResult(t >= 0, t);
            }
        }

        /// <summary>
        ///		Sphere/Box intersection test.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="box"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Sphere sphere, AxisAlignedBox box)
        {
            if (box.IsNull) return false;

            // Use splitting planes
            Vector3D center = sphere.Center;
            Scalar radius = sphere.Radius;
            Vector3D min = box.Minimum;
            Vector3D max = box.Maximum;

            // just test facing planes, early fail if sphere is totally outside
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

            if (center.Z < min.Z &&
                min.Z - center.Z > radius)
            {
                return false;
            }
            if (center.Z > max.Z &&
                center.Z - max.Z > radius)
            {
                return false;
            }

            // Must intersect
            return true;
        }

        /// <summary>
        ///		Plane/Box intersection test.
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="box"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Plane plane, AxisAlignedBox box)
        {
            if (box.IsNull) return false;

            // Get corners of the box
            Vector3D[] corners = box.Corners;

            // Test which side of the plane the corners are
            // Intersection occurs when at least one corner is on the 
            // opposite side to another
            PlaneSide lastSide = plane.GetSide(corners[0]);

            for (int corner = 1; corner < 8; corner++)
            {
                if (plane.GetSide(corners[corner]) != lastSide)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///		Sphere/Plane intersection test.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="plane"></param>
        /// <returns>True if there was an intersection, false otherwise.</returns>
        public static bool Intersects(Sphere sphere, Plane plane)
        {
            return MathUtil.Abs(plane.Normal.Dot(sphere.Center)) <= sphere.Radius;
        }

        /// <summary>
        ///    Ray/PlaneBoundedVolume intersection test.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="volume"></param>
        /// <returns>Struct that contains a bool (hit?) and distance.</returns>
        public static IntersectResult Intersects(Ray ray, PlaneBoundedVolume volume)
        {
            PlaneList planes = volume.planes;

            Scalar maxExtDist = 0.0f;
            Scalar minIntDist = Scalar.PositiveInfinity;

            Scalar dist, denom, nom;

            for (int i = 0; i < planes.Count; i++)
            {
                Plane plane = (Plane)planes[i];

                denom = plane.Normal.Dot(ray.Direction);
                if (MathUtil.Abs(denom) < Scalar.Epsilon)
                {
                    // Parallel
                    if (plane.GetSide(ray.Origin) == volume.outside)
                        return new IntersectResult(false, 0);

                    continue;
                }

                nom = plane.Normal.Dot(ray.Origin) + plane.D;
                dist = -(nom / denom);

                if (volume.outside == PlaneSide.Negative)
                    nom = -nom;

                if (dist > 0.0f)
                {
                    if (nom > 0.0f)
                    {
                        if (maxExtDist < dist)
                            maxExtDist = dist;
                    }
                    else
                    {
                        if (minIntDist > dist)
                            minIntDist = dist;
                    }
                }
                else
                {
                    //Ray points away from plane
                    if (volume.outside == PlaneSide.Negative)
                        denom = -denom;

                    if (denom > 0.0f)
                        return new IntersectResult(false, 0);
                }
            }

            if (maxExtDist > minIntDist)
                return new IntersectResult(false, 0);

            return new IntersectResult(true, maxExtDist);
        }

        #endregion Intersection Methods
        */
        #endregion Static Methods


    }
    /*
    #region Return returnvalue structures

    /// <summary>
    ///		Simple struct to allow returning a complex intersection returnvalue.
    /// </summary>
    public struct IntersectResult
    {
        #region Fields

        /// <summary>
        ///		Did the intersection test returnvalue in a hit?
        /// </summary>
        public bool Hit;

        /// <summary>
        ///		If Hit was true, this will hold a query specific distance value.
        ///		i.e. for a Ray-Box test, the distance will be the distance from the start point
        ///		of the ray to the point of intersection.
        /// </summary>
        public Scalar Distance;

        #endregion Fields

        /// <summary>
        ///		Constructor.
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="distance"></param>
        public IntersectResult(bool hit, Scalar distance)
        {
            this.Hit = hit;
            this.Distance = distance;
        }
    }

    #endregion Return returnvalue structures
    */
}
