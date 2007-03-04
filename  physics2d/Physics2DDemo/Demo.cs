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
using AdvanceMath;
using Physics2DDotNet.Math2D;
using System.Media;
using Tao.OpenGl;
using SdlDotNet.Core;
using System.Diagnostics;
namespace Physics2DDemo
{
    class Demo
    {

        #region fields
        static Random rand = new Random();
        ManualResetEvent waitHandle;
        PhysicsEngine engine;
        Stopwatch watch;
        List<GlDrawObject> objects;
        Body bomb;
        Body avatar;
        Body clipper;
        BoundingBox2DShape clippersShape;
        Vector2D bombTarget;
        float friction = .3f;

        float forceMag = 5000;
        Vector2D force;

        bool isSlow;
        bool isFast;

        bool started;
        bool updated;

        bool sparkle;
        Vector2D sparkPoint;
        #endregion
        #region constructor
        public Demo()
        {

            Events.MouseButtonDown += new EventHandler<SdlDotNet.Input.MouseButtonEventArgs>(Events_MouseButtonDown);
            Events.MouseButtonUp += new EventHandler<SdlDotNet.Input.MouseButtonEventArgs>(Events_MouseButtonUp);
            Events.KeyboardDown += new EventHandler<SdlDotNet.Input.KeyboardEventArgs>(Events_KeyboardDown);
            Events.KeyboardUp += new EventHandler<SdlDotNet.Input.KeyboardEventArgs>(Events_KeyboardUp);
            Events.MouseMotion += new EventHandler<SdlDotNet.Input.MouseMotionEventArgs>(Events_MouseMotion);
            waitHandle = new ManualResetEvent(true);
            watch = new Stopwatch();
            objects = new List<GlDrawObject>();





            CreateEngine();
            CreateBomb();
            CreateAvatar();
            CreateClipper();
            Demo1();
        }
        #endregion
        #region methods

        #region event handlers
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
            if (sparkle)
            {
                sparkPoint = new Vector2D(e.X, e.Y);
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
                case SdlDotNet.Input.Key.LeftArrow:
                    force += new Vector2D(-forceMag, 0);
                    break;
                case SdlDotNet.Input.Key.RightArrow:
                    force += new Vector2D(forceMag, 0);
                    break;
                case SdlDotNet.Input.Key.UpArrow:
                    avatar.IgnoresGravity = true;
                    force += new Vector2D(0, -forceMag);
                    break;
                case SdlDotNet.Input.Key.DownArrow:
                    force += new Vector2D(0, forceMag);
                    break;
            }
        }
        void Events_KeyboardUp(object sender, SdlDotNet.Input.KeyboardEventArgs e)
        {
            switch (e.Key)
            {
                case SdlDotNet.Input.Key.LeftArrow:
                    force -= new Vector2D(-forceMag, 0);
                    break;
                case SdlDotNet.Input.Key.RightArrow:
                    force -= new Vector2D(forceMag, 0);
                    break;
                case SdlDotNet.Input.Key.UpArrow:
                    avatar.IgnoresGravity = false;
                    force -= new Vector2D(0, -forceMag);
                    break;
                case SdlDotNet.Input.Key.DownArrow:
                    force -= new Vector2D(0, forceMag);
                    break;
            }
        }
        void Events_MouseButtonDown(object sender, SdlDotNet.Input.MouseButtonEventArgs e)
        {
            if (e.Button == SdlDotNet.Input.MouseButton.PrimaryButton)
            {
                bombTarget = new Vector2D(e.X, e.Y);
                bomb.Lifetime.IsExpired = true;
            }
            else if (e.Button == SdlDotNet.Input.MouseButton.SecondaryButton)
            {
                sparkPoint = new Vector2D(e.X, e.Y);
                sparkle = true;
                /* waitHandle.Reset();
                 AddParticles(new Vector2D(e.X, e.Y), 50);
                 waitHandle.Set();*/
            }
        }
        void Events_MouseButtonUp(object sender, SdlDotNet.Input.MouseButtonEventArgs e)
        {
            if (e.Button == SdlDotNet.Input.MouseButton.SecondaryButton)
            {
                sparkle = false;
            }
        }
        void bomb_Removed(object sender, RemovedEventArgs e)
        {
            Vector2D position = new Vector2D(rand.Next(0, 1400), 0);
            float velocityMag = rand.Next(1000, 2000);
            // Vector2D target = new Vector2D(rand.Next(400, 900), rand.Next(200, 700));
            Vector2D velocity = Vector2D.SetMagnitude(bombTarget - position, velocityMag);

            bomb.Lifetime = new Lifespan();
            bomb.State.Position.Linear = position;
            bomb.State.Velocity.Linear = velocity;
            engine.AddBody(bomb);
            AddGlObject(bomb);
        }
        void avatar_Updated(object sender, UpdatedEventArgs e)
        {
            avatar.State.ForceAccumulator.Linear += force;
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
            GlDrawObject o = (GlDrawObject)e.Other.Tag;
            o.collided = true;
        }
        void clipper_Updated(object sender, UpdatedEventArgs e)
        {
            try
            {
                for (int index = 0; index < objects.Count; ++index)
                {
                    GlDrawObject o = objects[index];
                    o.shouldDraw = o.collided;
                    o.collided = false;
                }
            }
            catch { }
        }
        #endregion

