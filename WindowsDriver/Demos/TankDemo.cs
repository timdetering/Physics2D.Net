#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1fof the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Physics2D;
using Physics2D.CollidableBodies;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using System.Drawing;

using Physics2D.Joints;
using global::SdlDotNet;
namespace WindowsDriver.Demos
{


    public static class HeightMap
    {
        public static List<ICollidableBody> GetHeightMap(Vector2D start, float lengthper, float thickness, float[] heightDiffs, Coefficients coe)
        {
            Vector2D lastpos = start;
            List<ICollidableBody> returnvalue = new List<ICollidableBody>();
            foreach (float height in heightDiffs)
            {
                Vector2D newpos = lastpos;
                newpos.X += lengthper;
                newpos.Y += height;
                returnvalue.Add(GetBlock(lastpos, newpos, thickness, coe));
                lastpos = newpos;
                //lastpos.X -= lengthper * .01f;
            }
            return returnvalue;
        }
        static int[] colors = new int[] { Color.Green.ToArgb(), Color.Green.ToArgb(), Color.Gray.ToArgb(), Color.Gray.ToArgb() };
        static ICollidableBody GetBlock(Vector2D start, Vector2D stop, float thickness, Coefficients coe)
        {
            Vector2D[] verticies = new Vector2D[]{
                start,
                stop,
                stop,
                start,
            };
            verticies[2].Y += thickness;
            verticies[3].Y += thickness;
            float Area = Polygon2D.CalcArea(verticies);
            Vector2D center = Polygon2D.CalcCentroid(verticies);
            verticies = Vector2D.Translate(-center, verticies);
            PhysicsState state = new PhysicsState();
            state.Position.Linear = center;
            RigidBodyPart[] parts = new RigidBodyPart[] { new RigidBodyPart(ALVector2D.Zero, new Polygon2D(verticies), coe) };
            return new RigidBody(new Immortal(), new MassInertia(float.PositiveInfinity, float.PositiveInfinity), state, BodyFlags.InfiniteMass | BodyFlags.NoImpulse | BodyFlags.IgnoreGravity, parts);
        }
    }
    public class TankDemo : BaseDisplayDemo
    {
        protected PhysicsState InitialShipState;
        protected RigidBody mainship;
        protected float scale = 20;

        DateTime scalechanged = DateTime.Now;
        DateTime keypressed = DateTime.Now;
        DateTime paused = DateTime.Now;
        Random rand = new Random();
        DateTime lastFired = DateTime.Now;
        DateTime lastFiredSecondary = DateTime.Now;

