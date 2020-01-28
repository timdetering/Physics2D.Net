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
namespace AdvanceMath.Geometry2D
{
	/// <summary>
	/// The purpose of this class is to calculate what vertexes can contact which edges. 
	/// The calculations only take into account the preCalcedRotation of each object. 
	/// </summary>
	/// <remarks>
	///  I’m pretty sure there is some kind of theorem that this class is implementing,
	///  because I know this logic is to easy for some one not to have written about it. 
	///  I would like to rename it after that theorem if some one would find out what’s it called.
	/// </remarks>
    [Serializable]
    public class VertexEdgeContactDecider2D
	{
		bool[][] possibleVertexEdgeCollisionsFor1;
		bool[][] possibleVertexEdgeCollisionsFor2;
		public VertexEdgeContactDecider2D(Polygon2D geometry1,Polygon2D geometry2)
		{
			Edge2D[] edges1 = geometry1.Edges;
			Edge2D[] edges2 = geometry2.Edges;
			int length1 = edges1.Length;
			int length2 = edges2.Length;
			int[,] normalDotNormalizedMatrix = new int[length1,length2];
			Scalar dot;
			


			// Fills the normalDotNormalizedMatrix array with -1, 0 or 1.
			// which tells the relation of the edges to each other.
			for(int pos1 = 0; pos1 != length1;++pos1)
			{
				for(int pos2 = 0; pos2 != length2;++pos2)
				{
					dot = edges1[pos1].Normal*edges2[pos2].NormalizedEdge;
					normalDotNormalizedMatrix[pos1,pos2] = Math.Sign(Math.Round(dot,5));
				}
			}
			possibleVertexEdgeCollisionsFor1 = new bool[length1][]; 
			int firstpos;
			int secondpos;
			int firstvalue;
			int secondvalue;
			// an vertex can only touch an edge only if the edges joined at that vertex point towards and away from the edge or if are in line with it.
			//these nested for loops determine if a vertex can make contact with a edge and fills arrays with the results.
			for(int pos1 = 0; pos1 != length1;++pos1)
			{
				possibleVertexEdgeCollisionsFor1[pos1] = new bool[length2];
				firstpos = pos1;
				if(pos1 == 0)
				{
					secondpos = length1 -1;
				}
				else
				{
					secondpos = pos1 - 1;
				}
				for(int pos2 = 0; pos2 != length2;++pos2)
				{
					secondvalue = normalDotNormalizedMatrix[firstpos,pos2];
					firstvalue = normalDotNormalizedMatrix[secondpos,pos2];
					possibleVertexEdgeCollisionsFor1[pos1][pos2] = ((firstvalue==-1)||(firstvalue==0))&&((secondvalue==1)||(secondvalue==0));
				}
			}
			possibleVertexEdgeCollisionsFor2 = new bool[length2][]; 
			for(int pos2 = 0; pos2 != length2;++pos2)
			{
				possibleVertexEdgeCollisionsFor2[pos2] = new bool[length1];
				firstpos = pos2;
				if(pos2 == 0)
				{
					secondpos = length2 - 1;
				}
				else
				{
					secondpos = pos2 - 1;
				}
				for(int pos1 = 0; pos1 != length1;++pos1)
				{
					firstvalue =  normalDotNormalizedMatrix[pos1,firstpos];
					secondvalue = normalDotNormalizedMatrix[pos1,secondpos];
					possibleVertexEdgeCollisionsFor2[pos2][pos1] = ((firstvalue==-1)||(firstvalue==0))&&((secondvalue==1)||(secondvalue==0));
				}
			}
		}
		public bool[][] PossibleVertexEdgeCollisionsFor1
		{
			get
			{
				return possibleVertexEdgeCollisionsFor1;
			}
		}
		public bool[][] PossibleVertexEdgeCollisionsFor2
		{
			get
			{
				return possibleVertexEdgeCollisionsFor2;
			}
		}
	}
}
