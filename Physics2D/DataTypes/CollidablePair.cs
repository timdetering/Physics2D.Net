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
namespace Physics2D
{
    [Serializable]
    public class CollidablePair
	{
		protected ICollidableBody collidable1;
		protected ICollidableBody collidable2;
		protected CollidablePair()
		{
		}
		public CollidablePair(ICollidableBody collidable1,ICollidableBody collidable2)
		{
			this.collidable1 = collidable2;
			this.collidable2 = collidable1;
		}
        /// <summary>
        /// Gets the first ICollidableBody in the pair.
        /// </summary>
        public ICollidableBody Collidable1
		{
			get
			{
				return collidable1;
			}
		}
		/// <summary>
        /// Gets the second ICollidableBody in the pair.
		/// </summary>
        public ICollidableBody Collidable2
		{
			get
			{
				return collidable2;
			}
		}
        /// <summary>
        /// Gets the Relative velocity without any angular calculations.
        /// </summary>
        public Vector2D RelativeLinearVelocity
        {
            get
            {
                return this.Collidable1.Current.Velocity.Linear - this.Collidable2.Current.Velocity.Linear;
            }
        }
        /// <summary>
        /// Updates both ICollidableBodys velocity.
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateVelocity(float dt)
        {
            collidable1.UpdateVelocity(dt);
            collidable2.UpdateVelocity(dt);
        }
        /// <summary>
        /// Updates both ICollidableBodys position.
        /// </summary>
        /// <param name="dt"></param>
        public void UpdatePosition(float dt)
        {
            collidable1.UpdatePosition(dt);
            collidable2.UpdatePosition(dt);
        }
        /// <summary>
        /// Saves boths ICollidableBodys state.
        /// </summary>
        public void SaveState()
        {
            collidable1.SaveState();
            collidable2.SaveState();
        }
        /// <summary>
        /// Loads boths ICollidableBodys state.
        /// </summary>
        public void LoadState()
        {
            collidable1.LoadState();
            collidable2.LoadState();
        }
	}
}
