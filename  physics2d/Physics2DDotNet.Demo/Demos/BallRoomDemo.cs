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
    [PhysicsDemo("Stress", "Ball Room", "TODO")]
    public class BallRoomDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            Layer.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));
            Shape bombShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("rocket.png"), 2, 16, 3);
            dispose += DemoHelper.RegisterBombLaunching(DemoInfo, bombShape, 120);
            dispose += DemoHelper.RegisterMousePicking(DemoInfo);

            Coefficients o = DemoHelper.Coefficients;
            DemoHelper.Coefficients = new Coefficients(1, .5f);
            DemoHelper.AddFloor(DemoInfo, new ALVector2D(0, new Vector2D(700, 750)));
            Body b1 = DemoHelper.AddRectangle(DemoInfo, 750, 100, Scalar.PositiveInfinity, new ALVector2D(0, 0, 750 / 2));
            b1.IgnoresGravity = true;
            Body b2 = DemoHelper.AddRectangle(DemoInfo, 750, 100, Scalar.PositiveInfinity, new ALVector2D(0, 1024, 750 / 2));
            b2.IgnoresGravity = true;
            DemoHelper.Coefficients = new Coefficients(.7f, .05f);


            for (int x = 60; x < 80; x += 10)
            {
                for (int y = -2000; y < 700; y += 12)
                {
                    Body g = DemoHelper.AddCircle(DemoInfo, 5, 7, 3, new ALVector2D(0, x, y));
                    g.State.Velocity.Angular = 1;
                    //  g.State.Velocity.Linear = new Vector2D(0, 500);
                }
            }
            DemoHelper.Coefficients = o;
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}