#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
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
using SdlDotNet.OpenGl;
using SdlDotNet.Graphics;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Ignorers;

namespace Physics2DDemo
{
    /// <summary>
    /// This demo is not written to high standards as the rest of the engine is. 
    /// Nor does it use the engine in a optimal way. 
    /// </summary>
    class Demo
    {
        #region fields
        static readonly string dataDir = @"..|..|..|data".Replace('|',Path.DirectorySeparatorChar);
        static Random rand = new Random();
        PhysicsEngine engine;
        PhysicsTimer timer;
        List<OpenGlObject> objects;
        Dictionary<string,Sprite> sprites = new Dictionary<string,Sprite>();
        Body bomb;
        Body clipper;
        RectangleShape clippersShape;
        Vector2D bombTarget;
        Coefficients coefficients = new Coefficients(.5f, .4f);

        SurfaceGl pauseSprite;

        bool updated;

        bool sparkle;
        Vector2D sparkPoint;
        List<Body> avatarBodies;

        Font font;
        Font font2;
        #endregion
        #region constructor
        public Demo()
        {

            Events.MouseButtonDown += new EventHandler<SdlDotNet.Input.MouseButtonEventArgs>(Events_MouseButtonDown);
            Events.MouseButtonUp += new EventHandler<SdlDotNet.Input.MouseButtonEventArgs>(Events_MouseButtonUp);
            Events.KeyboardDown += new EventHandler<SdlDotNet.Input.KeyboardEventArgs>(Events_KeyboardDown);
            Events.KeyboardUp += new EventHandler<SdlDotNet.Input.KeyboardEventArgs>(Events_KeyboardUp);
            Events.MouseMotion += new EventHandler<SdlDotNet.Input.MouseMotionEventArgs>(Events_MouseMotion);
            objects = new List<OpenGlObject>();



            font = new Font(Path.Combine(dataDir, "FreeSans.ttf"), 40);
            font.Bold = true;
            font2 = new Font(Path.Combine(dataDir, "FreeSans.ttf"), 15);
            font2.Bold = true;
            pauseSprite = new SurfaceGl(font.Render("PAUSED", System.Drawing.Color.White, System.Drawing.Color.Black, true));
            upsSprite = new SurfaceGl(font2.Render("UPS:", System.Drawing.Color.White, System.Drawing.Color.Black, true));
            CreateNumbers();

            CreateEngine();
            timer = new PhysicsTimer(Update, .010f);

            CreateBomb();
            CreateAvatar();
            CreateClipper();
            Demo1();
        }
        SurfaceGl upsSprite;
        SurfaceGl[] numberSprites;
        public  void CreateNumbers()
        {
            numberSprites = new SurfaceGl[10];
            for (int index = 0; index < numberSprites.Length; ++index)
            {
                numberSprites[index] = new SurfaceGl(font2.Render(index.ToString(), System.Drawing.Color.White, System.Drawing.Color.Black, true));
            }
        }
        #endregion
        #region methods

        bool wasPaused;
        void BeginDemoChange()
        {
            if (timer.IsRunning)
            {
                timer.IsRunning = false;
                wasPaused = false;
            }
            else
            {
                wasPaused = true;
            }
        }
        void EndDemoChange()
        {
            if (!wasPaused)
            {
                timer.IsRunning = true;
            }
        }
        Sprite GetSprite(string path)
        {
            Sprite result;
            if (!sprites.TryGetValue(path, out result))
            {
                result = new Sprite(Path.Combine(dataDir, path));
                sprites.Add(path, result);
            }
            return result;
        }

        #region event handlers

        FixedHingeJoint cursorJoint;
        void engine_BodiesRemoved(object sender, CollectionEventArgs<Body> e)
        {
            Console.WriteLine("BodiesRemoved: {0}", e.Collection.Count);
        }
        void engine_BodiesAdded(object sender, CollectionEventArgs<Body> e)
        {
            Console.WriteLine("BodiesAdded: {0}", e.Collection.Count);
        }
        void Events_MouseMotion(object sender, SdlDotNet.Input.MouseMotionEventArgs e)
        {
            sparkPoint = new Vector2D(e.X, e.Y);
            if (cursorJoint != null)
            {
                Vector2D point = cursorJoint.Anchor;
                point.X += e.RelativeX;
                point.Y += e.RelativeY;
                cursorJoint.Anchor = point;
            }
        }
        Body lazer;

        void Events_MouseButtonDown(object sender, SdlDotNet.Input.MouseButtonEventArgs e)
        {
            if (e.Button == SdlDotNet.Input.MouseButton.PrimaryButton)
            {
                Vector2D point = new Vector2D(e.X, e.Y);
                IntersectionInfo info;
                foreach (Body b in engine.Bodies)
                {
                    if (b.IsCollidable && !b.Shape.BroadPhaseDetectionOnly &&
                        b.Shape.CanGetIntersection)
                    {
                        Vector2D temp = b.Matrices.ToBody * point;
                        if (b.Shape.TryGetIntersection(temp, out info))
                        {
                            //cursorJoint = new PivotJoint(b, b.State.Position.Linear, new Lifespan());
                            cursorJoint = new FixedHingeJoint(b, point, new Lifespan());
                            cursorJoint.Softness = .01f;
                            engine.AddJoint(cursorJoint);
                            break;
                        }
                    }
                }
            }
            else if (e.Button == SdlDotNet.Input.MouseButton.SecondaryButton)
            {
                sparkPoint = new Vector2D(e.X, e.Y);
                sparkle = true;
            }
            else if ((e.Button == SdlDotNet.Input.MouseButton.MiddleButton))
            {
                bombTarget = new Vector2D(e.X, e.Y);
                bomb.Lifetime.IsExpired = true;
            }
        }
        void Events_MouseButtonUp(object sender, SdlDotNet.Input.MouseButtonEventArgs e)
        {
            if (e.Button == SdlDotNet.Input.MouseButton.SecondaryButton)
            {
                sparkle = false;
            }
            else if (e.Button == SdlDotNet.Input.MouseButton.PrimaryButton)
            {
                if (cursorJoint != null)
                {
                    cursorJoint.Lifetime.IsExpired = true;
                    cursorJoint = null;
                }
            }
        }
        void Events_KeyboardDown(object sender, SdlDotNet.Input.KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case SdlDotNet.Input.Key.One:
                    Demo1();
                    break;
                case SdlDotNet.Input.Key.Two:
                    Demo2();
                    break;
                case SdlDotNet.Input.Key.Three:
                    Demo3();
                    break;
                case SdlDotNet.Input.Key.Four:
                    Demo4();
                    break;
                case SdlDotNet.Input.Key.Five:
                    Demo5();
                    break;
                case SdlDotNet.Input.Key.Six:
                    Demo6();
                    break;
                case SdlDotNet.Input.Key.Seven:
                    Demo7();
                    break;
                case SdlDotNet.Input.Key.Eight:
                    Demo8();
                    break;
                case SdlDotNet.Input.Key.Nine:
                    Demo9();
                    break;
                case SdlDotNet.Input.Key.Zero:
                    Demo0();
                    break;
                case SdlDotNet.Input.Key.W:
                    DemoW();
                    break;
                case SdlDotNet.Input.Key.E:
                    DemoE();
                    break;
                case SdlDotNet.Input.Key.R:
                    DemoR();
                    break;
                case SdlDotNet.Input.Key.T:
                    DemoT();
                    break;
                case SdlDotNet.Input.Key.Y:
                    DemoY();
                    break;
                case SdlDotNet.Input.Key.U:
                    DemoU();
                    break;
                case SdlDotNet.Input.Key.I:
                    DemoI();
                    break;
                case SdlDotNet.Input.Key.O:
                    DemoO();
                    break;
                case SdlDotNet.Input.Key.A:
                    DemoA();
                    break;
                case SdlDotNet.Input.Key.S:
                    DemoS();
                    break;
                case SdlDotNet.Input.Key.D:
                    DemoD();
                    break;
                case SdlDotNet.Input.Key.LeftArrow:
                    torque += torqueMag;
                    //force += new Vector2D(-forceMag, 0);
                    break;
                case SdlDotNet.Input.Key.RightArrow:
                    torque -= torqueMag;
                    // force += new Vector2D(forceMag, 0);
                    break;
                case SdlDotNet.Input.Key.UpArrow:
                    //avatarBodies.IgnoresGravity = true;
                    //force += new Vector2D(0, -forceMag);
                    break;
                case SdlDotNet.Input.Key.DownArrow:
                    //force += new Vector2D(0, forceMag);
                    break;
                case SdlDotNet.Input.Key.Space:
                    LaunchProjectile();
                    break;
                case SdlDotNet.Input.Key.Pause:
                case SdlDotNet.Input.Key.P:
                    timer.IsRunning = !timer.IsRunning;
                    break;
                case SdlDotNet.Input.Key.M:
                    AddRays();
                    break;
                case SdlDotNet.Input.Key.N:
                    AddExplosion();
                    break;
            }
        }
        void AddExplosion()
        {
            System.Drawing.Point p = SdlDotNet.Input.Mouse.MousePosition;

            ExplosionLogic logic = new ExplosionLogic(
                new Vector2D(p.X, p.Y),
                Vector2D.Zero,
                2000, 3.1f, 80,
                new Lifespan(1));
            engine.AddLogic(logic);
        }


