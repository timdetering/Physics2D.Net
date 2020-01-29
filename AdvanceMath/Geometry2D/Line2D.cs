// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Line2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public sealed class Line2D
  {
    private Vector2D normal;
    private float nDistance;

    public static Line2D From2Points(Vector2D first, Vector2D second)
    {
      Vector2D vector2D = first - second;
      float magnitude = vector2D.Magnitude;
      if ((double) magnitude <= 0.0)
        return (Line2D) null;
      Line2D line2D = new Line2D()
      {
        normal = 1f / magnitude ^ vector2D
      };
      line2D.nDistance = line2D.Normal * first;
      return line2D;
    }

    public static float CalcDistance(Line2D line, Vector2D point)
    {
      return point * line.normal + line.nDistance;
    }

    public static LineSide CalcLineSide(Line2D line, Vector2D point)
    {
      return (LineSide) Math.Sign(Line2D.CalcDistance(line, point));
    }

    private Line2D()
    {
    }

    public Line2D(Vector2D normal, float c)
    {
      this.normal = normal;
      this.nDistance = c;
    }

    public float NDistance
    {
      get
      {
        return this.nDistance;
      }
    }

    public Vector2D Normal
    {
      get
      {
        return this.normal;
      }
    }
  }
}
