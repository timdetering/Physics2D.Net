#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
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
using System.Collections.Generic;

namespace AdvanceSystem
{
    public sealed class IDManager
    {
        #region const fields
        private const long DefaultMaxValue = (long)(ulong)uint.MaxValue;
        private const long DefaultMinValue = 0;
        private const bool DefaultCheckForReRelease = true;
        #endregion
        #region fields
        private bool checkForReRelease;
        private long highestInUse;
        private List<uint> avaliableIDs;
        private long maxValue;
        private long minValue;
        #endregion
        #region constructors
        public IDManager() : this(DefaultMinValue, DefaultMaxValue, DefaultCheckForReRelease) { }
        public IDManager(bool checkForReRelease) : this(DefaultMinValue, DefaultMaxValue, checkForReRelease) { }
        public IDManager(long minValue, long maxValue) : this(minValue, maxValue, DefaultCheckForReRelease) { }
        public IDManager(long minValue, long maxValue, bool checkForReRelease)
        {
            if (minValue < DefaultMinValue)
            {
                throw new ArgumentOutOfRangeException("minValue");
            }
            if ((ulong)maxValue > (ulong)DefaultMaxValue)
            {
                throw new ArgumentOutOfRangeException("maxValue");
            }
            if (maxValue <= minValue)
            {
                throw new ArgumentOutOfRangeException("maxValue", "The value must be higher then minValue");
            }
            this.minValue = minValue - 1;
            this.highestInUse = this.minValue;
            this.maxValue = maxValue;
            this.checkForReRelease = checkForReRelease;
            this.avaliableIDs = new List<uint>();
        }
        #endregion
        #region properties
        public long MinValue
        {
            get { return minValue + 1; }
        }

        public long MaxValue
        {
            get { return maxValue; }
        }

        public long HighestInUse
        {
            get { return highestInUse; }
        }

        public bool CheckForReRelease
        {
            get { return checkForReRelease; }
            set { checkForReRelease = value; }
        }

        public long TotalIDCount
        {
            get { return maxValue - minValue; }
        }

        public long UsedIDCount
        {
            get { return TotalIDCount - AvaliableIDCount; }
        }

        public long AvaliableIDCount
        {
            get { return (maxValue - highestInUse) + avaliableIDs.Count; }
        }
        #endregion
        #region methods
        private uint GenerateIDInternal()
        {
            if (highestInUse > maxValue)
            {
                throw new Exception("Out of IDS");
            }
            return (uint)++highestInUse;
        }
        public uint GenerateID()
        {
            CheckState();
            lock (avaliableIDs)
            {
                if (avaliableIDs.Count == 0)
                {
                    return GenerateIDInternal();
                }
                else
                {
                    int removeindex = avaliableIDs.Count - 1;
                    uint ID = avaliableIDs[removeindex];
                    avaliableIDs.RemoveAt(removeindex);
                    return ID;
                }
            }
        }
        public uint[] GenerateIDRange(int count)
        {
            uint[] IDs = new uint[count];
            GenerateIDRange(IDs);
            return IDs;
        }
        public void GenerateIDRange(uint[] IDs)
        {
            CheckState();
            if (IDs == null)
            {
                throw new ArgumentNullException("IDs");
            }
            lock (avaliableIDs)
            {
                CheckIDsAvaliable(IDs.GetLongLength(0));
                int copyindex;
                if (avaliableIDs.Count <= IDs.Length)
                {
                    copyindex = IDs.Length - avaliableIDs.Count;
                    for (int index = 0; index < copyindex; ++index)
                    {
                        IDs[index] = GenerateIDInternal();
                    }
                    avaliableIDs.CopyTo(IDs, copyindex);
                    avaliableIDs.Clear();
                }
                else
                {
                    copyindex = avaliableIDs.Count - IDs.Length;
                    avaliableIDs.CopyTo(copyindex, IDs, 0, avaliableIDs.Count - copyindex);
                    avaliableIDs.RemoveRange(copyindex, avaliableIDs.Count - copyindex);
                }
            }
        }

        public IEnumerable<uint> GetUsedIDEnumerator()
        {
            lock (avaliableIDs)
            {
                for (long pos = MinValue; pos <= HighestInUse; ++pos)
                {
                    if (!avaliableIDs.Contains((uint)pos))
                    {
                        yield return (uint)pos;
                    }
                }
            }
        }
        public IEnumerable<uint> GetAvaliableIDEnumerator()
        {
            lock (avaliableIDs)
            {
                foreach (uint ID in avaliableIDs)
                {
                    yield return ID;
                }
                for (long pos = HighestInUse + 1; pos <= MaxValue; ++pos)
                {
                    yield return (uint)pos;
                }
            }
        }


        public void ReleaseID(uint ID)
        {
            CheckState();
            CheckRelease(ID);
            lock (avaliableIDs)
            {
                if (ID == highestInUse)
                {
                    Trim();
                }
                else
                {
                    avaliableIDs.Add(ID);
                    CheckAvaliableIDs();
                }
            }
        }
        public void ReleaseIDRange(uint[] IDs)
        {
            CheckState();
            if (IDs == null)
            {
                throw new ArgumentNullException("IDs");
            }
            if (IDs.Length > 0)
            {
                lock (avaliableIDs)
                {
                    for (int pos = 0; pos < IDs.Length; ++pos)
                    {
                        CheckRelease(IDs[pos]);
                    }
                    int index = Array.IndexOf<uint>(IDs, (uint)highestInUse);
                    if (index != -1)
                    {
                        IDs[index] = IDs[0];
                        avaliableIDs.AddRange(new SubArray<uint>(IDs, 1));
                        Trim();
                    }
                    else
                    {
                        avaliableIDs.AddRange(IDs);
                        CheckAvaliableIDs();
                    }
                }
            }
        }
        private void Trim()
        {
            avaliableIDs.Sort();
            long value;
            int index;
            for (index = avaliableIDs.Count - 1, value = highestInUse - 1; index > -1; --index, --value)
            {
                if (avaliableIDs[index] != value)
                {
                    index++;
                    highestInUse = value;
                    avaliableIDs.RemoveRange(index, avaliableIDs.Count - index);
                    return;
                }
            }
            avaliableIDs.Clear();
            highestInUse = value;
        }
        private void CheckIDsAvaliable(long count)
        {
            if (AvaliableIDCount < count)
            {
                throw new Exception("Not Enough Ids");
            }
        }
        private void CheckRelease(uint ID)
        {
            if (ID <= minValue || ID > highestInUse || (checkForReRelease && avaliableIDs.Contains(ID)))
            {
                throw new ArgumentOutOfRangeException("ID", string.Format("The ID {0} has already been released or was never generated.", ID));
            }
        }
        private void CheckAvaliableIDs()
        {
            if (avaliableIDs.Count > highestInUse)
            {
                avaliableIDs = null;
                throw new InvalidOperationException("The IDManager is corrupted. There has been more Releases then Generations. You should set CheckForReRelease to true next time.");
            }
        }
        private void CheckState()
        {
            if (avaliableIDs == null)
            {
                throw new InvalidOperationException("The IDManager is corrupted. There has been more Releases then Generations. You should set CheckForReRelease to true next time.");
            }
        }
        #endregion
    }
}