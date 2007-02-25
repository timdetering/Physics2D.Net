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
    public class WrappedEnumerable<T, TEnumerable> : IEnumerable<T>
       where TEnumerable : IEnumerable<T>
    {
        AdvReaderWriterLock selfLock;
        private TEnumerable self;

        public WrappedEnumerable(TEnumerable self)
            : this(self, new AdvReaderWriterLock()) { }
        public WrappedEnumerable(TEnumerable self, AdvReaderWriterLock selfLock)
        {
            if (self == null) { throw new ArgumentNullException("self"); }
            if (selfLock == null) { throw new ArgumentNullException("selfLock"); }
            this.self = self;
            this.selfLock = selfLock;
        }

        /// <summary>
        /// A way to access the collection without locking
        /// </summary>
        protected TEnumerable This
        {
            get { return self; }
        }

        /// <summary>
        /// The AdvReaderWriterLock that control access to this collection.
        /// you can use it in conjuction with This to do you own operations.
        /// </summary>
        protected AdvReaderWriterLock Lock
        {
            get { return selfLock; }
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new WrappedEnumerator<T>(Lock.Read, self.GetEnumerator());
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}