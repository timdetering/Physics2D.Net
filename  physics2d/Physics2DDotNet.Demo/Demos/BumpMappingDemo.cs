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
    [PhysicsDemo("Graphical", "Bump Mapping", "This shows off a new feature in Graphics2D.Net")]
    public class BumpMappingDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            dispose += DemoHelper.BasicDemoSetup(DemoInfo);


            Scene.Engine.AddLogic(new GravityPointField(new Vector2D(500, 500), 1000, new Lifespan()));


            Light light = new Light();
            light.Position.X = 000;
            light.Position.Y = 000;
            light.Position.Z = 0100;

            Body lightBody = new Body(new PhysicsState(), ShapeFactory.CreateCircle(15, 20), 40, new Coefficients(0, 1), new Lifespan());
            BodyGraphic lightGraphic = new BodyGraphic(lightBody);

            lightGraphic.DrawProperties.Add(new Color3Property(1, 1, 1));

            Scene.AddGraphic(lightGraphic);

            lightBody.PositionChanged += delegate(object sender, EventArgs e)
            {
                light.Position = lightBody.State.Position.Linear.ToVector3D(100);
            };

            IShape shape = ShapeFactory.CreateSprite(
                Cache<SurfacePolygons>.GetItem("Monkey.png"),
                Cache<Surface>.GetItem("MonkeyNormal.bmp"),
                false, true,
                3, 8, 16,
                light);
        }

        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}