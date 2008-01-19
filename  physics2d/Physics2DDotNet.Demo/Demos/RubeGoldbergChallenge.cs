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
namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Advance", "Rube Goldberg Challenge", "TODO")]
    public class RubeGoldbergChallenge : BaseDemo
    {
        public static DisposeCallback RegisterDup(DemoOpenInfo info, List<Body> bodies)
        {
            LinkedList<Body> removable = new LinkedList<Body>();
            Body body = null;
            Scalar rotation = 0;
            Vector2D scale = new Vector2D(1, 1);
            bool duplicate = false;
            bool makeJoint = false;
            Vector2D anchor = Vector2D.Zero;
            int count = 0;
            Body bj1 = null;
            Body bj2 = null;
            EventHandler<ViewportMouseButtonEventArgs> mouseDown = delegate(object sender, ViewportMouseButtonEventArgs e)
            {
                IntersectionInfo temp;
                switch (e.Button)
                {
                    case MouseButton.PrimaryButton:
                        if (makeJoint)
                        {
                            count++;
                            switch (count)
                            {
                                case 1:
                                    for (LinkedListNode<Body> node = removable.First;
                                        node != null;
                                        node = node.Next)
                                    {
                                        Vector2D pos = node.Value.Matrices.ToBody * e.Position;
                                        if (node.Value.Shape.TryGetIntersection(pos, out temp))
                                        {
                                            bj1 = node.Value;
                                            return;
                                        }
                                    }
                                    count--;
                                    break;
                                case 2:
                                    anchor = e.Position;
                                    break;
                                case 3:
                                    for (LinkedListNode<Body> node = removable.First;
                                                                           node != null;
                                                                           node = node.Next)
                                    {
                                        Vector2D pos = node.Value.Matrices.ToBody * e.Position;
                                        if (node.Value.Shape.TryGetIntersection(pos, out temp))
                                        {
                                            bj2 = node.Value;
                                            return;
                                        }
                                    }
                                    count--;
                                    break;
                            }
                        }
                        else
                        {

                            foreach (Body b in bodies)
                            {
                                Vector2D pos = b.Matrices.ToBody * e.Position;
                                if (b.Shape.TryGetIntersection(pos, out temp))
                                {
                                    body = b.Duplicate();
                                    info.Scene.AddGraphic(new BodyGraphic(body));
                                    return;
                                }
                            }
                            for (LinkedListNode<Body> node = removable.First;
                                node != null;
                                node = node.Next)
                            {
                                Vector2D pos = node.Value.Matrices.ToBody * e.Position;
                                if (node.Value.Shape.TryGetIntersection(pos, out temp))
                                {
                                    if (duplicate)
                                    {
                                        body = node.Value.Duplicate();
                                        info.Scene.AddGraphic(new BodyGraphic(body));
                                        return;
                                    }
                                    else
                                    {
                                        body = node.Value;
                                        removable.Remove(node);
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case MouseButton.SecondaryButton:
                        for (LinkedListNode<Body> node = removable.First;
                           node != null;
                           node = node.Next)
                        {
                            Vector2D pos = node.Value.Matrices.ToBody * e.Position;
                            if (node.Value.Shape.TryGetIntersection(pos, out temp))
                            {
                                node.Value.Lifetime.IsExpired = true;
                                removable.Remove(node);
                                return;
                            }
                        }
                        break;
                }
            };
            EventHandler<ViewportMouseMotionEventArgs> mouseMotion = delegate(object sender, ViewportMouseMotionEventArgs e)
            {
                if (body != null)
                {
                    body.State.Position.Linear = e.Position;
                }
            };
            EventHandler<ViewportMouseButtonEventArgs> mouseUp = delegate(object sender, ViewportMouseButtonEventArgs e)
            {
                if (body != null)
                {
                    removable.AddLast(body);
                    body = null;
                }
            };
            EventHandler<KeyboardEventArgs> keyDown = delegate(object sender, KeyboardEventArgs e)
            {
                switch (e.Key)
                {
                    case Key.Q:
                        rotation -= .05f;
                        break;
                    case Key.E:
                        rotation += .05f;
                        break;
                    case Key.W:
                        scale.Y += .05f;
                        break;
                    case Key.S:
                        scale.Y -= .05f;
                        break;
                    case Key.A:
                        scale.X -= .05f;
                        break;
                    case Key.D:
                        scale.X += .05f;
                        break;
                    case Key.LeftControl:
                    case Key.RightControl:
                        duplicate = true;
                        break;
                    case Key.LeftShift:
                    case Key.RightShift:
                        count = 0;
                        makeJoint = true;
                        break;
                }
            };
            EventHandler<KeyboardEventArgs> keyUp = delegate(object sender, KeyboardEventArgs e)
            {
                switch (e.Key)
                {
                    case Key.Q:
                        rotation += .05f;
                        break;
                    case Key.E:
                        rotation -= .05f;
                        break;
                    case Key.W:
                        scale.Y -= .05f;
                        break;
                    case Key.S:
                        scale.Y += .05f;
                        break;
                    case Key.A:
                        scale.X += .05f;
                        break;
                    case Key.D:
                        scale.X -= .05f;
                        break;
                    case Key.LeftControl:
                    case Key.RightControl:
                        duplicate = false;
                        break;
                    case Key.LeftShift:
                    case Key.RightShift:
                        switch (count)
                        {
                            case 2:
                                info.Scene.Engine.AddJoint(new FixedHingeJoint(bj1, anchor, new Lifespan()));
                                break;
                            case 3:
                                info.Scene.Engine.AddJoint(new HingeJoint(bj1, bj2, anchor, new Lifespan()));
                                break;
                        }

                        count = 0;
                        makeJoint = false;
                        break;
                }
            };
            EventHandler<DrawEventArgs> onDraw = delegate(object sender, DrawEventArgs e)
            {
                if (body != null)
                {
                    body.State.Position.Angular += rotation;
                    body.Transformation *= Matrix2x3.FromScale(scale);
                    body.ApplyPosition();
                }
            };
            info.Scene.BeginDrawing += onDraw;
            info.Viewport.MouseDown += mouseDown;
            info.Viewport.MouseMotion += mouseMotion;
            info.Viewport.MouseUp += mouseUp;
            Events.KeyboardDown += keyDown;
            Events.KeyboardUp += keyUp;
            return delegate()
            {
                info.Scene.BeginDrawing -= onDraw;
                info.Viewport.MouseDown -= mouseDown;
                info.Viewport.MouseMotion -= mouseMotion;
                info.Viewport.MouseUp -= mouseUp;
                Events.KeyboardDown -= keyDown;
                Events.KeyboardUp -= keyUp;
            };
        }


        DisposeCallback dispose;
        protected override void Open()
        {
            Scene.IsPaused = true;
            List<Body> bodies = new List<Body>();

            Body b = DemoHelper.AddLine(DemoInfo, new Vector2D(300, 200), new Vector2D(400, 200), 40, Scalar.PositiveInfinity);

            b.IgnoresPhysicsLogics = true;
            bodies.Add(b);
            Body b2 = DemoHelper.AddCircle(DemoInfo, 20, 40, Scalar.PositiveInfinity, new ALVector2D(0, 300, 100));
            b2.IgnoresPhysicsLogics = true;
            bodies.Add(b2);
            Body b3 = DemoHelper.AddCircle(DemoInfo, 20, 40, 50, new ALVector2D(0, 100, 100));
            bodies.Add(b3);
            Body b4 = DemoHelper.AddRectangle(DemoInfo, 20, 20, 20, new ALVector2D(0, 150, 150));
            bodies.Add(b4);

            dispose += RegisterDup(DemoInfo, bodies);

            DemoHelper.AddShell(DemoInfo, new BoundingRectangle(0, 0, 200, 200), 10, Scalar.PositiveInfinity).ForEach(delegate(Body sb) { sb.IgnoresPhysicsLogics = true; });



            Body bStart = DemoHelper.AddShape(DemoInfo,ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("start.png"),2,16,3),Scalar.PositiveInfinity,new ALVector2D(0,100, 650));
           // Body bStart = DemoHelper.AddLine(DemoInfo, new Vector2D(10, 600), new Vector2D(100, 600), 40, Scalar.PositiveInfinity);
            bStart.IgnoresPhysicsLogics = true;
            Body bStop = DemoHelper.AddShape(DemoInfo, ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("stop.png"), 2, 16, 3), Scalar.PositiveInfinity, new ALVector2D(0, 100, 700));
            //Body bEnd = DemoHelper.AddLine(DemoInfo, new Vector2D(10, 700), new Vector2D(100, 700), 40, Scalar.PositiveInfinity);
            bStop.IgnoresPhysicsLogics = true;

            Scene.Engine.AddLogic (new GravityField(new Vector2D(0, 1000), new Lifespan()));

            dispose += DemoHelper.RegisterClick(DemoInfo, bStart, MouseButton.PrimaryButton,
                delegate(object sender, EventArgs e)
                {
                    Scene.IsPaused = false;
                });
            dispose += DemoHelper.RegisterClick(DemoInfo, bStop, MouseButton.PrimaryButton,
               delegate(object sender, EventArgs e)
               {
                   Scene.IsPaused = true;
               });
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}