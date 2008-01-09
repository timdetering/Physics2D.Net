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
using System.Runtime.InteropServices;
using AdvanceMath;
using Tao.OpenGl;
using SdlDotNet.Graphics;
namespace Graphics2DDotNet
{
    public sealed class MultiARBArrayBuffer<T> : IDisposable
    {
        T[][] array;
        int[] bufferNames;
        int refresh;
        int elementSize;
        public MultiARBArrayBuffer(T[][] array, int elementSize)
        {
            if (array == null) { throw new ArgumentNullException("array"); }
            this.array = array;
            this.bufferNames = new int[array.Length];
            this.elementSize = elementSize;
            this.refresh = -1;
        }
        ~MultiARBArrayBuffer()
        {
            Dispose(false);
        }
        public int Count { get { return array.Length; } }
        public void Buffer(int refresh)
        {
            Gl.glGenBuffersARB(array.Length, bufferNames);
            for (int index = 0; index < array.Length; ++index)
            {
                Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, bufferNames[index]);
                GlHelper.GlBufferDataARB(
                    Gl.GL_ARRAY_BUFFER_ARB,
                    array[index],
                    array[index].Length * elementSize,
                    Gl.GL_STATIC_DRAW_ARB);
            }
        }
        public void Bind(int index)
        {
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, bufferNames[index]);
        }
        public int GetLength(int index) { return array[index].Length; }
        private void Dispose(bool disposing)
        {
            GlHelper.GlDeleteBuffersARB(refresh, bufferNames);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}