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
namespace AdvanceMath
{
    /// <summary>
    ///		Class encapsulating a standard 4x4 homogenous matrix.
    /// </summary>
    /// <remarks>
    ///		The engine uses column vectors when applying matrix multiplications,
    ///		This means a vector is represented as a single column, 4-row
    ///		matrix. This has the effect that the tranformations implemented
    ///		by the matrices happens right-to-left e.g. if vector V is to be
    ///		transformed by M1 then M2 then M3, the calculation would be
    ///		M3 * M2 * M1 * V. The order that matrices are concatenated is
    ///		vital since matrix multiplication is not cummatative, i.e. you
    ///		can get a different returnvalue if you concatenate in the wrong order.
    /// 		<p/>
    ///		The use of column vectors and right-to-left ordering is the
    ///		standard in most mathematical texts, and is the same as used in
    ///		OpenGL. It is, however, the opposite of Direct3D, which has
    ///		inexplicably chosen to differ from the accepted standard and uses
    ///		row vectors and left-to-right matrix multiplication.
    ///		<p/>
    ///		The engine deals with the differences between D3D and OpenGL etc.
    ///		internally when operating through different render systems. The engine
    ///		users only need to conform to standard maths conventions, i.e.
    ///		right-to-left matrix multiplication, (The engine transposes matrices it
    ///		passes to D3D to compensate).
    ///		<p/>
    ///		The generic form M * V which shows the layout of the matrix 
    ///		entries is shown below:
    ///		<p/>
    ///		| m[0][0]  m[0][1]  m[0][2]  m[0][3] |   {x}
    ///		| m[1][0]  m[1][1]  m[1][2]  m[1][3] |   {y}
    ///		| m[2][0]  m[2][1]  m[2][2]  m[2][3] |   {z}
    ///		| m[3][0]  m[3][1]  m[3][2]  m[3][3] |   {1}
    ///	</remarks>
    ///	<ogre headerVersion="1.18" sourceVersion="1.8" />
    //[StructLayout(LayoutKind.Sequential), Serializable]
    [StructLayout(LayoutKind.Explicit, Size = Matrix4x4.Size, Pack = 0), Serializable]
    public struct Matrix4x4 : IMatrix<Matrix4x4, Vector4D>
    {
        #region const fields
        /// <summary>
        /// The number of rows.
        /// </summary>
        public const int RowCount = 4;
        /// <summary>
        /// The number of columns.
        /// </summary>
        public const int ColumnCount = 4;
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = RowCount * ColumnCount;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region static method Constructors

        public static Matrix4x4 FromArray(Scalar[] array)
        {
            Matrix4x4 returnvalue = Identity;
            returnvalue.CopyFrom(array, 0);
            return returnvalue;
        }
        public static Matrix4x4 FromTransposedArray(Scalar[] array)
        {
            Matrix4x4 returnvalue = Identity;
            returnvalue.CopyTransposedFrom(array, 0);
            return returnvalue;
        }
        public static Matrix4x4 FromTranslation(Vector3D translation)
        {
            Matrix4x4 returnvalue = Identity;
            returnvalue.m03 = translation.X;
            returnvalue.m13 = translation.Y;
            returnvalue.m23 = translation.Z;
            return returnvalue;
        }
        public static Matrix4x4 FromScale(Vector3D scale)
        {
            Matrix4x4 returnvalue = Identity;
            returnvalue.m00 = scale.X;
            returnvalue.m11 = scale.Y;
            returnvalue.m22 = scale.Z;
            return returnvalue;
        }

