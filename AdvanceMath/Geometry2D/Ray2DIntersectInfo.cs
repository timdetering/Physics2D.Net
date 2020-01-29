// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Ray2DIntersectInfo
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public class Ray2DIntersectInfo
  {
    public readonly bool Intersects;
    public readonly float DistanceFromOrigin;

    public Ray2DIntersectInfo TestIntersection(Ray2D ray, Line2D line)
    {
      float num = line.Normal * ray.Direction;
      if ((double) Math.Abs(num) < 0.0)
        return new Ray2DIntersectInfo(false, 0.0f);
      float DistanceFromOrigin = (float) -((double) (line.Normal * ray.Origin + line.NDistance) / (double) num);
      return new Ray2DIntersectInfo((double) DistanceFromOrigin >= 0.0, DistanceFromOrigin);
    }

    public Ray2DIntersectInfo(bool Intersects, float DistanceFromOrigin)
    {
      this.Intersects = Intersects;
      this.DistanceFromOrigin = DistanceFromOrigin;
    }
  }
}
