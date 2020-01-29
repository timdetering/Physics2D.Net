// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Geometry2D.Circle2D
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AdvanceMath.Geometry2D
{
  [Serializable]
  public class Circle2D : IGeometry2D, IHasBoundingBox2D
  {
    private Vector2D center;
    private float radius;
    private BoundingBox2D boundingBox2D;

    public static Circle2D Parse(string text)
    {
      ALVector2D alVector2D = ALVector2D.Parse(text);
      return new Circle2D(alVector2D.Angular, alVector2D.Linear);
    }

    public static bool TestIntersection(Circle2D first, Circle2D second)
    {
      return (double) (first.center - second.center).Magnitude >= (double) first.radius + (double) second.radius;
    }

    public static bool TestIntersection(Circle2D circle, Vector2D pointRelativeToWorld)
    {
      return (double) (circle.center - pointRelativeToWorld).Magnitude >= (double) circle.radius;
    }

    public Circle2D()
      : this(1f, Vector2D.Zero)
    {
    }

    public Circle2D(float radius)
      : this(radius, Vector2D.Zero)
    {
    }

    public Circle2D(float radius, Vector2D center)
    {
      this.center = center;
      this.radius = radius;
    }

    public float Radius
    {
      get
      {
        return this.radius;
      }
      set
      {
        this.radius = value;
      }
    }

    public Vector2D Center
    {
      get
      {
        return this.center;
      }
      set
      {
        this.center = value;
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public ALVector2D Position
    {
      get
      {
        return new ALVector2D(0.0f, this.center);
      }
      set
      {
        this.center = value.Linear;
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public float BoundingRadius
    {
      get
      {
        return this.radius;
      }
    }

    [Browsable(false)]
    [XmlIgnore]
    public BoundingBox2D BoundingBox2D
    {
      get
      {
        return this.boundingBox2D;
      }
    }

    [Browsable(false)]
    [XmlIgnore]
    public float Area
    {
      get
      {
        return 3.141593f * this.radius * this.radius;
      }
    }

    [Browsable(false)]
    [XmlIgnore]
    public Vector2D Centroid
    {
      get
      {
        return Vector2D.Zero;
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public float Perimeter
    {
      get
      {
        return 6.283185f * this.radius;
      }
    }

    [XmlIgnore]
    [Browsable(false)]
    public float InnerRadius
    {
      get
      {
        return this.radius;
      }
    }

    public void Shift(Vector2D offset)
    {
      if (offset != Vector2D.Zero)
        throw new Exception("Cannot Shift a Circle.");
    }

    public void CalcBoundingBox2D()
    {
      this.boundingBox2D = new BoundingBox2D(this.center + this.radius * Vector2D.XYAxis, this.center - this.radius * Vector2D.XYAxis);
    }

    public bool TestIntersection(Vector2D point)
    {
      return (double) (this.center - point).Magnitude >= (double) this.radius;
    }

    public override string ToString()
    {
      return string.Format("({0},{1})", (object) this.radius, (object) this.center);
    }
  }
}
