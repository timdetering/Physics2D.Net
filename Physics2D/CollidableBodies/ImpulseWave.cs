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

using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;using AdvanceMath;

namespace Physics2D.CollidableBodies
{

	/// <summary>
	/// This class is used to make a expanding circle that applys impulse to what it touches.
	/// basically it is meant to emulate an explosion.
	/// </summary>
    [Serializable]
    public class ImpulseWave : FormlessBody, ICollidableBody
    {
        #region fields
        protected float radius;
        protected float initialRadius;
        protected float expansionRate;
        protected float initialMass;
        protected float mass;
        protected RigidBodyPart[] rigidBodyParts = new RigidBodyPart[1];
        protected RigidBodyPart rigidBodyPart;
        protected CollisionState collisionState = new CollisionState();
        #endregion
        #region constructors
        public ImpulseWave(ILifeTime lifeTime, float mass, PhysicsState state, float initialRadius, float expansionRate)
            : base(
			lifeTime, BodyFlags.NoContact | BodyFlags.NoImpulse, 
			MassInertia.FromSolidCylinder(mass,initialRadius), 
			state)
			
		{
			this.radius = initialRadius;
			this.initialRadius = initialRadius;
			this.expansionRate = expansionRate;			
			this.mass = mass;
			this.initialMass = mass;
            rigidBodyParts[0] = new RigidBodyPart(ALVector2D.Zero, state.Position, new Circle2D(this.radius),new Coefficients(-.1f, 0.1f, 0.1f));
            rigidBodyPart = rigidBodyParts[0];
        }
        /*public ImpulseWave(ILifeTime lifeTime, float mass, PhysicsState state, float initialRadius, float expansionRate, int[] colors, int primaryColor)
            : base(
            lifeTime, BodyFlags.NoContact | BodyFlags.NoImpulse, 
            MassInertia.FromSolidCylinder(mass, initialRadius), 
            state)
        {
            this.radius = initialRadius;
            this.initialRadius = initialRadius;
            this.expansionRate = expansionRate;
            this.mass = mass;
            this.initialMass = mass;
            rigidBodyParts[0] = new RigidBodyPart(ALVector2D.Zero, state.Position, new Circle2D(this.radius), new Coefficients(-.1f, 0.1f, 0.1f), colors, primaryColor);
            rigidBodyPart = rigidBodyParts[0];
        }*/
        protected ImpulseWave(ImpulseWave copy)
            : base(copy)
        {
            this.radius = copy.radius;
            this.initialRadius = copy.initialRadius;
            this.expansionRate = copy.expansionRate;
            this.initialMass = copy.initialMass;
            this.mass = copy.mass;
            this.rigidBodyParts = new RigidBodyPart[] { (RigidBodyPart)copy.rigidBodyPart.Clone() };
            this.rigidBodyPart = this.rigidBodyParts[0];
            this.collisionState = (CollisionState)copy.collisionState.Clone();
        }
        #endregion
        #region properties
        public BoundingBox2D SweepBoundingBox2D
        {
            get
            {
                Vector2D Upper = this.current.Position.Linear;
                Vector2D Lower = this.initial.Position.Linear;
                if (Upper.X < Lower.X)
                {
                    Upper.X = this.initial.Position.Linear.X;
                    Lower.X = this.current.Position.Linear.X;
                }
                if (Upper.Y < Lower.Y)
                {
                    Upper.Y = this.initial.Position.Linear.Y;
                    Lower.Y = this.current.Position.Linear.Y;
                }
                Upper.X += this.radius;
                Upper.Y += this.radius;
                Lower.X -= this.radius;
                Lower.Y -= this.radius;
                return new BoundingBox2D(Upper, Lower);
            }
        }
        public CollisionState CollisionState
        {
            get
            {
                return collisionState;
            }
            set
            {
                collisionState = value;
            }
        }
        public float BoundingRadius { get { return this.radius; } }
        public ICollidableBodyPart[] CollidableParts
        {
            get
            {
                return (ICollidableBodyPart[])rigidBodyParts;
            }
        }
        #endregion
        #region methods
        public void SetAllPositions()
        {
            matrix = current.Position.ToMatrix2D();
            rigidBodyParts[0].SetPosition(current.Position, matrix);
            CalcBoundingBox2D();
            SaveState();
            SaveGood();
        }
        public void SetAllPositions(ALVector2D position)
        {
            current.Position = position;
            SetAllPositions();
        }
        public override Vector2D GetVelocityAtWorld(Vector2D pointRelativeToWorld)
		{
			return GetVelocityAtRelative(pointRelativeToWorld-this.current.Position.Linear);
		}
		public override Vector2D GetVelocityAtRelative(Vector2D pointRelativeToBody)
		{
			Vector2D pointRelativeToBodyNorm = Vector2D.Normalize(pointRelativeToBody);
			return this.current.Velocity.Linear+pointRelativeToBodyNorm*expansionRate;
		}
        public override sealed void CalcBoundingBox2D()
		{
            this.rigidBodyPart.CalcBoundingBox2D();
            this.boundingBox2D = this.rigidBodyPart.BoundingBox2D;
		}
        public override void SaveGood()
        {
           base.SaveGood();
           rigidBodyParts[0].SaveGood();
        }
        public override void Update(float dt)
		{
			base.Update(dt);
            UpdateRadius(dt);
		}
        public void UpdateRadius(float dt)
        {
            radius += expansionRate * Math.Min(dt, lifeTime.TimeLeft);
            this.mass = ((initialRadius * initialRadius) / (radius * radius)) * this.initialMass;
            rigidBodyPart.BaseGeometry = new Circle2D(radius);
            this.massInertia = MassInertia.FromSolidCylinder(mass, radius);
        }
		public override void UpdatePosition(float dt)
		{
			base.UpdatePosition(dt);
            rigidBodyParts[0].SetPosition(current.Position,matrix);// = current.Position;
		}
		public float GetK(Vector2D pointRelativeToBody,Vector2D normal)
		{
			/*if (this.collisionState.Frozen || float.IsInfinity(massInertia.Mass))
			{
				return 0;
			}*/
			/*if (float.IsInfinity(massInertia.Mass))
			{
				return 0;
			}*/
			return massInertia.MassInv+(normal* ((massInertia.MomentofInertiaInv * ((pointRelativeToBody^normal)))^ pointRelativeToBody));
		}
		public void ApplyImpulse(Vector2D pointRelativeToBody,Vector2D Impulse)
		{
        }
        public override void SaveState()
        {
            base.SaveState();
            rigidBodyPart.Save();
        }
        public override void LoadState()
        {
            base.LoadState();
            rigidBodyPart.Load();
        }
        public override void LoadPositionState()
        {
            base.LoadPositionState();
            rigidBodyPart.Load();
        }
        public Circle2D ToCircle2D()
        {
            return new Circle2D(radius, current.Position.Linear);
        }
        public override object Clone()
        {
            return new ImpulseWave(this);
        }
        #endregion
    }
}
