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
using SdlDotNet.Input;
using SdlDotNet.Core;

namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Simple", "Jump Demo", "Shows you can use the new collision event architecture. Use the arrows to control the character.")]
    public class JumpDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {

            Coefficients coefficients = DemoHelper.Coefficients;
            DemoHelper.Coefficients.Restitution = 0;


            Scene.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));
            DemoHelper.AddFloor(DemoInfo, new ALVector2D(0, new Vector2D(700, 750)));

            int canJump = 0;


            IShape fighter = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("fighter.png"), 3, 16, 3);
            Body body = DemoHelper.AddShape(DemoInfo, fighter, 50, new ALVector2D(0, 500, 300));
            body.Mass.MomentOfInertia = Scalar.PositiveInfinity;

            Scalar desiredVelocity = 0;
            Scalar maxVelocity = 150;

            body.Collided += delegate(object sender, CollisionEventArgs e)
            {
                canJump++;
                e.Contact.Ended += delegate(object sender2, EventArgs e2)
                {
                    canJump--;
                };
            };
            EventHandler<KeyboardEventArgs> keyDownHandler = delegate(object sender, KeyboardEventArgs e)
            {
                if (canJump > 0 && e.Key == Key.UpArrow)
                {
                    body.State.ForceAccumulator.Linear.Y -= 50 * 60000;
                }

                if (e.Key == Key.RightArrow)
                {
                    desiredVelocity += maxVelocity;
                }
                else if (e.Key == Key.LeftArrow)
                {
                    desiredVelocity -= maxVelocity;
                }
            };
            EventHandler<KeyboardEventArgs> keyUpHandler = delegate(object sender, KeyboardEventArgs e)
            {
                if (e.Key == Key.RightArrow)
                {
                    desiredVelocity -= maxVelocity;
                }
                else if (e.Key == Key.LeftArrow)
                {
                    desiredVelocity += maxVelocity;
                }
            };

            body.Updated += delegate(object sender, UpdatedEventArgs e)
            {
                if (canJump > 0)
                {
                    if (desiredVelocity < 0)
                    {
                        if (desiredVelocity < body.State.Velocity.Linear.X)
                        {
                            body.State.ForceAccumulator.Linear.X -= 50 * 6000;
                        }
                    }
                    if (desiredVelocity > 0)
                    {
                        if (desiredVelocity > body.State.Velocity.Linear.X)
                        {
                            body.State.ForceAccumulator.Linear.X += 50 * 6000;
                        }
                    }
                }
            };

            DemoHelper.AddRectangle(DemoInfo, 40, 300, 200, new ALVector2D(0, 200, 600));
            DemoHelper.AddRectangle(DemoInfo, 40, 300, 200, new ALVector2D(0, 400, 400));
            DemoHelper.AddRectangle(DemoInfo, 40, 40, 20, new ALVector2D(0, 100, 100));
            DemoHelper.AddRectangle(DemoInfo, 40, 40, 20, new ALVector2D(0, 200, 200));
            DemoHelper.AddRectangle(DemoInfo, 40, 40, 20, new ALVector2D(0, 300, 300));
            DemoHelper.AddRagDoll(DemoInfo, new Vector2D(700, 300));


            Events.KeyboardDown += keyDownHandler;
            Events.KeyboardUp += keyUpHandler;
            dispose += delegate()
            {
                Events.KeyboardDown -= keyDownHandler;
                Events.KeyboardUp -= keyUpHandler;
            };
            DemoHelper.Coefficients = coefficients;
        }

        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}