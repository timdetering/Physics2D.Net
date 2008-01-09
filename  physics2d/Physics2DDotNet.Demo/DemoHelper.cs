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
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Physics2DDotNet;
using Physics2DDotNet.PhysicsLogics;
using AdvanceMath;
using AdvanceMath.Geometry2D;

using System.Media;
using Tao.OpenGl;
using SdlDotNet.Core;
using SdlDotNet.Input;
using SdlDotNet.OpenGl;
using SdlDotNet.Graphics;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Ignorers;
using Graphics2DDotNet;

namespace Physics2DDotNet.Demo
{
    public delegate void DisposeCallback();
    public delegate IPhysicsEntity SpawnCallback(Vector2D position);

    public static class DemoHelper
    {
        public static Coefficients Coefficients = new Coefficients(.5f, 1);

        public static readonly Random Rand = new Random();

        static DemoHelper()
        {
            ParticleShape.Default.Tag = DrawableFactory.CreateSprite(Cache<Surface>.GetItem("particle.png"), new Vector2D(8, 8));
        }

        public static Scalar NextScalar()
        {
            return (Scalar)Rand.NextDouble();
        }
        public static Scalar NextScalar(Scalar minValue, Scalar maxValue)
        {
            return minValue + ((Scalar)Rand.NextDouble()) * (maxValue - minValue);
        }
        public static DisposeCallback BasicDemoSetup(DemoOpenInfo info)
        {
            DisposeCallback dispose = null;
            IShape bombShape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("rocket.png"), 2, 16, 3);
            dispose += DemoHelper.RegisterBombLaunching(info, bombShape, 120);
            dispose += DemoHelper.RegisterMousePicking(info);

            dispose += DemoHelper.RegisterBodyStreamSpawning(info,
                new Body(new PhysicsState(), ParticleShape.Default, 1, Coefficients.Duplicate(), new Lifespan(.5f)), 2, 120, 1000, Key.B);
            dispose += DemoHelper.RegisterMaintainSpawning(info, SdlDotNet.Input.Key.N,
                delegate(Vector2D position)
                {
                    ExplosionLogic result = new ExplosionLogic(position, Vector2D.Zero, 9000, .4f, 600, new Lifespan(.5f));
                    info.Scene.Engine.AddLogic(result);
                    return result;
                });

            List<RaySegment> segments = new List<RaySegment>();

            for (Scalar angle = 0; angle < MathHelper.PiOver2; angle += .05f)
            {
                RaySegment seg = new RaySegment();
                seg.Length = 500;
                seg.RayInstance = new Ray(Vector2D.Zero, Vector2D.FromLengthAndAngle(1, angle));
                segments.Add(seg);
            }

            IShape rayShape = ShapeFactory.CreateRays(segments.ToArray());
            dispose += DemoHelper.RegisterMaintainSpawning(info, SdlDotNet.Input.Key.M,
                delegate(Vector2D position)
                {
                    Body lazer = new Body(new PhysicsState(), rayShape, 1, new Coefficients(1, 1), new Lifespan());
                    lazer.State.Position.Linear = position;
                    lazer.State.Velocity.Angular = .91f;
                    lazer.IgnoresGravity = true;
                    lazer.ApplyPosition();
                    info.Scene.AddGraphic(new BodyGraphic(lazer));
                    return lazer;
                });



            return dispose;
        }

        public static DisposeCallback CreateTank(DemoOpenInfo info, Vector2D position)
        {
            return CreateTank(info, position, new List<Body>());
        }
        public static DisposeCallback CreateTank(DemoOpenInfo info, Vector2D position,List<Body> result)
        {
            Lifespan avatarLifespan = new Lifespan();

            IShape shape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("tank.png"), 4, 18, 2);

            ObjectIgnorer ignorer = new ObjectIgnorer();
            Body tankBody = new Body(new PhysicsState(new ALVector2D(0, 0, 0)),
                shape,
                300,//new MassInfo(40, Scalar.PositiveInfinity),
                new Coefficients(0, 1),
                avatarLifespan);
            result.Add(tankBody);
            tankBody.State.Position.Linear += position;
            tankBody.ApplyPosition();

            tankBody.CollisionIgnorer = ignorer;
            BodyGraphic graphic = new BodyGraphic(tankBody);
            graphic.ZOrder = 2;
            info.Scene.AddGraphic(graphic);

