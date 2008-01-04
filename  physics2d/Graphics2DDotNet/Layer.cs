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
    public class Layer
    {
        class ZOrderComparer : IComparer<IGraphic>
        {
            public int Compare(IGraphic x, IGraphic y)
            {
                int result = x.ZOrder.CompareTo(y.ZOrder);
                if (result == 0)
                {
                    result = x.ID.CompareTo(y.ID);
                }
                return result;
            }
        }
        static ZOrderComparer zOrderComparer = new ZOrderComparer();
        static bool IsGraphicExpired(IGraphic graphic) { return graphic.Lifetime.IsExpired; }
        object syncRoot;
        [NonSerialized]
        AdvReaderWriterLock rwLock;
        PhysicsEngine engine;
        PhysicsTimer timer;
        List<IGraphic> pendingGraphics;
        List<IGraphic> graphics;
        bool zOrderChanged;
        Layer overlay;
        public Layer()
        {
            this.syncRoot = new object();
            this.engine = new PhysicsEngine();
            this.pendingGraphics = new List<IGraphic>();
            this.graphics = new List<IGraphic>();
            this.rwLock = new AdvReaderWriterLock();
            this.timer = new PhysicsTimer(Update, .01f);
        }
        public ReadOnlyThreadSafeCollection<IGraphic> Graphics
        {
            get
            {
                return new ReadOnlyThreadSafeCollection<IGraphic>(rwLock, graphics);
            }
        }
        public Layer Overlay
        {
            get { return overlay; }
            set
            {
                if (overlay == this) { throw new ArgumentException("cant overlay itself"); }
                overlay = value;
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

        public void AddGraphic(IGraphic item)
        {
            lock (syncRoot)
            {
                item.OnPending(this);
                pendingGraphics.Add(item);
            }
        }
        public void AddGraphicRange(ICollection<IGraphic> collection)
        {
            lock (syncRoot)
            {
                List<Body> bodies = new List<Body>();
                RangeInfo info = new RangeInfo(bodies);
                foreach (IGraphic item in collection)
                {
                    item.OnPendingRange(this, info);
                }
                engine.AddBodyRange(bodies);
                pendingGraphics.AddRange(collection);
            }
        }
        public void Draw(DrawInfo drawInfo)
        {
            rwLock.EnterWrite();
            try
            {
                RemoveExpired();
                lock (syncRoot)
                {
                    AddPending();
                }
                if (zOrderChanged)
                {
                    zOrderChanged = false;
                    graphics.Sort(zOrderComparer);
                }
                for (int index = 0; index < graphics.Count; ++index)
                {
                    IGraphic graphic = graphics[index];
                    if (graphic.ShouldDraw)
                    {
                        graphic.Draw(drawInfo);
                    }
                }
            }
            finally
            {
                rwLock.ExitWrite();
            }
            if (overlay != null)
            {
                overlay.Draw(drawInfo);
            }
        }
        void AddPending()
        {
            if (pendingGraphics.Count > 0)
            {
                graphics.AddRange(pendingGraphics);
                pendingGraphics.Clear();
                zOrderChanged = true;
            }
        }
        void RemoveExpired()
        {
            graphics.RemoveAll(IsGraphicExpired);
        }
        protected virtual void Update(Scalar dt,Scalar trueDt)
        {
            engine.Update(dt, trueDt);
        }
        public virtual void Begin()
        {
            timer.IsRunning = true;
            if (overlay != null)
            {
                overlay.Begin();
            }
        }
        public virtual void End()
        {
            timer.IsRunning = false;
            if (overlay != null)
            {
                overlay.End();
            }
        }
        public void Clear()
        {
            rwLock.EnterWrite();
            try
            {
                graphics.Clear();
                engine.Clear();
                lock (syncRoot)
                {
                    pendingGraphics.Clear();
                }
            }
            finally
            {
                rwLock.ExitWrite();
            }

        }
    }
}