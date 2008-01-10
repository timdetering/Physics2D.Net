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
using SdlDotNet.Core;
using SdlDotNet.Input;
using SdlDotNet.Graphics;
using Tao.OpenGl;
namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Advance", "Split Screen Flight", SplitScreenFlightDemo.Description)]
    public class SplitScreenFlightDemo : BaseDemo
    {
        #region Description
        public const string Description = @"
In this demo there are 2 ships. Each ship has its own viewport that’s projection matrix is set to the ship’s this make it so the ship always appears facing up and everything else rotates. (unfortunate side effect is that this makes it so the graphics half their speed, so you may want to run it outside of the debugger.)
Both ships can fire. If a ship gets hit by a projectile there is a large explosion (ExplosionLogic) and quite a few particles are added to increase the visual effect. 
After the particles dissipate the ship is re-added at a random location. Its kind of like a game.

Controls:

Left Ship:
Fire: Q
Turn: A, D
Move W, S

Right Ship:
Fire: Enter
Turn: Left, Right 
Move Up, Down
"; 
        #endregion
        private void DoGuns(Body body, Body other, Key fire)
        {
            //this method requires a deep understanding of delegates and events. ENJOY :P
            Scalar projectileSpeed = 500;
            Scalar projectileRadius = 3;
            Scalar projectileMass = 3;

            BoundingRectangle rect;
            Matrix2x3 ident = Matrix2x3.Identity;
            body.Shape.CalcBoundingRectangle(ref ident, out rect);


            DateTime lastFire = DateTime.Now;
            Scalar distance = rect.Max.X + projectileRadius * 2;
            EventHandler<KeyboardEventArgs> keyDown = delegate(object sender, KeyboardEventArgs e)
            {
                DateTime now = DateTime.Now;
                if (e.Key == fire &&
                    !body.Lifetime.IsExpired &&
                    now.Subtract(lastFire).TotalSeconds > .3)
                {
                    lastFire = now;
                    Vector2D normal = body.Matrices.ToWorldNormal * Vector2D.XAxis;
                    Vector2D position = new Vector2D(body.Matrices.ToWorld.m02, body.Matrices.ToWorld.m12) + (normal * distance);
                    Vector2D offset = normal.LeftHandNormal * 3 * projectileRadius;
                    Body projectile1 = DemoHelper.AddCircle(DemoInfo, projectileRadius, 8, projectileMass, new ALVector2D(0, position + offset));
                    Body projectile2 = DemoHelper.AddCircle(DemoInfo, projectileRadius, 8, projectileMass, new ALVector2D(0, position - offset));
                    projectile1.Lifetime.MaxAge = 5;
                    projectile2.Lifetime.MaxAge = 5;
                    Vector2D velocity = normal * projectileSpeed;
                    body.State.Velocity.Linear -= (projectileMass * 2 * velocity) * body.Mass.MassInv;
                    projectile1.State.Velocity.Linear = velocity + body.State.Velocity.Linear;
                    projectile2.State.Velocity.Linear = velocity + body.State.Velocity.Linear;
                    EventHandler<CollisionEventArgs> collided = delegate(object sender1, CollisionEventArgs e1)
                    {
                        Body projectile = (Body)sender1;
                        projectile.Lifetime.IsExpired = true;
                        bool isHit = e1.Other == other || e1.Other == body;

                        List<Body> particles = DemoHelper.AddParticles(
                            DemoInfo,
                            projectile.State.Position.Linear,
                            projectile.State.Velocity.Linear,
                            (isHit) ? (200) : (10));
                        if (isHit && !e1.Other.Lifetime.IsExpired)
                        {
                            ExplosionLogic logic = new ExplosionLogic(e1.Other.State.Position.Linear, e1.Other.State.Velocity.Linear, 4000, 2, e1.Other.Mass.Mass, new Lifespan(.5f));
                            Scene.Engine.AddLogic(logic);

                            e1.Other.Lifetime.IsExpired = true;
                            particles[0].Removed += delegate(object sender2, RemovedEventArgs e2)
                            {
                                if (particles[0].Lifetime.IsExpired)
                                {
                                    e1.Other.State.Position = new ALVector2D(DemoHelper.NextScalar(-600, 900), DemoHelper.NextScalar(-600, 900), DemoHelper.NextScalar(-600, 900));
                                    e1.Other.ApplyPosition();
                                    e1.Other.Lifetime.IsExpired = false;
                                    Scene.AddGraphic(new BodyGraphic(e1.Other));
                                }
                            };
                        }
                    };
                    projectile1.Collided += collided;
                    projectile2.Collided += collided;
                }
            };

            Events.KeyboardDown += keyDown;
            dispose += delegate()
            {
                Events.KeyboardDown -= keyDown;
            };
        }




        DisposeCallback dispose;
        protected override void Open()
        {
            IShape shape = ShapeFactory.CreateColoredCircle(8, 15);
            DemoHelper.AddGrid(DemoInfo, shape, 40,
                new BoundingRectangle(200, 200, 800, 500),
                5, 5);

            Viewport viewport2 = new Viewport(new Rectangle(Window.Size.Width / 2, 0, Window.Size.Width / 2, Window.Size.Height), Matrix2x3.Identity, Scene,new Lifespan());
            Window.AddViewport(viewport2);
            Viewport.Rectangle = new Rectangle(0, 0, Window.Size.Width / 2, Window.Size.Height);

            IShape furyShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("starfury.png"), 2, 16, 3);
            Body b1 = DemoHelper.AddShape(DemoInfo, furyShape, 400, new ALVector2D(0, 0, 0));
            Body b2 = DemoHelper.AddShape(DemoInfo, furyShape, 400, new ALVector2D(0, 0, 0));


            dispose += DemoHelper.RegisterBodyTracking(DemoInfo, b1, Matrix2x3.FromRotationZ(-MathHelper.PiOver2) * Matrix2x3.FromScale(new Vector2D(.5f, .5f)));
            dispose += DemoHelper.RegisterBodyMovement(DemoInfo, b1, new ALVector2D(1000000, 100000, 0), Key.W, Key.S, Key.A, Key.D);
            DoGuns(b1, b2, Key.Q);

            dispose += DemoHelper.RegisterBodyTracking(
                new DemoOpenInfo(Window, viewport2, Scene)
                , b2, Matrix2x3.FromRotationZ(-MathHelper.PiOver2) * Matrix2x3.FromScale(new Vector2D(.5f,.5f)));
            dispose += DemoHelper.RegisterBodyMovement(
                DemoInfo, b2, new ALVector2D(1000000, 100000, 0));
            DoGuns(b2, b1, Key.Return);

            b1.State.Position.Linear = new Vector2D(200, 0);
            b2.State.Position = new ALVector2D(MathHelper.Pi, 1, 0);
            b1.ApplyPosition();
            b2.ApplyPosition();

            DemoHelper.AddShell(DemoInfo, new BoundingRectangle(-700, -700, 1000, 1000), 90, Scalar.PositiveInfinity);

            dispose += delegate()
            {
                viewport2.Lifetime.IsExpired = true;
                Viewport.Rectangle = new Rectangle(0, 0, Window.Size.Width, Window.Size.Height);
            };

            DemoHelper.AddStarField(DemoInfo, 1000, new BoundingRectangle(-1000, -1000, 1300, 1300));
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}