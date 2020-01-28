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
using AdvanceMath.Geometry2D;
using AdvanceMath;

namespace Physics2D
{
    /// <summary>
    /// Describes all you need to know about a Force.
    /// </summary>
    public sealed class ForceInfo 
	{
        /// <summary>
        /// This is the TransformType that must be applied to the Force before it is applied to the Body.
        /// </summary>
		public readonly Vector2DTransformType ForceTT;
        /// <summary>
        /// This is the TransformType that must be applied to the Poistion of the Force before it is applied to the Body.
        /// </summary>
		public readonly Vector2DTransformType PositionTT;
        /// <summary>
        /// This is the Force.
        /// </summary>
		public readonly Vector2D Force;
        /// <summary>
        /// This is the position of the Force.
        /// </summary>
		public readonly Vector2D Position;
        /// <summary>
        /// This states if the position is other that the grouppairs origin.
        /// </summary>
		public readonly bool HasPosition;
        /// <summary>
        /// Creates a new Force Instance
        /// </summary>
        /// <param name="Force">This is the Force.</param>
        public ForceInfo(Vector2D Force)
		{
			this.Force = Force;
			this.Position = Vector2D.Zero;
			this.ForceTT = Vector2DTransformType.None;
			this.PositionTT = Vector2DTransformType.None;
			this.HasPosition = false;
		}
        /// <summary>
        /// Creates a new Force Instance
        /// </summary>
        /// <param name="Force">This is the Force.</param>
        /// <param name="ForceTT">This is the TransformType that must be applied to the Force before it is applied to the Body.</param>
        public ForceInfo(Vector2D Force,Vector2DTransformType ForceTT)
		{
			this.Force = Force;
			this.Position = Vector2D.Zero;
			this.ForceTT = ForceTT;
			this.PositionTT = Vector2DTransformType.None;
			this.HasPosition = false;
		}
        /// <summary>
        /// Creates a new Force Instance
        /// </summary>
        /// <param name="Force">This is the Force.</param>
        /// <param name="ForceTT">This is the TransformType that must be applied to the Force before it is applied to the Body.</param>
        /// <param name="Position">This is the position of the Force.</param>
		public ForceInfo(Vector2D Force,Vector2DTransformType ForceTT,Vector2D Position)
		{
			this.Force = Force;
			this.Position = Position;
			this.ForceTT = ForceTT;
			this.PositionTT = Vector2DTransformType.None;
			this.HasPosition = true;
		}
        /// <summary>
        /// Creates a new Force Instance
        /// </summary>
        /// <param name="Force">This is the Force.</param>
        /// <param name="Position">This is the position of the Force.</param>
		public ForceInfo(Vector2D Force,Vector2D Position)
		{
			this.Force = Force;
			this.Position = Position;
			this.ForceTT = Vector2DTransformType.None;
			this.PositionTT = Vector2DTransformType.None;
			this.HasPosition = true;
		}
        /// <summary>
        /// Creates a new Force Instance
        /// </summary>
        /// <param name="Force">This is the Force.</param>
        /// <param name="ForceTT">This is the TransformType that must be applied to the Force before it is applied to the Body.</param>
        /// <param name="Position">This is the position of the Force.</param>
        /// <param name="PositionTT">This is the TransformType that must be applied to the Position of the Force before it is applied to the Body.</param>
		public ForceInfo(Vector2D Force,Vector2DTransformType ForceTT,Vector2D Position,Vector2DTransformType PositionTT)
		{
			this.Force = Force;
			this.Position = Position;
			this.ForceTT = ForceTT;
			this.PositionTT = PositionTT;
			this.HasPosition = true;
		}
	}
}
