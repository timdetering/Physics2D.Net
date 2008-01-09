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
    [PhysicsDemo("Simple", "Newton’s Cradle", "Its newtons cradle, now in 2D!")]
    public class NewtonsCradleDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            dispose += DemoHelper.BasicDemoSetup(DemoInfo);
            
            
            Scene.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));

            for (int x = 0; x < 400; x += 45)
            {
                DemoHelper.AddRectangle(DemoInfo, 80, 15, 90, new ALVector2D(0, x, 145));
            }

            DemoHelper.AddLine(DemoInfo, new Vector2D(-40, 200), new Vector2D(400, 200), 30, Scalar.PositiveInfinity).IgnoresGravity = true;

            DemoHelper.AddLine(DemoInfo, new Vector2D(400, 150), new Vector2D(430, 150), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(430, 150), new Vector2D(600, 200), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(600, 200), new Vector2D(700, 300), 30, Scalar.PositiveInfinity).IgnoresGravity = true;

            DemoHelper.AddLine(DemoInfo, new Vector2D(1200, 200), new Vector2D(800, 420), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(800, 420), new Vector2D(700, 470), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(700, 470), new Vector2D(600, 486), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(600, 486), new Vector2D(570, 486), 30, Scalar.PositiveInfinity).IgnoresGravity = true;

            Scalar rest = DemoHelper.Coefficients.Restitution;
            DemoHelper.Coefficients.Restitution = 1;
            DemoHelper.AddCircle(DemoInfo, 20, 20, 300, new ALVector2D(0, 409, 115));


            for (int x = 160; x < 500; x += 42)
            {
                Body b = DemoHelper.AddCircle(DemoInfo, 20, 20, 300, new ALVector2D(0, x, 450));
                Scene.Engine.AddJoint(new FixedHingeJoint(b, b.State.Position.Linear - new Vector2D(0, 500), new Lifespan()));
            }
            DemoHelper.Coefficients.Restitution = rest;

        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}