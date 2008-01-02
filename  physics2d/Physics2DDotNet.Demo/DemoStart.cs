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
using System.Drawing;
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
    public static class DemoStart
    {
        /// <summary>
        /// Call this to start the demo
        /// (Entry Point)
        /// </summary>
        public static void Run()
        {
            //do this becuase .Net has been erroring with file permissions
            //just leave this line it.
            string dir = Settings.DataDir;

            //Create a new Layer 
            Layer layer = new Layer();
            //Get the layers physics engine
            PhysicsEngine physicsEngine = layer.Engine;
            //initialize the engine 
            physicsEngine.BroadPhase = new Physics2DDotNet.Detectors.SelectiveSweepDetector();
            physicsEngine.Solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();

            //graphics engine is basically a window with some events that draws veiwports
            Window window = new Window(new System.Drawing.Size(1000, 750));
            window.Title = "Physics2D.Net Demo";

            Viewport viewport = window.CreateViewport(
                new Rectangle(0, 0, window.Size.Width, window.Size.Height), //where
                Matrix2x3.Identity, //how
                layer); //who

            // you can change the veiwport via this 
            viewport.ToScreen = Matrix2x3.FromTransformation(.09f,new Vector2D(40,0));
            
            //make it so the veiwport will be resized whn the window is
            window.Resized += delegate(object sender, SizeEventArgs e)
            {
                viewport.Width = e.Width;
                viewport.Height = e.Height;
            };
            System.Windows.Forms.Application.EnableVisualStyles();
            //create the GUI
            DemoSelector selector = new DemoSelector();
            //initialize the GUI
            selector.Initialize(window, viewport, layer);
            //Create the window
            window.Intialize();

            //Add some intro text
            DemoHelper.AddText(
                new DemoOpenInfo(window,viewport,layer),
@"WELCOME TO THE PHYSICS2D.NET DEMO.
PLEASE ENJOY MESSING WITH IT.
A LOT OF HARD WORK WENT INTO IT,
SO THAT YOU COULD ENJOY IT.
PLEASE SEND FEEDBACK.
EACH CHARACTER HERE IS AN
ACTUAL BODY IN THE ENGINE.
THIS IS TO SHOW OFF THE BITMAP
TO POLYGON ALGORITHM.
LOAD THE INTRO TEXT DEMO TO MANIPULATE 
THIS TEXT.", new Vector2D(20, 20));



            //Show the GUI
            selector.Show();
            //start the physicstimer for Layer.PhysicsEngine
            layer.Begin();
            //Begin the rendering loop. 
            window.Run();
            return;
        }
    }
}