        /// <summary>
        /// initializes the PhysicsEngine
        /// </summary>
        void CreateEngine()
        {
            //creates it
            engine = new PhysicsEngine();

            //sets the broadphase
            engine.BroadPhase = new Physics2DDotNet.Detectors.SweepAndPruneDetector();

            //setups the Solver and sets it.
            Physics2DDotNet.Solvers.SequentialImpulsesSolver solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();
            solver.Iterations = 13;
            solver.SplitImpulse = true;
            solver.BiasFactor = .7f;
            solver.AllowedPenetration = .1f;
            engine.Solver = solver;

            engine.BodiesAdded += new EventHandler<CollectionEventArgs<Body>>(engine_BodiesAdded);
            engine.BodiesRemoved += new EventHandler<CollectionEventArgs<Body>>(engine_BodiesRemoved);
        }

        void CreateClipper()
        {
            clippersShape = new BoundingBox2DShape();
            clipper = new Body(new PhysicsState(), clippersShape, 0, new Coefficients(0, 0, 0), new Lifespan());
            clipper.IgnoresGravity = true;
            clipper.BroadPhaseDetectionOnly = true;
            clipper.Collided += new EventHandler<CollisionEventArgs>(clipper_Collided);
            clipper.Updated += new EventHandler<UpdatedEventArgs>(clipper_Updated);
        }




        void CreateBomb()
        {
            bomb = new Body(new PhysicsState(),
                    new Physics2DDotNet.Circle(20, 20),
                    120,
                    new Coefficients(.2f, .2f, friction),
                    new Lifespan());
        }
        void CreateAvatar()
        {
            float Ld2 = 30 / 2;
            float Wd2 = 10 / 2;


            Vector2D[] vertexes = new Vector2D[]{
                new Vector2D(Wd2*2.5f, Ld2*1.2f),
                new Vector2D(Wd2, Ld2*1.5f),
                new Vector2D(-Wd2, Ld2*1.5f),
                new Vector2D(-Wd2*2.5f, Ld2*1.2f),
                new Vector2D(-Wd2, -Ld2),
                new Vector2D(Wd2, -Ld2)
            };
            vertexes = Polygon.Subdivide(vertexes, 2);

            avatar = new Body(new PhysicsState(new ALVector2D(0, 0, 60)),
                new Polygon(vertexes, 4),
                new MassInfo(5, float.PositiveInfinity),
                new Coefficients(.2f, .2f, friction),
                new Lifespan());
            avatar.Updated += new EventHandler<UpdatedEventArgs>(avatar_Updated);
        }

