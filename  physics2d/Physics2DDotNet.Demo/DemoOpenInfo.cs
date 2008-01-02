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
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Physics2DDotNet;
using Physics2DDotNet.PhysicsLogics;
using AdvanceMath;
using AdvanceMath.Geometry2D;

using System.Media;
using Tao.OpenGl;
using SdlDotNet.Core;
using SdlDotNet.Input;
using SdlDotNet.OpenGl;
using SdlDotNet.Graphics;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Ignorers;
using Graphics2DDotNet;

namespace Physics2DDotNet.Demo
{
    public class DemoOpenInfo
    {
        Window window;
        Viewport viewport;
        Layer layer;
        public DemoOpenInfo(Window window, Viewport viewport, Layer layer)
        {
            this.window = window;
            this.viewport = viewport;
            this.layer = layer;
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
    }


}