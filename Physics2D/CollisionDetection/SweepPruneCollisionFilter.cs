#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter And Magnus Wolffelt
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using Physics2D;
using AdvanceMath.Geometry2D;
using AdvanceMath;

using System.Collections.Generic;
namespace Physics2D.CollisionDetection
{
    [Serializable]
    public sealed class SweepAndPruneCollisionFilter<CB> : ICollisionFilter<CB>
        where CB : class, ICollidableBody
    {
        #region feilds
        List<SweepEntry> sweepEntries;
        List<SweepStub> xAxisStubs;
        List<SweepStub> yAxisStubs;
        bool[,] collisionGrid;
        int collisionGridSize;
        int lastCollisionCount;
        CollisionPairParameters parameters;
        #endregion
        #region constructors
        public SweepAndPruneCollisionFilter()
        {
            this.xAxisStubs = new List<SweepStub>();
            this.yAxisStubs = new List<SweepStub>();
            this.sweepEntries = new List<SweepEntry>();
            this.collisionGridSize = -1;
            this.lastCollisionCount = 10;
            this.collisionGrid = null;
            this.parameters = new CollisionPairParameters();
        }
        #endregion
        #region properties
        public CollisionPairParameters CollisionPairParameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }
        #endregion
        #region methods
        #region public
        public void AddICollidableBody(CB collidable)
        {
            SweepEntry Entry = new SweepEntry(collidable, sweepEntries.Count);
            AddSweepEntry(Entry);
            ResizeGrid();
        }
        public void AddICollidableBodyRange(List<CB> collidables)
        {
            foreach (CB collidable in collidables)
            {
                SweepEntry Entry = new SweepEntry(collidable, sweepEntries.Count);
                AddSweepEntry(Entry);
            }
            ResizeGrid();
        }
        public bool RemoveExpired()
        {

            int sweepEntriescount = sweepEntries.Count;
            sweepEntries.RemoveAll(SweepEntry.ShouldRemove);
            xAxisStubs.RemoveAll(SweepStub.ShouldRemove);
            yAxisStubs.RemoveAll(SweepStub.ShouldRemove);

            /*for (int pos = sweepEntriescount - 1; pos != -1; --pos)
            {
                if (sweepEntries[pos].Collidable.IsExpired)
                {
                    sweepEntries.RemoveAt(pos);
                }
            }
            for (int pos = xAxisStubs.Count - 1; pos != -1; --pos)
            {
                if (xAxisStubs[pos].entry.Collidable.IsExpired)
                {
                    xAxisStubs.RemoveAt(pos);
                }
            }
            for (int pos = yAxisStubs.Count - 1; pos != -1; --pos)
            {
                if (yAxisStubs[pos].entry.Collidable.IsExpired)
                {
                    yAxisStubs.RemoveAt(pos);
                }
            }*/
            if (sweepEntriescount != sweepEntries.Count)
            {
                ResizeGrid();
                sweepEntriescount = sweepEntries.Count;
                for (int pos = sweepEntriescount - 1; pos != -1; --pos)
                {
                    sweepEntries[pos].Position = pos;
                }
            }
            return false;

        }
        public void ApplyFilter(BodyFlags filter, bool IncludeNonImpulseApplied)
        {
            foreach (SweepEntry entry in sweepEntries)
            {
                entry.Detect = (IncludeNonImpulseApplied || entry.Collidable.CollisionState.CollisionImpulseApplied) && ((filter & entry.Collidable.Flags) == BodyFlags.None);
            }
        }
        public List<CollisionPair> GetPossibleCollisions()
        {

            ResetGrid();
            CalcStubValues();
            Sort();
            int stubcount = xAxisStubs.Count;
            // Temp list for knowing which bounding points we are currently inside.
            LinkedList<SweepEntry> currentInsides = new LinkedList<SweepEntry>();
            List<CollisionPair> returnvalues = new List<CollisionPair>(lastCollisionCount);
            SweepStub stub;
            for (int pos = 0; pos < stubcount; pos++)
            {
                stub = xAxisStubs[pos];
                if (stub.Detect)
                {
                    if (stub.stubtype == StubType.MinX)
                    {
                        foreach (SweepEntry entry in currentInsides)
                        {
                            collisionGrid[stub.entry.Position, entry.Position] = true;
                            collisionGrid[entry.Position, stub.entry.Position] = true;
                        }
                        currentInsides.AddLast(stub.entry);
                    }
                    else
                    {
                        currentInsides.Remove(stub.entry);
                    }
                }
            }

            // Clear the temp list
            currentInsides.Clear();
            CB body1;
            CB body2;
            for (int pos = 0; pos < stubcount; pos++)
            {
                stub = yAxisStubs[pos];
                if (stub.Detect)
                {
                    if (stub.stubtype == StubType.MinY)
                    {
                        foreach (SweepEntry entry in currentInsides)
                        {
                            if (collisionGrid[stub.entry.Position, entry.Position] || collisionGrid[entry.Position, stub.entry.Position])
                            {
                                body1 = stub.entry.Collidable;
                                body2 = entry.Collidable;
                                if (body1.IgnoreInfo.CanCollideWith(body2.IgnoreInfo))
                                {
                                    returnvalues.Add(new CollisionPair(body1, body2, parameters));
                                }
                            }
                        }
                        currentInsides.AddLast(stub.entry);
                    }
                    else
                    {
                        currentInsides.Remove(stub.entry);
                    }
                }
            }
            lastCollisionCount = returnvalues.Count;
            if (lastCollisionCount < 10)
            {
                lastCollisionCount = 10;
            }
            return returnvalues;

        }
        public List<ICollidableBody>[] GetIntersections<BC>(List<BC> baseCollidables)
            where BC : class, IBaseCollidable
        {

            int boxesCount = baseCollidables.Count;
            int basePosition = this.sweepEntries.Count;
            CalcStubValues();
            for (int pos = 0; pos != boxesCount; ++pos)
            {
                SweepEntry entry = new SweepEntry(null, basePosition + pos);
                baseCollidables[pos].CalcBoundingBox2D();
                entry.CalcStubValues(baseCollidables[pos].BoundingBox2D);
                AddSweepEntry(entry);
            }
            Sort();
            if (!ResizeGrid())
            {
                ResetGrid();
            }
            LinkedList<SweepEntry> currentInsides = new LinkedList<SweepEntry>();
            List<ICollidableBody>[] returnvalues = new List<ICollidableBody>[boxesCount];
            for (int pos = 0; pos != boxesCount; ++pos)
            {
                returnvalues[pos] = new List<ICollidableBody>();
            }
            for (int i = 0; i < xAxisStubs.Count; i++)
            {
                if (xAxisStubs[i].stubtype == StubType.MinX)
                {
                    foreach (SweepEntry entry in currentInsides)
                    {
                        collisionGrid[xAxisStubs[i].entry.Position, entry.Position] = true;
                        collisionGrid[entry.Position, xAxisStubs[i].entry.Position] = true;
                    }
                    currentInsides.AddLast(xAxisStubs[i].entry);
                }
                else
                {
                    currentInsides.Remove(xAxisStubs[i].entry);
                }
            }

            // Clear the temp list
            currentInsides.Clear();
            int position1;
            int position2;
            bool Is1Box;
            bool Is2Box;

            for (int i = 0; i < yAxisStubs.Count; i++)
            {
                position1 = yAxisStubs[i].entry.Position;
                if (yAxisStubs[i].stubtype == StubType.MinY)
                {

                    foreach (SweepEntry entry in currentInsides)
                    {
                        position2 = entry.Position;
                        if (collisionGrid[position1, position2] || collisionGrid[position2, position1])
                        {
                            Is1Box = position1 >= basePosition;
                            Is2Box = position2 >= basePosition;
                            if (Is1Box ^ Is2Box)
                            {

                                if (Is1Box)
                                {
                                    int pos = position1 - basePosition;
                                    if (entry.Collidable.IgnoreInfo.CanCollideWith(baseCollidables[pos].IgnoreInfo))
                                    {
                                        returnvalues[pos].Add(entry.Collidable);
                                    }
                                }
                                else
                                {
                                    int pos = position2 - basePosition;
                                    if (yAxisStubs[i].entry.Collidable.IgnoreInfo.CanCollideWith(baseCollidables[pos].IgnoreInfo))
                                    {
                                        returnvalues[pos].Add(yAxisStubs[i].entry.Collidable);
                                    }
                                }
                            }
                        }
                    }
                    currentInsides.AddLast(yAxisStubs[i].entry);
                }
                else
                {
                    currentInsides.Remove(yAxisStubs[i].entry);
                }
            }
            RemoveAtAndAbove(basePosition);
            return returnvalues;
        }