        protected float ShipForce = 4000;
        protected float ShipTorque = 400;
        protected int ProjectilesVelocity = 1200;
        protected int ProjectilesMass = 900;
        public override string Name
        {
            get
            {
                return "Tank Demo";
            }
        }
        public override string Description
        {
            get
            {
                return "You Control a Tank on uneven ground";
            }
        }
        public override string Instructions
        {
            get
            {
                return "Movement:\n" +
                    "Left     == A\n" +
                    "Right     == D\n" +
                    "Force Right    == W\n" +
                    "Force Left   == S\n" +
                    "Rotate Tank Left      == LeftArrow\n" +
                    "Rotate Tank Right   == RightArrow\n" +
                    
                    "Pause 	== P \n" +
                    "ZoomIn 	== PageUp \n" +
                    "ZoomOut 	== PageDown \n\n";
            }
        }
        public TankDemo()
            : base()
        {
            InitialShipState = new PhysicsState();
        }
        public override void InitObjects()
        {
            

        }
        RigidBody[] wheels;
        private void CreateTank()
        {
            List<Vector2D> vertices = new List<Vector2D>();

            vertices.Add(new Vector2D(-0.4f, .1f));
            vertices.Add(new Vector2D(-0.5f, .04f));
            vertices.Add(new Vector2D(-0.5f, 0.01f));
            vertices.Add(new Vector2D(-0.4f, -.07f));
            vertices.Add(new Vector2D(0.4f, -.07f));
            vertices.Add(new Vector2D(0.5f, 0.01f));
            vertices.Add(new Vector2D(0.5f, 0.03f));
            vertices.Add(new Vector2D(0.4f, .1f));

            vertices.Add(new Vector2D(-0.2f, 0.05f));
            vertices.Add(new Vector2D(-0.15f, -.08f));
            vertices.Add(new Vector2D(0.11f, -.08f));
            vertices.Add(new Vector2D(0.2f, .05f));



            RigidBodyPart partHull = new RigidBodyPart(
                new ALVector2D(0, new Vector2D(0, 0)),
                new Polygon2D(vertices.GetRange(0, 8).ToArray()),
                new Coefficients(0.7f, 0.4f, 0.2f));

            RigidBodyPart partTower = new RigidBodyPart(
                new ALVector2D(0, new Vector2D(0, -0.071f)),
                new Polygon2D(vertices.GetRange(8, 4).ToArray()),
                new Coefficients(0.7f, 0.6f, 0.4f));
            Coefficients wheelCoefficients = new Coefficients(0.1f, 0.2f, 0.2f);

            /*RigidBodyPart wheelPart = new RigidBodyPart(
                new ALVector2D(),
                new Circle2D(0.06f),
                wheelCoefficients);*/
            float wheelmass = 1f;
            float wheelradius = 0.06f;
            RigidBodyPart wheelPart = new RigidBodyPart(
                new ALVector2D(),
                new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 10,wheelradius)),
                wheelCoefficients);
            Vector2D pos = new Vector2D(3, 1);
            wheels = new RigidBody[5];
            RigidBody tank = new RigidBody(new MassInertia(50, 6), new PhysicsState(new ALVector2D(0, pos + new Vector2D(0, 0))), new RigidBodyPart[] { partHull, partTower });
            tank.IgnoreInfo.JoinGroupToIgnore(0);

            MassInertia wheelMI = MassInertia.FromSolidCylinder(wheelmass, wheelradius);
            wheels[0] = new RigidBody(wheelMI, new PhysicsState(new ALVector2D(0, pos + new Vector2D(-0.4f, 0.1f))), new RigidBodyPart[] { (RigidBodyPart)wheelPart.Clone() });
            wheels[1] = new RigidBody(wheelMI, new PhysicsState(new ALVector2D(0, pos + new Vector2D(-0.2f, 0.12f))), new RigidBodyPart[] { (RigidBodyPart)wheelPart.Clone() });
            wheels[2] = new RigidBody(wheelMI, new PhysicsState(new ALVector2D(0, pos + new Vector2D(0, 0.12f))), new RigidBodyPart[] { (RigidBodyPart)wheelPart.Clone() });
            wheels[3] = new RigidBody(wheelMI, new PhysicsState(new ALVector2D(0, pos + new Vector2D(0.2f, 0.12f))), new RigidBodyPart[] { (RigidBodyPart)wheelPart.Clone() });
            wheels[4] = new RigidBody(wheelMI, new PhysicsState(new ALVector2D(0, pos + new Vector2D(0.4f, 0.1f))), new RigidBodyPart[] { (RigidBodyPart)wheelPart.Clone() });
            foreach (RigidBody wheel in wheels)
            {
                wheel.IgnoreInfo.JoinGroupToIgnore(0);
            }
            PinJoint joint1 = new PinJoint(new CollidablePair(tank, wheels[0]), wheels[0].Good.Position.Linear, 0, .1f);
            PinJoint joint2 = new PinJoint(new CollidablePair(tank, wheels[1]), wheels[1].Good.Position.Linear, 0, .1f);
            PinJoint joint3 = new PinJoint(new CollidablePair(tank, wheels[2]), wheels[2].Good.Position.Linear, 0, .1f);
            PinJoint joint4 = new PinJoint(new CollidablePair(tank, wheels[3]), wheels[3].Good.Position.Linear, 0, .1f);
            PinJoint joint5 = new PinJoint(new CollidablePair(tank, wheels[4]), wheels[4].Good.Position.Linear, 0, .1f);


