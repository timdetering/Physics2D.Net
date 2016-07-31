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
using Physics2DDotNet.Joints;
using Physics2DDotNet.PhysicsLogics;
using Physics2DDotNet.Collections;
using Tao.OpenGl;


namespace Graphics2DDotNet
{
    public sealed class BodyGraphic : Graphic
    {
        private static IDrawable GetIDrawable(Body body)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            return body.Shape.Tag as IDrawable;
        }
        Body body;
        int collidedStep = -1;
        bool isBodyOwner;


        public BodyGraphic(Body body)
            : base(GetIDrawable(body), body.Matrices.ToWorld, body.Lifetime)
        {
            SetBody(body);
            this.IsLifetimeOwner = false;
            this.isBodyOwner = true;
        }
        private BodyGraphic(BodyGraphic copy)
            : base(copy)
        {
            SetBody(copy.body.Duplicate());
            this.Lifetime = body.Lifetime;
            this.isBodyOwner = copy.isBodyOwner;
        }
        public bool IsBodyOwner
        {
            get { return isBodyOwner; }
            set { isBodyOwner = value; }
        }
        public Body Body { get { return body; } }
        public PhysicsState State { get { return body.State; } }
        private void SetBody(Body body)
        {
            this.body = body;
            this.body.Tag = this;
            this.body.PositionChanged += OnPositionChanged;
            this.body.ShapeChanged += OnShapeChanged;
        }
        internal void SetCollidedStep(int step)
        {
            collidedStep = step;
        }
        void OnShapeChanged(object sender, EventArgs e)
        {
            IDrawable newDrawable = body.Shape.Tag as IDrawable;
            if (newDrawable == null)
            {
                this.Lifetime = new Lifespan();
                this.Lifetime.IsExpired = true;
            }
            else
            {
                this.Drawable = newDrawable;
            }
        }
        void OnPositionChanged(object sender, EventArgs e)
        {
            Matrix2x3.Copy2DToOpenGlMatrix(ref body.Matrices.ToWorld, MatrixArray);
        }
        public override void Draw(DrawInfo drawInfo)
        {
            if (collidedStep >= body.Lifetime.LastUpdate - 1)
            {
                base.Draw(drawInfo);
            }
        }
        protected override void OnPending(EventArgs e)
        {
            if (isBodyOwner)
            {
                Parent.bodies.Add(body);
            }
            base.OnPending(e);
        }
        public override Graphic Duplicate()
        {
            return new BodyGraphic(this);
        }
    }
}