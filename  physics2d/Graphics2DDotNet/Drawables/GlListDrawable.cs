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
using Tao.OpenGl;
namespace Graphics2DDotNet
{
    public sealed class GlListDrawable : IDrawable, IDisposable
    {
        object tag;
        int list;
        int refresh;
        bool isDisposed;
        IDrawable drawable;
        BufferedDrawable bufferedDrawable;
        public GlListDrawable(IDrawable drawable)
        {
            if (drawable == null) { throw new ArgumentNullException("drawable"); }
            this.refresh = -1;
            this.drawable = drawable;
            this.bufferedDrawable = drawable as BufferedDrawable;
        }
        ~GlListDrawable()
        {
            Dispose(false);
        }
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public bool IsDisposed
        {
            get { return isDisposed; }
        }
        public int LastRefresh
        {
            get { return refresh; }
        }
        public IDrawable Drawable
        {
            get { return drawable; }
        }
        public IDrawableState CreateState()
        {
            return drawable.CreateState();
        }
        public void Draw(DrawInfo drawInfo, IDrawableState state)
        {
            if (isDisposed) { throw new ObjectDisposedException(this.ToString()); }
            if (refresh != drawInfo.RefreshCount)
            {
                refresh = drawInfo.RefreshCount;
                list = Gl.glGenLists(1);
                if (bufferedDrawable != null)
                {
                    bufferedDrawable.TestBuffer(drawInfo);
                }
                Gl.glNewList(list, Gl.GL_COMPILE);
                drawable.Draw(drawInfo, state);
                Gl.glEndList();
            }
            Gl.glCallList(list);
        }
        private void Dispose(bool disposing)
        {
            GlHelper.GlDeleteLists(refresh, list, 1);
        }
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}