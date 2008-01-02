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
    [PhysicsDemo("Simple", "One Way Platform", "The platform allows objects to go up but not down.")]
    public class OneWayPlatformDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            Layer.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));

            Shape bombShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("rocket.png"), 2, 16, 3);
            dispose += DemoHelper.RegisterBombLaunching(DemoInfo, bombShape, 120);
            dispose += DemoHelper.CreateTank(DemoInfo, new Vector2D(50, 0));
            dispose += DemoHelper.RegisterMousePicking(DemoInfo);
            DemoHelper.AddFloor(DemoInfo, new ALVector2D(0, new Vector2D(700, 750)));

            Shape shape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("fighter.png"), 3, 16, 4);
            for (int i = 128 * 3; i > -128; i -= 128)
            {
                Body b = DemoHelper.AddShape(DemoInfo, shape, 40, new ALVector2D(1, new Vector2D(700, 272 + i)));
                //b.Transformation *= Matrix2x3.FromScale(new Vector2D(1, .5f));
            }

            Body line = DemoHelper.AddLine(DemoInfo, new Vector2D(300, 400), new Vector2D(400, 400), 20, Scalar.PositiveInfinity);
            line.IgnoresGravity = true;
            line.CollisionIgnorer = new OneWayPlatformIgnorer(-Vector2D.YAxis, 10);

            Body ball = DemoHelper.AddCircle(DemoInfo, 80, 20, 4000, new ALVector2D(0, 1028, 272));// //AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(1028, 272)));
            ball.Transformation *=
                Matrix2x3.FromRotationZ(1) *
                Matrix2x3.FromScale(new Vector2D(.9f, .5f)) *
                Matrix2x3.FromRotationZ(-1);
        }




        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}