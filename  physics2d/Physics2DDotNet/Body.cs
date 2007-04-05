#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion




#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    /// <summary>
    /// This is the Physical Body that collides in the engine.
    /// </summary>
    [Serializable]
    public sealed class Body : IPhysicsEntity, IDuplicateable<Body>
    {
        #region static methods
        private static MassInfo GetMassInfo(Scalar mass, Shape shape)
        {
            if (shape == null) { throw new ArgumentNullException("shape"); }
            return new MassInfo(mass, shape.MomentofInertiaMultiplier * mass);
        }
        public static bool CanCollide(Body body1, Body body2)
        {
            if (body1 == null) { throw new ArgumentNullException("body1"); }
            if (body2 == null) { throw new ArgumentNullException("body2"); }
            return
                body1.isCollidable &&
                body2.isCollidable &&
                (body1.ignorer == null ||
                (body1.ignorer.CanCollide(body2)))
                &&
                (body2.ignorer == null ||
                (body2.ignorer.CanCollide(body1)));
        }
        #endregion
        #region events
        /// <summary>
        /// Raised when the Lifetime property has been Changed.
        /// </summary>
        public event EventHandler LifetimeChanged;
        /// <summary>
        /// Raised when the Shape of the Body has been Changed.
        /// </summary>
        public event EventHandler ShapeChanged;
        /// <summary>
        /// Raised when the object is added to a Physics Engine.
        /// </summary>
        public event EventHandler Added;
        /// <summary>
        /// Raised when the object is Removed from a Physics Engine. 
        /// </summary>
        public event EventHandler<RemovedEventArgs> Removed;
        /// <summary>
        /// Raised when the object is Added to the engine but is not yet part of the update process.
        /// </summary>
        public event EventHandler Pending;
        /// <summary>
        /// Raised when the State has been Changed.
        /// Raised by either the Solver or a call to ApplyMatrix.
        /// </summary>
        public event EventHandler StateChanged;
        /// <summary>
        /// Raised when the Body has been updated to a change in time.
        /// </summary>
        public event EventHandler<UpdatedEventArgs> Updated;
        /// <summary>
        /// Raised when the Body collides with another.
        /// </summary>
        public event EventHandler<CollisionEventArgs> Collided;
        #endregion
        #region fields
        PhysicsEngine engine;
        Shape shape;
        PhysicsState state;
        MassInfo massInfo;
        Coefficients coefficients;
        Lifespan lifetime;
        CollisionIgnorer ignorer;
        int id = -1;
        internal int jointCount;
        internal bool isChecked;
        bool ignoresGravity;
        bool ignoresCollisionResponce;
        bool broadPhaseDetectionOnly;
        bool isPending;
        bool isCollidable;
        object tag;
        object solverTag;
        object detectorTag;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a new Body Instance.
        /// </summary>
        /// <param name="state">The State of the Body.</param>
        /// <param name="shape">The Shape of the Body.</param>
        /// <param name="mass">The mass of the Body The inertia will be aquired from the Shape.</param>
        /// <param name="coefficients">A object containing coefficients.</param>
        /// <param name="lifeTime">A object Describing how long the object will be in the engine.</param>
        public Body(
            PhysicsState state,
            Shape shape,
            Scalar mass,
            Coefficients coefficients,
            Lifespan lifetime)
            : this(
                state, shape,
                GetMassInfo(mass, shape),
                coefficients, lifetime) { }
        /// <summary>
        /// Creates a new Body Instance.
        /// </summary>
        /// <param name="state">The State of the Body.</param>
        /// <param name="shape">The Shape of the Body.</param>
        /// <param name="massInfo">A object describing the mass and inertia of the Body.</param>
        /// <param name="coefficients">A object containing coefficients.</param>
        /// <param name="lifeTime">A object Describing how long the object will be in the engine.</param>
        public Body(
            PhysicsState state,
            Shape shape,
            MassInfo massInfo,
            Coefficients coefficients,
            Lifespan lifetime)
        {
            if (state == null) { throw new ArgumentNullException("state"); }
            if (shape == null) { throw new ArgumentNullException("shape"); }
            if (massInfo == null) { throw new ArgumentNullException("massInfo"); }
            if (coefficients == null) { throw new ArgumentNullException("coefficients"); }
            if (lifetime == null) { throw new ArgumentNullException("lifetime"); }
            this.state = new PhysicsState(state);
            this.Shape = shape;
            this.massInfo = massInfo;
            this.coefficients = coefficients;
            this.lifetime = lifetime;
            this.isCollidable = true;
        }

        private Body(Body copy)
        {
            this.ignoresGravity = copy.ignoresGravity;
            this.ignoresCollisionResponce = copy.ignoresCollisionResponce;

            this.state = new PhysicsState(copy.state);
            this.shape = copy.shape.Duplicate(); ;
            this.massInfo = copy.massInfo;
            this.coefficients = copy.coefficients;
            this.lifetime = copy.lifetime.Duplicate();
            if (copy.ignorer is ICloneable)
            {
                this.ignorer = (CollisionIgnorer)((ICloneable)copy.ignorer).Clone();
            }
            else
            {
                this.ignorer = copy.ignorer;
            }
            if (copy.tag is ICloneable)
            {
                this.tag = ((ICloneable)copy.tag).Clone();
            }
            else
            {
                this.tag = copy.tag;
            }
            this.isCollidable = copy.isCollidable;
            this.broadPhaseDetectionOnly = copy.broadPhaseDetectionOnly;
            this.ignoresCollisionResponce = copy.ignoresCollisionResponce;
            this.ignoresGravity = copy.ignoresGravity;
        }
        #endregion
        #region properties
        /// <summary>
        /// Gets if it has been added the the Engine's PendingQueue, but not yet added to the engine.
        /// </summary>
        public bool IsPending
        {
            get { return isPending; }
        }
        /// <summary>
        /// Unique ID of a PhysicsEntity in the PhysicsEngine
        /// Assigned on being Added.
        /// </summary>
        public int ID
        {
            get { return id; }
            internal set { id = value; }
        }
        /// <summary>
        /// Gets The PhysicsEngine the object is currently in. Null if it is in none.
        /// </summary>
        public PhysicsEngine Engine
        {
            get { return engine; }
        }
        /// <summary>
        /// Gets The current State of the object IE Velocity 
        /// </summary>
        public PhysicsState State
        {
            get { return state; }
        }
        /// <summary>
        /// Gets and Sets the Shape of the Body. 
        /// If setting the shape to a shape another body has it will duplicate the shape.
        /// </summary>
        public Shape Shape
        {
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                if (value != this.shape)
                {
                    if (this.shape != null) { this.shape.OnRemoved(); }
                    this.shape = null;
                    if (value.Parent != null)
                    {
                        value = value.Duplicate();
                    }
                    this.shape = value;
                    value.OnAdded(this);
                    if (ShapeChanged != null) { ShapeChanged(this, EventArgs.Empty); }
                }
            }
            get { return shape; }
        }
        /// <summary>
        /// Gets The MassInfo of the body.
        /// </summary>
        public MassInfo Mass
        {
            get { return massInfo; }
        }
        /// <summary>
        /// Gets and Sets the Ignore object that decides what collisons to ignore.
        /// </summary>
        public CollisionIgnorer Ignorer
        {
            get { return ignorer; }
            set { ignorer = value; }
        }
        /// <summary>
        /// Gets and Sets the Coefficients for the class.
        /// </summary>
        public Coefficients Coefficients
        {
            get { return coefficients; }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                coefficients = value;
            }
        }
        /// <summary>
        /// Gets and Sets the LifeTime of the object. The object will be removed from the engine when it is Expired.
        /// </summary>
        public Lifespan Lifetime
        {
            get
            {
                return lifetime;
            }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                if (this.lifetime != value)
                {
                    lifetime = value;
                    if (LifetimeChanged != null) { LifetimeChanged(this, EventArgs.Empty); }
                }
            }
        }
        /// <summary>
        /// Gets and Sets a User defined object.
        /// </summary>
        public object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }
        /// <summary>
        /// Gets a object Solver Defined object.
        /// </summary>
        public object SolverTag
        {
            get { return solverTag; }
            internal set { solverTag = value; }
        }
        /// <summary>
        /// Gets a object Detector Defined object.
        /// </summary>
        public object DetectorTag
        {
            get { return detectorTag; }
            internal set { detectorTag = value; }
        }
        /// <summary>
        /// the number of Joints attached to this body.
        /// </summary>
        public int JointCount
        {
            get { return jointCount; }
        }
        /// <summary>
        /// Gets and Sets if the Body will ignore Gravity.
        /// </summary>
        public bool IgnoresGravity
        {
            get { return ignoresGravity; }
            set { ignoresGravity = value; }
        }
        /// <summary>
        /// Gets and Sets if the Object will ignore the collison Responce but still generate the Collision event.
        /// </summary>
        public bool IgnoresCollisionResponse
        {
            get { return ignoresCollisionResponce; }
            set { ignoresCollisionResponce = value; }
        }
        /// <summary>
        /// Gets and Sets if this body detects collisions only with bounding boxes 
        /// and if it does then only bodies colliding it will also generate collision events as well.
        /// Setting this to true can allow you to write your own collision Solver just for this Body. 
        /// Or you can use this to do clipping.
        /// </summary>
        public bool BroadPhaseDetectionOnly
        {
            get { return broadPhaseDetectionOnly; }
            set { broadPhaseDetectionOnly = value; }
        }
        /// <summary>
        /// Gets if the object has been added to the engine.
        /// </summary>
        public bool IsAdded
        {
            get
            {
                return engine != null && !isPending;
            }
        }
        /// <summary>
        /// gets and sets if the body will have any collision detection ran on it.
        /// </summary>
        public bool IsCollidable
        {
            get { return isCollidable; }
            set { isCollidable = value; }
        }
        public Scalar KineticEnergy
        {
            get
            {
                Scalar velocityMag;
                Vector2D.GetMagnitude(ref state.Velocity.Linear,out velocityMag);
                return
                    .5f * (velocityMag * velocityMag * massInfo.Mass +
                    state.Velocity.Angular * state.Velocity.Angular * massInfo.MomentofInertia);

            }
        }
        #endregion
        #region methods
        public void UpdatePosition(Scalar dt)
        {
            state.Position.Linear.X += state.Velocity.Linear.X * dt;
            state.Position.Linear.Y += state.Velocity.Linear.Y * dt;
            state.Position.Angular += state.Velocity.Angular * dt;
            ApplyMatrix();
        }

        public void UpdatePosition(Scalar dt, ALVector2D extraVelocity)
        {
            UpdatePosition(dt, ref extraVelocity);
        }
        [CLSCompliant(false)]
        public void UpdatePosition(Scalar dt, ref ALVector2D extraVelocity)
        {
            state.Position.Linear.X += (state.Velocity.Linear.X + extraVelocity.Linear.X) * dt;
            state.Position.Linear.Y += (state.Velocity.Linear.Y + extraVelocity.Linear.Y) * dt;
            state.Position.Angular += (state.Velocity.Angular + extraVelocity.Angular) * dt;
            ApplyMatrix();
        }
        public void UpdateVelocity(Scalar dt)
        {
            float massInv = massInfo.MassInv;
            if (massInv != 0)
            {
                state.Acceleration.Linear.X += state.ForceAccumulator.Linear.X * massInv;
                state.Acceleration.Linear.Y += state.ForceAccumulator.Linear.Y * massInv;
                state.Acceleration.Angular += state.ForceAccumulator.Angular * massInfo.MomentofInertiaInv;
            }
            state.Velocity.Linear.X += state.Acceleration.Linear.X * dt;
            state.Velocity.Linear.Y += state.Acceleration.Linear.Y * dt;
            state.Velocity.Angular += state.Acceleration.Angular * dt;
        }
        internal void UpdateTime(Scalar dt)
        {
            lifetime.Update(dt);
            shape.UpdateTime(dt);
            if (ignorer != null) { ignorer.UpdateTime(dt); }
            if (Updated != null) { Updated(this, new UpdatedEventArgs(dt)); }
        }

        /// <summary>
        /// Sets Acceleration and Force Acumilator to Zero.
        /// </summary>
        public void ClearForces()
        {
            this.state.Acceleration = ALVector2D.Zero;
            this.state.ForceAccumulator = ALVector2D.Zero;
        }

        public void ApplyForce(Vector2D force)
        {
            Vector2D.Add(ref state.ForceAccumulator.Linear, ref force, out state.ForceAccumulator.Linear);
        }
        [CLSCompliant(false)]
        public void ApplyForce(ref Vector2D force)
        {
            Vector2D.Add(ref state.ForceAccumulator.Linear, ref force, out state.ForceAccumulator.Linear);
        }
        public void ApplyForce(Vector2D force, Vector2D position)
        {
            Scalar torque;
            Vector2D.Add(ref state.ForceAccumulator.Linear, ref force, out state.ForceAccumulator.Linear);
            Vector2D.ZCross(ref position, ref force, out torque);
            state.ForceAccumulator.Angular += torque;
        }
        [CLSCompliant(false)]
        public void ApplyForce(ref Vector2D force, ref Vector2D position)
        {
            Scalar torque;
            Vector2D.Add(ref state.ForceAccumulator.Linear, ref force, out state.ForceAccumulator.Linear);
            Vector2D.ZCross(ref position, ref force, out torque);
            state.ForceAccumulator.Angular += torque;
        }
        public void ApplyTorque(Scalar torque)
        {
            state.ForceAccumulator.Angular += torque;
        }

        public void ApplyImpulse(Vector2D impulse)
        {
            ApplyImpulse(ref impulse);
        }
        [CLSCompliant(false)]
        public void ApplyImpulse(ref Vector2D impulse)
        {
            Scalar massInv = massInfo.MassInv;
            state.Velocity.Linear.X += impulse.X * massInv;
            state.Velocity.Linear.Y += impulse.Y * massInv;
        }
        public void ApplyImpulse(Vector2D impulse, Vector2D position)
        {
            ApplyImpulse(ref impulse, ref position);
        }
        [CLSCompliant(false)]
        public void ApplyImpulse(ref Vector2D impulse, ref Vector2D position)
        {
            Scalar massInv = massInfo.MassInv;
            Scalar IInv = massInfo.MomentofInertia;
            PhysicsHelper.AddImpulse(ref state.Velocity, ref impulse, ref position, ref massInv, ref IInv);
        }

        /// <summary>
        /// Applys a Matrix Created from the State.Position AlVector2D to the Shape.
        /// </summary>
        public void ApplyMatrix()
        {
            Matrix2D matrix;
            MathHelper.ClampAngle(ref state.Position.Angular);
            Matrix2D.FromALVector2D(ref state.Position, out matrix);
            ApplyMatrixInternal(ref matrix);
        }
        /// <summary>
        /// Applys a Matrix to the Shape and to the State.Position AlVector2D.
        /// </summary>
        /// <param name="matrix">The matrix being applied to the Body</param>
        public void ApplyMatrix(Matrix2D matrix)
        {
            ApplyMatrix(ref matrix);
        }
        /// <summary>
        /// Applys a Matrix to the Shape and to the State.Position AlVector2D.
        /// </summary>
        /// <param name="matrix">The matrix being applied to the Body</param>
        [CLSCompliant(false)]
        public void ApplyMatrix(ref Matrix2D matrix)
        {
            ALVector2D.Transform(ref matrix, ref state.Position, out state.Position);
            ApplyMatrix();
        }
        private void ApplyMatrixInternal(ref Matrix2D matrix)
        {
            shape.ApplyMatrix(ref matrix);
            if (engine == null || !engine.inUpdate)
            {
                OnStateChanged();
            }
        }

        public Body Duplicate()
        {
            return new Body(this);
        }
        public object Clone()
        {
            return Duplicate();
        }

        internal void OnCollision(Body other, Solvers.ICollisionInfo collisionInfo)
        {
            if (Collided != null)
            {
                Collided(this, new CollisionEventArgs(other, (collisionInfo == null) ? (null) : (collisionInfo.Contacts)));
            }
        }
        internal void OnStateChanged()
        {
            if (StateChanged != null) { StateChanged(this, EventArgs.Empty); }
        }
        internal void OnPending(PhysicsEngine engine)
        {
            this.isChecked = false;
            this.isPending = true;
            this.engine = engine;
            if (Pending != null) { Pending(this, EventArgs.Empty); }
        }
        internal void OnAdded()
        {
            this.isPending = false;
            if (Added != null) { Added(this, EventArgs.Empty); }
        }
        internal void OnRemoved()
        {
            bool wasPending = this.isPending;
            PhysicsEngine engine = this.engine;
            this.engine = null;
            this.id = -1;
            this.isPending = false;
            if (Removed != null) { Removed(this, new RemovedEventArgs(engine, wasPending)); }
        }
        #endregion
    }
}