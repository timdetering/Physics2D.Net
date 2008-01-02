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

using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet.Shapes;

namespace Physics2DDotNet.PhysicsLogics
{
    public delegate Vector2D MousePositionCallback();

    public sealed class MousePickingLogic : PhysicsLogic
    {
        public event EventHandler Updated;
        object syncRoot;
        List<Body> last;
        List<Body> current;
        List<Body> next;
        Body mousePoint;
        MousePositionCallback callback;
        public MousePickingLogic(MousePositionCallback callback, Lifespan lifetime)
            : base(lifetime)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }
            this.callback = callback;
            this.mousePoint = new Body(new PhysicsState(), new ParticleShape(), 1, new Coefficients(1, 1), lifetime);
            this.mousePoint.IgnoresCollisionResponse = true;
            this.mousePoint.IgnoresPhysicsLogics = true;
            this.mousePoint.IsEventable = false;
            this.mousePoint.Collided += OnCollided;
            this.last = new List<Body>();
            this.current = new List<Body>();
            this.next = new List<Body>();
            this.syncRoot = new object();
        }
        public ReadOnlyCollection<Body> Current
        {
            get
            {
                lock (syncRoot)
                {
                    return new ReadOnlyCollection<Body>(current);
                }
            }
        }
        public ReadOnlyCollection<Body> Last
        {
            get
            {
                lock (syncRoot)
                {
                    return new ReadOnlyCollection<Body>(last);
                }
            }
        }
        void OnCollided(object sender, CollisionEventArgs e)
        {
            next.Add(e.Other);
        }
        public override ReadOnlyCollection<Body> LogicBodies
        {
            get
            {
                return new System.Collections.ObjectModel.ReadOnlyCollection<Body>(new Body[] { mousePoint });
            }
        }
        protected internal override void UpdateTime(TimeStep step)
        {
            this.mousePoint.State.Position.Linear = callback();
            this.mousePoint.ApplyPosition();
            base.UpdateTime(step);
        }
        protected internal override void RunLogic(TimeStep step)
        {
            lock (syncRoot)
            {
                List<Body> temp = last;
                last = current;
                current = next;
                next = temp;
                temp.Clear();
            }
            if (Updated != null) { Updated(this, EventArgs.Empty); }
        }
    }
}