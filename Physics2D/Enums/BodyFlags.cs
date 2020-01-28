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
namespace Physics2D
{
    /// <summary>
    /// Flags that effect how an object is treated.
    /// </summary>
    [Serializable, Flags]
    public enum BodyFlags : int
    {
        None = 0,
        /// <summary>
        /// Its treated as if it has infinite mass. so it cant collide with other objects which have this flag
        /// </summary>
        InfiniteMass = 1 << 0,
        /// <summary>
        /// It ignores Gravity so gravity will not cuase it to be accelerated .
        /// </summary>
        IgnoreGravity = 1 << 1,
        /// <summary>
        /// It is a gravity well. this helps the stacking algorithm to find the floor.
        /// </summary>
        GravityWell = 1 << 2,
        /// <summary>
        /// The object is not updated. fo some reason.
        /// </summary>
        NoUpdate = 1 << 3,
        /// <summary>
        /// Does not react in the collision stage.
        /// </summary>
        NoCollision = 1 << 4,
        /// <summary>
        /// Does not react in the Contact  stage.
        /// </summary>
        NoContact = 1 << 5,
        /// <summary>
        /// Does not react to impulse being applied. so very close to InfiniteMass
        /// </summary>
        NoImpulse = 1 << 6,

        ImmovableMask = InfiniteMass | NoImpulse | NoUpdate,
    }
}