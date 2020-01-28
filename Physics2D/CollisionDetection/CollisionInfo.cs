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
using AdvanceMath.Geometry2D;using AdvanceMath;


namespace Physics2D.CollisionDetection 
{
    [Serializable]
	public enum CollisionType
	{
		PolygonPolygon,
		PolygonCircle,
		CircleCircle,
	}
	/// <summary>
	/// Used to Describe a Collisions location and direction.
	/// </summary>
    [Serializable]
    public class CollisionInfo
	{
		public Vector2D CollisionPoint;
		public Vector2D CollisionNormal;
		public float Distance;
		public CollisionInfo(Vector2D CollisionPoint,Vector2D CollisionNormal,float Distance)
		{
			this.CollisionPoint = CollisionPoint;
			this.CollisionNormal = CollisionNormal;	
			this.Distance = Distance;
		}
	}
}
