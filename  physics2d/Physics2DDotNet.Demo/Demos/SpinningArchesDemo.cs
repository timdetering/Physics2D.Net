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
    [PhysicsDemo("Stress", "Spinning Arches", "TODO")]
    public class SpinningArchesDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {

            dispose += DemoHelper.BasicDemoSetup(DemoInfo);


            Vector2D gravityCenter = new Vector2D(500, 500);
            Scalar gravityPower = 200;
            Layer.Engine.AddLogic(new GravityPointField(gravityCenter, gravityPower, new Lifespan()));
            DemoHelper.AddRagDoll(DemoInfo, gravityCenter + new Vector2D(0, -20));
            Scalar length = 41;
            Scalar size = 8
                ;
            bool reverse = false;
            for (Scalar distance = 250; distance < 650; length += 10, size *= 2, distance += 60 + length)
            {

                Scalar da = MathHelper.TwoPi / size;// ((MathHelper.TWO_PI * distance) / size);
                Scalar l2 = length / 2;
                // da /= 2;
                Vector2D[] vertexes = new Vector2D[]
                {
                     Vector2D.FromLengthAndAngle(distance-l2,da/2),
                     Vector2D.FromLengthAndAngle(distance-l2,-da/2),
                     Vector2D.FromLengthAndAngle(distance+l2,-da/2),
                     Vector2D.FromLengthAndAngle(distance+l2,da/2),
                };
                //da *= 2;
                Vector2D[] vertexes2 = PolygonShape.MakeCentroidOrigin(vertexes);
                vertexes = PolygonShape.Subdivide(vertexes2, 5);

                PolygonShape shape = ShapeFactory.CreateColoredPolygon(vertexes, 1.5f);
                for (Scalar angle = 0; angle < MathHelper.TwoPi; angle += da)
                {
                    Vector2D position = Vector2D.FromLengthAndAngle(distance, angle) + gravityCenter;
                    Body body = DemoHelper.AddShape(DemoInfo, shape, (size * length) / 10, new ALVector2D(angle, position));
                    body.State.Velocity.Linear = DemoHelper.GetOrbitVelocity(gravityCenter, Vector2D.FromLengthAndAngle(distance - length, angle) + gravityCenter, gravityPower);
                    body.State.Velocity.Linear *= .5f;
                    body.State.Velocity.Angular = -(body.State.Velocity.Linear.Magnitude) / (distance);// *(1 / MathHelper.TWO_PI);
                    if (reverse)
                    {
                        body.State.Velocity.Linear = -body.State.Velocity.Linear;
                        body.State.Velocity.Angular = -body.State.Velocity.Angular;
                    }
                }
                reverse = !reverse;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}