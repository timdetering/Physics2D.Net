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
using System.Collections.Generic;
using AdvanceMath;
namespace Physics2D
{
    /// <summary>
    /// Some friction implimentation code.
    /// </summary>
    public interface IExtraForceLogic
    {
        void ApplyLogic<T>(float dt, T collidable) where T : ICollidableBody;
    }
    [Serializable]
    public class ExtraForceLogicList : List<IExtraForceLogic>
    {
        public ExtraForceLogicList()
            : base()
        { }
        public ExtraForceLogicList(IEnumerable<IExtraForceLogic> collection)
            : base(collection)
        { }
        public ExtraForceLogicList(int capacity)
            : base(capacity)
        { }
        public void ApplyLogic<T>(float dt, T collidable)where T : ICollidableBody
        {
            int count = this.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                this[pos].ApplyLogic<T>(dt, collidable);
            }
        }
    }
    [Serializable]
    public class SlidingFriction : IExtraForceLogic 
    {
        float gravityOnZ;
        float friction;
        public SlidingFriction(float gravityOnZ, float friction)
        {
            this.gravityOnZ = gravityOnZ;
            this.friction = friction;
        }
        public void ApplyLogic<T>(float dt, T collidable)where T : ICollidableBody
        {
            Vector2D velocity = collidable.Current.Velocity.Linear;
            float speed = velocity.Magnitude;
            if (speed > 0)
            {
                float normalForce = gravityOnZ * collidable.MassInertia.Mass;
                float distance = speed * dt;
                Vector2D normal = velocity * (1 / speed);
                collidable.ApplyForce(new ForceInfo(normal * (-normalForce * distance * friction)));
                velocity = collidable.Current.GetVelocityAtRelative(Vector2D.XAxis * collidable.BoundingRadius);
                speed = velocity * Vector2D.YAxis;
                if (speed > 0)
                {
                    distance = speed * dt;
                    collidable.ApplyTorque(Math.Sign(collidable.Current.Velocity.Angular)* (-normalForce * distance * friction));
                }
            }
        }
    }
}