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
using System.Collections.Generic;
using System.Threading;
using System.IO;
namespace Physics2DDotNet.Collections
{
    public sealed class AdvReaderWriterLock
    {
        ReaderWriterLock readerWriterLock;
        public AdvReaderWriterLock() : this(new ReaderWriterLock()) { }
        public AdvReaderWriterLock(ReaderWriterLock readerWriterLock)
        {
            this.readerWriterLock = readerWriterLock;
        }
        public ReaderLock Read
        {
            get
            {
                return new ReaderLock(readerWriterLock);
            }
        }
        public WriterLock Write
        {
            get
            {
                return new WriterLock(readerWriterLock);
            }
        }
    }
    public sealed class ReaderLock : IDisposable
    {
        private bool isReleased;
        internal ReaderWriterLock readerWriterLock;
        internal ReaderLock(ReaderWriterLock readerWriterLock)
        {
            this.readerWriterLock = readerWriterLock;
            readerWriterLock.AcquireReaderLock(Timeout.Infinite);
        }
        public WriterLock ToWrite
        {
            get
            {
                if (isReleased)
                {
                    throw new InvalidOperationException();
                }
                return new WriterLock(this);
            }
        }
        public bool IsReleased
        {
            get { return isReleased; }
        }
        public void Release()
        {
            if (!isReleased)
            {
                readerWriterLock.ReleaseReaderLock();
                isReleased = true;
            }
        }
        void IDisposable.Dispose()
        {
            Release();
        }
    }
    public sealed class WriterLock : IDisposable
    {
        bool isReleased;
        internal ReaderWriterLock readerWriterLock;
        bool isUpgrade;
        LockCookie cookie;
        internal WriterLock(ReaderWriterLock readerWriterLock)
        {
            this.readerWriterLock = readerWriterLock;
            readerWriterLock.AcquireWriterLock(Timeout.Infinite);
        }
        internal WriterLock(ReaderLock readLock)
        {
            this.isUpgrade = true;
            this.readerWriterLock = readLock.readerWriterLock;
            this.cookie = readerWriterLock.UpgradeToWriterLock(Timeout.Infinite);
        }
        public bool IsUpgrade
        {
            get { return isUpgrade; }
        }
        public bool IsReleased
        {
            get { return isReleased; }
        }
        public void Release()
        {
            if (!isReleased)
            {
                if (isUpgrade)
                {
                    readerWriterLock.DowngradeFromWriterLock(ref cookie);
                }
                else
                {
                    readerWriterLock.ReleaseWriterLock();
                }
                isReleased = true;
            }
        }
        void IDisposable.Dispose()
        {
            Release();
        }
    }
}