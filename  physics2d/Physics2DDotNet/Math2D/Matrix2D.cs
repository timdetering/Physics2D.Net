#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion


#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Runtime.InteropServices;

using AdvanceMath;
using AdvanceMath.Design;

namespace Physics2DDotNet.Math2D
{
    [StructLayout(LayoutKind.Sequential, Size = Matrix2D.Size), Serializable]
    [AdvBrowsableOrder("NormalMatrix,VertexMatrix")]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Matrix2D>))]
#endif
    public struct Matrix2D : IEquatable<Matrix2D>
    {
        public const int Size = Matrix2x2.Size + Matrix3x3.Size;

        public static readonly Matrix2D Identity = new Matrix2D(Matrix2x2.Identity, Matrix3x3.Identity);

        public static void Invert(ref Matrix2D value, out Matrix2D result)
        {
            Matrix2x2.Invert(ref value.NormalMatrix, out result.NormalMatrix);
            Matrix3x3.Invert(ref value.VertexMatrix, out result.VertexMatrix);
        }

        public static Matrix2D FromALVector2D(ALVector2D source)
        {
            Matrix2D result;
            FromALVector2D(ref source, out result);
            return result;
        }
        public static void FromALVector2D(ref ALVector2D source, out Matrix2D result)
        {
            Matrix2x2.FromRotation(ref source.Angular, out result.NormalMatrix);
            Matrix3x3.FromTranslate2D(ref source.Linear, out result.VertexMatrix);
            Matrix3x3.Multiply(ref result.VertexMatrix, ref result.NormalMatrix, out result.VertexMatrix);
        }


        [AdvBrowsable]
        public Matrix2x2 NormalMatrix;
        [AdvBrowsable]
        public Matrix3x3 VertexMatrix;

        [InstanceConstructor("NormalMatrix,VertexMatrix")]
        public Matrix2D(Matrix2x2 normalMatrix, Matrix3x3 vertexMatrix)
        {
            this.NormalMatrix = normalMatrix;
            this.VertexMatrix = vertexMatrix;
        }



        #region Operator overloads
        public static Matrix2D Multiply(Matrix2D left, Matrix3x3 right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static Matrix2D operator *(Matrix2D left, Matrix3x3 right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static void Multiply(ref Matrix2D left, ref Matrix3x3 right, out Matrix2D result)
        {
            result.NormalMatrix = left.NormalMatrix;
            Matrix3x3.Multiply(ref left.VertexMatrix, ref right, out result.VertexMatrix);
        }

        public static Matrix2D operator *(Matrix3x3 left, Matrix2D right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static Matrix2D Multiply(Matrix3x3 left, Matrix2D right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static void Multiply(ref Matrix3x3 left, ref Matrix2D right, out Matrix2D result)
        {
            result.NormalMatrix = right.NormalMatrix;
            Matrix3x3.Multiply(ref left, ref right.VertexMatrix, out result.VertexMatrix);
        }

        public static Matrix2D operator *(Matrix2D left, Matrix2x2 right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static Matrix2D Multiply(Matrix2D left, Matrix2x2 right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static void Multiply(ref Matrix2D left, ref Matrix2x2 right, out Matrix2D result)
        {
            Matrix2x2.Multiply(ref left.NormalMatrix, ref right, out result.NormalMatrix);
            Matrix3x3.Multiply(ref left.VertexMatrix, ref right, out result.VertexMatrix);
        }

        public static Matrix2D operator *(Matrix2x2 left, Matrix2D right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static Matrix2D Multiply(Matrix2x2 left, Matrix2D right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static void Multiply(ref Matrix2x2 left, ref Matrix2D right, out Matrix2D result)
        {
            Matrix2x2.Multiply(ref left, ref right.NormalMatrix, out result.NormalMatrix);
            Matrix3x3.Multiply(ref left, ref right.VertexMatrix, out result.VertexMatrix);
        }

        public static Matrix2D operator *(Matrix2D left, Matrix2D right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static Matrix2D Multiply(Matrix2D left, Matrix2D right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static void Multiply(ref Matrix2D left, ref Matrix2D right, out Matrix2D result)
        {
            Matrix2x2.Multiply(ref left.NormalMatrix, ref right.NormalMatrix, out result.NormalMatrix);
            Matrix3x3.Multiply(ref left.VertexMatrix, ref right.VertexMatrix, out result.VertexMatrix);
        }
        #endregion


        #region Overrides
        public override int GetHashCode()
        {
            return this.NormalMatrix.GetHashCode() ^ this.VertexMatrix.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return (obj is Matrix2D) && Equals((Matrix2D)obj);
        }
        public bool Equals(Matrix2D other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(Matrix2D left, Matrix2D right)
        {
            return
                Matrix2x2.Equals(ref left.NormalMatrix, ref right.NormalMatrix) &&
                Matrix3x3.Equals(ref left.VertexMatrix, ref right.VertexMatrix);
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Matrix2D left, ref Matrix2D right)
        {
            return
                Matrix2x2.Equals(ref left.NormalMatrix, ref right.NormalMatrix) &&
                Matrix3x3.Equals(ref left.VertexMatrix, ref right.VertexMatrix);
        }
        public static bool operator ==(Matrix2D left, Matrix2D right)
        {
            return Equals(ref left, ref right);
        }
        public static bool operator !=(Matrix2D left, Matrix2D right)
        {
            return !Equals(ref left, ref right);
        } 
        #endregion
    }
}