        public void ClearICollidableBodys()
        {
            sweepEntries.Clear();
            xAxisStubs.Clear();
            yAxisStubs.Clear();
            collisionGrid = null;
            collisionGridSize = -1;
        }
        #endregion
        #region private
        private void AddSweepEntry(SweepEntry Entry)
        {
            sweepEntries.Add(Entry);
            xAxisStubs.Add(Entry.Stubs[0]);//(int)StubType.MinX
            xAxisStubs.Add(Entry.Stubs[1]);//(int)StubType.MaxX
            yAxisStubs.Add(Entry.Stubs[2]);//(int)StubType.MinY
            yAxisStubs.Add(Entry.Stubs[3]);//(int)StubType.MaxY
        }
        private bool ResizeGrid()
        {
            int diff = collisionGridSize - sweepEntries.Count;
            if (diff > 500 || diff < 0)
            {
                collisionGridSize = sweepEntries.Count;
                collisionGrid = new bool[collisionGridSize, collisionGridSize];
                return true;
            }
            return false;
        }
        private void ResetGrid()
        {
            int bodycount = sweepEntries.Count;
            int ypos;
            for (int xpos = 0; xpos < bodycount; ++xpos)
            {
                for (ypos = 0; ypos < bodycount; ++ypos)
                {
                    collisionGrid[xpos, ypos] = false;
                }
            }
        }
        private void RemoveAtAndAbove(int index)
        {
            int sweepEntriescount = sweepEntries.Count;
            sweepEntries.RemoveRange(index, sweepEntriescount - index);
            for (int pos = xAxisStubs.Count - 1; pos != -1; --pos)
            {
                if (xAxisStubs[pos].entry.Position >= index)
                {
                    xAxisStubs.RemoveAt(pos);
                }
            }
            for (int pos = yAxisStubs.Count - 1; pos != -1; --pos)
            {
                if (yAxisStubs[pos].entry.Position >= index)
                {
                    yAxisStubs.RemoveAt(pos);
                }
            }
            if (sweepEntriescount != sweepEntries.Count)
            {
                ResizeGrid();
                sweepEntriescount = sweepEntries.Count;
            }
        }
        private void CalcStubValues()
        {
            foreach (SweepEntry entry in this.sweepEntries)
            {
                entry.Collidable.CalcBoundingBox2D();
                entry.CalcStubValues();
            }
        }
        private void Sort()
        {
            xAxisStubs.Sort();
            yAxisStubs.Sort();
        }
        #endregion
        #endregion
        #region subclasses
        [Serializable]
        enum StubType : int
        {
            MinX = 0,
            MaxX = 1,
            MinY = 2,
            MaxY = 3,
            PointX = 4,
            PointY = 5,
        }
        [Serializable]
        class SweepEntry
        {
            public static bool ShouldRemove(SweepEntry entry)
            {
                return entry == null ||
                    entry.Collidable == null ||
                    entry.Collidable.IsExpired;
            }
            public int Position;
            public CB Collidable;
            public SweepStub[] Stubs;
            private bool detect;
            public SweepEntry(CB Collidable, int Position)
            {
                this.detect = true;
                this.Collidable = Collidable;
                this.Position = Position;
                this.Stubs = new SweepStub[4];
                for (int pos = 0; pos != 4; ++pos)
                {
                    this.Stubs[pos] = new SweepStub((StubType)pos, this);
                }
            }
            /*public SweepEntry(Vector2D dot, int Position)
            {
                this.detect = true;
                this.Collidable = null;
                this.Position = Position;
                this.Stubs = new SweepStub[2];
                this.Stubs[0] = new SweepStub(StubType.PointX, this);
                this.Stubs[1] = new SweepStub(StubType.PointY, this);
            }*/
            public bool Detect
            {
                get
                {
                    return detect;
                }
                set
                {
                    if (detect ^ value)
                    {
                        detect = value;
                        for (int pos = 0; pos != 4; ++pos)
                        {
                            this.Stubs[pos].Detect = value;
                        }
                    }
                }
            }
            public void CalcStubValues()
            {
                CalcStubValues(Collidable.SweepBoundingBox2D);
            }
            public void CalcStubValues(BoundingBox2D box)
            {
                Stubs[0].cachedValue = box.Lower.X;
                Stubs[1].cachedValue = box.Upper.X;
                Stubs[2].cachedValue = box.Lower.Y;
                Stubs[3].cachedValue = box.Upper.Y;
            }
        }
        [Serializable]
        class SweepStub : IComparable<SweepStub>
        {
            public static bool ShouldRemove(SweepStub stub)
            {
                return stub == null ||
                    stub.entry == null ||
                    stub.entry.Collidable == null ||
                    stub.entry.Collidable.IsExpired;
            }
            public StubType stubtype;
            public float cachedValue;
            public SweepEntry entry;
            public bool Detect;
            public SweepStub(StubType stubtype, SweepEntry entry)
            {
                this.Detect = true;
                this.stubtype = stubtype;
                this.entry = entry;
                this.cachedValue = 0;
            }
            #region IComparable<SweepStub> Members

            public int CompareTo(SweepStub other)
            {
                return this.cachedValue.CompareTo(other.cachedValue);
            }

            #endregion
        }
        #endregion
    }
}
