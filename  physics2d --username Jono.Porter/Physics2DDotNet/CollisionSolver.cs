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

    [Serializable]
    public abstract class CollisionSolver
    {
        protected static void SetTag(Body body, object tag)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            body.SolverTag = tag;
        }

        PhysicsEngine engine;

        protected List<Joint> Joints { get { return engine.joints; } }
        protected List<Body> Bodies { get { return engine.bodies; } }
        /// <summary>
        /// The engine this solver is in.
        /// </summary>
        public PhysicsEngine Engine
        {
            get { return engine; }
        }

        protected internal abstract bool HandleCollision(Scalar dt, Body first, Body second);
        protected internal abstract void Solve(Scalar dt);

        internal void OnAddedInternal(PhysicsEngine engine)
        {
            if (this.engine != null) { throw new InvalidOperationException(); }
            this.engine = engine;
            OnAdded();
            this.AddRange(engine.bodies);
        }
        internal void OnRemovedInternal()
        {
            engine = null;
            Clear();
            OnRemoved();
        }
        protected virtual void OnAdded() { }
        protected virtual void OnRemoved() { }


        protected internal virtual void Clear() { }

        protected internal virtual void AddRange(ICollection<Body> collection) { }
        protected internal virtual void RemoveExpired() { }
        protected void Detect(Scalar dt)
        {
            engine.BroadPhase.Detect(dt);
        }

    }

}