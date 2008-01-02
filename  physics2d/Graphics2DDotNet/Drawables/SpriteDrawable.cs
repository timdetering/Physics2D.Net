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
    public sealed class SpriteDrawable : BufferedDrawable
    {
        ScalarColor4 color;
        Surface surface;
        SurfaceGl texture;
        Vector2D[] vertexes;
        Vector2D[] coordinates;
        int vertexName;
        int coordName;
        public SpriteDrawable(Surface surface, Vector2D[] vertexes, Vector2D[] coordinates)
        {
            this.surface = surface;
            this.vertexes = vertexes;
            this.coordinates = coordinates;
            this.texture = new SurfaceGl(surface);
            this.color = new ScalarColor4(1, 1, 1, 1);
        }
        public ScalarColor4 Color
        {
            get { return color; }
            set { color = value; }
        }

        protected override void EnableState()
        {
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
        }

        protected override void DisableState()
        {
            Gl.glDisable(Gl.GL_BLEND);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
        }

        protected override void BufferData()
        {
            this.texture.Refresh();

            Gl.glGenBuffersARB(1, out vertexName);
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, vertexName);
            GlHelper.GlBufferDataARB(
                Gl.GL_ARRAY_BUFFER_ARB,
                vertexes,
                vertexes.Length * Vector2D.Size,
                Gl.GL_STATIC_DRAW_ARB);

            Gl.glGenBuffersARB(1, out coordName);
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, coordName);
            GlHelper.GlBufferDataARB(
                Gl.GL_ARRAY_BUFFER_ARB,
                coordinates,
                coordinates.Length * Vector2D.Size,
                Gl.GL_STATIC_DRAW_ARB);
        }

        protected override void DrawData(DrawInfo drawInfo, IDrawableState state)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture.TextureId);
            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, vertexName);
            Gl.glVertexPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);

            Gl.glBindBufferARB(Gl.GL_ARRAY_BUFFER_ARB, coordName);
            Gl.glTexCoordPointer(Vector2D.Count, GlHelper.GlScalar, 0, IntPtr.Zero);
            GlHelper.GlColor4(color.Red, color.Green, color.Blue, color.Alpha);
            Gl.glDrawArrays(Gl.GL_POLYGON, 0, vertexes.Length);
        }

        public override IDrawableState CreateState()
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                texture.Dispose();
            }
            GlHelper.GlDeleteBuffersARB(vertexName);
            GlHelper.GlDeleteBuffersARB(coordName);
        }
    }

}