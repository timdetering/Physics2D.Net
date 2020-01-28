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
using Physics2D;
using Physics2D.CollidableBodies;

using AdvanceMath.Geometry2D;
using AdvanceMath;
using Color = System.Drawing.Color;
namespace WindowsDriver.Demos.HyperMeleeDemo
{
    public class MissileTest : BaseControlDemo
    {

        static int[] ExposionColors = new int[] { Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Yellow.ToArgb() };
        static int ExplosionPrimaryColor = Color.Orange.ToArgb();
        private static void world_Collision(object sender, CollisionEventArgs args)
        {
            foreach (Physics2D.InterferenceInfo ginfo in args.InterferenceInfos)
            {
                if (ginfo.InterferenceType != InterferenceType.CollidablePair)
                {
                    continue;
                }
                CollidablePairInterferenceInfo info = ginfo.CollidablePairInfo;

                if (!info.AtContactStage && info.Step == 0)
                {

                    //if(collidables.Collidable1.Geometry.UseCircleCollision||collidables.Collidable2.Geometry.UseCircleCollision)
                    //{
                    if (!(info.ICollidableBody1 is ImpulseWave) || !(info.ICollidableBody2 is ImpulseWave))
                    {
                        //if()
                        //{
                        World2D world = sender as World2D;
                        PhysicsState bob;
                        if (info.ICollidableBody1.CollisionState.GenerateCollisionEvents)
                        {
                            bob = new PhysicsState(info.ICollidableBody1Before);// new PhysicsState(collidables.Collidable1.Current);
                        }
                        else
                        {
                            bob = new PhysicsState(info.ICollidableBody2Before);// new PhysicsState(collidables.Collidable2.Current);
                        }
                        float mass1 = info.ICollidableBody1.MassInertia.Mass;
                        float mass2 = info.ICollidableBody2.MassInertia.Mass;
                        float totalmass = mass1 + mass2;
                        bob.Velocity.Linear = info.ICollidableBody1Before.Velocity.Linear * (mass1 / totalmass) + info.ICollidableBody2Before.Velocity.Linear * (mass2 / totalmass);
                        //rand.Position.Linear += collidables.Distance*.5;
                        if (info.ICollidableBody1.LifeTime is Mortal)
                        {
                            if (!(info.ICollidableBody1 is ImpulseWave))
                            {
                                world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.ICollidableBody1.MassInertia.Mass * 100, bob, info.ICollidableBody1.BoundingRadius, 950));
                                info.ICollidableBody1.LifeTime.IsExpired = true;
                            }
                        }
                        if (info.ICollidableBody2.LifeTime is Mortal)
                        {
                            if (!(info.ICollidableBody2 is ImpulseWave))
                            {
                                world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.ICollidableBody2.MassInertia.Mass * 100, bob, info.ICollidableBody2.BoundingRadius, 950));
                                info.ICollidableBody2.LifeTime.IsExpired = true;
                            }
                        }
                        //}
                    }
                }
            }
        }
        #region newer stuff
        public MissileTest()
            : base()
        {

        }
        public override void InitObjects()
        {
            Vector2D[] misslevt = new Vector2D[]{
                new Vector2D(40,-7),
                new Vector2D(50,0),
                new Vector2D(40,7),
                new Vector2D(-40,7),
                new Vector2D(-40,-7)};
            Vector2D[] misslevt2 = new Vector2D[]{
                new Vector2D(100,-20),
                new Vector2D(150,0),
                new Vector2D(100,20),
                new Vector2D(-100,20),
                new Vector2D(-100,-20)};
            missleBody = new Polygon2D(misslevt2);
            missilepart = new RigidBodyPart(ALVector2D.Zero, missleBody, new Coefficients(0.8f, 0.4f, 0.1f));
            //missilemass = MassInertia.FromRectangle(400, 20, 100);
            missilemass = MassInertia.FromRectangle(400, 40, 200);
            base.InitObjects();
        }
        Polygon2D missleBody;
        RigidBodyPart missilepart;
        MassInertia missilemass;
        Random rand = new Random();
        int count = 100;
        RigidBody[] bodies;
        Missile[] missiles;


        public override void AddObjects()
        {
            
            bodies = new RigidBody[count];
            missiles = new Missile[count];
            this.mainship = OldDirectXTests.CreateUserRigidBody(world);
            for (int pos = 0; pos != count; ++pos)
            {
                bodies[pos] = NewMissileBody();
                world.AddICollidableBody(bodies[pos]);
            }
            


            //OldDirectXTests.SetEarthMoon(world);
            //OldDirectXTests.SetEvents(world);
            //this.InitialShipState = OldDirectXTests.staticInitialShipState;
            
            base.AddObjects();
            for (int pos = 0; pos != count; ++pos)
            {
                missiles[pos] = NewMissile(this.mainship, bodies[pos]);
            }
            world.Collision +=new CollisionEventDelegate(world_Collision);
        }
        private RigidBody NewMissileBody()
        {
            PhysicsState state = new PhysicsState();
            state.Position.Linear = new Vector2D(rand.Next(-1000000, 1000000), rand.Next(-1000000, 1000000)) + mainship.Current.Position.Linear;
            //RigidBody possibleCollisions = new RigidBody(new ICollidableBodyPart[] { (ICollidableBodyPart)missilepart.Clone() }, missilemass, state, new Mortal(500), BodyFlags.None);
            RigidBody returnvalue = new RigidBody(missilemass, state, missleBody);
            returnvalue.LifeTime = new Mortal(500);
            if ((rand.NextDouble() - .5) > 0)
            {
                //possibleCollisions.CollisionState.GenerateCollisionEvents = true;
            }
            return returnvalue;
        }
        private Missile NewMissile(RigidBody target, RigidBody body)
        {
            return new Missile(target, body, 90000000, 9, 4000000, 4000);
        }

        public override bool Update(float dt)
        {
            for (int pos = 0; pos != count; ++pos)
            {
                if (bodies[pos].LifeTime.IsExpired)
                {
                    bodies[pos] = NewMissileBody();
                    world.AddICollidableBody(bodies[pos]);
                    missiles[pos] = NewMissile(this.mainship, bodies[pos]);
                }
                if (bodies[pos].LifeTime.TimeLeft < 495)
                {
                    bodies[pos].CollisionState.GenerateCollisionEvents = true;
                }
            }
            foreach (Missile miss in missiles)
            {
                miss.Update(dt);
            }
            return base.Update(dt);
        }

        public override string Name
        {
            get
            {
                return "Missile Test";
            }
        }
        public override string Description
        {
            get
            {
                return "Tests that missle class";
            }
        }
        public override IDemo CreateNew()
        {
            return new MissileTest();
        }

        #endregion
    }
}
