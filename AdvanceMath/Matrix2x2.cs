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
#region Axiom LGPL License
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
http://www.wild-magic.com/

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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;


// NOTE.  The (x,y) coordinate system is assumed to be right-handed.
// where t > 0 indicates a counterclockwise rotation in the zx-plane
//   RZ =  cos(t) -sin(t)
//         sin(t)  cos(t) 
// where t > 0 indicates a counterclockwise rotation in the xy-plane.

namespace AdvanceMath
{




    /// <summary>
    /// A 2x2 matrix which can represent rotations for 2D vectors.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = Matrix2x2.Size, Pack = 0), Serializable]
    public struct Matrix2x2 : IMatrix<Matrix2x2, Vector2D>
    {
        #region const fields
        /// <summary>
        /// The number of rows.
        /// </summary>
        public const int RowCount = 2;
        /// <summary>
        /// The number of columns.
        /// </summary>
        public const int ColumnCount = 2;
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = RowCount * ColumnCount;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region static methods
        #region CLS complient method equivalents
        /// <summary>
        /// Multiply (concatenate) two Matrix3 instances together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 Multiply(Matrix2x2 left, Matrix2x2 right)
        {
            return left * right;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix2x2 Multiply(Matrix2x2 matrix, Scalar scalar)
        {
            return matrix * scalar;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix2x2 Multiply(Scalar scalar, Matrix2x2 matrix)
        {
            return scalar * matrix;
        }
        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 Add(Matrix2x2 left, Matrix2x2 right)
        {
            return left + right;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 Subtract(Matrix2x2 left, Matrix2x2 right)
        {
            return left - right;
        }
        /// <summary>
        /// Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix2x2 Negate(Matrix2x2 matrix)
        {
            return -matrix;
        }
        #endregion

        public static Matrix2x2 FromArray(Scalar[] array)
        {
            Matrix2x2 returnvalue = Identity;
            returnvalue.CopyFrom(array, 0);
            return returnvalue;
        }
        public static Matrix2x2 FromTransposedArray(Scalar[] array)
        {
            Matrix2x2 returnvalue = Identity;
            returnvalue.CopyTransposedFrom(array, 0);
            return returnvalue;
        }

        public static Matrix2x2 FromRotation(Scalar radianAngle)
        {
            Matrix2x2 returnvalue = Identity;
            returnvalue.m00 = MathAdv.Cos(radianAngle);
            returnvalue.m10 = MathAdv.Sin(radianAngle);
            returnvalue.m01 = -returnvalue.m10;
            returnvalue.m11 = returnvalue.m00;
            return returnvalue;
        }
        private static Matrix2x2 FromRotation(Scalar cos, Scalar sin)
        {
            Matrix2x2 returnvalue = Identity;
            returnvalue.m00 = cos;
            returnvalue.m10 = sin;
            returnvalue.m01 = -sin;
            returnvalue.m11 = cos;
            return returnvalue;
        }
        public static Matrix2x2 FromScale(Vector2D scale)
        {
            Matrix2x2 rv = Identity;
            rv.m00 = scale.X;
            rv.m11 = scale.Y;
            return rv;
        }
        #endregion
        #region fields 

        /// <summary>
        /// The X Row or row zero.
        /// </summary>
        [FieldOffset(Vector2D.Size * 0), NonSerialized, XmlIgnore]
        public Vector2D Rx;
        /// <summary>
        /// The Y Row or row one.
        /// </summary>
        [FieldOffset(Vector2D.Size * 1), NonSerialized, XmlIgnore]
        public Vector2D Ry;



        // | m00 m01 |
        // | m10 m11 |
        [FieldOffset(sizeof(Scalar) * 0)]
        public Scalar m00;
        [FieldOffset(sizeof(Scalar) * 1)]
        public Scalar m01;
        [FieldOffset(sizeof(Scalar) * 2)]
        public Scalar m10;
        [FieldOffset(sizeof(Scalar) * 3)]
        public Scalar m11;


        public static readonly Matrix2x2 Identity = new Matrix2x2(1, 0,
                                                                    0, 1);

        public static readonly Matrix2x2 Zero = new Matrix2x2(0, 0, 0, 0);

        #endregion
        #region Constructors

        /// <summary>
        ///		Creates a new Matrix3 with all the specified parameters.
        /// </summary>
        public Matrix2x2(Scalar m00, Scalar m01,
            Scalar m10, Scalar m11)
        {
            this.Rx = Vector2D.XAxis;
            this.Ry = Vector2D.YAxis;
            this.m00 = m00; this.m01 = m01;
            this.m10 = m10; this.m11 = m11;
        }

        /// <summary>
        /// Create a new Matrix from 3 Vertex3 objects.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        public Matrix2x2(Vector2D xAxis, Vector2D yAxis)
        {
            m00 = 1; m01 = 0;
            m10 = 0; m11 = 1;
            this.Rx = xAxis;
            this.Ry = yAxis;
        }
        public Matrix2x2(Scalar[] vals)
        {
            this = Zero;
            this.CopyFrom(vals,0);
        }
        #endregion
        #region Properties
        [XmlIgnore, SoapIgnore]
        public Vector2D Cx
        {
            get
            {
                return new Vector2D(m00, m10);
            }
            set
            {
                this.m00 = value.X;
                this.m10 = value.Y;
            }
        }
        [XmlIgnore, SoapIgnore]
        public Vector2D Cy
        {
            get
            {
                return new Vector2D(m01, m11);
            }
            set
            {
                this.m01 = value.X;
                this.m11 = value.Y;
            }
        }
        public Scalar Determinant
        {
            get
            {
                return Vector2D.ZCross(Rx, Ry);
            }
        }
        public int Length { get { return Count; } }
        public int RowLength { get { return RowCount; } }
        public int ColumnLength { get { return ColumnCount; } }
        /// <summary>
        /// Swap the rows of the matrix with the columns.
        /// </summary>
        /// <returns>A transposed Matrix.</returns>
        public Matrix2x2 Transpose
        {
            get
            {

                return new Matrix2x2(m00, m10,
                    m01, m11);
            }
        }
        public Matrix2x2 Adjoint
        {
            get
            {
                return new Matrix2x2(m11, -m01,
                                     -m10, m00);
            }
        }
        public Matrix2x2 Cofactor
        {
            get
            {
                return new Matrix2x2(m11,-m10 ,
                                     -m01, m00);
            }
        }
        public Matrix2x2 Inverse
        {
            get
            {
                return this.Adjoint * (1.0f / this.Determinant);
            }
        }
        #endregion Properties
        #region Public methods
        public Vector2D GetColumn(int col)
        {
            switch (col)
            {
                case 0:
                    return Cx;
                case 1:
                    return Cy;
                default:
                    throw new ArgumentOutOfRangeException("col", "Attempted to get an invalid column of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public void SetColumn(int col, Vector2D value)
        {
            switch (col)
            {
                case 0:
                    Cx = value;
                    return;
                case 1:
                    Cy = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("col", "Attempted to set an invalid column of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public Vector2D GetRow(int row)
        {
            switch (row)
            {
                case 0:
                    return Rx;
                case 1:
                    return Ry;
                default:
                    throw new ArgumentOutOfRangeException("row", "Attempted to get an invalid row of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public void SetRow(int row, Vector2D value)
        {
            switch (row)
            {
                case 0:
                    Rx = value;
                    return;
                case 1:
                    Ry = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("row", "Attempted to set an invalid row of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }


        public Scalar[,] ToMatrixArray()
        {
            return new Scalar[RowCount, ColumnCount] { { m00, m01 }, { m10, m11 } };
        }
        public Scalar[] ToArray()
        {
            return new Scalar[Count] { m00, m01, m10, m11 };
        }
        public Scalar[] ToTransposedArray()
        {
            return new Scalar[Count] { m00, m10, m01, m11 };
        }

        public Matrix3x3 ToMatrix3x3()
        {
            Matrix3x3 returnvalue = Matrix3x3.Identity;
            returnvalue.m00 = this.m00; returnvalue.m01 = this.m01;
            returnvalue.m10 = this.m10; returnvalue.m11 = this.m11;
            return returnvalue;
        }
        public void CopyTo(Scalar[] array, int index)
        {
            array[index] = m00;
            array[index + 1] = m01;

            array[index + 2] = m10;
            array[index + 3] = m11;
        }
        public void CopyTransposedTo(Scalar[] array, int index)
        {
            array[index] = m00;
            array[index + 1] = m10;

            array[index + 2] = m01;
            array[index + 3] = m11;

        }
        public void CopyFrom(Scalar[] array, int index)
        {
            m00 = array[index];
            m01 = array[index + 1];

            m10 = array[index + 2];
            m11 = array[index + 3];

        }
        public void CopyTransposedFrom(Scalar[] array, int index)
        {
            m00 = array[index];
            m10 = array[index + 1];

            m01 = array[index + 2];
            m11 = array[index + 3];

        }
        #endregion
        #region Indexors

        /// <summary>
        /// Indexer for accessing the matrix like a 2d array (i.e. matrix[2,3]).
        /// </summary>
        public Scalar this[int row, int col]
        {
            get
            {
                Debug.Assert((row >= 0 && row < RowCount), "Attempted to get a Matrix" + RowCount + "x" + ColumnCount + " value with Row indexer out of bounds.");
                Debug.Assert((col >= 0 && col < ColumnCount), "Attempted to get a  Matrix" + RowCount + "x" + ColumnCount + " value with Column indexer out of bounds.");
                unsafe
                {
                    fixed (Scalar* pM = &m00)
                    {
                        return pM[(ColumnCount * row) + col];
                    }
                }
            }
            set
            {
                Debug.Assert((row >= 0 && row < RowCount), "Attempted to set a Matrix" + RowCount + "x" + ColumnCount + " value with Row indexer out of bounds.");
                Debug.Assert((col >= 0 && col < ColumnCount), "Attempted to set a  Matrix" + RowCount + "x" + ColumnCount + " value with Column indexer out of bounds.");

                unsafe
                {
                    fixed (Scalar* pM = &m00)
                    {
                        pM[(ColumnCount * row) + col] = value;
                    }
                }
            }
        }

        /// <summary>
        ///		Allows the Matrix to be accessed linearly (m[0] -> m[8]).  
        /// </summary>
        public Scalar this[int index]
        {
            get
            {
                Debug.Assert(index < Count, "Attempt to get a index of a Matrix" + RowCount + "x" + ColumnCount + " greater than " + (Count - 1) + ".");
                Debug.Assert(index >= 0, "Attempt to get a index of a Matrix" + RowCount + "x" + ColumnCount + " less than 0.");
                unsafe
                {
                    fixed (Scalar* pMatrix = &m00)
                    {
                        return pMatrix[index];
                    }
                }
            }
            set
            {
                Debug.Assert(index < Count, "Attempt to set a index of a Matrix" + RowCount + "x" + ColumnCount + " greater than " + (Count - 1) + ".");
                Debug.Assert(index >= 0, "Attempt to set a index of a Matrix" + RowCount + "x" + ColumnCount + " less than 0."); unsafe
                {
                    fixed (Scalar* pMatrix = &m00)
                    {
                        pMatrix[index] = value;
                    }
                }
            }
        }
        #endregion
        #region Operator overloads





        /// <summary>
        /// Multiply (concatenate) two Matrix3 instances together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 operator *(Matrix2x2 left, Matrix2x2 right)
        {

            Matrix2x2 returnvalue = new Matrix2x2();

            returnvalue.m00 = left.m00 * right.m00 + left.m01 * right.m10;
            returnvalue.m01 = left.m00 * right.m01 + left.m01 * right.m11;

            returnvalue.m10 = left.m10 * right.m00 + left.m11 * right.m10;
            returnvalue.m11 = left.m10 * right.m01 + left.m11 * right.m11;

            return returnvalue;
        }


        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 operator +(Matrix2x2 left, Matrix2x2 right)
        {
            Matrix2x2 returnvalue = new Matrix2x2();

            returnvalue.Rx = left.Rx + right.Rx;
            returnvalue.Ry = left.Ry + right.Ry;

            return returnvalue;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 operator -(Matrix2x2 left, Matrix2x2 right)
        {
            Matrix2x2 returnvalue = new Matrix2x2();

            returnvalue.Rx = left.Rx - right.Rx;
            returnvalue.Ry = left.Ry - right.Ry;

            return returnvalue;
        }

        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix2x2 operator *(Matrix2x2 matrix, Scalar scalar)
        {
            Matrix2x2 returnvalue = new Matrix2x2();

            returnvalue.m00 = matrix.m00 * scalar;
            returnvalue.m01 = matrix.m01 * scalar;

            returnvalue.m10 = matrix.m10 * scalar;
            returnvalue.m11 = matrix.m11 * scalar;

            return returnvalue;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix2x2 operator *(Scalar scalar, Matrix2x2 matrix)
        {
            Matrix2x2 returnvalue = new Matrix2x2();

            returnvalue.m00 = matrix.m00 * scalar;
            returnvalue.m01 = matrix.m01 * scalar;

            returnvalue.m10 = matrix.m10 * scalar;
            returnvalue.m11 = matrix.m11 * scalar;

            return returnvalue;
        }

        /// <summary>
        /// Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix2x2 operator -(Matrix2x2 matrix)
        {
            Matrix2x2 returnvalue = new Matrix2x2();

            returnvalue.m00 = -matrix.m00;
            returnvalue.m01 = -matrix.m01;

            returnvalue.m10 = -matrix.m10;
            returnvalue.m11 = -matrix.m11;

            return returnvalue;
        }

        /// <summary>
        /// 	Test two matrices for (value) equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix2x2 left, Matrix2x2 right)
        {
            return (
                left.m00 == right.m00 && left.m01 == right.m01 &&
                left.m10 == right.m10 && left.m11 == right.m11);
        }

        public static bool operator !=(Matrix2x2 left, Matrix2x2 right)
        {
            return !(left == right);
        }

        public static explicit operator Matrix2x2(Matrix3x3 source)
        {
            Matrix2x2 returnvalue = new Matrix2x2();

            returnvalue.m00 = source.m00;
            returnvalue.m01 = source.m01;

            returnvalue.m10 = source.m10;
            returnvalue.m11 = source.m11;

            return returnvalue;
        }

        #endregion
        #region Object overloads

        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Matrix4.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat(" | {0} {1} |\n", m00, m01);
            builder.AppendFormat(" | {0} {1} |\n", m10, m11);

            return builder.ToString();
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
            int hashCode = 0;

            unsafe
            {
                fixed (Scalar* pM = &m00)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        hashCode ^= (int)(pM[i]);
                    }
                }
                return hashCode;
            }
        }

        /// <summary>
        ///		Compares this Matrix to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix2x2)
                return (this == (Matrix2x2)obj);
            else
                return false;
        }
        #endregion
    }
}