        void AddBomb()
        {
            AddGlObject(bomb);
            bomb.Removed += bomb_Removed;
            engine.AddBody(bomb);
        }
        void AddAvatar()
        {
            avatar.State.Position.Linear = new Vector2D(200, 200);
            avatar.State.Velocity.Linear = Vector2D.Zero;
            avatar.ApplyMatrix();
            AddGlObject(avatar);
            engine.AddBody(avatar);
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
        void Reset()
        {
            Clear();
            AddClipper();
            AddBomb();
            AddAvatar();
        }

        void AddGlObject(Body obj)
        {
            lock (objects)
            {
                GlDrawObject o = new GlDrawObject(obj);
                obj.Tag = o;
                objects.Add(o);
            }
        }
        void AddGlObjectRange(IList<Body> collection)
        {
            GlDrawObject[] arr = new GlDrawObject[collection.Count];
            for (int index = 0; index < arr.Length; ++index)
            {
                arr[index] = new GlDrawObject(collection[index]);
                collection[index].Tag = arr[index];
            }
            lock (objects)
            {
                objects.AddRange(arr);
            }
        }
        void AddFloor(ALVector2D position)
        {
            Body line = new Body(
                new PhysicsState(position),
                new Physics2DDotNet.Polygon(Physics2DDotNet.Polygon.CreateRectangle(60, 2000), 40),
                new MassInfo(float.PositiveInfinity, float.PositiveInfinity),
                new Coefficients(.2f, .2f, friction),
                new Lifespan());
            line.IgnoresGravity = true;
            AddGlObject(line);
            engine.AddBody(line);
        }
        void AddLine(Vector2D point1, Vector2D point2, float thickness)
        {
            Vector2D line = point1 - point2;
            Vector2D avg = (point1 + point2) * .5f;
            float length = line.Magnitude;
            float angle = line.Angle;
            Vector2D[] vertexes = new Vector2D[]
            {
                new Vector2D(-length/2,0),
                new Vector2D(length/2,0)
            };


            Body body = new Body(
                new PhysicsState(new ALVector2D(angle, avg)),
                new Physics2DDotNet.Line(vertexes, thickness, thickness / 2),
                new MassInfo(float.PositiveInfinity, float.PositiveInfinity),
                new Coefficients(.2f, .2f, friction),
                new Lifespan());
            body.IgnoresGravity = true;
            AddGlObject(body);
            engine.AddBody(body);
        }
        List<Body> AddChain(Vector2D position, float boxLenght, float boxWidth, float boxMass, float spacing, float length)
        {
            List<Body> bodies = new List<Body>();
            Body last = null;
            for (float x = 0; x < length; x += boxLenght + spacing, position.X += boxLenght + spacing)
            {
                Body current = AddRectangle(boxWidth, boxLenght, boxMass, new ALVector2D(0, position));
                bodies.Add(current);
                if (last != null)
                {
                    Vector2D anchor = (current.State.Position.Linear + last.State.Position.Linear) * .5f;
                    HingeJoint joint = new HingeJoint(last, current, anchor, new Lifespan());
                    joint.SplitImpulse = true;
                    this.engine.AddJoint(joint);
                }
                last = current;
            }
            return bodies;
        }
        void AddPyramid()
        {


            float size = 30;
            float spacing = .01f;
            float Xspacing = 1f;

            float xmin = 300;
            float xmax = 800;
            float ymin = 50;
            float ymax = 720 - size / 2;
            float step = (size + spacing + Xspacing) / 2;
            float currentStep = 0;

            for (float y = ymax; y >= ymin; y -= (spacing + size))
            {
                for (float x = xmin + currentStep; x < (xmax - currentStep); x += spacing + Xspacing + size)
                {
                    AddRectangle(size, size, 20, new ALVector2D(.01f, new Vector2D(x, y)));
                }
                currentStep += step;
            }
        }
        void AddTowers()
        {
            float xmin = 200;
            float xmax = 800;
            float ymin = 400;
            float ymax = 700;

            float size = 25;
            float spacing = 20;
            float Xspacing = -1;

            for (float x = xmin; x < xmax; x += spacing + Xspacing + size)
            {
                for (float y = ymax; y > ymin; y -= spacing + size)
                {
                    AddRectangle(size, size, 20, new ALVector2D(0, new Vector2D(x + rand.Next(-1, 1) * .1f, y + rand.Next(-1, 1) * .1f)));
                }
            }
        }
        Body AddShape(Shape shape, float mass, ALVector2D position)
        {
            Body e =
                new Body(
                     new PhysicsState(position),
                     shape,
                     mass,
                     new Coefficients(.2f, .2f, friction),
                     new Lifespan());
            AddGlObject(e);
            engine.AddBody(e);
            return e;

        }
        Body AddRectangle(float length, float width, float mass, ALVector2D position)
        {
            width += rand.Next(-4, 5) * .01f;
            length += rand.Next(-4, 5) * .01f;
            Vector2D[] vertices = Physics2DDotNet.Polygon.CreateRectangle(length, width);
            vertices = Physics2DDotNet.Polygon.Subdivide(vertices, (length + width) / 4);

            Shape boxShape = new Physics2DDotNet.Polygon(vertices, MathHelper.Min(length, width) / 2);
            Body e =
                new Body(
                     new PhysicsState(position),
                     boxShape,
                     mass,
                     new Coefficients(.2f, .2f, friction),
                     new Lifespan());
            AddGlObject(e);
            engine.AddBody(e);
            return e;

        }
        Body AddCircle(float radius, int vertexCount, float mass, ALVector2D position)
        {

            Shape circleShape = new Physics2DDotNet.Circle(radius, vertexCount); ;
            Body e =
                new Body(
                     new PhysicsState(position),
                     circleShape,
                     mass,
                     new Coefficients(.2f, .2f, friction),
                     new Lifespan());
            AddGlObject(e);
            engine.AddBody(e);
            return e;

        }
        void RemoveBombEvents()
        {
            bomb.Removed -= bomb_Removed;
        }
        void AddTower()
        {
            float size = 30;
            float x = 500;
            float spacing = size + 2;

            float minY = 300;
            float maxY = 720 - size / 2;
            for (float y = maxY; y > minY; y -= spacing)
            {
                AddRectangle(size, size, 20, new ALVector2D(0, new Vector2D(x + rand.Next(-3, 4) * .1f, y)));
            }
        }
        void AddGravityField()
        {
            engine.AddLogic(new GravityField(new Vector2D(0, 960f), new Lifespan()));
        }
        void AddParticles(Vector2D position, int count)
        {
            Body[] particles = new Body[count];
            float angle = MathHelper.TWO_PI / count;

            for (int index = 0; index < count; ++index)
            {
                Body particle = new Body(
                    new PhysicsState(new ALVector2D(0, position)),
                    new Particle(),
                    1,
                    new Coefficients(.2f, .2f, friction),
                    new Lifespan(.5f));

                particle.State.Velocity.Linear = Vector2D.FromLengthAndAngle(rand.Next(200, 1001), index * angle + ((float)rand.NextDouble() - .5f) * angle);
                particles[index] = particle;
                particle.Collided += new EventHandler<CollisionEventArgs>(particle_Collided);

            }
            AddGlObjectRange(particles);
            engine.AddBodyRange(particles);
        }

        void ApplyMatrix(ALVector2D vector, IList<Body> collection)
        {
            Matrix2D matrix;
            Matrix2D.FromALVector2D(ref vector, out matrix);
            ApplyMatrix(matrix, collection);
        }
        void ApplyMatrix(Matrix2D matrix, IList<Body> collection)
        {
            foreach (Body b in collection)
            {
                b.ApplyMatrix(ref matrix);
            }
        }

        void AddTower2()
        {
            float size = 30;
            float x = 500;
            float spacing = size + 2;

            float minY = 200;
            float maxY = 400 - size / 2;
            for (float y = maxY; y > minY; y -= spacing)
            {
                AddRectangle(size, size, 20, new ALVector2D(0, new Vector2D(x + rand.Next(-3, 4) * .1f, y)));
            }
        }
        List<Body> AddRagDoll(Vector2D location)
        {
            List<Body> result = new List<Body>();
            float mass = 10;
            Body head = AddCircle(12, 9, mass, new ALVector2D(0, location + new Vector2D(0, 0)));

            float Ld2 = 50 / 2;
            float Wd2 = 25 / 2;
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
            Shape shape = new Polygon(vertexes, 5);

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
            List<HingeJoint> joints = new List<HingeJoint>();
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
                joint.SplitImpulse = true;
            }
            engine.AddJointRange<HingeJoint>(joints);
            return result;
        }


