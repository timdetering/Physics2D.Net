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
using AdvanceMath;
using Tao.OpenGl;
namespace Graphics2DDotNet
{
    public sealed class MultiVertexesDrawable : BufferedDrawable
    {
        MultiARBArrayBuffer<Vector2D> buffer;
        int mode;
        public MultiVertexesDrawable(int mode,Vector2D[][] polygon)
        {
            if (polygon == null) { throw new ArgumentNullException("vertexes"); }
            this.buffer = new MultiARBArrayBuffer<Vector2D>(polygon, Vector2D.Size);
            this.mode = mode;
        }

        protected override void BufferData(int refresh)
        {
            buffer.Buffer(refresh);
        }
        protected override void DrawData(DrawInfo drawInfo, IDrawableState state)
        {
            for (int index = 0; index < buffer.Count; ++index)
            {
                buffer.Bind(index);
                Gl.glVertexPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);
                Gl.glDrawArrays(mode, 0, buffer.GetLength(index));
            }
        }
        protected override void EnableState()
        {
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
        }
        protected override void DisableState()
        {
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);
        }
        public override IDrawableState CreateState()
        {
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                buffer.Dispose();
            }
        }
    }
}