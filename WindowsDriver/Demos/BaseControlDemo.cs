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
using Physics2D.CollidableAreas;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using System.Drawing;

using global::SdlDotNet;
namespace WindowsDriver.Demos
{
    public class BaseControlDemo : BaseDisplayDemo
    {
        protected PhysicsState InitialShipState;
        protected RigidBody mainship;
        protected float scale = 1;

        DateTime scalechanged = DateTime.Now;
        DateTime keypressed = DateTime.Now;
        DateTime paused = DateTime.Now;
        Random rand = new Random();
        DateTime lastFired = DateTime.Now;
        DateTime lastFiredSecondary = DateTime.Now;

        protected float ShipForce = 70000000;
        protected float ShipTorque = 900000000;
        protected int ProjectilesVelocity = 1200;
        protected int ProjectilesMass = 900;
        public override string Name
        {
            get
            {
                return "BaseControlDemo";
            }
        }
        public override string Description
        {
            get
            {
                return "adds a ship and controls of it to the BaseDisplayDemo";
            }
        }
        public override string Instructions
        {
            get
            {
                return "Movement:\n" +
                    "*Forwards      == W,UpArrow\n" +
                    "*Backwards     == S,DownArrow\n" +
                    "Strafe Left    == A\n" +
                    "Strafe Right   == D\n" +
                    "Turn Left      == LeftArrow\n" +
                    "Turn Right   == RightArrow\n" +
                    "Stop           == C\n\n" +
                    "*Holding down Both keys will float the force in that direction.\n\n" +
                    "Weapons:\n" +
                    "First == SpaceBar \n" +
                    "Second  == LeftCtrl \n" +
                    "Auto Targeting  == X \n" +
                    "Make Explosive == Shift \n" +
                    "Gravity Gun  == G \n\n" +
                    "Others: \n" +
                    "Pause 	== P \n" +
                    "ZoomIn 	== PageUp \n" +
                    "ZoomOut 	== PageDown \n\n"+
                    "Rays 	== P \n" +
                    "Foward Fan 	== E \n" +
                    "OmniDirectional 	== R \n"+
                    "Lightning 	== Z \n";
            }
        }
        public BaseControlDemo()
            : base()
        {
            InitialShipState = new PhysicsState();
        }
        public override void InitObjects()
        {
            Vector2D[] vertexlist = new Vector2D[6];
            vertexlist[0] = new Vector2D(50, 30);
            vertexlist[1] = new Vector2D(-50, 30);
            vertexlist[2] = new Vector2D(-50, -30);
            vertexlist[3] = new Vector2D(50, -30);
            vertexlist[4] = new Vector2D(60, -10);
            vertexlist[5] = new Vector2D(60, 10);
            Polygon2D mainshape = new Polygon2D(vertexlist);
            mainship = new RigidBody(MassInertia.FromRectangle(60000, 30, 100), InitialShipState, mainshape);
            
        }
        public override void AddObjects()
        {
            world.AddICollidableBody(mainship);
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
        
        public override void UpdateKeyBoard(KeyboardState keys, float dt)
        {
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
                this.mainship.ApplyForce(new ForceInfo(new Vector2D(ShipForce, 0), Vector2DTransformType.ToWorldAngular));
            }
            if (keys[Key.DownArrow])
            {
                this.mainship.ApplyForce(new ForceInfo(new Vector2D(-ShipForce, 0), Vector2DTransformType.ToWorldAngular));
            }
            if (keys[Key.RightArrow])
            {
                this.mainship.ApplyTorque(ShipTorque);

            }
            else if (keys[Key.LeftArrow])
            {
                this.mainship.ApplyTorque(-ShipTorque);
            }
            else
            {
                float dv = ShipTorque * this.mainship.MassInertia.MomentofInertiaInv * dt;
                if ((float)Math.Abs(this.mainship.Current.Velocity.Angular) < .1f)
                {
                    this.mainship.Current.Velocity.Angular = 0;
                }
                else if (this.mainship.Current.Velocity.Angular > 0)//Physics2D.Physics.Tolerance)
                {

                    if (dv > this.mainship.Current.Velocity.Angular)
                    {

                        this.mainship.ApplyTorque(this.mainship.Current.Velocity.Angular * this.mainship.MassInertia.MomentofInertia);
                    }
                    else
                    {
                        this.mainship.ApplyTorque(-ShipTorque);
                    }
                }
                else if (this.mainship.Current.Velocity.Angular < 0)//-Physics2D.Physics.Tolerance)
                {
                    if (-dv < this.mainship.Current.Velocity.Angular)
                    {
                        this.mainship.ApplyTorque(this.mainship.Current.Velocity.Angular * this.mainship.MassInertia.MomentofInertia);
                    }
                    else
                    {
                        this.mainship.ApplyTorque(ShipTorque);
                    }
                }
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
                this.mainship.ApplyForce(new ForceInfo(new Vector2D(0, -ShipForce), Vector2DTransformType.ToWorldAngular));
            }
            if (keys[Key.D])
            {
                this.mainship.ApplyForce(new ForceInfo(new Vector2D(0, ShipForce), Vector2DTransformType.ToWorldAngular));
            }
            if (keys[Key.C])
            {
                float dv = 2 * ShipForce * this.mainship.MassInertia.MassInv * dt;
                Vector2D velocity = this.mainship.Current.Velocity.Linear;
                float velocityMag = velocity.Magnitude;
                if (velocityMag != 0)
                {
                    if (velocityMag < 10)
                    {
                        this.mainship.Current.Velocity.Linear = Vector2D.Zero;
                        this.mainship.Current.Acceleration.Linear = Vector2D.Zero;
                    }
                    else if (dv > velocityMag)
                    {
                        this.mainship.ApplyForce(new ForceInfo(velocity * (-this.mainship.MassInertia.Mass)));
                    }
                    else
                    {
                        Vector2D velocityNorm = velocity * (1 / velocityMag);
                        this.mainship.ApplyForce(new ForceInfo(velocityNorm * (-2 * ShipForce)));
                    }
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
            TimeSpan timesincefired = now.Subtract(lastFired);
            if (timesincefired.TotalMilliseconds > 1)
            {

                float radius = 10;
                float mass = ProjectilesMass;
                float lifetime = 10;
                float velocity = ProjectilesVelocity+2000;


                if (keys[Key.Space])
                {
                    lastFired = now;
                    float anglediff = (float)rand.NextDouble() * .2f - .1f;
                    Vector2D position = this.mainship.Current.Position.Linear + Vector2D.FromLengthAndAngle(this.mainship.BoundingRadius + radius + 10, mainship.Current.Position.Angular);
                    RigidBody projectile = new RigidBody(MassInertia.FromSolidCylinder(mass, radius), this.mainship.Current.Position.Angular, position, new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 4, radius)));
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
                if (keys[Key.LeftControl])
                {
                    lastFired = now;

                    float ro = (float)((float)rand.NextDouble() + .5f);
                    Vector2D positionl = this.mainship.Current.Position.Linear + Vector2D.FromLengthAndAngle(this.mainship.BoundingRadius + radius, mainship.Current.Position.Angular - ro);//.LeftHandNormal;
                    Vector2D positionr = this.mainship.Current.Position.Linear + Vector2D.FromLengthAndAngle(this.mainship.BoundingRadius + radius, mainship.Current.Position.Angular + ro);//.RightHandNormal;
                    RigidBody projectilel = new RigidBody(MassInertia.FromSolidCylinder(mass * .5f, radius * 2), this.mainship.Current.Position.Angular - ro, positionl, new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 3, radius)));
                    RigidBody projectiler = new RigidBody(MassInertia.FromSolidCylinder(mass * .5f, radius * 2), this.mainship.Current.Position.Angular + ro, positionr, new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 3, radius)));
                    Vector2D RLV = Vector2D.FromLengthAndAngle(velocity, mainship.Current.Position.Angular - ro).RightHandNormal;
                    projectiler.Current.Velocity.Linear = mainship.Current.Velocity.Linear + RLV;
                    Vector2D LLV = Vector2D.FromLengthAndAngle(velocity, mainship.Current.Position.Angular + ro).LeftHandNormal;
                    projectilel.Current.Velocity.Linear = mainship.Current.Velocity.Linear + LLV;
                    if (keys[Key.LeftShift])
                    {
                        projectilel.CollisionState.GenerateCollisionEvents = true;
                        projectiler.CollisionState.GenerateCollisionEvents = true;
                    }
                    projectilel.LifeTime = new Mortal(lifetime);
                    projectiler.LifeTime = new Mortal(lifetime);
                    world.AddICollidableBody(projectilel);
                    world.AddICollidableBody(projectiler);

