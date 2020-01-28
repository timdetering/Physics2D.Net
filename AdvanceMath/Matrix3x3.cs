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
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;


// NOTE.  The (x,y,z) coordinate system is assumed to be right-handed.
// Coordinate axis rotation matrices are of the form
//   RX =    1       0       0
//           0     cos(t) -sin(t)
//           0     sin(t)  cos(t)
// where t > 0 indicates a counterclockwise rotation in the yz-plane
//   RY =  cos(t)    0     sin(t)
//           0       1       0
//        -sin(t)    0     cos(t)
// where t > 0 indicates a counterclockwise rotation in the zx-plane
//   RZ =  cos(t) -sin(t)    0
//         sin(t)  cos(t)    0
//           0       0       1
// where t > 0 indicates a counterclockwise rotation in the xy-plane.

namespace AdvanceMath
{
    /// <summary>
    /// A 3x3 matrix which can represent rotations around axes.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = Matrix3x3.Size, Pack = 0), Serializable]
    public struct Matrix3x3 : IMatrix<Matrix3x3, Vector3D>
    {
        #region const fields
        /// <summary>
        /// The number of rows.
        /// </summary>
        public const int RowCount = 3;
        /// <summary>
        /// The number of columns.
        /// </summary>
        public const int ColumnCount = 3;
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
        public static Matrix3x3 Multiply(Matrix3x3 left, Matrix3x3 right)
        {
            return left * right;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix3x3 Multiply(Matrix3x3 matrix, Scalar scalar)
        {
            return matrix * scalar;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix3x3 Multiply(Scalar scalar, Matrix3x3 matrix)
        {
            return scalar * matrix;
        }
        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3x3 Add(Matrix3x3 left, Matrix3x3 right)
        {
            return left + right;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3x3 Subtract(Matrix3x3 left, Matrix3x3 right)
        {
            return left - right;
        }
        /// <summary>
        /// Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 Negate(Matrix3x3 matrix)
        {
            return -matrix;
        }
        #endregion
        public static Matrix3x3 FromArray(Scalar[] array)
        {
            Matrix3x3 returnvalue = Identity;
            returnvalue.CopyFrom(array, 0);
            return returnvalue;
        }
        public static Matrix3x3 FromTransposedArray(Scalar[] array)
        {
            Matrix3x3 returnvalue = Identity;
            returnvalue.CopyTransposedFrom(array, 0);
            return returnvalue;
        }

        public static Matrix3x3 FromRotationX(Scalar radianAngle)
        {
            Matrix3x3 returnvalue = Identity;
            returnvalue.m11 = MathAdv.Cos(radianAngle);
            returnvalue.m21 = MathAdv.Sin(radianAngle);
            returnvalue.m12 = -returnvalue.m21;
            returnvalue.m22 = returnvalue.m11;
            return returnvalue;
        }
        private static Matrix3x3 FromRotationX(Scalar cos, Scalar sin)
        {
            Matrix3x3 returnvalue = Identity;
            returnvalue.m11 = cos;
            returnvalue.m21 = sin;
            returnvalue.m12 = -sin;
            returnvalue.m22 = cos;
            return returnvalue;
        }
        public static Matrix3x3 FromRotationY(Scalar radianAngle)
        {
            Matrix3x3 returnvalue = Identity;
            returnvalue.m00 = MathAdv.Cos(radianAngle);
            returnvalue.m22 = returnvalue.m00;
            returnvalue.m02 = MathAdv.Sin(radianAngle);
            returnvalue.m20 = -returnvalue.m02;
            return returnvalue;
        }
        private static Matrix3x3 FromRotationY(Scalar cos, Scalar sin)
        {
            Matrix3x3 returnvalue = Identity;
            returnvalue.m00 = cos;
            returnvalue.m22 = cos;
            returnvalue.m02 = sin;
            returnvalue.m20 = -sin;
            return returnvalue;
        }
        public static Matrix3x3 FromRotationZ(Scalar radianAngle)
        {
            Matrix3x3 returnvalue = Identity;
            returnvalue.m00 = MathAdv.Cos(radianAngle);
            returnvalue.m10 = MathAdv.Sin(radianAngle);
            returnvalue.m01 = -returnvalue.m10;
            returnvalue.m11 = returnvalue.m00;
            return returnvalue;
        }
        private static Matrix3x3 FromRotationZ(Scalar cos, Scalar sin)
        {
            Matrix3x3 returnvalue = Identity;
            returnvalue.m00 = cos;
            returnvalue.m10 = sin;
            returnvalue.m01 = -sin;
            returnvalue.m11 = cos;
            return returnvalue;
        }
        public static Matrix3x3 FromRotationAxisUsingAtan(Scalar radianAngle, Vector3D axis)
        {
            Scalar zAngle;
            Scalar yAngle;
            if (axis.X == 0)
            {
                if (axis.Y == 0)
                {
                    return FromRotationZ(radianAngle);
                }
                else
                {
                    zAngle = MathAdv.HALF_PI;
                    yAngle = (Scalar)Math.Atan(axis.Z / axis.Y);
                }
            }
            else
            {
                zAngle = (Scalar)Math.Atan(axis.Y / axis.X);
                yAngle = (Scalar)Math.Atan(axis.Z / Math.Sqrt(axis.X * axis.X + axis.Y * axis.Y));
            }
            return FromRotationZ(-zAngle) *
                FromRotationY(-yAngle) *
                FromRotationX(radianAngle) *
                FromRotationY(yAngle) *
                FromRotationZ(zAngle);
        }

        public static Matrix3x3 FromRotationAxis(Scalar radianAngle, Vector3D axis)
        {
            Matrix3x3 first = FromGraphicsBookMethod2(Vector3D.Zero, axis, new Vector3D(axis.Z, axis.X, axis.Y));
            return first.Inverse * FromRotationZ(radianAngle) * first;
        }

        internal static Matrix3x3 FromGraphicsBookMethod1(Vector3D origin, Vector3D positiveZAxis, Vector3D positiveY)
        {
            Vector3D P12 = positiveZAxis - origin;
            Vector3D P13 = positiveY - origin;

            Scalar D = MathAdv.Sqrt(P12.Z * P12.Z + P12.X * P12.X);
            Scalar cos = P12.Z / D;
            Scalar sin = -P12.X / D;
            Matrix3x3 aboutY = FromRotationY(cos, sin);

            Vector3D P12r = aboutY * P12;


            Scalar D2 = P12r.Magnitude;
            Scalar cos2 = P12r.Z / D2;
            Scalar sin2 = P12r.Y / D2;
            Matrix3x3 aboutX = FromRotationX(cos2, sin2);

            Matrix3x3 returnvalue = aboutX * aboutY;
            Vector3D P13rr = returnvalue * P13;

            Scalar D3 = MathAdv.Sqrt(P13rr.X * P13rr.X + P13rr.Y * P13rr.Y);
            Scalar cos3 = P13rr.Y / D3;
            Scalar sin3 = P13rr.X / D3;
            Matrix3x3 aboutZ = FromRotationZ(cos3, sin3);

            return aboutZ * returnvalue;
        }
        internal static Matrix3x3 FromGraphicsBookMethod2(Vector3D origin, Vector3D positiveZAxis, Vector3D onPositiveY)
        {
            Matrix3x3 rv = Identity;
            rv.Rz = Vector3D.Normalize(positiveZAxis - origin);
            rv.Rx = Vector3D.Normalize((onPositiveY - origin) ^ rv.Rz);
            rv.Ry = Vector3D.Normalize(rv.Rz ^ rv.Rx);
            return rv;
        }


        public static Matrix3x3 FromScale(Vector3D scale)
        {
            Matrix3x3 rv = Identity;
            rv.m00 = scale.X;
            rv.m11 = scale.Y;
            rv.m11 = scale.Z;
            return rv;
        }
        public static Matrix3x3 FromScale(Vector2D scale)
        {
            Matrix3x3 rv = Identity;
            rv.m00 = scale.X;
            rv.m11 = scale.Y;
            return rv;
        }
        public static Matrix3x3 FromTranslate2D(Vector2D value)
        {
            Matrix3x3 rv = Identity;
            rv.m02 = value.X;
            rv.m12 = value.Y;
            return rv;
        }
        public static Matrix3x3 FromShear3D(Vector2D value)
        {
            Matrix3x3 rv = Identity;
            rv.m02 = value.X;
            rv.m12 = value.Y;
            return rv;
        }

        public static Scalar GetDeterminant(Scalar m00, Scalar m01, Scalar m02,
                                     Scalar m10, Scalar m11, Scalar m12,
                                     Scalar m20, Scalar m21, Scalar m22)
        {
            Scalar cofactor00 = m11 * m22 - m12 * m21;
            Scalar cofactor10 = m12 * m20 - m10 * m22;
            Scalar cofactor20 = m10 * m21 - m11 * m20;
            Scalar returnvalue =
                m00 * cofactor00 +
                m01 * cofactor10 +
                m02 * cofactor20;
            return returnvalue;
        }
        public static Scalar GetDeterminant(Vector3D Rx, Vector3D Ry, Vector3D Rz)
        {
            Scalar cofactor00 = Ry.Y * Rz.Z - Ry.Z * Rz.Y;
            Scalar cofactor10 = Ry.Z * Rz.X - Ry.X * Rz.Z;
            Scalar cofactor20 = Ry.X * Rz.Y - Ry.Y * Rz.X;
            Scalar returnvalue =
                Rx.X * cofactor00 +
                Rx.Y * cofactor10 +
                Rx.Z * cofactor20;
            return returnvalue;
        }

        /// <summary>
        ///    Constructs this Matrix from 3 euler angles, in degrees.
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        public static Matrix3x3 FromEulerAnglesXYZ(Scalar yaw, Scalar pitch, Scalar roll)
        {
            //Scalar cos = MathAdv.Cos(yaw);
            // Scalar sin = MathAdv.Sin(yaw);
            Matrix3x3 xMat = FromRotationX(yaw);
            //new Matrix3x3(1, 0, 0,
            //                             0, cos, -sin,
            //                             0, sin, cos);

            //cos = MathAdv.Cos(pitch);
            //sin = MathAdv.Sin(pitch);
            Matrix3x3 yMat = FromRotationY(pitch);
            //new Matrix3x3(cos, 0, sin,
            //  0, 1, 0,
            //  -sin, 0, cos);

            //cos = MathAdv.Cos(roll);
            //sin = MathAdv.Sin(roll);
            Matrix3x3 zMat = FromRotationZ(roll);
            //new Matrix3x3(cos, -sin, 0,
            //                           sin, cos, 0,
            //                          0, 0, 1);

            return xMat * (yMat * zMat);
        }
        #endregion
        #region fields


        [FieldOffset(Vector3D.Size * 0), NonSerialized]
        public Vector3D Rx;
        [FieldOffset(Vector3D.Size * 1), NonSerialized]
        public Vector3D Ry;
        [FieldOffset(Vector3D.Size * 2), NonSerialized]
        public Vector3D Rz;


        // | m00 m01 m02 |
        // | m10 m11 m12 |
        // | m20 m21 m22 |
        [FieldOffset(sizeof(Scalar) * 0), XmlIgnore, SoapIgnore]
        public Scalar m00;
        [FieldOffset(sizeof(Scalar) * 1), XmlIgnore, SoapIgnore]
        public Scalar m01;
        [FieldOffset(sizeof(Scalar) * 2), XmlIgnore, SoapIgnore]
        public Scalar m02;
        [FieldOffset(sizeof(Scalar) * 3), XmlIgnore, SoapIgnore]
        public Scalar m10;
        [FieldOffset(sizeof(Scalar) * 4), XmlIgnore, SoapIgnore]
        public Scalar m11;
        [FieldOffset(sizeof(Scalar) * 5), XmlIgnore, SoapIgnore]
        public Scalar m12;
        [FieldOffset(sizeof(Scalar) * 6), XmlIgnore, SoapIgnore]
        public Scalar m20;
        [FieldOffset(sizeof(Scalar) * 7), XmlIgnore, SoapIgnore]
        public Scalar m21;
        [FieldOffset(sizeof(Scalar) * 8), XmlIgnore, SoapIgnore]
        public Scalar m22;

        public static readonly Matrix3x3 Identity = new Matrix3x3(1, 0, 0,
            0, 1, 0,
            0, 0, 1);

        public static readonly Matrix3x3 Zero = new Matrix3x3(0, 0, 0,
            0, 0, 0,
            0, 0, 0);

        #endregion
        #region Constructors

        /// <summary>
        ///		Creates a new Matrix3 with all the specified parameters.
        /// </summary>
        public Matrix3x3(Scalar m00, Scalar m01, Scalar m02,
            Scalar m10, Scalar m11, Scalar m12,
            Scalar m20, Scalar m21, Scalar m22)
        {
            this.Rx = Vector3D.XAxis;
            this.Ry = Vector3D.YAxis;
            this.Rz = Vector3D.ZAxis;

            this.m00 = m00; this.m01 = m01; this.m02 = m02;
            this.m10 = m10; this.m11 = m11; this.m12 = m12;
            this.m20 = m20; this.m21 = m21; this.m22 = m22;
        }

        /// <summary>
        /// Create a new Matrix from 3 Vertex3 objects.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        public Matrix3x3(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
        {
            //m00 = xAxis.X; m01 = yAxis.X; m02 = zAxis.X;
            //m10 = xAxis.Y; m11 = yAxis.Y; m12 = zAxis.Y;
            // m20 = xAxis.Z; m21 = yAxis.Z; m22 = zAxis.Z;

            m00 = 0; m01 = 0; m02 = 0;
            m10 = 0; m11 = 0; m12 = 0;
            m20 = 0; m21 = 0; m22 = 0;

            this.Rx = xAxis;
            this.Ry = yAxis;
            this.Rz = zAxis;


        }
        public Matrix3x3(Scalar[] vals)
        {
            this = Zero;
            this.CopyFrom(vals,0);
        }
        #endregion
        #region Properties
        [XmlIgnore, SoapIgnore]
        public Vector3D Cx
        {
            get
            {
                return new Vector3D(m00, m10, m20);
            }
            set
            {
                this.m00 = value.X;
                this.m10 = value.Y;
                this.m20 = value.Z;
            }
        }
        [XmlIgnore, SoapIgnore]
        public Vector3D Cy
        {
            get
            {
                return new Vector3D(m01, m11, m21);
            }
            set
            {
                this.m01 = value.X;
                this.m11 = value.Y;
                this.m21 = value.Z;
            }
        }
        [XmlIgnore, SoapIgnore]
        public Vector3D Cz
        {
            get
            {
                return new Vector3D(m02, m12, m22);
            }
            set
            {
                this.m02 = value.X;
                this.m12 = value.Y;
                this.m22 = value.Z;
            }
        }
        public Scalar Determinant
        {
            get
            {
                Scalar cofactor00 = m11 * m22 - m12 * m21;
                Scalar cofactor10 = m12 * m20 - m10 * m22;
                Scalar cofactor20 = m10 * m21 - m11 * m20;

                Scalar returnvalue =
                    m00 * cofactor00 +
                    m01 * cofactor10 +
                    m02 * cofactor20;
                return returnvalue;
            }
        }
        public int Length { get { return Count; } }
        public int RowLength { get { return RowCount; } }
        public int ColumnLength { get { return ColumnCount; } }
        /// <summary>
        /// Swap the rows of the matrix with the columns.
        /// </summary>
        /// <returns>A transposed Matrix.</returns>
        public Matrix3x3 Transpose
        {
            get
            {
                return new Matrix3x3(m00, m10, m20,
                    m01, m11, m21,
                    m02, m12, m22);
            }
        }
        public Matrix3x3 Cofactor
        {
            get
            {
                Matrix3x3 returnvalue = new Matrix3x3();
                returnvalue.Rx = this.Ry ^ this.Rz;
                returnvalue.Ry = -this.Rx ^ this.Rz;
                returnvalue.Rz = this.Rx ^ this.Ry;
                return returnvalue;
            }
        }
        public Matrix3x3 Adjoint
        {
            get
            {
                Matrix3x3 returnvalue = new Matrix3x3();
                returnvalue.Cx = this.Ry ^ this.Rz;
                returnvalue.Cy = -this.Rx ^ this.Rz;
                returnvalue.Cz = this.Rx ^ this.Ry;
                return returnvalue;
            }
        }
        public Matrix3x3 Inverse
        {
            get
            {
                return Adjoint * (1.0f / this.Determinant);
            }
        }
        #endregion Properties
        #region Public methods

        public Vector3D GetColumn(int col)
        {
            switch (col)
            {
                case 0:
                    return Cx;
                case 1:
                    return Cy;
                case 2:
                    return Cz;
                default:
                    throw new ArgumentOutOfRangeException("col", "Attempted to get an invalid column of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public void SetColumn(int col, Vector3D value)
        {
            switch (col)
            {
                case 0:
                    Cx = value;
                    return;
                case 1:
                    Cy = value;
                    return;
                case 2:
                    Cz = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("col", "Attempted to set an invalid column of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public Vector3D GetRow(int row)
        {
            switch (row)
            {
                case 0:
                    return Rx;
                case 1:
                    return Ry;
                case 2:
                    return Rz;
                default:
                    throw new ArgumentOutOfRangeException("row", "Attempted to get an invalid row of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public void SetRow(int row, Vector3D value)
        {
            switch (row)
            {
                case 0:
                    Rx = value;
                    return;
                case 1:
                    Ry = value;
                    return;
                case 2:
                    Rz = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("row", "Attempted to set an invalid row of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }

        /// <summary>
        ///		Creates a Matrix3 from 3 axes.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        public void FromAxis(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
        {
            SetColumn(0, xAxis);
            SetColumn(1, yAxis);
            SetColumn(2, zAxis);
        }

        public Scalar[,] ToMatrixArray()
        {
            return new Scalar[RowCount, ColumnCount] { { m00, m01, m02 }, { m10, m11, m12 }, { m20, m21, m22 } };
        }
        public Scalar[] ToArray()
        {
            return new Scalar[Count] { m00, m01, m02, m10, m11, m12, m20, m21, m22 };
        }
        public Scalar[] ToTransposedArray()
        {
            return new Scalar[Count] { m00, m10, m20, m01, m11, m21, m02, m12, m22 };
        }

        public Matrix4x4 ToMatrix4x4From2D()
        {
            Matrix4x4 returnvalue = Matrix4x4.Identity;
            returnvalue.m00 = this.m00; returnvalue.m01 = this.m01; returnvalue.m03 = this.m02;
            returnvalue.m10 = this.m10; returnvalue.m11 = this.m11; returnvalue.m13 = this.m12;
            returnvalue.m30 = this.m20; returnvalue.m31 = this.m21; returnvalue.m33 = this.m22;
            return returnvalue;
        }
        public Matrix4x4 ToMatrix4x4()
        {
            Matrix4x4 returnvalue = Matrix4x4.Identity;
            returnvalue.m00 = this.m00; returnvalue.m01 = this.m01; returnvalue.m02 = this.m02;
            returnvalue.m10 = this.m10; returnvalue.m11 = this.m11; returnvalue.m12 = this.m12;
            returnvalue.m20 = this.m20; returnvalue.m21 = this.m21; returnvalue.m22 = this.m22;
            return returnvalue;
        }
        public void CopyTo(Scalar[] array, int index)
        {
            array[index] = m00;
            array[index + 1] = m01;
            array[index + 2] = m02;

            array[index + 3] = m10;
            array[index + 4] = m11;
            array[index + 5] = m12;

            array[index + 6] = m20;
            array[index + 7] = m21;
            array[index + 8] = m22;
        }
        public void CopyTransposedTo(Scalar[] array, int index)
        {
            array[index] = m00;
            array[index + 1] = m10;
            array[index + 2] = m20;

            array[index + 3] = m01;
            array[index + 4] = m11;
            array[index + 5] = m21;

            array[index + 6] = m02;
            array[index + 7] = m12;
            array[index + 8] = m22;
        }
        public void CopyFrom(Scalar[] array, int index)
        {
            m00 = array[index];
            m01 = array[index + 1];
            m02 = array[index + 2];

            m10 = array[index + 3];
            m11 = array[index + 4];
            m12 = array[index + 5];

            m20 = array[index + 6];
            m21 = array[index + 7];
            m22 = array[index + 8];
        }
        public void CopyTransposedFrom(Scalar[] array, int index)
        {
            m00 = array[index];
            m10 = array[index + 1];
            m20 = array[index + 2];

            m01 = array[index + 3];
            m11 = array[index + 4];
            m21 = array[index + 5];

            m02 = array[index + 6];
            m12 = array[index + 7];
            m22 = array[index + 8];
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
                Debug.Assert(index >= 0, "Attempt to set a index of a Matrix" + RowCount + "x" + ColumnCount + " less than 0.");
                unsafe
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
        public static Matrix3x3 operator *(Matrix3x3 left, Matrix3x3 right)
        {

            Matrix3x3 returnvalue = new Matrix3x3();

            returnvalue.m00 = left.m00 * right.m00 + left.m01 * right.m10 + left.m02 * right.m20;
            returnvalue.m01 = left.m00 * right.m01 + left.m01 * right.m11 + left.m02 * right.m21;
            returnvalue.m02 = left.m00 * right.m02 + left.m01 * right.m12 + left.m02 * right.m22;

            returnvalue.m10 = left.m10 * right.m00 + left.m11 * right.m10 + left.m12 * right.m20;
            returnvalue.m11 = left.m10 * right.m01 + left.m11 * right.m11 + left.m12 * right.m21;
            returnvalue.m12 = left.m10 * right.m02 + left.m11 * right.m12 + left.m12 * right.m22;

            returnvalue.m20 = left.m20 * right.m00 + left.m21 * right.m10 + left.m22 * right.m20;
            returnvalue.m21 = left.m20 * right.m01 + left.m21 * right.m11 + left.m22 * right.m21;
            returnvalue.m22 = left.m20 * right.m02 + left.m21 * right.m12 + left.m22 * right.m22;

            return returnvalue;
        }
        /// <summary>
        /// Multiply (concatenate) a Matrix3x3 and a Matrix2x2
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3x3 operator *(Matrix2x2 left, Matrix3x3 right)
        {

            Matrix3x3 returnvalue = right;

            returnvalue.m00 = left.m00 * right.m00 + left.m01 * right.m10;
            returnvalue.m01 = left.m00 * right.m01 + left.m01 * right.m11;
            returnvalue.m02 = left.m00 * right.m02 + left.m01 * right.m12;

            returnvalue.m10 = left.m10 * right.m00 + left.m11 * right.m10;
            returnvalue.m11 = left.m10 * right.m01 + left.m11 * right.m11;
            returnvalue.m12 = left.m10 * right.m02 + left.m11 * right.m12;

            //returnvalue.m20 = right.m20;
            //returnvalue.m21 = right.m21;
            //returnvalue.m22 = right.m22;

            return returnvalue;
        }
        /// <summary>
        /// Multiply (concatenate) a Matrix3x3 and a Matrix2x2
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3x3 operator *(Matrix3x3 left, Matrix2x2 right)
        {

            Matrix3x3 returnvalue = left;

            returnvalue.m00 = left.m00 * right.m00 + left.m01 * right.m10;
            returnvalue.m01 = left.m00 * right.m01 + left.m01 * right.m11;
            //returnvalue.m02 = left.m02;

            returnvalue.m10 = left.m10 * right.m00 + left.m11 * right.m10;
            returnvalue.m11 = left.m10 * right.m01 + left.m11 * right.m11;
            //returnvalue.m12 = left.m12;

            returnvalue.m20 = left.m20 * right.m00 + left.m21 * right.m10;
            returnvalue.m21 = left.m20 * right.m01 + left.m21 * right.m11;
            //returnvalue.m22 = left.m22;

            return returnvalue;
        }

        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix3x3 operator *(Matrix3x3 matrix, Scalar scalar)
        {
            Matrix3x3 returnvalue = new Matrix3x3();

            returnvalue.m00 = matrix.m00 * scalar;
            returnvalue.m01 = matrix.m01 * scalar;
            returnvalue.m02 = matrix.m02 * scalar;
            returnvalue.m10 = matrix.m10 * scalar;
            returnvalue.m11 = matrix.m11 * scalar;
            returnvalue.m12 = matrix.m12 * scalar;
            returnvalue.m20 = matrix.m20 * scalar;
            returnvalue.m21 = matrix.m21 * scalar;
            returnvalue.m22 = matrix.m22 * scalar;

            return returnvalue;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix3x3 operator *(Scalar scalar, Matrix3x3 matrix)
        {
            Matrix3x3 returnvalue = new Matrix3x3();

            returnvalue.m00 = matrix.m00 * scalar;
            returnvalue.m01 = matrix.m01 * scalar;
            returnvalue.m02 = matrix.m02 * scalar;
            returnvalue.m10 = matrix.m10 * scalar;
            returnvalue.m11 = matrix.m11 * scalar;
            returnvalue.m12 = matrix.m12 * scalar;
            returnvalue.m20 = matrix.m20 * scalar;
            returnvalue.m21 = matrix.m21 * scalar;
            returnvalue.m22 = matrix.m22 * scalar;

            return returnvalue;
        }
        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3x3 operator +(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 returnvalue = new Matrix3x3();

            returnvalue.Rx = left.Rx + right.Rx;
            returnvalue.Ry = left.Ry + right.Ry;
            returnvalue.Rz = left.Rz + right.Rz;

            return returnvalue;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix3x3 operator -(Matrix3x3 left, Matrix3x3 right)
        {
            Matrix3x3 returnvalue = new Matrix3x3();

            returnvalue.Rx = left.Rx - right.Rx;
            returnvalue.Ry = left.Ry - right.Ry;
            returnvalue.Rz = left.Rz - right.Rz;

            return returnvalue;
        }
        /// <summary>
        /// Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix3x3 operator -(Matrix3x3 matrix)
        {
            Matrix3x3 returnvalue = new Matrix3x3();

            returnvalue.m00 = -matrix.m00;
            returnvalue.m01 = -matrix.m01;
            returnvalue.m02 = -matrix.m02;
            returnvalue.m10 = -matrix.m10;
            returnvalue.m11 = -matrix.m11;
            returnvalue.m12 = -matrix.m12;
            returnvalue.m20 = -matrix.m20;
            returnvalue.m21 = -matrix.m21;
            returnvalue.m22 = -matrix.m22;

            return returnvalue;
        }
        /// <summary>
        /// 	Test two matrices for (value) equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix3x3 left, Matrix3x3 right)
        {
            return (
                left.m00 == right.m00 && left.m01 == right.m01 && left.m02 == right.m02 &&
                left.m10 == right.m10 && left.m11 == right.m11 && left.m12 == right.m12 &&
                left.m20 == right.m20 && left.m21 == right.m21 && left.m22 == right.m22);
        }
        public static bool operator !=(Matrix3x3 left, Matrix3x3 right)
        {
            return !(left == right);
        }


        public static explicit operator Matrix3x3(Matrix4x4 source)
        {
            Matrix3x3 returnvalue = new Matrix3x3();

            returnvalue.m00 = source.m00;
            returnvalue.m01 = source.m01;
            returnvalue.m02 = source.m02;

            returnvalue.m10 = source.m10;
            returnvalue.m11 = source.m11;
            returnvalue.m12 = source.m12;

            returnvalue.m20 = source.m20;
            returnvalue.m21 = source.m21;
            returnvalue.m22 = source.m22;

            return returnvalue;
        }
        public static explicit operator Matrix3x3(Matrix2x2 source)
        {
            Matrix3x3 returnvalue = Identity;

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

            builder.AppendFormat(" | {0} {1} {2} |\n", m00, m01, m02);
            builder.AppendFormat(" | {0} {1} {2} |\n", m10, m11, m12);
            builder.AppendFormat(" | {0} {1} {2} |", m20, m21, m22);

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
            if (obj is Matrix3x3)
                return (this == (Matrix3x3)obj);
            else
                return false;
        }

        #endregion

    }
}
