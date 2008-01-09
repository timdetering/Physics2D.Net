#region MIT License
/*
 * Copyright (c) 2005-2008 Jonathan Mark Porter. http://physics2d.googlepages.com/
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

#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;

namespace Graphics2DDotNet
{
    public abstract class BufferedDrawable : IDrawable, IDisposable
    {
        object tag;
        int refresh;
        public int LastRefresh
        {
            get { return refresh; }
            protected set { refresh = value; }
        }
        bool isDisposed;
        protected BufferedDrawable()
        {
            this.refresh = -1;
        }
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        protected abstract void EnableState();
        protected abstract void DisableState();
        protected abstract void BufferData(int refresh);
        protected abstract void DrawData(DrawInfo drawInfo, IDrawableState state);
        public void Draw(DrawInfo drawInfo, IDrawableState state)
        {
            if (isDisposed) { throw new ObjectDisposedException(this.ToString()); }
            EnableState();
            if (refresh != drawInfo.RefreshCount)
            {
                refresh = drawInfo.RefreshCount;
                BufferData(drawInfo.RefreshCount);
            }
            DrawData(drawInfo, state);
            DisableState();
        }
        public abstract IDrawableState CreateState();
        protected abstract void Dispose(bool disposing);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            isDisposed = true;
        }
    }

}