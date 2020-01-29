// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Ray2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public class Ray2D
  {
    private Vector2D origin;
    private Vector2D direction;

    public Ray2D(Vector2D origin, Vector2D direction)
    {
      this.origin = origin;
      this.direction = direction;
    }

    public Vector2D Origin
    {
      get
      {
        return this.origin;
      }
      set
      {
        this.origin = value;
      }
    }

    public Vector2D Direction
    {
      get
      {
        return this.direction;
      }
      set
      {
        this.direction = value;
      }
    }

    public Vector2D GetPoint(Ray2DIntersectInfo info)
    {
      return this.direction * info.DistanceFromOrigin + this.origin;
    }
  }
}
