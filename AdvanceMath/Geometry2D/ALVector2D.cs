// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.ALVector2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using AdvanceMath.Design;
using System;
using System.ComponentModel;

namespace AdvanceMath.Geometry2D
{
  [TypeConverter(typeof (AdvTypeConverter<ALVector2D>))]
  [AdvBrowsableOrder("Angular,Linear")]
  [Serializable]
  public struct ALVector2D
  {
    public static readonly ALVector2D Zero = new ALVector2D(0.0f, Vector2D.Zero);
    [AdvBrowsable]
    public float Angular;
    [AdvBrowsable]
    public Vector2D Linear;

    public static ALVector2D Parse(string text)
    {
      string[] strArray = text.Trim(' ', '(', '[', '<', ')', ']', '>').Split(new char[1]
      {
        ','
      }, 2);
      if (strArray.Length != 2)
        throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 2 parts separated by commas in the form (x,y) with optional parenthesis.", (object) text));
      try
      {
        ALVector2D alVector2D;
        alVector2D.Angular = float.Parse(strArray[0]);
        alVector2D.Linear = Vector2D.Parse(strArray[1]);
        return alVector2D;
      }
      catch (Exception ex)
      {
        throw new FormatException("The parts of the vectors must be decimal numbers", ex);
      }
    }

    [InstanceConstructor("Angular,Linear")]
    public ALVector2D(float Angular, Vector2D Linear)
    {
      this.Angular = Angular;
      this.Linear = Linear;
    }

    public ALVector2D(float Angular, float X, float Y)
    {
      this.Angular = Angular;
      this.Linear = new Vector2D(X, Y);
    }

    public static ALVector2D operator +(ALVector2D left, ALVector2D right)
    {
      return new ALVector2D(left.Angular + right.Angular, left.Linear + right.Linear);
    }

    public static ALVector2D operator -(ALVector2D left, ALVector2D right)
    {
      return new ALVector2D(left.Angular - right.Angular, left.Linear - right.Linear);
    }

    public static ALVector2D operator *(ALVector2D source, float scalar)
    {
      return new ALVector2D(source.Angular * scalar, source.Linear * scalar);
    }

    public static ALVector2D operator *(float scalar, ALVector2D source)
    {
      return new ALVector2D(scalar * source.Angular, scalar * source.Linear);
    }

    public static bool operator ==(ALVector2D left, ALVector2D right)
    {
      return (double) left.Angular == (double) right.Angular && left.Linear == right.Linear;
    }

    public static bool operator !=(ALVector2D left, ALVector2D right)
    {
      return (double) left.Angular != (double) right.Angular && left.Linear != right.Linear;
    }

    public override bool Equals(object obj)
    {
      return obj is ALVector2D alVector2D ? this == alVector2D : base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.Angular.GetHashCode() ^ this.Linear.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("({0}, {1})", (object) this.Angular, (object) this.Linear);
    }

    public Matrix4x4 ToMatrix4x4()
    {
      return Matrix4x4.FromTranslation((Vector3D) this.Linear) * Matrix3x3.FromRotationZ(this.Angular);
    }

    public Matrix2D ToMatrix2D()
    {
      Matrix2x2 NormalMatrix = Matrix2x2.FromRotation(this.Angular);
      return new Matrix2D(NormalMatrix, Matrix3x3.FromTranslate2D(this.Linear) * NormalMatrix);
    }
  }
}
