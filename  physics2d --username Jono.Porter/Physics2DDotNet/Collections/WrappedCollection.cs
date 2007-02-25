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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
namespace Physics2DDotNet.Collections
{
    public class WrappedCollection<T, TCollection> : WrappedEnumerable<T, TCollection>, ICollection<T>
        where TCollection : ICollection<T>
    {
        protected bool isReadOnlyWrapper;

        public WrappedCollection(TCollection self) : base(self) { }
        public WrappedCollection(TCollection self, AdvReaderWriterLock selfLock) : base(self, selfLock) { }

        public int Count
        {
            get
            {
                using (Lock.Read)
                {
                    return This.Count;
                }
            }
        }
        public bool IsReadOnly
        {
            get
            {
                using (Lock.Read)
                {
                    return isReadOnlyWrapper ||This.IsReadOnly;
                }
            }
        }

        public void Add(T item)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Add(item);
            }
        }
        public void Clear()
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Clear();
            }
        }
        public bool Contains(T item)
        {
            using (Lock.Read)
            {
                return This.Contains(item);
            }
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            using (Lock.Read)
            {
                This.CopyTo(array, arrayIndex);
            }
        }
        public bool Remove(T item)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                return This.Remove(item);
            }
        }
        protected void ThrowReadOnly()
        {
            throw new InvalidOperationException("Thre collection is ReadOnly");
        }
        public void MakeReadOnly()
        {
            this.isReadOnlyWrapper = true;
        }
    }
    public class CollectionWrapper<T> : WrappedCollection<T, ICollection<T>>
    {
        public CollectionWrapper(ICollection<T> self) : base(self) { }
        public CollectionWrapper(ICollection<T> self, AdvReaderWriterLock selfLock) : base(self, selfLock) { }
    }
}