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
using AdvanceMath;

namespace Physics2D
{


	/// <summary>
	/// Describes the method need for an object to be a gravity source.
	/// </summary>
    public interface IGravitySource : IExpireable
	{
		/// <summary>
		/// Gets the Vector2D that describes the Strenght and direction of the pull of gravity at the point provided.
		/// </summary>
		/// <param name="pointRelativeToWorld">the point where to get the gravity at.</param>
		/// <returns>Vector2D that describes the Strenght and direction of the pull of gravity.</returns>
		Vector2D GetGravityPullAt(Vector2D pointRelativeToWorld);
	}
}
