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
                (body1.ignorer == null ||
                (body1.ignorer.IsCollidable && body1.ignorer.CanCollide(body2)))
                &&
                (body2.ignorer == null ||
                (body2.ignorer.IsCollidable && body2.ignorer.CanCollide(body1)));
        }
        #endregion
        #region events
        /// <summary>
        /// Raised when the Lifetime of the Body has been Changed.
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
        public event EventHandler Removed;
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
        Shape shape;
        PhysicsState state;
        MassInfo massInfo;
        Coefficients coefficients;
        Lifespan lifetime;
        CollisionIgnorer ignorer;
        object tag;
        int id = -1;
        object solverTag;
        object detectorTag;
        bool ignoresGravity;
        bool ignoresCollisionResponce;
        bool broadPhaseDetectionOnly;
        internal int jointCount;



        PhysicsEngine engine;
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
            this.ignorer = copy.ignorer;

            if (copy.tag is ICloneable)
            {
                this.tag = ((ICloneable)copy.tag).Clone();
            }
            else
            {
                this.tag = copy.tag;
            }
        }
        #endregion
        #region properties
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
        /// Gets The Engine the Body is currently in. Null if it is in none.
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
        /// Gets and Sets the LifeTime of the Object. The Body will be removed from the engine when it is Expired.
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
        /// Gets and Sets A User defined object.
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
            ALVector2D temp;
            if (massInfo.MassInv != 0)
            {
                temp.Angular = massInfo.MassInv;
                Vector2D.Multiply(ref state.ForceAccumulator.Linear, ref temp.Angular, out temp.Linear);
                Vector2D.Add(ref state.Acceleration.Linear, ref temp.Linear, out state.Acceleration.Linear);
                state.Acceleration.Angular += state.ForceAccumulator.Angular * massInfo.MomentofInertiaInv;
            }
            ALVector2D.Multiply(ref state.Acceleration, ref dt, out temp);
            ALVector2D.Add(ref state.Velocity, ref temp, out state.Velocity);

        }
        internal void UpdateTime(Scalar dt)
        {
            Lifetime.Update(dt);
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
            if (state.Position.Angular > MathHelper.TWO_PI)
            {
                state.Position.Angular -= MathHelper.Floor((state.Position.Angular / MathHelper.TWO_PI)) * MathHelper.TWO_PI;
            }
            else if (-state.Position.Angular < -MathHelper.TWO_PI)
            {
                state.Position.Angular += MathHelper.Ceiling((state.Position.Angular / MathHelper.TWO_PI)) * MathHelper.TWO_PI;
            }
            Matrix2D matrix;
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
            Vector2D Temp = Vector2D.Zero;
            Vector2D.Transform(ref matrix.VertexMatrix, ref Temp, out state.Position.Linear);
            Vector2D.Transform(ref matrix.NormalMatrix, ref Temp, out Temp);
            Vector2D.GetAngle(ref Temp, out state.Position.Angular);
            ApplyMatrixInternal(ref matrix);
        }
        private void ApplyMatrixInternal(ref Matrix2D matrix)
        {
            shape.ApplyMatrix(ref matrix);
            if (engine == null || !engine.inUpdate)
            {
                OnStateChanged();
            }
        }

        public bool CanCollide(Body other)
        {
            if (other == null) { throw new ArgumentNullException("other"); }
            return
                (ignorer == null ||
                (ignorer.IsCollidable && ignorer.CanCollide(other)))
                &&
                (other.ignorer == null ||
                (other.ignorer.IsCollidable && other.ignorer.CanCollide(other)));

        }
        public Body Duplicate()
        {
            return new Body(this);
        }

        internal void OnCollision(Body other,  Solvers.ICollisionInfo collisionInfo)
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
        internal void OnAdded(PhysicsEngine engine)
        {
            if (this.engine != null) { throw new InvalidOperationException("The IPhysicsEntity cannot be added to more then one engine or added twice."); }
            this.engine = engine;
            if (Added != null) { Added(this, EventArgs.Empty); }
        }
        internal void OnRemoved()
        {
            this.engine = null;
            this.id = -1;
            if (Removed != null) { Removed(this, EventArgs.Empty); }
        }

        #endregion
    }
}