#region MIT License
/*
 * Copyright (c) 2005-2008 Jonathan Mark Porter. http://physics2d.googlepages.com/
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
using System.Collections.ObjectModel;
using System.Text;
using System.Drawing;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Tao.OpenGl;
using SdlDotNet.Graphics;
using SdlDotNet.Core;
using SdlDotNet.Input;

namespace Graphics2DDotNet
{

    public class Pendable<TParent> 
        where TParent : class
    {
        public event EventHandler Pending;
        public event EventHandler Added;
        public event EventHandler<RemovedEventArgs<TParent>> Removed;
        public event EventHandler ZOrderChanged;
        public event EventHandler LifetimeChanged;
       
        int id;
        int zOrder;
        bool isAdded;
        bool isChecked;
        TParent parent;
        Lifespan lifetime;
        object tag;
       
        public Pendable(Lifespan lifetime)
        {
            if (lifetime == null) { throw new ArgumentNullException("lifetime"); }
            this.lifetime = lifetime;
        }
        protected Pendable(Pendable<TParent> copy)
        {
            this.lifetime = copy.lifetime.Duplicate();
        }
        public int ID
        {
            get { return id; }
        }
        public int ZOrder
        {
            get
            {
                return zOrder;
            }
            set
            {
                if (zOrder != value)
                {
                    zOrder = value;
                    if (ZOrderChanged != null) { ZOrderChanged(this, EventArgs.Empty); }
                }
            }
        }
        public bool IsPending
        {
            get { return parent != null && !isAdded; }
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
        public TParent Parent
        {
            get { return parent; }
        }
        public bool IsAdded
        {
            get
            {
                return isAdded;
            }
        }

        internal void PreCheckInternal()
        {
            this.isChecked = false;
        }
        internal void CheckInternal()
        {
            if (this.parent != null || this.isChecked)
            {
                throw new InvalidOperationException(String.Format("A {0} cannot be added to more then one {1} or added twice.", this.GetType().Name, typeof(TParent).Name));
            }
            this.isChecked = true;
        }
        internal void OnPendingInternal(TParent parent)
        {
            this.isChecked = false;
            this.parent = parent;
            OnPending(EventArgs.Empty);
        }
        internal void OnAddedInternal(int id)
        {
            this.isAdded = true;
            this.id = id;
            OnAdded(EventArgs.Empty);
        }
        internal void OnRemovedInternal()
        {
            bool isPending = IsPending;
            TParent parent = this.parent;
            bool wasPending = isPending;
            this.isAdded = false;
            this.parent = null;
            this.id = -1;
            OnRemoved(new RemovedEventArgs<TParent>(parent, wasPending));
        }
        protected virtual void OnPending(EventArgs e)
        {
            if (Pending != null)
            {
                Pending(this, e);
            }
        }
        protected virtual void OnAdded(EventArgs e)
        {
            if (Added != null)
            {
                Added(this, e);
            }
        }
        protected virtual void OnRemoved(RemovedEventArgs<TParent> e)
        {
            if (Removed != null)
            {
                Removed(this, e);
            }
        }
    }
}