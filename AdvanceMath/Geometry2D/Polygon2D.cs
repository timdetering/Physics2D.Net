// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Polygon2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public sealed class Polygon2D : IGeometry2D, IHasBoundingBox2D, IDeserializationCallback
  {
    [NonSerialized]
    private Edge2D[] edges;
    private Vertex2D[] vertices;
    private ALVector2D position;
    [NonSerialized]
    private float boundingRadius;
    [NonSerialized]
    private float innerRadius;
    [NonSerialized]
    private int length;
    [NonSerialized]
    private bool isBoxlike;
    [NonSerialized]
    private BoundingBox2D boundingBox2D;

    public static Vector2D[] FromSquare(float length)
    {
      float num1 = length / 2f;
      double num2 = (double) MathHelper.Sqrt((float) (2.0 * ((double) num1 * (double) num1)));
      return new Vector2D[4]
      {
        new Vector2D(num1, num1),
        new Vector2D(-num1, num1),
        new Vector2D(-num1, -num1),
        new Vector2D(num1, -num1)
      };
    }

    public static Vector2D[] FromRectangle(float length, float width)
    {
      float num1 = length / 2f;
      float num2 = width / 2f;
      double num3 = (double) MathHelper.Sqrt((float) ((double) num1 * (double) num1 + (double) num2 * (double) num2));
      double num4 = (double) MathHelper.Min(num1, num2);
      return new Vector2D[4]
      {
        new Vector2D(num2, num1),
        new Vector2D(-num2, num1),
        new Vector2D(-num2, -num1),
        new Vector2D(num2, -num1)
      };
    }

    public static Vector2D[] FromNumberofSidesAndRadius(int NumberofSides, float Radius)
    {
      if (NumberofSides < 3)
        throw new ArgumentException("Too Few Number of Sides. There must be more then 2. ");
      Vector2D[] vector2DArray = new Vector2D[NumberofSides];
      float num = 6.283185f / (float) NumberofSides;
      for (int index = 0; index != NumberofSides; ++index)
        vector2DArray[index] = Vector2D.FromLengthAndAngle(Radius, (float) index * num);
      return vector2DArray;
    }

    public static float CalculateInertia(Vector2D[] A, float mass)
    {
      if (A.Length == 1)
        return 0.0f;
      float num1 = 0.0f;
      float num2 = 0.0f;
      int index1 = A.Length - 1;
      for (int index2 = 0; index2 < A.Length; ++index2)
      {
        Vector2D vector2D1 = A[index1];
        Vector2D vector2D2 = A[index2];
        float num3 = MathHelper.Abs(vector2D1 ^ vector2D2);
        float num4 = vector2D2 * vector2D2 + vector2D2 * vector2D1 + vector2D1 * vector2D1;
        num1 += num3 * num4;
        num2 += num3;
        index1 = index2;
      }
      return (float) ((double) mass / 6.0 * ((double) num1 / (double) num2));
    }

    public static explicit operator Polygon2D(Vector2D[] vertices)
    {
      return new Polygon2D(vertices);
    }

    public static float CalcBoundingRadius(Vector2D[] vertices)
    {
      float num = vertices[0].MagnitudeSq;
      int length = vertices.Length;
      for (int index = 1; index < length; ++index)
        num = MathHelper.Max(num, vertices[index].MagnitudeSq);
      return MathHelper.Sqrt(num);
    }

    public static float CalcArea(Vector2D[] vertices)
    {
      float num = 0.0f;
      int length = vertices.Length;
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = (index1 + 1) % length;
        num += vertices[index1] ^ vertices[index2];
      }
      return (float) Math.Abs((double) num * 0.5);
    }

    public static Vector2D CalcCentroid(Vector2D[] vertices)
    {
      Vector2D zero = Vector2D.Zero;
      int length = vertices.Length;
      for (int index1 = 0; index1 != length; ++index1)
      {
        int index2 = (index1 + 1) % length;
        zero += (vertices[index1] + vertices[index2]) * (vertices[index1] ^ vertices[index2]);
      }
      return zero * (float) (1.0 / ((double) Polygon2D.CalcArea(vertices) * 6.0));
    }

    public static Vector2D[] MakeCentroidOrigin(Vector2D[] vertices)
    {
      Vector2D left = Polygon2D.CalcCentroid(vertices);
      return OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(ref left, vertices, new RefOperation<Vector2D, Vector2D, Vector2D>(Vector2D.Add));
    }

    public static float CalcPerimeter(Vector2D[] vertices)
    {
      float num = 0.0f;
      int length = vertices.Length;
      for (int index1 = 0; index1 != length; ++index1)
      {
        int index2 = (index1 + 1) % length;
        num += (vertices[index1] - vertices[index2]).Magnitude;
      }
      return num;
    }

    public static void CalcRadius(
      Vector2D[] vertices,
      out float innerRadius,
      out float outerRadius)
    {
      innerRadius = float.MaxValue;
      outerRadius = 0.0f;
      int length = vertices.Length;
      for (int index = 0; index < length; ++index)
      {
        Vector2D rightHandNormal = Vector2D.GetRightHandNormal(Vector2D.Normalize(index != length - 1 ? vertices[index] - vertices[index + 1] : vertices[index] - vertices[0]));
        float magnitude1 = (vertices[index] * rightHandNormal * rightHandNormal).Magnitude;
        float magnitude2 = vertices[index].Magnitude;
        if ((double) magnitude1 < (double) innerRadius)
          innerRadius = magnitude1;
        if ((double) magnitude2 < (double) innerRadius)
          innerRadius = magnitude2;
        if ((double) magnitude2 > (double) outerRadius)
          outerRadius = magnitude2;
      }
    }

    public Polygon2D()
      : this(Polygon2D.FromNumberofSidesAndRadius(3, 1f))
    {
    }

    public Polygon2D(Vector2D[] baseVertices)
      : this(ALVector2D.Zero, baseVertices)
    {
    }

    public Polygon2D(ALVector2D Position, Vector2D[] baseVertices)
    {
      this.Set(Position, baseVertices);
    }

    private void Set(ALVector2D Position, Vector2D[] baseVertices)
    {
      Polygon2D.CalcRadius(baseVertices, out this.innerRadius, out this.boundingRadius);
      this.position = Position;
      this.length = baseVertices.Length;
      this.vertices = new Vertex2D[this.length];
      this.edges = new Edge2D[this.length];
      Matrix2D matrix2D = this.position.ToMatrix2D();
      for (int index = 0; index < this.length; ++index)
        this.vertices[index] = new Vertex2D(baseVertices[index], matrix2D);
      for (int index1 = 0; index1 < this.length; ++index1)
      {
        int index2 = (index1 + 1) % this.length;
        this.edges[index1] = new Edge2D(this.vertices[index1], this.vertices[index2], matrix2D);
      }
      this.isBoxlike = this.length == 4 && (double) (this.edges[0].Normal * this.edges[1].Normal) == 0.0;
    }

    public Polygon2D(Polygon2D copy)
    {
      this.boundingBox2D = copy.boundingBox2D;
      this.boundingRadius = copy.boundingRadius;
      this.innerRadius = copy.innerRadius;
      this.position = copy.position;
      this.length = copy.length;
      this.vertices = new Vertex2D[this.length];
      this.edges = new Edge2D[this.length];
      for (int index = 0; index < this.length; ++index)
        this.vertices[index] = new Vertex2D(copy.vertices[index]);
      for (int index1 = 0; index1 < this.length; ++index1)
      {
        int index2 = (index1 + 1) % this.length;
        this.edges[index1] = new Edge2D(copy.edges[index1], this.vertices[index1], this.vertices[index2]);
      }
      this.isBoxlike = copy.isBoxlike;
    }

    [Browsable(false)]
    [XmlIgnore]
    public Edge2D[] Edges
    {
      get
      {
        return this.edges;
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public Vertex2D[] Vertices
    {
      get
      {
        return this.vertices;
      }
    }

    public ALVector2D Position
    {
      get
      {
        return this.position;
      }
      set
      {
        this.SetPosition(value, value.ToMatrix2D());
      }
    }

    public Vector2D[] BaseVertices
    {
      get
      {
        return Vertex2D.OriginalPositionToVector2DArray(this.vertices);
      }
      set
      {
        this.Set(this.position, value);
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public BoundingBox2D BoundingBox2D
    {
      get
      {
        if (this.boundingBox2D == null)
          this.CalcBoundingBox2D();
        return this.boundingBox2D;
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public float BoundingRadius
    {
      get
      {
        return this.boundingRadius;
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public float InnerRadius
    {
      get
      {
        return this.innerRadius;
      }
    }

    [Browsable(false)]
    [XmlIgnore]
    public bool IsBoxlike
    {
      get
      {
        return this.isBoxlike;
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public float Area
    {
      get
      {
        return Polygon2D.CalcArea(Vertex2D.OriginalPositionToVector2DArray(this.vertices));
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public float Perimeter
    {
      get
      {
        return Polygon2D.CalcPerimeter(Vertex2D.OriginalPositionToVector2DArray(this.vertices));
      }
    }

    [Browsable(false)]
    [XmlIgnore]
    public Vector2D Centroid
    {
      get
      {
        return Polygon2D.CalcCentroid(Vertex2D.OriginalPositionToVector2DArray(this.vertices));
      }
    }

    public void CalcBoundingBox2D()
    {
      float x = this.position.Linear.X;
      float y = this.position.Linear.Y;
      float lowerX = x;
      float lowerY = y;
      for (int index = 0; index < this.length; ++index)
      {
        Vector2D position = this.vertices[index].Position;
        if ((double) x < (double) position.X)
          x = position.X;
        else if ((double) lowerX > (double) position.X)
          lowerX = position.X;
        if ((double) y < (double) position.Y)
          y = position.Y;
        else if ((double) lowerY > (double) position.Y)
          lowerY = position.Y;
      }
      this.boundingBox2D = new BoundingBox2D(x, y, lowerX, lowerY);
    }

    public void Set(Polygon2D copy)
    {
      this.boundingBox2D = copy.boundingBox2D;
      this.position = copy.position;
      for (int index = 0; index < this.length; ++index)
      {
        this.vertices[index].Set(copy.vertices[index]);
        this.edges[index].Set(copy.edges[index]);
      }
    }

    public void SetPosition(ALVector2D position, Matrix2D matrix)
    {
      this.position = position;
      for (int index = 0; index < this.length; ++index)
        this.vertices[index].Transform(matrix.VertexMatrix);
      for (int index = 0; index < this.length; ++index)
        this.edges[index].Transform(matrix);
    }

    public Polygon2DDistanceInfo CalcDistanceInfo(Vector2D pointRelativeToWorld)
    {
      Edge2D ClosestEdge = (Edge2D) null;
      Edge2DDistanceInfo edge2DdistanceInfo1 = (Edge2DDistanceInfo) null;
      bool InsideEdgesChecked = true;
      bool flag = true;
      for (int index = 0; index < this.length; ++index)
      {
        Edge2D edge = this.edges[index];
        Edge2DDistanceInfo edge2DdistanceInfo2 = edge.CalcDistanceInfo(pointRelativeToWorld);
        InsideEdgesChecked = InsideEdgesChecked && edge2DdistanceInfo2.BehindEdge2D;
        if (!flag)
        {
          if (!edge2DdistanceInfo2.InEdgesVoronoiRegion ? (double) edge2DdistanceInfo2.DistanceProjOnNormal > (double) edge2DdistanceInfo1.DistanceProjOnNormal : (double) edge2DdistanceInfo2.DistanceProjOnNormal >= (double) edge2DdistanceInfo1.DistanceProjOnNormal)
          {
            ClosestEdge = edge;
            edge2DdistanceInfo1 = edge2DdistanceInfo2;
          }
        }
        else
        {
          ClosestEdge = edge;
          edge2DdistanceInfo1 = edge2DdistanceInfo2;
        }
        flag = false;
      }
      Edge2DNormalInfo NormalInfo = ClosestEdge.CalcNormalInfo(edge2DdistanceInfo1, false);
      return new Polygon2DDistanceInfo(ref pointRelativeToWorld, ClosestEdge, edge2DdistanceInfo1, NormalInfo, ref InsideEdgesChecked);
    }

    public Polygon2DDistanceInfo CalcDistanceInfo(
      Vector2D pointRelativeToWorld,
      bool[] goodedges)
    {
      Edge2D ClosestEdge = (Edge2D) null;
      Edge2DDistanceInfo edge2DdistanceInfo1 = (Edge2DDistanceInfo) null;
      bool InsideEdgesChecked = true;
      bool flag1 = true;
      bool flag2 = true;
      for (int index = 0; index < this.length; ++index)
      {
        if (goodedges[index])
        {
          Edge2D edge = this.edges[index];
          Edge2DDistanceInfo edge2DdistanceInfo2 = edge.CalcDistanceInfo(pointRelativeToWorld);
          InsideEdgesChecked = InsideEdgesChecked && edge2DdistanceInfo2.BehindEdge2D;
          if (!flag1)
          {
            if (!edge2DdistanceInfo2.InEdgesVoronoiRegion ? (double) edge2DdistanceInfo2.DistanceProjOnNormal > (double) edge2DdistanceInfo1.DistanceProjOnNormal : (double) edge2DdistanceInfo2.DistanceProjOnNormal >= (double) edge2DdistanceInfo1.DistanceProjOnNormal)
            {
              ClosestEdge = edge;
              edge2DdistanceInfo1 = edge2DdistanceInfo2;
            }
          }
          else
          {
            ClosestEdge = edge;
            edge2DdistanceInfo1 = edge2DdistanceInfo2;
            flag2 = false;
          }
          flag1 = false;
        }
      }
      if (flag2)
        return (Polygon2DDistanceInfo) null;
      Edge2DNormalInfo NormalInfo = ClosestEdge.CalcNormalInfo(edge2DdistanceInfo1, false);
      return new Polygon2DDistanceInfo(ref pointRelativeToWorld, ClosestEdge, edge2DdistanceInfo1, NormalInfo, ref InsideEdgesChecked);
    }

    public bool CrossingNumberPointInsideTest(Vector2D pointRelativeToWorld)
    {
      int num1 = 0;
      Vector2D position1 = this.edges[0].FirstVertex.Position;
      bool flag1 = (double) position1.Y <= (double) pointRelativeToWorld.Y;
      bool flag2 = (double) position1.Y > (double) pointRelativeToWorld.Y;
      for (int index = 0; index != this.length; ++index)
      {
        Edge2D edge = this.edges[index];
        Vector2D position2 = edge.SecondVertex.Position;
        bool flag3 = (double) position2.Y > (double) pointRelativeToWorld.Y;
        bool flag4 = (double) position2.Y <= (double) pointRelativeToWorld.Y;
        bool flag5 = flag1 && flag3;
        bool flag6 = flag2 && flag4;
        if (flag5 || flag6)
        {
          float num2 = (position2.Y - pointRelativeToWorld.Y) / edge.Edge.Y;
          float num3 = position2.X - num2 * edge.Edge.X;
          if ((double) pointRelativeToWorld.X < (double) num3)
            ++num1;
        }
        flag1 = flag4;
        flag2 = flag3;
      }
      return (num1 & 1) == 1;
    }

    public bool TestIntersection(Vector2D point)
    {
      return this.CrossingNumberPointInsideTest(point);
    }

    public void Shift(Vector2D offset)
    {
      Vector2D[] vector2Darray = Vertex2D.OriginalPositionToVector2DArray(this.vertices);
      Matrix2D matrix2D = this.Position.ToMatrix2D();
      OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(ref offset, vector2Darray, vector2Darray, new RefOperation<Vector2D, Vector2D, Vector2D>(Vector2D.Add));
      for (int index = 0; index < this.length; ++index)
        this.vertices[index].OriginalPosition = vector2Darray[index];
      this.boundingRadius = Polygon2D.CalcBoundingRadius(vector2Darray);
      offset = matrix2D.NormalMatrix * offset;
      ALVector2D position = new ALVector2D(this.Position.Angular, this.Position.Linear - offset);
      this.SetPosition(position, position.ToMatrix2D());
    }

    public void OnDeserialization(object sender)
    {
      this.Set(this.position, Vertex2D.OriginalPositionToVector2DArray(this.vertices));
    }
  }
}
