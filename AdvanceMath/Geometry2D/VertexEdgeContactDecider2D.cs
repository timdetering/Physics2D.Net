// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.VertexEdgeContactDecider2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public class VertexEdgeContactDecider2D
  {
    private bool[][] possibleVertexEdgeCollisionsFor1;
    private bool[][] possibleVertexEdgeCollisionsFor2;

    public VertexEdgeContactDecider2D(Polygon2D geometry1, Polygon2D geometry2)
    {
      Edge2D[] edges1 = geometry1.Edges;
      Edge2D[] edges2 = geometry2.Edges;
      int length1 = edges1.Length;
      int length2 = edges2.Length;
      int[,] numArray = new int[length1, length2];
      for (int index1 = 0; index1 != length1; ++index1)
      {
        for (int index2 = 0; index2 != length2; ++index2)
        {
          float num = edges1[index1].Normal * edges2[index2].NormalizedEdge;
          numArray[index1, index2] = Math.Sign(Math.Round((double) num, 5));
        }
      }
      this.possibleVertexEdgeCollisionsFor1 = new bool[length1][];
      for (int index1 = 0; index1 != length1; ++index1)
      {
        this.possibleVertexEdgeCollisionsFor1[index1] = new bool[length2];
        int index2 = index1;
        int index3 = index1 != 0 ? index1 - 1 : length1 - 1;
        for (int index4 = 0; index4 != length2; ++index4)
        {
          int num1 = numArray[index2, index4];
          int num2 = numArray[index3, index4];
          this.possibleVertexEdgeCollisionsFor1[index1][index4] = (num2 == -1 || num2 == 0) && (num1 == 1 || num1 == 0);
        }
      }
      this.possibleVertexEdgeCollisionsFor2 = new bool[length2][];
      for (int index1 = 0; index1 != length2; ++index1)
      {
        this.possibleVertexEdgeCollisionsFor2[index1] = new bool[length1];
        int index2 = index1;
        int index3 = index1 != 0 ? index1 - 1 : length2 - 1;
        for (int index4 = 0; index4 != length1; ++index4)
        {
          int num1 = numArray[index4, index2];
          int num2 = numArray[index4, index3];
          this.possibleVertexEdgeCollisionsFor2[index1][index4] = (num1 == -1 || num1 == 0) && (num2 == 1 || num2 == 0);
        }
      }
    }

    public bool[][] PossibleVertexEdgeCollisionsFor1
    {
      get
      {
        return this.possibleVertexEdgeCollisionsFor1;
      }
    }

    public bool[][] PossibleVertexEdgeCollisionsFor2
    {
      get
      {
        return this.possibleVertexEdgeCollisionsFor2;
      }
    }
  }
}
