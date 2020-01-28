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
using AdvanceMath.Geometry2D;
namespace Physics2D
{
    /// <summary>
    /// This interface describes the methods and properties required for something to be part of a ICollidableBody.
    /// </summary>
    [AdvanceSystem.ComponentModel.EditorXmlInclude(typeof(Physics2D.CollidableBodies.RigidBodyPart))]

    public interface ICollidableBodyPart : IHasBoundingBox2D, ICloneable
    {
        /// <summary>
        /// Gets the Coefficients that describe the surface of this part.
        /// </summary>
        Coefficients Coefficients{ get;}
        /// <summary>
        /// Gets the offset that descibes where this is fromt he center of the ICollidableBody.
        /// </summary>
        ALVector2D Offset { get;}
        Matrix2D Matrix { get;}
        BoundingBox2D SweepBoundingBox2D { get;}
        /// <summary>
        /// Gets the current Polygon2D for this part of the ICollidableBody.
        /// </summary>
        Polygon2D Polygon2D { get;}
        /// <summary>
        /// Gets the BaseGeometry2D for this part of the ICollidableBody.
        /// </summary>
        IGeometry2D BaseGeometry { get;set;}
        /// <summary>
        /// Gets the current position relative to the world of this ICollidableBodyPart. 
        /// </summary>
        ALVector2D Position { get;}
        /// <summary>
        /// Gets the Initial position relative to the world of this ICollidableBodyPart. 
        /// </summary>
        ALVector2D InitialPosition { get;}
        /// <summary>
        /// Gets the Initial Polygon2D for this part of the ICollidableBody.
        /// </summary>
        Polygon2D InitialPolygon2D { get;}
        /// <summary>
        /// Gets the position that is to be used to display this part relative to the world of this ICollidableBodyPart. 
        /// </summary>
        ALVector2D GoodPosition { get;}
        /// <summary>
        /// Gets the GoodPolygon2D that is to be used to display this part relative to the world of this ICollidableBodyPart. 
        /// </summary>
        Polygon2D GoodPolygon2D { get;}
        /// <summary>
        /// Gets the Vertices for Display by a graphics engine.
        /// </summary>
        Vector2D[] DisplayVertices { get;}

        bool UseCircleCollision { get;}
        /// <summary>
        /// Saves the current info to the Initial info.
        /// </summary> 
        void Save();
        /// <summary>
        /// loads the current info from the Initial info.
        /// </summary>
        void Load();
        /// <summary>
        /// Saves the current info to the Good info.
        /// </summary>
        void SaveGood();
        /// <summary>
        /// Sets the Position of the Part.
        /// </summary>
        /// <param name="Position">The position to set it to.</param>
        /// <param name="matrix">The pre-calculated Matrix2D</param>
        void SetPosition(ALVector2D Position, Matrix2D matrix);
        /// <summary>
        /// Sets the Position of the Part.
        /// </summary>
        /// <param name="Position">The position to set it to.</param>
        void SetPosition(ALVector2D Position);
    }
}
