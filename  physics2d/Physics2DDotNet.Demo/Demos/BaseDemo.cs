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
using System.Threading;
using System.Text;
using System.Drawing;
using AdvanceMath;
using Graphics2DDotNet;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.PhysicsLogics;
using SdlDotNet;
using SdlDotNet.Graphics;
namespace Physics2DDotNet.Demo.Demos
{
    public abstract class BaseDemo : IPhysicsDemo
    {
        Window window;
        Viewport viewport;
        Layer layer;
        DemoOpenInfo demoInfo;
        public void Open(DemoOpenInfo demoInfo)
        {
            this.window = demoInfo.Window;
            this.viewport = demoInfo.Viewport;
            this.layer = demoInfo.Layer;
            this.demoInfo = demoInfo;
            Open();
        }
        public Window Window
        {
            get { return window; }
        }
        public Viewport Viewport
        {
            get { return viewport; }
        }
        public Layer Layer
        {
            get { return layer; }
        }
        public DemoOpenInfo DemoInfo
        {
            get { return demoInfo; }
        }
        protected abstract void Open();
        protected abstract void Dispose(bool disposing);
        public void Dispose()
        {
            Dispose(true);
        }
    }
}