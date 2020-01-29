// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Polygon2DDistanceInfo
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

namespace AdvanceMath.Geometry2D
{
  public sealed class Polygon2DDistanceInfo
  {
    public readonly Vector2D PointRelativeToWorld;
    public readonly Edge2D ClosestEdge;
    public readonly Edge2DDistanceInfo DistanceInfo;
    public readonly Edge2DNormalInfo NormalInfo;
    public readonly bool InsideEdgesChecked;

    internal Polygon2DDistanceInfo(
      ref Vector2D PointRelativeToWorld,
      Edge2D ClosestEdge,
      Edge2DDistanceInfo DistanceInfo,
      Edge2DNormalInfo NormalInfo,
      ref bool InsideEdgesChecked)
    {
      this.PointRelativeToWorld = PointRelativeToWorld;
      this.ClosestEdge = ClosestEdge;
      this.DistanceInfo = DistanceInfo;
      this.NormalInfo = NormalInfo;
      this.InsideEdgesChecked = InsideEdgesChecked;
    }
  }
}
