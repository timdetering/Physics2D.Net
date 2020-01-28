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
using System.Runtime.InteropServices;
using AdvanceMath;
namespace AdvanceMath.Geometry2D
{
	/// <summary>
	/// This is the Vertex Class that describes the point where 2 edges of a polygon join.
	/// </summary>
	[StructLayout(LayoutKind.Sequential),Serializable]
	public sealed class Vertex2D
    {
        #region static methods
        public static Vector2D[] PositionToVector2DArray(Vertex2D[] Vertices)
        {
            int lenght = Vertices.Length;
            Vector2D[] returnvalue = new Vector2D[lenght];
            for (int pos = 0; pos < lenght; ++pos)
            {
                returnvalue[pos] = Vertices[pos].Position;
            }
            return returnvalue;
        }
        public static Vector2D[] OriginalPositionToVector2DArray(Vertex2D[] Vertices)
        {
            int lenght = Vertices.Length;
            Vector2D[] returnvalue = new Vector2D[lenght];
            for (int pos = 0; pos < lenght; ++pos)
            {
                returnvalue[pos] = Vertices[pos].original;
            }
            return returnvalue;
        }
        #endregion
        #region fields
        /// <summary>
        /// The Position Vector2D.
        /// </summary>
        private Vector2D position;
		/// <summary>
        /// This is the originalPosition of the Vertex.
		/// </summary>
        private Vector2D original;
        #endregion
        #region constructors
        public Vertex2D(Vector2D position)
        {
            this.original = position;
            this.position = position;
        }
        /// <summary>
		/// This Creates a new Vertex2D instance on the Heap.
		/// </summary>
		/// <param name="center">The Center of the BasePolygon this Vertex2D belongs to.</param>
		/// <param name="relativeToCenter">The Relative location of this Vertex2D to the center.</param>
        public Vertex2D(Vector2D position,Matrix2D matrix)
        {
            this.original = position;
            this.position = matrix.VertexMatrix * position;
        }
		/// <summary>
		/// This is a Copy Constructor.
		/// </summary>
		/// <param name="copy">The Vertex2D to be Copied.</param>
		public Vertex2D(Vertex2D copy)
		{
			this.position = copy.position;
			this.original = copy.original;
		}
        #endregion
        #region properties
        /// <summary>
        /// Gets the OriginalPosition of the Vertex.
        /// </summary>
        public Vector2D OriginalPosition
        {
            get { return original; }
            internal set
            {
                if (value != original)
                {
                    position -= original;
                    original = value;
                    position += original;
                }
            }
        }
        /// <summary>
        /// Gets the position of the Vertex in relative to the world's origin.
        /// </summary>
        public Vector2D Position
        {
            get
            {
                return position;
            }
        }
        #endregion
        #region methods
        /// <summary>
        /// Sets the values in this vertex to that of anothers.
        /// </summary>
        /// <param name="copy">the Vertex2D containing the new values.</param>
        internal void Set(Vertex2D copy)
		{
            this.position = copy.position;
		}
        /// <summary>
        /// Transforms the vertex.
        /// </summary>
        /// <param name="matrix"></param>
        public void Transform(Matrix3x3 matrix)
        {
            position = matrix * original;
        }
        #endregion
    }
}
