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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Xml.Serialization;
namespace AdvanceMath
{
    /// <summary>
    /// This is the Vector Class.
    /// </summary>
    /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29"/></remarks>
    [StructLayout(LayoutKind.Sequential, Size = Vector2D.Size, Pack = 0), Serializable]
    //[System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.VectorConverter<Vector2D>))]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)),AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public struct Vector2D : IVector<Vector2D>
    {
        #region const fields
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = 2;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region readonly fields
        /// <summary>
        /// Vector2D(0,0)
        /// </summary>
        public static readonly Vector2D Origin = new Vector2D();
        /// <summary>
        /// Vector2D(0,0)
        /// </summary>
        public static readonly Vector2D Zero = new Vector2D();
        /// <summary>
        /// Vector2D(1,0)
        /// </summary>
        public static readonly Vector2D XAxis = new Vector2D(1, 0);
        /// <summary>
        /// Vector2D(0,1)
        /// </summary>
        public static readonly Vector2D YAxis = new Vector2D(0, 1);
        /// <summary>
        /// Vector2D(0.707...,0.707...)
        /// </summary>
        public static readonly Vector2D XYAxis = new Vector2D((Scalar)0.7071067811865475244008443621052, (Scalar)0.7071067811865475244008443621052);
        #endregion
        #region static methods
        public static Vector2D FromArray(Scalar[] array)
        {
            Vector2D returnvalue = Zero;
            returnvalue.CopyFrom(array, 0);
            return returnvalue;
        }
        #region array
        public static Vector2D[] Transform(Matrix3x3 matrix, Vector2D[] stack)
        {
            Scalar inverseZ = 1.0f / (matrix.m20 + matrix.m21 + matrix.m22);
            int lenght = stack.Length;
            Vector2D[] returnvalue = new Vector2D[lenght];
            for (int pos = 0; pos < lenght; ++pos)
            {
                returnvalue[pos].x = inverseZ * (stack[pos].x * matrix.m00 + stack[pos].y * matrix.m01 + matrix.m02);
                returnvalue[pos].y = inverseZ * (stack[pos].x * matrix.m10 + stack[pos].y * matrix.m11 + matrix.m12);
            }
            return returnvalue;
        }
        public static Vector2D[] Translate(Vector2D source, Vector2D[] stack)
        {
            int lenght = stack.Length;
            Vector2D[] returnvalue = new Vector2D[lenght];
            for (int pos = 0; pos < lenght; ++pos)
            {
                returnvalue[pos].x = stack[pos].x + source.x;
                returnvalue[pos].y = stack[pos].y + source.y;
            }
            return returnvalue;
        }
        public static Vector2D[] Rotate(Scalar radianAngle, Vector2D[] stack)
        {
            Scalar negradianAngle = -radianAngle;
            Scalar cos = (Scalar)Math.Cos(negradianAngle);
            Scalar sin = (Scalar)Math.Sin(negradianAngle);
            int lenght = stack.Length;
            Vector2D[] returnvalue = new Vector2D[lenght];
            for (int pos = 0; pos < lenght; ++pos)
            {
                returnvalue[pos].x = stack[pos].x * cos + stack[pos].y * sin;
                returnvalue[pos].y = stack[pos].y * cos - stack[pos].x * sin;
            }
            return returnvalue;
        }
        public static Vector2D[] Rotate(Matrix2x2 rotation, Vector2D[] stack)
        {
            int lenght = stack.Length;
            Vector2D[] returnvalue = new Vector2D[lenght];
            for (int pos = 0; pos < lenght; ++pos)
            {
                returnvalue[pos].x = stack[pos].x * rotation.m00 + stack[pos].y * rotation.m01;
                returnvalue[pos].y = stack[pos].x * rotation.m10 + stack[pos].y * rotation.m11;
            }
            return returnvalue;
        }
        public static Vector2D[] Multiply(Scalar scalar, Vector2D[] stack)
        {
            int lenght = stack.Length;
            Vector2D[] returnvalue = new Vector2D[lenght];
            for (int pos = 0; pos < lenght; ++pos)
            {
                returnvalue[pos].x = stack[pos].x * scalar;
                returnvalue[pos].y = stack[pos].y * scalar;
            }
            return returnvalue;
        }
        public static void Transform(Matrix3x3 matrix, ref Vector2D[] stack)
        {
            Scalar inverseZ = 1.0f / (matrix.m20 + matrix.m21 + matrix.m22);
            Scalar X;
            int lenght = stack.Length;
            for (int pos = 0; pos < lenght; ++pos)
            {
                X = stack[pos].x;
                stack[pos].x = inverseZ * (X * matrix.m00 + stack[pos].y * matrix.m01 + matrix.m02);
                stack[pos].y = inverseZ * (X * matrix.m10 + stack[pos].y * matrix.m11 + matrix.m12);
            }
        }
        public static void Translate(Vector2D source, ref Vector2D[] stack)
        {
            int lenght = stack.Length;
            for (int pos = 0; pos < lenght; ++pos)
            {
                stack[pos].x = stack[pos].x + source.x;
                stack[pos].y = stack[pos].y + source.y;
            }
        }
        public static void Rotate(Scalar radianAngle, ref Vector2D[] stack)
        {
            Scalar negradianAngle = -radianAngle;
            Scalar cos = (Scalar)Math.Cos(negradianAngle);
            Scalar sin = (Scalar)Math.Sin(negradianAngle);
            int lenght = stack.Length;
            Scalar X;
            for (int pos = 0; pos < lenght; ++pos)
            {
                X = stack[pos].x;
                stack[pos].x = X * cos + stack[pos].y * sin;
                stack[pos].y = stack[pos].y * cos - X * sin;
            }
        }
        public static void Rotate(Matrix2x2 rotation, ref Vector2D[] stack)
        {
            int lenght = stack.Length;
            Scalar X;
            for (int pos = 0; pos < lenght; ++pos)
            {
                X = stack[pos].x;
                stack[pos].x = X * rotation.m00 + stack[pos].y * rotation.m01;
                stack[pos].y = stack[pos].y * rotation.m10 + X * rotation.m11;
            }
        }
        public static void Multiply(Scalar scalar, ref Vector2D[] stack)
        {
            int lenght = stack.Length;
            for (int pos = 0; pos < lenght; ++pos)
            {
                stack[pos].x = stack[pos].x * scalar;
                stack[pos].y = stack[pos].y * scalar;
            }
        }
        #endregion
        /// <summary>
        /// Creates a Vector2D With the given length (<see cref="Magnitude"/>) and the given <see cref="Angle"/>.
        /// </summary>
        /// <param name="length">The length (<see cref="Magnitude"/>) of the Vector2D to be created</param>
        /// <param name="radianAngle">The angle of the from the (<see cref="XAxis"/>) in Radians</param>
        /// <returns>a Vector2D With the given length and angle.</returns>
        /// <remarks>
        /// <code>FromLengthAndAngle(1,Math.PI/2)</code>
        ///  would create a Vector2D equil to 
        ///  <code>new Vector2D(0,1)</code>. 
        ///  And <code>FromLengthAndAngle(1,0)</code>
        ///  would create a Vector2D equil to 
        ///  <code>new Vector2D(1,0)</code>.
        /// </remarks>
        public static Vector2D FromLengthAndAngle(Scalar length, Scalar radianAngle)
        {
            Scalar cos = (Scalar)Math.Cos(radianAngle);
            Scalar sin = (Scalar)Math.Sin(radianAngle);
            Vector2D returnvalue;
            returnvalue.x = length * cos;
            returnvalue.y = length * sin;
            return returnvalue;
        }
        /// <summary>
        /// Rotates a Vector2D.
        /// </summary>
        /// <param name="radianAngle">The <see cref="Angle"/> in radians of the amount it is to be rotated.</param>
        /// <param name="source">The Vector2D to be Rotated.</param>
        /// <returns>The Rotated Vector2D</returns>
        public static Vector2D Rotate(Scalar radianAngle, Vector2D source)
        {
            Scalar negradianAngle = -radianAngle;
            Scalar cos = (Scalar)Math.Cos(negradianAngle);
            Scalar sin = (Scalar)Math.Sin(negradianAngle);
            Vector2D returnvalue;
            returnvalue.x = source.x * cos + source.y * sin;
            returnvalue.y = source.y * cos - source.x * sin;
            return returnvalue;
        }
        /// <summary>
        /// Rotates a Vector2D.
        /// </summary>
        /// <param name="source">The Vector2D to be Rotated.</param>
        /// <param name="rotation">The rotation Matrix, but can also scale and shear.</param>
        /// <returns>The Rotated Vector2D.</returns>
        public static Vector2D Rotate(Matrix2x2 rotation, Vector2D source)
        {
            Vector2D returnvalue;
            returnvalue.x = source.x * rotation.m00 + source.y * rotation.m01;
            returnvalue.y = source.x * rotation.m10 + source.y * rotation.m11;
            return returnvalue;
        }
        /// <summary>
        /// Sets the <see cref="Angle"/> of a Vector2D without changing the <see cref="Magnitude"/>.
        /// </summary>
        /// <param name="source">The Vector2D to have its Angle set.</param>
        /// <param name="radianAngle">The angle of the from the (<see cref="XAxis"/>) in Radians</param>
        /// <returns>A Vector2D with a new Angle.</returns>
        public static Vector2D SetAngle(Vector2D source, Scalar radianAngle)
        {
            Scalar magnitude = GetMagnitude(source);
            Scalar cos = (Scalar)Math.Cos(radianAngle);
            Scalar sin = (Scalar)Math.Sin(radianAngle);
            Vector2D returnvalue;
            returnvalue.x = magnitude * cos;
            returnvalue.y = magnitude * sin;
            return returnvalue;
        }
        /// <summary>
        /// Determines the current <see cref="Angle"/> in radians of the Vector2D and Returns it.
        /// </summary>
        /// <param name="source">The Vector2D of whos angle is to be Determined.</param>
        /// <returns>The <see cref="Angle"/> in radians of the Vector2D.</returns>
        public static Scalar GetAngle(Vector2D source)
        {
            if (source.x == 0)
            {
                if (source.y > 0)
                {
                    return MathAdv.HALF_PI;
                }
                else if (source.y == 0)
                {
                    return 0;
                }
                else
                {
                    return MathAdv.HALF_THREE_PI;
                }
            }
            else if (source.x > 0)
            {
                if (source.y < 0)
                {
                    return (Scalar)Math.Atan(source.y / source.x) + MathAdv.TWO_PI;
                }
                else
                {
                    return (Scalar)Math.Atan(source.y / source.x);
                }
            }
            else
            {
                return (Scalar)Math.Atan(source.y / source.x) + MathAdv.PI;
            }
        }
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Sum of the 2 Vector2Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector2D Add(Vector2D left, Vector2D right)
        {
            Vector2D returnvalue;
            returnvalue.x = left.x + right.x;
            returnvalue.y = left.y + right.y;
            return returnvalue;
        }
        /// <summary>
        /// Subtracts 2 Vector2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Difference of the 2 Vector2Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector2D Subtract(Vector2D left, Vector2D right)
        {
            Vector2D returnvalue;
            returnvalue.x = left.x - right.x;
            returnvalue.y = left.y - right.y;
            return returnvalue;
        }
        /// <summary>
        /// Uses a matrix multiplication to Transform the vector.
        /// </summary>
        /// <param name="matrix">The Transformation matrix</param>
        /// <param name="source">The Vector to be transformed</param>
        /// <returns>The transformed vector.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Transformation_matrix#Affine_transformations"/></remarks>
        public static Vector2D Multiply(Matrix3x3 matrix, Vector2D source)
        {
            return matrix * source;
        }
        /// <summary>
        /// Uses a matrix multiplication to Transform the vector.
        /// </summary>
        /// <param name="matrix">The rotation matrix</param>
        /// <param name="source">The Vector to be transformed</param>
        /// <returns>The transformed vector.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Transformation_matrix#Rotation"/></remarks>
        public static Vector2D Multiply(Matrix2x2 matrix, Vector2D source)
        {
            return matrix * source;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector2D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector2D.</param>
        /// <param name="source">The Vector2D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector2D Multiply(Vector2D source, Scalar scalar)
        {
            Vector2D returnvalue;
            returnvalue.x = source.x * scalar;
            returnvalue.y = source.y * scalar;
            return returnvalue;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar Dot(Vector2D left, Vector2D right)
        {
            return left.y * right.y + left.x * right.x;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Z value of the returnvalueing vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will returnvalue in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Scalar ZCross(Vector2D left, Vector2D right)
        {
            return left.x * right.y - left.y * right.x;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="leftZ">The Z value of the left vector operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Vector2D that fully represents the returnvalueing vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will returnvalue in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D ZCross(Scalar leftZ, Vector2D right)
        {
            Vector2D returnvalue;
            returnvalue.x = -leftZ * right.y;
            returnvalue.y = leftZ * right.x;
            return returnvalue;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="rightZ">The Z value of the right vector operand.</param>
        /// <returns>The Vector2D that fully represents the returnvalueing vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will returnvalue in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D ZCross(Vector2D left, Scalar rightZ)
        {
            Vector2D returnvalue;
            returnvalue.x = left.y * rightZ;
            returnvalue.y = -left.x * rightZ;
            return returnvalue;
        }
        /// <summary>
        /// Does repeated "2D" Cross Products (Outer Products) on muliple Vectors.
        /// </summary>
        /// <param name="first">The first Vector2D operand.</param>
        /// <param name="second">The second Vector2D operand.</param>
        /// <param name="third">The third Vector2D operand.</param>
        /// <returns>The same Value that would be returned by <c>ZCross(ZCross(first,second),third)</c></returns>
        /// <remarks>
        /// Not to be Confused with a Scalar Triple Product.
        /// <seealso cref="AdvanceMath.Vector2D.ZCross(Vector2D,Vector2D)"/>
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D TripleCrossLeft2First(Vector2D first, Vector2D second, Vector2D third)
        {
            Scalar z = first.x * second.y - first.y * second.x;
            Vector2D returnvalue;
            returnvalue.x = -z * third.y;
            returnvalue.y = z * third.x;
            return returnvalue;
        }
        /// <summary>
        /// Does repeated "2D" Cross Products (Outer Products) on muliple Vectors.
        /// </summary>
        /// <param name="first">The first Vector2D operand.</param>
        /// <param name="second">The second Vector2D operand.</param>
        /// <param name="third">The third Vector2D operand.</param>
        /// <returns>The same value that would be returned by ZCross(first,ZCross(second,third))</returns>
        /// <remarks>
        /// Not to be Confused with a Scalar Triple Product.
        /// <seealso cref="AdvanceMath.Vector2D.ZCross(Vector2D,Vector2D)"/>
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D TripleCrossRight2First(Vector2D first, Vector2D second, Vector2D third)
        {
            Scalar z = second.x * third.y - second.y * third.x;
            Vector2D returnvalue;
            returnvalue.x = first.y * z;
            returnvalue.y = -first.x * z;
            return returnvalue;
        }
        /// <summary>
        /// Gets the Squared <see cref="Magnitude"/> of the Vector2D that is passed.
        /// </summary>
        /// <param name="source">The Vector2D whos Squared Magnitude is te be returned.</param>
        /// <returns>The Squared Magnitude.</returns>
        public static Scalar GetMagnitudeSq(Vector2D source)
        {
            return source.x * source.x + source.y * source.y;
        }
        /// <summary>
        /// Gets the <see cref="Magnitude"/> of the Vector2D that is passed.
        /// </summary>
        /// <param name="source">The Vector2D whos Magnitude is te be returned.</param>
        /// <returns>The Magnitude.</returns>
        public static Scalar GetMagnitude(Vector2D source)
        {
            return (Scalar)Math.Sqrt(source.x * source.x + source.y * source.y);
        }
        /// <summary>
        /// Sets the <see cref="Magnitude"/> of a Vector2D without changing the  <see cref="Angle"/>.
        /// </summary>
        /// <param name="source">The Vector2D whose Magnitude is to be changed.</param>
        /// <param name="magnitude">The Magnitude.</param>
        /// <returns>A Vector2D with the new Magnitude</returns>
        public static Vector2D SetMagnitude(Vector2D source, Scalar magnitude)
        {
            Scalar oldmagnitude = GetMagnitude(source);
            if (oldmagnitude > 0)
            {
                return source * (magnitude / oldmagnitude);
            }
            else
            {
                return Zero;
            }
        }
        /// <summary>
        /// Negates a Vector2D.
        /// </summary>
        /// <param name="source">The Vector2D to be Negated.</param>
        /// <returns>The Negated Vector2D.</returns>
        public static Vector2D Negate(Vector2D source)
        {
            Vector2D returnvalue;
            returnvalue.x = -source.x;
            returnvalue.y = -source.y;
            return returnvalue;
        }
        /// <summary>
        /// This returns the Normalized Vector2D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector2D to be Normalized.</param>
        /// <returns>The Normalized Vector2D. (Unit Vector)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public static Vector2D Normalize(Vector2D source)
        {
            Scalar magnitude = GetMagnitude(source);
            if (magnitude > 0)
            {
                Scalar magnitudeInv = (1 / magnitude);
                Vector2D returnvalue;
                returnvalue.x = source.x * magnitudeInv;
                returnvalue.y = source.y * magnitudeInv;
                return returnvalue;
            }
            else
            {
                return Vector2D.Zero;
            }
        }
        /// <summary>
        /// This returns the Normalized Vector2D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector2D to be Normalized.</param>
        /// <param name="magnitude">the magitude of the Vector2D passed</param>
        /// <returns>The Normalized Vector2D. (Unit Vector)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public static Vector2D Normalize(Vector2D source, out Scalar magnitude)
        {
            magnitude = (Scalar)Math.Sqrt(source.x * source.x + source.y * source.y);
            if (magnitude != 0)
            {
                Scalar magnitudeInv = (1 / magnitude);
                Vector2D returnvalue;
                returnvalue.x = source.x * magnitudeInv;
                returnvalue.y = source.y * magnitudeInv;
                return returnvalue;
            }
            else
            {
                magnitude = 0;
                return Vector2D.Zero;
            }
        }
        /// <summary>
        /// Thie Projects the left Vector2D onto the Right Vector2D.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Projected Vector2D.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Projection_%28linear_algebra%29"/></remarks>
        public static Vector2D Project(Vector2D left, Vector2D right)
        {
            Scalar tmp = Dot(left, right) / GetMagnitudeSq(right);
            Vector2D returnvalue;
            returnvalue.x = tmp * right.x;
            returnvalue.y = tmp * right.y;
            return returnvalue;
        }
        /// <summary>
        /// Gets a Vector2D that is perpendicular(orthogonal) to the passed Vector2D while staying on the XY Plane.
        /// </summary>
        /// <param name="source">The Vector2D whose perpendicular(orthogonal) is to be determined.</param>
        /// <returns>An perpendicular(orthogonal) Vector2D using the Right Hand Rule</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Right-hand_rule"/></remarks>
        public static Vector2D GetRightHandNormal(Vector2D source)
        {
            Vector2D returnvalue;
            returnvalue.x = -source.y;
            returnvalue.y = source.x;
            return returnvalue;
        }
        /// <summary>
        /// Gets a Vector2D that is perpendicular(orthogonal) to the passed Vector2D while staying on the XY Plane.
        /// </summary>
        /// <param name="source">The Vector2D whose perpendicular(orthogonal) is to be determined.</param>
        /// <returns>An perpendicular(orthogonal) Vector2D using the Left Hand Rule (opposite of the Right hand Rule)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Right-hand_rule#Left-hand_rule"/></remarks>
        public static Vector2D GetLeftHandNormal(Vector2D source)
        {
            Vector2D returnvalue;
            returnvalue.x = source.y;
            returnvalue.y = -source.x;
            return returnvalue;
        }

        internal static void HermiteHelper(Scalar weightingFactor, out Scalar h1, out Scalar h2, out Scalar h3, out Scalar h4)
        {
            Scalar wf2 = weightingFactor * weightingFactor;
            Scalar wf3 = wf2 * weightingFactor;
            Scalar wf3t2 = 2 * wf3;
            Scalar wf2t3 = 3 * wf2;
            h1 = wf3t2 - wf2t3 + 1;
            h2 = -wf3t2 + wf2t3;
            h3 = wf3 - 2 * wf2 + weightingFactor;
            h4 = wf3 - wf2;
        }
        public static Vector2D HermiteSpline(Vector2D position, Vector2D tangent, Vector2D position2, Vector2D tangent2, Scalar weightingFactor)
        {
            Scalar h1, h2, h3, h4;
            HermiteHelper(weightingFactor, out h1, out h2, out h3, out h4);
            Vector2D p = h1 * position + h2 * position2 + h3 * tangent + h4 * tangent2;
            return p;
        }
        public static Vector2D CatmullRomSpline(Vector2D position1, Vector2D position2, Vector2D position3, Vector2D position4, Scalar weightingFactor)
        {
            return HermiteSpline(position2, position3, (position3 - position1) * .5f, (position4 - position2) * .5f, weightingFactor);
        }
        public static Vector2D LinearInterpolation(Vector2D left, Vector2D right, Scalar interpolater)
        {
            return left + interpolater * (right - left);
        }
        

        #endregion
        #region fields
        internal Scalar x;
        internal Scalar y;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a New Vector2D Instance on the Stack.
        /// </summary>
        /// <param name="X">The X value.</param>
        /// <param name="Y">The Y value.</param>
        [AdvanceSystem.ComponentModel.UTCConstructor]
        public Vector2D(Scalar X, Scalar Y)
        {
            this.x = X;
            this.y = Y;
        }
        public Vector2D(Scalar[] vals)
        {
            this = Zero;
            this.CopyFrom(vals,0);
        }
        #endregion
        #region indexers
        public Scalar this[int index]
        {
            get
            {
                Debug.Assert(index < Count, "Attempt to get a index of a Vector" + Count + "D greater than " + (Count - 1) + ".");
                Debug.Assert(index >= 0, "Attempt to get a index of a Vector" + Count + "D less than 0.");
                unsafe
                {
                    fixed (Scalar* ptr = &x)
                    {
                        return ptr[index];
                    }
                }
            }
            set
            {
                Debug.Assert(index < Count, "Attempt to set a index of a Vector" + Count + "D greater than " + (Count - 1) + ".");
                Debug.Assert(index >= 0, "Attempt to set a index of a Vector" + Count + "D less than 0.");
                unsafe
                {
                    fixed (Scalar* ptr = &x)
                    {
                        ptr[index] = value;
                    }
                }
            }
        }
        #endregion
        #region public properties
        /// <summary>
        /// This is the X value. (Usually represents a horizontal position or direction.)
        /// </summary>
        [XmlAttribute]
        public Scalar X
        {
            get { return x; }
            set { x = value; }
        }
        /// <summary>
        /// This is the Y value. (Usually represents a vertical position or direction.)
        /// </summary>
        [XmlAttribute]
        public Scalar Y
        {
            get { return y; }
            set { y = value; }
        }
        /// <summary>
        /// The Number of Variables accesable though the indexer.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public int Length { get { return Count; } }
        /// <summary>
        /// Gets A perpendicular(orthogonal) Vector2D using the Right Hand Rule.
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Right-hand_rule"/></remarks>
        [System.ComponentModel.Browsable(false)]
        public Vector2D RightHandNormal
        {
            get
            {
                return GetRightHandNormal(this);
            }
        }
        /// <summary>
        /// Gets A perpendicular(orthogonal) Vector2D using the Left Hand Rule.
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Right-hand_rule#Left-hand_rule"/></remarks>
        [System.ComponentModel.Browsable(false)]
        public Vector2D LeftHandNormal
        {
            get
            {
                return GetLeftHandNormal(this);
            }
        }
        /// <summary>
        /// Gets or Sets the Magnitude (Length) of the Vector2D. 
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        [XmlIgnore, SoapIgnore]
        [System.ComponentModel.Browsable(false)]
        public Scalar Magnitude
        {
            get
            {
                return (Scalar)Math.Sqrt(this.x * this.x + this.y * this.y);
            }
            set
            {
                this = SetMagnitude(this, value);
            }
        }
        /// <summary>
        /// Gets the Squared Magnitude of the Vector2D.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Scalar MagnitudeSq
        {
            get
            {
                return this.x * this.x + this.y * this.y;
            }
        }
        /// <summary>
        /// Gets or Sets the Angle in radians of the Vector2D.
        /// </summary>
        /// <remarks>
        /// If the Magnitude of the Vector is 1 then The 
        /// Angles {0,Math.PI/2,Math.PI/2,3*Math.PI/2} would have the vectors {(1,0),(0,1),(-1,0),(0,-1)} respectively.
        /// </remarks>
        [XmlIgnore, SoapIgnore]
        [System.ComponentModel.Browsable(false)]
        public Scalar Angle
        {
            get
            {
                return GetAngle(this);
            }
            set
            {
                this = SetAngle(this, value);
            }
        }
        /// <summary>
        /// Gets the Normalized Vector2D. (Unit Vector)
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        [System.ComponentModel.Browsable(false)]
        public Vector2D Normalized
        {
            get
            {
                return Normalize(this);
            }
        }
        #endregion
        #region public methods
        public Scalar[] ToArray()
        {
            return new Scalar[Count] { x, y };
        }
        public void CopyFrom(Scalar[] array, int index)
        {
            x = array[index];
            y = array[index + 1];
        }
        public void CopyTo(Scalar[] array, int index)
        {
            array[index] = x;
            array[index + 1] = y;
        }
        #endregion
        #region operators
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Sum of the 2 Vector2Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector2D operator +(Vector2D left, Vector2D right)
        {
            Vector2D returnvalue;
            returnvalue.x = left.x + right.x;
            returnvalue.y = left.y + right.y;
            return returnvalue;
        }
        /// <summary>
        /// Subtracts 2 Vector2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Difference of the 2 Vector2Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector2D operator -(Vector2D left, Vector2D right)
        {
            Vector2D returnvalue;
            returnvalue.x = left.x - right.x;
            returnvalue.y = left.y - right.y;
            return returnvalue;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector2D.
        /// </summary>
        /// <param name="source">The Vector2D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector2D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector2D operator *(Vector2D source, Scalar scalar)
        {
            Vector2D returnvalue;
            returnvalue.x = source.x * scalar;
            returnvalue.y = source.y * scalar;
            return returnvalue;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector2D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector2D.</param>
        /// <param name="source">The Vector2D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector2D operator *(Scalar scalar, Vector2D source)
        {
            Vector2D returnvalue;
            returnvalue.x = scalar * source.x;
            returnvalue.y = scalar * source.y;
            return returnvalue;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar operator *(Vector2D left, Vector2D right)
        {
            return left.y * right.y + left.x * right.x;
        }
        public static Vector2D operator *(Matrix3x3 matrix, Vector2D source)
        {
            Scalar inverseZ = 1.0f / (source.x * matrix.m20 + source.y * matrix.m21 + matrix.m22);
            Vector2D returnvalue;
            returnvalue.x = (source.x * matrix.m00 + source.y * matrix.m01 + matrix.m02) * inverseZ;
            returnvalue.y = (source.x * matrix.m10 + source.y * matrix.m11 + matrix.m12) * inverseZ;
            return returnvalue;
        }
        public static Vector2D operator *(Matrix2x2 matrix, Vector2D source)
        {
            Vector2D returnvalue;
            returnvalue.x = (source.x * matrix.m00 + source.y * matrix.m01);
            returnvalue.y = (source.x * matrix.m10 + source.y * matrix.m11);
            return returnvalue;
        }
        /// <summary>
        /// Negates a Vector2D.
        /// </summary>
        /// <param name="source">The Vector2D to be Negated.</param>
        /// <returns>The Negated Vector2D.</returns>
        public static Vector2D operator -(Vector2D source)
        {
            Vector2D returnvalue;
            returnvalue.x = -source.x;
            returnvalue.y = -source.y;
            return returnvalue;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Z value of the returnvalueing vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will returnvalue in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Scalar operator ^(Vector2D left, Vector2D right)
        {
            return left.x * right.y - left.y * right.x;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="leftZ">The Z value of the left vector operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Vector2D that fully represents the returnvalueing vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will returnvalue in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D operator ^(Scalar leftZ, Vector2D right)
        {
            Vector2D returnvalue;
            returnvalue.x = -leftZ * right.y;
            returnvalue.y = leftZ * right.x;
            return returnvalue;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="rightZ">The Z value of the right vector operand.</param>
        /// <returns>The Vector2D that fully represents the returnvalueing vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will returnvalue in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D operator ^(Vector2D left, Scalar rightZ)
        {
            Vector2D returnvalue;
            returnvalue.x = left.y * rightZ;
            returnvalue.y = -left.x * rightZ;
            return returnvalue;
        }
        /// <summary>
        /// Specifies whether the Vector2Ds contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector2D to test.</param>
        /// <param name="right">The right Vector2D to test.</param>
        /// <returns>true if the Vector2Ds have the same coordinates; otherwise false</returns>
        public static bool operator ==(Vector2D left, Vector2D right)
        {
            return left.x == right.x && left.y == right.y;
        }
        /// <summary>
        /// Specifies whether the Vector2Ds do not contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector2D to test.</param>
        /// <param name="right">The right Vector2D to test.</param>
        /// <returns>true if the Vector2Ds do not have the same coordinates; otherwise false</returns>
        public static bool operator !=(Vector2D left, Vector2D right)
        {
            return !(left.x == right.x && left.y == right.y);
        }



        public static explicit operator Vector2D(Vector3D source)
        {
            Vector2D returnvalue;
            returnvalue.x = source.X;
            returnvalue.y = source.Y;
            return returnvalue;
        }

        #endregion
        #region overrides
        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector2D.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        [AdvanceSystem.ComponentModel.UTCFormater]
        public override string ToString()
        {
            return string.Format("({0},{1})", this.x, this.y);
        }
        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector2D.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        public string ToIntegerString()
        {
            return string.Format("({0},{1})", (int)this.x, (int)this.y);
        }
        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector2D.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        public string ToString(bool shortenDecmialPlaces)
        {
            if (shortenDecmialPlaces)
            {
                return string.Format("({0:0.##}, {1:0.##})", this.x, this.y);
            }
            else
            {
                return ToString();
            }
        }
        [AdvanceSystem.ComponentModel.UTCParser]
        public static Vector2D Parse(string text)
        {
            string[] vals = text.Trim(' ','(', '[', '<',')', ']', '>').Split(',');
            if (vals.Length != Count)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 2 parts separated by commas in the form (x,y) with optional parenthesis.", text));
            }
            else
            {
                try
                {
                    Vector2D returnvalue;
                    returnvalue.x = Scalar.Parse(vals[0].Trim());
                    returnvalue.y = Scalar.Parse(vals[1].Trim());
                    return returnvalue;
                }
                catch (Exception ex)
                {
                    throw new FormatException("The parts of the vectors must be decimal numbers", ex);
                }
            }
        }
        /// <summary>
        ///		Provides a unique hash code based on the member variables of this
        ///		class.  This should be done because the equality operators (==, !=)
        ///		have been overriden by this class.
        ///		<p/>
        ///		The standard implementation is a simple XOR operation between all local
        ///		member variables.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector2D) && (this == (Vector2D)obj);
        }
        #endregion
    }
}