        void Demo1()
        {
            waitHandle.Reset();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddRectangle(40, 40, 20, new ALVector2D(0, new Vector2D(600, 300)));
            waitHandle.Set();
        }
        void Demo2()
        {
            waitHandle.Reset();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddTower();
            waitHandle.Set();
        }
        void Demo3()
        {
            waitHandle.Reset();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(.1f, new Vector2D(600, 760)));
            AddTower();
            waitHandle.Set();
        }
        void Demo4()
        {
            waitHandle.Reset();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));

            AddPyramid();
            waitHandle.Set();
        }
        void Demo5()
        {
            waitHandle.Reset();
            Reset();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddTowers();
            waitHandle.Set();
        }
        void Demo6()
        {
            waitHandle.Reset();
            Reset();
            AddGravityField();

            List<Body> chain = AddChain(new Vector2D(400, 50), 100, 30, 200, 10, 800);
            Vector2D point = new Vector2D(300, 50);

            Body Anchor = AddRectangle(60, 60, float.PositiveInfinity, new ALVector2D(0, point));
            Anchor.IgnoresGravity = true;
            HingeJoint joint = new HingeJoint(chain[0], Anchor, point, new Lifespan());
            engine.AddJoint(joint);
            waitHandle.Set();
        }
        void Demo7()
        {
            waitHandle.Reset();
            Reset();
            engine.AddLogic(new GravityPointField(new Vector2D(500, 500), 200, new Lifespan()));
            AddTowers();
            float y = 0;
            for (float x = 200; x < 500; x += 100, y -= 100)
            {
                AddRagDoll(new Vector2D(x, y));
            }
            waitHandle.Set();
        }
        void Demo8()
        {
            waitHandle.Reset();
            Reset();
            AddGravityField();

            float boxlength = 50;
            float spacing = 4;
            float anchorLenght = 30;
            float anchorGap = (boxlength / 2) + spacing + (anchorLenght / 2);
            List<Body> chain = AddChain(new Vector2D(200, 500), boxlength, 20, 200, spacing, 600);

            Vector2D point2 = new Vector2D(chain[chain.Count - 1].State.Position.Linear.X + anchorGap, 500);
            Body end2 = AddRectangle(anchorLenght, anchorLenght, float.PositiveInfinity, new ALVector2D(0, point2));
            end2.IgnoresGravity = true;
            HingeJoint joint2 = new HingeJoint(chain[chain.Count - 1], end2, point2, new Lifespan());
            engine.AddJoint(joint2);

            Vector2D point1 = new Vector2D(chain[0].State.Position.Linear.X - anchorGap, 500);
            Body end1 = AddRectangle(anchorLenght, anchorLenght, float.PositiveInfinity, new ALVector2D(0, point1));
            end1.IgnoresGravity = true;
            HingeJoint joint1 = new HingeJoint(chain[0], end1, point1, new Lifespan());
            engine.AddJoint(joint1);
            joint1.SplitImpulse = true;
            joint2.SplitImpulse = true;
            end2.State.Position.Linear.X -= 10;
            end1.State.Position.Linear.X += 10;
            end2.ApplyMatrix();
            end1.ApplyMatrix();

            AddTower2();


            waitHandle.Set();
        }
        void Demo9()
        {
            waitHandle.Reset();
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
            ApplyMatrix(new ALVector2D(MathHelper.PI, 400, 200), AddRagDoll(new Vector2D(0, 0)));
            AddRagDoll(new Vector2D(500, 100));
            AddRagDoll(new Vector2D(600, 0));
            AddRagDoll(new Vector2D(700, -100));

            waitHandle.Set();
        }
        //stress test
        void Demo0()
        {
            waitHandle.Reset();
            Reset();
            // Clear();
            // AddClipper();

            Vector2D gravityCenter = new Vector2D(500, 500);
            float gravityPower = 200;
            engine.AddLogic(new GravityPointField(gravityCenter, gravityPower, new Lifespan()));
            AddRagDoll(gravityCenter + new Vector2D(0, -20));
            float length = 41;
            float size = 4
                ;
            bool reverse = false;
            for (float distance = 180; distance < 500; length += 10, size += 10, distance += 60 + length)
            {

                float da = MathHelper.TWO_PI / size;// ((MathHelper.TWO_PI * distance) / size);
                float l2 = length / 2;
                Vector2D[] vertexes = new Vector2D[]
                {
                     Vector2D.FromLengthAndAngle(distance-l2,da/2),
                     Vector2D.FromLengthAndAngle(distance-l2,-da/2),
                     Vector2D.FromLengthAndAngle(distance+l2,-da/2),
                     Vector2D.FromLengthAndAngle(distance+l2,da/2),
                };
                Vector2D[] vertexes2 = Polygon.MakeCentroidOrigin(vertexes);
                vertexes = Polygon.Subdivide(vertexes2, 5);

                Polygon shape = new Polygon(vertexes, 1.5f);
                for (float angle = 0; angle < MathHelper.TWO_PI; angle += da)
                {

                    Vector2D position = Vector2D.FromLengthAndAngle(distance, angle) + gravityCenter;
                    Body body = AddShape(shape, size * length, new ALVector2D(angle, position));
                    // body.State.Velocity.Linear = GetOrbitVelocity(gravityCenter, Vector2D.FromLengthAndAngle(distance - length, angle) + gravityCenter, gravityPower);
                    if (reverse)
                    {
                        // body.State.Velocity.Linear = -body.State.Velocity.Linear;
                    }
                    //body.State.Velocity.Angular = -(MathHelper.TWO_PI * distance) / (body.State.Velocity.Linear.Magnitude) * (1 / MathHelper.TWO_PI);
                }
                reverse = !reverse;
            }
            waitHandle.Set();
        }
        public static Vector2D GetOrbitVelocity(Vector2D PosOfAccelPoint, Vector2D PosofShip, float AccelDoToGravity)
        {
            float MyOrbitConstant = 0.63661977236758134307553505349036f; //(area under 1/4 of a sin wave)/(PI/2)

            Vector2D distance = PosOfAccelPoint - PosofShip;
            float distanceMag = distance.Magnitude;
            Vector2D distanceNorm = distance * (1 / distanceMag);

            float VelocityForOrbit = MathHelper.Sqrt(2 * MyOrbitConstant * AccelDoToGravity * distanceMag);
            Vector2D orbitvelocity = distanceNorm.RightHandNormal * VelocityForOrbit;
            return orbitvelocity;
        }


        /// <summary>
        /// created this to run the physics update in becuase opengl was taking up to much CPU.
        /// </summary>
        public void PhysicsProcess()
        {

            watch.Start();
            while (true)
            {
                float dt = watch.ElapsedMilliseconds / 1000f;
                if (dt < .001f)
                {
                    //GC.Collect();
                    isFast = true;
                    Thread.Sleep(6);
                }
                else
                {

                    isFast = false;
                    if (dt > .015f)
                    {
                        isSlow = true;
                        dt = .015f;
                    }
                    else
                    {
                        isSlow = false;
                    }
                    watch.Reset();
                    watch.Start();
                    engine.Update(dt);
                    updated = true;
                }
                waitHandle.WaitOne();
            }

        }
        public void Draw(int width, int height)
        {
            Gl.glPointSize(3);
            if (sparkle && updated)
            {
                updated = false;
                AddParticles(sparkPoint, 50);
            }

            clippersShape.SetBoundingBox(new BoundingBox2D(width, height, 0, 0));

            if (!started)
            {
                started = true;
                Thread t = new Thread(PhysicsProcess);
                t.IsBackground = true;
                t.Start();
            }
            Gl.glTranslatef(width / 2, height / 2, 0);
            Gl.glRotatef(180, 0, 0, 1);
            Gl.glRotatef(180, 0, 1, 0);
            Gl.glTranslatef(-width / 2, -height / 2, 0);
            if (isSlow || isFast)
            {
                Gl.glBegin(Gl.GL_QUADS);
                if (isSlow)
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
            }
            lock (objects)
            {
                objects.RemoveAll(delegate(GlDrawObject o) { if (o.Removed) { o.Dispose(); return true; } return false; });
                foreach (GlDrawObject obj in objects)
                {
                    obj.Draw();
                }
            }
        }
        #endregion
    }

    class GlDrawObject : IDisposable
    {
        public bool collided = true;
        public bool shouldDraw = true;
        float[] matrix = new float[16];
        Body entity;
        int list = -1;
        bool removed;
        public bool Removed
        {
            get { return removed; }
        }

        public GlDrawObject(Body entity)
        {
            this.entity = entity;
            this.entity.StateChanged += entity_NewState;
            this.entity.Removed += entity_Removed;
        }
        void entity_Removed(object sender, RemovedEventArgs e)
        {
            this.entity.StateChanged -= entity_NewState;
            this.entity.Removed -= entity_Removed;
            this.removed = true;
        }
        void entity_NewState(object sender, EventArgs e)
        {
            Matrix3x3 mat = entity.Shape.Matrix.VertexMatrix;
            Matrix3x3.Copy2DToOpenGlMatrix(ref mat, matrix);
        }

        void DrawInternal()
        {
            if (entity.Shape is Physics2DDotNet.Particle)
            {
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glColor3f(1, 0, 0);
                foreach (Vector2D vector in entity.Shape.OriginalVertices)
                {
                    Gl.glVertex2f((float)vector.X, (float)vector.Y);
                }
                Gl.glEnd();
            }
            else if (entity.Shape is Physics2DDotNet.Line)
            {
                Physics2DDotNet.Line line = (Physics2DDotNet.Line)entity.Shape;
                Gl.glLineWidth((float)line.Thickness);
                Gl.glColor3f(0, 0, 1);
                Gl.glBegin(Gl.GL_LINE_STRIP);
                foreach (Vector2D vector in entity.Shape.OriginalVertices)
                {
                    Gl.glVertex2f((float)vector.X, (float)vector.Y);
                }
                Gl.glEnd();
            }
            else
            {
                Gl.glBegin(Gl.GL_POLYGON);
                bool first = true;
                bool second = true;
                foreach (Vector2D vector in entity.Shape.OriginalVertices)
                {
                    if (first)
                    {
                        Gl.glColor3f(1, .5f, 0);
                        first = false;
                    }
                    else if (second)
                    {
                        Gl.glColor3f(1, 1, 1);
                        second = false;
                    }
                    Gl.glVertex2f((float)vector.X, (float)vector.Y);
                }
                Gl.glEnd();
            }
        }

        public void Draw()
        {
            if (entity.Lifetime.IsExpired || !shouldDraw)
            {
                return;
            }
            if (Gl.glIsList(list) == 0)
            {
                Gl.glPushMatrix();
                Gl.glLoadIdentity();
                list = Gl.glGenLists(1);
                Gl.glNewList(list, Gl.GL_COMPILE);
                DrawInternal();
                Gl.glEndList();
                Gl.glPopMatrix();
            }
            Gl.glPushMatrix();
            Gl.glMultMatrixf(matrix);
            Gl.glCallList(list);
            Gl.glPopMatrix();
            
        }

        public void Dispose()
        {
            if (Gl.glIsList(list) != 0)
            {
                Gl.glDeleteLists(list, 1);
            }
            list = -1;
        }
    }
}
