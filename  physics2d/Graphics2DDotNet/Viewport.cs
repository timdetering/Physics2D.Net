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
using System.Drawing;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Tao.OpenGl;
using SdlDotNet.Graphics;
using SdlDotNet.Core;
using SdlDotNet.Input;


namespace Graphics2DDotNet
{
    /// <summary>
    /// This represents a section of a Window. 
    /// </summary>
    public class Viewport : Pendable<Window>
    {
        static int idCounter = 0;
        sealed class ViewportClipperLogic : Physics2DDotNet.PhysicsLogics.PhysicsLogic
        {
            Viewport viewport;
            Body clipper;
            PolygonShape shape;
            Vector2D[] vertexes;
            public ViewportClipperLogic(Viewport viewport)
                : base(new Lifespan())
            {
                this.viewport = viewport;

                this.vertexes = new Vector2D[4];
                vertexes = VertexHelper.Subdivide(vertexes,10);
                SetVertexes();
                this.clipper = new Body(new PhysicsState(), shape, 1, new Coefficients(1, 1), this.Lifetime);
                clipper.IgnoresPhysicsLogics = true;
                clipper.IgnoresCollisionResponse = true;
                clipper.Transformation = viewport.ToWorld;
                clipper.IsBroadPhaseOnly = true;
                clipper.IsEventable = false;
                clipper.Collided += clipper_Collided;
                viewport.Changed += viewport_Changed;
                viewport.Removed += viewport_Removed;
            }
            void ClearEvents()
            {
                clipper.Collided -= clipper_Collided;
                viewport.Changed -= viewport_Changed;
                viewport.Removed -= viewport_Removed;
            }
            void viewport_Removed(object sender, EventArgs e)
            {
                ClearEvents();
                this.Lifetime.IsExpired = true;
            }

            protected override void OnRemoved(RemovedEventArgs e)
            {
                base.OnRemoved(e);
                if (this.Engine == null)
                {
                    ClearEvents();
                }
            }

            void viewport_Changed(object sender, EventArgs e)
            {
                SetVertexes();
                clipper.Shape = shape;
                clipper.Transformation = viewport.ToWorld;
            }
            void SetVertexes()
            {
                Rectangle rect = viewport.Rectangle;
                vertexes[0] = new Vector2D(rect.Left, rect.Top);
                vertexes[1] = new Vector2D(rect.Left, rect.Bottom);
                vertexes[2] = new Vector2D(rect.Right, rect.Bottom);
                vertexes[3] = new Vector2D(rect.Right, rect.Top);
                shape = new PolygonShape(vertexes, Math.Min(viewport.Width, viewport.Height) / 5);
            }
            void clipper_Collided(object sender, CollisionEventArgs e)
            {
                BodyGraphic graphic = e.Other.Tag as BodyGraphic;
                if (graphic != null)
                {
                    graphic.SetCollidedStep(e.Step.UpdateCount);
                }
            }
            public override ReadOnlyCollection<Body> LogicBodies
            {
                get
                {
                    return new ReadOnlyCollection<Body>(new Body[] { clipper });
                }
            }
            protected override void RunLogic(TimeStep step)
            { }
        }
        int id = idCounter++;
        bool registeredMouseDown;
        private event EventHandler<ViewportMouseButtonEventArgs> mouseDown;
        public event EventHandler<ViewportMouseButtonEventArgs> MouseDown
        {
            add
            {
                mouseDown += value;
                if (!registeredMouseDown && mouseDown != null)
                {
                    registeredMouseDown = true;
                    Events.MouseButtonDown += OnMouseButtonDown;
                }
            }
            remove
            {
                mouseDown -= value;
                if (registeredMouseDown && mouseDown == null)
                {
                    registeredMouseDown = false;
                    Events.MouseButtonDown -= OnMouseButtonDown;
                }
            }
        }
        void OnMouseButtonDown(object sender, SdlDotNet.Input.MouseButtonEventArgs e)
        {
            if (!rectangle.Contains(e.Position)) { return; }
            Vector2D position = new Vector2D(e.X, e.Y);
            Vector2D.Transform(ref toWorld, ref position, out position);
            if (mouseDown != null)
            {
                mouseDown(this, new ViewportMouseButtonEventArgs(position, e.ButtonPressed, e.Button));
            }
        }

        bool registeredMouseUp;
        private event EventHandler<ViewportMouseButtonEventArgs> mouseUp;
        public event EventHandler<ViewportMouseButtonEventArgs> MouseUp
        {
            add
            {
                mouseUp += value;
                if (!registeredMouseUp && mouseUp != null)
                {
                    registeredMouseUp = true;
                    Events.MouseButtonUp += OnMouseButtonUp;
                }
            }
            remove
            {
                mouseUp -= value;
                if (registeredMouseUp && mouseUp == null)
                {
                    registeredMouseUp = false;
                    Events.MouseButtonUp -= OnMouseButtonUp;
                }
            }
        }
        void OnMouseButtonUp(object sender, SdlDotNet.Input.MouseButtonEventArgs e)
        {
            if (!rectangle.Contains(e.Position)) { return; }
            Vector2D position = new Vector2D(e.X, e.Y);
            Vector2D.Transform(ref toWorld, ref position, out position);
            if (mouseUp != null)
            {
                mouseUp(this, new ViewportMouseButtonEventArgs(position, e.ButtonPressed, e.Button));
            }
        }