            Scalar wheelSize = 18;
            Scalar wheelSpacing = -9;
            Scalar lenghtPercent = .84f;
            Matrix2x3 ident = Matrix2x3.Identity;
            BoundingRectangle rect;
            shape.CalcBoundingRectangle(ref ident, out rect);
            Scalar y = (rect.Max.Y + 4);
            Body lastWheel = null;
            BoundingPolygon polygon = new BoundingPolygon(shape.Vertexes);

            Ray ray2 = new Ray(new Vector2D(rect.Max.X, y), -Vector2D.YAxis);
            Scalar y3 = y - polygon.Intersects(ray2);
            Vector2D avatarBarrelOffset = new Vector2D(rect.Max.X + 10, y3);

            CircleShape wheelShape = ShapeFactory.CreateColoredCircle(wheelSize, 30);
            Scalar force = 0;

            for (Scalar x = rect.Min.X + wheelSize; x < (rect.Max.X - wheelSize) * lenghtPercent; x += (wheelSize * 2 + wheelSpacing))
            {

                Ray ray = new Ray(new Vector2D(x, y), -Vector2D.YAxis);
                Scalar y2 = y - polygon.Intersects(ray);



                Vector2D offset = new Vector2D(x, y2);

                Body wheel = new Body(
                    new PhysicsState(new ALVector2D(0, offset + position)),
                    wheelShape,
                    10,
                    new Coefficients(0, 3),//  coefficients.Duplicate(),
                    avatarLifespan);
                result.Add(wheel);

                wheel.CollisionIgnorer = ignorer;
                wheel.AngularDamping = .9f;
                wheel.Updated += delegate(object sender, UpdatedEventArgs e)
                {
                    wheel.State.ForceAccumulator.Angular += force;
                };
                info.Scene.AddGraphic(new BodyGraphic(wheel));

                HingeJoint joint = new HingeJoint(tankBody, wheel, offset + position, avatarLifespan);
                joint.Softness = .1f;
                info.Scene.Engine.AddJoint(joint);

                if (lastWheel != null)
                {
                    AngleJoint joint2 = new AngleJoint(lastWheel, wheel, avatarLifespan);
                    info.Scene.Engine.AddJoint(joint2);
                }
                lastWheel = wheel;
            }


            CircleShape weaponShape = ShapeFactory.CreateColoredCircle(5, 8);

            //now begins the abuse of anominous delegates (BIG TIME)

            EventHandler<KeyboardEventArgs> keyDownHandler = delegate(object sender, KeyboardEventArgs e)
            {
                switch (e.Key)
                {
                    case Key.LeftArrow:
                        force = -1500000;
                        break;
                    case Key.RightArrow:
                        force = 1500000;
                        break;
                    case Key.Space:

                        Scalar velocity = 2000;

                        Matrix2x3 toWorld = tankBody.Matrices.ToWorld;
                        Matrix2x2 toWorldNormal = tankBody.Matrices.ToWorldNormal;

                        //  Matrix2D mat = avatarBodies[0].Matrices.ToWorld;
                        Vector2D direction = toWorldNormal * Vector2D.XAxis;
                        PhysicsState state = new PhysicsState();
                        state.Position.Linear = toWorld * (avatarBarrelOffset);
                        state.Velocity.Linear = velocity * direction + tankBody.State.Velocity.Linear;

                        Body weapon = new Body(state,
                            weaponShape,
                            5,
                            new Coefficients(1, 1),
                            new Lifespan(10));
                        //weapon.CollisionIgnorer = tankBody.CollisionIgnorer;

                        weapon.Collided += delegate(object sender2, CollisionEventArgs e2)
                        {
                            if (!weapon.Lifetime.IsExpired)
                            {
                                weapon.Lifetime.IsExpired = true;
                                AddParticles(info, weapon.State.Position.Linear, weapon.State.Velocity.Linear * .5f, 50);
                            }
                        };

                        //  weapon.Collided += weapon_Collided;
                        tankBody.State.Velocity.Linear -= (velocity * weapon.Mass.Mass * tankBody.Mass.MassInv) * direction;
                        info.Scene.AddGraphic(new BodyGraphic(weapon));
                        break;
                }
            };
            EventHandler<KeyboardEventArgs> keyUpHandler = delegate(object sender, KeyboardEventArgs e)
            {
                switch (e.Key)
                {
                    case Key.LeftArrow:
                        force = 0;
                        break;
                    case Key.RightArrow:
                        force = 0;
                        break;
                }
            };
            Events.KeyboardDown += keyDownHandler;
            Events.KeyboardUp += keyUpHandler;

