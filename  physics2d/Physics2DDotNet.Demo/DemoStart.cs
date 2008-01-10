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


            //create a new window to display stuff
            Window window = new Window(new System.Drawing.Size(1000, 750));
            window.Title = "Physics2D.Net Demo";


            //Create a new Scene 
            Scene scene = new Scene();

            //Get the scenes physics engine
            PhysicsEngine physicsEngine = scene.Engine;
            //initialize the engine 
            physicsEngine.BroadPhase = new Physics2DDotNet.Detectors.SelectiveSweepDetector();
            //physicsEngine.BroadPhase = new Physics2DDotNet.Detectors.SpatialHashDetector();
            physicsEngine.Solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();



            Viewport viewport = new Viewport(
                new Rectangle(0, 0, window.Size.Width, window.Size.Height), //where
                Matrix2x3.Identity, //how
                scene, //who
                new Lifespan()); // how long 
            window.AddViewport(viewport);

            // you can change the veiwport via this 
            viewport.ToScreen = Matrix2x3.FromTransformation(.09f, new Vector2D(40, 0));

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
            selector.Initialize(window, viewport, scene);
            //Create the window
            window.Intialize();
            window.DrawingInterval = .02f;
            //Add some intro text


            AddIntoText(window, viewport, scene);
            SetupStatus(window, scene);


            //Show the GUI
            selector.Show();
            //start the physicstimer for Layer.PhysicsEngine
            scene.Timer.IsRunning = true;
            //Begin the rendering loop. 
            window.Run();
            return;
        }

        //below here is code that displays the fps, ups and so on.

        static string numberString = "0123456789";
        static SurfacePolygons[] numbers2;
        static IShape[] numbers;

        private static void AddIntoText(Window window, Viewport viewport, Scene scene)
        {
            DemoHelper.AddText(
    new DemoOpenInfo(window, viewport, scene),
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
THIS TEXT.", new Vector2D(20, 20), 40);
        }
        private static void SetupStatus(Window window, Scene scene)
        {

            Scene scene2 = new Scene();
            PhysicsEngine physicsEngine2 = scene2.Engine;
            physicsEngine2.BroadPhase = new Physics2DDotNet.Detectors.SelectiveSweepDetector();
            //physicsEngine.BroadPhase = new Physics2DDotNet.Detectors.SpatialHashDetector();
            physicsEngine2.Solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();

            Viewport viewport2 = new Viewport(
                    new Rectangle(0, 0, window.Size.Width, window.Size.Height), //where
                    Matrix2x3.Identity, //how
                    scene2, //who
                    new Lifespan()); // how long
            window.AddViewport(viewport2);


            window.Resized += delegate(object sender, SizeEventArgs e)
            {
                viewport2.Width = e.Width;
                viewport2.Height = e.Height;
            };

            numbers = new IShape[10];
            numbers2 = new SurfacePolygons[10];
            for (int index = 0; index < numbers.Length; ++index)
            {
                numbers2[index] = Cache<SurfacePolygons>.GetItem(numberString[index] + "|FreeSans.ttf:20", Color.Black);
                numbers[index] = ShapeFactory.CreateSprite(numbers2[index], 0, 8, 2);
                SpriteDrawable s = numbers[index].Tag as SpriteDrawable;
                s.Color = new ScalarColor4(.1f, .1f, 1, 1);
            }

            DoBodyCount(window, viewport2, scene, scene2, new Vector2D(100, 2));
            DoJointCount(window, viewport2, scene, scene2, new Vector2D(100, 30));
            DoLogicsCount(window, viewport2, scene, scene2, new Vector2D(270, 2));
            DoFPS(window, viewport2, scene, scene2, new Vector2D(2, 2));
            DoUPS(window, viewport2, scene, scene2, new Vector2D(2, 30));
        }

        private static void DoUPS(Window window, Viewport viewport2, Scene scene1, Scene scene2, Vector2D pos)
        {
            List<Body> bodies = DemoHelper.AddText(new DemoOpenInfo(window, viewport2, scene2), "UPS: 000", pos,20);
            foreach (Body body in bodies)
            {
                SpriteDrawable s = body.Shape.Tag as SpriteDrawable;
                s.Color = new ScalarColor4(.1f, .1f, 1, 1);
            }
            bodies.RemoveRange(0, 4);
            Vector2D[] positions = new Vector2D[bodies.Count];
            for (int index = 0; index < positions.Length; index++)
            {
                positions[index] = bodies[index].State.Position.Linear - numbers2[0].Offset;
            }
            int frames = 50;
            Scalar frameSeconds = 1;
            scene1.Engine.Updated += delegate(object sender, UpdatedEventArgs e)
            {
                if (frames >= 10)
                {
                    frames /= 2;
                    frameSeconds /= 2;
                }
                frames++;
                frameSeconds += e.Step.TrueDt;
                int ups = (int)(frames / frameSeconds);
                if (ups < 0) { ups = 0; }
                string val = ups.ToString();
                SetBodiesText(bodies, positions, val);
            };
        }
        private static void DoFPS(Window window, Viewport viewport2, Scene scene1, Scene scene2, Vector2D pos)
        {
            List<Body> bodies = DemoHelper.AddText(new DemoOpenInfo(window, viewport2, scene2), "FPS: 000", pos,20);
            foreach (Body body in bodies)
            {
                SpriteDrawable s = body.Shape.Tag as SpriteDrawable;
                s.Color = new ScalarColor4(.1f, .1f, 1, 1);
            }
            bodies.RemoveRange(0, 4);
            Vector2D[] positions = new Vector2D[bodies.Count];
            for (int index = 0; index < positions.Length; index++)
            {
                positions[index] = bodies[index].State.Position.Linear - numbers2[0].Offset;
            }
            int frames = 100;
            Scalar frameSeconds = 1;
            viewport2.BeginDrawing += delegate(object sender, DrawEventArgs e)
            {
                if (frames >= 10)
                {
                    frames /= 2;
                    frameSeconds /= 2;
                }
                frames++;
                frameSeconds += e.DrawInfo.TrueDt;
                int ups = (int)(frames / frameSeconds);
                if (ups < 0) { ups = 0; }
                string val = ups.ToString();
                SetBodiesText(bodies, positions, val);
            };
        }
        private static void DoLogicsCount(Window window, Viewport viewport2, Scene scene1, Scene scene2, Vector2D pos)
        {
            List<Body> bodies = DemoHelper.AddText(new DemoOpenInfo(window, viewport2, scene2), "Logics: 000000", pos, 20);
            foreach (Body body in bodies)
            {
                SpriteDrawable s = body.Shape.Tag as SpriteDrawable;
                s.Color = new ScalarColor4(.1f, .1f, 1, 1);
            }
            bodies.RemoveRange(0, 7);
            Vector2D[] positions = new Vector2D[bodies.Count];
            for (int index = 0; index < positions.Length; index++)
            {
                positions[index] = bodies[index].State.Position.Linear - numbers2[0].Offset;
            }
            EventHandler<CollectionEventArgs<PhysicsLogic>> handler = delegate(object sender, CollectionEventArgs<PhysicsLogic> e)
            {
                string val = scene1.Engine.Logics.Count.ToString();
                SetBodiesText(bodies, positions, val);
            };
            scene1.Engine.LogicsAdded += handler;
            scene1.Engine.LogicsRemoved += handler;
        }
        private static void DoJointCount(Window window, Viewport viewport2, Scene scene1, Scene scene2, Vector2D pos)
        {
            List<Body> bodies = DemoHelper.AddText(new DemoOpenInfo(window, viewport2, scene2), "Joints: 000000", pos, 20);
            foreach (Body body in bodies)
            {
                SpriteDrawable s = body.Shape.Tag as SpriteDrawable;
                s.Color = new ScalarColor4(.1f, .1f, 1, 1);
            }
            bodies.RemoveRange(0, 7);
            Vector2D[] positions = new Vector2D[bodies.Count];
            for (int index = 0; index < positions.Length; index++)
            {
                positions[index] = bodies[index].State.Position.Linear - numbers2[0].Offset;
            }
            EventHandler<CollectionEventArgs<Joint>> handler = delegate(object sender, CollectionEventArgs<Joint> e)
            {
                string val = scene1.Engine.Joints.Count.ToString();
                SetBodiesText(bodies, positions, val);
            };
            scene1.Engine.JointsAdded += handler;
            scene1.Engine.JointsRemoved += handler;
        }
        private static void DoBodyCount(Window window, Viewport viewport2, Scene scene1, Scene scene2, Vector2D pos)
        {
            List<Body> bodies = DemoHelper.AddText(new DemoOpenInfo(window, viewport2, scene2), "Bodies: 000000", pos,20);
            foreach (Body body in bodies)
            {
                SpriteDrawable s = body.Shape.Tag as SpriteDrawable;
                s.Color = new ScalarColor4(.1f, .1f, 1, 1);
            }
            bodies.RemoveRange(0, 7);
            Vector2D[] positions = new Vector2D[bodies.Count];
            for (int index = 0; index < positions.Length; index++)
            {
                positions[index] = bodies[index].State.Position.Linear - numbers2[0].Offset;
            }
            EventHandler<CollectionEventArgs<Body>> handler = delegate(object sender, CollectionEventArgs<Body> e)
            {
                string val = scene1.Engine.Bodies.Count.ToString();
                SetBodiesText(bodies, positions, val);
            };
            scene1.Engine.BodiesAdded += handler;
            scene1.Engine.BodiesRemoved += handler;
        }
        private static void SetBodiesText(List<Body> bodies, Vector2D[] positions, string val)
        {
            int offset = bodies.Count - val.Length;
            for (int index = 0; index < offset; ++index)
            {
                bodies[index].Shape = numbers[0];
                bodies[index].State.Position.Linear = positions[index] + numbers2[0].Offset;
                bodies[index].ApplyPosition();
            }
            for (int index = 0; index < val.Length; ++index)
            {
                int number = numberString.IndexOf(val[index]);
                bodies[index + offset].Shape = numbers[number];
                bodies[index + offset].State.Position.Linear = positions[index + offset] + numbers2[number].Offset;
                bodies[index + offset].ApplyPosition();
            }
        }
    }
}