        bool registeredMouseMotion;
        private event EventHandler<ViewportMouseMotionEventArgs> mouseMotion;
        public event EventHandler<ViewportMouseMotionEventArgs> MouseMotion
        {
            add
            {
                mouseMotion += value;
                if (!registeredMouseMotion && mouseMotion != null)
                {
                    registeredMouseMotion = true;
                    Events.MouseMotion += OnMouseMotion;
                }
            }
            remove
            {
                mouseMotion -= value;
                if (registeredMouseMotion && mouseMotion == null)
                {
                    registeredMouseMotion = false;
                    Events.MouseMotion -= OnMouseMotion;
                }
            }
        }
        void OnMouseMotion(object sender, SdlDotNet.Input.MouseMotionEventArgs e)
        {
            if (!rectangle.Contains(e.Position)) { return; }
            Vector2D position = new Vector2D(e.X, e.Y);
            Vector2D relative = new Vector2D(e.RelativeX, e.RelativeY);
            Vector2D.Add(ref relative, ref position, out relative);
            Vector2D.Transform(ref toWorld, ref position, out position);
            Vector2D.Transform(ref toWorld, ref relative, out relative);
            Vector2D.Subtract(ref relative, ref position, out relative);
            if (mouseMotion != null)
            {
                mouseMotion(this, new ViewportMouseMotionEventArgs(position, relative, e.ButtonPressed, e.Button));
            }
        }

        public event EventHandler Changed;
        public event EventHandler<DrawEventArgs> BeginDrawing;
        public event EventHandler<DrawEventArgs> EndDrawing;

        object syncRoot;
        bool calculated;
        Rectangle rectangle;
        Scalar[] matrixArray;
        Matrix2x3 toWorld;
        Matrix2x3 toScreen;
        Scene scene;
        ViewportClipperLogic clipper;
        public Viewport( Rectangle rectangle,
            Matrix2x3 projection,
            Scene scene,Lifespan lifetime):base(lifetime)
        {
            this.syncRoot = new object();
            this.matrixArray = new Scalar[16];
            this.rectangle = rectangle;
            this.scene = scene;
            this.toScreen = projection;
            Calc();
            this.scene.AddViewport(this);
            AddClipper();
        }
        void AddClipper()
        {
            clipper = new ViewportClipperLogic(this);
            clipper.Removed += new EventHandler<RemovedEventArgs>(OnClipperRemoved);
            scene.Engine.AddLogic(clipper);
        }
        void OnClipperRemoved(object sender, RemovedEventArgs e)
        {
            clipper.Lifetime.IsExpired = false;
            e.Engine.AddLogic(clipper);
        }
        void RemoveClipper()
        {
            clipper.Removed -= OnClipperRemoved;
            clipper.Lifetime.IsExpired = true;
        }
        protected override void OnRemoved(RemovedEventArgs<Window> e)
        {
            RemoveClipper();
            base.OnRemoved(e);
        }

        public Vector2D MousePosition
        {
            get
            {
                Point point = Mouse.MousePosition;
                Vector2D position = new Vector2D(point.X, point.Y);
                Vector2D.Transform(ref toWorld, ref position, out position);
                return position;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
            set
            {
                this.rectangle = value;
                calculated = false;
            }
        }
        public Matrix2x3 ToWorld
        {
            get { return toWorld; }
        }
        public Matrix2x3 ToScreen
        {
            get { return toScreen; }
            set
            {
                toScreen = value;
                Matrix2x3.Invert(ref value, out toWorld);
                calculated = false;
            }
        }
        public Scene Scene
        {
            get { return scene; }
            set
            {
                if (value == null) { throw new ArgumentOutOfRangeException("value"); }
                lock (syncRoot)
                {
                    RemoveClipper();
                    scene.RemoveViewport(this);
                    scene = value;
                    value.AddViewport(this);
                    AddClipper();
                }
            }
        }
        public int X
        {
            get { return rectangle.X; }
            set
            {
                rectangle.X = value;
                calculated = false;
            }
        }
        public int Y
        {
            get { return rectangle.Y; }
            set
            {
                rectangle.Y = value;
                calculated = false;
            }
        }
        public int Width
        {
            get { return rectangle.Width; }
            set
            {
                rectangle.Width = value;
                calculated = false;
            }
        }
        public int Height
        {
            get { return rectangle.Height; }
            set
            {
                rectangle.Height = value;
                calculated = false;
            }
        }
        private void Calc()
        {
            Matrix4x4 ortho = Matrix4x4.FromOrthographic(
                rectangle.Left,
                rectangle.Right,
                rectangle.Bottom,
                rectangle.Top,
                -1, 1);
            Matrix4x4 transform2 = Matrix4x4.From2DMatrix(toScreen);
            Matrix4x4 result = ortho * transform2;
            Matrix4x4.CopyTranspose(ref result, matrixArray);
            if (Changed != null) { Changed(this, EventArgs.Empty); }
        }
        public void Draw(DrawInfo drawInfo)
        {
            if (BeginDrawing != null) { BeginDrawing(this, new DrawEventArgs(drawInfo)); }
            Gl.glViewport(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            if (!calculated)
            {
                calculated = true;
                Calc();
            }
            GlHelper.GlLoadMatrix(matrixArray);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            scene.Draw(drawInfo);
            if (EndDrawing != null) { EndDrawing(this, new DrawEventArgs(drawInfo)); }
        }
    }
}