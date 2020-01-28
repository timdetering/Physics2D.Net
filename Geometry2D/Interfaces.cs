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
    /// Describes that memers a class needs to be considered to have a BoundingBox2D
    /// </summary>
    public interface IHasBoundingBox2D
    {
        /// <summary>
        /// Tells the object to calculate the BoundingBox2D that contains the object.
        /// </summary>
        void CalcBoundingBox2D();
        /// <summary>
        /// Gets the BoundingBox2D calculated by the call to <see cref="CalcBoundingBox2D"/>
        /// </summary>
        BoundingBox2D BoundingBox2D { get;}
    }
    [AdvanceSystem.ComponentModel.EditorXmlInclude(typeof(Polygon2D))]
    [AdvanceSystem.ComponentModel.EditorXmlInclude(typeof(Circle2D))]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public interface IGeometry2D : IHasBoundingBox2D
    {
       
        bool TestIntersection(Vector2D point);
        /// <summary>
        /// Gets the area of the shape.
        /// </summary>
        Scalar Area { get;}
        /// <summary>
        /// Gets the Perimeter of the shape.
        /// </summary>
        Scalar Perimeter { get;}
        /// <summary>
        /// Gets the Centroid of a shape.
        /// </summary>
        /// <remarks>
        /// This is Also known as Center of Gravity/Mass.
        /// </remarks>
        Vector2D Centroid { get;}
        /// <summary>
        /// gets the center of the shape.
        /// </summary>
        ALVector2D Position { get;set;}
        /// <summary>
        /// Shifts the verticies that make the shape.
        /// </summary>
        /// <param name="offset"></param>
        void Shift(Vector2D offset);

        Scalar BoundingRadius { get;}
        Scalar InnerRadius { get;}
    }
}
