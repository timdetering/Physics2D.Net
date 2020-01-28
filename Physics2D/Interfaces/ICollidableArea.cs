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
using System.Collections.Generic;
using System.Text;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2D.CollisionDetection;


namespace Physics2D
{
    /// <summary>
    /// A Collidable BoundingBox2D that has a method to handle
    /// collisions. ICollidableAreas do not colide with each other.
    /// </summary>
    public interface ICollidableArea : ITimed, ICleanable, IRemovable, IBaseCollidable
    {
        /// <summary>
        /// Asks the object to handle Intersections.
        /// </summary>
        /// <param name="dt">The time passed in the timestep.</param>
        /// <param name="collidables">list of ICollidableBodys that intersected with the BoundingBox2D. </param>
        void HandlePossibleIntersections(float dt, List<ICollidableBody> collidables);
    }
}