            this.mainship = tank;
            world.AddICollidableBody(wheels[0]);
            world.AddICollidableBody(wheels[1]);
            world.AddICollidableBody(wheels[2]);
            world.AddICollidableBody(wheels[3]);
            world.AddICollidableBody(wheels[4]);
            world.AddICollidableBody(tank);
            world.AddIJoint(joint1);
            world.AddIJoint(joint2);
            world.AddIJoint(joint3);
            world.AddIJoint(joint4);
            world.AddIJoint(joint5);

        }
        public override void AddObjects()
        {
            float leghtper =  5;
            float thickness = 5;
            float heightdiff = .25f;
            Vector2D start = new Vector2D(-10, -15);

            /*for (int pos = 0; pos < 10; ++pos)
            {
                PhysicsState state = new PhysicsState();
                state.Position.Linear = new Vector2D((float)rand.NextDouble()*10,-(float)rand.NextDouble()*10);
                world.AddICollidableBody(new RigidBody(new Polygon2D(3, .1f), MassInertia.FromSolidCylinder(2, .1f), state));

            }*/
            for (int pos = 0; pos < 10; ++pos)
            {
                PhysicsState state = new PhysicsState();
                state.Position.Linear = new Vector2D((float)rand.NextDouble() * 10, -(float)rand.NextDouble() * 10);
                world.AddICollidableBody(new RigidBody(MassInertia.FromSolidCylinder(2, .05f), state, new Circle2D(.05f)));
            }
            world.AddIGravitySource(new StaticGravityField(new Vector2D(0, 9.8f)));
            world.CalcGravity = true ;
            CreateTank();
            Coefficients coe = new Coefficients(.7f,.1f,.1f);
            //world.AddICollidableBody(mainship);

            int length = 100;
            float[] heightdiffs = new float[length];
            for(int pos = 0; pos <length;++pos)
            {
                heightdiff *= 1.05f;
                heightdiffs[pos] = ((float)rand.NextDouble() - .5f) * heightdiff;
            }
            heightdiffs[0] = 10;
            heightdiffs[1] = 10;
            heightdiffs[length - 2] = -10;
            heightdiffs[length - 1] = -10;
            List<ICollidableBody> cs = HeightMap.GetHeightMap(start, leghtper, thickness, heightdiffs, coe);
            foreach (ICollidableBody c in cs)
            {
                world.AddICollidableBody(c);
            }
            bubble = new ShipBubble(lightninglenght, mainship, lightningcollidables);
            world.AddICollidableArea(bubble);
        }
        public override Vector2D CameraPosition
        {
            get
            {
                if (mainship == null)
                {
                    return base.CameraPosition;
                }
                else
                {
                    return mainship.Good.Position.Linear;
                }
            }
        }
        public override float Scale
        {
            get
            {
                return scale;
            }
        }


        //forward rays
        protected float forwardlenght = 700;
        protected float forwardimpulse = 5000000;
        protected float forwarddeltaAngle = .03f;
        protected float forwardendAngle = 1.9f;
        //lighting rays
        protected float lightninglenght = 4000;
        protected float lightningimpulse = 5000000;
        protected float lightningdisplace = 2000;
        protected float lightningdetail = 200;
        //omniDirectional rays
        protected float omniDirectionalimpulse = 50000000;
        protected float omniDirectionallenght = 900;
        protected float omniDirectionaldeltaAngle = .03f;
        ShipBubble bubble;

        protected List<ICollidableBody> lightningcollidables = new List<ICollidableBody>();
        float wheelTorque = 20;
        float maxwheelspeed = 80;
        public override void UpdateKeyBoard(KeyboardState keys, float dt)
        {
            float wheelspeed = 0;
            foreach (RigidBody wheel in wheels)
            {
                wheelspeed += wheel.Current.Velocity.Angular;
            }
            wheelspeed = wheelspeed / wheels.Length;
            if ((float)Math.Abs(wheelspeed) > maxwheelspeed)
            {
                wheelspeed = (float)Math.Sign(wheelspeed)*maxwheelspeed;
            }
            foreach (RigidBody wheel in wheels)
            {
                wheel.Current.Velocity.Angular = wheelspeed;
            }
            DateTime now = DateTime.Now;
            if (now.Subtract(keypressed).TotalSeconds < Physics2D.Physics.Tolerance)
            {
                return;
            }
            keypressed = now;
            if (keys[Key.P])
            {
                if (now.Subtract(paused).TotalSeconds > .35)
                {
                    this.world.Enabled = false;
                }
                paused = now;
            }
            if (keys[Key.UpArrow])
            {
                wheelTorque += 10;
                maxwheelspeed += 2;
                //this.mainship.ApplyForce(new ForceInfo(new Vector2D(ShipForce, 0), Vector2DTransformType.ToBodyAngular));
            }
            if (keys[Key.DownArrow])
            {
                wheelTorque -= 10;
                maxwheelspeed -= 2;

                //this.mainship.ApplyForce(new ForceInfo(new Vector2D(-ShipForce, 0), Vector2DTransformType.ToBodyAngular));
            }
            if (keys[Key.RightArrow])
            {
                this.mainship.ApplyTorque(ShipTorque);

            }
            else if (keys[Key.LeftArrow])
            {
                this.mainship.ApplyTorque(-ShipTorque);
            }
            
            if (keys[Key.W])
            {
                this.mainship.ApplyForce(new ForceInfo(new Vector2D(ShipForce, 0), Vector2DTransformType.ToWorldAngular));
            }
            if (keys[Key.S])
            {
                this.mainship.ApplyForce(new ForceInfo(new Vector2D(-ShipForce, 0), Vector2DTransformType.ToWorldAngular));
            }
            if (keys[Key.A])
            {
                foreach (RigidBody wheel in wheels)
                {
                    wheel.ApplyTorque(-wheelTorque);
                }
                //this.mainship.ApplyForce(new ForceInfo(new Vector2D(0, -ShipForce), Vector2DTransformType.ToBodyAngular));
            }
            if (keys[Key.D])
            {
                foreach (RigidBody wheel in wheels)
                {
                    wheel.ApplyTorque(wheelTorque);
                }
            }
            
            if (keys[Key.PageDown])
            {
                //DateTime now = DateTime.Now;
                if (now.Subtract(scalechanged).TotalSeconds > .25)
                {
                    scalechanged = now;
                    this.scale /= 1.5f;
                }
            }
            else if (keys[Key.PageUp])
            {
                //DateTime now = DateTime.Now;
                if (now.Subtract(scalechanged).TotalSeconds > .25)
                {
                    scalechanged = now;
                    this.scale *= 1.5f;
                }
            }
            /*TimeSpan timesincefired = now.Subtract(lastFired);
            if (timesincefired.TotalMilliseconds > 100)
            {

                float radius = 10;
                float mass = ProjectilesMass;
                float lifetime = 5;
                float velocity = ProjectilesVelocity;
                if (keys[Key.Space])
                {
                    lastFired = now;
                    float anglediff = (float)rand.NextDouble() * .2 - .1f;
                    Vector2D position = this.mainship.Current.Position.Linear + Vector2D.FromLengthAndAngle(this.mainship.BoundingRadius + radius + 10, mainship.Current.Position.Angular);
                    RigidBody projectile = new RigidBody(new Polygon2D(4, radius), MassInertia.FromSolidCylinder(mass, radius), this.mainship.Current.Position.Angular, position);
                    Vector2D LV = Vector2D.FromLengthAndAngle(velocity, mainship.Current.Position.Angular + anglediff);
                    projectile.Current.Velocity.Linear = mainship.Current.Velocity.Linear + LV;
                    projectile.LifeTime = new Mortal(lifetime);
                    if (keys[Key.LeftShift])
                    {
                        projectile.CollisionState.GenerateCollisionEvents = true;
                    }
                    world.AddICollidableBody(projectile);
                    if (!keys[Key.C])
                    {
                        mainship.Current.Velocity.Linear -= LV * (mass / mainship.MassInertia.Mass);
                    }
                }
            }*/
        }
        public override IDemo CreateNew()
        {
            return new TankDemo();
        }

    }
}
