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
using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath;

namespace Physics2D
{
    /// <summary>
    /// Describes the methods and properties required for the object to be IsCollidable.
    /// </summary>
    public interface ICollidableBody : IMassive, IBaseCollidable
    {
        #region properties
        /// <summary>
        /// Gets the ICollidableBodyParts that describe all the Polygons that make up the rigid collidable.
        /// </summary>
        ICollidableBodyPart[] CollidableParts { get;}
        /// <summary>
        /// Gets the BoundingBox2D calculated by the call merging the the current and initials bounding boxes.
        /// </summary>
        BoundingBox2D SweepBoundingBox2D { get;}
        /// <summary>
        /// Gets the CollisionState that describes the current collisions the object is in.
        /// </summary>
        CollisionState CollisionState { get;}
        #endregion
        #region methods
        /// <summary>
        /// Calculates the Value used in LastImpulse Calculations.
        /// </summary>
        /// <param name="pointRelativeToBody">The location where to calculate K at.</param>
        /// <param name="normal">The normal of the collision.</param>
        /// <returns>The K value.</returns>
        float GetK(Vector2D pointRelativeToBody,Vector2D normal);
        /// <summary>
        /// Changes the current Velocity of the object by applying impulse.
        /// </summary>
        /// <param name="pointRelativeToBody">where to apply impulse at.</param>
        /// <param name="LastImpulse">the impulse being applied.</param>
        void ApplyImpulse(Vector2D pointRelativeToBody,Vector2D Impulse);
        /// <summary>
        /// Gets the BoundingRadius of the Entire ICollidableBody.
        /// </summary>
        float BoundingRadius { get;}
        /// <summary>
        /// Converts the ICollidableBody to a Circle2D that can contain it.
        /// </summary>
        /// <returns>A Circle2D that can contain the ICollidableBody.</returns>
        Circle2D ToCircle2D();
        /// <summary>
        /// Sets The Initial and Good Position to the Current.
        /// </summary>
        void SetAllPositions();
        /// <summary>
        /// Sets The Initial and Good Position to the value Passed.
        /// </summary>
        /// <param name="position">The Position to set all values.</param>
        void SetAllPositions(ALVector2D position);
        #endregion
    }
}
