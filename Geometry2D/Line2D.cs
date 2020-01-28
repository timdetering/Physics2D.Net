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
namespace AdvanceMath.Geometry2D
{
    /// <summary>
    /// (NOT USED BY PHYSICS2D)
    /// </summary>
    public enum LineSide : int
    {
        Positive = 1,
        Negitive = -1,
        Neither = 0,
    }
    /// <summary>
    /// (NOT USED BY PHYSICS2D)
    /// </summary>
    [Serializable]
	public class Line2D
	{
		public static Line2D From2Points(Vector2D first,Vector2D second)
		{
			Vector2D edge = first - second;
			Scalar Magnitude = edge.Magnitude;
            if (Magnitude > 0)
            {
                Line2D returnvalue = new Line2D();
                returnvalue.normal = (1 / Magnitude) ^ edge;
                returnvalue.nDistance = returnvalue.Normal * first;
                return returnvalue;
            }
            return null;
		}
        public static Scalar CalcDistance(Line2D line,Vector2D point)
        {
            return point * line.normal + line.nDistance;
        }
        public static LineSide CalcLineSide(Line2D line, Vector2D point)
        {
            return (LineSide)Math.Sign(CalcDistance(line, point));
        }
		protected Vector2D normal;
        protected Scalar nDistance;
		protected Line2D(){}
		public Line2D(Vector2D normal,Scalar c)
		{
			this.normal = normal;
			this.nDistance = c;
		}
		public Scalar NDistance
		{
			get
			{
				return nDistance;
			}
		}
		public Vector2D Normal
		{
			get
			{
				return normal;
			}
		}
    }
}
