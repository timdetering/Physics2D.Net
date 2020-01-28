#region LGPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace AdvanceSystem
{
    /// <summary>
    /// Contains all the static methods for SubArray equivilant to the static methods for Array.
    /// </summary>
    /// <remarks>just needs to copy all the comments from array</remarks>
    public static class SubArray
    {
        #region Sort
        public static void Sort<T>(SubArray<T> subArray)
        {
            Array.Sort(subArray.array, subArray.offset, subArray.length);
        }


        public static void Sort<TKey, TValue>(SubArray<TKey> keys, SubArray<TValue> items)
        {
            Sort<TKey, TValue>(keys, items, 0, keys.length);
        }
        public static void Sort<TKey, TValue>(SubArray<TKey> keys, SubArray<TValue> items, int index, int length)
        {
            TKey[] keysarray = AsArray<TKey>(keys);
            if (items == null)
            {
                Array.Sort<TKey, TValue>(keysarray, null, index, length);
            }
            else
            {
                TValue[] itemsarray = AsArray<TValue>(items);
                Array.Sort<TKey, TValue>(keysarray, itemsarray, index, length);
                Copy<TValue, TValue>(itemsarray, items);
            }
            Copy<TKey, TKey>(keysarray, keys);
        }
        public static void Sort<TKey, TValue>(SubArray<TKey> keys, SubArray<TValue> items, IComparer<TKey> comparer)
        {
            Sort<TKey, TValue>(keys, items, 0, keys.length, comparer);
        }
        public static void Sort<TKey, TValue>(SubArray<TKey> keys, SubArray<TValue> items, int index, int length, IComparer<TKey> comparer)
        {
            TKey[] keysarray = AsArray<TKey>(keys);
            if (items == null)
            {
                Array.Sort<TKey, TValue>(keysarray, null, index, length, comparer);
            }
            else
            {
                TValue[] itemsarray = AsArray<TValue>(items);
                Array.Sort<TKey, TValue>(keysarray, itemsarray, index, length, comparer);
                Copy<TValue, TValue>(itemsarray, items);
            }
            Copy<TKey, TKey>(keysarray, keys);
        }


        public static void Sort<TKey, TValue>(TKey[] keys, SubArray<TValue> items)
        {
            Sort<TKey, TValue>(keys, items, 0, keys.Length);
        }
        public static void Sort<TKey, TValue>(TKey[] keys, SubArray<TValue> items, int index, int length)
        {
            if (items == null)
            {
                Array.Sort<TKey, TValue>(keys, null, index, length);
            }
            else
            {
                TValue[] itemsarray = AsArray<TValue>(items);
                Array.Sort<TKey, TValue>(keys, itemsarray, index, length);
                Copy<TValue, TValue>(itemsarray, items);
            }
        }
        public static void Sort<TKey, TValue>(TKey[] keys, SubArray<TValue> items, IComparer<TKey> comparer)
        {
            Sort<TKey, TValue>(keys, items, 0, keys.Length, comparer);
        }
        public static void Sort<TKey, TValue>(TKey[] keys, SubArray<TValue> items, int index, int length, IComparer<TKey> comparer)
        {
            if (items == null)
            {
                Array.Sort<TKey, TValue>(keys, null, index, length, comparer);
            }
            else
            {
                TValue[] itemsarray = AsArray<TValue>(items);
                Array.Sort<TKey, TValue>(keys, itemsarray, index, length, comparer);
                Copy<TValue, TValue>(itemsarray, items);
            }
        }


        public static void Sort<TKey, TValue>(SubArray<TKey> keys, TValue[] items)
        {
            Sort<TKey, TValue>(keys, items, 0, keys.length);
        }
        public static void Sort<TKey, TValue>(SubArray<TKey> keys, TValue[] items, int index, int length)
        {
            TKey[] keysarray = AsArray<TKey>(keys);
            Array.Sort<TKey, TValue>(keysarray, items, index, length);
            Copy<TKey, TKey>(keysarray, keys);
        }
        public static void Sort<TKey, TValue>(SubArray<TKey> keys, TValue[] items, IComparer<TKey> comparer)
        {
            Sort<TKey, TValue>(keys, items, 0, keys.length, comparer);
        }
        public static void Sort<TKey, TValue>(SubArray<TKey> keys, TValue[] items, int index, int length, IComparer<TKey> comparer)
        {
            TKey[] keysarray = AsArray<TKey>(keys);
            Array.Sort<TKey, TValue>(keysarray, items, index, length, comparer);
            Copy<TKey, TKey>(keysarray, keys);
        }
        #endregion
        #region IndexOf
        public static int IndexOf<T>(SubArray<T> subArray, T value)
        {
            return IndexOf<T>(subArray, value, 0);
        }
        public static int IndexOf<T>(SubArray<T> subArray, T value, int startindex)
        {
            return IndexOf<T>(subArray, value, startindex, subArray.length - startindex);
        }
        public static int IndexOf<T>(SubArray<T> subArray, T value, int startindex, int count)
        {
            if (startindex + count >= subArray.length || startindex < 0)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }
            return Array.IndexOf(subArray.array, value, subArray.offset + startindex, count);
        }
        public static int LastIndexOf<T>(SubArray<T> subArray, T value)
        {
            return LastIndexOf<T>(subArray, value, 0);
        }
        public static int LastIndexOf<T>(SubArray<T> subArray, T value, int startindex)
        {
            return LastIndexOf<T>(subArray, value, startindex, subArray.length - startindex);
        }
        public static int LastIndexOf<T>(SubArray<T> subArray, T value, int startindex, int count)
        {
            if (startindex + count >= subArray.length || startindex < 0)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }
            return Array.LastIndexOf(subArray.array, value, subArray.offset + startindex, count);
        }
        #endregion
        #region Copy
        #region SubArray To SubArray
        public static void Copy<TSource, TDest>(SubArray<TSource> sourceSubArray, SubArray<TDest> destinationSubArray)
            where TSource : TDest
        {
            Copy<TSource, TDest>(sourceSubArray, destinationSubArray, sourceSubArray.length);
        }
        public static void Copy<TSource, TDest>(SubArray<TSource> sourceSubArray, SubArray<TDest> destinationSubArray, int length)
            where TSource : TDest
        {
            Array.Copy(sourceSubArray.array, sourceSubArray.offset, destinationSubArray.array, destinationSubArray.offset, length);
        }
        #endregion

        #region SubArray To Array
        public static void Copy<TSource, TDest>(SubArray<TSource> sourceSubArray, TDest[] destinationArray)
            where TSource : TDest
        {
            Copy<TSource, TDest>(sourceSubArray, destinationArray, 0, sourceSubArray.length);
        }
        public static void Copy<TSource, TDest>(SubArray<TSource> sourceSubArray, TDest[] destinationArray, int length)
            where TSource : TDest
        {
            Copy<TSource, TDest>(sourceSubArray, destinationArray, 0, length);
        }
        public static void Copy<TSource, TDest>(SubArray<TSource> sourceSubArray, TDest[] destinationArray, int destinationIndex, int length)
            where TSource : TDest
        {
            Array.Copy(sourceSubArray.array, sourceSubArray.offset, destinationArray, destinationIndex, length);
        }
        #endregion

        #region Array To SubArray
        public static void Copy<TSource, TDest>(TSource[] sourceArray, SubArray<TDest> destinationSubArray)
            where TSource : TDest
        {
            Copy<TSource, TDest>(sourceArray, 0, destinationSubArray, sourceArray.Length);
        }
        public static void Copy<TSource, TDest>(TSource[] sourceArray, SubArray<TDest> destinationSubArray, int length)
            where TSource : TDest
        {
            Copy<TSource, TDest>(sourceArray, 0, destinationSubArray, length);
        }
        public static void Copy<TSource, TDest>(TSource[] sourceArray, int sourceIndex, SubArray<TDest> destinationSubArray, int length)
            where TSource : TDest
        {
            Array.Copy(sourceArray, sourceIndex, destinationSubArray.array, destinationSubArray.length, length);
        }
        #endregion
        #endregion
        #region BinarySearch
        public static int BinarySearch<T>(SubArray<T> subArray, T value)
        {

            int result = Array.BinarySearch<T>(subArray.array, subArray.offset, subArray.length, value);
            if (result < 0)
            {
                return result + subArray.offset;
            }
            else
            {
                return result - subArray.offset;
            }
        }
        public static int BinarySearch<T>(SubArray<T> subArray, T value, IComparer<T> comparer)
        {
            int result = Array.BinarySearch<T>(subArray.array, subArray.offset, subArray.length, value, comparer);
            if (result < 0)
            {
                return result + subArray.offset;
            }
            else
            {
                return result - subArray.offset;
            }
        }
        #endregion
        #region Find
        public static T Find<T>(SubArray<T> subArray, Predicate<T> match)
        {
            int index = Array.FindIndex(subArray.array, subArray.offset, subArray.length, match);
            if (index == -1)
            {
                return default(T);
            }
            else
            {
                return subArray.array[index - subArray.offset];
            }
        }
        public static int FindIndex<T>(SubArray<T> subArray, Predicate<T> match)
        {
            int index = Array.FindIndex(subArray.array, subArray.offset, subArray.length, match);
            if (index == -1)
            {
                return index;
            }
            else
            {
                return index - subArray.offset;
            }
        }
        public static T FindLast<T>(SubArray<T> subArray, Predicate<T> match)
        {
            int index = Array.FindLastIndex(subArray.array, subArray.offset, subArray.length, match);
            if (index == -1)
            {
                return default(T);
            }
            else
            {
                return subArray.array[index - subArray.offset];
            }
        }
        public static int FindLastIndex<T>(SubArray<T> subArray, Predicate<T> match)
        {

            int index = Array.FindLastIndex(subArray.array, subArray.offset, subArray.length, match);
            if (index == -1)
            {
                return index;
            }
            else
            {
                return index - subArray.offset;
            }
        }
        #endregion
        #region Other
        public static T[] FindAll<T>(SubArray<T> subArray, Predicate<T> match)
        {
            List<T> returnvalue = new List<T>();
            int endpos = subArray.offset + subArray.length;
            for (int pos = subArray.offset; pos < endpos; ++pos)
            {
                if (match(subArray.array[pos]))
                {
                    returnvalue.Add(subArray.array[pos]);
                }
            }
            return returnvalue.ToArray();
            //return Array.FindAll<T>(CreateFrom<T>(subArray), match);
        }

        public static void Clear<T>(SubArray<T> subArray)
        {
            Array.Clear(subArray.array, subArray.offset, subArray.length);
        }
        public static void Reverse<T>(SubArray<T> subArray)
        {
            Array.Reverse(subArray.array, subArray.offset, subArray.length);
        }
        public static void ForEach<T>(SubArray<T> subArray, Action<T> action)
        {
            int endpos = subArray.offset + subArray.length;
            for (int pos = subArray.offset; pos < endpos; ++pos)
            {
                action(subArray.array[pos]);
            }
            //T[] array = CreateFrom<T>(subArray);
            //Array.ForEach(array, action);
            //Copy<T, T>(array, subArray);
        }
        public static bool Exists<T>(SubArray<T> subArray, Predicate<T> match)
        {
            int endpos = subArray.offset + subArray.length;
            for (int pos = subArray.offset; pos < endpos; ++pos)
            {
                if (match(subArray.array[pos]))
                {
                    return true;
                }
            }
            return false;
            //return Array.Exists<T>(CreateFrom<T>(subArray), match);
        }
        public static bool TrueForAll<T>(SubArray<T> subArray, Predicate<T> match)
        {
            bool returnvalue = true;
            int endpos = subArray.offset + subArray.length;
            for (int pos = subArray.offset; pos < endpos; ++pos)
            {
                returnvalue = returnvalue && match(subArray.array[pos]);
            }
            return returnvalue;
            //return Array.TrueForAll(CreateFrom<T>(subArray), match);
        }

        /// <summary>
        /// Creates a Array From the SubArray.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static T[] AsArray<T>(SubArray<T> segment)
        {
            T[] returnvalue = new T[segment.length];
            Copy<T, T>(segment, returnvalue);
            return returnvalue;
        }
        public static T[] AsArray<T>(SubArray<T> segment, int length)
        {
            T[] returnvalue = new T[segment.length];
            Copy<T, T>(segment, returnvalue, length);
            return returnvalue;
        }
        public static ReadOnlyCollection<T> AsReadonly<T>(SubArray<T> subArray)
        {
            return new ReadOnlyCollection<T>(subArray);
        }
        #endregion
    }
    /// <summary>
    /// A generic class taht represents a Segment of a Array AKA SubArray.
    /// </summary>
    /// <remarks>just needs to copy all the comments from array</remarks>
    [Serializable]
    public sealed class SubArray<T> : ICloneable, IEnumerable<T>, ICollection<T>, IList<T>
    {
        #region static Methods
        private static void ThrowErrors(T[] array, int offset, int length)
        {
            if (offset < 0 || offset > array.Length)
            {
                throw new ArgumentOutOfRangeException("offset", "Index was out of range. Must be non-negative and less than the size of the collection.");
            }
            if (length < 0 || length + offset > array.Length)
            {
                throw new ArgumentOutOfRangeException("offset", "Length must be positive and Length must refer to a location within the string/array/collection.");
            }
        }
        #endregion
        #region fields
        internal T[] array;
        internal int offset;
        internal int length;
        #endregion
        #region constructors
        public SubArray(T[] array) : this(array, 0, array.Length) { }
        public SubArray(T[] array, int offset) : this(array, offset, array.Length - offset) { }
        public SubArray(T[] array, int offset, int length)
        {
            ThrowErrors(array, offset, length);
            this.array = array;
            this.offset = offset;
            this.length = length;
        }
        #endregion
        #region Properties
        public bool IsReadOnly
        {
            get { return array.IsReadOnly; }
        }
        public T[] Array
        {
            get
            {
                return array;
            }
        }
        public int Offset
        {
            get { return offset; }
            set
            {
                ThrowErrors(array, value, length);
                offset = value;
            }
        }
        public int Length
        {
            get { return length; }
            set
            {
                ThrowErrors(array, offset, value);
                length = value;
            }
        }
        int ICollection<T>.Count
        {
            get { return length; }
        }
        #endregion
        #region Indexers
        public T this[int index]
        {
            get
            {
                if (index > length || index < 0)
                {
                    throw new IndexOutOfRangeException("Index was outside the bounds of the array.");
                }
                return array[index + offset];
            }
            set
            {
                if (index > length || index < 0)
                {
                    throw new IndexOutOfRangeException("Index was outside the bounds of the array.");
                }
                array[index + offset] = value;
            }
        }
        #endregion
        #region Methods

        public T[] ToArray()
        {
            return SubArray.AsArray<T>(this);
        }
        public void CopyTo(T[] array)
        {
            SubArray.Copy<T, T>(this, array);
        }
        public void CopyTo(T[] array, int index)
        {
            SubArray.Copy<T, T>(this, array, index, length);
        }
        public void CopyTo(T[] array, int index, int length)
        {
            SubArray.Copy<T, T>(this, array, index, length);
        }
        public void CopyTo(SubArray<T> subArray)
        {
            SubArray.Copy<T, T>(this, subArray);
        }
        public object Clone()
        {
            return new SubArray<T>(this.array, this.length, this.offset);
        }

        public IEnumerator<T> GetEnumerator()
        {
            int endpos = offset + length;
            for (int pos = offset; pos < endpos; ++pos)
            {
                yield return array[pos];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        int IList<T>.IndexOf(T item)
        {
            return SubArray.IndexOf(this, item);
        }
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException("Collection was of a fixed size.");
        }
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException("Collection was of a fixed size.");
        }
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException("Collection was of a fixed size.");
        }
        void ICollection<T>.Clear()
        {
            SubArray.Clear(this);
        }
        bool ICollection<T>.Contains(T item)
        {
            return (SubArray.IndexOf(this, item) >= 0);
        }
        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException("Collection was of a fixed size.");
        }
        #endregion
    }
}