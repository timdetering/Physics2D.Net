// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.RaySegment2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public class RaySegment2D : Ray2D
  {
    private float length;

    public static RaySegment2D From2Points(Vector2D start, Vector2D end)
    {
      Vector2D vector2D = end - start;
      float magnitudeSq = vector2D.MagnitudeSq;
      if ((double) magnitudeSq <= 0.0)
        return (RaySegment2D) null;
      float length = MathHelper.Sqrt(magnitudeSq);
      Vector2D direction = vector2D * (1f / length);
      return new RaySegment2D(start, direction, length);
    }

    public RaySegment2D(Vector2D origin, Vector2D direction, float length)
      : base(origin, direction)
    {
      this.length = length;
    }

    public float Length
    {
      get
      {
        return this.length;
      }
    }

    public BoundingBox2D BoundingBox2D
    {
      get
      {
        return BoundingBox2D.From2Vectors(this.Origin, this.Origin + this.Direction * this.Length);
      }
    }
  }
}
