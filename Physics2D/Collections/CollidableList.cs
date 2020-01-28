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
using System.Collections.Generic;
namespace Physics2D.Collections
{
    [Serializable]
    public class CollidableBodyList<T> : BaseCollidableList<T>
        where T : ICollidableBody
    {
        public CollidableBodyList()
            : base()
        { }
        public CollidableBodyList(IEnumerable<T> collection)
            : base(collection)
        { }
        public CollidableBodyList(int capacity)
            : base(capacity)
        { }
        /// <summary>
        /// Calls <see cref="IMassive.UpdatePosition"/> foreach of the <typeparamref name="T"/> in the list.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        public void UpdatePosition(float dt)
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].UpdatePosition(dt);
            }
        }
        /// <summary>
        /// Calls <see cref="IMassive.UpdateVelocity"/> foreach of the <typeparamref name="T"/> in the list.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        public void UpdateVelocity(float dt)
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].UpdateVelocity(dt);
            }
        }
        /// <summary>
        /// Calls <see cref="IMassive.SaveState"/> foreach of the <typeparamref name="T"/> in the list.
        /// </summary>
        public void SaveState()
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].SaveState();
            }
        }
        /// <summary>
        /// Calls <see cref="IMassive.LoadVelocityState"/> foreach of the <typeparamref name="T"/> in the list.
        /// </summary>
        public void LoadVelocityState()
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].LoadVelocityState();
            }
        }
        /// <summary>
        /// Calls <see cref="IMassive.LoadPositionState"/> foreach of the <typeparamref name="T"/> in the list.
        /// </summary>
        public void LoadPositionState()
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].LoadPositionState();
            }
        }
        /// <summary>
        /// Calls <see cref="IMassive.LoadState"/> foreach of the <typeparamref name="T"/> in the list.
        /// </summary>
        public void LoadState()
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].LoadState();
            }
        }
        /// <summary>
        /// Calls <see cref="IMassive.ClearForces"/> foreach of the <typeparamref name="T"/> in the  list.
        /// </summary>
        public void ClearForces()
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].ClearForces();
            }
        }
        /// <summary>
        /// Calls <see cref="CollisionState.ResetEventInfo"/> of <see cref="ICollidableBody.CollisionState"/> foreach of the  <typeparamref name="T"/> in the list.
        /// </summary>
        public void ResetEventInfo()
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].CollisionState.ResetEventInfo();
            }
        }
        /// <summary>
        /// Calls <see cref="CollisionState.Reset"/> of <see cref="ICollidableBody.CollisionState"/> foreach of the  <typeparamref name="T"/> in the  list.
        /// </summary>
        public void ResetCollisionStates()
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].CollisionState.Reset();
            }
        }
        /// <summary>
        /// Sets <see cref="CollisionState.CollisionImpulseApplied"/> and  <see cref="CollisionState.ContactImpulseApplied"/> to false, of <see cref="ICollidableBody.CollisionState"/> foreach of the  <typeparamref name="T"/> in the list.
        /// </summary>
        public void ResetImpulseApplied()
        {
            T item;
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                item = base[pos];
                item.CollisionState.CollisionImpulseApplied = false;
                item.CollisionState.ContactImpulseApplied = false;
            }
        }
        /// <summary>
        /// Calculates the current <see cref="PhysicsState.Acceleration"/> of all the <typeparamref name="T"/> in the list.
        /// </summary>
        /// <param name="gravitySourceList">The gravity sources the will cause Acceleration</param>
        public void CalcAcceleration(GravitySourceList gravitySourceList)
        {
            T item;
            count = base.Count;
            if (gravitySourceList == null)
            {
                for (int pos = 0; pos < count; ++pos)
                {
                    item = base[pos];
                    item.CalcAccelerations(Vector2D.Zero);
                }
            }
            else
            {
                for (int pos = 0; pos < count; ++pos)
                {
                    item = base[pos];
                    if (!((item.Flags & BodyFlags.IgnoreGravity) == BodyFlags.IgnoreGravity))
                    {
                        item.CalcAccelerations(gravitySourceList.GetGravityPullAt(item.Current.Position.Linear));
                    }
                    else
                    {
                        item.CalcAccelerations(Vector2D.Zero);
                    }
                }
            }
        }

            
    }
}