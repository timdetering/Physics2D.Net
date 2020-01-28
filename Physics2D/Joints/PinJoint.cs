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
namespace Physics2D.Joints
{

	/// <summary>
	/// This is a joint class that joins 2 ICollidableBodys as is there was a pin holding them together.
	/// </summary>
    [Serializable]
    public sealed class PinJoint :BaseRemovableObject, IJoint
	{
		CollidablePair pair;
		Vector2D pointRelativeToICollidableBody1;
		Vector2D pointRelativeToICollidableBody2;
		float distanceAllowed;
		float timescale;
        JointBreakInfo breakInfo;
		public PinJoint(CollidablePair pair,Vector2D pointRelativeToWorld,float distanceAllowed,float timescale)
            : this(new Immortal(),pair, pointRelativeToWorld, distanceAllowed, timescale, JointBreakInfo.Unbreakable)
		{}
        public PinJoint(CollidablePair pair, Vector2D pointRelativeToWorld, float distanceAllowed, float timescale, float breakDistance, float breakVelocity, float breakImpulse)
		:this(new Immortal(),pair,pointRelativeToWorld,distanceAllowed,timescale,new JointBreakInfo(breakDistance,breakVelocity,breakImpulse))
        {}
        public PinJoint(ILifeTime lifeTime, CollidablePair pair, Vector2D pointRelativeToWorld, float distanceAllowed, float timescale, JointBreakInfo breakInfo)
            : base(lifeTime)
        {
            this.pair = pair;
            this.pointRelativeToICollidableBody1 = pair.Collidable1.Vector2DTransform(pointRelativeToWorld, Vector2DTransformType.ToBody);
            this.pointRelativeToICollidableBody2 = pair.Collidable2.Vector2DTransform(pointRelativeToWorld, Vector2DTransformType.ToBody);
            this.distanceAllowed = distanceAllowed;
            this.timescale = timescale;
            this.breakInfo = breakInfo;
        }
		#region IJoint Members

		Vector2D rPoint1;
		Vector2D rPoint2;
		Vector2D extraVelocity;
		public void PreCalc(float dt)
		{
            if (pair.Collidable1.IsExpired || pair.Collidable2.IsExpired)
            {
                this.IsExpired = true;
            }
            rPoint1 = pair.Collidable1.Vector2DTransform(pointRelativeToICollidableBody1, Vector2DTransformType.ToWorldAngular);
            rPoint2 = pair.Collidable2.Vector2DTransform(pointRelativeToICollidableBody2, Vector2DTransformType.ToWorldAngular);
			Vector2D worldPoint1 = rPoint1+pair.Collidable1.Current.Position.Linear;
			Vector2D worldPoint2 = rPoint2+pair.Collidable2.Current.Position.Linear;
			Vector2D deviation = worldPoint1 - worldPoint2;
			float deviationMag = deviation.Magnitude;
			float tmpTimeScale;
			if(timescale > 0)
			{
				tmpTimeScale = timescale;
			}
			else
			{
				tmpTimeScale = -timescale*dt;
			}
			if (tmpTimeScale < Physics.Tolerance)
			{
				tmpTimeScale = dt;
			}
			
			if (this.breakInfo.Distance < deviationMag)
			{
				this.IsExpired = true;
			}
            if (deviationMag > distanceAllowed && deviationMag > 0)
			{
				Vector2D deviationNormal = deviation*(1/deviationMag);
				if(deviationMag>10)
				{
					//TODO put in some max Velocity Checking.
					deviationMag = 10;
				}
				extraVelocity = ((deviationMag - distanceAllowed) / tmpTimeScale) * deviationNormal;
			}
			else
			{
				extraVelocity = Vector2D.Origin;
			}
		}
		public bool CalcAndApply(float dt)
		{
			if (pair.Collidable1.CollisionState.Frozen && pair.Collidable2.CollisionState.Frozen )
			{
				return false;
			} 
			Vector2D velocity1 = pair.Collidable1.Current.GetVelocityAtRelative(rPoint1);
			Vector2D velocity2 = pair.Collidable2.Current.GetVelocityAtRelative(rPoint2);
			Vector2D relativeVelocity = extraVelocity + (velocity1 - velocity2);
			float relativeVelocityMagSq = relativeVelocity.MagnitudeSq;
			if (relativeVelocityMagSq <= Physics.Tolerance)
			{
				return false;
			}
			float relativeVelocityMag = (float)Math.Sqrt(relativeVelocityMagSq);
			Vector2D relativeVelocityNormal = relativeVelocity * (1/relativeVelocityMag);
            if (relativeVelocityMag > this.breakInfo.Velocity)
			{
				//TODO put in some max Velocity Checking.
				this.IsExpired = true;
				//relativeVelocityMag = 90000;
			}
			float numerator = -relativeVelocityMag;
			float K = pair.Collidable1.GetK( rPoint1, relativeVelocityNormal)  + pair.Collidable2.GetK( rPoint2, relativeVelocityNormal);
			if (K <= Physics.Tolerance)
			{
				return false;
			}
			float impulse = (numerator / K);// * relativeVelocityNormal;
            if (Math.Abs(impulse) > this.breakInfo.Impulse)
            {
                this.IsExpired = true;
            }
            if(!pair.Collidable2.CollisionState.Frozen)
			{
				pair.Collidable2.ApplyImpulse(rPoint2,(-impulse)*relativeVelocityNormal);
			}
			if(!pair.Collidable1.CollisionState.Frozen)
			{
				pair.Collidable1.ApplyImpulse(rPoint1,(impulse)*relativeVelocityNormal);
			}
			return true;
		}
		#endregion
    }
}
