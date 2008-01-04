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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Tao.OpenGl;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.OpenGl;
namespace Graphics2DDotNet
{
    public sealed class Colored3MultiVertexesDrawable : BufferedDrawable
    {
        int[] vertexNames;
        int[] colorNames;
        Vector2D[][] polygon;
        ScalarColor3[][] colors;
        int mode;
        public Colored3MultiVertexesDrawable(int mode,Vector2D[][] polygon, ScalarColor3[][] colors)
        {
            if (polygon == null) { throw new ArgumentNullException("vertexes"); }
            if (colors.Length != polygon.Length) { throw new ArgumentException("TODO length !="); }
            this.polygon = polygon;
            this.colors = colors;
            this.vertexNames = new int[polygon.Length];
            this.colorNames = new int[colors.Length];
            this.mode = mode;
        }
        protected override void BufferData()
        {
            Gl.glGenBuffersARB(vertexNames.Length, vertexNames);
            Gl.glGenBuffersARB(colorNames.Length, colorNames);
            for (int index = 0; index < polygon.Length; ++index)
            {
                Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, vertexNames[index]);
                GlHelper.GlBufferDataARB(
                    Gl.GL_ARRAY_BUFFER_ARB,
                    polygon[index],
                    polygon[index].Length * Vector2D.Size,
                    Gl.GL_STATIC_DRAW_ARB);

                Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, colorNames[index]);
                GlHelper.GlBufferDataARB(
                    Gl.GL_ARRAY_BUFFER_ARB,
                    colors[index],
                    colors[index].Length * ScalarColor3.Size,
                    Gl.GL_STATIC_DRAW_ARB);
            }
        }
        protected override void DrawData(DrawInfo drawInfo, IDrawableState state)
        {
            for (int index = 0; index < polygon.Length; ++index)
            {
                Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, vertexNames[index]);
                Gl.glVertexPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);

                Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, colorNames[index]);
                Gl.glColorPointer(ScalarColor3.Count, GlHelper.GlScalar, 0, IntPtr.Zero);

                Gl.glDrawArrays(mode, 0, polygon[index].Length);
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
            GlHelper.GlDeleteBuffersARB(LastRefresh, vertexNames);
            GlHelper.GlDeleteBuffersARB(LastRefresh, colorNames);
        }
    }

}