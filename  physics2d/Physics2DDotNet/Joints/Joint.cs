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
using System.Collections.ObjectModel;


namespace Physics2DDotNet
{



    /// <summary>
    /// Describes a Connection between 2 objects. 
    /// </summary>
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [Serializable]
#endif
    public abstract class Joint : IJoint
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
        object tag;
        PhysicsEngine engine;
        Lifespan lifetime;
        bool isPending;
        internal bool isChecked;

        protected Joint(Lifespan lifetime)
        {
            Lifetime = lifetime;
        }

        /// <summary>
        /// Gets if it has been added the the Engine's PendingQueue, but not yet added to the engine.
        /// </summary>
        public bool IsPending
        {
            get { return isPending; }
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
            get { return lifetime; }
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
        /// Gets The PhysicsEngine the object is currently in. Null if it is in none.
        /// </summary>
        public PhysicsEngine Engine
        {
            get { return engine; }
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
        /// Gets the bodies the Joint effects.
        /// </summary>
        public abstract ReadOnlyCollection<Body> Bodies { get;}


        protected internal virtual void UpdateTime(Scalar dt) { }
        protected internal void BeforeAddCheckInternal(PhysicsEngine engine)
        {
            foreach (Body item in Bodies)
            {
                if (item.Engine != engine)
                {
                    throw new InvalidOperationException("All Bodies the Joint Effects Must Be added to the Same Engine Before the Joint is added.");
                }
            }
            BeforeAddCheck(engine);
        }
        internal void OnPendingInternal(PhysicsEngine engine)
        {
            this.isChecked = false;
            this.isPending = true;
            this.engine = engine;
            OnPending();
            if (Pending != null) { Pending(this, EventArgs.Empty); }
        }
        internal void OnAddedInternal(PhysicsEngine engine)
        {
            this.isPending = false;
            this.engine = engine;
            foreach (Body b in Bodies)
            {
                b.Removed += OnBodyRemoved;
                b.jointCount++;
            }
            OnAdded();
            if (Added != null) { Added(this, EventArgs.Empty); }
        }
        internal void OnRemovedInternal()
        {
            foreach (Body b in Bodies)
            {
                if (!isPending)
                {
                    b.jointCount--;
                }
                b.Removed -= OnBodyRemoved;
            }

            PhysicsEngine engine = this.engine;
            bool wasPending = this.isPending;
            this.isPending = false;
            this.engine = null;
            OnRemoved(engine, wasPending);
            if (Removed != null) { Removed(this, new RemovedEventArgs(engine, wasPending)); }
        }
        void OnBodyRemoved(object sender, EventArgs e)
        {
            this.lifetime.IsExpired = true;
        }
        protected virtual void OnPending() { }
        protected virtual void OnAdded() { }
        protected virtual void OnRemoved(PhysicsEngine engine, bool wasPending) { }
        /// <summary>
        /// Before the item is allowed to be added to pending this method is called to 
        /// throw any exceptions without corrupting the state of the Physics engine.
        /// </summary>
        /// <param name="engine">The engine the item is about to be added too.</param>
        protected virtual void BeforeAddCheck(PhysicsEngine engine) { } 
    }
}