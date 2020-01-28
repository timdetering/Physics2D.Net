#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1fof the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Physics2D;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using System.Drawing;

using global::SdlDotNet;
using global::Tao.OpenGl;
namespace WindowsDriver.Demos
{
    public class WindowState
    {
        public Size ViewableAreaSize;
        public float Scale;
        public Vector2D CameraPosition;
        public Vector2D Offset;
        public BoundingBox2D ScreenBoundingBox;
        public WindowState(Size ViewableAreaSize, float Scale, Vector2D CameraPosition)
        {
            this.ViewableAreaSize = ViewableAreaSize;
            this.Scale = Scale;
            this.CameraPosition = CameraPosition;
            Vector2D tmp = new Vector2D(ViewableAreaSize.Width / (2 * Scale), ViewableAreaSize.Height / (2 * Scale));
            this.Offset = tmp - CameraPosition;
            Vector2D OtherBound = -tmp - CameraPosition;
            this.ScreenBoundingBox = new BoundingBox2D(CameraPosition + tmp, CameraPosition - tmp);
        }
        public void DrawPoint(Vector2D point, int color)
        {
            Color c = Color.FromArgb(color);
            float r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
            float g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
            float b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);
            Gl.glColor3f(r, g, b);
            Gl.glVertex2f((float)point.X, (float)point.Y);
        }
    }

    public class BaseDisplayDemo : IDemo
    {
        protected World2D world;
        protected int numberofCircleVertexes;
        protected Color backgroundColor;
        protected Color defaultColor1;
        protected Color defaultColor2;
        public BaseDisplayDemo()
        {
            GenerateDots();
            this.world = new World2D();
            this.numberofCircleVertexes = 15;
            this.backgroundColor = Color.Black;
            this.defaultColor1 = Color.Blue;
            this.defaultColor2 = Color.Gray;
        }
        public virtual string Name
        {
            get
            {
                return "BaseDisplayDemo";

            }
        }
        public virtual string Description
        {
            get
            {
                return "Impliments methods for subclasses";
            }
        }
        public virtual string Instructions
        {
            get
            {
                return "None";
            }
        }
        public virtual Vector2D CameraPosition
        {
            get
            {
                return Vector2D.Zero;
            }
        }
        public virtual float Scale
        {
            get
            {
                return 1;
            }
        }
        public virtual World2D World2D
        {
            get
            {
                return world;
            }
        }
        public virtual Color BackgroundColor { get { return backgroundColor; } set { backgroundColor = value; } }


        public virtual bool Update(float dt)
        {
            world.Update(dt);
            return false;
        }
        public virtual void UpdateKeyBoard(KeyboardState keys, float dt)
        {
        }
        public virtual void InitObjects() { }
        public virtual void AddObjects() { }
        public virtual IDemo CreateNew()
        {
            return new BaseDisplayDemo();
        }
        public override string ToString()
        {
            return Name;
        }
        DotBox dotbox;
        int DotsCount = 60;
        int MaxDots = 10000;
        Vector2D dotsBoxSize = new Vector2D(15000, 15000);
        private void GenerateDots()
        {
            Random rand = new Random();
            Vector2D[] dots = new Vector2D[DotsCount];
            int[] colors = new int[DotsCount];
            for (int pos = 0; pos != DotsCount; ++pos)
            {
                dots[pos].X = (float)rand.NextDouble() * dotsBoxSize.X;
                dots[pos].Y = (float)rand.NextDouble() * dotsBoxSize.Y;
                switch (rand.Next(3))
                {
                    case 0:
                        colors[pos] = Color.Wheat.ToArgb();
                        break;
                    case 1:
                        colors[pos] = Color.Green.ToArgb();
                        break;
                    default:
                        colors[pos] = Color.White.ToArgb();
                        break;
                }
            }
            dotbox = new DotBox(MaxDots, dotsBoxSize, dots, colors);

        }
        void DrawVertexes(WindowState state)
        {
            List<ICollidableBody> BODIES = new List<ICollidableBody>(world.Collidables);
            float baseradiusInc = (float)(MathAdv.PI * 2) / ((float)numberofCircleVertexes);
            float radiusInc = baseradiusInc;
            int numerofCV = numberofCircleVertexes;
            float sizedCV = 100;

            int color = defaultColor1.ToArgb();
            int color2 = defaultColor2.ToArgb();
            Matrix3x3 matrix = Matrix3x3.FromScale(new Vector2D(state.Scale, state.Scale)) * Matrix3x3.FromTranslate2D(state.Offset);
            foreach (ICollidableBody body in BODIES)
            {
                ICollidableBody cont = body;
                if (body.BoundingBox2D == null || !body.IgnoreInfo.IsCollidable)
                {
                    body.CalcBoundingBox2D();
                }
                if (!state.ScreenBoundingBox.TestIntersection(body.BoundingBox2D) || body.IsExpired)
                {
                    continue;
                }
                foreach (Physics2D.ICollidableBodyPart part in body.CollidableParts)
                {
                    Gl.glBegin(Gl.GL_POLYGON);
                    if (part.UseCircleCollision)
                    {
                        if (part.BaseGeometry.BoundingRadius > sizedCV)
                        {
                            numerofCV = numberofCircleVertexes + (int)MathAdv.Sqrt(part.BaseGeometry.BoundingRadius - sizedCV);
                            if (numerofCV > 100)
                            {
                                numerofCV = 100;
                            }
                            radiusInc = (float)(MathAdv.PI * 2) / ((float)numerofCV);
                        }
                        else
                        {
                            numerofCV = numberofCircleVertexes;
                            radiusInc = baseradiusInc;
                        }
                        for (int angle = 0; angle != numerofCV; ++angle)
                        {
                            Vector2D vect = matrix * (Vector2D.FromLengthAndAngle(part.BaseGeometry.BoundingRadius, ((float)angle) * radiusInc + part.GoodPosition.Angular) + part.GoodPosition.Linear);

                            int vectColor = color2;
                            if (angle == 0)
                            {
                                vectColor = color;
                            }


                            Color c = Color.FromArgb(vectColor);
                            float r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
                            float g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
                            float b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);
                            Gl.glColor3f(r, g, b);
                            Gl.glVertex3f((float)vect.X, (float)vect.Y, 0);
                        }

                        //soundInfo.Add(points);
                    }
                    else
                    {
                        Vector2D[] vects = part.DisplayVertices;
                        if (vects != null)
                        {
                            vects = Vector2D.Transform(matrix, part.DisplayVertices);
                            for (int pos = 0; pos != vects.Length; ++pos)
                            {
                                int vectColor = color2;
                                if (pos == 0)
                                {
                                    vectColor = color;
                                }


                                Color c = Color.FromArgb(vectColor);
                                float r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
                                float g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
                                float b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);

                                Gl.glColor3f(r, g, b);
                                Gl.glVertex3f((float)vects[pos].X, (float)vects[pos].Y, 0);
                            }
                        }
                    }
                    Gl.glEnd();
                }
            }
        }
        void DrawLines(WindowState state)
        {


            List<IRay2DEffect> effects = world.Ray2DEffects;
            if (effects != null && effects.Count > 0)
            {
                Gl.glLineWidth(1);
                Gl.glBegin(Gl.GL_LINES);
                foreach (IRay2DEffect effect in effects)
                {
                    if (effect != null && effect.RaySegment != null)
                    {

                        Vector2D vect1 = (effect.RaySegment.Origin + state.Offset) * state.Scale;


                        Color c = Color.White;
                        float r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
                        float g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
                        float b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);

                        Gl.glColor3f(r, g, b);
                        Gl.glVertex3f((float)vect1.X, (float)vect1.Y, 0);

                        Vector2D vect2 = (effect.RaySegment.Origin + effect.RaySegment.Direction * effect.DisplayLength + state.Offset) * state.Scale;
                        c = Color.LightCyan;
                        r = ((float)(int)c.R) / ((float)(int)Byte.MaxValue);
                        g = ((float)(int)c.G) / ((float)(int)Byte.MaxValue);
                        b = ((float)(int)c.B) / ((float)(int)Byte.MaxValue);
                        Gl.glColor3f(r, g, b);
                        Gl.glVertex3f((float)vect2.X, (float)vect2.Y, 0);
                    }
                }
                Gl.glEnd();

            }


        }

        public void UpdateAI(object dt)
        {
            if (dt is float)
            {
                UpdateAI((float)dt);
            }
        }
        public virtual void UpdateAI(float dt)
        {}

        #region IDemo Members


        public virtual void DrawGraphics(Size ViewableAreaSize)
        {
            WindowState state = new WindowState(ViewableAreaSize, Scale, CameraPosition);
            dotbox.DrawDots(state);
            DrawLines(state);
            DrawVertexes(state);
        }

        #endregion
    }
    [Serializable]
    public class DotBox
    {
        Vector2D size;
        Vector2D sizeInv;
        Vector2D[] dots;
        int[] colors;
        int maxdots;
        Random rand = new Random();
        public DotBox(int maxdots, Vector2D size, Vector2D[] dots, int[] colors)
        {
            this.size = size;
            this.dots = dots;
            this.colors = colors;
            this.maxdots = maxdots;
            this.sizeInv.X = 1 / size.X;
            this.sizeInv.Y = 1 / size.Y;
        }
        public void DrawDots(WindowState state)
        {
            Vector2D Offset = state.Offset;
            BoundingBox2D screenbox = state.ScreenBoundingBox;
            Vector2D startpos = new Vector2D();
            Vector2D endpos = new Vector2D();
            startpos.X = size.X * (float)Math.Floor(screenbox.Lower.X * sizeInv.X);
            startpos.Y = size.Y * (float)Math.Floor(screenbox.Lower.Y * sizeInv.Y);
            endpos.X = size.X * (float)Math.Floor(screenbox.Upper.X * sizeInv.X);
            endpos.Y = size.Y * (float)Math.Floor(screenbox.Upper.Y * sizeInv.Y);
            startpos += Offset;
            endpos += Offset;

            int length = dots.Length;
            Vector2D pos = new Vector2D();
            int count = 0;
            Gl.glBegin(Gl.GL_POINTS);
            for (pos.X = startpos.X; pos.X <= endpos.X; pos.X += size.X)
            {
                for (pos.Y = startpos.Y; pos.Y <= endpos.Y; pos.Y += size.Y)
                {
                    Matrix3x3 matrix = Matrix3x3.FromScale(new Vector2D(state.Scale, state.Scale)) * Matrix3x3.FromTranslate2D(pos);

                    Vector2D[] values = Vector2D.Transform(matrix, dots);
                    for (int vpos = 0; vpos < length; ++vpos)
                    {
                        state.DrawPoint(values[vpos], colors[vpos]);
                        count++;
                    }
                    if (count > maxdots)
                    {
                        return;
                    }
                }
            }
            Gl.glEnd();
        }
    }
}
