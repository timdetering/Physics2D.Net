// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Vertex2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.Runtime.InteropServices;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  [StructLayout(LayoutKind.Sequential)]
  public sealed class Vertex2D
  {
    private Vector2D position;
    private Vector2D original;

    public static Vector2D[] PositionToVector2DArray(Vertex2D[] Vertices)
    {
      int length = Vertices.Length;
      Vector2D[] vector2DArray = new Vector2D[length];
      for (int index = 0; index < length; ++index)
        vector2DArray[index] = Vertices[index].Position;
      return vector2DArray;
    }

    public static Vector2D[] OriginalPositionToVector2DArray(Vertex2D[] Vertices)
    {
      int length = Vertices.Length;
      Vector2D[] vector2DArray = new Vector2D[length];
      for (int index = 0; index < length; ++index)
        vector2DArray[index] = Vertices[index].original;
      return vector2DArray;
    }

    public Vertex2D(Vector2D position)
    {
      this.original = position;
      this.position = position;
    }

    public Vertex2D(Vector2D position, Matrix2D matrix)
    {
      this.original = position;
      this.position = matrix.VertexMatrix * position;
    }

    public Vertex2D(Vertex2D copy)
    {
      this.position = copy.position;
      this.original = copy.original;
    }

    public Vector2D OriginalPosition
    {
      get
      {
        return this.original;
      }
      internal set
      {
        if (!(value != this.original))
          return;
        this.position -= this.original;
        this.original = value;
        this.position += this.original;
      }
    }

    public Vector2D Position
    {
      get
      {
        return this.position;
      }
    }

    internal void Set(Vertex2D copy)
    {
      this.position = copy.position;
    }

    public void Transform(Matrix3x3 matrix)
    {
      this.position = matrix * this.original;
    }
  }
}
