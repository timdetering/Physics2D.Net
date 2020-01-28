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

namespace AdvanceMath
{
    public interface IAdvanceValueType
    {
        /// <summary>
        /// Gets a 32-bit integer that represents the total number of elements in all the dimensions of IAdvanceValueType. 
        /// Dont Confuse this with Magnitude for IVector.
        /// </summary>
        int Length { get;}
        /// <summary>
        /// Gets or sets the <see cref="Scalar"/>  at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="Scalar"/> to get or set.</param>
        /// <returns>The <see cref="Scalar"/> at the specified index.</returns>
        Scalar this[int index] { get;set;}
        /// <summary>
        /// Copies the elements of the IAdvanceValueType to a new array of <see cref="Scalar"/> . 
        /// </summary>
        /// <returns>An array containing copies of the elements of the IAdvanceValueType.</returns>
        Scalar[] ToArray();
        /// <summary>
        /// Copies all the elements of the IAdvanceValueType to the specified one-dimensional Array of <see cref="Scalar"/>. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from the IAdvanceValueType.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        void CopyTo(Scalar[] array, int index);
        /// <summary>
        /// Copies all the elements, up to the <see cref="Length"/> of the IAdvanceValueType, of the specified one-dimensional Array to the IAdvanceValueType. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the source of the elements copied to the IAdvanceValueType.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        void CopyFrom(Scalar[] array, int index);
    }
    public interface IVector<V> : IAdvanceValueType
        where V : struct, IVector<V>
    {
        /// <summary>
        /// Gets or Sets the Magnitude (Length of a Vector).
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        Scalar Magnitude { get;set;}
        /// <summary>
        /// Gets the Squared Magnitude (IE Magnitude*Magnitude).
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        Scalar MagnitudeSq { get;}
        /// <summary>
        /// Gets the Normalized Vector. (Unit Vector)
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        V Normalized { get;}
    }
    public interface IMatrix<M, V> : IAdvanceValueType
        where M : struct, IMatrix<M, V>
        where V : struct, IVector<V>
    {
        /// <summary>
        /// Gets a 32-bit integer that represents the total number of Rows in the IMatrix. 
        /// </summary>
        int RowLength { get;}
        /// <summary>
        /// Gets a 32-bit integer that represents the total number of Columns in the IMatrix. 
        /// </summary>
        int ColumnLength { get;}
        /// <summary>
        /// Gets or sets the <see cref="Scalar"/>  at the specified row and column.
        /// </summary>
        /// <param name="row">The zero-based index of the Row to get or set.</param>
        /// <param name="column">The zero-based index of the Column to get or set.</param>
        /// <returns>The <see cref="Scalar"/> at the specified index.</returns>
        Scalar this[int row, int column] { get;set;}
        /// <summary>
        /// Copies the elements of the IMatrix to a new 2-dimensional array of <see cref="Scalar"/>s. 
        /// </summary>
        /// <returns>A 2-dimensional array containing copies of the elements of the IMatrix.</returns>
        Scalar[,] ToMatrixArray();
        /// <returns></returns>
        /// <summary>
        /// Copies the elements, in a Transposed order, of the IMatrix to a new array of <see cref="Scalar"/>. 
        /// </summary>
        /// <returns>An array containing copies of the elements, in a Transposed order, of the IAdvanceValueType.</returns>
        /// <remarks>
        /// This is the Format Accepted by OpenGL.
        /// </remarks>
        Scalar[] ToTransposedArray();
        /// <summary>
        /// Copies all the elements, in a Transposed order, of the IAdvanceValueType to the specified one-dimensional Array of <see cref="Scalar"/>. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from the IAdvanceValueType.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        void CopyTransposedTo(Scalar[] array, int index);
        /// <summary>
        /// Copies all the elements, in a Transposed order, up to the <see cref="IAdvanceValueType.Length"/> of the IAdvanceValueType, of the specified one-dimensional Array to the IAdvanceValueType. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the source of the elements copied to the IAdvanceValueType.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        void CopyTransposedFrom(Scalar[] array, int index);
        /// <summary>
        /// Gets the <typeparamref name="V"/> at the specified Column.
        /// </summary>
        /// <param name="column">The zero-based index of the Column of the <typeparamref name="V"/> to get.</param>
        /// <returns>The <typeparamref name="V"/> at the specified Column.</returns>
        V GetColumn(int column);
        /// <summary>
        /// Sets the <typeparamref name="V"/>  at the specified Column.
        /// </summary>
        /// <param name="column">The zero-based index of the Column of the <typeparamref name="V"/> to set.</param>
        /// <param name="value">The <typeparamref name="V"/> to set at the specified Column.</param>
        void SetColumn(int column, V value);
        /// <summary>
        /// Gets the <typeparamref name="V"/> at the specified Row.
        /// </summary>
        /// <param name="row">The zero-based index of the Row of the <typeparamref name="V"/> to get.</param>
        /// <returns>The <typeparamref name="V"/> at the specified Row.</returns>
        V GetRow(int row);
        /// <summary>
        /// Sets the <typeparamref name="V"/> at the specified Row.
        /// </summary>
        /// <param name="row">The zero-based index of the Row of the <typeparamref name="V"/> to set.</param>
        /// <param name="value">The <typeparamref name="V"/> to set at the specified Row.</param>
        void SetRow(int row, V value);



        /// <summary>
        /// Gets the Determinant of the IMatrix
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Determinant"/></remarks>
        Scalar Determinant { get;}
        /// <summary>
        /// Gets the Inverse of the IMatrix
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Invertible_matrix"/></remarks>
        M Inverse { get;}
        /// <summary>
        /// Gets the Transpose of the IMatrix
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Transpose"/></remarks>
        M Transpose { get;}
        /// <summary>
        /// Gets the Adjoint (Conjugate Transpose) of the IMatrix
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Conjugate_transpose"/></remarks>
        M Adjoint { get;}
        /// <summary>
        /// Gets the Cofactor (The Transpose of the Adjoint) of the IMatrix
        /// </summary>
        M Cofactor { get;}
    }


   
}