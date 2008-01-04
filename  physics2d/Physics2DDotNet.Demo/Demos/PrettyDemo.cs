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
using Graphics2DDotNet;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.PhysicsLogics;
using SdlDotNet;
using SdlDotNet.Graphics;
namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Stress","Pretty Demo", PrettyDemo.Description)]
    public class PrettyDemo : BaseDemo
    {
        public const string Description = 
@"
This Demo shows something pretty
A large number of particles being pushed arround by 2 spinning guys
";

        DisposeCallback dispose;
        protected override void Open()
        {
            dispose += DemoHelper.BasicDemoSetup(DemoInfo);

            IShape fighterShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("fighter.png"), 4, 50, 5);
            Body fighter = new Body(new PhysicsState(new ALVector2D(0, 300, 300)), fighterShape, 5000, new Coefficients(0, 1), new Lifespan());
            fighter.State.Velocity.Angular = 9;
            fighter.Mass.MomentOfInertia = Scalar.PositiveInfinity;
            
            BodyGraphic fighterGraphic = new BodyGraphic(fighter);
            Layer.AddGraphic(fighterGraphic);

            BodyGraphic fighterGraphic2 = (BodyGraphic)fighterGraphic.Duplicate();
            fighterGraphic2.Body.State.Position.Linear.Y = 500;
            fighterGraphic2.Body.State.Velocity.Angular = -8.5f;
            Layer.AddGraphic(fighterGraphic2);

            ParticleShape particleShape = new ParticleShape();
            particleShape.Tag = DrawableFactory.CreateSprite(Cache<Surface>.GetItem("particle.png"), new Vector2D(8, 8));

            Body template2 = new Body(new PhysicsState(new ALVector2D(0, 200, 200)), particleShape, 4, new Coefficients(0, 1), new Lifespan());
            Matrix2x3 m = Matrix2x3.FromRotationZ(2);
            int count = 0;
            for (int x = -0; x < 1000; x += 25)
            {
                for (int y = -0; y < 1000; y += 25)
                {
                    count++;
                    Body body = template2.Duplicate();
                    body.State.Position.Linear = new AdvanceMath.Vector2D(x, y);
                    body.ApplyPosition();
                    body.State.Velocity.Linear.X = (250 - x) / 10f;
                    body.State.Velocity.Linear.Y = (250 - y) / 10f;
                    body.State.Velocity.Linear = m * body.State.Velocity.Linear;
                    body.LinearDamping = .9999f;
                    BodyGraphic g1 = new BodyGraphic(body);
                    Layer.AddGraphic(g1);
                }
            }
            Layer.Engine.AddLogic(new GravityPointField(new Vector2D(400, 400), 500, new Lifespan()));

        }

        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}