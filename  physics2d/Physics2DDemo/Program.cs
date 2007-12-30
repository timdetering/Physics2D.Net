#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
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




using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Drawing;
using AdvanceMath;
//using Graphics2DDotNet;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.PhysicsLogics;
namespace Physics2DDemo
{
    class Program
    {
        /*static int direction = -1;
static float value = 1;
static void port_BeginDrawing(object sender, EventArgs e)
{
    Viewport viewport = sender as Viewport;
    if (direction < 0)
    {
        value /= 1.1f;
    }
    else
    {
        value *= 1.1f;
    }
    if (value > 1) { direction = -1; value = 1; }
    if (value <= .1f) { direction = 1; value = .1f; }
    viewport.ToScreen  = Matrix2x3.FromScale(new Vector2D(value, value));
}*/

        [STAThread]
        static void Main(string[] args)
        {
           /* GraphicsEngine engine = new GraphicsEngine(new System.Drawing.Size(800, 800));
            Layer layer = new Layer();
            layer.Engine.BroadPhase = new Physics2DDotNet.Detectors.SelectiveSweepDetector();
            layer.Engine.Solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();
            
            CircleShape shape = new CircleShape(10,20);
            ScalarColor3[] colors = new ScalarColor3[shape.Vertexes.Length];
            colors[0].Red = 1;
            colors[0].Blue = .2f;
            for (int index = 1; index < colors.Length; ++index)
            {
                colors[index].Blue = 1;
                colors[index].Red = 1;
                colors[index].Green = 1;
            }

            Colored3PolygonDrawable drawable = new Colored3PolygonDrawable(shape.Vertexes, colors);
            shape.Tag = drawable;
            Body template = new Body(new PhysicsState(new ALVector2D(0,200,200)),shape,4,new Coefficients(1,1),new Lifespan());
            Matrix2x3 m = Matrix2x3.FromRotationZ(2);
            int count = 0;
            for (int x = -800; x < 800; x += 25)
            {
                for (int y = -800; y < 800; y += 25)
                {
                    count++;
                    Body body = template.Duplicate();
                    body.State.Position.Linear = new AdvanceMath.Vector2D(x, y);
                    body.ApplyPosition();
                    body.State.Velocity.Linear.X = (250 - x) / 1f;
                    body.State.Velocity.Linear.Y = (250 - y) / 1f;
                    body.State.Velocity.Linear = m * body.State.Velocity.Linear;
                    layer.AddGraphic(new BodyGraphic(body));
                }
            }
            Console.WriteLine(count);

            layer.Engine.AddLogic(new GravityPointField(new Vector2D(300, 300), 50, new Lifespan()));

            Viewport port = engine.CreateViewport(new Rectangle(0, 0, 800,800), Matrix2x3.Identity, layer);
            //port.ToScreen = Matrix2x3.FromTranslate2D(new Vector2D(450, 450)) * Matrix2x3.FromScale(new Vector2D(.1f, .1f));
            port.ToScreen = Matrix2x3.FromRotationZ(.1f);
            //Viewport port2 = engine.CreateViewport(new Rectangle(0, 0, 50, 50), Matrix2x3.Identity, layer);
           // port2.Projection = Matrix2x3.FromScale(new Vector2D(.1f,.1f));


            port.BeginDrawing += new EventHandler(port_BeginDrawing);
            engine.Intialize();
            layer.Begin();
            engine.Run();
            return;*/

            Console.WriteLine("Welcome to the Physics2D.Net Demo");

            Console.WriteLine("In the demo pressing the number keys or W-O will load different demos.");
            Console.WriteLine("Left Clicking will allow you to pick objects up.");
            Console.WriteLine("Middle clicking on the screen will launch a projectile where you click.");
            Console.WriteLine("Right clicking and holding will shoot out particles where you click.");
            Console.WriteLine("holding M will place 3 rotating rays that shoot out particles on impact.");
            Console.WriteLine("The left and right arrow keys will move the tank.");
            Console.WriteLine("SpaceBar will fire the tanks cannon.");
            Console.WriteLine("In the upper left corner a small colored box will appear.");
            Console.WriteLine("If it is green then the engine has too little to do and is skipping a timestep");
            Console.WriteLine("If it is red then the engine has too much to do.");
            Console.WriteLine("P or Pause will pause the simulation.");
            Console.WriteLine("Press Enter To Start");
            Console.ReadLine();

            OpenGlWindow g = new OpenGlWindow(1024, 768);
            Demo demo = new Demo();
            g.Draw  += demo.Draw;
            g.ReShape += demo.Reshape;
            g.Run();
        }

    }
}