                    if (!keys[Key.C])
                    {
                        mainship.Current.Velocity.Linear -= LLV * (mass / mainship.MassInertia.Mass);
                        mainship.Current.Velocity.Linear -= RLV * (mass / mainship.MassInertia.Mass);
                    }
                }
                if (keys[Key.H])
                {
                    lastFired = now + TimeSpan.FromMilliseconds(600);
                    float anglediff = (float)rand.NextDouble() * .2f - .1f;
                    Vector2D position = this.mainship.Current.Position.Linear + Vector2D.FromLengthAndAngle(this.mainship.BoundingRadius + radius + 300, mainship.Current.Position.Angular);
                    //RigidBody projectile = new RigidBody(MassInertia.FromSolidCylinder(mass, radius), this.mainship.Current.Position.Angular, position, new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 4, radius)));
                    Vector2D LV = Vector2D.FromLengthAndAngle(ProjectilesVelocity, mainship.Current.Position.Angular + anglediff);
                    LV = mainship.Current.Velocity.Linear + LV;



                    OldDirectXTests.SetHumanRigidBodyV2(world, position, LV, new Mortal(10), 90,
                    float.PositiveInfinity,
                    9000000,
                    .5f);

                }
                if (keys[Key.X])
                {
                    bool fired = false;
                    foreach (ICollidableBody collidable in lightningcollidables)
                    {
                        if (!collidable.CollidableParts[0].UseCircleCollision)
                        {
                            //float distance = (collidable.Current.Position.Linear - mainship.Current.Position.Linear).Magnitude;
                            //Vector2D direction = ((collidable.Current.Velocity.Linear - mainship.Current.Velocity.Linear) * (distance / velocity) + collidable.Current.Position.Linear - mainship.Current.Position.Linear).Normalized;
                            Vector2D direction = (collidable.Current.Position.Linear - mainship.Current.Position.Linear).Normalized;
                            Vector2D position = (this.mainship.BoundingRadius + radius + 50) * direction;
                            PhysicsState state = new PhysicsState(mainship.Current);
                            state.Position.Linear += position;
                            state.Velocity.Linear += velocity * direction;
                            RigidBody projectile = new RigidBody(MassInertia.FromSolidCylinder(mass, radius), state, new Circle2D(radius));
                            projectile.LifeTime = new Mortal(lifetime);
                            if (keys[Key.LeftShift])
                            {
                                projectile.CollisionState.GenerateCollisionEvents = true;
                            }
                            world.AddICollidableBody(projectile);
                            if (!keys[Key.C])
                            {
                                //mainship.Current.Velocity.Linear -= state.Velocity.Linear * (mass / mainship.MassInertia.Mass);
                            }
                            fired = true;
                        }
                    }
                    if (fired)
                    {
                        lastFired = now;
                    }
                }
            }
            if (keys[Key.E])
            {


                Vector2D direction = this.mainship.Matrix.NormalMatrix*Vector2D.XAxis;
                Vector2D origin = this.mainship.Current.Position.Linear + direction * (this.mainship.BoundingRadius + 30);
                RaySegment2D forwardSegment = new RaySegment2D(origin, direction, forwardlenght);
                Ray2DEffectGroup group = new Ray2DEffectGroup();

                group.Add(new AttachedImpulseRay(new SingleStep(), forwardSegment, forwardimpulse, this.mainship));

                for (float angle = forwarddeltaAngle; angle <= forwardendAngle; angle += forwarddeltaAngle)
                {
                    Matrix2x2 matrix = Matrix2x2.FromRotation(angle);
                    //Rotation2D subRotation = new Rotation2D(angle);
                    RaySegment2D leftSegment = new RaySegment2D(origin, Vector2D.Rotate(matrix, direction), forwardlenght);
                    RaySegment2D rightSegment = new RaySegment2D(origin, Vector2D.Rotate(matrix.Transpose, direction), forwardlenght);
                    group.Add(new AttachedImpulseRay(new SingleStep(), leftSegment, forwardimpulse, this.mainship));
                    group.Add(new AttachedImpulseRay(new SingleStep(), rightSegment, forwardimpulse, this.mainship));
                }
                world.AddICollidableArea(group);
            }
            if (keys[Key.Z])
            {

/*
                Vector2D direction = Vector2D.Rotate(Vector2D.XAxis, this.mainship.Rotation);
                Vector2D origin = this.mainship.Current.Position.Linear + direction * (this.mainship.BoundingRadius + 50);
                Vector2D end = origin + direction * lightninglenght ;

                world.AddICollidableArea(new LightningRayGroup(origin, end + (((float)rand.NextDouble() - .5f) * lightninglenght) * direction.LeftHandNormal, lightningdisplace, lightningdetail, this.mainship, lightningimpulse, new SingleStep()));
                world.AddICollidableArea(new LightningRayGroup(origin, end + (((float)rand.NextDouble() - .5f) * lightninglenght) * direction.LeftHandNormal, lightningdisplace, lightningdetail, this.mainship, lightningimpulse, new SingleStep()));
                world.AddICollidableArea(new LightningRayGroup(origin, end + (((float)rand.NextDouble() - .5f) * lightninglenght) * direction.LeftHandNormal, lightningdisplace, lightningdetail, this.mainship, lightningimpulse, new SingleStep()));
                world.AddICollidableArea(new LightningRayGroup(origin, end + (((float)rand.NextDouble() - .5f) * lightninglenght) * direction.LeftHandNormal, lightningdisplace, lightningdetail, this.mainship, lightningimpulse, new SingleStep()));
                world.AddICollidableArea(new LightningRayGroup(origin, end + (((float)rand.NextDouble() - .5f) * lightninglenght) * direction.LeftHandNormal, lightningdisplace, lightningdetail, this.mainship, lightningimpulse, new SingleStep()));
                world.AddICollidableArea(new LightningRayGroup(origin, end + (((float)rand.NextDouble() - .5f) * lightninglenght) * direction.LeftHandNormal, lightningdisplace, lightningdetail, this.mainship, lightningimpulse, new SingleStep()));
                world.AddICollidableArea(new LightningRayGroup(origin, end + (((float)rand.NextDouble() - .5f) * lightninglenght) * direction.LeftHandNormal, lightningdisplace, lightningdetail, this.mainship, lightningimpulse, new SingleStep()));
               */
                foreach (ICollidableBody collidable in lightningcollidables)
                {
                    world.AddICollidableArea(new LightningRayGroup(mainship.Current.Position.Linear, collidable.Current.Position.Linear, lightningdisplace, lightningdetail, this.mainship, lightningimpulse, new SingleStep()));
                    

                }
            }
            if (keys[Key.R])
            {

                Vector2D direction = this.mainship.Matrix.NormalMatrix*Vector2D.XAxis;
                float distance = this.mainship.BoundingRadius + 5;
                Ray2DEffectGroup group = new Ray2DEffectGroup();
                for (float angle = 0; angle <= (float)Math.PI * 2; angle += omniDirectionaldeltaAngle)
                {
                    //Rotation2D rotation2 = new Rotation2D(angle);
                    Matrix2x2 matrix = Matrix2x2.FromRotation(angle);
                    Vector2D direction2 = Vector2D.Rotate(matrix, direction);
                    Vector2D origin = this.mainship.Current.Position.Linear + direction2 * distance;
                    group.Add(new AttachedImpulseRay(new SingleStep(), new RaySegment2D(origin, direction2, omniDirectionallenght), omniDirectionalimpulse, this.mainship));
                }
                world.AddICollidableArea(group);
            }

            TimeSpan timesincefiredSecondary = now.Subtract(lastFiredSecondary);
            if (timesincefiredSecondary.TotalMilliseconds > 4000)
            {
                if (keys[Key.G])
                {
                    lastFiredSecondary = now;
                    world.CalcGravity = true;
                    float wellmass = 1e24f;
                    float wellradius = 20;


                    float tmp = (float)((float)Math.PI - (float)Math.PI / 8);
                    float tmp2 = (float)((float)Math.PI + (float)Math.PI / 8);
                    float tmp3 = (float)((float)Math.PI * 2 - (float)Math.PI / 8);
                    for (float rad = 0; rad < (float)Math.PI * 2; rad += (float)((float)Math.PI / 16))
                    {
                        if (rad < (float)Math.PI / 8 || rad > tmp && rad < tmp2 || rad > tmp3)
                        {
                            Vector2D wellposition = Vector2D.FromLengthAndAngle(this.mainship.BoundingRadius + wellradius + 120, mainship.Current.Position.Angular + rad);
                            RigidBody gravityWell = new RigidBody(MassInertia.FromSolidCylinder(wellmass, wellradius), 0, this.mainship.Current.Position.Linear + wellposition, new Circle2D(wellradius));
                            gravityWell.Current.Velocity.Linear = mainship.Current.Velocity.Linear + Vector2D.FromLengthAndAngle(2000, mainship.Current.Position.Angular + rad);
                            gravityWell.Flags = BodyFlags.IgnoreGravity | BodyFlags.InfiniteMass;
                            gravityWell.LifeTime = new Mortal(2);
                            world.AddICollidableBody(gravityWell);
                            world.AddIGravitySource(gravityWell);
                        }
                    }
                }
            }
        }
        public override IDemo CreateNew()
        {
            return new BaseControlDemo();
        }
    }
    public class ShipBubble : BaseCollidable, ICollidableArea
    {
        ICollidableBody ship;
        float range;
        List<ICollidableBody> collidables;
        public ShipBubble(float range, ICollidableBody ship, List<ICollidableBody> collidables)
            : base(new ChildLifeTime(ship.LifeTime))
        {
            this.ship = ship;
            this.range = range;
            this.collidables = collidables;
        }
        #region ICollidableArea Members
        public override void CalcBoundingBox2D()
        {
            Vector2D position1 = ship.Current.Position.Linear;
            Vector2D position2 = position1;
            position1.X += range;
            position1.Y += range;
            position2.X -= range;
            position2.Y -= range;
            this.boundingBox2D =  new BoundingBox2D(position1, position2);
        }
        public void HandlePossibleIntersections(float dt, List<ICollidableBody> pairs)
        {
            pairs.Remove(ship);
            collidables.Clear();
            foreach (ICollidableBody c in pairs)
            {
                if (!float.IsNaN(c.Current.Position.Linear.X))
                {
                    collidables.Add(c);
                }
            }
        }



        #endregion
    }
}
