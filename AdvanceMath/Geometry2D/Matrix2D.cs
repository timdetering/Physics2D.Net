// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Matrix2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Geometry2D
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

    public static Matrix2D operator *(Matrix2D left, Matrix3x3 right)
    {
      Matrix2D matrix2D;
      matrix2D.NormalMatrix = left.NormalMatrix;
      matrix2D.VertexMatrix = left.VertexMatrix * right;
      return matrix2D;
    }

    public static Matrix2D operator *(Matrix3x3 left, Matrix2D right)
    {
      Matrix2D matrix2D;
      matrix2D.NormalMatrix = right.NormalMatrix;
      matrix2D.VertexMatrix = left * right.VertexMatrix;
      return matrix2D;
    }

    public static Matrix2D operator *(Matrix2D left, Matrix2x2 right)
    {
      Matrix2D matrix2D;
      matrix2D.NormalMatrix = left.NormalMatrix * right;
      matrix2D.VertexMatrix = left.VertexMatrix * right;
      return matrix2D;
    }

    public static Matrix2D operator *(Matrix2x2 left, Matrix2D right)
    {
      Matrix2D matrix2D;
      matrix2D.NormalMatrix = left * right.NormalMatrix;
      matrix2D.VertexMatrix = left * right.VertexMatrix;
      return matrix2D;
    }

    public static Matrix2D operator *(Matrix2D left, Matrix2D right)
    {
      Matrix2D matrix2D;
      matrix2D.NormalMatrix = left.NormalMatrix * right.NormalMatrix;
      matrix2D.VertexMatrix = left.VertexMatrix * right.VertexMatrix;
      return matrix2D;
    }
  }
}
