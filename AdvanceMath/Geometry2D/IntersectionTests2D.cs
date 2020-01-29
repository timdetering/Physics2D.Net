// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.IntersectionTests2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.Collections.Generic;

namespace AdvanceMath.Geometry2D
{
  public class IntersectionTests2D
  {
    private IntersectionTests2D()
    {
    }

    public static Ray2DIntersectInfo TestIntersection(Ray2D ray, Edge2D[] edges)
    {
      Ray2DIntersectInfo ray2DintersectInfo1 = (Ray2DIntersectInfo) null;
      foreach (Edge2D edge in edges)
      {
        Ray2DIntersectInfo ray2DintersectInfo2 = IntersectionTests2D.TestIntersection(ray, edge);
        if (ray2DintersectInfo2.Intersects && (ray2DintersectInfo1 == null || (double) ray2DintersectInfo1.DistanceFromOrigin > (double) ray2DintersectInfo2.DistanceFromOrigin))
          ray2DintersectInfo1 = ray2DintersectInfo2;
      }
      if (ray2DintersectInfo1 == null)
        ray2DintersectInfo1 = new Ray2DIntersectInfo(false, 0.0f);
      return ray2DintersectInfo1;
    }

    public static Ray2DIntersectInfo TestIntersectionOLD(
      Ray2D ray,
      Edge2D[] edges)
    {
      List<Ray2DIntersectInfo> ray2DintersectInfoList = new List<Ray2DIntersectInfo>();
      foreach (Edge2D edge in edges)
        ray2DintersectInfoList.Add(IntersectionTests2D.TestIntersection(ray, edge));
      int length = edges.Length;
      Ray2DIntersectInfo ray2DintersectInfo1 = (Ray2DIntersectInfo) null;
      foreach (Ray2DIntersectInfo ray2DintersectInfo2 in ray2DintersectInfoList)
      {
        if (ray2DintersectInfo2.Intersects && (ray2DintersectInfo1 == null || (double) ray2DintersectInfo1.DistanceFromOrigin > (double) ray2DintersectInfo2.DistanceFromOrigin))
          ray2DintersectInfo1 = ray2DintersectInfo2;
      }
      return ray2DintersectInfo1 ?? ray2DintersectInfoList[0];
    }

    public static Ray2DIntersectInfo TestIntersection(Ray2D ray, Edge2D edge)
    {
      float num1 = edge.Normal * ray.Direction;
      if ((double) Math.Abs(num1) < 9.99999971718069E-10)
        return new Ray2DIntersectInfo(false, 0.0f);
      Vector2D vector2D = ray.Origin - edge.SecondVertex.Position;
      float DistanceFromOrigin = (float) -((double) (edge.Normal * vector2D) / (double) num1);
      if ((double) DistanceFromOrigin < 0.0)
        return new Ray2DIntersectInfo(false, 0.0f);
      float num2 = (vector2D + ray.Direction * DistanceFromOrigin) * edge.NormalizedEdge;
      return new Ray2DIntersectInfo((double) num2 >= 0.0 && (double) num2 <= (double) edge.Magnitude, DistanceFromOrigin);
    }

    public static Ray2DIntersectInfo TestIntersection(Ray2D ray, Line2D line)
    {
      float num = line.Normal * ray.Direction;
      if (-(double) num <= 0.0)
        return new Ray2DIntersectInfo(false, 0.0f);
      float DistanceFromOrigin = (float) -((double) (line.Normal * ray.Origin + line.NDistance) / (double) num);
      return new Ray2DIntersectInfo((double) DistanceFromOrigin >= 0.0, DistanceFromOrigin);
    }

    public static Ray2DIntersectInfo TestIntersection(
      Ray2D ray,
      Line2D[] lines,
      bool normalIsOutside)
    {
      bool flag = true;
      bool Intersects = false;
      float num = 0.0f;
      LineSide lineSide = normalIsOutside ? LineSide.Positive : LineSide.Negitive;
      foreach (Line2D line in lines)
      {
        if (Line2D.CalcLineSide(line, ray.Origin) == lineSide)
        {
          flag = false;
          Ray2DIntersectInfo ray2DintersectInfo = IntersectionTests2D.TestIntersection(ray, line);
          if (ray2DintersectInfo.Intersects)
          {
            Intersects = true;
            num = MathHelper.Max(num, ray2DintersectInfo.DistanceFromOrigin);
          }
        }
      }
      if (flag)
      {
        Intersects = true;
        num = 0.0f;
      }
      return new Ray2DIntersectInfo(Intersects, num);
    }

