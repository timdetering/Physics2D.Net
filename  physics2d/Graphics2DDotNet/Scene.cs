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
using Physics2DDotNet.PhysicsLogics;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Tao.OpenGl;
using SdlDotNet.Graphics;
using SdlDotNet.Core;
using SdlDotNet.Input;

namespace Graphics2DDotNet
{
    /// <summary>
    /// this class represents a scene (a collection of objects that can be drawn)
    /// that is drawn via a Viewport
    /// </summary>
    public class Scene
    {
        public event EventHandler<CollectionEventArgs<Graphic>> GraphicsAdded
        {
            add { graphics.ItemsAdded += value; }
            remove { graphics.ItemsAdded -= value; }
        }
        public event EventHandler<CollectionEventArgs<Graphic>> GraphicsRemoved
        {
            add { graphics.ItemsRemoved += value; }
            remove { graphics.ItemsRemoved -= value; }
        }
        public event EventHandler<ItemEventArgs<Viewport>> ViewportAdded;
        public event EventHandler<ItemEventArgs<Viewport>> ViewportRemoved;
        public event EventHandler<DrawEventArgs> BeginDrawing;
        public event EventHandler<DrawEventArgs> EndDrawing;
        List<Viewport> viewports;
        PendableCollection<Scene, Graphic> graphics;
        object syncRoot;
        [NonSerialized]
        internal AdvReaderWriterLock rwLock;
        PhysicsEngine engine;
        PhysicsTimer timer;
        internal List<Body> bodies;
        internal List<Joint> joints;
        internal List<PhysicsLogic> physicsLogics;
        bool isPaused;


        object tag;
        public Scene()
        {
            this.syncRoot = new object();
            this.engine = new PhysicsEngine();
            this.rwLock = new AdvReaderWriterLock();
            this.timer = new PhysicsTimer(Update, .01f);
            this.graphics = new PendableCollection<Scene, Graphic>(this);
            this.viewports = new List<Viewport>();
            this.bodies = new List<Body>();
            this.joints = new List<Joint>();
            this.physicsLogics = new List<PhysicsLogic>();
        }
        public ReadOnlyThreadSafeCollection<Viewport> Viewports
        {
            get
            {
                return new ReadOnlyThreadSafeCollection<Viewport>(rwLock, viewports);
            }
        }
        public ReadOnlyThreadSafeCollection<Graphic> Graphics
        {
            get
            {
                return new ReadOnlyThreadSafeCollection<Graphic>(rwLock, graphics.Items);
            }
        }
        public PhysicsTimer Timer
        {
            get { return timer; }
        }
        public PhysicsEngine Engine
        {
            get { return engine; }
        }
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }
        internal void AddViewport(Viewport item)
        {
            rwLock.EnterWrite();
            try
            {
                viewports.Add(item);
            }
            finally
            {
                rwLock.ExitWrite();
            }
            if (ViewportAdded != null) { ViewportAdded(this, new ItemEventArgs<Viewport>(item)); }
        }
        internal void RemoveViewport(Viewport item)
        {
            rwLock.EnterWrite();
            try
            {
                viewports.Remove(item);
            }
            finally
            {
                rwLock.ExitWrite();
            }
            if (ViewportRemoved != null) { ViewportRemoved(this, new ItemEventArgs<Viewport>(item)); }
        }

        public void AddGraphic(Graphic item)
        {
            lock (syncRoot)
            {
                graphics.Add(item);
                engine.AddBodyRange(bodies);
                bodies.Clear();
                engine.AddJointRange(joints);
                joints.Clear();
                engine.AddLogicRange(physicsLogics);
                physicsLogics.Clear();
            }
        }
        public void AddGraphicRange(ICollection<Graphic> collection)
        {
            lock (syncRoot)
            {
                graphics.AddRange(collection);
                engine.AddBodyRange(bodies);
                bodies.Clear();
                engine.AddJointRange(joints);
                joints.Clear();
                engine.AddLogicRange(physicsLogics);
                physicsLogics.Clear();
            }
        }
        public void Draw(DrawInfo drawInfo)
        {
            if (BeginDrawing != null) { BeginDrawing(this, new DrawEventArgs(drawInfo)); }
            rwLock.EnterWrite();
            try
            {
                graphics.RemoveExpired();
                lock (syncRoot)
                {
                    graphics.AddPending();
                }
                graphics.CheckZOrder();
                List<Graphic> graphics1 = graphics.Items;
                for (int index = 0; index < graphics1.Count; ++index)
                {
                    Graphic graphic = graphics1[index];
                    if (graphic.IsVisible)
                    {
                        graphic.Draw(drawInfo);
                    }
                }
            }
            finally
            {
                rwLock.ExitWrite();
            }
            if (EndDrawing != null) { EndDrawing(this, new DrawEventArgs(drawInfo)); }
        }
        protected virtual void Update(Scalar dt, Scalar trueDt)
        {
            engine.Update(
                (isPaused) ? (0) : (dt),
                trueDt);
        }
        public void Clear()
        {
            rwLock.EnterWrite();
            try
            {
                engine.Clear();
                lock (syncRoot)
                {
                    graphics.Clear();
                }
            }
            finally
            {
                rwLock.ExitWrite();
            }

        }
    }
}