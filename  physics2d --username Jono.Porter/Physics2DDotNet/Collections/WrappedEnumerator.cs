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



    public sealed class WrappedEnumerator<T> : IEnumerator<T>

    {
        ReaderLock readLock;
        IEnumerator<T> self;

        public WrappedEnumerator(ReaderLock readLock, IEnumerator<T> self)
        {
            if (self == null) { throw new ArgumentNullException("self"); }
            if (readLock == null) { throw new ArgumentNullException("readLock"); }
            this.readLock = readLock;
            this.self = self;
        }

        public IEnumerator<T> Self
        {
            get { return self; }
        }
        object IEnumerator.Current
        {
            get { return ((IEnumerator)self).Current; }
        }
        public T Current
        {
            get { return self.Current; }
        }

        public void Dispose()
        {
            self.Dispose();
            readLock.Release();
        }
        public bool MoveNext()
        {
            return self.MoveNext();
        }
        public void Reset()
        {
            self.Reset();
        }
    }
}