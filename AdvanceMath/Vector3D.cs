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
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
namespace AdvanceMath
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29"/></remarks>
    [StructLayout(LayoutKind.Sequential, Size = Vector3D.Size, Pack = 0), Serializable]
    //[System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.VectorConverter<Vector3D>))]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)),AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public struct Vector3D : IVector<Vector3D>
    {
        #region const fields
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = 3;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region readonly fields
        /// <summary>
        /// Vector3D(0,0,0)
        /// </summary>
        public static readonly Vector3D Origin = new Vector3D();
        /// <summary>
        /// Vector3D(0,0,0)
        /// </summary>
        public static readonly Vector3D Zero = new Vector3D();
        /// <summary>
        /// Vector3D(1,0,0)
        /// </summary>
        public static readonly Vector3D XAxis = new Vector3D(1, 0, 0);
        /// <summary>
        /// Vector3D(0,1,0)
        /// </summary>
        public static readonly Vector3D YAxis = new Vector3D(0, 1, 0);
        /// <summary>
        /// Vector3D(0,0,1)
        /// </summary>
        public static readonly Vector3D ZAxis = new Vector3D(0, 0, 1);
        #endregion
        #region static methods
        public static Vector3D FromArray(Scalar[] array)
        {
            Vector3D returnvalue = Vector3D.Zero;
            returnvalue.CopyFrom(array, 0);
            return returnvalue;
        }
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Sum of the 2 Vector3Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector3D Add(Vector3D left, Vector3D right)
        {
            Vector3D returnvalue;
            returnvalue.x = left.x + right.x;
            returnvalue.y = left.y + right.y;
            returnvalue.z = left.z + right.z;
            return returnvalue;
        }
        /// <summary>
        /// Subtracts 2 Vector3Ds.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Difference of the 2 Vector3Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector3D Subtract(Vector3D left, Vector3D right)
        {
            Vector3D returnvalue;
            returnvalue.x = left.x - right.x;
            returnvalue.y = left.y - right.y;
            returnvalue.z = left.z - right.z;
            return returnvalue;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector3D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector3D Multiply(Vector3D source, Scalar scalar)
        {
            Vector3D returnvalue;
            returnvalue.x = source.x * scalar;
            returnvalue.y = source.y * scalar;
            returnvalue.z = source.z * scalar;
            return returnvalue;
        }
        /// <summary>
        ///		matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D Multiply(Matrix3x3 matrix, Vector3D vector)
        {
            return matrix * vector;
        }
        /// <summary>
        ///		vector * matrix [1x3 * 3x3 = 1x3]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D Multiply(Vector3D vector, Matrix3x3 matrix)
        {
            return vector * matrix;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar Dot(Vector3D left, Vector3D right)
        {
            return left.y * right.y + left.x * right.x + left.z * right.z;
        }
        /// <summary>
        /// Does a Cross Operation Also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Cross Product.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Cross_product"/></remarks>
        public static Vector3D Cross(Vector3D left, Vector3D right)
        {
            Vector3D returnvalue;
            returnvalue.x = left.y * right.z - left.z * right.y;
            returnvalue.y = left.z * right.x - left.x * right.z;
            returnvalue.z = left.x * right.y - left.y * right.x;
            return returnvalue;
        }
        /// <summary>
        /// Gets the Squared <see cref="Magnitude"/> of the Vector3D that is passed.
        /// </summary>
        /// <param name="source">The Vector3D whos Squared Magnitude is te be returned.</param>
        /// <returns>The Squared Magnitude.</returns>
        public static Scalar GetMagnitudeSq(Vector3D source)
        {
            return source.x * source.x + source.y * source.y + source.z * source.z;
        }
        /// <summary>
        /// Gets the <see cref="Magnitude"/> of the Vector3D that is passed.
        /// </summary>
        /// <param name="source">The Vector3D whos Magnitude is te be returned.</param>
        /// <returns>The Magnitude.</returns>
        public static Scalar GetMagnitude(Vector3D source)
        {
            return (Scalar)Math.Sqrt(source.x * source.x + source.y * source.y + source.z * source.z);
        }
        /// <summary>
        /// Sets the <see cref="Magnitude"/> of a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D whose Magnitude is to be changed.</param>
        /// <param name="magnitude">The Magnitude.</param>
        /// <returns>A Vector3D with the new Magnitude</returns>
        public static Vector3D SetMagnitude(Vector3D source, Scalar magnitude)
        {
            Scalar oldmagnitude = GetMagnitude(source);
            if (oldmagnitude > 0)
            {
                return source * (magnitude / oldmagnitude); ;
            }
            else
            {
                return Zero;
            }
        }
        /// <summary>
        /// Negates a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D to be Negated.</param>
        /// <returns>The Negated Vector3D.</returns>
        public static Vector3D Negate(Vector3D source)
        {
            Vector3D returnvalue;
            returnvalue.x = -source.x;
            returnvalue.y = -source.y;
            returnvalue.z = -source.z;
            return returnvalue;
        }
        /// <summary>
        /// This returns the Normalized Vector3D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector3D to be Normalized.</param>
        /// <returns>The Normalized Vector3D. (Unit Vector)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public static Vector3D Normalize(Vector3D source)
        {
            Scalar magnitude = GetMagnitude(source);
            if (magnitude > 0)
            {
                Scalar magnitudeInv = (1 / magnitude);
                Vector3D returnvalue;
                returnvalue.x = source.x * magnitudeInv;
                returnvalue.y = source.y * magnitudeInv;
                returnvalue.z = source.z * magnitudeInv;
                return returnvalue;
            }
            else
            {
                return Vector3D.Zero;
            }
        }
        /// <summary>
        /// Thie Projects the left Vector3D onto the Right Vector3D.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Projected Vector3D.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Projection_%28linear_algebra%29"/></remarks>
        public static Vector3D Project(Vector3D left, Vector3D right)
        {
            Scalar tmp = Dot(left, right) / GetMagnitudeSq(right);
            Vector3D returnvalue;
            returnvalue.x = right.x * tmp;
            returnvalue.y = right.y * tmp;
            returnvalue.z = right.z * tmp;
            return returnvalue;
        }
        #endregion
        #region fields
        internal Scalar x;
        internal Scalar y;
        internal Scalar z;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a New Vector3D Instance on the Stack.
        /// </summary>
        /// <param name="X">The X value.</param>
        /// <param name="Y">The Y value.</param>
        /// <param name="Z">The Z value.</param>
        [AdvanceSystem.ComponentModel.UTCConstructor]
        public Vector3D(Scalar X, Scalar Y, Scalar Z)
        {
            //this.Vector2D = Vector2D.Zero;
            this.x = X;
            this.y = Y;
            this.z = Z;
        }
        public Vector3D(Scalar[] vals)
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
        /// This is the X value.
        /// </summary>
        [XmlAttribute, SoapAttribute]
        public Scalar X
        {
            get { return x; }
            set { x = value; }
        }
        /// <summary>
        /// This is the Y value.
        /// </summary>
        [XmlAttribute, SoapAttribute]
        public Scalar Y
        {
            get { return y; }
            set { y = value; }
        }
        /// <summary>
        /// This is the Z value. 
        /// </summary>
        [XmlAttribute, SoapAttribute]
        public Scalar Z
        {
            get { return z; }
            set { z = value; }
        }
        /// <summary>
        /// The Number of Variables accesable though the indexer.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public int Length { get { return Count; } }
        /// <summary>
        /// Gets or Sets the Magnitude (Length) of the Vector3D. 
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        [XmlIgnore, SoapIgnore]
        [System.ComponentModel.Browsable(false)]
        public Scalar Magnitude
        {
            get
            {
                return (Scalar)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
            }
            set
            {
                this = SetMagnitude(this, value);
            }
        }
        /// <summary>
        /// Gets the Squared Magnitude of the Vector3D.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Scalar MagnitudeSq
        {
            get
            {
                return this.x * this.x + this.y * this.y + this.z * this.z;
            }
        }
        /// <summary>
        /// Gets the Normalized Vector3D. (Unit Vector)
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        [System.ComponentModel.Browsable(false)]
        public Vector3D Normalized
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
            return new Scalar[Count] { x, y, z };
        }
        public void CopyFrom(Scalar[] array, int index)
        {
            x = array[index];
            y = array[index + 1];
            z = array[index + 2];
        }
        public void CopyTo(Scalar[] array, int index)
        {
            array[index] = x;
            array[index + 1] = y;
            array[index + 2] = z;
        }
        #endregion
        #region operators
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Sum of the 2 Vector3Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector3D operator +(Vector3D left, Vector3D right)
        {
            Vector3D returnvalue;
            returnvalue.x = left.x + right.x;
            returnvalue.y = left.y + right.y;
            returnvalue.z = left.z + right.z;
            return returnvalue;
        }
        /// <summary>
        /// Subtracts 2 Vector3Ds.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Difference of the 2 Vector3Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector3D operator -(Vector3D left, Vector3D right)
        {
            Vector3D returnvalue;
            returnvalue.x = left.x - right.x;
            returnvalue.y = left.y - right.y;
            returnvalue.z = left.z - right.z;
            return returnvalue;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector3D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector3D operator *(Vector3D source, Scalar scalar)
        {
            Vector3D returnvalue;
            returnvalue.x = source.x * scalar;
            returnvalue.y = source.y * scalar;
            returnvalue.z = source.z * scalar;
            return returnvalue;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector3D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector3D.</param>
        /// <param name="source">The Vector3D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector3D operator *(Scalar scalar, Vector3D source)
        {
            Vector3D returnvalue;
            returnvalue.x = scalar * source.x;
            returnvalue.y = scalar * source.y;
            returnvalue.z = scalar * source.z;
            return returnvalue;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar operator *(Vector3D left, Vector3D right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z;
        }
        /// <summary>
        ///		Transforms the given 3-D vector by the matrix, projecting the 
        ///		returnvalue back into <i>w</i> = 1.
        ///		<p/>
        ///		This means that the initial <i>w</i> is considered to be 1.0,
        ///		and then all the tree elements of the returnvalueing 3-D vector are
        ///		divided by the returnvalueing <i>w</i>.
        /// </summary>
        /// <param name="matrix">A Matrix4.</param>
        /// <param name="vector">A Vector3D.</param>
        /// <returns>A new vector.</returns>
        public static Vector3D operator *(Matrix4x4 matrix, Vector3D vector)
        {
            Vector3D returnvalue;

            Scalar inverseW = 1.0f / (matrix.m30 * vector.x + matrix.m31 * vector.y + matrix.m32 * vector.z + matrix.m33);
            returnvalue.x = ((matrix.m00 * vector.x) + (matrix.m01 * vector.y) + (matrix.m02 * vector.z) + matrix.m03) * inverseW;
            returnvalue.y = ((matrix.m10 * vector.x) + (matrix.m11 * vector.y) + (matrix.m12 * vector.z) + matrix.m13) * inverseW;
            returnvalue.z = ((matrix.m20 * vector.x) + (matrix.m21 * vector.y) + (matrix.m22 * vector.z) + matrix.m23) * inverseW;

            return returnvalue;
        }
        /// <summary>
        ///		matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D operator *(Matrix3x3 matrix, Vector3D vector)
        {
            Vector3D product;

            product.x = matrix.m00 * vector.x + matrix.m01 * vector.y + matrix.m02 * vector.z;
            product.y = matrix.m10 * vector.x + matrix.m11 * vector.y + matrix.m12 * vector.z;
            product.z = matrix.m20 * vector.x + matrix.m21 * vector.y + matrix.m22 * vector.z;

            return product;
        }
        /// <summary>
        ///		vector * matrix [1x3 * 3x3 = 1x3]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D operator *(Vector3D vector, Matrix3x3 matrix)
        {
            Vector3D product;

            product.x = matrix.m00 * vector.x + matrix.m01 * vector.y + matrix.m02 * vector.z;
            product.y = matrix.m10 * vector.x + matrix.m11 * vector.y + matrix.m12 * vector.z;
            product.z = matrix.m20 * vector.x + matrix.m21 * vector.y + matrix.m22 * vector.z;

            return product;
        }
        /// <summary>
        /// Negates a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D to be Negated.</param>
        /// <returns>The Negated Vector3D.</returns>
        public static Vector3D operator -(Vector3D source)
        {
            Vector3D returnvalue;
            returnvalue.x = -source.x;
            returnvalue.y = -source.y;
            returnvalue.z = -source.z;
            return returnvalue;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Z value of the returnvalueing vector.</returns>
        /// <remarks>
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector3D operator ^(Vector3D left, Vector3D right)
        {
            Vector3D returnvalue;
            returnvalue.x = left.y * right.z - left.z * right.y;
            returnvalue.y = left.z * right.x - left.x * right.z;
            returnvalue.z = left.x * right.y - left.y * right.x;
            return returnvalue;
        }
        /// <summary>
        /// Specifies whether the Vector3Ds contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector3D to test.</param>
        /// <param name="right">The right Vector3D to test.</param>
        /// <returns>true if the Vector3Ds have the same coordinates; otherwise false</returns>
        public static bool operator ==(Vector3D left, Vector3D right)
        {
            return left.x == right.x && left.y == right.y && left.z == right.z;
        }
        /// <summary>
        /// Specifies whether the Vector3Ds do not contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector3D to test.</param>
        /// <param name="right">The right Vector3D to test.</param>
        /// <returns>true if the Vector3Ds do not have the same coordinates; otherwise false</returns>
        public static bool operator !=(Vector3D left, Vector3D right)
        {
            return !(left.x == right.x && left.y == right.y && left.z == right.z);
        }

        public static explicit operator Vector3D(Vector2D source)
        {
            Vector3D returnvalue;
            returnvalue.x = source.X;
            returnvalue.y = source.Y;
            returnvalue.z = 0;
            return returnvalue;
        }

        public static explicit operator Vector3D(Vector4D source)
        {
            Vector3D returnvalue;
            returnvalue.x = source.X;
            returnvalue.y = source.Y;
            returnvalue.z = source.Z;
            return returnvalue;
        }
        #endregion
        #region overrides
        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector3D.
        /// </summary>
        /// <returns>A string representation of a Vector3D.</returns>
        [AdvanceSystem.ComponentModel.UTCFormater]
        public override string ToString()
        {
            return string.Format("({0},{1},{2})", this.x, this.y, this.z);
        }
        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector3D.
        /// </summary>
        /// <returns>A string representation of a Vector3D.</returns>
        public string ToIntegerString()
        {
            return string.Format("({0},{1},{2})", (int)this.x, (int)this.y, (int)this.z);
        }
        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector3D.
        /// </summary>
        /// <returns>A string representation of a Vector3D.</returns>
        public string ToString(bool shortenDecmialPlaces)
        {
            if (shortenDecmialPlaces)
            {
                return string.Format("({0:0.##}, {1:0.##} ,{2:0.##})", this.x, this.y, this.z);
            }
            else
            {
                return ToString();
            }
        }
        [AdvanceSystem.ComponentModel.UTCParser]

        public static Vector3D Parse(string text)
        {
            string[] vals = text.Trim(' ', '(', '[', '<', ')', ']', '>').Split(',');
            
            if (vals.Length != Count)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 3 parts separated by commas in the form (x,y,z) with optional parenthesis.", text));
            }
            else
            {
                try
                {
                    Vector3D returnvalue;
                    returnvalue.x = Scalar.Parse(vals[0].Trim());
                    returnvalue.y = Scalar.Parse(vals[1].Trim());
                    returnvalue.z = Scalar.Parse(vals[2].Trim());
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
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector3D) && (this == (Vector3D)obj);
        }
        #endregion
    }
}