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
using System.Collections.Generic;
using AdvanceMath;
namespace AdvanceMath.Geometry2D
{
	/// <summary>
	/// This class is used to descibe a the smallest box that can contain a Polygon2D object.
	/// </summary>
    [Serializable]
    public sealed class BoundingBox2D
	{
		/// <summary>
		/// Creates a new BoundingBox2D Instance from 2 Vector2Ds.
		/// </summary>
		/// <param name="first">the first Vector2D.</param>
		/// <param name="second">the second Vector2D.</param>
		/// <returns>a new BoundingBox2D</returns>
		/// <remarks>The Upper and Lower values are automatically determined.</remarks>
		public static BoundingBox2D From2Vectors(Vector2D first, Vector2D second)
		{
            Vector2D Upper = second;
            Vector2D Lower = first;
			if(first.X>second.X)
			{
				Upper.X = first.X;
				Lower.X = second.X;
			}
			if(first.Y>second.Y)
			{
				Upper.Y = first.Y;
				Lower.Y = second.Y;
			}
			return new BoundingBox2D(Upper,Lower);
		}
        /// <summary>
        /// Creates a new BoundingBox2D Instance from multiple Vector2Ds.
        /// </summary>
        /// <param name="vectors">the list of vectors</param>
        /// <returns>a new BoundingBox2D</returns>
        /// <remarks>The Upper and Lower values are automatically determined.</remarks>
        public static BoundingBox2D FromVectors(Vector2D[] vectors)
        {
            int length = vectors.Length;
            Vector2D Upper = vectors[0];
            Vector2D Lower = vectors[0];
            for (int pos = 1; pos < length; ++pos)
            {
                if (vectors[pos].X > Upper.X)
                {
                    Upper.X = vectors[pos].X;
                }
                else if (vectors[pos].X < Lower.X)
                {
                    Lower.X = vectors[pos].X;
                }
                if (vectors[pos].Y > Upper.Y)
                {
                    Upper.Y = vectors[pos].Y;
                }
                else if (vectors[pos].Y < Lower.Y)
                {
                    Lower.Y = vectors[pos].Y;
                }
            }
            return new BoundingBox2D(Upper, Lower);
        }
        /// <summary>
        /// Creates a new BoundingBox2D Instance from multiple Vector2Ds.
        /// </summary>
        /// <param name="vectors">the list of vectors</param>
        /// <returns>a new BoundingBox2D</returns>
        /// <remarks>The Upper and Lower values are automatically determined.</remarks>
        public static BoundingBox2D FromVectors(IList<Vector2D> vectors)
        {
            int length = vectors.Count;
            Vector2D Upper = vectors[0];
            Vector2D Lower = vectors[0];
            for (int pos = 1; pos < length; ++pos)
            {
                if (vectors[pos].X > Upper.X)
                {
                    Upper.X = vectors[pos].X;
                }
                else if (vectors[pos].X < Lower.X)
                {
                    Lower.X = vectors[pos].X;
                }
                if (vectors[pos].Y > Upper.Y)
                {
                    Upper.Y = vectors[pos].Y;
                }
                else if (vectors[pos].Y < Lower.Y)
                {
                    Lower.Y = vectors[pos].Y;
                }
            }
            return new BoundingBox2D(Upper, Lower);
        }
        /// <summary>
        /// Makes a BoundingBox2D that can contain the 2 BoundingBox2Ds passed.
        /// </summary>
        /// <param name="first">The First BoundingBox2D.</param>
        /// <param name="second">The Second BoundingBox2D.</param>
        /// <returns>The BoundingBox2D that can contain the 2 BoundingBox2Ds passed.</returns>
        public static BoundingBox2D From2BoundingBox2Ds(BoundingBox2D first, BoundingBox2D second)
        {
            Vector2D Upper = Vector2D.Zero;
            Vector2D Lower = Vector2D.Zero;
            Upper.X = Math.Max(first.Upper.X, second.Upper.X);
            Upper.Y = Math.Max(first.Upper.Y, second.Upper.Y);
            Lower.X = Math.Min(first.Lower.X, second.Lower.X);
            Lower.Y = Math.Min(first.Lower.Y, second.Lower.Y);
            return new BoundingBox2D(Upper, Lower);
        }
        /// <summary>
        /// Makes a BoundingBox2D that contains the area both BoundingBox2Ds cover.
        /// </summary>
        /// <param name="first">The First BoundingBox2D.</param>
        /// <param name="second">The Second BoundingBox2D.</param>
        /// <returns>The BoundingBox2D that can contain the 2 BoundingBox2Ds passed.</returns>
        public static BoundingBox2D SmallestFrom2BoundingBox2Ds(BoundingBox2D first, BoundingBox2D second)
        {
            Vector2D Upper = Vector2D.Zero;
            Vector2D Lower = Vector2D.Zero;
            Upper.X = Math.Min(first.Upper.X, second.Upper.X);
            Upper.Y = Math.Min(first.Upper.Y, second.Upper.Y);
            Lower.X = Math.Max(first.Lower.X, second.Lower.X);
            Lower.Y = Math.Max(first.Lower.Y, second.Lower.Y);
            return new BoundingBox2D(Upper, Lower);
        }
        /// <summary>
        /// Tests the Interection between a BoundingBox2D and a point.
        /// </summary>
        /// <param name="box">The BoundingBox2D.</param>
        /// <param name="point">the point.</param>
        /// <returns>true if the point is inside the BoundingBox2D; otherwise false.</returns>
        public static bool TestIntersection(BoundingBox2D box, Vector2D point)
        {
            return box.Upper.X >= point.X && box.Lower.X <= point.X && 
                box.Upper.Y >= point.Y && box.Lower.Y <= point.Y;
        }
        public static bool TestIntersection(BoundingBox2D box, Edge2D edge)
        {
            Vector2D point1 = edge.FirstVertex.Position;
            Vector2D point2 = edge.SecondVertex.Position;
            return !(
                (box.Upper.X < point1.X && box.Upper.X < point2.X)||
                (box.Lower.X > point1.X && box.Lower.X > point2.X)||
                (box.Upper.Y < point1.Y && box.Upper.Y < point2.Y) ||
                (box.Lower.Y > point1.Y && box.Lower.Y > point2.Y)
                );
        }
        /// <summary>
		/// The Upper Bound.
		/// </summary>
		public readonly Vector2D Upper;
		/// <summary>
		/// The Lower Bound.
		/// </summary>
		public readonly Vector2D Lower;
		/// <summary>
		/// Creates a new BoundingBox2D Instance.
		/// </summary>
		/// <param name="upperX">The Upper Bound on the XAxis.</param>
		/// <param name="upperY">The Upper Bound on the YAxis.</param>
		/// <param name="lowerX">The Lower Bound on the XAxis.</param>
		/// <param name="lowerY">The Lower Bound on the YAxis.</param>
		public BoundingBox2D(Scalar upperX,Scalar upperY,Scalar lowerX,Scalar lowerY)
		{
			this.Upper.X = upperX;
			this.Upper.Y = upperY;
			this.Lower.X = lowerX;
			this.Lower.Y = lowerY;
		}
		/// <summary>
		/// Creates a new BoundingBox2D Instance from 2 Vector2Ds.
		/// </summary>
		/// <param name="Upper">The Upper Vector2D.</param>
		/// <param name="Lower">The Lower Vector2D.</param>
		public BoundingBox2D(Vector2D Upper, Vector2D Lower)
		{
			this.Upper = Upper;
			this.Lower = Lower;
		}
        public Vector2D[] Corners
        {
            get
            {
                Vector2D[] returnvalue = new Vector2D[4];
                returnvalue[0] = Upper;
                returnvalue[1] = new Vector2D(Lower.X, Upper.Y);
                returnvalue[2] = Lower;
                returnvalue[3] = new Vector2D(Upper.X, Lower.Y);
                return returnvalue;
            }
        }
		/// <summary>
		/// Determines if this bounding box is in Contact with another.
		/// </summary>
		/// <param name="otherBox">The other BoundingBox2D to check against.</param>
		/// <returns>true if they are in contact; otherwise false.</returns>
		public bool TestIntersection(BoundingBox2D otherBox)
		{
			return !((Lower.X >= otherBox.Upper.X) || (Upper.X <= otherBox.Lower.X) ||
                (otherBox.Lower.Y >= Upper.Y) || (otherBox.Upper.Y <= Lower.Y));
		}
		/// <summary>
		/// Moves a BoundingBox2D Linearly.
		/// </summary>
		/// <param name="changeInPosition">How far the box will be moved.</param>
		/// <returns>The repositioned BoundingBox2D.</returns>
		public BoundingBox2D Move(Vector2D changeInPosition)
		{
			return new BoundingBox2D(Upper + changeInPosition,Lower+ changeInPosition);
		}
	}
}
