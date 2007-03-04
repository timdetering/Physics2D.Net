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
using System.Collections.Generic;

using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    /// <summary>
    /// A physics logic is a way for the engine to effect object within the Update call.
    /// Gravity is a Example of a PhysicsLogic.
    /// </summary>
    [Serializable]
    public abstract class PhysicsLogic : IPhysicsEntity
    {
        /// <summary>
        /// Raised when the Lifetime property has been Changed.
        /// </summary>
        public event EventHandler LifetimeChanged;
        /// <summary>
        /// Raised when the object is added to a Physics Engine.
        /// </summary>
        public event EventHandler Added;
        /// <summary>
        /// Raised when the object is Added to the engine but is not yet part of the update process.
        /// </summary>
        public event EventHandler Pending;
        /// <summary>
        /// Raised when the object is Removed from a Physics Engine. 
        /// </summary>
        public event EventHandler<RemovedEventArgs> Removed;

        Lifespan lifetime;
        PhysicsEngine engine;
        object tag;
        bool isPending;
        internal bool isChecked;
        protected PhysicsLogic(Lifespan lifetime)
        {
            this.Lifetime = lifetime;
        }

        /// <summary>
        /// Gets if it has been added the the Engine's PendingQueue, but not yet added to the engine.
        /// </summary>
        public bool IsPending
        {
            get { return isPending; }
        }
        /// <summary>
        /// Gets The PhysicsEngine the object is currently in. Null if it is in none.
        /// </summary>
        public PhysicsEngine Engine
        {
            get { return engine; }
        }
        /// <summary>
        /// Gets and Sets a User defined object.
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
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
        /// Gets if the object has been added to the engine.
        /// </summary>
        public bool IsAdded
        {
            get
            {
                return engine != null && !isPending;
            }
        }
        protected List<Body> Bodies
        {
            get
            {
                return engine.bodies;
            }
        }


        protected internal abstract void RunLogic(Scalar dt);
        internal void OnPendingInternal(PhysicsEngine engine)
        {
            this.isChecked = true;
            this.isPending = true;
            this.engine = engine;
            OnPending();
            if (Pending != null) { Pending(this, EventArgs.Empty); }
        }
        protected virtual void OnPending() { }
        internal void OnAddedInternal()
        {
            this.isPending = false;
            OnAdded();
            if (Added != null) { Added(this, EventArgs.Empty); }
        }
        internal void OnRemovedInternal()
        {
            bool wasPending = this.isPending;
            PhysicsEngine engine = this.engine;
            this.isPending = false;
            this.engine = null;
            OnRemoved(engine, wasPending);
            if (Removed != null) { Removed(this, new RemovedEventArgs(engine, wasPending)); }
        }

        protected virtual void OnAdded() { }
        protected virtual void OnRemoved(PhysicsEngine engine, bool wasPending) { }
        protected internal virtual void UpdateTime(Scalar dt)
        {
            this.lifetime.Update(dt);
        }
    }

}