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
using Tao.OpenGl;
namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Advance", "Split Screen Flight", "TODO")]
    public class SplitScreenFlightDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            IShape bombShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("rocket.png"), 2, 16, 3);
            IShape shape = ShapeFactory.CreateColoredCircle(8, 15);
            DemoHelper.AddGrid(DemoInfo, shape, 40,
                new BoundingRectangle(200, 200, 800, 500),
                5, 5);

            Viewport viewport2 = Window.CreateViewport(new Rectangle(Window.Size.Width / 2, 0, Window.Size.Width / 2, Window.Size.Height), Matrix2x3.Identity, Layer);

            Viewport.Rectangle = new Rectangle(0, 0, Window.Size.Width / 2, Window.Size.Height);

            IShape furyShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("starfury.png"), 2, 16, 3);
            Body b1 = DemoHelper.AddShape(DemoInfo, furyShape, 400, new ALVector2D(0, 0, 0));
            Body b2 = DemoHelper.AddShape(DemoInfo, furyShape, 400, new ALVector2D(0, 0, 0));


            dispose += DemoHelper.RegisterBodyTracking(DemoInfo, b1, Matrix2x3.FromRotationZ(-MathHelper.PiOver2));
            dispose += DemoHelper.RegisterBodyMovement(DemoInfo, b1, new ALVector2D(1000000, 100000, 0), Key.W, Key.S, Key.A, Key.D);


            dispose += DemoHelper.RegisterBodyTracking(
                new DemoOpenInfo(Window, viewport2, Layer)
                , b2, Matrix2x3.FromRotationZ(-MathHelper.PiOver2));
            dispose += DemoHelper.RegisterBodyMovement(
                DemoInfo, b2, new ALVector2D(1000000, 100000, 0));

            b1.State.Position.Linear = new Vector2D(200, 0);
            b2.State.Position = new ALVector2D(MathHelper.Pi, 1, 0);
            b1.ApplyPosition();
            b2.ApplyPosition();

            DemoHelper.AddShell(DemoInfo, new BoundingRectangle(-700, -700, 1000, 1000), 90, Scalar.PositiveInfinity);

            dispose += delegate()
            {
                viewport2.IsExpired = true;
                Viewport.Rectangle = new Rectangle(0, 0, Window.Size.Width, Window.Size.Height);
            };

            DemoHelper.AddStarField(DemoInfo, 1000, new BoundingRectangle(-1000, -1000, 1300, 1300));
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}