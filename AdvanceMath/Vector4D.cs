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
    /// (NOT USED BY PHYSICS2D)
    /// </summary>
    /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29"/></remarks>
    //[StructLayout(LayoutKind.Sequential), Serializable]
    [StructLayout(LayoutKind.Sequential, Size = Vector4D.Size, Pack = 0), Serializable]
    //[System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.VectorConverter<Vector4D>))]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public struct Vector4D : IVector<Vector4D>
    {
        #region const fields
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = 4;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region readonly fields
        /// <summary>
        /// Vector4D(0,0,0,0)
        /// </summary>
        public static readonly Vector4D Origin = new Vector4D();
        /// <summary>
        /// Vector4D(0,0,0,0)
        /// </summary>
        public static readonly Vector4D Zero = new Vector4D();
        /// <summary>
        /// Vector4D(1,0,0,0)
        /// </summary>
        public static readonly Vector4D XAxis = new Vector4D(1, 0, 0, 0);
        /// <summary>
        /// Vector4D(0,1,0,0)
        /// </summary>
        public static readonly Vector4D YAxis = new Vector4D(0, 1, 0, 0);
        /// <summary>
        /// Vector4D(0,0,1,0)
        /// </summary>
        public static readonly Vector4D ZAxis = new Vector4D(0, 0, 1, 0);
        /// <summary>
        /// Vector4D(0,0,0,1)
        /// </summary>
        public static readonly Vector4D WAxis = new Vector4D(0, 0, 0, 1);
        #endregion
        #region static methods
        public static Vector4D FromArray(Scalar[] array)
        {
            Vector4D returnvalue = Vector4D.Zero;
            returnvalue.CopyFrom(array, 0);
            return returnvalue;
        }
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Sum of the 2 Vector4Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector4D Add(Vector4D left, Vector4D right)
        {
            Vector4D returnvalue;
            returnvalue.x = left.x + right.x;
            returnvalue.y = left.y + right.y;
            returnvalue.z = left.z + right.z;
            returnvalue.w = left.w + right.w;
            return returnvalue;
        }
        /// <summary>
        /// Subtracts 2 Vector4Ds.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Difference of the 2 Vector4Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector4D Subtract(Vector4D left, Vector4D right)
        {
            Vector4D returnvalue;
            returnvalue.x = left.x - right.x;
            returnvalue.y = left.y - right.y;
            returnvalue.z = left.z - right.z;
            returnvalue.w = left.w - right.w;
            return returnvalue;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector4D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector4D Multiply(Vector4D source, Scalar scalar)
        {
            Vector4D returnvalue;
            returnvalue.x = source.x * scalar;
            returnvalue.y = source.y * scalar;
            returnvalue.z = source.z * scalar;
            returnvalue.w = source.w * scalar;
            return returnvalue;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar Dot(Vector4D left, Vector4D right)
        {
            return left.y * right.y + left.x * right.x + left.z * right.z + left.w * right.w;
        }
        /// <summary>
        /// Gets the Squared <see cref="Magnitude"/> of the Vector4D that is passed.
        /// </summary>
        /// <param name="source">The Vector4D whos Squared Magnitude is te be returned.</param>
        /// <returns>The Squared Magnitude.</returns>
        public static Scalar GetMagnitudeSq(Vector4D source)
        {
            return source.x * source.x + source.y * source.y + source.z * source.z + source.w * source.w;
        }
        /// <summary>
        /// Gets the <see cref="Magnitude"/> of the Vector4D that is passed.
        /// </summary>
        /// <param name="source">The Vector4D whos Magnitude is te be returned.</param>
        /// <returns>The Magnitude.</returns>
        public static Scalar GetMagnitude(Vector4D source)
        {
            return (Scalar)Math.Sqrt(source.x * source.x + source.y * source.y + source.z * source.z + source.w * source.w);
        }
        /// <summary>
        /// Sets the <see cref="Magnitude"/> of a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D whose Magnitude is to be changed.</param>
        /// <param name="magnitude">The Magnitude.</param>
        /// <returns>A Vector4D with the new Magnitude</returns>
        public static Vector4D SetMagnitude(Vector4D source, Scalar magnitude)
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
        /// Negates a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D to be Negated.</param>
        /// <returns>The Negated Vector4D.</returns>
        public static Vector4D Negate(Vector4D source)
        {
            Vector4D returnvalue;
            returnvalue.x = -source.x;
            returnvalue.y = -source.y;
            returnvalue.z = -source.z;
            returnvalue.w = -source.w;
            return returnvalue;
        }
        /// <summary>
        /// This returns the Normalized Vector4D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector4D to be Normalized.</param>
        /// <returns>The Normalized Vector4D. (Unit Vector)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public static Vector4D Normalize(Vector4D source)
        {
            Scalar magnitude = GetMagnitude(source);
            if (magnitude > 0)
            {
                return source * (1 / magnitude);
            }
            else
            {
                return Vector4D.Zero;
            }
        }
        /// <summary>
        /// Thie Projects the left Vector4D onto the Right Vector4D.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Projected Vector4D.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Projection_%28linear_algebra%29"/></remarks>
        public static Vector4D Project(Vector4D left, Vector4D right)
        {
            return ((left * right) / GetMagnitudeSq(right)) * right;
        }



        public static Vector4D TripleCross(Vector4D top, Vector4D middle, Vector4D bottom)
        {
            Vector4D returnvalue;
            returnvalue.x = Matrix3x3.GetDeterminant(
                top.y, top.z, top.w,
                 middle.y, middle.z, middle.w,
                 bottom.y, bottom.z, bottom.w);

            returnvalue.y = -Matrix3x3.GetDeterminant(
                top.x, top.z, top.w,
                middle.x, middle.z, middle.w,
                bottom.x, bottom.z, bottom.w);

            returnvalue.z = Matrix3x3.GetDeterminant(
                top.x, top.y, top.w,
                middle.x, middle.y, middle.w,
                bottom.x, bottom.y, bottom.w);

            returnvalue.w = -Matrix3x3.GetDeterminant(
                top.x, top.y, top.z,
                middle.x, middle.y, middle.z,
                bottom.x, bottom.y, bottom.z);
            return returnvalue;
        }


        #endregion
        #region fields
        internal Scalar x;
        internal Scalar y;
        internal Scalar z;
        internal Scalar w;

        #endregion
        #region constructors
        /// <summary>
        /// Creates a New Vector4D Instance on the Stack.
        /// </summary>
        /// <param name="X">The X value.</param>
        /// <param name="Y">The Y value.</param>
        /// <param name="Z">The Z value.</param>
        /// <param name="W">The W value.</param>
        [AdvanceSystem.ComponentModel.UTCConstructor]
        public Vector4D(Scalar X, Scalar Y, Scalar Z, Scalar W)
        {
            this.x = X;
            this.y = Y;
            this.z = Z;
            this.w = W;
        }
        public Vector4D(Scalar[] vals)
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
                Debug.Assert(index < Count, "Attempt to get a index of a Vector" + Count + "D greater than " + (Count-1) + ".");
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
        /// This is the W value. 
        /// </summary>
        [XmlAttribute, SoapAttribute]
        public Scalar W
        {
            get { return w; }
            set { w = value; }
        }


        /// <summary>
        /// The Number of Variables accesable though the indexer.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public int Length { get { return Count; } }
        /// <summary>
        /// Gets or Sets the Magnitude (Length) of the Vector4D. 
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        [XmlIgnore, SoapIgnore]
        [System.ComponentModel.Browsable(false)]
        public Scalar Magnitude
        {
            get
            {
                return (Scalar)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w);
            }
            set
            {
                this = SetMagnitude(this, value);
            }
        }
        /// <summary>
        /// Gets the Squared Magnitude of the Vector4D.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        public Scalar MagnitudeSq
        {
            get
            {
                return this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
            }
        }
        /// <summary>
        /// Gets the Normalized Vector4D. (Unit Vector)
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        [System.ComponentModel.Browsable(false)]
        public Vector4D Normalized
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
            return new Scalar[Count] { x, y, z, w };
        }
        public void CopyFrom(Scalar[] array, int index)
        {
            x = array[index];
            y = array[index + 1];
            z = array[index + 2];
            w = array[index + 3];
        }
        public void CopyTo(Scalar[] array, int index)
        {
            array[index] = x;
            array[index + 1] = y;
            array[index + 2] = z;
            array[index + 3] = w;
        }
        #endregion
        #region operators
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Sum of the 2 Vector4Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector4D operator +(Vector4D left, Vector4D right)
        {
            Vector4D returnvalue;
            returnvalue.x = left.x + right.x;
            returnvalue.y = left.y + right.y;
            returnvalue.z = left.z + right.z;
            returnvalue.w = left.w + right.w;
            return returnvalue;
        }
        /// <summary>
        /// Subtracts 2 Vector4Ds.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Difference of the 2 Vector4Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector4D operator -(Vector4D left, Vector4D right)
        {
            Vector4D returnvalue;
            returnvalue.x = left.x - right.x;
            returnvalue.y = left.y - right.y;
            returnvalue.z = left.z - right.z;
            returnvalue.w = left.w - right.w;
            return returnvalue;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector4D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector4D operator *(Vector4D source, Scalar scalar)
        {
            Vector4D returnvalue;
            returnvalue.x = source.x * scalar;
            returnvalue.y = source.y * scalar;
            returnvalue.z = source.z * scalar;
            returnvalue.w = source.w * scalar;
            return returnvalue;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector4D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector4D.</param>
        /// <param name="source">The Vector4D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector4D operator *(Scalar scalar, Vector4D source)
        {
            Vector4D returnvalue;
            returnvalue.x = scalar * source.x;
            returnvalue.y = scalar * source.y;
            returnvalue.z = scalar * source.z;
            returnvalue.w = scalar * source.w;
            return returnvalue;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar operator *(Vector4D left, Vector4D right)
        {
            return left.x * right.x + left.y * right.y + left.z * right.z + left.w * right.w;
        }


        public static Vector4D operator *(Matrix4x4 matrix, Vector4D vector)
        {
            Vector4D returnvalue;

            returnvalue.x = vector.x * matrix.m00 + vector.y * matrix.m01 + vector.z * matrix.m02 + vector.w * matrix.m03;
            returnvalue.y = vector.x * matrix.m10 + vector.y * matrix.m11 + vector.z * matrix.m12 + vector.w * matrix.m13;
            returnvalue.z = vector.x * matrix.m20 + vector.y * matrix.m21 + vector.z * matrix.m22 + vector.w * matrix.m23;
            returnvalue.w = vector.x * matrix.m30 + vector.y * matrix.m31 + vector.z * matrix.m32 + vector.w * matrix.m33;

            return returnvalue;
        }

        public static Vector4D operator *(Vector4D vector, Matrix4x4 matrix)
        {
            Vector4D returnvalue;

            returnvalue.x = vector.x * matrix.m00 + vector.y * matrix.m10 + vector.z * matrix.m20 + vector.w * matrix.m30;
            returnvalue.y = vector.x * matrix.m01 + vector.y * matrix.m11 + vector.z * matrix.m21 + vector.w * matrix.m31;
            returnvalue.z = vector.x * matrix.m02 + vector.y * matrix.m12 + vector.z * matrix.m22 + vector.w * matrix.m32;
            returnvalue.w = vector.x * matrix.m03 + vector.y * matrix.m13 + vector.z * matrix.m23 + vector.w * matrix.m33;

            return returnvalue;
        }

        /// <summary>
        /// Negates a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D to be Negated.</param>
        /// <returns>The Negated Vector4D.</returns>
        public static Vector4D operator -(Vector4D source)
        {
            Vector4D returnvalue;
            returnvalue.x = -source.x;
            returnvalue.y = -source.y;
            returnvalue.z = -source.z;
            returnvalue.w = -source.w;
            return returnvalue;
        }
        /// <summary>
        /// Specifies whether the Vector4Ds contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector4D to test.</param>
        /// <param name="right">The right Vector4D to test.</param>
        /// <returns>true if the Vector4Ds have the same coordinates; otherwise false</returns>
        public static bool operator ==(Vector4D left, Vector4D right)
        {
            return left.x == right.x && left.y == right.y && left.z == right.z && left.w == right.w;
        }
        /// <summary>
        /// Specifies whether the Vector4Ds do not contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector4D to test.</param>
        /// <param name="right">The right Vector4D to test.</param>
        /// <returns>true if the Vector4Ds do not have the same coordinates; otherwise false</returns>
        public static bool operator !=(Vector4D left, Vector4D right)
        {
            return !(left.x == right.x && left.y == right.y && left.z == right.z && left.w == right.w);
        }


        public static explicit operator Vector4D(Vector3D source)
        {
            Vector4D returnvalue;
            returnvalue.x = source.X;
            returnvalue.y = source.Y;
            returnvalue.z = source.Z;
            returnvalue.w = 1;
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
            return string.Format("({0},{1},{2},{3})", this.x, this.y, this.z, this.w);
        }
        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Vector3D.
        /// </summary>
        /// <returns>A string representation of a Vector3D.</returns>
        public string ToIntegerString()
        {
            return string.Format("({0},{1},{2},{3})", (int)this.x, (int)this.y, (int)this.z, (int)this.w);
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
                return string.Format("({0:0.##}, {1:0.##}, {2:0.##}, {3:0.##})", this.x, this.y, this.z, this.w);
            }
            else
            {
                return ToString();
            }
        }
        [AdvanceSystem.ComponentModel.UTCParser]
        public static Vector4D Parse(string text)
        {
            string[] vals = text.Trim(' ', '(', '[', '<', ')', ']', '>').Split(',');
            
            if (vals.Length != Count)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 4 parts separated by commas in the form (x,y,z,w) with optional parenthesis.", text));
            }
            else
            {
                try
                {
                    Vector4D returnvalue;
                    returnvalue.x = Scalar.Parse(vals[0].Trim());
                    returnvalue.y = Scalar.Parse(vals[1].Trim());
                    returnvalue.z = Scalar.Parse(vals[2].Trim());
                    returnvalue.w = Scalar.Parse(vals[3].Trim());
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
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector4D) && (this == (Vector4D)obj);
        }
        #endregion
    }
}