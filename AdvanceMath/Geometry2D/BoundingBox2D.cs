// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.BoundingBox2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.Collections.Generic;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public sealed class BoundingBox2D
  {
    public readonly Vector2D Upper;
    public readonly Vector2D Lower;

    public static BoundingBox2D From2Vectors(Vector2D first, Vector2D second)
    {
      Vector2D Upper = second;
      Vector2D Lower = first;
      if ((double) first.X > (double) second.X)
      {
        Upper.X = first.X;
        Lower.X = second.X;
      }
      if ((double) first.Y > (double) second.Y)
      {
        Upper.Y = first.Y;
        Lower.Y = second.Y;
      }
      return new BoundingBox2D(Upper, Lower);
    }

    public static BoundingBox2D FromVectors(Vector2D[] vectors)
    {
      int length = vectors.Length;
      Vector2D vector1 = vectors[0];
      Vector2D vector2 = vectors[0];
      for (int index = 1; index < length; ++index)
      {
        if ((double) vectors[index].X > (double) vector1.X)
          vector1.X = vectors[index].X;
        else if ((double) vectors[index].X < (double) vector2.X)
          vector2.X = vectors[index].X;
        if ((double) vectors[index].Y > (double) vector1.Y)
          vector1.Y = vectors[index].Y;
        else if ((double) vectors[index].Y < (double) vector2.Y)
          vector2.Y = vectors[index].Y;
      }
      return new BoundingBox2D(vector1, vector2);
    }

    public static BoundingBox2D FromVectors(IList<Vector2D> vectors)
    {
      int count = vectors.Count;
      Vector2D vector1 = vectors[0];
      Vector2D vector2 = vectors[0];
      for (int index = 1; index < count; ++index)
      {
        if ((double) vectors[index].X > (double) vector1.X)
          vector1.X = vectors[index].X;
        else if ((double) vectors[index].X < (double) vector2.X)
          vector2.X = vectors[index].X;
        if ((double) vectors[index].Y > (double) vector1.Y)
          vector1.Y = vectors[index].Y;
        else if ((double) vectors[index].Y < (double) vector2.Y)
          vector2.Y = vectors[index].Y;
      }
      return new BoundingBox2D(vector1, vector2);
    }

    public static BoundingBox2D From2BoundingBox2Ds(
      BoundingBox2D first,
      BoundingBox2D second)
    {
      Vector2D zero1 = Vector2D.Zero;
      Vector2D zero2 = Vector2D.Zero;
      zero1.X = MathHelper.Max(first.Upper.X, second.Upper.X);
      zero1.Y = MathHelper.Max(first.Upper.Y, second.Upper.Y);
      zero2.X = MathHelper.Min(first.Lower.X, second.Lower.X);
      zero2.Y = MathHelper.Min(first.Lower.Y, second.Lower.Y);
      return new BoundingBox2D(zero1, zero2);
    }

    public static BoundingBox2D SmallestFrom2BoundingBox2Ds(
      BoundingBox2D first,
      BoundingBox2D second)
    {
      Vector2D zero1 = Vector2D.Zero;
      Vector2D zero2 = Vector2D.Zero;
      zero1.X = MathHelper.Min(first.Upper.X, second.Upper.X);
      zero1.Y = MathHelper.Min(first.Upper.Y, second.Upper.Y);
      zero2.X = MathHelper.Max(first.Lower.X, second.Lower.X);
      zero2.Y = MathHelper.Max(first.Lower.Y, second.Lower.Y);
      return new BoundingBox2D(zero1, zero2);
    }

    public static bool TestIntersection(BoundingBox2D box, Vector2D point)
    {
      return (double) box.Upper.X >= (double) point.X && (double) box.Lower.X <= (double) point.X && (double) box.Upper.Y >= (double) point.Y && (double) box.Lower.Y <= (double) point.Y;
    }

    public static bool TestIntersection(BoundingBox2D box, Edge2D edge)
    {
      Vector2D position1 = edge.FirstVertex.Position;
      Vector2D position2 = edge.SecondVertex.Position;
      if ((double) box.Upper.X < (double) position1.X && (double) box.Upper.X < (double) position2.X || (double) box.Lower.X > (double) position1.X && (double) box.Lower.X > (double) position2.X || (double) box.Upper.Y < (double) position1.Y && (double) box.Upper.Y < (double) position2.Y)
        return false;
      return (double) box.Lower.Y <= (double) position1.Y || (double) box.Lower.Y <= (double) position2.Y;
    }

    public BoundingBox2D(float upperX, float upperY, float lowerX, float lowerY)
    {
      this.Upper.X = upperX;
      this.Upper.Y = upperY;
      this.Lower.X = lowerX;
      this.Lower.Y = lowerY;
    }

    public BoundingBox2D(Vector2D Upper, Vector2D Lower)
    {
      this.Upper = Upper;
      this.Lower = Lower;
    }

    public Vector2D[] Corners
    {
      get
      {
        return new Vector2D[4]
        {
          this.Upper,
          new Vector2D(this.Lower.X, this.Upper.Y),
          this.Lower,
          new Vector2D(this.Upper.X, this.Lower.Y)
        };
      }
    }

    public bool TestIntersection(BoundingBox2D otherBox)
    {
      return (double) this.Lower.X < (double) otherBox.Upper.X && (double) this.Upper.X > (double) otherBox.Lower.X && (double) otherBox.Lower.Y < (double) this.Upper.Y && (double) otherBox.Upper.Y > (double) this.Lower.Y;
    }

    public BoundingBox2D Move(Vector2D changeInPosition)
    {
      return new BoundingBox2D(this.Upper + changeInPosition, this.Lower + changeInPosition);
    }
  }
}
