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
    /*
    [StructLayout(LayoutKind.Sequential, Size = Matrix2D.Size), Serializable]
    [AdvBrowsableOrder("Normal,Vertex")]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Matrix2D>))]
#endif
    public struct Matrix2D : IEquatable<Matrix2D>
    {
        public const int Size = Matrix2x2.Size + Matrix2x3.Size;

        public static readonly Matrix2D Identity = new Matrix2D(Matrix2x2.Identity, Matrix2x3.Identity);

        public static void Invert(ref Matrix2D value, out Matrix2D result)
        {
            Matrix2x2.Invert(ref value.Normal, out result.Normal);
            Matrix2x3.Invert(ref value.Vertex, out result.Vertex);
        }

        public static Matrix2D FromALVector2D(ALVector2D source)
        {
            Matrix2D result;
            FromALVector2D(ref source, out result);
            return result;
        }
        public static void FromALVector2D(ref ALVector2D source, out Matrix2D result)
        {
            Matrix2x3 vertex;
            vertex.m00 = MathHelper.Cos(source.Angular);
            vertex.m10 = MathHelper.Sin(source.Angular);
            vertex.m01 = -vertex.m10;
            vertex.m11 = vertex.m00;
            vertex.m02 = source.Linear.X;
            vertex.m12 = source.Linear.Y;
            result.Vertex = vertex;
            Matrix2x2.Copy(ref vertex, out result.Normal);
        }


        [AdvBrowsable]
        public Matrix2x2 Normal;
        [AdvBrowsable]
        public Matrix2x3 Vertex;

        [InstanceConstructor("Normal,Vertex")]
        public Matrix2D(Matrix2x2 normal, Matrix2x3 vertex)
        {
            this.Normal = normal;
            this.Vertex = vertex;
        }



        #region Operator overloads
        public static Matrix2D Multiply(Matrix2D left, Matrix2x3 right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static Matrix2D operator *(Matrix2D left, Matrix2x3 right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static void Multiply(ref Matrix2D left, ref Matrix2x3 right, out Matrix2D result)
        {
            result.Normal = left.Normal;
            Matrix2x3.Multiply(ref left.Vertex, ref right, out result.Vertex);
        }

        public static Matrix2D operator *(Matrix2x3 left, Matrix2D right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static Matrix2D Multiply(Matrix2x3 left, Matrix2D right)
        {
            Matrix2D result;
            Multiply(ref left, ref right, out result);
            return result;
        }
        public static void Multiply(ref Matrix2x3 left, ref Matrix2D right, out Matrix2D result)
        {
            result.Normal = right.Normal;
            Matrix2x3.Multiply(ref left, ref right.Vertex, out result.Vertex);
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
            Matrix2x2.Multiply(ref left.Normal, ref right, out result.Normal);
            Matrix2x3.Multiply(ref left.Vertex, ref right, out result.Vertex);
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
            Matrix2x2.Multiply(ref left, ref right.Normal, out result.Normal);
            Matrix2x3.Multiply(ref left, ref right.Vertex, out result.Vertex);
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
            Matrix2x2.Multiply(ref right.Normal, ref left.Normal, out result.Normal);
            Matrix2x3.Multiply(ref left.Vertex, ref right.Vertex, out result.Vertex);
        }
        #endregion


        #region Overrides
        public override int GetHashCode()
        {
            return this.Normal.GetHashCode() ^ this.Vertex.GetHashCode();
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
                Matrix2x2.Equals(ref left.Normal, ref right.Normal) &&
                Matrix2x3.Equals(ref left.Vertex, ref right.Vertex);
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Matrix2D left, ref Matrix2D right)
        {
            return
                Matrix2x2.Equals(ref left.Normal, ref right.Normal) &&
                Matrix2x3.Equals(ref left.Vertex, ref right.Vertex);
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
      */
}