        public static Matrix4x4 FromGraphicsBookMethod1(Vector3D origin, Vector3D positiveZAxis, Vector3D onPositiveY)
        {
            return Matrix3x3.FromGraphicsBookMethod1(origin, positiveZAxis, onPositiveY) * FromTranslation(-origin);
        }
        public static Matrix4x4 FromGraphicsBookMethod2(Vector3D origin, Vector3D positiveZAxis, Vector3D onPositiveY)
        {
            return Matrix3x3.FromGraphicsBookMethod2(origin, positiveZAxis, onPositiveY) * FromTranslation(-origin);
        }
        #endregion
        #region static methods
        /// <summary>
        ///		Used to multiply (concatenate) two 4x4 Matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 Multiply(Matrix4x4 left, Matrix4x4 right)
        {
            return left * right;
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
        public static Vector3D Multiply(Matrix4x4 matrix, Vector3D vector)
        {
            return matrix * vector;
        }
        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 Add(Matrix4x4 left, Matrix4x4 right)
        {
            return left + right;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 Subtract(Matrix4x4 left, Matrix4x4 right)
        {
            return left - right;
        }
        #endregion
        #region Member variables

        [FieldOffset(Vector4D.Size * 0), NonSerialized]
        public Vector4D Rx;
        [FieldOffset(Vector4D.Size * 1), NonSerialized]
        public Vector4D Ry;
        [FieldOffset(Vector4D.Size * 2), NonSerialized]
        public Vector4D Rz;
        [FieldOffset(Vector4D.Size * 3), NonSerialized]
        public Vector4D Rw;

        [FieldOffset(sizeof(Scalar) * 0), XmlIgnore, SoapIgnore]
        public Scalar m00;
        [FieldOffset(sizeof(Scalar) * 1), XmlIgnore, SoapIgnore]
        public Scalar m01;
        [FieldOffset(sizeof(Scalar) * 2), XmlIgnore, SoapIgnore]
        public Scalar m02;
        [FieldOffset(sizeof(Scalar) * 3), XmlIgnore, SoapIgnore]
        public Scalar m03;
        [FieldOffset(sizeof(Scalar) * 4), XmlIgnore, SoapIgnore]
        public Scalar m10;
        [FieldOffset(sizeof(Scalar) * 5), XmlIgnore, SoapIgnore]
        public Scalar m11;
        [FieldOffset(sizeof(Scalar) * 6), XmlIgnore, SoapIgnore]
        public Scalar m12;
        [FieldOffset(sizeof(Scalar) * 7), XmlIgnore, SoapIgnore]
        public Scalar m13;
        [FieldOffset(sizeof(Scalar) * 8), XmlIgnore, SoapIgnore]
        public Scalar m20;
        [FieldOffset(sizeof(Scalar) * 9), XmlIgnore, SoapIgnore]
        public Scalar m21;
        [FieldOffset(sizeof(Scalar) * 10), XmlIgnore, SoapIgnore]
        public Scalar m22;
        [FieldOffset(sizeof(Scalar) * 11), XmlIgnore, SoapIgnore]
        public Scalar m23;
        [FieldOffset(sizeof(Scalar) * 12), XmlIgnore, SoapIgnore]
        public Scalar m30;
        [FieldOffset(sizeof(Scalar) * 13), XmlIgnore, SoapIgnore]
        public Scalar m31;
        [FieldOffset(sizeof(Scalar) * 14), XmlIgnore, SoapIgnore]
        public Scalar m32;
        [FieldOffset(sizeof(Scalar) * 15), XmlIgnore, SoapIgnore]
        public Scalar m33;

        public readonly static Matrix4x4 Zero = new Matrix4x4(
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0);
        public readonly static Matrix4x4 Identity = new Matrix4x4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);

        public readonly static Matrix4x4 ClipSpace2dToImageSpace = new Matrix4x4(
            0.5f, 0, 0, 0.5f,
            0, -0.5f, 0, 0.5f,
            0, 0, 1, 0,
            0, 0, 0, 1);

        #endregion
        #region Constructors

        /// <summary>
        ///		Creates a new Matrix4 with all the specified parameters.
        /// </summary>
        public Matrix4x4(Scalar m00, Scalar m01, Scalar m02, Scalar m03,
            Scalar m10, Scalar m11, Scalar m12, Scalar m13,
            Scalar m20, Scalar m21, Scalar m22, Scalar m23,
            Scalar m30, Scalar m31, Scalar m32, Scalar m33)
        {
            this.Rx = Vector4D.Zero;
            this.Ry = Vector4D.Zero;
            this.Rz = Vector4D.Zero;
            this.Rw = Vector4D.Zero;
            this.m00 = m00; this.m01 = m01; this.m02 = m02; this.m03 = m03;
            this.m10 = m10; this.m11 = m11; this.m12 = m12; this.m13 = m13;
            this.m20 = m20; this.m21 = m21; this.m22 = m22; this.m23 = m23;
            this.m30 = m30; this.m31 = m31; this.m32 = m32; this.m33 = m33;
        }
        public Matrix4x4(Scalar[] vals)
        {
            this = Zero;
            this.CopyFrom(vals, 0);
        }
        #endregion
        #region Public properties
        [XmlIgnore, SoapIgnore]
        public Vector4D Cx
        {
            get
            {
                Vector4D rv;
                rv.x = m00;
                rv.y = m10;
                rv.z = m20;
                rv.w = m30;
                return rv;
            }
            set
            {
                this.m00 = value.X;
                this.m10 = value.Y;
                this.m20 = value.Z;
                this.m30 = value.W;
            }
        }
        [XmlIgnore, SoapIgnore]
        public Vector4D Cy
        {
            get
            {
                Vector4D rv;
                rv.x = m01;
                rv.y = m11;
                rv.z = m21;
                rv.w = m31;
                return rv;
            }
            set
            {
                this.m01 = value.X;
                this.m11 = value.Y;
                this.m21 = value.Z;
                this.m31 = value.W;
            }
        }
        [XmlIgnore, SoapIgnore]
        public Vector4D Cz
        {
            get
            {
                Vector4D rv;
                rv.x = m02;
                rv.y = m12;
                rv.z = m22;
                rv.w = m32;
                return rv;
            }
            set
            {
                this.m02 = value.X;
                this.m12 = value.Y;
                this.m22 = value.Z;
                this.m32 = value.W;
            }
        }
        [XmlIgnore, SoapIgnore]
        public Vector4D Cw
        {
            get
            {
                Vector4D rv;
                rv.x = m03;
                rv.y = m13;
                rv.z = m23;
                rv.w = m33;
                return rv;
            }
            set
            {
                this.m03 = value.X;
                this.m13 = value.Y;
                this.m23 = value.Z;
                this.m33 = value.W;
            }
        }

        public int Length { get { return Count; } }
        public int RowLength { get { return RowCount; } }
        public int ColumnLength { get { return ColumnCount; } }
        /// <summary>
        ///    Gets the determinant of this matrix.
        /// </summary>
        public Scalar Determinant
        {
            get
            {
                // note: this is an expanded version of the Ogre determinant() method, to give better performance in C#. Generated using a script
                Scalar returnvalue = m00 * (m11 * (m22 * m33 - m32 * m23) - m12 * (m21 * m33 - m31 * m23) + m13 * (m21 * m32 - m31 * m22)) -
                    m01 * (m10 * (m22 * m33 - m32 * m23) - m12 * (m20 * m33 - m30 * m23) + m13 * (m20 * m32 - m30 * m22)) +
                    m02 * (m10 * (m21 * m33 - m31 * m23) - m11 * (m20 * m33 - m30 * m23) + m13 * (m20 * m31 - m30 * m21)) -
                    m03 * (m10 * (m21 * m32 - m31 * m22) - m11 * (m20 * m32 - m30 * m22) + m12 * (m20 * m31 - m30 * m21));

                return returnvalue;
            }
        }


        /// <summary>
        ///    Swap the rows of the matrix with the columns.
        /// </summary>
        /// <returns>A transposed Matrix.</returns>
        public Matrix4x4 Transpose
        {
            get
            {
                return new Matrix4x4(this.m00, this.m10, this.m20, this.m30,
                    this.m01, this.m11, this.m21, this.m31,
                    this.m02, this.m12, this.m22, this.m32,
                    this.m03, this.m13, this.m23, this.m33);
            }
        }
        /// <summary>
        ///    Used to generate the adjoint of this matrix.  Used internally for <see cref="Inverse"/>.
        /// </summary>
        /// <returns>The adjoint matrix of the current instance.</returns>
        public Matrix4x4 AdjointOriginal
        {
            get
            {
                // note: this is an expanded version of the Ogre adjoint() method, to give better performance in C#. Generated using a script
                Scalar val0 = m11 * (m22 * m33 - m32 * m23) - m12 * (m21 * m33 - m31 * m23) + m13 * (m21 * m32 - m31 * m22);
                Scalar val1 = -(m01 * (m22 * m33 - m32 * m23) - m02 * (m21 * m33 - m31 * m23) + m03 * (m21 * m32 - m31 * m22));
                Scalar val2 = m01 * (m12 * m33 - m32 * m13) - m02 * (m11 * m33 - m31 * m13) + m03 * (m11 * m32 - m31 * m12);
                Scalar val3 = -(m01 * (m12 * m23 - m22 * m13) - m02 * (m11 * m23 - m21 * m13) + m03 * (m11 * m22 - m21 * m12));

                Scalar val4 = -(m10 * (m22 * m33 - m32 * m23) - m12 * (m20 * m33 - m30 * m23) + m13 * (m20 * m32 - m30 * m22));
                Scalar val5 = m00 * (m22 * m33 - m32 * m23) - m02 * (m20 * m33 - m30 * m23) + m03 * (m20 * m32 - m30 * m22);
                Scalar val6 = -(m00 * (m12 * m33 - m32 * m13) - m02 * (m10 * m33 - m30 * m13) + m03 * (m10 * m32 - m30 * m12));
                Scalar val7 = m00 * (m12 * m23 - m22 * m13) - m02 * (m10 * m23 - m20 * m13) + m03 * (m10 * m22 - m20 * m12);

                Scalar val8 = m10 * (m21 * m33 - m31 * m23) - m11 * (m20 * m33 - m30 * m23) + m13 * (m20 * m31 - m30 * m21);
                Scalar val9 = -(m00 * (m21 * m33 - m31 * m23) - m01 * (m20 * m33 - m30 * m23) + m03 * (m20 * m31 - m30 * m21));
                Scalar val10 = m00 * (m11 * m33 - m31 * m13) - m01 * (m10 * m33 - m30 * m13) + m03 * (m10 * m31 - m30 * m11);
                Scalar val11 = -(m00 * (m11 * m23 - m21 * m13) - m01 * (m10 * m23 - m20 * m13) + m03 * (m10 * m21 - m20 * m11));

                Scalar val12 = -(m10 * (m21 * m32 - m31 * m22) - m11 * (m20 * m32 - m30 * m22) + m12 * (m20 * m31 - m30 * m21));
                Scalar val13 = m00 * (m21 * m32 - m31 * m22) - m01 * (m20 * m32 - m30 * m22) + m02 * (m20 * m31 - m30 * m21);
                Scalar val14 = -(m00 * (m11 * m32 - m31 * m12) - m01 * (m10 * m32 - m30 * m12) + m02 * (m10 * m31 - m30 * m11));
                Scalar val15 = m00 * (m11 * m22 - m21 * m12) - m01 * (m10 * m22 - m20 * m12) + m02 * (m10 * m21 - m20 * m11);

                return new Matrix4x4(val0, val1, val2, val3, val4, val5, val6, val7, val8, val9, val10, val11, val12, val13, val14, val15);
            }
        }
        public Matrix4x4 Cofactor
        {
            get
            {
                Matrix4x4 rv = Identity;
                rv.Rx = Vector4D.TripleCross(this.Ry, this.Rz, this.Rw);
                rv.Ry = -Vector4D.TripleCross(this.Rx, this.Rz, this.Rw);
                rv.Rz = Vector4D.TripleCross(this.Rx, this.Ry, this.Rw);
                rv.Rw = -Vector4D.TripleCross(this.Rx, this.Ry, this.Rz);
                return rv;
            }
        }
        public Matrix4x4 Adjoint2
        {
            get
            {
                //written to test out a theory. a very wasteful implimentation.
                Matrix4x4 rv = Identity;
                rv.Cx = Vector4D.TripleCross(this.Ry, this.Rz, this.Rw);
                rv.Cy = -Vector4D.TripleCross(this.Rx, this.Rz, this.Rw);
                rv.Cz = Vector4D.TripleCross(this.Rx, this.Ry, this.Rw);
                rv.Cw = -Vector4D.TripleCross(this.Rx, this.Ry, this.Rz);
                return rv;
            }
        }

        /// <summary>
        ///    Returns an inverted 4d matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 Inverse
        {
            get
            {
                Scalar m22m33m32m23 = (m22 * m33 - m32 * m23);
                Scalar m21m33m31m23 = (m21 * m33 - m31 * m23);
                Scalar m21m32m31m22 = (m21 * m32 - m31 * m22);

                Scalar m12m33m32m13 = (m12 * m33 - m32 * m13);
                Scalar m11m33m31m13 = (m11 * m33 - m31 * m13);
                Scalar m11m32m31m12 = (m11 * m32 - m31 * m12);

                Scalar m12m23m22m13 = (m12 * m23 - m22 * m13);
                Scalar m11m23m21m13 = (m11 * m23 - m21 * m13);
                Scalar m11m22m21m12 = (m11 * m22 - m21 * m12);

                Scalar m20m33m30m23 = (m20 * m33 - m30 * m23);
                Scalar m20m32m30m22 = (m20 * m32 - m30 * m22);
                Scalar m10m33m30m13 = (m10 * m33 - m30 * m13);

                Scalar m10m32m30m12 = (m10 * m32 - m30 * m12);
                Scalar m10m23m20m13 = (m10 * m23 - m20 * m13);
                Scalar m10m22m20m12 = (m10 * m22 - m20 * m12);

                Scalar m20m31m30m21 = (m20 * m31 - m30 * m21);
                Scalar m10m31m30m11 = (m10 * m31 - m30 * m11);
                Scalar m10m21m20m11 = (m10 * m21 - m20 * m11);


                // note: this is an expanded version of the Ogre determinant() method, to give better performance in C#. Generated using a script
                Scalar detInv = 1 / (m00 * (m11 * m22m33m32m23 - m12 * m21m33m31m23 + m13 * m21m32m31m22) -
                    m01 * (m10 * m22m33m32m23 - m12 * m20m33m30m23 + m13 * m20m32m30m22) +
                    m02 * (m10 * m21m33m31m23 - m11 * m20m33m30m23 + m13 * m20m31m30m21) -
                    m03 * (m10 * m21m32m31m22 - m11 * m20m32m30m22 + m12 * m20m31m30m21));

                // note: this is an expanded version of the Ogre adjoint() method, to give better performance in C#. Generated using a script
                Matrix4x4 inverse = new Matrix4x4();
                inverse.m00 = detInv * (m11 * m22m33m32m23 - m12 * m21m33m31m23 + m13 * m21m32m31m22);
                inverse.m01 = detInv * (-(m01 * m22m33m32m23 - m02 * m21m33m31m23 + m03 * m21m32m31m22));
                inverse.m02 = detInv * (m01 * m12m33m32m13 - m02 * m11m33m31m13 + m03 * m11m32m31m12);
                inverse.m03 = detInv * (-(m01 * m12m23m22m13 - m02 * m11m23m21m13 + m03 * m11m22m21m12));

                inverse.m10 = detInv * (-(m10 * m22m33m32m23 - m12 * m20m33m30m23 + m13 * m20m32m30m22));
                inverse.m11 = detInv * (m00 * m22m33m32m23 - m02 * m20m33m30m23 + m03 * m20m32m30m22);
                inverse.m12 = detInv * (-(m00 * m12m33m32m13 - m02 * m10m33m30m13 + m03 * m10m32m30m12));
                inverse.m13 = detInv * (m00 * m12m23m22m13 - m02 * m10m23m20m13 + m03 * m10m22m20m12);

                inverse.m20 = detInv * (m10 * m21m33m31m23 - m11 * m20m33m30m23 + m13 * m20m31m30m21);
                inverse.m21 = detInv * (-(m00 * m21m33m31m23 - m01 * m20m33m30m23 + m03 * m20m31m30m21));
                inverse.m22 = detInv * (m00 * m11m33m31m13 - m01 * m10m33m30m13 + m03 * m20m31m30m21);
                inverse.m23 = detInv * (-(m00 * m11m23m21m13 - m01 * m10m23m20m13 + m03 * m10m21m20m11));

                inverse.m30 = detInv * (-(m10 * m21m32m31m22 - m11 * m20m32m30m22 + m12 * m20m31m30m21));
                inverse.m31 = detInv * (m00 * m21m32m31m22 - m01 * m20m32m30m22 + m02 * m20m31m30m21);
                inverse.m32 = detInv * (-(m00 * m11m32m31m12 - m01 * m10m32m30m12 + m02 * m10m31m30m11));
                inverse.m33 = detInv * (m00 * m11m22m21m12 - m01 * m10m22m20m12 + m02 * m10m21m20m11);
                return inverse;
            }
        }
        /// <summary>
        ///    Returns an inverted 4d matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 InverseOriginal
        {
            get
            {
                return AdjointOriginal * (1.0f / this.Determinant);
            }
        }
        public Matrix4x4 Adjoint
        {
            get
            {
                //even further expanded to give even better performance. Generated using a keyboard and mouse
                Scalar m22m33m32m23 = (m22 * m33 - m32 * m23);
                Scalar m21m33m31m23 = (m21 * m33 - m31 * m23);
                Scalar m21m32m31m22 = (m21 * m32 - m31 * m22);
                Scalar m12m33m32m13 = (m12 * m33 - m32 * m13);
                Scalar m11m33m31m13 = (m11 * m33 - m31 * m13);
                Scalar m11m32m31m12 = (m11 * m32 - m31 * m12);
                Scalar m12m23m22m13 = (m12 * m23 - m22 * m13);
                Scalar m11m23m21m13 = (m11 * m23 - m21 * m13);
                Scalar m11m22m21m12 = (m11 * m22 - m21 * m12);
                Scalar m20m33m30m23 = (m20 * m33 - m30 * m23);
                Scalar m20m32m30m22 = (m20 * m32 - m30 * m22);
                Scalar m10m33m30m13 = (m10 * m33 - m30 * m13);
                Scalar m10m32m30m12 = (m10 * m32 - m30 * m12);
                Scalar m10m23m20m13 = (m10 * m23 - m20 * m13);
                Scalar m10m22m20m12 = (m10 * m22 - m20 * m12);
                Scalar m20m31m30m21 = (m20 * m31 - m30 * m21);
                Scalar m10m31m30m11 = (m10 * m31 - m30 * m11);
                Scalar m10m21m20m11 = (m10 * m21 - m20 * m11);

                // note: this is an expanded version of the Ogre adjoint() method, to give better performance in C#. Generated using a script
                Matrix4x4 adjoint = new Matrix4x4();
                adjoint.m00 = m11 * m22m33m32m23 - m12 * m21m33m31m23 + m13 * m21m32m31m22;
                adjoint.m01 = -(m01 * m22m33m32m23 - m02 * m21m33m31m23 + m03 * m21m32m31m22);
                adjoint.m02 = m01 * m12m33m32m13 - m02 * m11m33m31m13 + m03 * m11m32m31m12;
                adjoint.m03 = -(m01 * m12m23m22m13 - m02 * m11m23m21m13 + m03 * m11m22m21m12);

                adjoint.m10 = -(m10 * m22m33m32m23 - m12 * m20m33m30m23 + m13 * m20m32m30m22);
                adjoint.m11 = m00 * m22m33m32m23 - m02 * m20m33m30m23 + m03 * m20m32m30m22;
                adjoint.m12 = -(m00 * m12m33m32m13 - m02 * m10m33m30m13 + m03 * m10m32m30m12);
                adjoint.m13 = m00 * m12m23m22m13 - m02 * m10m23m20m13 + m03 * m10m22m20m12;

                adjoint.m20 = m10 * m21m33m31m23 - m11 * m20m33m30m23 + m13 * m20m31m30m21;
                adjoint.m21 = -(m00 * m21m33m31m23 - m01 * m20m33m30m23 + m03 * m20m31m30m21);
                adjoint.m22 = m00 * m11m33m31m13 - m01 * m10m33m30m13 + m03 * m20m31m30m21;
                adjoint.m23 = -(m00 * m11m23m21m13 - m01 * m10m23m20m13 + m03 * m10m21m20m11);

                adjoint.m30 = -(m10 * m21m32m31m22 - m11 * m20m32m30m22 + m12 * m20m31m30m21);
                adjoint.m31 = m00 * m21m32m31m22 - m01 * m20m32m30m22 + m02 * m20m31m30m21;
                adjoint.m32 = -(m00 * m11m32m31m12 - m01 * m10m32m30m12 + m02 * m10m31m30m11);
                adjoint.m33 = m00 * m11m22m21m12 - m01 * m10m22m20m12 + m02 * m10m21m20m11;
                return adjoint;
            }
        }
        #endregion
        #region Public methods

        public Vector4D GetColumn(int col)
        {
            switch (col)
            {
                case 0:
                    return Cx;
                case 1:
                    return Cy;
                case 2:
                    return Cz;
                case 3:
                    return Cw;
                default:
                    throw new ArgumentOutOfRangeException("col", "Attempted to get an invalid column of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public void SetColumn(int col, Vector4D value)
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
                case 3:
                    Cw = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("col", "Attempted to set an invalid column of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public Vector4D GetRow(int row)
        {
            switch (row)
            {
                case 0:
                    return Rx;
                case 1:
                    return Ry;
                case 2:
                    return Rz;
                case 3:
                    return Rw;
                default:
                    throw new ArgumentOutOfRangeException("row", "Attempted to get an invalid row of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }
        public void SetRow(int row, Vector4D value)
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
                case 3:
                    Rw = value;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("row", "Attempted to set an invalid row of a Matrix" + RowCount + "x" + ColumnCount);
            }
        }

        /// <summary>
        ///    Returns a 3x3 portion of this 4x4 matrix.
        /// </summary>
        public Matrix3x3 ToMatrix3x3()
        {
            Matrix3x3 returnalue = Matrix3x3.Identity;
            returnalue.m00 = this.m00;
            returnalue.m01 = this.m01;
            returnalue.m02 = this.m02;

            returnalue.m10 = this.m10;
            returnalue.m11 = this.m11;
            returnalue.m12 = this.m12;

            returnalue.m20 = this.m20;
            returnalue.m21 = this.m21;
            returnalue.m22 = this.m22;
            return returnalue;
        }
        public Scalar[,] ToMatrixArray()
        {
            return new Scalar[RowCount, ColumnCount] { { m00, m01, m02, m03 }, { m10, m11, m12, m13 }, { m20, m21, m22, m23 }, { m30, m31, m32, m33 } };
        }
        public Scalar[] ToArray()
        {
            return new Scalar[Count] { m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33 };
        }
        public Scalar[] ToTransposedArray()
        {
            return new Scalar[Count] { m00, m10, m20, m30, m01, m11, m21, m31, m02, m12, m22, m32, m03, m13, m23, m33 };
        }
        public void CopyTo(Scalar[] array, int index)
        {
            array[index] = m00;
            array[index + 1] = m01;
            array[index + 2] = m02;
            array[index + 3] = m03;

            array[index + 4] = m10;
            array[index + 5] = m11;
            array[index + 6] = m12;
            array[index + 7] = m13;

            array[index + 8] = m20;
            array[index + 9] = m21;
            array[index + 10] = m22;
            array[index + 11] = m23;

            array[index + 12] = m30;
            array[index + 13] = m31;
            array[index + 14] = m32;
            array[index + 15] = m33;
        }
        public void CopyTransposedTo(Scalar[] array, int index)
        {
            array[index] = m00;
            array[index + 1] = m10;
            array[index + 2] = m20;
            array[index + 3] = m30;

            array[index + 4] = m01;
            array[index + 5] = m11;
            array[index + 6] = m21;
            array[index + 7] = m31;

            array[index + 8] = m02;
            array[index + 9] = m12;
            array[index + 10] = m22;
            array[index + 11] = m32;

            array[index + 12] = m03;
            array[index + 13] = m13;
            array[index + 14] = m23;
            array[index + 15] = m33;
        }
        public void CopyFrom(Scalar[] array, int index)
        {
            m00 = array[index];
            m01 = array[index + 1];
            m02 = array[index + 2];
            m03 = array[index + 3];

            m10 = array[index + 4];
            m11 = array[index + 5];
            m12 = array[index + 6];
            m13 = array[index + 7];

            m20 = array[index + 8];
            m21 = array[index + 9];
            m22 = array[index + 10];
            m23 = array[index + 11];

            m30 = array[index + 12];
            m31 = array[index + 13];
            m32 = array[index + 14];
            m33 = array[index + 15];
        }
        public void CopyTransposedFrom(Scalar[] array, int index)
        {
            m00 = array[index];
            m10 = array[index + 1];
            m20 = array[index + 2];
            m30 = array[index + 3];

            m01 = array[index + 4];
            m11 = array[index + 5];
            m21 = array[index + 6];
            m31 = array[index + 7];

            m02 = array[index + 8];
            m12 = array[index + 9];
            m22 = array[index + 10];
            m32 = array[index + 11];

            m03 = array[index + 12];
            m13 = array[index + 13];
            m23 = array[index + 14];
            m33 = array[index + 15];
        }
        #endregion
        #region indexers
        /// <summary>
        ///    Allows the Matrix to be accessed like a 2d array (i.e. matrix[2,3])
        /// </summary>
        /// <remarks>
        ///    This indexer is only provided as a convenience, and is <b>not</b> recommended for use in
        ///    intensive applications.  
        /// </remarks>
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
        public Scalar this[int index]
        {
            get
            {
                Debug.Assert(index < Count, "Attempt to get a index of a Matrix" + RowCount + "x" + ColumnCount + " greater than " + (Count - 1) + ".");
                Debug.Assert(index >= 0, "Attempt to get a index of a Matrix" + RowCount + "x" + ColumnCount + " less than 0.");
                unsafe
                {
                    fixed (Scalar* pMatrix = &this.m00)
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
                    fixed (Scalar* pMatrix = &this.m00)
                    {
                        pMatrix[index] = value;
                    }
                }
            }
        }
        #endregion
        #region Operators

        /// <summary>
        ///		Used to multiply (concatenate) two 4x4 Matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
        {
            Matrix4x4 returnvalue = new Matrix4x4();

            returnvalue.m00 = left.m00 * right.m00 + left.m01 * right.m10 + left.m02 * right.m20 + left.m03 * right.m30;
            returnvalue.m01 = left.m00 * right.m01 + left.m01 * right.m11 + left.m02 * right.m21 + left.m03 * right.m31;
            returnvalue.m02 = left.m00 * right.m02 + left.m01 * right.m12 + left.m02 * right.m22 + left.m03 * right.m32;
            returnvalue.m03 = left.m00 * right.m03 + left.m01 * right.m13 + left.m02 * right.m23 + left.m03 * right.m33;

            returnvalue.m10 = left.m10 * right.m00 + left.m11 * right.m10 + left.m12 * right.m20 + left.m13 * right.m30;
            returnvalue.m11 = left.m10 * right.m01 + left.m11 * right.m11 + left.m12 * right.m21 + left.m13 * right.m31;
            returnvalue.m12 = left.m10 * right.m02 + left.m11 * right.m12 + left.m12 * right.m22 + left.m13 * right.m32;
            returnvalue.m13 = left.m10 * right.m03 + left.m11 * right.m13 + left.m12 * right.m23 + left.m13 * right.m33;

            returnvalue.m20 = left.m20 * right.m00 + left.m21 * right.m10 + left.m22 * right.m20 + left.m23 * right.m30;
            returnvalue.m21 = left.m20 * right.m01 + left.m21 * right.m11 + left.m22 * right.m21 + left.m23 * right.m31;
            returnvalue.m22 = left.m20 * right.m02 + left.m21 * right.m12 + left.m22 * right.m22 + left.m23 * right.m32;
            returnvalue.m23 = left.m20 * right.m03 + left.m21 * right.m13 + left.m22 * right.m23 + left.m23 * right.m33;

            returnvalue.m30 = left.m30 * right.m00 + left.m31 * right.m10 + left.m32 * right.m20 + left.m33 * right.m30;
            returnvalue.m31 = left.m30 * right.m01 + left.m31 * right.m11 + left.m32 * right.m21 + left.m33 * right.m31;
            returnvalue.m32 = left.m30 * right.m02 + left.m31 * right.m12 + left.m32 * right.m22 + left.m33 * right.m32;
            returnvalue.m33 = left.m30 * right.m03 + left.m31 * right.m13 + left.m32 * right.m23 + left.m33 * right.m33;

            return returnvalue;
        }

        /// <summary>
        ///		Used to multiply a Matrix4 object by a scalar value..
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix4x4 operator *(Matrix4x4 left, Scalar scalar)
        {
            Matrix4x4 returnvalue = new Matrix4x4();

            returnvalue.m00 = left.m00 * scalar;
            returnvalue.m01 = left.m01 * scalar;
            returnvalue.m02 = left.m02 * scalar;
            returnvalue.m03 = left.m03 * scalar;

            returnvalue.m10 = left.m10 * scalar;
            returnvalue.m11 = left.m11 * scalar;
            returnvalue.m12 = left.m12 * scalar;
            returnvalue.m13 = left.m13 * scalar;

            returnvalue.m20 = left.m20 * scalar;
            returnvalue.m21 = left.m21 * scalar;
            returnvalue.m22 = left.m22 * scalar;
            returnvalue.m23 = left.m23 * scalar;

            returnvalue.m30 = left.m30 * scalar;
            returnvalue.m31 = left.m31 * scalar;
            returnvalue.m32 = left.m32 * scalar;
            returnvalue.m33 = left.m33 * scalar;

            return returnvalue;
        }

        /// <summary>
        ///		Used to multiply (concatenate) a Matrix4x4 and a Matrix3x3.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 operator *(Matrix4x4 left, Matrix3x3 right)
        {
            Matrix4x4 returnvalue = left;

            returnvalue.m00 = left.m00 * right.m00 + left.m01 * right.m10 + left.m02 * right.m20;
            returnvalue.m01 = left.m00 * right.m01 + left.m01 * right.m11 + left.m02 * right.m21;
            returnvalue.m02 = left.m00 * right.m02 + left.m01 * right.m12 + left.m02 * right.m22;
            //returnvalue.m03 = left.m03;

            returnvalue.m10 = left.m10 * right.m00 + left.m11 * right.m10 + left.m12 * right.m20;
            returnvalue.m11 = left.m10 * right.m01 + left.m11 * right.m11 + left.m12 * right.m21;
            returnvalue.m12 = left.m10 * right.m02 + left.m11 * right.m12 + left.m12 * right.m22;
            //returnvalue.m13 = left.m13;

            returnvalue.m20 = left.m20 * right.m00 + left.m21 * right.m10 + left.m22 * right.m20;
            returnvalue.m21 = left.m20 * right.m01 + left.m21 * right.m11 + left.m22 * right.m21;
            returnvalue.m22 = left.m20 * right.m02 + left.m21 * right.m12 + left.m22 * right.m22;
            //returnvalue.m23 = left.m23;

            returnvalue.m30 = left.m30 * right.m00 + left.m31 * right.m10 + left.m32 * right.m20;
            returnvalue.m31 = left.m30 * right.m01 + left.m31 * right.m11 + left.m32 * right.m21;
            returnvalue.m32 = left.m30 * right.m02 + left.m31 * right.m12 + left.m32 * right.m22;
            //returnvalue.m33 = left.m33;

            return returnvalue;
        }

        /// <summary>
        ///		Used to multiply (concatenate) a Matrix4x4 and a Matrix3x3.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 operator *(Matrix3x3 left, Matrix4x4 right)
        {
            Matrix4x4 returnvalue = right;

            returnvalue.m00 = left.m00 * right.m00 + left.m01 * right.m10 + left.m02 * right.m20;
            returnvalue.m01 = left.m00 * right.m01 + left.m01 * right.m11 + left.m02 * right.m21;
            returnvalue.m02 = left.m00 * right.m02 + left.m01 * right.m12 + left.m02 * right.m22;
            returnvalue.m03 = left.m00 * right.m03 + left.m01 * right.m13 + left.m02 * right.m23;

            returnvalue.m10 = left.m10 * right.m00 + left.m11 * right.m10 + left.m12 * right.m20;
            returnvalue.m11 = left.m10 * right.m01 + left.m11 * right.m11 + left.m12 * right.m21;
            returnvalue.m12 = left.m10 * right.m02 + left.m11 * right.m12 + left.m12 * right.m22;
            returnvalue.m13 = left.m10 * right.m03 + left.m11 * right.m13 + left.m12 * right.m23;

            returnvalue.m20 = left.m20 * right.m00 + left.m21 * right.m10 + left.m22 * right.m20;
            returnvalue.m21 = left.m20 * right.m01 + left.m21 * right.m11 + left.m22 * right.m21;
            returnvalue.m22 = left.m20 * right.m02 + left.m21 * right.m12 + left.m22 * right.m22;
            returnvalue.m23 = left.m20 * right.m03 + left.m21 * right.m13 + left.m22 * right.m23;

            //returnvalue.m30 = right.m30;
            //returnvalue.m31 = right.m31;
            //returnvalue.m32 = right.m32;
            //returnvalue.m33 = right.m33;

            return returnvalue;
        }



        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 operator +(Matrix4x4 left, Matrix4x4 right)
        {
            Matrix4x4 returnvalue = new Matrix4x4();

            returnvalue.m00 = left.m00 + right.m00;
            returnvalue.m01 = left.m01 + right.m01;
            returnvalue.m02 = left.m02 + right.m02;
            returnvalue.m03 = left.m03 + right.m03;

            returnvalue.m10 = left.m10 + right.m10;
            returnvalue.m11 = left.m11 + right.m11;
            returnvalue.m12 = left.m12 + right.m12;
            returnvalue.m13 = left.m13 + right.m13;

            returnvalue.m20 = left.m20 + right.m20;
            returnvalue.m21 = left.m21 + right.m21;
            returnvalue.m22 = left.m22 + right.m22;
            returnvalue.m23 = left.m23 + right.m23;

            returnvalue.m30 = left.m30 + right.m30;
            returnvalue.m31 = left.m31 + right.m31;
            returnvalue.m32 = left.m32 + right.m32;
            returnvalue.m33 = left.m33 + right.m33;

            return returnvalue;
        }

        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix4x4 operator -(Matrix4x4 left, Matrix4x4 right)
        {
            Matrix4x4 returnvalue = new Matrix4x4();

            returnvalue.m00 = left.m00 - right.m00;
            returnvalue.m01 = left.m01 - right.m01;
            returnvalue.m02 = left.m02 - right.m02;
            returnvalue.m03 = left.m03 - right.m03;

            returnvalue.m10 = left.m10 - right.m10;
            returnvalue.m11 = left.m11 - right.m11;
            returnvalue.m12 = left.m12 - right.m12;
            returnvalue.m13 = left.m13 - right.m13;

            returnvalue.m20 = left.m20 - right.m20;
            returnvalue.m21 = left.m21 - right.m21;
            returnvalue.m22 = left.m22 - right.m22;
            returnvalue.m23 = left.m23 - right.m23;

            returnvalue.m30 = left.m30 - right.m30;
            returnvalue.m31 = left.m31 - right.m31;
            returnvalue.m32 = left.m32 - right.m32;
            returnvalue.m33 = left.m33 - right.m33;

            return returnvalue;
        }

        /// <summary>
        /// Compares two Matrix4 instances for equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true if the Matrix 4 instances are equal, false otherwise.</returns>
        public static bool operator ==(Matrix4x4 left, Matrix4x4 right)
        {
            return (
                left.m00 == right.m00 && left.m01 == right.m01 && left.m02 == right.m02 && left.m03 == right.m03 &&
                left.m10 == right.m10 && left.m11 == right.m11 && left.m12 == right.m12 && left.m13 == right.m13 &&
                left.m20 == right.m20 && left.m21 == right.m21 && left.m22 == right.m22 && left.m23 == right.m23 &&
                left.m30 == right.m30 && left.m31 == right.m31 && left.m32 == right.m32 && left.m33 == right.m33);
        }

        /// <summary>
        /// Compares two Matrix4 instances for inequality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true if the Matrix 4 instances are not equal, false otherwise.</returns>
        public static bool operator !=(Matrix4x4 left, Matrix4x4 right)
        {
            return !(left == right);
        }



        public static explicit operator Matrix4x4(Matrix3x3 source)
        {
            Matrix4x4 returnvalue = Identity;

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
        #endregion
        #region Object overloads

        /// <summary>
        ///		Overrides the Object.ToString() method to provide a text representation of 
        ///		a Matrix4.
        /// </summary>
        /// <returns>A string representation of a vector3.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(" | {0} {1} {2} {3} |\n", this.m00, this.m01, this.m02, this.m03);
            sb.AppendFormat(" | {0} {1} {2} {3} |\n", this.m10, this.m11, this.m12, this.m13);
            sb.AppendFormat(" | {0} {1} {2} {3} |\n", this.m20, this.m21, this.m22, this.m23);
            sb.AppendFormat(" | {0} {1} {2} {3} |\n", this.m30, this.m31, this.m32, this.m33);

            return sb.ToString();
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
            }

            return hashCode;
        }

        /// <summary>
        ///		Compares this Matrix to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix4x4)
                return (this == (Matrix4x4)obj);
            else
                return false;
        }

        #endregion
    }
}