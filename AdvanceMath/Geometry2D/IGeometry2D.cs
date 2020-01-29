// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.IGeometry2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

namespace AdvanceMath.Geometry2D
{
  public interface IGeometry2D : IHasBoundingBox2D
  {
    bool TestIntersection(Vector2D point);

    float Area { get; }

    float Perimeter { get; }

    Vector2D Centroid { get; }

    ALVector2D Position { get; set; }

    void Shift(Vector2D offset);

    float BoundingRadius { get; }

    float InnerRadius { get; }
  }
}
