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
using AdvanceMath.Geometry2D;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet.Detectors
{

    [Serializable]
    public sealed class SweepAndPruneDetector : BroadPhaseCollisionDetector
    {
        sealed class Wrapper
        {
            public Dictionary<int, object> colliders = new Dictionary<int, object>();
            public LinkedListNode<Wrapper> node;
            public Body body;
            Stub[] stubs;
            public Wrapper(Body body)
            {
                this.body = body;
                this.node = new LinkedListNode<Wrapper>(this);
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
                colliders.Clear();
                body.Shape.CalcBoundingRectangle();
                BoundingRectangle rect = body.Shape.Rectangle;
                stubs[0].value = rect.Min.X;
                stubs[1].value = rect.Max.X;

                stubs[2].value = rect.Min.Y;
                stubs[3].value = rect.Max.Y;
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
            if (result == 0 && left != right)
            {
                result = ((left.begin) ? (-1) : (1));
            }
            return result;
        }
        List<Wrapper> wrappers;
        List<Stub> xStubs;
        List<Stub> yStubs;
        int lastXCount = 0;
        int lastYCount = 0;

        public SweepAndPruneDetector()
        {
            this.wrappers = new List<Wrapper>();
            this.xStubs = new List<Stub>();
            this.yStubs = new List<Stub>();
        }
        /// <summary>
        /// The amount of extra capacity for the collection that stores all collisions along the x-axis.
        /// </summary>
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
            for (int index = 0; index < wrappers.Count; ++index)
            {
                wrappers[index].Update();
            }
            xStubs.Sort(StubComparison);
            yStubs.Sort(StubComparison);
            int count1 = 0;
            int count2 = 0;
            List<Stub> list1;
            List<Stub> list2;
            bool xSmall = lastXCount < lastYCount;
            if (xSmall)
            {
                list1 = yStubs;
                list2 = xStubs;
            }
            else
            {
                list1 = xStubs;
                list2 = yStubs;
            }
            LinkedList<Wrapper> currentBodies = new LinkedList<Wrapper>();
            LinkedListNode<Wrapper> node;
            for (int index = 0; index < list1.Count; ++index)
            {
                Stub stub = list1[index];
                if (stub.begin)
                {
                    Body body1 = stub.wrapper.body;
                    node = currentBodies.First;
                    while (node != null)
                    {
                        count1++;
                        Body body2 = node.Value.body;
                        if ((body1.Mass.MassInv != 0 || body2.Mass.MassInv != 0) &&
                            Body.CanCollide(body1, body2))
                        {
                            node.Value.colliders.Add(body1.ID, null);
                            stub.wrapper.colliders.Add(body2.ID, null);
                        }
                        node = node.Next;
                    }
                    currentBodies.AddLast(stub.wrapper.node);
                }
                else
                {
                    currentBodies.Remove(stub.wrapper.node);
                }
            }
            if (currentBodies.Count > 0)
            {
                throw new InvalidOperationException("The Detector is Corrupt!");
            }
            if (count1 == 0)
            {
                if (xSmall) { lastYCount = 0; }
                else { lastXCount = 0; }
                return;
            }
            for (int index = 0; index < list2.Count; ++index)
            {
                Stub stub = list2[index];
                if (stub.begin)
                {
                    Body body1 = stub.wrapper.body;
                    node = currentBodies.First;
                    while (node != null)
                    {
                        count2++;
                        Body body2 = node.Value.body;
                        if (node.Value.colliders.ContainsKey(body1.ID))
                        {
                            this.OnCollision(dt, body1, body2);
                        }
                        node = node.Next;
                    }
                    currentBodies.AddLast(stub.wrapper.node);
                }
                else
                {
                    currentBodies.Remove(stub.wrapper.node);
                }
            }
            if (currentBodies.Count > 0)
            {
                throw new InvalidOperationException("The Detector is Corrupt!");
            }
            if (xSmall)
            {
                lastYCount = count1;
                lastXCount = count2;
            }
            else
            {
                lastXCount = count1;
                lastYCount = count2;
            }
        }
    }
}