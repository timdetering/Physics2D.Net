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
using System.Diagnostics;
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
    [Serializable]
    public struct Matrix2D
    {
        public static readonly Matrix2D Identity = new Matrix2D(Matrix2x2.Identity, Matrix3x3.Identity);

        public Matrix2x2 NormalMatrix;
        public Matrix3x3 VertexMatrix;

        public Matrix2D(Matrix2x2 NormalMatrix, Matrix3x3 VertexMatrix)
        {
            this.NormalMatrix = NormalMatrix;
            this.VertexMatrix = VertexMatrix;
        }
        #region Operator overloads
        public static Matrix2D operator *(Matrix2D left, Matrix3x3 right)
        {
            Matrix2D returnvalue;
            returnvalue.NormalMatrix = left.NormalMatrix;
            returnvalue.VertexMatrix = left.VertexMatrix * right;
            return returnvalue;
        }
        public static Matrix2D operator *(Matrix3x3 left, Matrix2D right)
        {
            Matrix2D returnvalue;
            returnvalue.NormalMatrix = right.NormalMatrix;
            returnvalue.VertexMatrix = left * right.VertexMatrix;
            return returnvalue;
        }
        public static Matrix2D operator *(Matrix2D left, Matrix2x2 right)
        {
            Matrix2D returnvalue;
            returnvalue.NormalMatrix = left.NormalMatrix * right;
            returnvalue.VertexMatrix = left.VertexMatrix * right;
            return returnvalue;
        }
        public static Matrix2D operator *(Matrix2x2 left, Matrix2D right)
        {
            Matrix2D returnvalue;
            returnvalue.NormalMatrix = left * right.NormalMatrix;
            returnvalue.VertexMatrix = left * right.VertexMatrix;
            return returnvalue;
        }
        public static Matrix2D operator *(Matrix2D left, Matrix2D right)
        {
            Matrix2D returnvalue;
            returnvalue.NormalMatrix = left.NormalMatrix * right.NormalMatrix;
            returnvalue.VertexMatrix = left.VertexMatrix * right.VertexMatrix;
            return returnvalue;
        }
        #endregion
    }
}