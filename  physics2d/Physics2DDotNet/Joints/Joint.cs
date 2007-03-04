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
using System.Collections.Generic;
using System.Threading;

using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{



    /// <summary>
    /// Describes a Connection between 2 objects. 
    /// </summary>
    [Serializable]
    public abstract class Joint : IJoint
    {
        public event EventHandler LifetimeChanged;
        public event EventHandler Added;
        public event EventHandler Pending;
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
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
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
        public abstract Body[] Bodies { get;}


        protected internal virtual void UpdateTime(Scalar dt) { }
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
            OnRemoved(engine,wasPending);
            if (Removed != null) { Removed(this, new RemovedEventArgs(engine,wasPending)); }
        }
        void OnBodyRemoved(object sender, EventArgs e)
        {
            this.lifetime.IsExpired = true;
        }
        protected virtual void OnPending() { }
        protected virtual void OnAdded() { }
        protected virtual void OnRemoved(PhysicsEngine engine,bool wasPending) { }
    }
}