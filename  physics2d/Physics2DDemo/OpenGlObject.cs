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




using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.PhysicsLogics;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.OpenGl;
using Tao.OpenGl;
using Tao.Sdl;
using Color = System.Drawing.Color;

#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
namespace Physics2DDemo
{
    class Sprite
    {
        Surface surface;
        SurfaceGl texture;


        Vector2D offset;


        Vector2D[][] vertexes;

        public Sprite(string path):this(new Surface(path))
        {

        }
        public Sprite(Surface surface2)
        {
            this.surface = surface2;
            texture = new SurfaceGl(surface, true);
            texture.WrapS = WrapOption.Clamp;
            texture.WrapT = WrapOption.Clamp;
            //texture.MagFilter = MagnificationOption.GL_LINEAR;
            //texture.MinFilter = MinifyingOption.GL_LINEAR_MIPMAP_LINEAR;
            int blank = surface.TransparentColor.ToArgb();
            bool[,] bitmap = new bool[surface.Width, surface.Height];
            Color[,] pixels = surface.GetColors(new System.Drawing.Rectangle(0, 0, surface.Width, surface.Height));

            for (int x = 0; x < bitmap.GetLength(0); ++x)
            {
                for (int y = 0; y < bitmap.GetLength(1); ++y)
                {
                    bitmap[x, y] = !(pixels[x, y].A == 0 || pixels[x, y].ToArgb() == blank);
                }
            }
            vertexes = MultiPolygonShape.CreateFromBitmap(bitmap);
            Console.WriteLine("Before {0}", GetCount);
            vertexes = MultiPolygonShape.Reduce(vertexes, 1);
            vertexes = MultiPolygonShape.Reduce(vertexes, 2);
            vertexes = MultiPolygonShape.Reduce(vertexes, 3);
            Console.WriteLine("After {0}", GetCount);
            vertexes = MultiPolygonShape.Subdivide(vertexes, 10);
            Console.WriteLine("Subdivide {0}", GetCount);
            offset = MultiPolygonShape.GetCentroid(vertexes);
            vertexes = MultiPolygonShape.MakeCentroidOrigin(vertexes);
        }
        private int GetCount
        {
            get
            {
                int result = 0;
                foreach (Vector2D[] array in vertexes)
                {
                    result += array.Length;
                }
                return result;
            }

        }
        public Vector2D Offset
        {
            get { return offset; }
        }
        public SurfaceGl Texture
        {
            get { return texture; }
        }
        public Vector2D[][] Polygons
        {
            get { return vertexes; }
        }
        public void Draw()
        {
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glColor3f(1, 1, 1);
            texture.Draw(-offset.X, -offset.Y);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_BLEND);
        }
        public void Refresh()
        {
            texture.Delete();
        }
    }


    class OpenGlObject : IDisposable
    {
        public static Random rand = new Random();

        public static bool DrawCollisionPoints = false;
        public static bool DrawLinesAndNormalsForSprites = false;
        public static bool DrawBoundingBoxes = false ;

        public bool collided = true;
        public bool shouldDraw = true;

        Scalar[] matrix = new Scalar[16];
        Body entity;
        int list = -1;
        bool removed;

        public bool Removed
        {
            get { return removed; }
        }

        List<Vector2D> points;

        RaySegmentsCollisionLogic logic;

        public OpenGlObject(Body entity)
        {
            this.points = new List<Vector2D>();
            this.entity = entity;
            this.entity.PositionChanged += entity_NewState;
            this.entity.Removed += entity_Removed;
            Matrix2x3 mat = entity.Matrices.ToWorld;
            Matrix2x3.Copy2DToOpenGlMatrix(ref mat, matrix);
            if (entity.Shape is RaySegmentsShape)
            {
                /*RaySegmentsShape se = (RaySegmentsShape)entity.Shape;
                entity.Collided += new EventHandler<CollisionEventArgs>(entity_Collided);
                distances = new Scalar[se.Segments.Length];
                entity.Updated += new EventHandler<UpdatedEventArgs>(entity_Updated);*/
            }
            else if (DrawCollisionPoints && !(entity.Shape is ParticleShape))
            {
                entity.Collided += entity_Collided2;
            }
            entity.ApplyPosition();
        }
        public OpenGlObject(Body entity, RaySegmentsCollisionLogic logic):this(entity)
        {
            this.logic = logic;
        }

        void entity_Collided2(object sender, CollisionEventArgs e)
        {
            lock (points)
            {
                foreach (IContactInfo info in e.Contacts)
                {
                    points.Add(info.Position);
                }
            }
        }


        void entity_Removed(object sender, RemovedEventArgs e)
        {
            this.entity.PositionChanged -= entity_NewState;
            this.entity.Removed -= entity_Removed;
            this.removed = true;
        }
        void entity_NewState(object sender, EventArgs e)
        {
            Matrix2x3 mat = entity.Matrices.ToWorld;
            Matrix2x3.Copy2DToOpenGlMatrix(ref mat, matrix);
        }
        public void Invalidate()
        {
            list = -1;
        }

        void DrawNormal(ref Vector2D vertex1, ref Vector2D vertex2)
        {
            Scalar edgeLength;
            Vector2D tangent, normal;

            Vector2D.Subtract(ref vertex1, ref vertex2, out tangent);
            Vector2D.Normalize(ref tangent, out edgeLength, out tangent);
            Vector2D.GetRightHandNormal(ref tangent, out normal);

            Vector2D pos1 = vertex1 - tangent * (edgeLength * .5f);

            Vector2D pos2 = pos1 + normal * 9;
            Gl.glColor3f(0, 0, 1);
            Gl.glVertex2f(pos1.X, pos1.Y);
            Gl.glColor3f(0, 1, 1);
            Gl.glVertex2f(pos2.X, pos2.Y);
        }
        void DrawInternal()
        {
            if (entity.Shape.Tag is Sprite)
            {
                Sprite s = (Sprite)entity.Shape.Tag;
                s.Draw();
                if (DrawLinesAndNormalsForSprites)
                {
                    foreach (Vector2D[] vertexes in s.Polygons)
                    {
                        Gl.glLineWidth(1);
                        Gl.glBegin(Gl.GL_LINES);
                        Vector2D v1 = vertexes[vertexes.Length - 1];
                        Vector2D v2;
                        for (int index = 0; index < vertexes.Length; ++index, v1 = v2)
                        {
                            v2 = vertexes[index];
                            Gl.glColor3f(1, 1, 1);
                            Gl.glVertex2f(v1.X, v1.Y);
                            Gl.glColor3f(1, 0, 0);
                            Gl.glVertex2f(v2.X, v2.Y);
                            DrawNormal(ref v1, ref v2);
                        }
                        Gl.glEnd();
                    }
                }
            }
            else if (entity.Shape is ParticleShape)
            {

                Gl.glBegin(Gl.GL_POINTS);
                Gl.glColor3f(1, 0, 0);
                //Gl.glColor3d(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());
                foreach (Vector2D vector in entity.Shape.Vertexes)
                {
                    Gl.glVertex2f((Scalar)vector.X, (Scalar)vector.Y);
                }
                Gl.glEnd();
            }
            else if (entity.Shape is RaySegmentsShape)
            {
                Gl.glLineWidth(1);
                RaySegmentsShape collection = (RaySegmentsShape)entity.Shape;
                Gl.glBegin(Gl.GL_LINES);
                for (int index = 0; index < collection.Segments.Length; ++index)
                {
                    Gl.glColor3f(1, 0, 1);
                    RaySegment ray = collection.Segments[index];
                    Gl.glVertex2f(ray.RayInstance.Origin.X, ray.RayInstance.Origin.Y);
                    Scalar length;
                    if (logic.Collisions[index].Distance == -1)
                    {
                        length = ray.Length;
                    }
                    else
                    {
                        length = logic.Collisions[index].Distance;
                    }
                    Vector2D temp = ray.RayInstance.Origin + ray.RayInstance.Direction * length;
                    Gl.glColor3f(1, 1, 1);
                    Gl.glVertex2f(temp.X, temp.Y);
                }
                Gl.glEnd();
            }
            else
            {
                Gl.glBegin(Gl.GL_POLYGON);
                bool first = true;
                bool second = true;
                foreach (Vector2D vector in entity.Shape.Vertexes)
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
                    Gl.glVertex2f((Scalar)vector.X, (Scalar)vector.Y);
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
            if (logic == null)
            {
                if (list == -1)
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
            else
            {
               // Gl.glLoadIdentity();
                Gl.glLoadMatrixf(matrix);
                DrawInternal();
            }
            if (DrawBoundingBoxes)
            {
                Gl.glLineWidth(1);
                BoundingRectangle rect = entity.Rectangle;
                Gl.glLoadIdentity();
                Gl.glColor3f(1,1,1);
                Gl.glBegin(Gl.GL_LINES);

                Gl.glVertex2f(rect.Min.X, rect.Min.Y);
                Gl.glVertex2f(rect.Min.X, rect.Max.Y);

                Gl.glVertex2f(rect.Min.X, rect.Max.Y);
                Gl.glVertex2f(rect.Max.X, rect.Max.Y);

                Gl.glVertex2f(rect.Max.X, rect.Max.Y);
                Gl.glVertex2f(rect.Max.X, rect.Min.Y);

                Gl.glVertex2f(rect.Max.X, rect.Min.Y);
                Gl.glVertex2f(rect.Min.X, rect.Min.Y);

                Gl.glEnd();

            }
            if (DrawCollisionPoints)
            {
                Gl.glLoadIdentity();
                Gl.glColor3f(0, 0, 1);
                Gl.glPointSize(3);
                Gl.glBegin(Gl.GL_POINTS);
                lock (points)
                {
                    foreach (Vector2D point in points)
                    {
                        Gl.glVertex2f(point.X, point.Y);
                    }
                    points.Clear();
                }
                Gl.glEnd();
            }
        }
        public void Dispose()
        {
            if (list != -1)
            {
                Gl.glDeleteLists(list, 1);
                list = -1;
            }
        }
    }
}