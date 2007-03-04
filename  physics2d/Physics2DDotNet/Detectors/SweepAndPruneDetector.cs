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

namespace Physics2DDotNet.Detectors
{
    [Serializable]
    public sealed class SweepAndPruneDetector : BroadPhaseCollisionDetector
    {
        sealed class Wrapper
        {
            public LinkedListNode<Body> node;
            public Body body;
            Stub[] stubs;
            public Wrapper(Body entity)
            {
                this.body = entity;
                stubs = new Stub[4];
                stubs[0] = new Stub(this, true);  //x
                stubs[1] = new Stub(this, false); //x
                stubs[2] = new Stub(this, true);  //y
                stubs[3] = new Stub(this, false); //y
            }
            public void AddStubs(List<Stub> xStubs, List<Stub> yStubs)
            {
                xStubs.Add(stubs[0]);
                xStubs.Add(stubs[1]);

                yStubs.Add(stubs[2]);
                yStubs.Add(stubs[3]);
            }
            public void Update()
            {
                body.Shape.CalcBoundingBox2D();
                BoundingBox2D box = body.Shape.BoundingBox2D;
                stubs[0].value = box.Lower.X;
                stubs[1].value = box.Upper.X;

                stubs[2].value = box.Lower.Y;
                stubs[3].value = box.Upper.Y;
            }
        }
        sealed class Stub
        {
            public Wrapper wrapper;
            public bool begin;
            public Scalar value;
            public Stub(Wrapper wrapper, bool begin)
            {
                this.wrapper = wrapper;
                this.begin = begin;
            }
            public override string ToString()
            {
                return string.Format("{0}:{1}", wrapper.body.ID, (begin) ? ("Begin") : ("End"));
            }
        }
        static bool WrapperIsRemoved(Wrapper wrapper)
        {
            return !wrapper.body.IsAdded;
        }
        static bool StubIsRemoved(Stub stub)
        {
            return !stub.wrapper.body.IsAdded;
        }
        static int StubComparison(Stub left, Stub right)
        {
            int result = left.value.CompareTo(right.value);
            if (result == 0)
            {
                if (left == right)
                {

                }
                else if (left.begin)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            return result;
        }
        List<Wrapper> wrappers;
        List<Stub> xStubs;
        List<Stub> yStubs;
        int extraCapacity = 1000;
        int lastXCount = 50;
        public SweepAndPruneDetector()
        {
            this.wrappers = new List<Wrapper>();
            this.xStubs = new List<Stub>();
            this.yStubs = new List<Stub>();
        }
        /// <summary>
        /// The amount of extra capacity for the collection that stores all collisions along the x-axis.
        /// </summary>
        public int ExtraCapacity
        {
            get { return extraCapacity; }
            set
            {
                if (extraCapacity < 0) { throw new ArgumentOutOfRangeException("value", "The value must be equal or greater to zero."); }
                extraCapacity = value;
            }
        }
        protected internal override void AddBodyRange(List<Body> collection)
        {
            int wrappercount = collection.Count + wrappers.Count;
            if (wrappers.Capacity < wrappercount)
            {
                wrappers.Capacity = wrappercount;
            }
            int nodeCount = collection.Count * 2 + xStubs.Count;
            if (xStubs.Capacity < nodeCount)
            {
                xStubs.Capacity = nodeCount;
                yStubs.Capacity = nodeCount;
            }
            foreach (Body item in collection)
            {
                Wrapper wrapper = new Wrapper(item);
                wrappers.Add(wrapper);
                wrapper.AddStubs(xStubs, yStubs);
            }
        }
        protected internal override void Clear()
        {
            wrappers.Clear();
            xStubs.Clear();
            yStubs.Clear();
        }
        protected internal override void RemoveExpiredBodies()
        {
            wrappers.RemoveAll(WrapperIsRemoved);
            xStubs.RemoveAll(StubIsRemoved);
            yStubs.RemoveAll(StubIsRemoved);
        }
        public override void Detect(Scalar dt)
        {
            int Count = wrappers.Count;
            for (int index = 0; index < Count; ++index)
            {
                wrappers[index].Update();
            }
            xStubs.Sort(StubComparison);
            yStubs.Sort(StubComparison);
            Dictionary<long, object> colliders = new Dictionary<long, object>(lastXCount + extraCapacity);

            LinkedList<Body> currentBodies = new LinkedList<Body>();
            Count = xStubs.Count;
            for (int index = 0; index < Count; ++index)
            {
                Stub stub = xStubs[index];
                if (stub.begin)
                {
                    Body body1 = stub.wrapper.body;
                    foreach (Body body2 in currentBodies)
                    {
                        if (Body.CanCollide(body1, body2))
                        {
                            long id = PairID.GetId(body1.ID, body2.ID);
                            colliders.Add(id, null);
                        }
                    }
                    stub.wrapper.node = currentBodies.AddLast(body1);
                }
                else
                {
                    currentBodies.Remove(stub.wrapper.node);
                    stub.wrapper.node = null;
                }
            }
            if (currentBodies.Count > 0)
            {
                throw new InvalidOperationException("The Detector is Corrupt!");
            }
            for (int index = 0; index < Count; ++index)
            {
                Stub stub = yStubs[index];
                if (stub.begin)
                {
                    Body body1 = stub.wrapper.body;
                    foreach (Body body2 in currentBodies)
                    {
                        if (colliders.ContainsKey(PairID.GetId(body1.ID, body2.ID)))
                        {
                            this.OnCollision(dt, body1, body2);
                        }
                    }
                    stub.wrapper.node = currentBodies.AddLast(body1);
                }
                else
                {
                    currentBodies.Remove(stub.wrapper.node);
                    stub.wrapper.node = null;
                }
            }
            if (currentBodies.Count > 0)
            {
                throw new InvalidOperationException("The Detector is Corrupt!");
            }
            lastXCount = colliders.Count;
            colliders.Clear();
        }
    }
}