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
    public class RaySegment2D : Ray2D
    {
        public static RaySegment2D From2Points(Vector2D start, Vector2D end)
        {
            Vector2D direction = end - start;
            Scalar length = direction.MagnitudeSq;
            if (length > 0)
            {
                length = (Scalar)Math.Sqrt(length);
                direction = direction * (1 / length);
                return new RaySegment2D(start, direction, length);
            }
            return null;
        }
        protected Scalar length;
        public RaySegment2D(Vector2D origin, Vector2D direction, Scalar length):base(origin,direction)
        {
            this.length = length;
        }
        public Scalar Length
        {
            get
            {
                return length;
            }
        }
        public BoundingBox2D BoundingBox2D
        {
            get
            {
                return BoundingBox2D.From2Vectors(origin, origin + direction * length);
            }
        }
    }
}
