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
using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath;

namespace Physics2D.CollidableAreas
{
    [Serializable]
    public class ImpulseRay :BaseTimed, IRay2DEffect
    {
        protected RaySegment2D raySegment;
        protected float displayLength;
        protected float impulse;
        protected bool effectApplied = false;
        public ImpulseRay(
                    ILifeTime lifeTime, 
                    RaySegment2D raySegment, 
                    float impulse):base(lifeTime)
        {
            this.raySegment = raySegment;
            this.displayLength = raySegment.Length;
            this.impulse = impulse;
        }
        public virtual RaySegment2D RaySegment
        {
            get
            {
                return raySegment;
            }
        }
        public virtual void ApplyEffect(float dt, RayICollidableBodyPair pair)
        {
            displayLength = pair.BestIntersectInfo.DistanceFromOrigin;
            effectApplied = true;
            if ((pair.ICollidableBody.Flags & BodyFlags.NoImpulse) != BodyFlags.NoImpulse)
            {
                Vector2D point = raySegment.GetPoint(pair.BestIntersectInfo) - pair.ICollidableBody.Current.Position.Linear;
                float effectImpulse = impulse * dt;
                pair.ICollidableBody.ApplyImpulse(point, effectImpulse * raySegment.Direction);
            }
        }
        public float DisplayLength
        {
            get
            {
                return displayLength;
            }
        }
        public override void Update(float dt)
        {
            base.Update(dt);
            if (!effectApplied)
            {
                displayLength = raySegment.Length;
            }
            effectApplied = false;
        }
        public bool IsCollidable
        {
            get { return true; }
        }
    }
    [Serializable]
    public class AttachedImpulseRay : ImpulseRay
    {
        ICollidableBody collidable;
        Vector2D  offset;
        public AttachedImpulseRay(
                    ILifeTime lifeTime, 
                    RaySegment2D raySegment, 
                    float impulse, 
                    ICollidableBody collidable):base(lifeTime, raySegment, impulse)
        {
            this.collidable = collidable;
            this.offset = raySegment.Origin - collidable.Current.Position.Linear;
            //this.raySegment.Direction = collidable.Matrix.NormalMatrix.Transpose * raySegment.Direction;
        }
        public override RaySegment2D RaySegment
        {
            get
            {
                //return new RaySegment2D(collidable.Matrix.VertexMatrix * offset, collidable.Matrix.NormalMatrix * raySegment.Direction, raySegment.Length);

                raySegment.Origin = offset + collidable.Current.Position.Linear;
                return raySegment;
            }
        }
    }
}