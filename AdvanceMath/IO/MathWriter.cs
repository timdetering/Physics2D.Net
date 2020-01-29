// Decompiled with JetBrains decompiler
// Type: AdvanceMath.IO.MathWriter
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using AdvanceMath.Geometry2D;
using System.IO;
using System.Text;

namespace AdvanceMath.IO
{
  public class MathWriter : BinaryWriter
  {
    public MathWriter(Stream output)
      : base(output)
    {
    }

    public MathWriter(Stream output, Encoding encoding)
      : base(output, encoding)
    {
    }

    public MathWriter(BinaryWriter writer)
      : base(writer.BaseStream)
    {
    }

    public void Write(Vector2D value)
    {
      this.Write(value.X);
      this.Write(value.Y);
    }

    public void Write(Vector3D value)
    {
      this.Write(value.X);
      this.Write(value.Y);
      this.Write(value.Z);
    }

    public void Write(Vector4D value)
    {
      this.Write(value.X);
      this.Write(value.Y);
      this.Write(value.Z);
      this.Write(value.W);
    }

    public void Write(Quaternion value)
    {
      this.Write(value.X);
      this.Write(value.Y);
      this.Write(value.Z);
      this.Write(value.W);
    }

    public void Write(Matrix2x2 value)
    {
      this.Write(value.m00);
      this.Write(value.m01);
      this.Write(value.m10);
      this.Write(value.m11);
    }

    public void Write(Matrix3x3 value)
    {
      this.Write(value.m00);
      this.Write(value.m01);
      this.Write(value.m02);
      this.Write(value.m10);
      this.Write(value.m11);
      this.Write(value.m12);
      this.Write(value.m20);
      this.Write(value.m21);
      this.Write(value.m22);
    }

    public void Write(Matrix4x4 value)
    {
      this.Write(value.m00);
      this.Write(value.m01);
      this.Write(value.m02);
      this.Write(value.m03);
      this.Write(value.m10);
      this.Write(value.m11);
      this.Write(value.m12);
      this.Write(value.m13);
      this.Write(value.m20);
      this.Write(value.m21);
      this.Write(value.m22);
      this.Write(value.m23);
      this.Write(value.m30);
      this.Write(value.m31);
      this.Write(value.m32);
      this.Write(value.m33);
    }

    public void Write(ALVector2D value)
    {
      this.Write(value.Angular);
      this.Write(value.Linear);
    }

    public void Write(float[] array)
    {
      this.Write(array.Length);
      for (int index = 0; index < array.Length; ++index)
        this.Write(array[index]);
    }
  }
}
