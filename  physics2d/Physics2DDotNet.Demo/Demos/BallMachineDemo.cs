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
using AdvanceMath.Geometry2D;
using Graphics2DDotNet;
using Physics2DDotNet;
using Physics2DDotNet.Ignorers;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.PhysicsLogics;

namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Stress", "Ball Machine", "May take a while to load")]
    public class BallMachineDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {


            dispose += DemoHelper.BasicDemoSetup(DemoInfo);

            Scene.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));

            SurfacePolygons surfacePolygons = Cache<SurfacePolygons>.GetItem("physicsplayGround.png");
            foreach (Vector2D[] vertexes in surfacePolygons.Polygons)
            {
                Vector2D[] processed = vertexes;
                for (int index = 1; index < 4; index++)
                {
                    processed = VertexHelper.Reduce(processed, index);
                }
                processed = VertexHelper.Subdivide(processed, 16);
                IShape shape = ShapeFactory.CreateColoredPolygon(processed, 10);
                DemoHelper.AddShape(DemoInfo, shape, Scalar.PositiveInfinity, new ALVector2D(0, surfacePolygons.Offset)).IgnoresGravity = true;

            }
            for (int x = 440; x < 480; x += 10)
            {
                for (int y = -2000; y < 0; y += 12)
                {
                    Body body = DemoHelper.AddCircle(DemoInfo, 5, 7, 3, new ALVector2D(0, x + DemoHelper.NextScalar(-400, 400), y));
                    body.Updated += delegate(object sender, UpdatedEventArgs e)
                         {
                             if (body.State.Position.Linear.Y > 900)
                             {
                                 body.State.Position.Linear.Y = -100;
                             }
                         };

                }
            }
            for (int x = 490; x < 510; x += 10)
            {
                for (int y = -550; y < -500; y += 12)
                {
                    Body body = DemoHelper.AddRectangle(DemoInfo, 10, 20, 10, new ALVector2D(0, x + DemoHelper.NextScalar(-400, 400), y));
                    body.Updated += delegate(object sender, UpdatedEventArgs e)
                         {
                             if (body.State.Position.Linear.Y > 900)
                             {
                                 body.State.Position.Linear.Y = -100;
                             }
                         };
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}