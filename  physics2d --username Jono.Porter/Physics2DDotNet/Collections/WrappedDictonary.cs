#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
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
    public class WrappedDictionary<TKey, TValue, TDictionary> : WrappedCollection<KeyValuePair<TKey, TValue>, TDictionary>, IDictionary<TKey, TValue>
        where TDictionary : IDictionary<TKey, TValue>
    {
        public WrappedDictionary(TDictionary self) : base(self) { }
        public WrappedDictionary(TDictionary self, AdvReaderWriterLock selfLock) : base(self, selfLock) { }

        public ICollection<TKey> Keys
        {
            get
            {
                using (Lock.Read)
                {
                    return new WrappedCollection<TKey, ICollection<TKey>>(This.Keys,Lock);
                }
            }
        }
        public ICollection<TValue> Values
        {
            get
            {
                using (Lock.Read)
                {
                    return new WrappedCollection<TValue, ICollection<TValue>>(This.Values, Lock);
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                This.Add(key, value);
            }
        }
        public bool ContainsKey(TKey key)
        {
            using (Lock.Read)
            {
                return This.ContainsKey(key);
            }
        }
        public bool Remove(TKey key)
        {
            if (isReadOnlyWrapper) { ThrowReadOnly(); }
            using (Lock.Write)
            {
                return This.Remove(key);
            }
        }
        public bool TryGetValue(TKey key, out TValue value)
        {
            using (Lock.Read)
            {
                return This.TryGetValue(key, out value);
            }
        }
        public TValue this[TKey key]
        {
            get
            {
                using (Lock.Read)
                {
                    return This[key];
                }
            }
            set
            {
                using (Lock.Write)
                {
                    This[key] = value;
                }
            }
        }
    }
    public class DictionaryWrapper<TKey, TValue> : WrappedDictionary<TKey, TValue, IDictionary<TKey, TValue>>
    {
        public DictionaryWrapper(IDictionary<TKey, TValue> self) : base(self) { }
        public DictionaryWrapper(IDictionary<TKey, TValue> self, AdvReaderWriterLock selfLock) : base(self, selfLock) { }
    }

}