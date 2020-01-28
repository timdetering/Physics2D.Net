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
using System.Runtime.Serialization;
using AdvanceMath.Geometry2D;
using AdvanceMath;


namespace Physics2D
{
    [Serializable]
    public abstract class FormlessBody : BaseCollidable, IMassive, IDeserializationCallback
    {
        #region fields
        private BodyFlags flags;
		protected MassInertia massInertia;	
		protected PhysicsState current;
        [NonSerialized]
		protected PhysicsState initial;
        [NonSerialized]
		protected PhysicsState good;
        [NonSerialized]
        protected Matrix2D matrix;
        #endregion
		#region constructors
		public FormlessBody(ILifeTime lifeTime, BodyFlags flags, MassInertia massInertia, PhysicsState state)
            :base(lifeTime)
		{
			if (float.IsPositiveInfinity(massInertia.Mass))
			{
                this.Flags = flags | BodyFlags.InfiniteMass;
			}
			else
			{
                this.Flags = flags;
			}
			this.massInertia = massInertia;
			this.current = new PhysicsState(state);
            this.initial = new PhysicsState(state);
            this.good = new PhysicsState(state);
            this.matrix  = Matrix2D.Identity;
		}
        protected FormlessBody(FormlessBody copy):base(copy)
        {
            this.current = new PhysicsState(copy.current);
            this.good = new PhysicsState(copy.good);
            this.initial = new PhysicsState(copy.initial);
            this.flags = copy.flags;
            this.massInertia = copy.massInertia;
            this.matrix = copy.matrix;
        }
        #endregion
        #region properties
        public MassInertia MassInertia
		{
			get
			{
				return massInertia;
			}
		}
        [System.ComponentModel.Browsable(false)]
        public PhysicsState Initial
        {
            get
            {
                return initial;
            }
            set
            {
				initial = value;
            }
        }
        [System.ComponentModel.Browsable(false)]
        public PhysicsState Good
        {
            get
            {
                return good;
            }
            set
            {
				good = value;
            }
        }
        public PhysicsState Current
        {
            get
            {
                return current;
            }
            set
            {
				current = value;
            }
        }
        [System.ComponentModel.Editor(typeof(AdvanceSystem.ComponentModel.Design.EnumEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public BodyFlags Flags
        {
            get
            {
                return flags;
            }
            set
            {
                flags = value;
                if ((value & BodyFlags.ImmovableMask) != BodyFlags.None)
                {
                    ignoreInfo.JoinGroupToIgnore(CollisionIgnoreInfo.ImmovableGroup);
                }
                else
                {
                    ignoreInfo.LeaveGroupToIgnore(CollisionIgnoreInfo.ImmovableGroup);
                }
            }
        }
        [System.ComponentModel.Browsable(false)]
        public Matrix2D Matrix
        {
            get
            {
                return matrix;
            }
        }
        #endregion
        #region methods
        protected virtual Matrix2D CalculateMatrix()
        {
            return current.Position.ToMatrix2D();
        }
        public virtual void LoadState()
		{
            current.Set(initial);
		}
        public virtual void LoadVelocityState()
        {
            current.Velocity = initial.Velocity;
        }
        public virtual void LoadPositionState()
        {
            current.Position = initial.Position;
        }
        public virtual void SaveState()
		{
            initial.Set(current);
		}
        public virtual void CalcAccelerations(Vector2D accelerationDueToGravity)
		{
			current.Acceleration.Angular = current.ForceAccumulator.Angular*massInertia.MomentofInertiaInv;
			current.Acceleration.Linear = current.ForceAccumulator.Linear*massInertia.MassInv;
            current.Acceleration.Linear += accelerationDueToGravity;
		}
		public virtual void ClearForces()
		{
            this.current.ForceAccumulator = ALVector2D.Zero;
            this.initial.ForceAccumulator = ALVector2D.Zero;
		}
		public virtual void SaveGood()
        {
			good.Set(current);
        }
        public override void Update(float dt)
		{
            if ((flags & BodyFlags.NoUpdate) != BodyFlags.NoUpdate)
            {
                this.current.Position.Angular = MathAdv.RadianMin(this.current.Position.Angular);
                SaveGood();
                base.Update(dt);
            }
		}
		public virtual void UpdatePosition(float dt)
		{
            if ((flags & BodyFlags.NoUpdate) != BodyFlags.NoUpdate)
            {
                current.Position += (current.Velocity * dt);
                matrix = CalculateMatrix();
            }
		}
		public virtual void UpdateVelocity(float dt)
		{
			current.Velocity +=(current.Acceleration*dt);
		}
		public virtual void ApplyTorque(float torque)
		{
			current.ForceAccumulator.Angular+=torque;
		}
		public virtual void ApplyForce(ForceInfo forceInfo)
		{
			Vector2D force = Vector2DTransform(forceInfo.Force,forceInfo.ForceTT);
			current.ForceAccumulator.Linear+=force;
			if(forceInfo.HasPosition)
			{
				Vector2D position = Vector2DTransform(forceInfo.Position,forceInfo.PositionTT);
				current.ForceAccumulator.Angular += (position ^ force) ;
			}
		}
		public virtual Vector2D GetVelocityAtWorld(Vector2D pointRelativeToWorld)
		{
			return  this.current.GetVelocityAtWorld(pointRelativeToWorld);
		}
		public virtual Vector2D GetVelocityAtRelative(Vector2D pointRelativeToBody)
		{
			return this.current.GetVelocityAtRelative(pointRelativeToBody);
        }
        public Vector2D Vector2DTransform(Vector2D source, Vector2DTransformType convertType)
        {
            switch (convertType)
            {
                case Vector2DTransformType.ToWorldAngular:
                    return matrix.NormalMatrix * source;
                case Vector2DTransformType.ToBodyAngular:
                    return matrix.NormalMatrix.Transpose * source;
                case Vector2DTransformType.ToWorldLinear:
                    return (source + this.Current.Position.Linear);
                case Vector2DTransformType.ToBodyLinear:
                    return (source - this.Current.Position.Linear);
                case Vector2DTransformType.ToWorld:
                    return matrix.VertexMatrix * source;
                case Vector2DTransformType.ToBody:
                    return matrix.VertexMatrix.Inverse * source;
                default:
                    return source;
            }
        }
        public abstract object Clone();
        public virtual void OnDeserialization(object sender)
        {
            this.initial = new PhysicsState(this.current);
            this.good = new PhysicsState(this.current);
            this.matrix = this.current.Position.ToMatrix2D();
        }
        #endregion
    }
}
