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
using System;
using System.Runtime.Serialization;
using AdvanceMath;
using AdvanceMath.Geometry2D;

namespace Physics2D
{
	/// <summary>
	/// This class Stores mass information and Moment of Inertia Together since they are very closly related.
	/// </summary>
    [Serializable]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public sealed class MassInertia: IDeserializationCallback 
    {
        #region static methods
        /// <remarks>Warning Very Slow!</remarks>
        public static float CalcMomentofInertia(IGeometry2D[] geometries, float incriment)
        {
            int shapecount = geometries.Length;
            geometries[0].CalcBoundingBox2D();
            BoundingBox2D boundingBox2D = geometries[0].BoundingBox2D;
            for (int pos = 1; pos != shapecount; ++pos)
            {
                geometries[pos].CalcBoundingBox2D();
                boundingBox2D = BoundingBox2D.From2BoundingBox2Ds(boundingBox2D, geometries[pos].BoundingBox2D);
            }
            float returnvalue = 0;
            float mass = 0;
            Vector2D point = new Vector2D();
            for (point.X = boundingBox2D.Lower.X; point.X <= boundingBox2D.Upper.X; point.X += incriment)
            {
                for (point.Y = boundingBox2D.Lower.Y; point.Y <= boundingBox2D.Upper.Y; point.Y += incriment)
                {
                    for (int pos = 0; pos != shapecount; ++pos)
                    {
                        if (geometries[pos].TestIntersection(point))
                        {
                            returnvalue += point.MagnitudeSq * incriment;
                            mass += incriment;
                        }
                    }
                }
            }
            return returnvalue / mass;
        }
        /// <remarks>Warning Very Slow!</remarks>
        public static MassInertia FromIGeometryArray(float mass, IGeometry2D[] geometries, float incriment)
        {
            return new MassInertia(mass, mass * CalcMomentofInertia(geometries, incriment));//,Vector2D.Zero);
        }
        public static MassInertia FromCylindricalShell(float mass, float radius)
		{
            return new MassInertia(mass, mass * radius * radius);//,Vector2D.Zero);
		}
		public static MassInertia FromHallowCylinder(float mass,float innerRadius,float outerRadius)
		{
            return new MassInertia(mass, (float).5 * mass * (innerRadius * innerRadius + outerRadius * outerRadius));//,Vector2D.Zero);
		}
		public static MassInertia FromSolidCylinder(float mass,float radius)
		{
            return new MassInertia(mass, (float).5 * mass * (radius * radius));//,Vector2D.Zero);
		}
		public static MassInertia FromRectangle(float mass,float length,float width)
		{
            return new MassInertia(mass, (1f / 12f) * mass * (length * length + width * width));//,Vector2D.Zero);
		}
		public static MassInertia FromSquare(float mass,float sideLength)
		{
            return new MassInertia(mass, (1f / 6f) * mass * (sideLength * sideLength));//,Vector2D.Zero);
		}
		public static MassInertia FromRodsCenter(float mass,float length)
		{
            return new MassInertia(mass, (1f / 12f) * mass * (length * length));//,Vector2D.Zero);
		}
		/*public static MassInertia FromRodsEnd(float mass,float length,float radianAngle)
		{
			return new MassInertia(mass,(1f/3f)*mass*(length*length));//,Vector2D.FromLengthAndAngle(length/2,radianAngle));
        }*/
        #endregion
        #region fields
        private float mass;
		private float momentofInertia;
        [NonSerialized]
		private float massInv;
        [NonSerialized]
        private float momentofInertiaInv;
        [NonSerialized]
        private float accelerationDueToGravity;
        #endregion
        #region constructors
        public MassInertia(){}
        public MassInertia(float mass, float momentofInertia)
		{
			this.Mass = mass;
			this.MomentofInertia = momentofInertia;
        }
        #endregion
        #region properties
        public float Mass
		{
			get
			{
				return mass;
			}
            set
            {
                this.mass = value;
                this.massInv = 1 / value;
                this.accelerationDueToGravity = value * Physics.GravitationalConstant;
            }
		}

        public float MomentofInertia
        {
            get
            {
                return momentofInertia;
            }
            set
            {
                this.momentofInertia = value;
                this.momentofInertiaInv = 1 / value;
            }
        }
        [System.ComponentModel.Browsable(false)]
        public float MassInv
		{
			get
			{
				return massInv;
			}
		}
        [System.ComponentModel.Browsable(false)]
		public float MomentofInertiaInv
		{
			get
			{
				return momentofInertiaInv;
			}
		}
        [System.ComponentModel.Browsable(false)]
        public float AccelerationDueToGravity
		{
			get
			{
				return accelerationDueToGravity;
			}
        }
        #endregion
        #region IDeserializationCallback Members
        public void OnDeserialization(object sender)
        {
            this.massInv = 1 / this.mass;
            this.momentofInertiaInv = 1 / this.momentofInertia;
            this.accelerationDueToGravity = this.mass * Physics.GravitationalConstant;
        }
        #endregion
    }
}
