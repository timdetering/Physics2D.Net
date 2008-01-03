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
using SdlDotNet;
using SdlDotNet.Input;
using SdlDotNet.Graphics;
namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Simple", "Fluid Drag Buoyancy", "Fluid Drag Buoyancy, OH MY!")]
    public class FluidDragBuoyancyDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            dispose += DemoHelper.BasicDemoSetup(DemoInfo);


            Layer.Engine.AddLogic(new LineFluidLogic(new Line(0, -1, -400), 1.95f, .02f, new Vector2D(0, 0), new Lifespan()));
            Layer.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));

            Rectangle rect1 = Viewport.Rectangle;


            BoundingRectangle rect = new BoundingRectangle(rect1.Left, rect1.Top, rect1.Right, rect1.Bottom);
            rect.Min.X -= 75;
            rect.Min.Y -= 75;
            rect.Max.X += 75;
            rect.Max.Y += 75;
            DemoHelper.AddShell(DemoInfo, rect, 100, Scalar.PositiveInfinity).ForEach(delegate(Body b) { b.IgnoresGravity = true; });


            Shape shape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("fighter.png"), 3, 16, 4);

            DemoHelper.AddShape(DemoInfo, shape, 200, new ALVector2D(0, new Vector2D(200, 300)));
            DemoHelper.AddShape(DemoInfo, shape, 100, new ALVector2D(0, new Vector2D(500, 300)));
            DemoHelper.AddRectangle(DemoInfo, 50, 200, 25, new ALVector2D(0, 600, 600));
            DemoHelper.AddRectangle(DemoInfo, 50, 100, 50, new ALVector2D(0, 200, 400));
            DemoHelper.AddRectangle(DemoInfo, 50, 100, 50, new ALVector2D(0, 400, 200));

            Vector2D[] waterVertexes = new Vector2D[4]
            {
                 new Vector2D(-10, 400),
                 new Vector2D(10000, 400),
                 new Vector2D(10000, 1000),
                 new Vector2D(-10, 1000)
            };
            ScalarColor3[] waterColor = new ScalarColor3[4]
            {
                new ScalarColor3(0,0,1),
                new ScalarColor3(0,0,1),
                new ScalarColor3(0,0,1),
                new ScalarColor3(0,0,1)
            };
            Colored3PolygonDrawable drawable = new Colored3PolygonDrawable(waterVertexes, waterColor);

            SimpleGraphic graphic = new SimpleGraphic(drawable, Matrix2x3.Identity, new Lifespan());
            graphic.ZOrder = -1;
            Layer.AddGraphic(graphic);
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}