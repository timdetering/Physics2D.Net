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
using System.Runtime.InteropServices;
using AdvanceMath;
using Physics2DDotNet;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.OpenGl;
using Tao.OpenGl;
using Tao.Sdl;
using Color = System.Drawing.Color;


namespace Physics2DDemo
{
    class Sprite
    {
        Surface surface;
        SurfaceGl texture;
        Vector2D offset;
        Vector2D[] vertexes;
        public Sprite(string path)
        {
            surface = new Surface(path);
            texture = new SurfaceGl(surface, true); 
            Color blank = surface.TransparentColor;
            bool[,] bitmap = new bool[surface.Width, surface.Height];
            for (int x = 0; x < bitmap.GetLength(0); ++x)
            {
                for (int y = 0; y < bitmap.GetLength(1); ++y)
                {
                    Color t = surface.GetPixel(new System.Drawing.Point(x, y));
                    bitmap[x, y] = t.ToArgb() != blank.ToArgb();
                }
            }
            vertexes = Polygon.CreateFromBitmap(bitmap);
            offset = Polygon.GetCentroid(vertexes);
            vertexes = Polygon.MakeCentroidOrigin(vertexes);
        }
        public Vector2D[] Vertexes
        {
            get { return vertexes; }
        }
        public void Draw()
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glColor4f(1, 1, 1, 1);
            Gl.glTranslatef(-offset.X, -offset.Y, 0);
            texture.Draw();
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_BLEND);
        }
        public void Refresh()
        {
            texture.Refresh();
        }
    }


    class OpenGlObject : IDisposable
    {

        public bool collided = true;
        public bool shouldDraw = true;
        float[] matrix = new float[16];
        Body entity;
        int list = -1;
        bool removed;
        public bool Removed
        {
            get { return removed; }
        }


        public OpenGlObject(Body entity)
        {
            this.entity = entity;
            this.entity.StateChanged += entity_NewState;
            this.entity.Removed += entity_Removed;
        }
        void entity_Removed(object sender, RemovedEventArgs e)
        {
            this.entity.StateChanged -= entity_NewState;
            this.entity.Removed -= entity_Removed;
            this.removed = true;
        }
        void entity_NewState(object sender, EventArgs e)
        {
            Matrix3x3 mat = entity.Shape.Matrix.VertexMatrix;
            Matrix3x3.Copy2DToOpenGlMatrix(ref mat, matrix);
        }
        public void Invalidate()
        {
            list = -1;
        }
        void DrawInternal()
        {
            if (entity.Shape.Tag is Sprite)
            {
                Sprite s = (Sprite)entity.Shape.Tag;
                s.Draw();
            }
            else if (entity.Shape is Physics2DDotNet.Particle)
            {
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glColor3f(1, 0, 0);
                foreach (Vector2D vector in entity.Shape.OriginalVertices)
                {
                    Gl.glVertex2f((float)vector.X, (float)vector.Y);
                }
                Gl.glEnd();
            }
            else if (entity.Shape is Physics2DDotNet.Line)
            {
                Physics2DDotNet.Line line = (Physics2DDotNet.Line)entity.Shape;
                Gl.glLineWidth((float)line.Thickness);
                Gl.glColor3f(0, 0, 1);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                foreach (Vector2D vector in entity.Shape.OriginalVertices)
                {
                    Gl.glVertex2f((float)vector.X, (float)vector.Y);
                }
                Gl.glEnd();
            }
            else
            {
                Gl.glBegin(Gl.GL_POLYGON);
                bool first = true;
                bool second = true;
                foreach (Vector2D vector in entity.Shape.OriginalVertices)
                {
                    if (first)
                    {
                        Gl.glColor3f(1, .5f, 0);
                        first = false;
                    }
                    else if (second)
                    {
                        Gl.glColor3f(1, 1, 1);
                        second = false;
                    }
                    Gl.glVertex2f((float)vector.X, (float)vector.Y);
                }
                Gl.glEnd();
            }
        }
        public void Draw()
        {
            if (entity.Lifetime.IsExpired || !shouldDraw)
            {
                return;
            }
            if (Gl.glIsList(list) == 0)
            {
                Gl.glLoadIdentity();
                list = Gl.glGenLists(1);
                Gl.glNewList(list, Gl.GL_COMPILE);
                DrawInternal();
                Gl.glEndList();
            }
            Gl.glLoadMatrixf(matrix);
            Gl.glCallList(list);
        }
        public void Dispose()
        {
            if (Gl.glIsList(list) != 0)
            {
                Gl.glDeleteLists(list, 1);
            }
            list = -1;
        }
    }
}