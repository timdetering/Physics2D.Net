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
    public sealed class SweepAndPruneDetector : BroadPhaseCollisionDetector
    {
        sealed class IntList 
        {
            static int[] Default = new int[0];
            int[] array = Default;
            int length = 0;
            public int Count
            {
                get
                {
                    return length;
                }
            }
            public void Add(int item)
            {
                if (array.Length == length)
                {
                    int newLenght = ((array.Length != 0) ? (array.Length * 2) : (4));
                    int[] newArray = new int[newLenght];
                    array.CopyTo(newArray, 0);
                    this.array = newArray;
                }
                this.array[length++] = item;
            }
            public void Sort()
            {
                Array.Sort<int>(array, 0, length);
            }
            public bool Contains(int item)
            {
                int min = 0;
                int max = length - 1;
                while (min <= max)
                {
                    int index = ((max - min) >> 1) + min;
                    int value = array[index];
                    if (value < item)
                    {
                        min = index + 1;
                    }
                    else if (value > item)
                    {
                        max = index - 1;
                    }
                    else 
                    {
                        return true;
                    }
                }
                return false;
            }
            public void Clear()
            {
                length = 0;
            }
        }
        sealed class StubComparer : IComparer<Stub>
        {
            public int Compare(Stub left, Stub right)
            {
                if (left.value < right.value) { return -1; }
                else if (left.value > right.value) { return 1; }
                else { return ((left == right) ? (0) : ((left.begin) ? (-1) : (1))); }
            }
        }
        sealed class Wrapper
        {
            public int beginCount;
            public IntList colliders = new IntList();
            //public List<int> colliders = new List<int>();
            public LinkedListNode<Wrapper> node;
            public Body body;
            public bool shouldAddNode;
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
                beginCount = -1;
                colliders.Clear();
                BoundingRectangle rect = body.Shape.Rectangle;
                shouldAddNode = rect.Min.X != rect.Max.X || rect.Min.Y != rect.Max.Y;
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


        private void Update()
        {
            for (int index = 0; index < wrappers.Count; ++index)
            {
                wrappers[index].Update();
            }
            xStubs.Sort(comparer);
            yStubs.Sort(comparer);
        }
        public override void Detect(Scalar dt)
        {
            Update();
            int count1 = 0;
            int count2 = 0;
            int beginCount = 0;
            List<Stub> list1, list2;
            LinkedList<Wrapper> currentBodies = new LinkedList<Wrapper>();
            LinkedListNode<Wrapper> node;
            Stub stub;
            Wrapper wrapper;
            Body body1, body2;

            bool xSmall = lastXCount > lastYCount;
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

            for (int index = 0; index < list1.Count; ++index)
            {
                stub = list1[index];
                wrapper = stub.wrapper;

                if (stub.begin)
                {
                    body1 = wrapper.body;
                    node = currentBodies.First;
                    while (node != null)
                    {
                        count1++;
                        body2 = node.Value.body;
                        if ((body1.Mass.MassInv != 0 || body2.Mass.MassInv != 0) &&
                            Body.CanCollide(body1, body2))
                        {
                            node.Value.colliders.Add(body1.ID);
                            wrapper.colliders.Add(body2.ID);
                        }
                        node = node.Next;
                    }
                    if (wrapper.shouldAddNode)
                    {
                        currentBodies.AddLast(wrapper.node);
                    }
                }
                else
                {
                    if (wrapper.shouldAddNode)
                    {
                        currentBodies.Remove(wrapper.node);
                    }
                    if (wrapper.colliders.Count > 0)
                    {
                        wrapper.colliders.Sort();
                    }
                }
            }

            if (count1 == 0)
            {
                if (xSmall) { lastYCount = 0; }
                else { lastXCount = 0; }
                return;
            }
            for (int index = 0; index < list2.Count; ++index)
            {
                stub = list2[index];
                wrapper = stub.wrapper;
                if (stub.begin)
                {
                    beginCount++;
                    if (wrapper.colliders.Count == 0)
                    {
                        count2 += currentBodies.Count;
                        wrapper.beginCount = beginCount;
                    }
                    else
                    {
                        body1 = wrapper.body;
                        node = currentBodies.First;
                        while (node != null)
                        {
                            count2++;
                            if (node.Value.colliders.Contains(body1.ID))
                            {
                                this.OnCollision(dt, body1, node.Value.body);
                            }
                            node = node.Next;
                        }
                        if (wrapper.shouldAddNode)
                        {
                            currentBodies.AddLast(wrapper.node);
                        }
                    }
                }
                else
                {
                    if (wrapper.beginCount > 0)
                    {
                        count2 += beginCount - wrapper.beginCount;
                    }
                    else if (wrapper.shouldAddNode)
                    {
                        currentBodies.Remove(wrapper.node);
                    }
                }
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