        void AddRays()
        {
            List<RaySegment> segments = new List<RaySegment>();


            for (Scalar angle = 0; angle < MathHelper.PiOver2; angle += .05f)
            {
                RaySegment seg = new RaySegment();
                seg.Length = 500;
                seg.RayInstance = new Ray(Vector2D.Zero, Vector2D.FromLengthAndAngle(1, angle));
                segments.Add(seg);
            }

            lazer = new Body(new PhysicsState(), new RaySegmentsShape(segments.ToArray()), 1, new Coefficients(1, 1), new Lifespan());
            lazer.State.Position.Linear = sparkPoint;
            lazer.State.Velocity.Angular = .91f;
            lazer.IgnoresGravity = true;
            lazer.ApplyPosition();
            engine.AddBody(lazer);
            lazerLogic = new RaySegmentsCollisionLogic(lazer);
            engine.AddLogic(lazerLogic);
            lock (objects)
            {
                OpenGlObject o = new OpenGlObject(lazer, lazerLogic);
                lazer.Tag = o;
                objects.Add(o);
            }
        }
        RaySegmentsCollisionLogic lazerLogic;


        Scalar torqueMag = -1500000;
        Scalar torque = 0;
        void Events_KeyboardUp(object sender, SdlDotNet.Input.KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case SdlDotNet.Input.Key.LeftArrow:
                    torque -= torqueMag;
                    //force -= new Vector2D(-forceMag, 0);
                    break;
                case SdlDotNet.Input.Key.RightArrow:
                    torque += torqueMag;
                    // force -= new Vector2D(forceMag, 0);
                    break;
                case SdlDotNet.Input.Key.UpArrow:
                    //avatarBodies.IgnoresGravity = false;
                    //force -= new Vector2D(0, -forceMag);
                    break;
                case SdlDotNet.Input.Key.DownArrow:
                    //force -= new Vector2D(0, forceMag);
                    break;
                case SdlDotNet.Input.Key.M:
                    lazer.Lifetime.IsExpired = true;

                    break;

            }
        }

        void bomb_Removed(object sender, RemovedEventArgs e)
        {
            Vector2D position = new Vector2D(rand.Next(0, 1400), 0);
            Scalar velocityMag = rand.Next(1000, 2000);
            Vector2D velocity = Vector2D.SetMagnitude(bombTarget - position, velocityMag);
            bomb.Lifetime = new Lifespan();
            bomb.State.Position.Linear = position;
            bomb.State.Velocity.Linear = velocity;
            bomb.State.Position.Angular = velocity.Angle;
            bomb.State.Velocity.Angular = 0;
            engine.AddBody(bomb);
            AddGlObject(bomb);
        }
        void avatar_Updated(object sender, UpdatedEventArgs e)
        {
            Scalar maxVelocity = 40;

            Scalar vel = 0;
            for (int index = 1; index < avatarBodies.Count; ++index)
            {
                vel = Math.Max(Math.Abs(avatarBodies[index].State.Velocity.Angular), vel);
            }

            Scalar multiply = Math.Abs((vel) / maxVelocity);
            if (vel > maxVelocity)
            {
                multiply = 1;
            }
            for (int index = 1; index < avatarBodies.Count; ++index)
            {
                avatarBodies[index].State.ForceAccumulator.Angular += (torque - torque * multiply);
            }

            // avatarBodies.State.ForceAccumulator.Linear += force;
        }
        void particle_Collided(object sender, CollisionEventArgs e)
        {
            Body b1 = (Body)sender;
            Body b2 = e.Other;
            Vector2D p = e.Contacts[0].Position;
            Vector2D p1, p2, rv;
            Vector2D.Subtract(ref p, ref b1.State.Position.Linear, out p1);
            Vector2D.Subtract(ref p, ref b2.State.Position.Linear, out p2);
            PhysicsHelper.GetRelativeVelocity(ref b1.State.Velocity, ref b2.State.Velocity, ref p1, ref p2, out rv);
            if (rv.Magnitude < 1)
            {
                b1.Lifetime.IsExpired = true;
            }
        }
        void clipper_Collided(object sender, CollisionEventArgs e)
        {
            OpenGlObject o = (OpenGlObject)e.Other.Tag;
            if (o == null) { return; }
            o.collided = true;
        }
        void clipper_Updated(object sender, UpdatedEventArgs e)
        {
            try
            {
                for (int index = 0; index < objects.Count; ++index)
                {
                    OpenGlObject o = objects[index];
                    o.shouldDraw = o.collided;
                    o.collided = false;
                }
            }
            catch { }
        }
        #endregion

        void LaunchProjectile()
        {
            Scalar radius = 5;
            Scalar velocity = 2000;

            Matrix2x3 toWorld = avatarBodies[0].Matrices.ToWorld;
            Matrix2x2 toWorldNormal = avatarBodies[0].Matrices.ToWorldNormal;

          //  Matrix2D mat = avatarBodies[0].Matrices.ToWorld;
            Vector2D position = toWorld * (avatarBarrelOffset);
            Vector2D direction = toWorldNormal * Vector2D.XAxis;
            PhysicsState state = new PhysicsState();
            state.Position.Linear = position;
            state.Velocity.Linear = velocity * direction + avatarBodies[0].State.Velocity.Linear;

            Body weapon = new Body(state,
                new CircleShape(radius, 8),
                5,
                coefficients.Duplicate(),
                new Lifespan(10));
            weapon.CollisionIgnorer = avatarBodies[0].CollisionIgnorer;
            weapon.Collided += weapon_Collided;
            AddGlObject(weapon);
            engine.AddBody(weapon);
            avatarBodies[0].State.Velocity.Linear -= (velocity * weapon.Mass.Mass * avatarBodies[0].Mass.MassInv) * direction;
        }

        void weapon_Collided(object sender, CollisionEventArgs e)
        {
            Body weapon = (Body)sender;
            weapon.Lifetime.IsExpired = true;
            AddParticles(weapon.State.Position.Linear, weapon.State.Velocity.Linear*.5f, 200);
            weapon.Collided -= weapon_Collided;
        }
        /// <summary>
        /// initializes the PhysicsEngine
        /// </summary>
        void CreateEngine()
        {
            //creates it
            engine = new PhysicsEngine();

            //sets the broadphase
            //engine.BroadPhase = new Physics2DDotNet.Detectors.BruteForceDetector();
            //engine.BroadPhase = new Physics2DDotNet.Detectors.SweepAndPruneDetector();
            engine.BroadPhase = new Physics2DDotNet.Detectors.SelectiveSweepDetector();
            //engine.BroadPhase = new Physics2DDotNet.Detectors.FrameCoherentSAPDetector();
            //engine.BroadPhase = new Physics2DDotNet.Detectors.SpatialHashDetector();

            //setups the Solver and sets it.
            Physics2DDotNet.Solvers.SequentialImpulsesSolver solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();
            solver.Iterations = 12;
            solver.SplitImpulse = true;
            solver.BiasFactor = .7f;
            //solver.BiasFactor = .3f;
            solver.AllowedPenetration = .1f;
            engine.Solver = solver;

            engine.BodiesAdded += new EventHandler<CollectionEventArgs<Body>>(engine_BodiesAdded);
            engine.BodiesRemoved += new EventHandler<CollectionEventArgs<Body>>(engine_BodiesRemoved);
        }

        void CreateClipper()
        {
            clippersShape = new RectangleShape();
            clipper = new Body(new PhysicsState(), clippersShape, 0, new Coefficients(0, 0), new Lifespan());
            clipper.IgnoresGravity = true;
            clipper.Collided += new EventHandler<CollisionEventArgs>(clipper_Collided);
            clipper.Updated += new EventHandler<UpdatedEventArgs>(clipper_Updated);
        }

        void CreateBomb()
        {
             
            Sprite sprite = GetSprite("rocket.png");
            Vector2D[][] vertexes = MultiPolygonShape.Subdivide(sprite.Polygons, 10);
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite;

            bomb = new Body(new PhysicsState(new ALVector2D(0,500,-60)),
                    shape,//new Physics2DDotNet.Circle(20, 20),
                    120,
                    coefficients.Duplicate(),
                    new Lifespan());
        }

        List<Body> AddText(string text, Vector2D position)
        {
            /*if (font == null)
            {
                font = new Font(Path.Combine(dataDir, "FreeSans.ttf"), 40);
                font.Bold = true;
            }*/
            List<Body> result = new List<Body>();
            Scalar initialx = position.X;
            int maxy = 0;
            foreach (char c in text)
            {
                if (c == '\n')
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
                    Sprite sprite = GetLetter(c);
                    Vector2D[][] vertexes = MultiPolygonShape.Subdivide(sprite.Polygons, 3);
                    BoundingRectangle rect = BoundingRectangle.FromVectors(sprite.Polygons[0]);
                    MultiPolygonShape shape = new MultiPolygonShape(
                        vertexes,
                        Math.Min(Math.Min((rect.Max.X - rect.Min.X) / 8, (rect.Max.Y - rect.Min.Y) / 8), 3));
                    shape.Tag = sprite;
                    Body b = AddShape(shape, 40, new ALVector2D(0, position + sprite.Offset));
                    maxy = Math.Max(maxy, sprite.Texture.Surface.Height);
                    position.X += sprite.Texture.Surface.Width;
                    result.Add(b);
                }
            }
            return result;
        }
        Sprite GetLetter(char c)
        {
            string name = "Char:" + c;
            Sprite sprite;
            if (!sprites.TryGetValue(name, out sprite))
            {
                Surface surface = font.Render("" + c, System.Drawing.Color.Blue, System.Drawing.Color.Black, true);
                surface.TransparentColor = System.Drawing.Color.Black;
                sprite = new Sprite(surface);
                sprites.Add(name, sprite);
            }
            return sprite;
        }


        List<Vector2D> avatarOffsets;
        List<Joint> avatarJoints;
        Vector2D avatarBarrelOffset;
        void CreateAvatar()
        {
            Lifespan avatarLifespan = new Lifespan();

            Sprite sprite = GetSprite("tank.png");
            Vector2D[][] vertexes = sprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite;

            ObjectIgnorer ignorer = new ObjectIgnorer();
            Body a = new Body(new PhysicsState(new ALVector2D(0, 0, 0)),
                shape,
                300,//new MassInfo(40, Scalar.PositiveInfinity),
                coefficients.Duplicate(),
                avatarLifespan);
            a.Updated += new EventHandler<UpdatedEventArgs>(avatar_Updated);
            avatarBodies = new List<Body>();
            avatarOffsets = new List<Vector2D>();
            avatarJoints = new List<Joint>();
            avatarBodies.Add(a);
            a.CollisionIgnorer = ignorer;




            Scalar wheelSize = 18;
            Scalar wheelSpacing = -9;
            Scalar lenghtPercent = .84f;
            Matrix2x3 ident = Matrix2x3.Identity;
            Matrices matri = new Matrices();
            BoundingRectangle rect;
            shape.CalcBoundingRectangle(matri, out rect);
            Scalar y = (rect.Max.Y +4)  ;
            Body lastWheel = null ;
            BoundingPolygon polygon = new BoundingPolygon(vertexes[0]);

            Ray ray2 = new Ray(new Vector2D(rect.Max.X, y), -Vector2D.YAxis);
            Scalar y3 = y - polygon.Intersects(ray2);
            avatarBarrelOffset = new Vector2D(rect.Max.X, y3);
            
            for (Scalar x = rect.Min.X + wheelSize ; x < (rect.Max.X - wheelSize ) * lenghtPercent; x += (wheelSize*2 + wheelSpacing))
            {

                Ray ray = new Ray(new Vector2D(x, y), -Vector2D.YAxis);
                Scalar y2 = y-  polygon.Intersects(ray);



                Vector2D offset = new Vector2D(x, y2);

                Body wheel = new Body(
                    new PhysicsState(new ALVector2D(0, offset)),
                    new CircleShape(wheelSize, 30),
                    10,
                    new Coefficients(0,3),//  coefficients.Duplicate(),
                    avatarLifespan);
                HingeJoint joint = new HingeJoint(a, wheel, offset, avatarLifespan);
                joint.Softness = .1f;
                wheel.CollisionIgnorer = ignorer;

                if (lastWheel != null)
                {
                    AngleJoint joint2 = new AngleJoint(lastWheel, wheel, avatarLifespan);
                    avatarJoints.Add(joint2);
                }

                avatarJoints.Add(joint);
                avatarOffsets.Add(offset);
                avatarBodies.Add(wheel);
                lastWheel = wheel;
            }
        }

        void AddBomb()
        {
            AddGlObject(bomb);
            bomb.Removed += bomb_Removed;
            engine.AddBody(bomb);
        }
        void AddAvatar()
        {

            Vector2D position = new Vector2D(100, -100);

            avatarBodies[0].IsCollidable = true;
            avatarBodies[0].Lifetime.IsExpired = false;
            avatarBodies[0].State.Position.Linear = position;
            avatarBodies[0].State.Position.Angular = 0;
            avatarBodies[0].State.Velocity = ALVector2D.Zero;
            avatarBodies[0].ApplyPosition();
            for (int index = 1; index < avatarBodies.Count; ++index)
            {
                avatarBodies[index].IsCollidable = true;
              //  avatarBodies[index].Lifetime.IsExpired = false;
                avatarBodies[index].State.Position.Linear = position + avatarOffsets[index - 1];
                avatarBodies[index].State.Position.Angular = 0;
                avatarBodies[index].State.Velocity = ALVector2D.Zero;
                avatarBodies[index].ApplyPosition();
            }
            for (int index = 0; index < avatarJoints.Count; ++index)
            {
             //   avatarJoints[index].Lifetime.IsExpired = false;
            }
            AddGlObjectRange(avatarBodies);
            engine.AddBodyRange(avatarBodies);
            engine.AddJointRange(avatarJoints);
        }



        void AddClipper()
        {
            engine.AddBody(clipper);
        }
        void Clear()
        {
            RemoveBombEvents();
            engine.Clear();
            objects.Clear();
            GC.Collect();
        }
        void Reset() { Reset(true); }
        void Reset(bool addExtra)
        {
            hasWater = false;
            Clear();
            AddClipper();
            AddBomb();
            if (addExtra)
            {
                AddAvatar();
            }
        }

        void AddGlObject(Body obj)
        {
            lock (objects)
            {
                OpenGlObject o = new OpenGlObject(obj);
                obj.Tag = o;
                objects.Add(o);
            }
        }
        void AddGlObjectRange(IList<Body> collection)
        {
            OpenGlObject[] arr = new OpenGlObject[collection.Count];
            for (int index = 0; index < arr.Length; ++index)
            {
                arr[index] = new OpenGlObject(collection[index]);
                collection[index].Tag = arr[index];
            }
            lock (objects)
            {
                objects.AddRange(arr);
            }
        }
        Body AddFloor(ALVector2D position)
        {
            Body line = new Body(
                new PhysicsState(position),
                new PolygonShape(PolygonShape.CreateRectangle(60, 2000), 40),
                new MassInfo(Scalar.PositiveInfinity, Scalar.PositiveInfinity),
                coefficients.Duplicate(),
                new Lifespan());
            line.IgnoresGravity = true;
            AddGlObject(line);
            engine.AddBody(line);
            return line;
        }
        void AddLine(Vector2D point1, Vector2D point2, Scalar thickness)
        {
            Vector2D line = point1 - point2;
            Vector2D avg = (point1 + point2) * .5f;
            Scalar length = line.Magnitude;
            Scalar angle = line.Angle;

            Scalar Hd2 = thickness * .5f;
            Scalar Wd2 = length * .5f;

            int curveEdgeCount = 5;
            Scalar da = MathHelper.Pi/curveEdgeCount;

            List<Vector2D> vertexes = new List<Vector2D>();
            vertexes.Add(new Vector2D(Wd2, Hd2));
            vertexes.Add(new Vector2D(-Wd2, Hd2));
            for (Scalar angle2 = MathHelper.PiOver2 + da; angle2 < MathHelper.ThreePiOver2; angle2+= da)
            {
                vertexes.Add(new Vector2D(-Wd2, 0) + Vector2D.FromLengthAndAngle(Hd2, angle2));
            }
            vertexes.Add(new Vector2D(-Wd2, -Hd2));
            vertexes.Add(new Vector2D(Wd2, -Hd2));
            for (Scalar angle2 = -MathHelper.PiOver2 + da; angle2 < MathHelper.PiOver2; angle2 += da)
            {
                vertexes.Add(new Vector2D(Wd2, 0) + Vector2D.FromLengthAndAngle(Hd2, angle2));
            }
            Body body = new Body(
                new PhysicsState(new ALVector2D(angle, avg)),
              new PolygonShape(vertexes.ToArray(), thickness / 4),  //new Physics2DDotNet.Line(vertexes, thickness, thickness / 2),
                new MassInfo(Scalar.PositiveInfinity, Scalar.PositiveInfinity),
                coefficients.Duplicate(),
                new Lifespan());
            body.Shape.IgnoreVertexes = true;
            body.IgnoresGravity = true;
            AddGlObject(body);
            engine.AddBody(body);
        }
        List<Body> AddChain(Vector2D position, Scalar boxLenght, Scalar boxWidth, Scalar boxMass, Scalar spacing, Scalar length)
        {
            List<Body> bodies = new List<Body>();
            Body last = null;
            for (Scalar x = 0; x < length; x += boxLenght + spacing, position.X += boxLenght + spacing)
            {
                Body current = AddRectangle(boxWidth, boxLenght, boxMass, new ALVector2D(0, position));
                bodies.Add(current);
                if (last != null)
                {
                    Vector2D anchor = (current.State.Position.Linear + last.State.Position.Linear) * .5f;
                    HingeJoint joint = new HingeJoint(last, current, anchor, new Lifespan());
                    joint.DistanceTolerance = 10;
                    this.engine.AddJoint(joint);
                }
                last = current;
            }
            return bodies;
        }
        void AddPyramid()
        {


            Scalar size = 32;
            Scalar spacing = .01f;
            Scalar Xspacing = 2f;

            Scalar xmin = 300;
            Scalar xmax = 850;
            Scalar ymin = 50;
            Scalar ymax = 720 - size / 2;
            Scalar step = (size + spacing + Xspacing) / 2;
            Scalar currentStep = 0;




            Sprite sprite = GetSprite("block.png");
            Vector2D[][] vertexes = sprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite;

            for (Scalar y = ymax; y >= ymin; y -= (spacing + size))
            {
                for (Scalar x = xmin + currentStep; x < (xmax - currentStep); x += spacing + Xspacing + size)
                {
                    AddShape(shape, 20, new ALVector2D(0, new Vector2D(x, y)));
                }
                currentStep += step;
            }
        }
        void AddTowers()
        {
            Scalar xmin = 200;
            Scalar xmax = 800;
            Scalar ymin = 550;
            Scalar ymax = 700;

            Scalar size = 32;
            Scalar spacing = 1;
            Scalar Xspacing = 20;
            Scalar offset = 0;
            Scalar offsetchange = .9f;

            Sprite sprite = GetSprite("block.png");
            Vector2D[][] vertexes = sprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite;

            for (Scalar x = xmin; x < xmax; x += spacing + Xspacing + size)
            {
                for (Scalar y = ymax; y > ymin; y -= spacing + size)
                {
                    AddShape(shape, 20, new ALVector2D(0, new Vector2D(x + offset, y)));

                    //AddRectangle(size, size, 20, new ALVector2D(0, new Vector2D(x + offset, y)));
                    offset = MathHelper.WrapClamp(offset + offsetchange, -offsetchange,  offsetchange);
                }
            }
        }
        Body AddShape(Shape shape, Scalar mass, ALVector2D position)
        {
            Body e =
                new Body(
                     new PhysicsState(position),
                     shape,
                     mass,
                     coefficients.Duplicate(),
                     new Lifespan());
            AddGlObject(e);
            engine.AddBody(e);
            return e;
        }
        Body AddBody(Body e)
        {
            AddGlObject(e);
            engine.AddBody(e);
            return e;
        }
        Body AddRectangle(Scalar height, Scalar width, Scalar mass, ALVector2D position)
        {
            Vector2D[] vertices = PolygonShape.CreateRectangle(height, width);
            vertices =PolygonShape.Subdivide(vertices, (height + width) / 9);

            Shape boxShape = new PolygonShape(vertices, Math.Min(height, width) / 2);
            Body e =
                new Body(
                     new PhysicsState(position),
                     boxShape,
                     mass,
                     coefficients.Duplicate(),
                     new Lifespan());
            AddGlObject(e);
            engine.AddBody(e);
            return e;

        }
        Body AddCircle(Scalar radius, int vertexCount, Scalar mass, ALVector2D position)
        {

            Shape circleShape = new CircleShape(radius, vertexCount); ;
            Body e =
                new Body(
                     new PhysicsState(position),
                     circleShape,
                     mass,
                     coefficients.Duplicate(),
                     new Lifespan());
            AddGlObject(e);
            engine.AddBody(e);
            return e;

        }

        Body AddRectangle(BoundingRectangle rect, Scalar mass)
        {
            Scalar width = rect.Max.X - rect.Min.X;
            Scalar heigth = rect.Max.Y - rect.Min.Y;
            Vector2D pos = rect.Min + new Vector2D(width / 2, heigth / 2);
            return AddRectangle(heigth, width, mass, new ALVector2D(0, pos));
        }

        List<Body> AddShell(BoundingRectangle rect, Scalar thickness, Scalar mass)
        {
            List<Body> result = new List<Body>();

            result.Add(AddRectangle(
                new BoundingRectangle(
                    rect.Min,
                    new Vector2D(rect.Max.X - thickness, rect.Min.Y + thickness)),
                mass));
            result.Add(AddRectangle(
                new BoundingRectangle(
                    new Vector2D(rect.Max.X - thickness, rect.Min.Y),
                    new Vector2D(rect.Max.X, rect.Max.Y - thickness)
                    ),
                mass));
            result.Add(AddRectangle(
                new BoundingRectangle(
                    new Vector2D(rect.Min.X + thickness, rect.Max.Y - thickness),
                    rect.Max),
                mass));

            result.Add(AddRectangle(
                new BoundingRectangle(
                    new Vector2D(rect.Min.X, rect.Min.Y + thickness),
                    new Vector2D(rect.Min.X + thickness, rect.Max.Y)
                    ),
                mass));

            return result;
        }
        
        void RemoveBombEvents()
        {
            bomb.Removed -= bomb_Removed;
        }
        void AddTower()
        {
            Scalar size = 32;
            Scalar x = 500;
            Scalar spacing = size + 2;

            Scalar minY = 430;
            Scalar maxY = 720 - size / 2;
            Scalar offset = 0;
            Scalar offsetchange = .9f;

            Sprite sprite = GetSprite("block.png");
            Vector2D[][] vertexes = sprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite;

            for (Scalar y = maxY; y > minY; y -= spacing)
            {
                AddShape(shape, 20, new ALVector2D(0, new Vector2D(x + offset, y)));
                //AddRectangle(size, size, 20, new ALVector2D(0, new Vector2D(x + offset, y)));
                offset = MathHelper.WrapClamp(offset + offsetchange, -offsetchange,offsetchange);
            }
        }
        void AddGravityField()
        {
            engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));
        }
        void AddParticles(Vector2D position, int count)
        {
            AddParticles(position, Vector2D.Zero, count);
        }
        void AddParticles(Vector2D position,Vector2D velocity, int count)
        {
            Body[] particles = new Body[count];
            Scalar angle = MathHelper.TwoPi / count;
            for (int index = 0; index < count; ++index)
            {
                Body particle = new Body(
                    new PhysicsState(new ALVector2D(0, position)),
                     ParticleShape.Default,
                    1f,
                   new Coefficients(1,.5f),// coefficients.Duplicate(),
                    new Lifespan(.9f));
                Vector2D direction = Vector2D.FromLengthAndAngle(1, index * angle + ((Scalar)rand.NextDouble() - .5f) * angle);
                particle.State.Position.Linear += direction;
                particle.State.Velocity.Linear = direction * rand.Next(200, 1001) + velocity;
                particles[index] = particle;
                particle.Collided += new EventHandler<CollisionEventArgs>(particle_Collided);
            }
            AddGlObjectRange(particles);
            engine.AddBodyRange(particles);
        }

        void Transform(ALVector2D vector, IList<Body> collection)
        {
            Matrix2x3 matrix;
            ALVector2D.ToMatrix2x3(ref vector, out matrix);
            Transform(matrix, collection);
        }
        void Transform(Matrix2x3  matrix, IList<Body> collection)
        {
            foreach (Body b in collection)
            {
                ALVector2D.Transform(ref matrix,ref b.State.Position, out b.State.Position);
                b.ApplyPosition();
            }
        }

        void AddTower2()
        {
            Scalar size = 32;
            Scalar x = 500;
            Scalar spacing = size + 2;



            Sprite sprite = GetSprite("block.png");
            Vector2D[][] vertexes = sprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite; 
            Scalar minY = 100;
            Scalar maxY = 400 - size / 2;
            for (Scalar y = maxY; y > minY; y -= spacing)
            {
                AddShape(shape, 20, new ALVector2D(rand.Next(-10, 0) * .1f, new Vector2D(x + rand.Next(-90, 90) * .1f, y)));
            }
        }
        List<Body> AddRagDoll(Vector2D location)
        {
            List<Body> result = new List<Body>();
            Scalar mass = 10;
            Body head = AddCircle(12, 9, mass, new ALVector2D(0, location + new Vector2D(0, 0)));

            Scalar Ld2 = 50 / 2;
            Scalar Wd2 = 25 / 2;
            Vector2D[] vertexes = new Vector2D[]
            {
                new Vector2D(Wd2, 0),
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
            };
            Shape shape = new PolygonShape(vertexes, 5);

            Body torso = AddShape(shape, mass * 4, new ALVector2D(0, location + new Vector2D(0, 40)));

            Body ltarm = AddRectangle(10, 30, mass, new ALVector2D(0, location + new Vector2D(-30, 20)));
            Body lbarm = AddRectangle(10, 30, mass, new ALVector2D(0, location + new Vector2D(-65, 20)));

            Body rtarm = AddRectangle(10, 30, mass, new ALVector2D(0, location + new Vector2D(30, 20)));
            Body rbarm = AddRectangle(10, 30, mass, new ALVector2D(0, location + new Vector2D(65, 20)));

            Body ltleg = AddRectangle(40, 15, mass * 2, new ALVector2D(.06f, location + new Vector2D(-10, 95)));
            Body lbleg = AddRectangle(40, 15, mass * 2, new ALVector2D(0, location + new Vector2D(-11, 140)));

            Body rtleg = AddRectangle(40, 15, mass * 1.5f, new ALVector2D(-.06f, location + new Vector2D(10, 95)));
            Body rbleg = AddRectangle(40, 15, mass * 1.5f, new ALVector2D(0, location + new Vector2D(11, 140)));

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
            
            engine.AddJointRange<Joint>(joints);

            return result;
        }
        void AddDiaganol_StressTest()
        {

            Scalar size = 32;
            Scalar spacing = 3f;

            Scalar ymin = -90000;
            Scalar ymax = 90000;

            Sprite sprite = GetSprite("block.png");
            Vector2D[][] vertexes = sprite.Polygons;
            MultiPolygonShape shape =new  MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite;

            for (Scalar y = ymax; y >= ymin; y -= (spacing + size))
            {
                
                    AddShape(shape, 20, new ALVector2D(0, new Vector2D(y, y)));
                
            }
        }
        void AddTowers_StressTest()
        {

            Scalar box = 900;
            Scalar xmin = -box;
            Scalar xmax = box;
            Scalar ymin = -box;
            Scalar ymax = box;

            Scalar size = 32;
            Scalar spacing = 2;

            Sprite sprite = GetSprite("block.png");
            Vector2D[][] vertexes = sprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite;
            List<Body> bodies = new List<Body>();
            for (Scalar x = xmin; x < xmax; x += spacing + size)
            {
                for (Scalar y = ymax; y > ymin; y -= spacing + size)
                {
                    Body e =
                    new Body(
                         new PhysicsState(new ALVector2D(0, new Vector2D(x, y))),
                         shape,
                         20,
                         coefficients.Duplicate(),
                         new Lifespan());


                    bodies.Add(e);
                }
            }
            AddGlObjectRange(bodies);
            engine.AddBodyRange(bodies);
        }
        void AddRandom_StressTest()
        {

            Scalar box = 900;
            Scalar xmin = -box;
            Scalar xmax = box;
            Scalar ymin = -box;
            Scalar ymax = box;

            Scalar size = 32;
            Scalar spacing = 2;

            Sprite sprite = GetSprite("block.png");
            Vector2D[][] vertexes = sprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = sprite;
            List<Body> bodies = new List<Body>();
            for (Scalar x = xmin; x < xmax; x += spacing + size)
            {
                for (Scalar y = ymax; y > ymin; y -= spacing + size)
                {
                    Body e =
                    new Body(
                         new PhysicsState(new ALVector2D(0, new Vector2D(x + (Scalar)rand.NextDouble() * 900000, y + (Scalar)rand.NextDouble() * 900000))),
                         shape,
                         20,
                         coefficients.Duplicate(),
                         new Lifespan());


                    bodies.Add(e);
                }
            }
            AddGlObjectRange(bodies);
            engine.AddBodyRange(bodies);
        }
        void AddCircles_StressTest()
        {

            Scalar box = 550;
            Scalar xmin = 0;
            Scalar xmax = box;
            Scalar ymin = 0;
            Scalar ymax = box;

            Scalar size = 15;
            Scalar spacing = 2;

           // Particle shape = new Particle(); 
            CircleShape shape = new CircleShape(7, 10);
            List<Body> bodies = new List<Body>();
            for (Scalar x = xmin; x < xmax; x += spacing + size)
            {
                for (Scalar y = ymax; y > ymin; y -= spacing + size)
                {
                    Body e =
                    new Body(
                         new PhysicsState(new ALVector2D(0, new Vector2D(x, y))),
                         shape,
                         20,
                         coefficients.Duplicate(),
                         new Lifespan());


                    bodies.Add(e);
                }
            }
            AddGlObjectRange(bodies);
            engine.AddBodyRange(bodies);
        }


        BinaryFormatter formater = new BinaryFormatter();
        MemoryStream savedEngine = new MemoryStream();


       /* void Demo1()
        {
            BeginDemoChange();
            Reset();

            Body ball1 = AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(400, 400)));
            Body ball2 = AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(200, 400)));

            ball2.State.Velocity.Linear = new Vector2D(100, 0);
            ball1.State.Velocity.Linear = new Vector2D(-10, 0);

            Matrix2x3 mat =
                    Matrix2x3.FromRotationZ(1) *
                    Matrix2x3.FromScale(new Vector2D(.2f, .5f)) *
                    Matrix2x3.FromRotationZ(-1);
            Matrix2x3 mat2 = Matrix2x3.FromScale(new Vector2D(.2f, .5f));
            ball1.Transformation *= mat;

            ball2.Transformation *= mat;


            EndDemoChange();
        }*/
        void Demo1()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));

            Sprite blockSprite = GetSprite("fighter.png");
            Vector2D[][] vertexes = blockSprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = blockSprite;
            for (int i = 128 * 3; i > -128; i -= 128)
            {
                Body b = AddShape(shape, 40, new ALVector2D(1, new Vector2D(600, 272 + i)));
                //b.Transformation *= Matrix3x2.FromScale(new Vector2D(1, .5f));
            }
            Body b1 = AddShape(shape, 40, new ALVector2D(1, new Vector2D(400,400)));
            Body b2 = AddShape(shape, 40, new ALVector2D(1, new Vector2D(400, 200)));
            //b1.LinearDamping = .9f;
          //  b2.LinearDamping = .9f;
            //engine.AddProxy(b1, b2, Matrix2x2.FromRotation(MathHelper.PiOver2 ));
            b1.Transformation *= Matrix2x3.FromScale(new Vector2D(1, 1.8f));
            b2.Transformation *= Matrix2x3.FromScale(new Vector2D(.9f, 1)); 

            Body ball = AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(1028, 272)));
            ball.Transformation *=
                Matrix2x3.FromRotationZ(1) *
                Matrix2x3.FromScale(new Vector2D(.2f, .5f)) *
                Matrix2x3.FromRotationZ(-1);

            EndDemoChange();
        }
        void Demo2()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddTower();
            AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(1028, 272)));
            EndDemoChange();
        }
        void Demo3()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(.1f, new Vector2D(600, 770)));
            AddTower();
            AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(1028, 272)));
            EndDemoChange();
        }
        void Demo4()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));

            AddPyramid();
            AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(1028, 272)));
            EndDemoChange();
        }
        void Demo5()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddTowers();
            AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(1028, 272)));
            EndDemoChange();
        }
        void Demo6()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();

            List<Body> chain = AddChain(new Vector2D(400, 50), 100, 30, 200, 20, 800);
            Vector2D point = new Vector2D(300, 50);

            Body Anchor = AddCircle(30, 18, Scalar.PositiveInfinity, new ALVector2D(0, point));
            Anchor.IgnoresGravity = true;
            HingeJoint joint = new HingeJoint(chain[0], Anchor, point, new Lifespan());
            joint.DistanceTolerance = 10;
            engine.AddJoint(joint);
            EndDemoChange();
        }
        void Demo7()
        {
            BeginDemoChange();
            Reset();
            engine.AddLogic(new GravityPointField(new Vector2D(500, 500), 800, new Lifespan()));
            AddTowers();
            Scalar y = 0;
            for (Scalar x = 200; x < 500; x += 100, y -= 100)
            {
                AddRagDoll(new Vector2D(x, y));
            }
            EndDemoChange();
        }
        void Demo8()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();

            Scalar boxlength = 50;
            Scalar spacing = 4;
            Scalar anchorLenght = 30;
            Scalar anchorGap = (boxlength / 2) + spacing + (anchorLenght / 2);
            List<Body> chain = AddChain(new Vector2D(200, 500), boxlength, 20, 200, spacing, 600);

            Vector2D point2 = new Vector2D(chain[chain.Count - 1].State.Position.Linear.X + anchorGap, 500);
            Body end2 = AddCircle(anchorLenght / 2, 14, Scalar.PositiveInfinity, new ALVector2D(0, point2));
            end2.IgnoresGravity = true;
            HingeJoint joint2 = new HingeJoint(chain[chain.Count - 1], end2, point2, new Lifespan());
            joint2.DistanceTolerance = 10;
            engine.AddJoint(joint2);

            Vector2D point1 = new Vector2D(chain[0].State.Position.Linear.X - anchorGap, 500);
            Body end1 = AddCircle(anchorLenght/2, 14, Scalar.PositiveInfinity, new ALVector2D(0, point1));
            end1.IgnoresGravity = true;
            HingeJoint joint1 = new HingeJoint(chain[0], end1, point1, new Lifespan());
            joint1.DistanceTolerance = 10;
            engine.AddJoint(joint1);
            end2.State.Position.Linear.X -= 10;
            end1.State.Position.Linear.X += 10;
            end2.ApplyPosition();
            end1.ApplyPosition();

            AddTower2();


            EndDemoChange();
        }
        void Demo9()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();

            AddLine(new Vector2D(0, 700), new Vector2D(300, 700), 30);
            AddLine(new Vector2D(300, 700), new Vector2D(400, 650), 30);
            AddLine(new Vector2D(400, 650), new Vector2D(500, 650), 30);
            AddLine(new Vector2D(500, 650), new Vector2D(500, 500), 30);
            AddLine(new Vector2D(500, 500), new Vector2D(900, 550), 30);
            AddLine(new Vector2D(400, 400), new Vector2D(600, 300), 30);
            AddRagDoll(new Vector2D(200, 400));
            AddRagDoll(new Vector2D(300, 300));
            Transform(new ALVector2D(MathHelper.Pi, 400, 200), AddRagDoll(new Vector2D(0, 0)));
            AddRagDoll(new Vector2D(500, 100));
            AddRagDoll(new Vector2D(600, 0));
            AddRagDoll(new Vector2D(700, -100));

            EndDemoChange();
        }
        void Demo0()
        {
            BeginDemoChange();
            Reset(false);
          //  avatarBodies.Lifetime.IsExpired = true;
            AddGravityField();
            
            AddLine(new Vector2D(-40, 200), new Vector2D(400,200), 30);
            for (int x = 0; x < 400; x += 45)
            {
                AddRectangle(80, 15, 90, new ALVector2D(0, x, 145));
            }

            AddLine(new Vector2D(400, 150), new Vector2D(430, 150), 30);
            AddLine(new Vector2D(430, 150), new Vector2D(600, 200), 30);
            AddLine(new Vector2D(600, 200), new Vector2D(700, 300), 30);

            AddLine(new Vector2D(1200, 200), new Vector2D(800, 420), 30);
            AddLine(new Vector2D(800, 420), new Vector2D(700, 470), 30);
            AddLine(new Vector2D(700, 470), new Vector2D(600, 486), 30);
            AddLine(new Vector2D(600, 486), new Vector2D(570, 486), 30);
            
            Scalar rest = coefficients.Restitution;
            coefficients.Restitution = 1;
            AddCircle(20, 20, 300, new ALVector2D(0, 409, 115));
            

            for (int x = 160; x < 500; x += 42)
            {
                Body b = AddCircle(20, 20, 300, new ALVector2D(0, x, 450));
                engine.AddJoint(new FixedHingeJoint(b, b.State.Position.Linear - new Vector2D(0,500), new Lifespan()));
            }
            coefficients.Restitution = rest;
            EndDemoChange();
        }

        void DemoW()
        {
            BeginDemoChange();
            Reset(false);
            AddGravityField();
            Coefficients o = coefficients;
            coefficients = new Coefficients(1, .5f);
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            Body b1 = AddRectangle(750, 100, Scalar.PositiveInfinity, new ALVector2D(0, 0, 750 / 2));
            b1.IgnoresGravity = true;
            Body b2 = AddRectangle(750, 100, Scalar.PositiveInfinity, new ALVector2D(0, 1024, 750 / 2));
            b2.IgnoresGravity = true;
            coefficients = new Coefficients(.7f, .05f);


            for (int x = 60; x < 80; x += 10)
            {
                for (int y = -2000; y < 700; y += 12)
                {
                    Body g = AddCircle(5, 7, 3, new ALVector2D(0, x, y));
                    g.State.Velocity.Angular = 1;
                  //  g.State.Velocity.Linear = new Vector2D(0, 500);
                }
            }
            coefficients = o;

            EndDemoChange();
        }
        void DemoE()
        {
            BeginDemoChange();
            Reset(false);
            // Clear();
            // AddClipper();

            Vector2D gravityCenter = new Vector2D(500, 500);
            Scalar gravityPower = 200;
            engine.AddLogic(new GravityPointField(gravityCenter, gravityPower, new Lifespan()));
            AddRagDoll(gravityCenter + new Vector2D(0, -20));
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

                PolygonShape shape = new PolygonShape(vertexes, 1.5f);
                for (Scalar angle = 0; angle < MathHelper.TwoPi; angle += da)
                {
                    Vector2D position = Vector2D.FromLengthAndAngle(distance, angle) + gravityCenter;
                    Body body = AddShape(shape, (size * length) / 10, new ALVector2D(angle, position));
                    body.State.Velocity.Linear = GetOrbitVelocity(gravityCenter, Vector2D.FromLengthAndAngle(distance - length, angle) + gravityCenter, gravityPower);
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
            EndDemoChange();
        }
        void DemoR()
        {
            BeginDemoChange();
            Reset(false);
            AddTowers_StressTest();
            EndDemoChange();
        }
        void DemoT()
        {
            BeginDemoChange();
            Reset(false);
            //  avatarBodies.Lifetime.IsExpired = true;
            //avatarBodies.IsCollidable = false;
            //engine.AddLogic(new GravityPointField(new Vector2D(500, 500), 200, new Lifespan()));
            //AddDiaganol_StressTest();
            //AddRandom_StressTest();
            //AddTowers_StressTest();
            AddText(
                "WELCOME TO THE PHYSICS2D.NET DEMO.\nPLEASE ENJOY MESSING WITH IT.\nA LOT OF HARD WORK WENT INTO IT,\nSO THAT YOU COULD ENJOY IT.\nPLEASE SEND FEEDBACK.\nEACH CHARACTER HERE IS AN\nACTUAL BODY IN THE ENGINE.\nTHIS IS TO SHOW OFF THE BITMAP\nTO POLYGON ALGORITHM."
                , new Vector2D(00, 00));

            EndDemoChange();
        }
        void DemoY()
        {
            BeginDemoChange();
            Reset(false);
            AddCircles_StressTest();
            EndDemoChange();
        }
        void DemoU()
        {
            BeginDemoChange();
            Reset(false);
            AddGravityField();
            Sprite backSprite = GetSprite("physicsplayGround.png");
            Vector2D[][] polygons = backSprite.Polygons;
            //MultiPartPolygon shape = new MultiPartPolygon(vertexes, 4);
            //Body b = AddShape(shape, 40, new ALVector2D(0, backSprite.Offset));
            foreach (Vector2D[] vertexes in polygons)
            {
                PolygonShape shape = new PolygonShape(vertexes, 10);
                Body b = AddShape(shape, Scalar.PositiveInfinity, new ALVector2D(0, backSprite.Offset));
                b.IgnoresGravity = true;
            }
            for (int x = 440; x < 480; x += 10)
            {
                for (int y = -2000; y < 0; y += 12)
                {
                    Body g = AddCircle(5, 7, 3, new ALVector2D(0, x + rand.Next(-400,400), y));
                    g.State.Velocity.Angular = 1;
                    g.Updated += new EventHandler<UpdatedEventArgs>(DemoU_Body_Updated);
                    //  g.State.Velocity.Linear = new Vector2D(0, 500);
                }
            }
            for (int x = 490; x < 510; x += 10)
            {
                for (int y = -550; y < -500; y += 12)
                {
                    Body g = AddRectangle(10, 20, 10, new ALVector2D(0, x + rand.Next(-400, 400), y));
                    g.State.Velocity.Angular = 1;
                    g.Updated += new EventHandler<UpdatedEventArgs>(DemoU_Body_Updated);
                    //  g.State.Velocity.Linear = new Vector2D(0, 500);
                }
            }
            EndDemoChange();
        }
        void DemoI()
        {
            BeginDemoChange();
            Reset(false);
            Sprite blockSprite = GetSprite("fighter.png");
            Vector2D[][] vertexes = blockSprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = blockSprite;

            for (Scalar x = 100; x < 900; x += 200)
            {
                for (Scalar y = 100; y < 900; y += 200)
                {
                    Body c = AddShape(shape, 40, new ALVector2D(0, new Vector2D(x,y)));
                    c.Updated += new EventHandler<UpdatedEventArgs>(DemoI_Body_Updated);
                }
            }
            EndDemoChange();
        }
        void DemoO()
        {
            BeginDemoChange();
            Reset(false);
            BoundingRectangle rect = this.clippersShape.Rectangle;
            rect.Min.X -= 75;
            rect.Min.Y -= 75;
            rect.Max.X += 75;
            rect.Max.Y += 75;
            AddShell(rect, 100, Scalar.PositiveInfinity);
            rect.Min.X += 100;
            rect.Min.Y += 100;
            rect.Max.X -= 100;
            rect.Max.Y -= 100;


            Scalar spacing = 10;
            for (Scalar x = rect.Min.X; x < rect.Max.X; x += spacing)
            {
                for (Scalar y = rect.Min.Y; y < rect.Max.Y; y += spacing)
                {
                    Scalar radius = rand.Next(2, 5);
                    Body circle = AddCircle(radius, 10, radius * 2, new ALVector2D(0, x, y));
                    circle.State.Velocity.Linear.X = rand.Next(-500, 501);
                    circle.State.Velocity.Linear.Y = rand.Next(-500, 501);
                }
            }

            EndDemoChange();
        }
       
        void DemoA()
        {

            BeginDemoChange();
            Reset(false);
            AddGravityField();
            Body body1IT;
            Body body2IT;
            
            body1IT = AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            Sprite blockSprite = GetSprite("fighter.png");
            Vector2D[][] vertexes = blockSprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = blockSprite;
           //  body1IT = AddShape(shape, 40, new ALVector2D(0, new Vector2D(200, 300)));
             body2IT = AddShape(shape, 40, new ALVector2D(0, new Vector2D(300, 300)));
            AdvGroupIgnorer ignore1 = new AdvGroupIgnorer();
            AdvGroupIgnorer ignore2 = new AdvGroupIgnorer();
            ignore1.IsInverted = true;
            ignore2.IsInverted = true;
            //ignore1.Groups.Add(1);
            //ignore2.IgnoredGroups.Add(1);
            ignore2.Groups.Add(1);
            ignore1.IgnoredGroups.Add(1);
            body1IT.EventIgnorer = ignore1;
            body2IT.EventIgnorer = ignore2;

            body1IT.Collided += new EventHandler<CollisionEventArgs>(body2_Collided);
            //body2IT.Collided += new EventHandler<CollisionEventArgs>(body2_Collided);

            EndDemoChange();
        }
        void DemoS()
        {
            BeginDemoChange();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));

            Sprite blockSprite = GetSprite("fighter.png");
            Vector2D[][] vertexes = blockSprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = blockSprite;
            for (int i = 128 * 3; i > -128; i -= 128)
            {
                Body b = AddShape(shape, 40, new ALVector2D(0, new Vector2D(600, 272 + i)));

            }



            Body lever = AddRectangle(20, 100, 9, new ALVector2D(0, 275, 300));
            FixedAngleJoint joint1 = new FixedAngleJoint(lever, new Lifespan());
            FixedHingeJoint joint2 = new FixedHingeJoint(lever, new Vector2D(200, 300), new Lifespan());
            joint1.Softness = .95f;
            engine.AddJointRange(new Joint[] { joint1, joint2 });

            lever = AddRectangle(20, 100, 9, new ALVector2D(0, 385, 300));
            joint1 = new FixedAngleJoint(lever, new Lifespan());
            joint2 = new FixedHingeJoint(lever, new Vector2D(460, 300), new Lifespan());
            joint1.Softness = .95f;
            engine.AddJointRange(new Joint[] { joint1, joint2 });


            Body ball = AddShape(new CircleShape(80, 20), 4000, new ALVector2D(0, new Vector2D(1028, 272)));
            ball.Transformation *= Matrix2x3.FromScale(new Vector2D(1, .8f));

            EndDemoChange();
            /*   BeginDemoChange();
               Reset(false);
               AddGravityField();
               engine.AddLogic(new GlobalFluidLogic(.45f, .02f, Vector2D.Zero, new Lifespan()));

               BoundingRectangle rect = this.clippersShape.Rectangle;
               rect.Min.X -= 75;
               rect.Min.Y -= 75;
               rect.Max.X += 75;
               rect.Max.Y += 75;
               AddShell(rect, 100, Scalar.PositiveInfinity).ForEach(delegate(Body b) { b.IgnoresGravity = true; });
            
               Sprite blockSprite = GetSprite("fighter.png");
               Vector2D[][] vertexes = blockSprite.Polygons;
               MultipartPolygon shape = new MultipartPolygon(vertexes, 4);
               shape.Tag = blockSprite;

               AddShape(shape, 40, new ALVector2D(0, new Vector2D(200, 300)));
               AddShape(shape, 40, new ALVector2D(0, new Vector2D(500, 300)));
               AddRectangle(50, 50, 50, new ALVector2D(0, 600, 600));
               AddRectangle(50, 50, 500, new ALVector2D(0, 600, 450));
               AddCircle(10, 9, 5, new ALVector2D(0, 600, 600));
               AddCircle(10, 9, 50, new ALVector2D(0, 500, 600));
               AddCircle(10, 9, 500, new ALVector2D(0, 400, 600));
               AddCircle(10, 9, 5000, new ALVector2D(0, 300, 600));



               EndDemoChange();*/
        }

        bool hasWater = false;
        void DemoD()
        {
            BeginDemoChange();
            Reset(false);
            hasWater = true;
            //engine.AddLogic(new GlobalFluidLogic(2.95f, .02f, new Vector2D(0, 0), new Lifespan()));
            engine.AddLogic(new LineFluidLogic(new Line(0, -1, -400), 1.95f, .02f, new Vector2D(0, 0), new Lifespan()));
            AddGravityField();

            BoundingRectangle rect = this.clippersShape.Rectangle;
            rect.Min.X -= 75;
            rect.Min.Y -= 75;
            rect.Max.X += 75;
            rect.Max.Y += 75;
            AddShell(rect, 100, Scalar.PositiveInfinity).ForEach(delegate(Body b) { b.IgnoresGravity = true; });

           // AddPyramid();


            Sprite blockSprite = GetSprite("fighter.png");
            Vector2D[][] vertexes = blockSprite.Polygons;
            MultiPolygonShape shape = new MultiPolygonShape(vertexes, 4);
            shape.Tag = blockSprite;

            //AddShape(shape, 200, new ALVector2D(0, new Vector2D(200, 300)));
            AddShape(shape, 100, new ALVector2D(0, new Vector2D(500, 300)));
            AddRectangle(50, 200, 25, new ALVector2D(0, 600, 600));
            AddRectangle(50, 100, 50, new ALVector2D(0, 200, 400));
           // AddRectangle(50, 200, 50, new ALVector2D(0, 500, 600));
            //AddRectangle(50, 50, 50, new ALVector2D(0, 600, 600));
            //AddRectangle(50, 50, 100, new ALVector2D(0, 600, 450));
            //AddCircle(30, 9, 5, new ALVector2D(0, 600, 200));
            //AddCircle(25, 9, 25, new ALVector2D(0, 600, 200));
            //AddCircle(20, 9, 50, new ALVector2D(0, 500, 200));

            EndDemoChange();
        }


      
        void body2_Collided(object sender, CollisionEventArgs e)
        {
           /* if ((sender == body1IT ||
                sender == body2IT) &&
                (e.Other == body1IT ||
                e.Other == body2IT))
            {*/
                Console.WriteLine("HAHA");
            //}
        }
        const int Min = 0;
        const int Non = 1;
        const int Max = 2;

        static void FillRect(int[,] has, BoundingRectangle clip, BoundingRectangle other)
        {
            ContainmentType inter = clip.Contains(other);
            if (inter == ContainmentType.Contains)
            {
                has[Non, Non]++;
            }
            else
            {
                has[Non, Non]++;
                bool hasMin = false;
                bool hasMax = false;
                if (other.Min.X < clip.Min.X)
                {
                    hasMin = true;
                    has[Min, Non]++;
                }
                else if (other.Max.X > clip.Max.X)
                {
                    hasMax = true;
                    has[Max, Non]++;
                }

                if (other.Min.Y < clip.Min.Y)
                {
                    has[Non, Min]++;
                    if (hasMax)
                    {
                        has[Max, Min]++;
                    }
                    else if (hasMin)
                    {
                        has[Min, Min]++;
                    }
                }
                else if (other.Max.Y > clip.Max.Y)
                {
                    has[Non, Max]++;
                    if (hasMax)
                    {
                        has[Max, Max]++;
                    }
                    else if (hasMin)
                    {
                        has[Min, Max]++;
                    }
                }
            }
        }

        void DemoI_Body_Updated(object sender, UpdatedEventArgs e)
        {
            Body b = (Body)sender;
            BoundingRectangle clip  = this.clippersShape.Rectangle;
            BoundingRectangle mainRect =b.Rectangle;
            ContainmentType inter = clip.Contains( mainRect);
            if (inter == ContainmentType.Intersects)
            {
                Scalar yDiff = clip.Max.Y - clip.Min.Y;
                Scalar xDiff = clip.Max.X - clip.Min.X;

                int[,] has = new int[3, 3];
                bool[,] needs = new bool[3, 3];

                int[,] needsTemp = new int[3, 3];
                FillRect(needsTemp, clip, mainRect);
                for (int x = 0; x < 3; ++x)
                {
                    for (int y = 0; y < 3; ++y)
                    {
                        if (needsTemp[x, y] > 0)
                        {
                            needs[(-(x - Non) + Non), (-(y - Non) + Non)] = true;
                        }
                    }
                }



                if (mainRect.Min.X < clip.Min.X)
                {
                    needs[Max, Non] = true;
                }
                else if (mainRect.Max.X > clip.Max.X)
                {
                    needs[Min, Non] = true;
                }
                if (mainRect.Min.Y < clip.Min.Y)
                {
                    needs[Non,Max] = true;
                }
                else if (mainRect.Max.Y > clip.Max.Y)
                {
                    needs[Non, Min] = true;
                }
                FillRect(has, clip, mainRect);
                foreach (BodyProxy proxy in b.Proxies)
                {
                    BoundingRectangle rect = proxy.Body2.Rectangle;
                    FillRect(has, clip, rect);
                }
                for (int x = 0; x < 3; ++x)
                {
                    for (int y = 0; y < 3; ++y)
                    {
                        bool add = false;
                        if (x != Non || y != Non)
                        {
                            if (needs[x, y] && has[x, y] == 0)
                            {
                                add = true;
                            }
                        }
                        
                        if (add)
                        {
                            Vector2D location = b.State.Position.Linear;
                            location.X -= xDiff * (Non - x);
                            location.Y -= yDiff * (Non - y);
                            foreach (BodyProxy prox in b.Proxies)
                            {
                                if (prox.Body1.Rectangle.Contains(location) != ContainmentType.Disjoint)
                                {
                                    add = false;
                                    break;
                                }
                            }
                            if (add)
                            {
                                Body prox = b.Duplicate();
                                prox.Updated += new EventHandler<UpdatedEventArgs>(DemoI_Body_Updated);
                                prox.Collided += new EventHandler<CollisionEventArgs>(DemoI_Body_Collided);
                                prox.State.Position.Linear = location;
                                AddBody(prox);
                                engine.AddProxy(b, prox, Matrix2x2.Identity);
                            }
                        }
                    }
                }
            }
            else if (inter == ContainmentType.Disjoint)
            {
                b.Lifetime.IsExpired = true;
            }
        }

        void DemoI_Body_Collided(object sender, CollisionEventArgs e)
        {
            Body b = (Body)sender;
            foreach( BodyProxy prox in  b.Proxies)
            {
                if (e.Other == prox.Body2)
                {
                    if (b.ID > e.Other.ID)
                    {
                        b.Lifetime.IsExpired = true;
                    }
                    else
                    {
                        e.Other.Lifetime.IsExpired = true;
                    }
                }
            }  
        }
        void DemoU_Body_Updated(object sender, UpdatedEventArgs e)
        {
            Body b = (Body)sender;
            if (b.State.Position.Linear.Y > 900)
            {
                b.State.Position.Linear.Y = -100;
            }
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


        double time = 0;
        int updates = 0;
        DateTime last = DateTime.Now;
        public void Update(Scalar dt)
        {
            engine.Update(dt);
            updated = true;
            updates++;
        }


        bool started;
        public void Reshape(object sender, EventArgs e)
        {
            foreach (Sprite s in sprites.Values)
            {
                s.Refresh();
            }
            clippersShape.Rectangle=(new BoundingRectangle(0, 0, Video.Screen.Width, Video.Screen.Height));
            lock (objects)
            {
                foreach (OpenGlObject obj in objects)
                {
                    obj.Invalidate();
                }
            }
            pauseSprite.Refresh();
            upsSprite.Refresh();
            for (int index = 0; index < numberSprites.Length; ++index)
            {
                numberSprites[index].Refresh();
            }
        }
        void noth(object o) { }


        public void Draw(object sender,EventArgs e)
        {


            if (hasWater)
            {
                Gl.glColor3f(0, 0, 1);
                Gl.glLoadIdentity();
                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex2f(-10, 400);
                Gl.glVertex2f(10000, 400);
                Gl.glVertex2f(10000, 1000);
                Gl.glVertex2f(-10, 1000);
                Gl.glEnd();
            }

          // bool b = engine.Bodies.Exists(delegate(Body bo) { return bo.IsTransformed; });
          // noth(b);
            Gl.glPointSize(3);
            if (sparkle && updated)
            {
                updated = false;
                AddParticles(sparkPoint, 20);
            }
            if (lazer != null && !lazer.Lifetime.IsExpired)
            {
                RaySegmentsShape segments2 = (RaySegmentsShape)lazer.Shape;
                RaySegment[] segments = segments2.Segments;
                for (int index = 0; index < segments.Length; ++index)
                {
                    RaySegmentsCollisionInfo info = lazerLogic.Collisions[index];
                    if (info.Distance != -1)
                    {
                        Vector2D position = segments[index].RayInstance.Origin + segments[index].RayInstance.Direction * info.Distance;
                        position = lazer.Matrices.ToWorld * position;
                        //AddParticles(position, 1);
                    }
                }
            }



            if (!started)
            {
                started = true;
                timer.IsRunning = true;
            }
            lock (objects)
            {
                objects.RemoveAll(delegate(OpenGlObject o) { if (o.Removed) { o.Dispose(); return true; } return false; });
                for (int index = objects.Count - 1; index >= 0; --index)
                {
                    objects[index].Draw();
                }
            }
          /*  if (timer.State == TimerState.Slow || timer.State == TimerState.Fast)
            {
                Gl.glLoadIdentity();
                Gl.glBegin(Gl.GL_QUADS);
                if (timer.State == TimerState.Slow)
                {
                    Gl.glColor3f(.5f, 0, 0);
                }
                else
                {
                    Gl.glColor3f(0, .5f, 0);
                }
                Gl.glVertex2f(0, 0);
                Gl.glVertex2f(10, 0);
                Gl.glVertex2f(10, 10);
                Gl.glVertex2f(0, 10);
                Gl.glEnd();
            }*/
            DrawUPS();
            if (timer.State == TimerState.Paused)
            {
                DrawPaused();
            }
        }
        int lastups = 0;
        private void DrawUPS()
        {
            DateTime now = DateTime.Now;
            TimeSpan diff = now.Subtract(last);
            time += diff.TotalSeconds;
            last = now;
            int ups = (int)(updates / time);
            if (ups < 0) { ups = 0; }
            ups = (ups + lastups) / 2;
            lastups = ups;
            if (time > 1)
            {
                // Console.WriteLine("Updates/Second: {0}", updates / time);
                updates = 0;
                time = 0;
            }
            Gl.glLoadIdentity();
            int sep = 4;
            string val = ups.ToString();


            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            /*if (timer.State == TimerState.Slow)
            {
                Gl.glColor3f(.5f, 0, 0);
            }
            else if(timer.State == TimerState.Fast)
            {
                Gl.glColor3f(0, .5f, 0);
            }*/
            Gl.glColor3f(1, 0, 0);

            int y = 0;

            upsSprite.Draw(10, y);
            int pos = sep + upsSprite.TextureWidth;
            for (int index = 0; index < val.Length; ++index)
            {
                int numIndex = val[index] - '0';
                SurfaceGl spr = numberSprites[numIndex];

                spr.Draw(pos, y);
                pos += sep + spr.TextureWidth;
            }

            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_BLEND);
        }
        private void DrawPaused()
        {
            Gl.glLoadIdentity();
            BoundingRectangle rect = this.clippersShape.Rectangle;
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glColor3f(1, 1, 1);
            pauseSprite.Draw((rect.Max.X - rect.Min.X) / 2 - pauseSprite.Surface.Width / 2, (rect.Max.Y - rect.Min.Y) / 2 - pauseSprite.Surface.Height / 2);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            Gl.glDisable(Gl.GL_BLEND);
        }
        #endregion
    }
}
