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
    [PhysicsDemo("Stress", "Contained Circles Stress", "TODO")]
    public class CirclesContainedStressDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            dispose += DemoHelper.BasicDemoSetup(DemoInfo);

            Rectangle rect1 = Viewport.Rectangle;
            BoundingRectangle rect = new BoundingRectangle(rect1.Left, rect1.Top, rect1.Right, rect1.Bottom);
            rect.Min.X -= 75;
            rect.Min.Y -= 75;
            rect.Max.X += 75;
            rect.Max.Y += 75;
            DemoHelper.AddShell(DemoInfo, rect, 100, Scalar.PositiveInfinity).ForEach(delegate(Body b) { b.IgnoresGravity = true; });
            rect.Min.X += 110;
            rect.Min.Y += 110;
            rect.Max.X -= 110;
            rect.Max.Y -= 110;
            IShape shape = ShapeFactory.CreateColoredCircle(3, 7);
            DemoHelper.AddGrid(DemoInfo, shape, 40,
                rect,
                1, 1).ForEach(delegate(Body b) { b.State.Velocity.Linear = new Vector2D(DemoHelper.Rand.Next(-100, 100), DemoHelper.Rand.Next(-100, 100)); });
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}