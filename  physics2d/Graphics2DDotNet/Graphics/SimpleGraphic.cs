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
    public class SimpleGraphic : IGraphic
    {
        static int idCounter = int.MaxValue;
        public event EventHandler ZOrderChanged;
        object tag;

        int id;
        Scalar[] matrixArray;
        Layer parent;
        IDrawable drawable;
        IAnimation animation;
        Lifespan lifetime;
        int zOrder;
        IDrawableState drawableState;
        List<IDrawProperty> drawProperties;
        bool shouldDraw;
        public SimpleGraphic(IDrawable drawable, Matrix2x3 matrix, Lifespan lifetime)
        {
            if (drawable == null) { throw new ArgumentNullException("drawable"); }
            if (lifetime == null) { throw new ArgumentNullException("lifetime"); }
            Initialize();
            this.drawProperties = new List<IDrawProperty>();
            this.drawable = drawable;
            this.drawableState = drawable.CreateState();
            this.lifetime = lifetime;
            this.shouldDraw = true;
            this.matrixArray = new Scalar[16];
            Matrix2x3.Copy2DToOpenGlMatrix(ref matrix, this.matrixArray);
        }
        protected SimpleGraphic(SimpleGraphic copy)
        {
            Initialize();
            this.drawProperties = new List<IDrawProperty>(copy.drawProperties);
            this.drawable = copy.drawable;
            this.drawableState = drawable.CreateState();
            this.lifetime = copy.lifetime.Duplicate();
            this.matrixArray = (Scalar[])copy.matrixArray.Clone();
            this.shouldDraw = copy.shouldDraw;
        }
        private void Initialize()
        {
            id = System.Threading.Interlocked.Decrement(ref  idCounter);
        }

        public bool ShouldDraw
        {
            get { return shouldDraw; }
            set
            {
                shouldDraw = value;
            }
        }

        public int ID { get { return id; } }
        public Matrix2x3 Matrix
        {
            get
            {
                Matrix2x3 result;
                Matrix2x3.Copy2DFromOpenGlMatrix(matrixArray, out result);
                return result;
            }
            set
            {
                Matrix2x3.Copy2DToOpenGlMatrix(ref value, matrixArray);
            }
        }
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public List<IDrawProperty> DrawProperties { get { return drawProperties; } }
        public Scalar[] MatrixArray { get { return matrixArray; } }
        public Layer Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }
        public Lifespan Lifetime
        {
            get { return lifetime; }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                lifetime = value;
            }
        }
        public IDrawableState DrawableState
        {
            get { return drawableState; }
            set { drawableState = value; }
        }
        public int ZOrder
        {
            get { return zOrder; }
            set
            {
                if (value != zOrder)
                {
                    zOrder = value;
                    if (ZOrderChanged != null) { ZOrderChanged(this, EventArgs.Empty); }
                }
            }
        }
        public IDrawable Drawable
        {
            get { return drawable; }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                this.drawable = value;
                this.drawableState = value.CreateState();
                if (parent != null && drawableState != null)
                {
                    drawableState.OnPending(this);
                }
            }
        }
        public IAnimation Animation
        {
            get { return animation; }
            set { animation = value; }
        }
        public virtual void Draw(DrawInfo drawInfo)
        {
            GlHelper.GlLoadMatrix(matrixArray);
            ApplyProperties();
            if (animation != null)
            {
                if (animation.LifeTime.IsExpired)
                {
                    animation = null;
                }
                else
                {
                    animation.Apply(this, drawInfo);
                    animation.LifeTime.Update(drawInfo.Dt, drawInfo.DrawCount);
                }
            }
            drawable.Draw(drawInfo, drawableState);
            UpdateTime(drawInfo);
            RemoveProperties();
        }
        private void ApplyProperties()
        {
            for (int index = 0; index < drawProperties.Count; ++index)
            {
                drawProperties[index].Apply();
            }
        }
        private void RemoveProperties()
        {
            for (int index = 0; index < drawProperties.Count; ++index)
            {
                drawProperties[index].Remove();
            }
        }
        protected virtual void UpdateTime(DrawInfo drawInfo)
        {
            this.lifetime.Update(drawInfo.Dt, drawInfo.DrawCount);
        }
        public virtual void OnPending(Layer parent)
        {
            this.parent = parent;
            if (drawableState != null)
            {
                drawableState.OnPending(this);
            }
        }
        public virtual void OnPendingRange(Layer parent, RangeInfo rangeInfo)
        {
            this.parent = parent;
            if (drawableState != null)
            {
                drawableState.OnPending(this);
            }
        }

        public virtual IGraphic Duplicate()
        {
            return new SimpleGraphic(this);
        }
        public object Clone()
        {
            return Duplicate();
        }
    }

}