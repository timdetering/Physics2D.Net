// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Edge2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public sealed class Edge2D
  {
    private Vertex2D first;
    private Vertex2D second;
    private Vector2D edge;
    private Vector2D tangent;
    private Vector2D normal;
    private Edge2D.OriginalEdge2DInfo original;

    public Edge2D(Vertex2D first, Vertex2D second, Matrix2D matrix)
    {
      this.first = first;
      this.second = second;
      this.edge = this.first.OriginalPosition - this.second.OriginalPosition;
      float magnitude = this.edge.Magnitude;
      if ((double) magnitude > 9.99999971718069E-10)
      {
        this.tangent = this.edge * (1f / magnitude);
        this.normal = Vector2D.GetRightHandNormal(this.tangent);
      }
      else
      {
        this.tangent = Vector2D.Zero;
        this.normal = Vector2D.Zero;
      }
      this.original = new Edge2D.OriginalEdge2DInfo(this.edge, this.tangent, this.normal, magnitude);
      this.Transform(matrix);
    }

    public Edge2D(Edge2D copy, Vertex2D first, Vertex2D second)
    {
      this.first = first;
      this.second = second;
      this.edge = copy.edge;
      this.tangent = copy.tangent;
      this.normal = copy.normal;
      this.original = copy.original;
    }

    public Vertex2D FirstVertex
    {
      get
      {
        return this.first;
      }
    }

    public Vertex2D SecondVertex
    {
      get
      {
        return this.second;
      }
    }

    public Vector2D Edge
    {
      get
      {
        return this.edge;
      }
    }

    public Vector2D NormalizedEdge
    {
      get
      {
        return this.tangent;
      }
    }

    public Vector2D Normal
    {
      get
      {
        return this.normal;
      }
    }

    public float Magnitude
    {
      get
      {
        return this.original.Magnitude;
      }
    }

    internal void Set(Edge2D copy)
    {
      this.edge = copy.edge;
      this.tangent = copy.tangent;
      this.normal = copy.normal;
    }

    public void Transform(Matrix2D matrix)
    {
      this.tangent = matrix.NormalMatrix * this.original.Tangent;
      this.normal = matrix.NormalMatrix * this.original.Normal;
      this.edge = matrix.VertexMatrix * this.original.Edge;
    }

    public static bool TestIntersection(Edge2D first, Edge2D second)
    {
      Vector2D vector2D = second.FirstVertex.Position - first.FirstVertex.Position;
      Vector2D edge1 = first.edge;
      Vector2D edge2 = second.edge;
      float num1 = edge2 ^ vector2D;
      float num2 = edge1 ^ vector2D;
      float num3 = 1f / (edge1 ^ edge2);
      float num4 = num1 * num3;
      float num5 = num2 * num3;
      return (double) num5 <= 1.0 && 0.0 <= (double) num5 && (double) num4 <= 1.0 && 0.0 <= (double) num4;
    }

    public Edge2DDistanceInfo CalcDistanceInfo(Vector2D pointRelativeToWorld)
    {
      Edge2DDistanceInfo edge2DdistanceInfo = new Edge2DDistanceInfo();
      Vector2D vector2D = pointRelativeToWorld - this.second.Position;
      edge2DdistanceInfo.DistanceProjOnNormal = vector2D * this.normal;
      edge2DdistanceInfo.DistanceProjOnTangant = vector2D * this.tangent;
      edge2DdistanceInfo.BehindEdge2D = (double) edge2DdistanceInfo.DistanceProjOnNormal < 0.0;
      if ((double) edge2DdistanceInfo.DistanceProjOnTangant < 0.0)
      {
        edge2DdistanceInfo.DistanceSq = (float) ((double) edge2DdistanceInfo.DistanceProjOnTangant * (double) edge2DdistanceInfo.DistanceProjOnTangant + (double) edge2DdistanceInfo.DistanceProjOnNormal * (double) edge2DdistanceInfo.DistanceProjOnNormal);
        edge2DdistanceInfo.InEdgesVoronoiRegion = false;
      }
      else if ((double) edge2DdistanceInfo.DistanceProjOnTangant > (double) this.original.Magnitude)
      {
        edge2DdistanceInfo.DistanceProjOnTangant -= this.original.Magnitude;
        edge2DdistanceInfo.InEdgesVoronoiRegion = false;
        edge2DdistanceInfo.DistanceSq = (float) ((double) edge2DdistanceInfo.DistanceProjOnTangant * (double) edge2DdistanceInfo.DistanceProjOnTangant + (double) edge2DdistanceInfo.DistanceProjOnNormal * (double) edge2DdistanceInfo.DistanceProjOnNormal);
      }
      else
      {
        edge2DdistanceInfo.DistanceSq = edge2DdistanceInfo.DistanceProjOnNormal * edge2DdistanceInfo.DistanceProjOnNormal;
        edge2DdistanceInfo.DistanceProjOnTangant = 0.0f;
        edge2DdistanceInfo.InEdgesVoronoiRegion = true;
      }
      return edge2DdistanceInfo;
    }

    public Edge2DNormalInfo CalcNormalInfo(
      Edge2DDistanceInfo info,
      bool useEdgesNormal)
    {
      float Distance = MathHelper.Sqrt(info.DistanceSq);
      bool flag = (double) info.DistanceProjOnNormal < 0.0;
      Vector2D Normal;
      if (!info.InEdgesVoronoiRegion && !useEdgesNormal)
      {
        float num = 1f / Distance;
        Normal = !flag ? info.DistanceProjOnNormal * num * this.normal + info.DistanceProjOnTangant * num * this.tangent : -info.DistanceProjOnNormal * num * this.normal + info.DistanceProjOnTangant * num * this.tangent;
      }
      else
        Normal = this.normal;
      if (flag)
        Distance = -Distance;
      return new Edge2DNormalInfo(ref Distance, ref Normal);
    }

    public Line2D ToLine2D()
    {
      return new Line2D(this.normal, this.first.Position * this.normal);
    }

    private sealed class OriginalEdge2DInfo
    {
      public readonly Vector2D Edge;
      public readonly Vector2D Tangent;
      public readonly Vector2D Normal;
      public readonly float Magnitude;

      public OriginalEdge2DInfo(Vector2D Edge, Vector2D Tangent, Vector2D Normal, float Magnitude)
      {
        this.Edge = Edge;
        this.Tangent = Tangent;
        this.Normal = Normal;
        this.Magnitude = Magnitude;
      }
    }
  }
}
