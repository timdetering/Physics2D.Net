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
    public sealed class Colored3MultiVertexesDrawable : BufferedDrawable
    {
        MultiARBArrayBuffer<ScalarColor3> colors;
        MultiARBArrayBuffer<Vector2D> vertexes;
        int mode;
        public Colored3MultiVertexesDrawable(int mode, Vector2D[][] vertexes, ScalarColor3[][] colors)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (colors == null) { throw new ArgumentNullException("colors"); }
            if (colors.Length != vertexes.Length) { throw new ArgumentException("TODO length !="); }
            this.vertexes = new MultiARBArrayBuffer<Vector2D>(vertexes, Vector2D.Size);
            this.colors = new MultiARBArrayBuffer<ScalarColor3>(colors, ScalarColor3.Size);
            this.mode = mode;
        }
        protected override void BufferData(int refresh)
        {
            vertexes.Buffer(refresh);
            colors.Buffer(refresh);
        }
        protected override void DrawData(DrawInfo drawInfo, IDrawableState state)
        {
            for (int index = 0; index < vertexes.Count; ++index)
            {
                vertexes.Bind(index);
                Gl.glVertexPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);
                colors.Bind(index);
                Gl.glColorPointer(ScalarColor3.Count, GlHelper.GlScalar, 0, IntPtr.Zero);
                Gl.glDrawArrays(mode, 0, vertexes.GetLength(index));
            }
        }
        protected override void EnableState()
        {
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_COLOR_ARRAY);
        }
        protected override void DisableState()
        {
            Gl.glDisableClientState(Gl.GL_COLOR_ARRAY);
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
                vertexes.Dispose();
                colors.Dispose();
            }
        }
    }



}