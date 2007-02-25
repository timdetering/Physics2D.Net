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

    class GlDrawObject
    {
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

        void entity_Removed(object sender, EventArgs e)
        {
            this.entity.StateChanged -= entity_NewState;
            this.entity.Removed -= entity_Removed;
            this.removed = true;
        }

        public void Remove()
        {
            if (Gl.glIsList(list) != 0)
            {
                Gl.glDeleteLists(list, 1);
            }
            list = -1;
        }
        void entity_NewState(object sender, EventArgs e)
        {
            Matrix3x3 mat = entity.Shape.Matrix.VertexMatrix;
            Matrix3x3.Copy2DToOpenGlMatrix(ref mat, matrix);
        }

        public Body Entity
        {
            get { return entity; }
        }

        void DrawInternal()
        {
            if (entity.Shape is Physics2DDotNet.Particle)
            {
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glColor3f(0, 1, 0);
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
                        Gl.glColor3f(1, 0, 0);
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
            if (entity.Lifetime.IsExpired)
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
    }
    
    class Demo
    {
        static Random rand = new Random();
        ManualResetEvent waitHandle;
        PhysicsEngine engine;
        int count = 0;
        Stopwatch watch;
        List<GlDrawObject> objects;
        Body bomb;
        Body avatar;
        Vector2D bombTarget;
        float friction = .3f;
        public Demo()
        {
            Events.MouseButtonDown += new EventHandler<SdlDotNet.Input.MouseButtonEventArgs>(Events_MouseButtonDown);
            Events.MouseButtonUp +=new EventHandler<SdlDotNet.Input.MouseButtonEventArgs>(Events_MouseButtonUp); 
            Events.KeyboardDown += new EventHandler<SdlDotNet.Input.KeyboardEventArgs>(Events_KeyboardDown);
            Events.KeyboardUp += new EventHandler<SdlDotNet.Input.KeyboardEventArgs>(Events_KeyboardUp);
            Events.MouseMotion += new EventHandler<SdlDotNet.Input.MouseMotionEventArgs>(Events_MouseMotion);
            waitHandle = new ManualResetEvent(true);
            watch = new Stopwatch();
            engine = new PhysicsEngine();
            objects = new List<GlDrawObject>();
            engine.BroadPhase = new Physics2DDotNet.Detectors.SweepAndPruneDetector();
            Physics2DDotNet.Solvers.SequentialImpulsesSolver solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();
            solver.Iterations = 13;
            solver.BiasPreservesMomentum = true;
            solver.BiasFactor = .7f;
            solver.AllowedPenetration = .01f;
            engine.Solver = solver;

            bomb = new Body(new PhysicsState(),
                new Physics2DDotNet.Circle(20, 20),
                120,
                 new Coefficients(.2f, .2f, friction),
                 new Lifespan());
            CreateAvatar();
            Demo1();

            Console.WriteLine(count);
        }

        void Events_MouseMotion(object sender, SdlDotNet.Input.MouseMotionEventArgs e)
        {
            if (sparkle)
            {
                sparkPoint = new Vector2D(e.X, e.Y);
            }
        }




        float forceMag = 5000;
        Vector2D force;
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
        void AddAvatar()
        {
            avatar.State.Position.Linear = new Vector2D(200, 200);
            avatar.State.Velocity.Linear =  Vector2D.Zero;
            avatar.ApplyMatrix();
            count++;
            AddGlObject(avatar);
            engine.AddBody(avatar);
        }
        void avatar_Updated(object sender, UpdatedEventArgs e)
        {
            avatar.State.ForceAccumulator.Linear += force;
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
                case SdlDotNet.Input.Key.LeftArrow :
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
        Vector2D sparkPoint;
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
        bool sparkle;

        void bomb_Removed(object sender, EventArgs e)
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

        void AddGlObject(Body obj)
        {
            lock (objects)
            {
                objects.Add(new  GlDrawObject(obj));
            }
        }
        void AddGlObjectRange(IList< Body> collection)
        {
            GlDrawObject[] arr = new GlDrawObject[collection.Count];
            for (int index = 0; index < arr.Length; ++index)
            {
                arr[index] = new GlDrawObject(collection[index]);
            }
            lock (objects)
            {
                objects.AddRange(arr);
            }
        }
        void AddBomb()
        {
            AddAvatar();
            count++;
            AddGlObject(bomb);
            bomb.Removed += new EventHandler(bomb_Removed);
            engine.AddBody(bomb);
        }
        void AddFloor(ALVector2D position)
        {
            count++;
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
                new Physics2DDotNet.Line(vertexes, thickness, thickness/2),
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
                    joint.Relaxation = 0.5f;
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

            float xmin = 200;
            float xmax = 900;
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
        Body AddRectangle(float length, float width, float mass, ALVector2D position)
        {
            width += rand.Next(-4, 5) * .01f;
            length += rand.Next(-4, 5) * .01f;
            count++;
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
            count++;

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
        void Clear()
        {
            RemoveBombEvents();
            engine.Clear();
            objects.Clear();
            count = 0;
            GC.Collect();
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
        void AddParticles(Vector2D position,int count)
        {
            Body[] particles = new Body[count];
            float angle = MathHelper.TWO_PI / count;

            for (int index = 0; index < count;++index )
            {
                Body f = new Body(
                    new PhysicsState(new ALVector2D(0, position)),
                    new Particle(), 
                    1,
                    new Coefficients(.2f, .2f, friction),
                    new Lifespan(.5f));

                f.State.Velocity.Linear = Vector2D.FromLengthAndAngle(rand.Next(200, 1001), index * angle + ((float)rand.NextDouble()-.5f )* angle);
                //f.State.Velocity.Linear = new Vector2D(rand.Next(-1000, 1001), rand.Next(-1000, 1001));
                particles[index] = f;

            }
            AddGlObjectRange(particles);
            engine.AddBodyRange(particles);
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

        void Demo1()
        {
            waitHandle.Reset();
            Clear();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddBomb();
            AddRectangle(40, 40, 20, new ALVector2D(0, new Vector2D(600, 300)));
            Console.WriteLine(count);
            waitHandle.Set();
        }
        void Demo2()
        {
            waitHandle.Reset();
            Clear();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddBomb();
            AddTower();
            Console.WriteLine(count);
            waitHandle.Set();
        }
        void Demo3()
        {
            waitHandle.Reset();
            Clear();
            AddGravityField();
            AddFloor(new ALVector2D(.1f, new Vector2D(600, 760)));
            AddBomb();
            AddTower();
            Console.WriteLine(count);
            waitHandle.Set();
        }
        void Demo4()
        {
            waitHandle.Reset();
            Clear();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddBomb();
            AddPyramid();
            Console.WriteLine(count);
            waitHandle.Set();
        }
        void Demo5()
        {
            waitHandle.Reset();
            Clear();
            AddGravityField();
            AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddBomb();
            AddTowers();
            Console.WriteLine(count);
            waitHandle.Set();
        }
        void Demo6()
        {
            waitHandle.Reset();
            Clear();
            AddGravityField();
            // AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddBomb();

            List<Body> chain = AddChain(new Vector2D(400, 50), 100, 30, 200, 10, 800);
            Vector2D point = new Vector2D(300, 50);

            Body Anchor = AddRectangle(60, 60, float.PositiveInfinity, new ALVector2D(0, point));
            Anchor.IgnoresGravity = true;
            HingeJoint joint = new HingeJoint(chain[0], Anchor, point, new Lifespan());
            engine.AddJoint(joint);
            Console.WriteLine(count);
            waitHandle.Set();
        }
        void Demo7()
        {
            waitHandle.Reset();
            Clear();
            engine.AddLogic(new GravityPointField(new Vector2D(500, 500), 200, new Lifespan()));
            AddBomb();
            AddTowers();
            Console.WriteLine(count);
            waitHandle.Set();
        }
        void Demo8()
        {
            waitHandle.Reset();
            Clear();
            AddGravityField();
            // AddFloor(new ALVector2D(0, new Vector2D(700, 750)));
            AddBomb();

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
            joint1.Relaxation = 0.01f;
            joint2.Relaxation = 0.01f;
            end2.State.Position.Linear.X -= 10;
            end1.State.Position.Linear.X += 10;
            end2.ApplyMatrix();
            end1.ApplyMatrix();

            AddTower2();


            Console.WriteLine(count);
            waitHandle.Set();
        }
        void Demo9()
        {
            waitHandle.Reset();
            Clear();
            AddGravityField();
            AddBomb();


            AddLine(new Vector2D(0, 700), new Vector2D(300, 700), 30);
            AddLine(new Vector2D(300, 700), new Vector2D(400, 650), 30);
            AddLine(new Vector2D(400, 650), new Vector2D(500, 650), 30);
            AddLine(new Vector2D(500, 650), new Vector2D(500, 500), 30);
            AddLine(new Vector2D(500, 500), new Vector2D(900, 550), 30);
            AddLine(new Vector2D(400, 400), new Vector2D(600, 300), 30);

            waitHandle.Set();
        }

        bool isSlow;
        bool isFast;
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
                    if (dt > .020f)
                    {
                        isSlow = true;
                        dt = .020f;
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

        bool started = false;
        bool updated;
        public void Draw(int width, int height)
        {
            if (sparkle && updated)
            {
                updated = false;
                AddParticles(sparkPoint, 50);
            }

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
            if (isSlow|| isFast)
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
                objects.RemoveAll(delegate(GlDrawObject o) { if (o.Removed) { o.Remove(); return true; } return false; });
                foreach (GlDrawObject obj in objects)
                {
                    obj.Draw();
                }
            }
        }
    }
}
