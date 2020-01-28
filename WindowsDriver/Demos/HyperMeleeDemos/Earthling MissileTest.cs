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
    public class EarthlingMissileTest : BaseControlDemo
    {
        private static void world_Collision(object sender, CollisionEventArgs args)
        {
            foreach (Physics2D.InterferenceInfo ginfo in args.InterferenceInfos)
            {
                World2D world = sender as World2D;
                if (ginfo.InterferenceType == InterferenceType.RayCollidable)
                {
                    RayCollidableInterferenceInfo info = ginfo.RayCollidableInfo;
                    PhysicsState bob = new PhysicsState(info.After);// new PhysicsState(collidables.Collidable1.Current);
                    if (info.Collidable.LifeTime is Mortal)
                    {
                        if (!(info.Collidable is ImpulseWave))
                        {
                            if (!info.Collidable.LifeTime.IsExpired)
                            {
                                world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.Collidable.MassInertia.Mass * 100, bob, info.Collidable.BoundingRadius, 850));
                                info.Collidable.LifeTime.IsExpired = true;
                            }
                        }
                    }
                }
                else
                {
                    CollidablePairInterferenceInfo info = ginfo.CollidablePairInfo;

                    if (!info.AtContactStage && info.Step == 0)
                    {

                        //if(collidables.Collidable1.Geometry.UseCircleCollision||collidables.Collidable2.Geometry.UseCircleCollision)
                        //{
                        if (!(info.ICollidableBody1 is ImpulseWave) || !(info.ICollidableBody2 is ImpulseWave))
                        {
                            //if()
                            //{
                            
                            PhysicsState bob;
                            if (info.ICollidableBody1.CollisionState.GenerateCollisionEvents)
                            {
                                bob = new PhysicsState(info.ICollidableBody1After);// new PhysicsState(collidables.Collidable1.Current);
                            }
                            else
                            {
                                bob = new PhysicsState(info.ICollidableBody2After);// new PhysicsState(collidables.Collidable2.Current);
                            }
                            //float mass1 = info.Collidable1.MassInertia.Mass;
                            //float mass2 = info.Collidable2.MassInertia.Mass;
                            //float totalmass = mass1 + mass2;
                            //rand.Velocity.Linear = info.ICollidableBody1Before.Velocity.Linear * (mass1 / totalmass) + info.ICollidableBody2Before.Velocity.Linear * (mass2 / totalmass);
                            //rand.Position.Linear += collidables.Distance*.5f;
                            if (info.ICollidableBody1.LifeTime is Mortal)
                            {
                                if (!(info.ICollidableBody1 is ImpulseWave))
                                {
                                    if (!info.ICollidableBody1.LifeTime.IsExpired)
                                    {
                                        world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.ICollidableBody1.MassInertia.Mass * 100, bob, info.ICollidableBody1.BoundingRadius, 850));
                                        info.ICollidableBody1.LifeTime.IsExpired = true;
                                    }
                                }
                            }
                            if (info.ICollidableBody2.LifeTime is Mortal)
                            {
                                if (!(info.ICollidableBody2 is ImpulseWave))
                                {
                                    if (!info.ICollidableBody2.LifeTime.IsExpired)
                                    {
                                        world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.ICollidableBody2.MassInertia.Mass * 100, bob, info.ICollidableBody2.BoundingRadius, 850));
                                        info.ICollidableBody2.LifeTime.IsExpired = true;
                                    }
                                }
                            }
                            //}
                        }
                    }
                }
            }
        }
        #region newer stuff
        public EarthlingMissileTest()
            : base()
        {

        }
        public override void InitObjects()
        {
            this.scale = .1f;

            //missilemass = MassInertia.FromRectangle(400, 40, 200);
            base.InitObjects();
        }
        static EarthlingMissileTest()
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
            missleBody = new Polygon2D(misslevt);
            missleradius = missleBody.BoundingRadius;
            //missilepart = new ColoredRigidBodyPart(ALVector2D.Zero, missleBody, new Coefficients(0.8f, 0.4f, 0.1f), new int[] { Color.White.ToArgb(), Color.White.ToArgb(), Color.White.ToArgb(), Color.Red.ToArgb(), Color.Red.ToArgb() }, 0);
            //missilemass = MassInertia.FromRectangle(400, 20, 100);
            int[] tmp = new int[] { Color.White.ToArgb(), Color.White.ToArgb(), Color.White.ToArgb(), Color.Red.ToArgb(), Color.Red.ToArgb() };
            missletemplate = new RigidBodyTemplate(400, 800, new IGeometry2D[] { missleBody }, new Coefficients[] { new Coefficients(0.8f, 0.4f, 0.1f) }) ;
            missletemplate.BalanceBody();
            missletemplate.CalcInertiaMultiplier(.1f);

        }

        static RigidBodyTemplate missletemplate;
        static Polygon2D missleBody;
        //static ColoredRigidBodyPart missilepart;
        //static MassInertia missilemass;
        static Random rand = new Random();
        static float missleradius;
        int count = 20;
        RigidBody[] missiles;
        RigidBody[] ships;
        Missile[] missileAI;
        Missile[] shipAI;
        public override void AddObjects()
        {
            ships = new RigidBody[count];
            shipAI = new Missile[count];
            missiles = new RigidBody[count];
            missileAI = new Missile[count];
            this.mainship = OldDirectXTests.CreateUrQuan(world);
            for (int pos = 0; pos != count; ++pos)
            {
                ships[pos] = OldDirectXTests.CreateEarthling(world);
                ships[pos].Current.Position.Linear = new Vector2D(rand.Next(-4000, 4000), rand.Next(-4000, 4000)) + mainship.Current.Position.Linear;
                world.AddICollidableBody(ships[pos]);
            }
            for (int pos = 0; pos != count; ++pos)
            {
                missiles[pos] = NewMissileBody(pos);
                world.AddICollidableBody(missiles[pos]);
            }

            //OldDirectXTests.SetEarthMoon(world);
            //OldDirectXTests.SetEvents(world);
            //this.InitialShipState = OldDirectXTests.staticInitialShipState;

            base.AddObjects();
            for (int pos = 0; pos != count; ++pos)
            {
                missileAI[pos] = NewMissileAI(this.mainship, missiles[pos]);
            }
            for (int pos = 0; pos != count; ++pos)
            {
                shipAI[pos] = NewShipAI(this.mainship, ships[pos]);
            }
            world.Collision += new CollisionEventDelegate(world_Collision);
        }
        private RigidBody NewMissileBody(int pos)
        {
            PhysicsState state = new PhysicsState(ships[pos].Current);
            state.Position.Linear += ships[pos].Vector2DTransform(new Vector2D(ships[pos].BoundingRadius + .5f * missleradius + 50, 0), Vector2DTransformType.ToWorldAngular);
            state.Velocity.Linear = ships[pos].GetVelocityAtWorld(state.Position.Linear);
            state.Velocity.Linear += ships[pos].Vector2DTransform(new Vector2D(200, 0), Vector2DTransformType.ToWorldAngular);
            //RigidBody possibleCollisions = new RigidBody(new ICollidableBodyPart[] { (ICollidableBodyPart)missilepart.Clone() }, missilemass, state, new Mortal(500), BodyFlags.None);
            RigidBody returnvalue = new RigidBody(new Mortal(15), state, BodyFlags.None, missletemplate);
            //RigidBody returnvalue = new RigidBody(missleBody, missilemass, state);
            //returnvalue.LifeTime = new Mortal(15);
            returnvalue.CollisionState.GenerateRayEvents = true;
            
            /*if (((float)rand.NextDouble() - .5f) > 0)
            {
                //possibleCollisions.CollisionState.GenerateCollisionEvents = true;
            }*/
            return returnvalue;
        }
        private Missile NewMissileAI(RigidBody target, RigidBody body)
        {
            return new Missile(target, body, 9000000, 1, 6200000, 1200);
        }
        private Missile NewShipAI(RigidBody target, RigidBody body)
        {
            return new Missile(target, body, 900000000, .9f, 90000000, 10);
        }
        public override void UpdateAI(float dt)
        {
            for (int pos = 0; pos != count; ++pos)
            {
                if (missiles[pos].LifeTime.IsExpired)
                {
                    missiles[pos] = NewMissileBody(pos);
                    world.AddICollidableBody(missiles[pos]);
                    missileAI[pos] = NewMissileAI(this.mainship, missiles[pos]);
                }
                if (missiles[pos].LifeTime.TimeLeft < 14)
                {
                    if ((float)rand.NextDouble() > .95)
                    {
                        missiles[pos].CollisionState.GenerateCollisionEvents = true;
                    }
                }
            }
            foreach (Missile miss in missileAI)
            {
                miss.Update(dt);
            }
            foreach (Missile miss in shipAI)
            {
                miss.Update(dt);
            }
        }
        public override bool Update(float dt)
        {
            return base.Update(dt);
        }
        public override string Name
        {
            get
            {
                return "Earthling Missile Test";
            }
        }
        public override string Description
        {
            get
            {
                return "has a bunch or Earthling Cruisers being the origin of explosive missiles.";
            }
        }
        public override IDemo CreateNew()
        {
            return new EarthlingMissileTest();
        }

        #endregion
    }
}
