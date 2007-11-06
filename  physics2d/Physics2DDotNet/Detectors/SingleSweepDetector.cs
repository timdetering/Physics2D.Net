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

using AdvanceMath.Geometry2D;

namespace Physics2DDotNet.Detectors
{
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
    [Serializable]
#endif
    public sealed class SingleSweepDetector : BroadPhaseCollisionDetector
    {
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
        [Serializable]
#endif
        sealed class StubComparer : IComparer<Stub>
        {
            public int Compare(Stub left, Stub right)
            {
                if (left.value < right.value) { return -1; }
                else if (left.value > right.value) { return 1; }
                else { return ((left == right) ? (0) : ((left.begin) ? (-1) : (1))); }
            }
        }
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
        [Serializable]
#endif
        sealed class Wrapper
        {
            public LinkedListNode<Wrapper> node;
            public Body body;
            public bool shouldAddNode;
            Stub begin;
            Stub end;
            public Scalar min;
            public Scalar max;

            public Wrapper(Body body)
            {
                this.body = body;
                this.node = new LinkedListNode<Wrapper>(this);
                begin = new Stub(this, true);
                end = new Stub(this, false);
            }
            public void AddStubs(List<Stub> stubs)
            {
                stubs.Add(begin);
                stubs.Add(end);
            }
            public void Update(bool doX)
            {
                BoundingRectangle rect = body.Shape.Rectangle;
                shouldAddNode = rect.Min.X != rect.Max.X || rect.Min.Y != rect.Max.Y;
                if (doX)
                {
                    begin.value = rect.Min.X;
                    end.value = rect.Max.X;
                    min = rect.Min.Y;
                    max = rect.Max.Y;
                }
                else
                {
                    min = rect.Min.X;
                    max = rect.Max.X;
                    begin.value = rect.Min.Y;
                    end.value = rect.Max.Y;
                }
            }
        }
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
        [Serializable]
#endif
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
        }

        static StubComparer comparer = new StubComparer();

        static bool WrapperIsRemoved(Wrapper wrapper)
        {
            return !wrapper.body.IsAdded;
        }
        static bool StubIsRemoved(Stub stub)
        {
            return !stub.wrapper.body.IsAdded;
        }
        List<Wrapper> wrappers;
        List<Stub> stubs;
        int lastXCount = 0;
        int lastYCount = 0;

        public SingleSweepDetector()
        {
            this.wrappers = new List<Wrapper>();
            this.stubs = new List<Stub>();
        }

        protected internal override void AddBodyRange(List<Body> collection)
        {
            int wrappercount = collection.Count + wrappers.Count;
            if (wrappers.Capacity < wrappercount)
            {
                wrappers.Capacity = wrappercount;
            }
            int nodeCount = collection.Count * 2 + stubs.Count;
            if (stubs.Capacity < nodeCount)
            {
                stubs.Capacity = nodeCount;
            }
            foreach (Body item in collection)
            {
                Wrapper wrapper = new Wrapper(item);
                wrappers.Add(wrapper);
                wrapper.AddStubs(stubs);
            }
        }
        protected internal override void Clear()
        {
            wrappers.Clear();
            stubs.Clear();
        }
        protected internal override void RemoveExpiredBodies()
        {
            wrappers.RemoveAll(WrapperIsRemoved);
            stubs.RemoveAll(StubIsRemoved);
        }


        int xUpdates = 0;
        int yUpdates = 0;
        public override void Detect(Scalar dt)
        {
            bool doX = ShouldDoX();

            Update(doX);
            int collisions = Detect(dt, doX);

            if (doX)
            {
                lastXCount = collisions;
                xUpdates++;
            }
            else
            {
                lastYCount = collisions;
                yUpdates++;
            }
        }
        bool ShouldDoX()
        {
            bool doX = lastXCount < lastYCount;
            if (Math.Abs(xUpdates - yUpdates) > 7)
            {
                doX = !doX;
                xUpdates = 0;
                yUpdates = 0;
            }
            return doX;
        }
        private void Update(bool doX)
        {
            for (int index = 0; index < wrappers.Count; ++index)
            {
                wrappers[index].Update(doX);
            }
            stubs.Sort(comparer);
        }

        int Detect(Scalar dt, bool doX)
        {
            int collisions = 0;
            LinkedList<Wrapper> currentBodies = new LinkedList<Wrapper>();
            LinkedListNode<Wrapper> node;
            Stub stub;
            Wrapper wrapper1, wrapper2;
            Body body1, body2;

            for (int index = 0; index < stubs.Count; index++)
            {
                stub = stubs[index];
                wrapper1 = stub.wrapper;

                if (stub.begin)
                {
                    body1 = wrapper1.body;
                    node = currentBodies.First;
                    while (node != null)
                    {
                        collisions++;
                        wrapper2 = node.Value;
                        body2 = wrapper2.body;
                        if ((body1.Mass.MassInv != 0 || body2.Mass.MassInv != 0) &&
                            Body.CanCollide(body1, body2) &&
                            wrapper1.min <= wrapper2.max &&
                            wrapper2.min <= wrapper1.max)
                        {
                            OnCollision(dt, body1, body2);
                        }
                        node = node.Next;
                    }
                    if (wrapper1.shouldAddNode)
                    {
                        currentBodies.AddLast(wrapper1.node);
                    }
                }
                else
                {
                    if (wrapper1.shouldAddNode)
                    {
                        currentBodies.Remove(wrapper1.node);
                    }
                }
            }
            return collisions;

        }
    }


}