// Decompiled with JetBrains decompiler
// Type: AdvanceMath.IO.MathReader
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using AdvanceMath.Geometry2D;
using System.IO;
using System.Text;

namespace AdvanceMath.IO
{
  public class MathReader : BinaryReader
  {
    public MathReader(Stream input)
      : base(input)
    {
    }

    public MathReader(Stream input, Encoding encoding)
      : base(input, encoding)
    {
    }

    public MathReader(BinaryReader reader)
      : base(reader.BaseStream)
    {
    }

    private float ReadScalar()
    {
      return this.ReadSingle();
    }

    public Vector2D ReadVector2D()
    {
      Vector2D vector2D;
      vector2D.X = this.ReadScalar();
      vector2D.Y = this.ReadScalar();
      return vector2D;
    }

    public Vector3D ReadVector3D()
    {
      Vector3D vector3D;
      vector3D.X = this.ReadScalar();
      vector3D.Y = this.ReadScalar();
      vector3D.Z = this.ReadScalar();
      return vector3D;
    }

    public Vector4D ReadVector4D()
    {
      Vector4D vector4D;
      vector4D.X = this.ReadScalar();
      vector4D.Y = this.ReadScalar();
      vector4D.Z = this.ReadScalar();
      vector4D.W = this.ReadScalar();
      return vector4D;
    }

    public Quaternion ReadQuaternion()
    {
      Quaternion quaternion;
      quaternion.X = this.ReadScalar();
      quaternion.Y = this.ReadScalar();
      quaternion.Z = this.ReadScalar();
      quaternion.W = this.ReadScalar();
      return quaternion;
    }

    public Matrix2x2 ReadMatrix2x2()
    {
      Matrix2x2 matrix2x2;
      matrix2x2.m00 = this.ReadScalar();
      matrix2x2.m01 = this.ReadScalar();
      matrix2x2.m10 = this.ReadScalar();
      matrix2x2.m11 = this.ReadScalar();
      return matrix2x2;
    }

    public Matrix3x3 ReadMatrix3x3()
    {
      Matrix3x3 matrix3x3;
      matrix3x3.m00 = this.ReadScalar();
      matrix3x3.m01 = this.ReadScalar();
      matrix3x3.m02 = this.ReadScalar();
      matrix3x3.m10 = this.ReadScalar();
      matrix3x3.m11 = this.ReadScalar();
      matrix3x3.m12 = this.ReadScalar();
      matrix3x3.m20 = this.ReadScalar();
      matrix3x3.m21 = this.ReadScalar();
      matrix3x3.m22 = this.ReadScalar();
      return matrix3x3;
    }

    public Matrix4x4 ReadMatrix4x4()
    {
      Matrix4x4 matrix4x4;
      matrix4x4.m00 = this.ReadScalar();
      matrix4x4.m01 = this.ReadScalar();
      matrix4x4.m02 = this.ReadScalar();
      matrix4x4.m03 = this.ReadScalar();
      matrix4x4.m10 = this.ReadScalar();
      matrix4x4.m11 = this.ReadScalar();
      matrix4x4.m12 = this.ReadScalar();
      matrix4x4.m13 = this.ReadScalar();
      matrix4x4.m20 = this.ReadScalar();
      matrix4x4.m21 = this.ReadScalar();
      matrix4x4.m22 = this.ReadScalar();
      matrix4x4.m23 = this.ReadScalar();
      matrix4x4.m30 = this.ReadScalar();
      matrix4x4.m31 = this.ReadScalar();
      matrix4x4.m32 = this.ReadScalar();
      matrix4x4.m33 = this.ReadScalar();
      return matrix4x4;
    }

    public ALVector2D ReadAlVector2D()
    {
      ALVector2D alVector2D;
      alVector2D.Angular = this.ReadScalar();
      alVector2D.Linear = this.ReadVector2D();
      return alVector2D;
    }

    public float[] ReadScalarArray()
    {
      float[] numArray = new float[this.ReadInt32()];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = this.ReadScalar();
      return numArray;
    }
  }
}
