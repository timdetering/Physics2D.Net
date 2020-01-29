// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Edge2DNormalInfo
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public sealed class Edge2DNormalInfo
  {
    public readonly float Distance;
    public readonly Vector2D Normal;

    internal Edge2DNormalInfo(ref float Distance, ref Vector2D Normal)
    {
      this.Distance = Distance;
      this.Normal = Normal;
    }
  }
}
