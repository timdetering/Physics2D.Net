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
    [PhysicsDemo("Advance", "Flight", "Fly a rocket with its perspective")]
    public class FlightDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {

            IShape bombShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("rocket.png"), 2, 16, 3);
            dispose += DemoHelper.RegisterBombLaunching(DemoInfo, bombShape, 120);
            dispose += DemoHelper.RegisterMousePicking(DemoInfo);
            IShape shape = ShapeFactory.CreateColoredCircle(8, 15);
            DemoHelper.AddGrid(DemoInfo, shape, 40,
                new BoundingRectangle(200, 200, 1100, 1100),
                5, 5);



            Body b = DemoHelper.AddShape(DemoInfo, bombShape, 400, new ALVector2D(0, 0, 0));
            DemoHelper.RegisterBodyTracking(DemoInfo, b, Matrix2x3.FromRotationZ(-MathHelper.PiOver2));
            DemoHelper.RegisterBodyMovement(DemoInfo, b, new ALVector2D(50000,  50000,0));
            DemoHelper.AddStarField(DemoInfo, 1000, new BoundingRectangle(-1000, -1000, 2300, 2300));

        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }

}