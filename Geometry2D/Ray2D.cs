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
    [Serializable]
    public class Ray2D
	{
        protected Vector2D origin;
		protected Vector2D direction;
		public Ray2D(Vector2D origin,Vector2D direction)
		{
			this.origin = origin;
			this.direction = direction;
		}
		public Vector2D Origin
		{
			get
			{
				return origin;
			}
			set
			{
				origin = value;
			}
		}
		public Vector2D Direction
		{
			get
			{
				return direction;
			}
			set
			{
				direction = value;
			}
		}
		public Vector2D GetPoint(Ray2DIntersectInfo info)
		{
			return direction*info.DistanceFromOrigin+origin;
		}
	}
}
