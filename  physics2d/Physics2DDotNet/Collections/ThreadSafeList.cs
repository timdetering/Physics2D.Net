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



using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
namespace Physics2DDotNet.Collections
{
    public class ThreadSafeList<T> : ThreadSafeList<T, List<T>>
    {
        public ThreadSafeList() : base(new List<T>()) { }
        public ThreadSafeList(IEnumerable<T> collection) : base(new List<T>(collection)) { }
        public ThreadSafeList(int capacity) : base(new List<T>(capacity)) { }
        public ThreadSafeList(List<T> self) : base(self) { }
        public ThreadSafeList(List<T> self, AdvReaderWriterLock selfLock) : base(self, selfLock) { }
    }

    public class ThreadSafeList<T, TList> : WrappedList<T, TList>
        where TList : List<T>
    {
        public ThreadSafeList(TList self) : base(self) { }
        public ThreadSafeList(TList self, AdvReaderWriterLock selfLock) : base(self, selfLock) { }

        public int Capacity
        {
            get
            {
                using (Lock.Read)
                {
                    return This.Capacity;
                }
            }
            set
            {
                using (Lock.Write)
                {
                    This.Capacity = value;
                }
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.AddRange(collection);
            }
        }
        public int BinarySearch(T item)
        {
            using (Lock.Read)
            {
                return This.BinarySearch(item);
            }
        }
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            using (Lock.Read)
            {
                return This.BinarySearch(item, comparer);
            }
        }
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            using (Lock.Read)
            {
                return This.BinarySearch(index, count, item, comparer);
            }
        }
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            using (Lock.Read)
            {
                return This.ConvertAll<TOutput>(converter);
            }
        }
        public void CopyTo(T[] array)
        {
            using (Lock.Read)
            {
                This.CopyTo(array);
            }
        }
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            using (Lock.Read)
            {
                This.CopyTo(index, array, arrayIndex, count);
            }
        }
        public bool Exists(Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.Exists(match);
            }
        }
        public T Find(Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.Find(match);
            }
        }
        public List<T> FindAll(Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.FindAll(match);
            }
        }
        public int FindIndex(Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.FindIndex(match);
            }
        }
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.FindIndex(startIndex, match);
            }
        }
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.FindIndex(startIndex, count, match);
            }
        }
        public T FindLast(Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.FindLast(match);
            }
        }
        public int FindLastIndex(Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.FindLastIndex(match);
            }
        }
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.FindLastIndex(startIndex, match);
            }
        }
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.FindLastIndex(startIndex, count, match);
            }
        }
        public void ForEach(Action<T> action)
        {
            using (Lock.Read)
            {
                This.ForEach(action);
            }
        }
        public List<T> GetRange(int index, int count)
        {
            using (Lock.Read)
            {
                return This.GetRange(index, count);
            }
        }
        public ThreadSafeList<T> GetThreadSafeRange(int index, int count)
        {
            using (Lock.Read)
            {
                return new ThreadSafeList<T>(This.GetRange(index, count));
            }
        }
        public int IndexOf(T item, int index)
        {
            using (Lock.Read)
            {
                return This.IndexOf(item, index);
            }
        }
        public int IndexOf(T item, int index, int count)
        {
            using (Lock.Read)
            {
                return This.IndexOf(item, index, count);
            }
        }
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.InsertRange(index, collection);
            }
        }
        public int LastIndexOf(T item)
        {
            using (Lock.Read)
            {
                return This.LastIndexOf(item);
            }
        }
        public int LastIndexOf(T item, int index)
        {
            using (Lock.Read)
            {
                return This.LastIndexOf(item, index);
            }
        }
        public int LastIndexOf(T item, int index, int count)
        {
            using (Lock.Read)
            {
                return This.LastIndexOf(item, index, count);
            }
        }
        public int RemoveAll(Predicate<T> match)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                return This.RemoveAll(match);
            }
        }
        public void RemoveRange(int index, int count)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.RemoveRange(index, count);
            }
        }
        public void Reverse()
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Reverse();
            }
        }
        public void Reverse(int index, int count)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Reverse(index, count);
            }
        }
        public void Sort()
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Sort();
            }
        }
        public void Sort(IComparer<T> comparer)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Sort(comparer);
            }
        }
        public void Sort(Comparison<T> comparison)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Sort(comparison);
            }
        }
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Sort(index, count, comparer);
            }
        }
        public T[] ToArray()
        {
            using (Lock.Read)
            {
                return This.ToArray();
            }
        }
        public void TrimExcess()
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.TrimExcess();
            }
        }
        public bool TrueForAll(Predicate<T> match)
        {
            using (Lock.Read)
            {
                return This.TrueForAll(match);
            }
        }

    }
}