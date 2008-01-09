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
    [PhysicsDemo("Simple", "Towers", "There are many towers.\r\nIts like dominoes, but different!")]
    public class TowersDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {

            dispose += DemoHelper.CreateTank(DemoInfo, new Vector2D(50, 0));
            dispose += DemoHelper.BasicDemoSetup(DemoInfo);

            Scene.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));
            DemoHelper.AddFloor(DemoInfo, new ALVector2D(0, new Vector2D(700, 750)));

            IShape shape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("block.png"), 3, 7, 4);


            DemoHelper.AddGrid(
                DemoInfo, shape, 20,
                new BoundingRectangle(300, 500, 900, 710),
                50, 2);

            Body ball = DemoHelper.AddCircle(DemoInfo, 80, 20, 4000, new ALVector2D(0, 1028, 272));// //AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(1028, 272)));
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}