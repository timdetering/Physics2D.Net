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
    [PhysicsDemo("Simple", "Movement Demo", "Press H to move one of the balls to where the Mouse is.")]
    public class MovementDemo : BaseDemo
    {
        Random rand = new Random();
        DisposeCallback dispose;
        protected override void Open()
        {
            dispose += DemoHelper.BasicDemoSetup(DemoInfo);

            IShape shape = ShapeFactory.CreateColoredCircle(8, 15);
            List<Body> bodies = DemoHelper.AddGrid(DemoInfo, shape, 40,
                          new BoundingRectangle(100, 100, 600, 600),
                          40, 40);

            MoveToPointLogic logic = null;

            dispose += DemoHelper.RegisterSpawning(this.DemoInfo, SdlDotNet.Input.Key.H,
            delegate(Vector2D position)
            {
                if (logic != null)
                {
                    logic.Lifetime.IsExpired = true;
                }
                logic = new MoveToPointLogic(bodies[0], position, 90, 200000);
                this.DemoInfo.Scene.Engine.AddLogic(logic);
                return logic;
            });

            List<MoveToPointLogic> logics = new List<MoveToPointLogic>();
            dispose += DemoHelper.RegisterSpawning(this.DemoInfo, SdlDotNet.Input.Key.J,
            delegate(Vector2D position)
            {
                foreach (MoveToPointLogic logic2 in logics)
                {
                    logic2.Lifetime.IsExpired = true;
                }
                logics.Clear();

                Vector2D center = Vector2D.Zero;
                foreach (Body body in bodies)
                {
                    center += body.State.Position.Linear;
                }
                center *= (1f/(Scalar)bodies.Count);
                foreach (Body body in bodies)
                {
                    MoveToPointLogic logic2 = new MoveToPointLogic(body, position + body.State.Position.Linear - center, 90000, Scalar.MaxValue, true, new Lifespan());
                    logics.Add(logic2);
                    this.DemoInfo.Scene.Engine.AddLogic(logic2);
                }

                return null;
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}