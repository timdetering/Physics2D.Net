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
    public class Graphic : Pendable<Scene>, IDuplicateable<Graphic>
    {
        public event EventHandler<CollectionEventArgs<Graphic>> ChildrenAdded
        {
            add { children.ItemsAdded += value; }
            remove { children.ItemsAdded -= value; }
        }
        public event EventHandler<CollectionEventArgs<Graphic>> ChildrenRemoved
        {
            add { children.ItemsRemoved += value; }
            remove { children.ItemsRemoved -= value; }
        }


        object syncRoot;
        PendableCollection<Scene, Graphic> children;
        List<Graphic> preChildren = new List<Graphic>();
        Scalar[] matrixArray;
        IDrawable drawable;
        IAnimation animation;
        IDrawableState drawableState;
        List<IDrawProperty> drawProperties;
        bool isVisible;
        Graphic pendingGraphicParent;
        Graphic graphicParent;
        bool hasChildren;
        private bool doAnimation;
        bool isLifetimeOwner;



        public Graphic(IDrawable drawable, Matrix2x3 matrix, Lifespan lifetime)
            : base(lifetime)
        {
            if (drawable == null) { throw new ArgumentNullException("drawable"); }
            this.drawProperties = new List<IDrawProperty>();
            this.drawable = drawable;
            this.drawableState = drawable.CreateState();
            this.isVisible = true;
            this.matrixArray = new Scalar[16];
            Matrix2x3.Copy2DToOpenGlMatrix(ref matrix, this.matrixArray);
            this.syncRoot = new object();
            this.preChildren = new List<Graphic>();
            this.isLifetimeOwner = true;
        }
        protected Graphic(Graphic copy)
            : base(copy)
        {
            this.drawProperties = new List<IDrawProperty>(copy.drawProperties);
            this.drawable = copy.drawable;
            this.drawableState = drawable.CreateState();
            this.matrixArray = (Scalar[])copy.matrixArray.Clone();
            this.isVisible = copy.isVisible;
            this.syncRoot = new object();
            this.preChildren = new List<Graphic>(copy.preChildren);
            this.isLifetimeOwner = copy.isLifetimeOwner;
        }

        public ReadOnlyThreadSafeCollection<Graphic> Children
        {
            get
            {
                return new ReadOnlyThreadSafeCollection<Graphic>(Parent.rwLock, children.Items);
            }
        }

        public bool IsLifetimeOwner
        {
            get { return isLifetimeOwner; }
            set { isLifetimeOwner = value; }
        }
        public Graphic GraphicParent
        {
            get { return graphicParent; }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }
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
        public List<IDrawProperty> DrawProperties { get { return drawProperties; } }
        public Scalar[] MatrixArray { get { return matrixArray; } }
        public IDrawableState DrawableState
        {
            get { return drawableState; }
            set { drawableState = value; }
        }
        public IDrawable Drawable
        {
            get { return drawable; }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                this.drawable = value;
                this.drawableState = value.CreateState();
                if (Parent != null && drawableState != null)
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

        private void PreDraw(DrawInfo drawInfo)
        {
            ApplyProperties();
            doAnimation = false;
            if (animation != null)
            {
                if (animation.LifeTime.IsExpired)
                {
                    animation = null;
                }
                else
                {
                    doAnimation = true;
                    animation.Apply(this, drawInfo);
                    animation.LifeTime.Update(drawInfo.Dt, drawInfo.DrawCount);
                }
            }
        }
        private void PostDraw(DrawInfo drawInfo)
        {
            if (doAnimation)
            {
                animation.Remove(this, drawInfo);
            }
            RemoveProperties();
        }
        private void DrawWithChildren(DrawInfo drawInfo)
        {
            children.RemoveExpired();
            lock (syncRoot)
            {
                children.AddPending();
            }
            children.CheckZOrder();
            hasChildren = children.Items.Count > 0;
            PreDraw(drawInfo);
            int index = 0;
            List<Graphic> children2 = children.Items;
            for (; index < children2.Count && children2[index].ZOrder < this.ZOrder; ++index)
            {
                Gl.glPushMatrix();
                children2[index].Draw(drawInfo);
                Gl.glPopMatrix();
            }
            drawable.Draw(drawInfo, drawableState);
            for (; index < children2.Count; ++index)
            {
                Gl.glPushMatrix();
                children2[index].Draw(drawInfo);
                Gl.glPopMatrix();
            }
            PostDraw(drawInfo);
        }
        private void DrawWithoutChildren(DrawInfo drawInfo)
        {
            PreDraw(drawInfo);
            drawable.Draw(drawInfo, drawableState);
            PostDraw(drawInfo);
        }
        public virtual void Draw(DrawInfo drawInfo)
        {
            UpdateTime(drawInfo);
            if (graphicParent == null)
            {
                GlHelper.GlLoadMatrix(matrixArray);
            }
            else
            {
                GlHelper.GlMultMatrix(matrixArray);
            }
            if (hasChildren)
            {
                DrawWithChildren(drawInfo);
            }
            else
            {
                DrawWithoutChildren(drawInfo);
            }
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
        protected void UpdateTime(DrawInfo drawInfo)
        {
            if (isLifetimeOwner)
            {
                this.Lifetime.Update(drawInfo.Dt, drawInfo.DrawCount);
            }
        }
        public virtual Graphic Duplicate()
        {
            return new Graphic(this);
        }
        public object Clone()
        {
            return Duplicate();
        }

        public void AddChild(Graphic item)
        {
            hasChildren = true;
            lock (syncRoot)
            {
                if (children == null)
                {
                    preChildren.Add(item);
                }
                else
                {
                    children.Add(item);
                    item.graphicParent = this;
                }
            }
        }
        public void AddChildRange(ICollection<Graphic> collection)
        {
            hasChildren = true;
            lock (syncRoot)
            {
                if (children == null)
                {
                    preChildren.AddRange(collection);
                }
                else
                {
                    foreach (Graphic item in collection)
                    {
                        item.pendingGraphicParent = this;
                    }
                    children.AddRange(collection);
                }
            }
        }
        protected override void OnPending(EventArgs e)
        {
            this.graphicParent = pendingGraphicParent;
            if (drawableState != null)
            {
                drawableState.OnPending(this);
            }
            base.OnPending(e);
        }
        protected override void OnAdded(EventArgs e)
        {
            lock (syncRoot)
            {
                this.children = new PendableCollection<Scene, Graphic>(Parent, this);
                foreach (Graphic item in preChildren)
                {
                    item.pendingGraphicParent = this;
                }
                this.children.AddRange(preChildren);
                preChildren.Clear();
            }

            base.OnAdded(e);
        }
    }
}