    public static Ray2DIntersectInfo TestIntersection(
      Ray2D ray,
      Circle2D circle,
      bool discardInside)
    {
      Vector2D direction = ray.Direction;
      Vector2D vector2D = ray.Origin - circle.Center;
      float radius = circle.Radius;
      float num = radius * radius;
      float magnitudeSq = vector2D.MagnitudeSq;
      if ((double) magnitudeSq <= (double) num && discardInside)
        return new Ray2DIntersectInfo(true, 0.0f);
      float plus;
      float minus;
      if (!MathHelper.TrySolveQuadratic(direction.MagnitudeSq, 2f * vector2D * direction, magnitudeSq - num, out plus, out minus))
        return new Ray2DIntersectInfo(false, 0.0f);
      if ((double) minus >= 0.0)
        return new Ray2DIntersectInfo(true, minus);
      return (double) plus > 0.0 ? new Ray2DIntersectInfo(true, plus) : new Ray2DIntersectInfo(false, 0.0f);
    }

    public static Ray2DIntersectInfo TestIntersection(
      Ray2D ray,
      BoundingBox2D box)
    {
      Vector2D origin = ray.Origin;
      if (BoundingBox2D.TestIntersection(box, origin))
        return new Ray2DIntersectInfo(true, 0.0f);
      float DistanceFromOrigin = 0.0f;
      bool Intersects = false;
      Vector2D lower = box.Lower;
      Vector2D upper = box.Upper;
      Vector2D direction = ray.Direction;
      Vector2D vector2D;
      if ((double) origin.X < (double) lower.X && (double) direction.X > 0.0)
      {
        float num = (lower.X - origin.X) / direction.X;
        if ((double) num > 0.0)
        {
          vector2D = origin + direction * num;
          if ((double) vector2D.Y >= (double) lower.Y && (double) vector2D.Y <= (double) upper.Y && (!Intersects || (double) num < (double) DistanceFromOrigin))
          {
            Intersects = true;
            DistanceFromOrigin = num;
          }
        }
      }
      if ((double) origin.X > (double) upper.X && (double) direction.X < 0.0)
      {
        float num = (upper.X - origin.X) / direction.X;
        if ((double) num > 0.0)
        {
          vector2D = origin + direction * num;
          if ((double) vector2D.Y >= (double) lower.Y && (double) vector2D.Y <= (double) upper.Y && (!Intersects || (double) num < (double) DistanceFromOrigin))
          {
            Intersects = true;
            DistanceFromOrigin = num;
          }
        }
      }
      if ((double) origin.Y < (double) lower.Y && (double) direction.Y > 0.0)
      {
        float num = (lower.Y - origin.Y) / direction.Y;
        if ((double) num > 0.0)
        {
          vector2D = origin + direction * num;
          if ((double) vector2D.X >= (double) lower.X && (double) vector2D.X <= (double) upper.X && (!Intersects || (double) num < (double) DistanceFromOrigin))
          {
            Intersects = true;
            DistanceFromOrigin = num;
          }
        }
      }
      if ((double) origin.Y > (double) upper.Y && (double) direction.Y < 0.0)
      {
        float num = (upper.Y - origin.Y) / direction.Y;
        if ((double) num > 0.0)
        {
          vector2D = origin + direction * num;
          if ((double) vector2D.X >= (double) lower.X && (double) vector2D.X <= (double) upper.X && (!Intersects || (double) num < (double) DistanceFromOrigin))
          {
            Intersects = true;
            DistanceFromOrigin = num;
          }
        }
      }
      return new Ray2DIntersectInfo(Intersects, DistanceFromOrigin);
    }

    public static bool TestIntersection(Circle2D circle, BoundingBox2D box)
    {
      Vector2D center = circle.Center;
      float radius = circle.Radius;
      Vector2D lower = box.Lower;
      Vector2D upper = box.Upper;
      return ((double) center.X >= (double) lower.X || (double) lower.X - (double) center.X <= (double) radius) && ((double) center.X <= (double) upper.X || (double) center.X - (double) upper.X <= (double) radius) && (((double) center.Y >= (double) lower.Y || (double) lower.Y - (double) center.Y <= (double) radius) && ((double) center.Y <= (double) upper.Y || (double) center.Y - (double) upper.Y <= (double) radius));
    }

    public static bool TestIntersection(Line2D line, BoundingBox2D box)
    {
      Vector2D[] corners = box.Corners;
      LineSide lineSide = Line2D.CalcLineSide(line, corners[0]);
      for (int index = 1; index < 8; ++index)
      {
        if (Line2D.CalcLineSide(line, corners[index]) != lineSide)
          return true;
      }
      return false;
    }

    public static bool TestIntersection(Circle2D circle, Line2D line)
    {
      return (double) Math.Abs(line.Normal * circle.Center) <= (double) circle.Radius;
    }
  }
}
