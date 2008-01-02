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
    [PhysicsDemo("Simple", "Suspension Bridge", "TODO")]
    public class SuspensionBridgeDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            Layer.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));
            Shape bombShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("rocket.png"), 2, 16, 3);
            dispose += DemoHelper.RegisterBombLaunching(DemoInfo, bombShape, 120);
            dispose += DemoHelper.CreateTank(DemoInfo, new Vector2D(50, 0));
            dispose += DemoHelper.RegisterMousePicking(DemoInfo);

            Shape shape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("block.png"), 3, 7, 4);
            DemoHelper.AddGrid(
                DemoInfo, shape, 20,
                new BoundingRectangle(400, 200, 500, 510),
                50, 2);


            Scalar boxlength = 50;
            Scalar spacing = 4;
            Scalar anchorLenght = 30;
            Scalar anchorGap = (boxlength / 2) + spacing + (anchorLenght / 2);
            List<Body> chain = DemoHelper.AddChain(DemoInfo, new Vector2D(200, 500), boxlength, 20, 200, spacing, 600);

            Vector2D point2 = new Vector2D(chain[chain.Count - 1].State.Position.Linear.X + anchorGap, 500);
            Body end2 = DemoHelper.AddCircle(DemoInfo, anchorLenght / 2, 14, Scalar.PositiveInfinity, new ALVector2D(0, point2));
            end2.IgnoresGravity = true;
            HingeJoint joint2 = new HingeJoint(chain[chain.Count - 1], end2, point2, new Lifespan());
            joint2.DistanceTolerance = 10;
            Layer.Engine.AddJoint(joint2);

            Vector2D point1 = new Vector2D(chain[0].State.Position.Linear.X - anchorGap, 500);
            Body end1 = DemoHelper.AddCircle(DemoInfo, anchorLenght / 2, 14, Scalar.PositiveInfinity, new ALVector2D(0, point1));
            end1.IgnoresGravity = true;
            HingeJoint joint1 = new HingeJoint(chain[0], end1, point1, new Lifespan());
            joint1.DistanceTolerance = 10;
            Layer.Engine.AddJoint(joint1);
            end2.State.Position.Linear.X -= 10;
            end1.State.Position.Linear.X += 10;
            end2.ApplyPosition();
            end1.ApplyPosition();




        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}