            return delegate()
            {
                Events.KeyboardDown -= keyDownHandler;
                Events.KeyboardUp -= keyUpHandler;
            };
        }

        public static DisposeCallback RegisterMousePicking(DemoOpenInfo info)
        {
            return RegisterMousePicking(info, MouseButton.PrimaryButton);
        }
        public static DisposeCallback RegisterMousePicking(DemoOpenInfo info, MouseButton button)
        {
            FixedHingeJoint joint = null;

            EventHandler<ViewportMouseButtonEventArgs> mouseDown = delegate(object sender, ViewportMouseButtonEventArgs e)
            {
                if (e.Button == button)
                {
                    IntersectionInfo temp;
                    Body body = null;
                    foreach (Body b in info.Scene.Engine.Bodies)
                    {
                        Vector2D bodyVertex = b.Matrices.ToBody * e.Position;
                        if (b.Shape.CanGetIntersection &&
                           !b.IsBroadPhaseOnly &&
                           !b.IgnoresPhysicsLogics &&
                           b.Shape.TryGetIntersection(bodyVertex, out temp))
                        {
                            body = b;
                            break;
                        }
                    }
                    if (body != null)
                    {
                        joint = new FixedHingeJoint(body, e.Position, new Lifespan());
                        info.Scene.Engine.AddJoint(joint);
                    }
                }
            };
            EventHandler<ViewportMouseMotionEventArgs> mouseMotion = delegate(object sender, ViewportMouseMotionEventArgs e)
            {
                if (joint != null)
                {
                    joint.Anchor = e.Position;
                }
            };
            EventHandler<ViewportMouseButtonEventArgs> mouseUp = delegate(object sender, ViewportMouseButtonEventArgs e)
            {
                if (e.Button == button)
                {
                    if (joint != null)
                    {
                        joint.Lifetime.IsExpired = true;
                        joint = null;
                    }
                }
            };

            info.Viewport.MouseDown += mouseDown;
            info.Viewport.MouseUp += mouseUp;
            info.Viewport.MouseMotion += mouseMotion;
            return delegate()
            {
                info.Viewport.MouseDown -= mouseDown;
                info.Viewport.MouseUp -= mouseUp;
                info.Viewport.MouseMotion -= mouseMotion;
            };
        }
        public static DisposeCallback RegisterBombLaunching(DemoOpenInfo info, IShape shape, Scalar mass)
        {
            return RegisterBombLaunching(info, shape, mass, MouseButton.SecondaryButton);
        }
        public static DisposeCallback RegisterBombLaunching(DemoOpenInfo info, IShape shape, Scalar mass, MouseButton button)
        {
            EventHandler<ViewportMouseButtonEventArgs> mouseDown = delegate(object sender, ViewportMouseButtonEventArgs e)
            {
                if (e.Button == button)
                {
                    Vector2D position = new Vector2D(Rand.Next(0, 1400), 0);
                    Scalar velocityMag = Rand.Next(2000, 3000);
                    Vector2D velocity = Vector2D.SetMagnitude(e.Position - position, velocityMag);
                    Body newbomb = new Body(
                        new PhysicsState(
                            new ALVector2D(velocity.Angle, position),
                            new ALVector2D(0, velocity)),
                            shape, mass,
                            Coefficients.Duplicate(),
                            new Lifespan());
                    info.Scene.AddGraphic(new BodyGraphic(newbomb));
                }
            };
            info.Viewport.MouseDown += mouseDown;
            return delegate()
            {
                info.Viewport.MouseDown -= mouseDown;
            };
        }
        public static DisposeCallback RegisterBodyMovement(DemoOpenInfo info, Body body, ALVector2D force)
        {
            return RegisterBodyMovement(info, body, force, Key.UpArrow, Key.DownArrow, Key.LeftArrow, Key.RightArrow);
        }
        public static DisposeCallback RegisterBodyMovement(
            DemoOpenInfo info, Body body, ALVector2D force,
            Key forward, Key back, Key left, Key right)
        {
            ALVector2D currentForce = ALVector2D.Zero;

            EventHandler<KeyboardEventArgs> downHandler = delegate(object sender, KeyboardEventArgs e)
            {
                if (e.Key == forward)
                {
                    currentForce.Linear += force.Linear;
                }
                else if (e.Key == back)
                {
                    currentForce.Linear -= force.Linear;
                }
                else if (e.Key == left)
                {
                    currentForce.Angular -= force.Angular;
                }
                else if (e.Key == right)
                {
                    currentForce.Angular += force.Angular;
                }
            };
            EventHandler<KeyboardEventArgs> upHandler = delegate(object sender, KeyboardEventArgs e)
            {
                if (e.Key == forward)
                {
                    currentForce.Linear -= force.Linear;
                }
                else if (e.Key == back)
                {
                    currentForce.Linear += force.Linear;
                }
                else if (e.Key == left)
                {
                    currentForce.Angular += force.Angular;
                }
                else if (e.Key == right)
                {
                    currentForce.Angular -= force.Angular;
                }
            };
            EventHandler<UpdatedEventArgs> update = delegate(object sender, UpdatedEventArgs e)
            {
                Vector2D force2 = body.Matrices.ToWorldNormal * currentForce.Linear;
                body.State.ForceAccumulator.Linear += force2;
                body.State.ForceAccumulator.Angular += currentForce.Angular;
            };

            body.Updated += update;
            Events.KeyboardDown += downHandler;
            Events.KeyboardUp += upHandler;
            return delegate()
            {
                body.Updated -= update;
                Events.KeyboardDown -= downHandler;
                Events.KeyboardUp -= upHandler;
            };
        }


        public static DisposeCallback RegisterBodyTracking(DemoOpenInfo info, Body body, Matrix2x3 transformation)
        {
            EventHandler handler = delegate(object sender, EventArgs e)
            {
                info.Viewport.ToScreen =
                    Matrix2x3.FromTranslate2D(new Vector2D(info.Viewport.X + info.Viewport.Width * .5f, info.Viewport.Y + info.Viewport.Height * .5f)) *
                    transformation *
                    body.Matrices.ToBody
                    ;
            };
            body.PositionChanged += handler;
            return delegate()
            {
                body.PositionChanged -= handler;
            };
        }

        public static DisposeCallback RegisterBodyStreamSpawning(
            DemoOpenInfo info, Body body, int count,
            Scalar minVelocity, Scalar maxVelocity, Key key)
        {
            bool isSpawning = false;
            Scalar range = maxVelocity - minVelocity;
            EventHandler<KeyboardEventArgs> downHandler = delegate(object sender, KeyboardEventArgs e)
            {
                if (e.Key == key)
                {
                    isSpawning = true;
                }
            };
            EventHandler<KeyboardEventArgs> upHandler = delegate(object sender, KeyboardEventArgs e)
            {
                if (e.Key == key)
                {
                    isSpawning = false;
                }
            };
            EventHandler<UpdatedEventArgs> updatedHandler = delegate(object sender, UpdatedEventArgs e)
            {
                if (!isSpawning) { return; }
                Vector2D position = info.Viewport.MousePosition;
                if (count == 1)
                {
                    Body dub = body.Duplicate();
                    Vector2D velocityDirection = Vector2D.FromLengthAndAngle(1, NextScalar() * MathHelper.Pi);
                    dub.State.Position.Linear = position + velocityDirection;
                    dub.ApplyPosition();
                    dub.State.Velocity.Linear = velocityDirection * (minVelocity + NextScalar() * range);
                    info.Scene.AddGraphic(new BodyGraphic(dub));
                }
                else
                {
                    Graphic[] graphics = new Graphic[count];
                    for (int index = 0; index < count; ++index)
                    {
                        Body dub = body.Duplicate();
                        Vector2D velocityDirection = Vector2D.FromLengthAndAngle(1, NextScalar() * MathHelper.TwoPi);
                        dub.State.Position.Linear = position + velocityDirection;
                        dub.ApplyPosition();
                        dub.State.Velocity.Linear = velocityDirection * (minVelocity + NextScalar() * range);
                        graphics[index] = new BodyGraphic(dub);
                    }
                    info.Scene.AddGraphicRange(graphics);
                }
            };
            info.Scene.Engine.Updated += updatedHandler;
            Events.KeyboardDown += downHandler;
            Events.KeyboardUp += upHandler;
            return delegate()
            {
                info.Scene.Engine.Updated -= updatedHandler;
                Events.KeyboardDown -= downHandler;
                Events.KeyboardUp -= upHandler;
            };
        }


        public static DisposeCallback RegisterMaintainSpawning(DemoOpenInfo info, Key key, Body body)
        {
            return RegisterMaintainSpawning(info, key,
                delegate(Vector2D position)
                {
                    Body dub = body.Duplicate();
                    dub.State.Position.Linear = position;
                    dub.ApplyPosition();
                    info.Scene.AddGraphic(new BodyGraphic(dub));
                    return dub;
                });
        }
        public static DisposeCallback RegisterMaintainSpawning(DemoOpenInfo info, Key key, SpawnCallback callback)
        {
            IPhysicsEntity current = null;
            EventHandler<KeyboardEventArgs> downHandler = delegate(object sender, KeyboardEventArgs e)
            {
                if (e.Key == key)
                {
                    current = callback(info.Viewport.MousePosition);
                }
            };
            EventHandler<KeyboardEventArgs> upHandler = delegate(object sender, KeyboardEventArgs e)
            {
                if (e.Key == key)
                {
                    if (current != null)
                    {
                        current.Lifetime.IsExpired = true;
                        current = null;
                    }
                }
            };
            Events.KeyboardDown += downHandler;
            Events.KeyboardUp += upHandler;
            return delegate()
            {
                Events.KeyboardDown -= downHandler;
                Events.KeyboardUp -= upHandler;
            };
        }

        public static void AddParticles(DemoOpenInfo info, Vector2D position, Vector2D velocity, int count)
        {
            Graphic[] graphics = new Graphic[count];
            Scalar angle = MathHelper.TwoPi / count;
            for (int index = 0; index < count; ++index)
            {
                Body particle = new Body(
                    new PhysicsState(new ALVector2D(0, position)),
                     ParticleShape.Default,
                    1f,
                   new Coefficients(1, .5f),// coefficients.Duplicate(),
                    new Lifespan(.9f));
                Vector2D direction = Vector2D.FromLengthAndAngle(1, index * angle + ((Scalar)Rand.NextDouble() - .5f) * angle);
                particle.State.Position.Linear += direction;
                particle.State.Velocity.Linear = direction * Rand.Next(200, 1001) + velocity;
                //particle.Collided += new EventHandler<CollisionEventArgs>(particle_Collided);
                BodyGraphic graphic = new BodyGraphic(particle);
                graphics[index] = graphic;
            }
            info.Scene.AddGraphicRange(graphics);
        }
        public static Body AddCircle(DemoOpenInfo info, Scalar radius, int vertexCount, Scalar mass, ALVector2D position)
        {
            CircleShape shape = ShapeFactory.CreateColoredCircle(radius, vertexCount);
            Body result = new Body(new PhysicsState(position), shape, mass, Coefficients.Duplicate(), new Lifespan());
            info.Scene.AddGraphic(new BodyGraphic(result));
            return result;
        }
        public static Body AddRectangle(DemoOpenInfo info, Scalar height, Scalar width, Scalar mass, ALVector2D position)
        {
            Vector2D[] vertexes = VertexHelper.CreateRectangle(width, height);
            vertexes = VertexHelper.Subdivide(vertexes, (height + width) / 9);
            IShape boxShape = ShapeFactory.CreateColoredPolygon(vertexes, Math.Min(height, width) / 5);
            Body body = new Body(new PhysicsState(position), boxShape, mass, Coefficients.Duplicate(), new Lifespan());
            info.Scene.AddGraphic(new BodyGraphic(body));
            return body;
        }
        public static Body AddRectangle(DemoOpenInfo info, BoundingRectangle rect, Scalar mass)
        {
            Scalar width = rect.Max.X - rect.Min.X;
            Scalar heigth = rect.Max.Y - rect.Min.Y;
            Vector2D pos = rect.Min + new Vector2D(width / 2, heigth / 2);
            return AddRectangle(info, heigth, width, mass, new ALVector2D(0, pos));
        }

        public static Body AddLine(DemoOpenInfo info, Vector2D point1, Vector2D point2, Scalar thickness, Scalar mass)
        {
            Vector2D line = point1 - point2;
            Vector2D avg = (point1 + point2) * .5f;
            Scalar length = line.Magnitude;
            Scalar angle = line.Angle;

            Scalar Hd2 = thickness * .5f;
            Scalar Wd2 = length * .5f;

            int curveEdgeCount = 5;
            Scalar da = MathHelper.Pi / curveEdgeCount;

            List<Vector2D> vertexes = new List<Vector2D>();
            vertexes.Add(new Vector2D(Wd2, Hd2));
            vertexes.Add(new Vector2D(-Wd2, Hd2));
            for (Scalar angle2 = MathHelper.PiOver2 + da; angle2 < MathHelper.ThreePiOver2; angle2 += da)
            {
                vertexes.Add(new Vector2D(-Wd2, 0) + Vector2D.FromLengthAndAngle(Hd2, angle2));
            }
            vertexes.Add(new Vector2D(-Wd2, -Hd2));
            vertexes.Add(new Vector2D(Wd2, -Hd2));
            for (Scalar angle2 = -MathHelper.PiOver2 + da; angle2 < MathHelper.PiOver2; angle2 += da)
            {
                vertexes.Add(new Vector2D(Wd2, 0) + Vector2D.FromLengthAndAngle(Hd2, angle2));
            }
            IShape shape = ShapeFactory.CreateColoredPolygon(vertexes.ToArray(), thickness / 4);

            Body body = new Body(
                new PhysicsState(new ALVector2D(0, avg)),
                shape,
                mass,
                Coefficients.Duplicate(),
                new Lifespan());
            body.Transformation = Matrix2x3.FromRotationZ(angle);
            body.ApplyPosition();
            info.Scene.AddGraphic(new BodyGraphic(body));
            return body;
        }
        public static Body AddShape(DemoOpenInfo info, IShape shape, Scalar mass, ALVector2D position)
        {
            Body body = new Body(new PhysicsState(position), shape, mass, Coefficients.Duplicate(), new Lifespan());
            info.Scene.AddGraphic(new BodyGraphic(body));
            return body;
        }
        public static List<Body> AddShell(DemoOpenInfo info, BoundingRectangle rect, Scalar thickness, Scalar mass)
        {
            List<Body> result = new List<Body>();

            result.Add(AddRectangle(info,
                new BoundingRectangle(
                    rect.Min,
                    new Vector2D(rect.Max.X - thickness, rect.Min.Y + thickness)),
                mass));
            result.Add(AddRectangle(info,
                new BoundingRectangle(
                    new Vector2D(rect.Max.X - thickness, rect.Min.Y),
                    new Vector2D(rect.Max.X, rect.Max.Y - thickness)
                    ),
                mass));
            result.Add(AddRectangle(info,
                new BoundingRectangle(
                    new Vector2D(rect.Min.X + thickness, rect.Max.Y - thickness),
                    rect.Max),
                mass));
            result.Add(AddRectangle(info,
                new BoundingRectangle(
                    new Vector2D(rect.Min.X, rect.Min.Y + thickness),
                    new Vector2D(rect.Min.X + thickness, rect.Max.Y)
                    ),
                mass));

            return result;
        }
        public static Body AddFloor(DemoOpenInfo info, ALVector2D position)
        {
            Scalar height = 60;
            Scalar width = 2000;

            Vector2D[] vertexes = VertexHelper.CreateRectangle(width, height);
            IShape boxShape = ShapeFactory.CreateColoredPolygon(vertexes, Math.Min(height, width) / 5);
            Body body = new Body(new PhysicsState(position), boxShape, Scalar.PositiveInfinity, Coefficients.Duplicate(), new Lifespan());
            body.IgnoresGravity = true;
            info.Scene.AddGraphic(new BodyGraphic(body));
            return body;
        }

        public static List<Body> AddGrid(DemoOpenInfo info, IShape shape, Scalar mass, BoundingRectangle rect, Scalar xSpacing, Scalar ySpacing)
        {
            BoundingRectangle shapeRect;
            Matrix2x3 ident = Matrix2x3.Identity;
            shape.CalcBoundingRectangle(ref ident, out shapeRect);
            Vector2D size = shapeRect.Max - shapeRect.Min;
            Vector2D spacing = new Vector2D(xSpacing, ySpacing) + size;
            Vector2D end = rect.Max - size * .5f;
            Vector2D begin = rect.Min + size * .5f;
            Vector2D pos;
            List<Body> result = new List<Body>();
            for (pos.X = begin.X; pos.X <= end.X; pos.X += spacing.X)
            {
                for (pos.Y = begin.Y; pos.Y <= end.Y; pos.Y += spacing.Y)
                {
                    result.Add(AddShape(info, shape, mass, new ALVector2D(0, pos)));
                }
            }
            return result;
        }
        public static List<Body> AddPyramid(DemoOpenInfo info, IShape shape, Scalar mass, BoundingRectangle rect, Scalar xSpacing, Scalar ySpacing)
        {
            BoundingRectangle shapeRect;
            Matrix2x3 ident = Matrix2x3.Identity;
            shape.CalcBoundingRectangle(ref ident, out shapeRect);
            Vector2D size = shapeRect.Max - shapeRect.Min;
            Vector2D spacing = new Vector2D(xSpacing, ySpacing) + size;
            Vector2D end = rect.Max - size * .5f;
            Vector2D begin = rect.Min + size * .5f;
            Vector2D center = (end + begin) * .5f;
            List<Body> result = new List<Body>();
            for (int row = 1; begin.Y + row * spacing.Y < end.Y; row++)
            {
                Scalar start = center.X - ((spacing.X * row - 1) * .5f);

                for (int column = 0; column < row; ++column)
                {
                    Vector2D pos = new Vector2D(start + spacing.X * column, row * spacing.Y + begin.Y);
                    if (pos.X > begin.X && pos.X <= end.X)
                    {
                        result.Add(AddShape(info, shape, mass, new ALVector2D(0, pos)));
                    }
                }
            }
            return result;
        }

        public static List<Body> AddChain(DemoOpenInfo info, Vector2D position, Scalar boxLenght, Scalar boxWidth, Scalar boxMass, Scalar spacing, Scalar length)
        {
            List<Body> bodies = new List<Body>();
            Body last = null;
            for (Scalar x = 0; x < length; x += boxLenght + spacing, position.X += boxLenght + spacing)
            {
                Body current = AddRectangle(info, boxWidth, boxLenght, boxMass, new ALVector2D(0, position));
                bodies.Add(current);
                if (last != null)
                {
                    Vector2D anchor = (current.State.Position.Linear + last.State.Position.Linear) * .5f;
                    HingeJoint joint = new HingeJoint(last, current, anchor, new Lifespan());
                    joint.DistanceTolerance = 10;
                    info.Scene.Engine.AddJoint(joint);
                }
                last = current;
            }
            return bodies;
        }

        public static List<Body> AddRagDoll(DemoOpenInfo info, Vector2D location)
        {
            List<Body> result = new List<Body>();
            Scalar mass = 10;
            Body head = AddCircle(info, 12, 9, mass, new ALVector2D(0, location + new Vector2D(0, 0)));

            Scalar Ld2 = 50 / 2;
            Scalar Wd2 = 25 / 2;
            Vector2D[] vertexes = new Vector2D[]
            {
                new Vector2D(Wd2, Ld2),
                new Vector2D(5, Ld2+7),
                new Vector2D(-5, Ld2+7),
                new Vector2D(-Wd2, Ld2),
                new Vector2D(-Wd2, 0),
                new Vector2D(-(Wd2+4), -Ld2/2+6),
                new Vector2D(-Wd2+2, -Ld2),
                new Vector2D(0, -Ld2),
                new Vector2D(Wd2-2, -Ld2),
                new Vector2D(Wd2+4, -Ld2/2+6),
                new Vector2D(Wd2, 0),
            };

            IShape shape = ShapeFactory.CreateColoredPolygon(vertexes, 5);

            Body torso = AddShape(info, shape, mass * 4, new ALVector2D(0, location + new Vector2D(0, 40)));

            Body ltarm = AddRectangle(info, 10, 30, mass, new ALVector2D(0, location + new Vector2D(-30, 20)));
            Body lbarm = AddRectangle(info, 10, 30, mass, new ALVector2D(0, location + new Vector2D(-65, 20)));

            Body rtarm = AddRectangle(info, 10, 30, mass, new ALVector2D(0, location + new Vector2D(30, 20)));
            Body rbarm = AddRectangle(info, 10, 30, mass, new ALVector2D(0, location + new Vector2D(65, 20)));

            Body ltleg = AddRectangle(info, 40, 15, mass * 2, new ALVector2D(.06f, location + new Vector2D(-10, 95)));
            Body lbleg = AddRectangle(info, 40, 15, mass * 2, new ALVector2D(0, location + new Vector2D(-11, 140)));

            Body rtleg = AddRectangle(info, 40, 15, mass * 1.5f, new ALVector2D(-.06f, location + new Vector2D(10, 95)));
            Body rbleg = AddRectangle(info, 40, 15, mass * 1.5f, new ALVector2D(0, location + new Vector2D(11, 140)));

            result.Add(head);
            result.Add(torso);

            result.Add(ltarm);
            result.Add(lbarm);

            result.Add(rtarm);
            result.Add(rbarm);

            result.Add(ltleg);
            result.Add(lbleg);

            result.Add(rtleg);
            result.Add(rbleg);

            HingeJoint neck = new HingeJoint(head, torso, location + new Vector2D(0, 15), new Lifespan());

            HingeJoint lshoulder = new HingeJoint(ltarm, torso, location + new Vector2D(-18, 20), new Lifespan());
            HingeJoint lelbow = new HingeJoint(ltarm, lbarm, location + new Vector2D(-47, 20), new Lifespan());
            HingeJoint rshoulder = new HingeJoint(rtarm, torso, location + new Vector2D(18, 20), new Lifespan());
            HingeJoint relbow = new HingeJoint(rtarm, rbarm, location + new Vector2D(47, 20), new Lifespan());

            HingeJoint lhip = new HingeJoint(ltleg, torso, location + new Vector2D(-8, 72), new Lifespan());
            HingeJoint lknee = new HingeJoint(ltleg, lbleg, location + new Vector2D(-11, 115), new Lifespan());
            HingeJoint rhip = new HingeJoint(rtleg, torso, location + new Vector2D(8, 72), new Lifespan());
            HingeJoint rknee = new HingeJoint(rtleg, rbleg, location + new Vector2D(11, 115), new Lifespan());
            List<Joint> joints = new List<Joint>();
            joints.Add(neck);
            joints.Add(lelbow);
            joints.Add(rshoulder);

            joints.Add(relbow);
            joints.Add(lshoulder);

            joints.Add(lhip);
            joints.Add(lknee);

            joints.Add(rhip);
            joints.Add(rknee);
            foreach (HingeJoint joint in joints)
            {
                joint.DistanceTolerance = 10;
            }
            info.Scene.Engine.AddJointRange(joints);

            return result;
        }

        public static Vector2D GetOrbitVelocity(Vector2D PosOfAccelPoint, Vector2D PosofShip, Scalar AccelDoToGravity)
        {
            Scalar MyOrbitConstant = 0.63661977236758134307553505349036f; //(area under 1/4 of a sin wave)/(PI/2)

            Vector2D distance = PosOfAccelPoint - PosofShip;
            Scalar distanceMag = distance.Magnitude;
            Vector2D distanceNorm = distance * (1 / distanceMag);

            Scalar VelocityForOrbit = MathHelper.Sqrt(2 * MyOrbitConstant * AccelDoToGravity * distanceMag);
            Vector2D orbitvelocity = distanceNorm.RightHandNormal * VelocityForOrbit;
            return orbitvelocity;
        }

        public static List<Body> AddText(DemoOpenInfo info, string text, Vector2D position,int fontsize)
        {
            Dictionary<char, object[]> chars = new Dictionary<char, object[]>();
            Font font = Cache<Font>.GetItem("FreeSans.ttf:" + fontsize);

            List<Body> result = new List<Body>();
            Scalar initialx = position.X;
            int maxy = 0;
            foreach (char c in text)
            {
                if (c == '\r')
                {
                }
                else if (c == '\n')
                {
                    position.Y += maxy;
                    position.X = initialx;
                }
                else if (char.IsWhiteSpace(c))
                {
                    position.X += font.SizeText("" + c).Width;
                }
                else
                {
                    object[] temp;
                    IShape shape;
                    SurfacePolygons surfacePolygons;
                    if (chars.TryGetValue(c, out temp))
                    {
                        shape = (IShape)temp[0];
                        surfacePolygons = (SurfacePolygons)temp[1];
                    }
                    else
                    {
                        surfacePolygons = Cache<SurfacePolygons>.GetItem(c + "|FreeSans.ttf:" + fontsize, System.Drawing.Color.Black);
                        shape = ShapeFactory.CreateSprite(
                           surfacePolygons,
                           0, 5, 2);
                        temp = new object[] { shape, surfacePolygons };
                        chars.Add(c, temp);
                    }
                    Vector2D offset = surfacePolygons.Offset;
                    Body b = AddShape(info, shape, 40, new ALVector2D(0, position + offset));
                    maxy = Math.Max(maxy, surfacePolygons.Surface.Height);
                    position.X += surfacePolygons.Surface.Width;
                    result.Add(b);
                }
            }
            return result;
        }

        public static void AddStarField(DemoOpenInfo info, int count, BoundingRectangle rect)
        {
            Vector2D[] stars = new Vector2D[count];
            ScalarColor3[] starColors = new ScalarColor3[stars.Length];
            for (int index = 0; index < stars.Length; ++index)
            {
                stars[index] = new Vector2D(NextScalar(rect.Min.X, rect.Max.X), NextScalar(rect.Min.Y, rect.Max.Y));
                starColors[index] = new ScalarColor3(DemoHelper.NextScalar(), DemoHelper.NextScalar(), DemoHelper.NextScalar());
            }
            Colored3VertexesDrawable stardrawable = new Colored3VertexesDrawable(Gl.GL_POINTS, stars, starColors);
            Graphic stargraphic = new Graphic(stardrawable, Matrix2x3.Identity, new Lifespan());
            stargraphic.ZOrder = -1;
            stargraphic.DrawProperties.Add(new PointSizeProperty(1));
            info.Scene.AddGraphic(stargraphic);
        }
    }
}