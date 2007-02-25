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
    public class ThreadSafeDictionary<TKey, TValue> : ThreadSafeDictionary<TKey, TValue, Dictionary<TKey, TValue>>
    {
        public ThreadSafeDictionary()
            : base(new Dictionary<TKey, TValue>()) { }
        public ThreadSafeDictionary(IDictionary<TKey, TValue> dictionary)
            : base(new Dictionary<TKey, TValue>(dictionary)) { }
        public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TValue>(comparer)) { }
        public ThreadSafeDictionary(int capacity)
            : base(new Dictionary<TKey, TValue>(capacity)) { }
        public ThreadSafeDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TValue>(dictionary, comparer)) { }
        public ThreadSafeDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TValue>(capacity, comparer)) { }
    }
    public class ThreadSafeDictionary<TKey, TValue, TDictionary> : WrappedDictionary<TKey, TValue, TDictionary>
        where TDictionary : Dictionary<TKey, TValue>
    {
        public ThreadSafeDictionary(TDictionary self) : base(self) { }
        public ThreadSafeDictionary(TDictionary self, AdvReaderWriterLock selfLock) : base(self, selfLock) { }

        public IEqualityComparer<TKey> Comparer
        {
            get
            {
                using (Lock.Read)
                {
                    return This.Comparer;
                }
            }
        }
        public bool ContainsValue(TValue value)
        {
            using (Lock.Read)
            {
                return This.ContainsValue(value);
            }
        }
    }
}