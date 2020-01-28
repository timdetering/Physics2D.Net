#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using AdvanceMath;
using System.ComponentModel;
using System.Xml.Serialization;
namespace AdvanceMath.Geometry2D
{
    /// <summary>
    /// Represents a Circle in 2D.
    /// </summary>
    [Serializable]
    //[TypeConverter(typeof(AdvanceSystem.ComponentModel.Circle2DConverter))]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public class Circle2D : IGeometry2D
    {
        #region static methods
        [AdvanceSystem.ComponentModel.UTCParser]
        public static Circle2D Parse(string text)
        {
            ALVector2D a = ALVector2D.Parse(text);
            return new Circle2D(a.Angular, a.Linear);
        }
        public static bool TestIntersection(Circle2D first, Circle2D second)
        {
            return (first.center - second.center).Magnitude >= (first.radius + second.radius);
        }
        public static bool TestIntersection(Circle2D circle, Vector2D pointRelativeToWorld)
        {
            return (circle.center - pointRelativeToWorld).Magnitude >= circle.radius;
        } 
        #endregion
        #region feilds
        Vector2D center;
        Scalar radius;
        BoundingBox2D boundingBox2D; 
        #endregion
        #region constructors
        public Circle2D():this(1,Vector2D.Zero){}
        public Circle2D(Scalar radius):this(radius,Vector2D.Zero){}
        [AdvanceSystem.ComponentModel.UTCConstructor]
        public Circle2D(Scalar radius, Vector2D center)
        {
            this.center = center;
            this.radius = radius;
        } 
        #endregion
        #region properties
        public Scalar Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
            }
        }
        public Vector2D Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
            }
        }
        [Browsable(false), XmlIgnore()]
        public ALVector2D Position
        {
            get
            {
                return new ALVector2D(0, center);
            }
            set
            {
                center = value.Linear;
            }
        }
        [Browsable(false), XmlIgnore()]
        public Scalar BoundingRadius
        {
            get
            {
                return radius;
            }
        }
        [Browsable(false), XmlIgnore()]
        public BoundingBox2D BoundingBox2D
        {
            get
            {
                return boundingBox2D;
            }
        }
        [Browsable(false), XmlIgnore()]
        public Scalar Area
        {
            get
            {
                return (Scalar)(Math.PI * radius * radius);
            }
        }
        [Browsable(false), XmlIgnore()]
        public Vector2D Centroid
        {
            get
            {
                return Vector2D.Zero;
            }
        }
        [Browsable(false), XmlIgnore()]
        public Scalar Perimeter
        {
            get
            {
                return (Scalar)(2 * Math.PI * radius);
            }
        }
        [Browsable(false), XmlIgnore()]
        public float InnerRadius
        {
            get { return radius; }
        }
        #endregion
        #region methods
        public void Shift(Vector2D offset)
        {
            if (offset != Vector2D.Zero)
            {
                throw new Exception("Cannot Shift a Circle.");
            }
        }
        public void CalcBoundingBox2D()
        {
            boundingBox2D = new BoundingBox2D(center + radius * Vector2D.XYAxis, center - radius * Vector2D.XYAxis); ;
        }
        public bool TestIntersection(Vector2D point)
        {
            return (center - point).Magnitude >= radius;
        }
        [AdvanceSystem.ComponentModel.UTCFormater]
        public override string ToString()
        {
            return string.Format("({0},{1})",radius,center);
        }
        #endregion
    }
}
