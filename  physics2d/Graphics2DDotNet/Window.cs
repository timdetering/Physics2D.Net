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
    public class Window
    {
        class ZOrderComparer : IComparer<Viewport>
        {
            public int Compare(Viewport x, Viewport y)
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

        bool IsViewportExpired(Viewport item)
        {
            if (item.IsExpired)
            {
                item.ZOrderChanged -= OnZOrderChanged;
                item.OnRemoved();
                return true;
            }
            return false;
        }

        public event EventHandler<SizeEventArgs> Resized;
        object syncRoot;
        Surface screen;
        Size size;
        bool isResized;

        List<Viewport> pendingViewports;
        List<Viewport> viewports;

        int drawCount;
        int refreshCount;
        PhysicsTimer drawTimer;
        bool isRunning;
        bool zOrderChanged;
        public Window(Size size)
        {
            this.size = size;
            this.drawTimer = new PhysicsTimer(GraphicsProcess, .01f);
            this.pendingViewports = new List<Viewport>();
            this.viewports = new List<Viewport>();
            this.syncRoot = new object();
        }
        public string Title
        {
            get { return Video.WindowCaption; }
            set { Video.WindowCaption = value; }
        }
        public Size Size { get { return size; } }
        public Scalar DrawingInterval
        {
            get { return drawTimer.TargetInterval; }
            set { drawTimer.TargetInterval = value; }
        }
        public bool IsRunning
        {
            get { return isRunning; }
        }
        private void GraphicsProcess(Scalar dt,Scalar trueDt)
        {
            while (Events.Poll()) { }
            if (isResized)
            {
                Init();
                Resize();
                isResized = false;
            }
            GlHelper.DoGlDeleteBuffersARB(refreshCount);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            RemoveExpired();
            AddPending();
            Draw(dt, trueDt);
            Video.GLSwapBuffers();
        }
        void Draw(Scalar dt, Scalar trueDt)
        {
            drawCount++;
            DrawInfo drawInfo = new DrawInfo(dt,trueDt, drawCount, refreshCount);
            if (zOrderChanged)
            {
                zOrderChanged = false;
                viewports.Sort(zOrderComparer);
            }
            foreach (Viewport viewport in viewports)
            {
                viewport.Draw(drawInfo);
            }
        }
        void Init()
        {
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glClearColor(0, 0, 0, 0.5f);
            Gl.glClearDepth(1);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
        }
        void Resize()
        {
            refreshCount++;
            size.Width = screen.Width;
            size.Height = screen.Height;
        }
        bool isIntialized;
        public void Intialize()
        {
            if (!isIntialized)
            {
                isIntialized = true;
                Video.WindowIcon();
                screen = Video.SetVideoMode(size.Width, size.Height, true, true);
                Events.VideoResize += OnVideoResize;
                Events.Quit += OnQuit;
                Init();
            }
        }


        public void Run()
        {
            isRunning = true;
            Intialize();
            drawTimer.RunOnCurrentThread();
            isRunning = false;
            Events.Close();
        }

        void OnQuit(object sender, QuitEventArgs e)
        {
            Quit();
            Events.QuitApplication();
        }
        void OnVideoResize(object sender, VideoResizeEventArgs e)
        {
            isResized = e.Height != size.Height || e.Width != size.Width;
            if (isResized)
            {
                screen = Video.SetVideoMode(e.Width, e.Height, true, true);
                if (Resized != null) { Resized(this, new SizeEventArgs(e.Width, e.Height)); }
            }
        }
        public void Quit()
        {
            this.drawTimer.Dispose();
        }

        void RemoveExpired()
        {
            viewports.RemoveAll(IsViewportExpired);
        }
        void AddPending()
        {
            lock (syncRoot)
            {
                if (pendingViewports.Count > 0)
                {
                    viewports.AddRange(pendingViewports);
                    foreach (Viewport item in pendingViewports)
                    {
                        item.ZOrderChanged += OnZOrderChanged;

                    }
                    pendingViewports.Clear();
                    zOrderChanged = true;
                }
            }
        }
        void OnZOrderChanged(object sender, EventArgs e)
        {
            this.zOrderChanged = true;
        }
        public Viewport CreateViewport(Rectangle rectangle, Matrix2x3 projection, Layer layer)
        {
            Viewport viewport = new Viewport(this, rectangle, projection, layer);
            lock (syncRoot)
            {
                pendingViewports.Add(viewport);
            }
            return viewport;
        }
    }
}