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
using Physics2D.Joints;
namespace WindowsDriver.Demos
{
    public class OldDirectXTests : BaseControlDemo
    {
        #region the old tests (The Settings file)
        public static Random rand = new Random();
        public static PhysicsState staticInitialShipState = new PhysicsState();
        public static Vector2D StartingLocation
        {
            get
            {
                return staticInitialShipState.Position.Linear;
            }
            set
            {
                staticInitialShipState.Position.Linear = value;
            }
        }
        public static Vector2D StartingVelocity
        {
            get
            {
                return staticInitialShipState.Velocity.Linear;
            }
            set
            {
                staticInitialShipState.Velocity.Linear = value;
            }
        }
        public static float StartingOrientation
        {
            get
            {
                return staticInitialShipState.Position.Angular;
            }
            set
            {
                staticInitialShipState.Position.Angular = value;
            }
        }
		/// <summary>
		/// This will return an object that can be controlled by the user.
		/// </summary>
		/// <param name="world">The world</param>
		/// <returns>The Object that the user can control with configured keys.</returns>
        public static RigidBody CreateUserRigidBody(Physics2D.World2D world)
		{
			Vector2D[] vertexlist = new Vector2D[6];
			vertexlist[0] = new Vector2D(50,30);
			vertexlist[1] = new Vector2D(-50,30);
			vertexlist[2] = new Vector2D(-50,-30);
			vertexlist[3] = new Vector2D(50,-30);
			vertexlist[4] = new Vector2D(60,-10);
			vertexlist[5] = new Vector2D(60,10);
			//Polygon2D mainshape = new Polygon2D(vertexlist);
			//Circle mainshape = new Circle(50);

           /* Polygon2D mainshape = new Polygon2D(
                new Vector2D[]{
                    new Vector2D(-20,10),
                    new Vector2D(-20,-10),
                    new Vector2D(-10,-30),
                    new Vector2D(0,-40),
                    new Vector2D(20,-40),
                    new Vector2D(20,40),
                    new Vector2D(0,40),
                    new Vector2D(-10,30),


                    }
            );*/
            
			//RigidBody mainship = new RigidBody(mainshape,MassInertia.FromRectangle(60000, 30,100),StartingOrientation,StartingLocation);
            //RigidBody mainship = CreateUrQuan(world);
			RigidBody mainship =CreateEarthling(world);
            mainship.Current.Position = new ALVector2D(StartingOrientation,StartingLocation);
            mainship.Initial.Position = new ALVector2D(StartingOrientation, StartingLocation);
            //circle//RigidBody mainship = new RigidBody(new Circle(100),MassInertia.FromSolidCylinder(6000, 100),StartingOrientation,StartingLocation);
			//mainship.Current.Velocity.Linear = Vector2D.SetMagnitude(centerofOrbit-mainship.Current.Position.Linear,(float)Math.Sqrt(2*(gravity.Magnitude*0.63661977236758134307553505349036)*(centerofOrbit-mainship.Current.Position.Linear).Magnitude)).RightHandNormal;
			mainship.Current.Velocity.Linear = StartingVelocity;
			//mainship.Flags = mainship.Flags|BodyFlags.IgnoreGravity;
			
			
			/*Vector2D RWingPos = StartingLocation+Vector2D.Rotate(new Vector2D(0,60),StartingOrientation);
			RigidBody RWing = new RigidBody(new Polygon2D(3,50),MassInertia.FromSquare(500,35),((float)Math.PI/2),RWingPos);
			Vector2D RWaPos = StartingLocation+Vector2D.Rotate(new Vector2D(-30,60),StartingOrientation);
			PinJoint RWa = new PinJoint(new CollidablePair(mainship,RWing),RWaPos,0,.5f);
			Vector2D RWbPos = StartingLocation+Vector2D.Rotate(new Vector2D(30,60),StartingOrientation);
			PinJoint RWb = new PinJoint(new CollidablePair(mainship,RWing),RWbPos,0,.5f);
			
			
			
			
			
			
			Vector2D LWingPos = StartingLocation+Vector2D.Rotate(new Vector2D(0,-60),StartingOrientation);
			RigidBody LWing = new RigidBody(new Polygon2D(3,50),MassInertia.FromSquare(500,35),(3*(float)Math.PI/2),LWingPos);
			Vector2D LWaPos = StartingLocation+Vector2D.Rotate(new Vector2D(-30,-60),StartingOrientation);
			PinJoint LWa = new PinJoint(new CollidablePair(mainship,LWing),LWaPos,0,.5f);
			Vector2D LWbPos = StartingLocation+Vector2D.Rotate(new Vector2D(30,-60),StartingOrientation);
			PinJoint LWb = new PinJoint(new CollidablePair(mainship,LWing),LWbPos,0,.5f);
			
			world.AddICollidableBody(RWing);
			world.AddJoint(RWa);
			world.AddJoint(RWb);
			

			world.AddICollidableBody(LWing);
			world.AddJoint(LWa);
			world.AddJoint(LWb);*/
			return mainship;
		}
        public static RigidBody CreateEarthling(Physics2D.World2D world)
		{
			Polygon2D pods =  new Polygon2D( Polygon2D.FromRectangle(20, 70));
			Polygon2D mainhullp = new Polygon2D( Polygon2D.FromRectangle(30, 150));
			Polygon2D subhullp =new Polygon2D(  Polygon2D.FromRectangle(40, 20));
			Coefficients bob = new Coefficients(.5f, .1f, .1f);
			RigidBodyPart pod1 = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(40, 40)), pods, bob);
			RigidBodyPart pod2 = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(40, -40)), pods, bob);
			RigidBodyPart pod3 = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(-40, 40)), pods, bob);
			RigidBodyPart pod4 = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(-40, -40)), pods, bob);
			
			RigidBodyPart bridge = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(120, 0)), new Circle2D(35), bob);//, (new object[] { Color.Red, Color.Red, Color.LightGreen, Color.DarkGreen, Color.DarkGreen, Color.DarkGreen, Color.DarkGreen, Color.LightGreen }), Color.DarkGreen);
			RigidBodyPart mainhull = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(20, 0)), mainhullp, bob);
			
			RigidBodyPart subhull1 = new RigidBodyPart(new ALVector2D((float)Math.PI+.7f, new Vector2D(-35, 22)), subhullp, bob);
			RigidBodyPart subhull2 = new RigidBodyPart(new ALVector2D((float)Math.PI+.7f, new Vector2D(35, 22)), subhullp, bob);
			RigidBodyPart subhull3 = new RigidBodyPart(new ALVector2D((float)Math.PI-.7f, new Vector2D(-35, -22)), subhullp, bob);
			RigidBodyPart subhull4 = new RigidBodyPart(new ALVector2D((float)Math.PI-.7f, new Vector2D(35, -22)), subhullp, bob);
			

            RigidBody mainship = new RigidBody(MassInertia.FromRectangle(60000, 250, 100), new PhysicsState(), new RigidBodyPart[] { subhull1, subhull2, subhull3, subhull4, pod1, pod2, pod3, pod4, mainhull, bridge });
			return mainship;
		}
        public static RigidBody CreateUrQuanOld(Physics2D.World2D world)
        {
            Polygon2D Pod1p = new Polygon2D(
                 new Vector2D[]{
                      new Vector2D(50,-15),
                      new Vector2D(80,15),
                      new Vector2D(-50,15),
                      new Vector2D(-50,-15)});
            Polygon2D Pod2p = new Polygon2D(
                  new Vector2D[]{
                      new Vector2D(-50,-15),
                      new Vector2D(80,-15),
                      new Vector2D(50,15),
                      new Vector2D(-50,15)});
            Polygon2D bridgep = new Polygon2D(
                new Vector2D[]{
                    new Vector2D(-35,20),
                    new Vector2D(-35,-20),
                    new Vector2D(-22,-44),
                    new Vector2D(-10,-50),
                    new Vector2D(20,-50),
                    new Vector2D(20,50),
                    new Vector2D(-10,50),
                    new Vector2D(-22,44)});
            Polygon2D mainhullp = new Polygon2D( Polygon2D.FromRectangle(50, 150));
            Polygon2D subhullp = new Polygon2D( Polygon2D.FromRectangle(100, 30));
            Coefficients bob = new Coefficients(.5f, .1f, .1f);
            RigidBodyPart pod1 = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(0, 50)), Pod1p, bob);//, new int[] { Color.DarkGreen.ToArgb(), Color.Red.ToArgb() }, Color.LightGreen.ToArgb());
            RigidBodyPart pod2 = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(0, -50)), Pod2p, bob);//, new int[] { Color.DarkGreen.ToArgb(), Color.Red.ToArgb() }, Color.LightGreen.ToArgb());
            RigidBodyPart bridge = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(90, 0)), bridgep, bob);//, new int[] { Color.DarkGreen.ToArgb(), Color.SpringGreen.ToArgb() }, Color.LightGreen.ToArgb());//, (new object[] { Color.Red, Color.Red, Color.LightGreen, Color.DarkGreen, Color.DarkGreen, Color.DarkGreen, Color.DarkGreen, Color.LightGreen }), Color.DarkGreen);
            RigidBodyPart mainhull = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(0, 0)), mainhullp, bob);//, new int[] { Color.DarkGreen.ToArgb(), Color.SpringGreen.ToArgb() }, Color.LightGreen.ToArgb());
            RigidBodyPart subhull = new RigidBodyPart(new ALVector2D((float)Math.PI, new Vector2D(-10, 0)), subhullp, bob);//, new int[] { Color.DarkGreen.ToArgb(), Color.SpringGreen.ToArgb() }, Color.LightGreen.ToArgb());
            RigidBody mainship = new RigidBody(MassInertia.FromRectangle(60000, 250, 100), new PhysicsState(), new RigidBodyPart[] { subhull, pod1, pod2, mainhull, bridge });
            return mainship;
        }
        public static  RigidBody CreateUrQuan(Physics2D.World2D world)
        {
            Coefficients DefaultCoefficients = new Coefficients(.7f, .2f, .2f);
            Vector2D[] Pod1p = 
            new Vector2D[]{
                      new Vector2D(50,-15),
                      new Vector2D(80,15),
                      new Vector2D(-50,15),
                      new Vector2D(-50,-15)};
            Vector2D[] Pod2p = 
                  new Vector2D[]{
                      new Vector2D(-50,-15),
                      new Vector2D(80,-15),
                      new Vector2D(50,15),
                      new Vector2D(-50,15)};
            Vector2D[] bridgep = 
                new Vector2D[]{
                    new Vector2D(-35,20),
                    new Vector2D(-35,-20),
                    new Vector2D(-22,-44),
                    new Vector2D(-10,-50),
                    new Vector2D(20,-50),
                    new Vector2D(20,50),
                    new Vector2D(-10,50),
                    new Vector2D(-22,44)};


            Vector2D[] mainhullp = Polygon2D.FromRectangle(50, 150);
            Vector2D[] subhullp = Polygon2D.FromRectangle(100, 30);
            IGeometry2D[] geometry = new IGeometry2D[5];


            geometry[0] = new Polygon2D(new ALVector2D(MathAdv.PI, new Vector2D(-10, 0)), subhullp);
            geometry[1] = new Polygon2D(new ALVector2D(MathAdv.PI, new Vector2D(0, 50)), Pod1p);
            geometry[2] = new Polygon2D(new ALVector2D(MathAdv.PI, new Vector2D(0, -50)), Pod2p);
            geometry[3] = new Polygon2D(new ALVector2D(MathAdv.PI, new Vector2D(0, 0)), mainhullp);
            geometry[4] = new Polygon2D(new ALVector2D(MathAdv.PI, new Vector2D(90, 0)), bridgep);

            Coefficients[] coefficients = new Coefficients[5];
            for (int pos = 0; pos < 5; ++pos)
            {
                coefficients[pos] = DefaultCoefficients;
            }

            RigidBodyTemplate DefaultShape = new RigidBodyTemplate(60000, 3907.8407737525167f, geometry, coefficients);
            DefaultShape.BalanceBody();
            return new RigidBody(new Immortal(), new PhysicsState(), BodyFlags.None, DefaultShape);
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static void SetEvents(Physics2D.World2D world)
        {
            world.Collision += new CollisionEventDelegate(world_Collision);
        }
		/// <summary>
		/// This Method is called by the form to set the initial conditions of the simulation.
		/// It is full of commented out method calls the set up unique scenarios.
		/// </summary>
		/// <param name="world">A world that will be Configured.</param>
		public static  void Set(Physics2D.World2D world)
		{
			//SetTest(world);
			//SetWalls(world);
			//SetOneRectagle(world);
			//SetCircleTest(world);
			//SetTest6(world);
			//SetSpecialBalls(world);
			//Set2RectagleTest(world);
		    //Set2SquareTest(world);
			//Set2RotatingRectagleTest(world);
			//SetCircleRectagleTest(world);
			//SetCircleSquareTest(world);
            MassNumberOfRectTest(world);
			//MassNumberOfSquaresTest(world);
			//MassNumberOfCirclesTest(world);
			//MassNumberOfSquaresAndCirclesTest(world);
			//MassNumberOfPolygon2DsTest(world);
			//MassNumberOfBodiesTest(world);
			//SetTriangleTest(world);
			//SetPolygon2DTest(world);
			//SetSuperPolygon2DTest(world);
			
			//SetCage(world);
			//SetPlanet(world);
			SetBoxCage(world);
			//SetSol(world);
			//Set3SquareTest(world);
			//SetEarthMoon(world);
			//SetRealisticSol(world);
			
			//SetForceTest(world);
			//SetPinJointTest(world);//
			//SetILTest(world);
			

			//::JointTests
			//SetJim(world);
			//SetMom(world);
			//SetHumanRigidBody(world,new Vector2D(0,0));
			//SetHumanRigidBody(world,new Vector2D(0,0));
			
			//MassNumberOfMTest(world);
			//SetFrictionTest(world);
			//SetChain(world);

			//ImpulseWaveTest(world);
			//TrackedAnchorTest(world);

			//::cool tests
			//SetPoolTable(world);

			//::Heavy Test
			//MassNumberTestWithRestPlace(world);
			//SetVerySpecialBallTest(world);
			//GravityFieldTest(world);
			
			//SetHumanRigidBody(world,new Vector2D(700,-500));
			//MassNumberOfHumanBodiesTest(world);
			
			//::NewerTests
			//EdgeOnEdgeTest(world);
			//EdgeOnEdgeTestOffset(world);
			//EdgeOnEdgeTestRotating(world);
			//EdgeOnEdgeTestSliding(world);
			//VertexOnEdgeTestSliding(world);
			//VertexOnEdgeTest(world);
			//VertexOnEdgeTestRolling(world);
			//VertexOnEdgeTestRotating(world);
			//VertexOnEdgeTestFinite(world);
			//SandwhichTest(world);
			//SandwhichTest2(world);
			//SquareRestPlace(world);
			//RotatingRestPlace(world);
			//Sandbox(world);
			//DominoesTest(world);
			//DominoesStackTest(world);
			//FlipTest(world);
			//StackTest(world);
			//AnchorTest(world);
            //SetMultiPart(world);
            //MassNumberOfUrQuanTest(world);
			//MassNumberOfEarthlingTest(world);
			//SetCircleUrQuanCage(world);
            //EarthlingTest(world);
            //MassNumberOfMinesTest(world);

            //BalancingStackTest(world);
		}
		public static  void EdgeOnEdgeTest(Physics2D.World2D world)
		{
			Polygon2D rec = new Polygon2D(Polygon2D.FromRectangle(100,500));
			MassInertia mi = MassInertia.FromRectangle(2000,100,500);
			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(200,400);
			st1.Velocity.Linear = new Vector2D(0,80);
			RigidBody b1 = new RigidBody(mi, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(200,600);
			st2.Velocity.Linear = new Vector2D(0,-80);
			RigidBody b2 = new RigidBody(mi, st2, rec);
			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
		}
		public static  void EdgeOnEdgeTestOffset(Physics2D.World2D world)
		{
			Polygon2D rec = new Polygon2D(Polygon2D.FromRectangle(100,800));
			MassInertia mi = MassInertia.FromRectangle(2000,100,500);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(200,200);
			st1.Velocity.Linear = new Vector2D(0,80);
			RigidBody b1 = new RigidBody(mi, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(600,400);
			st2.Velocity.Linear = new Vector2D(0,-80);
			RigidBody b2 = new RigidBody(mi, st2, rec);
			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
		}
		public static  void EdgeOnEdgeTestRotating(Physics2D.World2D world)
		{
			Polygon2D rec = new Polygon2D(Polygon2D.FromRectangle(100,500));
			MassInertia mi = MassInertia.FromRectangle(2000,100,500);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(200,200);
			st1.Velocity.Angular = .5f;
			RigidBody b1 = new RigidBody(mi, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(500,400);
			st2.Velocity.Angular = .5f;
			RigidBody b2 = new RigidBody(mi, st2, rec);
			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
		}

		public static  void EdgeOnEdgeTestSliding(Physics2D.World2D world)
		{
			Polygon2D rec = new Polygon2D(Polygon2D.FromRectangle(100,900));
			MassInertia mi = MassInertia.FromRectangle(2000,100,900);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(200,250);
			st1.Velocity.Linear = new Vector2D(50,0);
			RigidBody b1 = new RigidBody(mi, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(500,350);
			st2.Velocity.Linear = new Vector2D(-50,0);
			RigidBody b2 = new RigidBody(mi, st2, rec);
			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
		}

		
		public static  void VertexOnEdgeTestSliding(Physics2D.World2D world)
		{
			Polygon2D rec = new Polygon2D(Polygon2D.FromRectangle(100,900));
			Polygon2D tri = new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 3,100));
			MassInertia mi1 = MassInertia.FromRectangle(2000,100,900);
			MassInertia mi2 = MassInertia.FromSolidCylinder(100,75);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(200,250);
			st1.Velocity.Linear = new Vector2D(50,0);
			RigidBody b1 = new RigidBody(mi1, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(500,410);
			st2.Position.Angular = .1f;
			st2.Velocity.Linear = new Vector2D(-90,-9);
			RigidBody b2 = new RigidBody(mi2, st2, tri);
			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
		}
		public static  void VertexOnEdgeTest(Physics2D.World2D world)
		{
			Polygon2D rec = new  Polygon2D(Polygon2D.FromRectangle(100,900));
			Polygon2D tri = new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 3,20));
			MassInertia mi1 = MassInertia.FromRectangle(2000,100,900);
			MassInertia mi2 = MassInertia.FromSolidCylinder(2,10);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(580,270);
			st1.Position.Angular = 4.5f;
			st1.Velocity.Linear = new Vector2D(-50,-40);
			//st1.Velocity.Angular = 1.5f;

			RigidBody b1 = new RigidBody(mi1, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(500,410);
			st2.Position.Angular = 2.0f;
			st2.Velocity.Linear = new Vector2D(2,-100);
			RigidBody b2 = new RigidBody(mi2, st2, tri);
			world.AddICollidableBody(b2);
			world.AddICollidableBody(b1);
		}
		public static  void VertexOnEdgeTestRolling(Physics2D.World2D world)
		{
			Polygon2D rec = new  Polygon2D(Polygon2D.FromRectangle(100,900));
			Polygon2D tri = new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 10,100));
			MassInertia mi1 = MassInertia.FromRectangle(2000,100,900);
			MassInertia mi2 = MassInertia.FromSolidCylinder(100,75);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(200,250);
			st1.Velocity.Linear = new Vector2D(50,0);
			RigidBody b1 = new RigidBody(mi1, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(500,410);
			st2.Position.Angular = .6f;
			st2.Velocity.Linear = new Vector2D(-50,-7);
			RigidBody b2 = new RigidBody(mi2, st2, tri);
			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
		}
		public static  void VertexOnEdgeTestRotating(Physics2D.World2D world)
		{
			Polygon2D rec = new  Polygon2D(Polygon2D.FromRectangle(100,900));
			Polygon2D tri = new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 5,100));
			MassInertia mi1 = MassInertia.FromRectangle(2000,100,900);
			MassInertia mi2 = MassInertia.FromSolidCylinder(100,75);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(510,450);
			st1.Position.Angular = .1f;

			st1.Velocity.Linear = new Vector2D(50,0);
			st1.Velocity.Angular = .1f;
			RigidBody b1 = new RigidBody(mi1, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(200,240);
			st2.Position.Angular = .1f;
			st2.Velocity.Linear = new Vector2D(-50,53);
			st2.Velocity.Angular = 10;
			RigidBody b2 = new RigidBody(mi2, st2, tri);
			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
		}
		
		public static  void VertexOnEdgeTestFinite(Physics2D.World2D world)
		{
			Polygon2D rec = new  Polygon2D(Polygon2D.FromRectangle(100,900));
			//Polygon2D tri = new Polygon2D(10,100);
			MassInertia mi = MassInertia.FromRectangle(2000,100,900);
			//MassInertia mi2 = MassInertia.FromSolidCylinder(100,75);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(500,250);
			st1.Velocity.Linear = new Vector2D(0,0);
			RigidBody b1 = new RigidBody(mi, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(200,580);
			st2.Position.Angular = .01f;
			st2.Velocity.Linear = new Vector2D(50,-50);
			//st2.Velocity.Angular = 10;
			RigidBody b2 = new RigidBody(mi, st2, rec);
			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
		}
		
		public static  void SandwhichTest(Physics2D.World2D world)
		{
			Polygon2D rec = new  Polygon2D(Polygon2D.FromRectangle(100,900));
			//Polygon2D tri = new Polygon2D(10,100);
			MassInertia mi = MassInertia.FromRectangle(2000,100,900);
			//MassInertia mi2 = MassInertia.FromSolidCylinder(100,75);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(200,200);
			st1.Velocity.Linear = new Vector2D(0,50);
			RigidBody b1 = new RigidBody(mi, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(200,400);
			st2.Velocity.Linear = new Vector2D(0,0);

			RigidBody b2 = new RigidBody(mi, st2, rec);
			PhysicsState st3 = new PhysicsState();
			st3.Position.Linear = new Vector2D(200,600);
			st3.Velocity.Linear = new Vector2D(0,-50);
			RigidBody b3 = new RigidBody(mi, st3, rec);

			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
			world.AddICollidableBody(b3);
		}
		public static  void SandwhichTest2(Physics2D.World2D world)
		{
			Polygon2D rec = new  Polygon2D(Polygon2D.FromRectangle(100,900));
			//Polygon2D tri = new Polygon2D(10,100);
			MassInertia mi = MassInertia.FromRectangle(2000,100,900);
			//MassInertia mi2 = MassInertia.FromSolidCylinder(100,75);

			PhysicsState st1 = new PhysicsState();
			st1.Position.Linear = new Vector2D(200,200);
			st1.Velocity.Linear = new Vector2D(0,50);
			RigidBody b1 = new RigidBody(mi, st1, rec);
			PhysicsState st2 = new PhysicsState();
			st2.Position.Linear = new Vector2D(200,400);
			st2.Velocity.Linear = new Vector2D(0,20);

			RigidBody b2 = new RigidBody(mi, st2, rec);
			PhysicsState st3 = new PhysicsState();
			st3.Position.Linear = new Vector2D(200,600);
			st3.Velocity.Linear = new Vector2D(0,-50);
			RigidBody b3 = new RigidBody(mi, st3, rec);

			world.AddICollidableBody(b1);
			world.AddICollidableBody(b2);
			world.AddICollidableBody(b3);
		}
		public static  void MassNumberTestWithRestPlace(Physics2D.World2D world)
		{
			//world.Collision +=new CollisionEventDelegate(world_Collision);
			StartingLocation = new Vector2D(1200,1200);
			RigidBody rest = new RigidBody(MassInertia.FromSolidCylinder(9.9891e23f,9900), 0, new Vector2D(0,0), new Circle2D(800));
			//rest.Current.Velocity.Angular =	.5f;
			world.AddICollidableBody(rest);
			world.AddIGravitySource(rest);
			world.CalcGravity = true;
			MassInertia boxmi = MassInertia.FromRectangle(2000,200,100);
			for(int pos1 = 860;pos1 <= 1400;pos1+=150)
			{
                RigidBody body = new RigidBody(boxmi, 0, new Vector2D(pos1, 0), (Polygon2D)Polygon2D.FromRectangle(200, 100));
				world.AddICollidableBody(body);
                RigidBody body2 = new RigidBody(boxmi, 0, new Vector2D(-pos1, 0), (Polygon2D)Polygon2D.FromRectangle(200, 100));
				world.AddICollidableBody(body2);
			}
			for(int pos1 = 850;pos1 <= 3900;pos1+=100)
			{
				for(int pos2 = 650;pos2 <= 1100;pos2+= 100)
				{
					float r = (float)((float)rand.NextDouble()*7);
					if(rand.Next(2)==0)
					{
						int s = rand.Next(4);
						RigidBody body = new RigidBody(MassInertia.FromSolidCylinder(100,40+r), 0, new Vector2D(pos1,pos2), new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 4+s,40+r)));
						//collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-collidable.Current.Position.Linear,(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-collidable.Current.Position.Linear).Magnitude)).RightHandNormal;
						world.AddICollidableBody(body);
					}
					else
					{
						RigidBody body = new RigidBody(MassInertia.FromSolidCylinder(100,40+r), 0, new Vector2D(pos1,pos2), new Circle2D(40+r));
						//collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(world.accel-collidable.Current.Position.Linear,(float)Math.Sqrt(2*(world.gravity*0.63661977236758134307553505349036)*(world.accel-collidable.Current.Position.Linear).Magnitude)).GetRightHandNormal();
						//collidable.collisionState.GenerateCollisionEvents = true;
						world.AddICollidableBody(body);
					}
				}
			}
												  
		}
		public static  void SquareRestPlace(Physics2D.World2D world)
		 {
             RigidBody rest = new RigidBody(MassInertia.FromSolidCylinder(1.9891e23f, 400), 0, new Vector2D(-2222, 0), (Polygon2D)Polygon2D.FromSquare(800));
			 rest.Current.Velocity.Angular = .7f;
			 world.AddICollidableBody(rest);
			 world.AddIGravitySource(rest);
			 world.CalcGravity = true;									  
		 }

		public static  void RotatingRestPlace(Physics2D.World2D world)
		{
			StartingLocation = new Vector2D(1200,1200);
			RigidBody rest = new RigidBody(MassInertia.FromSolidCylinder(9.9891e24f,9900), 0, new Vector2D(0,900), new Circle2D(800));
			rest.Current.Velocity.Angular =	1.4f;
			world.AddICollidableBody(rest);
			world.AddIGravitySource(rest);
			world.CalcGravity = true;							  
		}

		public static  void GravityFieldTest(Physics2D.World2D world)
		{
			StartingLocation = new Vector2D(0,-10200);
			StaticGravityField Field = new StaticGravityField(new Vector2D(0,800));
			world.AddIGravitySource(Field);
			world.CalcGravity = true;
            Polygon2D rect = (Polygon2D)Polygon2D.FromRectangle(500, 100000);
			MassInertia MI = new MassInertia(float.PositiveInfinity,float.PositiveInfinity);
			PhysicsState state = new PhysicsState();
			RigidBody platform;
			state.Position.Linear = new Vector2D(500,2000);
			state.Position.Angular = .9f;
			platform = new RigidBody(MI, state, rect);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity|BodyFlags.InfiniteMass;
			//world.AddICollidableBody(platform);
			
			Polygon2D rect2;


            float Width = 30000;
            float Length = 600;
            rect = (Polygon2D)Polygon2D.FromRectangle(Length, Width);
			MI = new MassInertia(float.PositiveInfinity,float.PositiveInfinity);
			state = new PhysicsState();
			//RigidBody platform;
			Vector2D positionOffset = new Vector2D(500,4000);
			state.Velocity.Linear = new Vector2D(0,0);
			
			state.Position.Linear = positionOffset;
			//state.Position.Angular = .01f; 
			platform = new RigidBody(MI, state, rect);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity|BodyFlags.InfiniteMass;
			
			world.AddICollidableBody(platform);
            float Width2 = 600;
            float Length2 = 12000;

            rect2 = (Polygon2D)Polygon2D.FromRectangle(Length2, Width2);
            state.Position.Linear = new Vector2D(positionOffset.X + Width / 2,  (positionOffset.Y + Length / 2) - Length2/2);
			
			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity|BodyFlags.InfiniteMass;
			
			world.AddICollidableBody(platform);

            state.Position.Linear = new Vector2D(positionOffset.X - Width / 2,  (positionOffset.Y + Length / 2) - Length2/2);

			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity|BodyFlags.InfiniteMass;
			
			world.AddICollidableBody(platform);




            rect2 = (Polygon2D)Polygon2D.FromRectangle(100, 200);
			MassInertia MI2 = MassInertia.FromRectangle(1000,100,200);
            rect = (Polygon2D)Polygon2D.FromRectangle(300, 1000);
			float range = 18000;
			float start = -9000;
			int num  = 20;
			float inc = range/num;
			for(int pos = 0;pos != num; ++pos)
			{
				state.Position.Linear = new Vector2D(inc*pos+start+rand.Next(-100,-100),rand.Next(-10000,-2000));
				state.Position.Angular = (float)rand.NextDouble()*1.5f-.75f;
				platform = new RigidBody(MI, state, rect);
				platform.Flags = platform.Flags|BodyFlags.IgnoreGravity|BodyFlags.InfiniteMass;
				
				world.AddICollidableBody(platform);
			}
			for(int pos = 0;pos < 70; ++pos)
			{
				PhysicsState state2 = new PhysicsState();
				state2.Position.Linear = new Vector2D(rand.Next(-6000,6000)+pos,rand.Next(-62030,-10000));
				RigidBody box = new RigidBody(MI2, state2, rect2);
				world.AddICollidableBody(box);
			}
			
			for(int pos = 0;pos < 50; ++pos)
			{
				PhysicsState state2 = new PhysicsState();
				state2.Position.Linear = new Vector2D(rand.Next(-6000,6000)+pos,rand.Next(-60000,-10000));
                //RigidBody box = new RigidBody(new Circle2D(100), MassInertia.FromSolidCylinder(1000, 100), state2);
                RigidBody box = new RigidBody(MassInertia.FromSolidCylinder(1000, 100), state2, new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 10,100)));
				world.AddICollidableBody(box);
			}
            for (int pos = 0; pos < 3; ++pos)
            {
                SetHumanRigidBodyV2(world, new Vector2D(rand.Next(-4000, 4000) + pos, rand.Next(-20000, -10000)),
                    90,
                    float.PositiveInfinity,
                    9000000,
                    .5f);
            }
            //world.ContactBroadPhaseOnlyOnce = true;
            //world.CollisionStepsCount = 2;
            //world.ContactStepsCount = 3;
		}
		public static  void StackTest(Physics2D.World2D world)
		{
			StartingLocation = new Vector2D(0,170);
			StaticGravityField Field = new StaticGravityField(new Vector2D(0,800));
			world.AddIGravitySource(Field);
			world.CalcGravity = true;
            Polygon2D rect = (Polygon2D)Polygon2D.FromRectangle(600, 8000);
			MassInertia MI = new MassInertia(float.PositiveInfinity,float.PositiveInfinity);
			PhysicsState state = new PhysicsState();
			RigidBody platform;
			
			
			state.Position.Linear = new Vector2D(500,500);
			platform = new RigidBody(MI, state, rect);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            float Width = 600;
            float Length = 1200;

            Polygon2D rect2 = (Polygon2D)Polygon2D.FromRectangle(Length, Width);
            state.Position.Linear = new Vector2D(500 + Width / 2, 500 - Length / 2);
			
			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			//world.AddICollidableBody(platform);

            state.Position.Linear = new Vector2D(500 - Width / 2, 500 - Length / 2);

			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			//world.AddICollidableBody(platform);


            Polygon2D sq = (Polygon2D)Polygon2D.FromSquare(100);
			MassInertia sqMI = MassInertia.FromSquare(300,100);
			PhysicsState sqstate = new PhysicsState();

			sqstate.Position.Linear = new Vector2D(300,150);

			world.AddICollidableBody(new RigidBody(sqMI, sqstate, sq));
			sqstate.Position.Linear += new Vector2D(0,-100);
			world.AddICollidableBody(new RigidBody(sqMI, sqstate, sq));
			sqstate.Position.Linear += new Vector2D(0,-100);
			world.AddICollidableBody(new RigidBody(sqMI, sqstate, sq));
		}
		
		public static  void Sandbox(Physics2D.World2D world)
		{
			StartingLocation = new Vector2D(0,150);
			StaticGravityField Field = new StaticGravityField(new Vector2D(0,700));
			world.AddIGravitySource(Field);
			world.CalcGravity = true;
            Polygon2D rect = (Polygon2D)Polygon2D.FromRectangle(600, 8000);
			MassInertia MI = new MassInertia(float.PositiveInfinity,float.PositiveInfinity);
			PhysicsState state = new PhysicsState();
			RigidBody platform;
			state.Velocity.Linear = new Vector2D(-.00009f,0);
			
			state.Position.Linear = new Vector2D(500,1300);
			platform = new RigidBody(MI, state, rect);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);
            float Width = 600;
            float Length = 1200;
            Polygon2D rect2 = (Polygon2D)Polygon2D.FromRectangle(Length, Width);
            state.Position.Linear = new Vector2D(500 + Width / 2, 500 - Length / 2);
			
			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            state.Position.Linear = new Vector2D(500 - Width / 2, 500 - Length / 2);

			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);
			
		}
		
		
		public static  void SetCircleUrQuanCage(Physics2D.World2D world)
		{
			StartingLocation = new Vector2D(0,0);
			//world.CalcGravity = false;
			/*int minx = -1300;
			int maxx = 1300;
			int miny = -1300;
			int maxy = 1300;*/
			float BoxSideSize = 8;
			for(float radius = 500; radius <= 1000; radius+= 400)
			{
				
			
				//float radius = 1500;

				float circ = 2*(float)Math.PI*radius;
				float NumberofBoxes = circ/(BoxSideSize*2);
				float Spacing = (NumberofBoxes/(circ/((float)Math.PI*2)));

				float sign = -1;
				for(float ra = 0; ra<= 2*(float)Math.PI;ra+=Spacing)
				{
					sign = -sign;
					RigidBody body = CreateUrQuan(world);// new RigidBody(new Square(BoxSideSize) ,MassInertia.FromSquare(float.PositiveInfinity,BoxSideSize),ra,);
					body.Current.Position = new ALVector2D(ra+(float)Math.PI,Vector2D.FromLengthAndAngle(radius,ra));
					//collidable.Current.Velocity.Angular = sign*90;
					//collidable.Flags = BodyFlags.InfiniteMass|BodyFlags.IgnoreGravity|BodyFlags.NoUpdate;
					world.AddICollidableBody(body);
				}
				BoxSideSize+= 8;
			}
			
		}

		public static  void FlipTest(Physics2D.World2D world)
		{
			StartingLocation = new Vector2D(0,-400);
			StaticGravityField Field = new StaticGravityField(new Vector2D(0,100));
			world.AddIGravitySource(Field);
			world.CalcGravity = true;
            Polygon2D rect = (Polygon2D)Polygon2D.FromRectangle(600, 8000);
			MassInertia MI = new MassInertia(float.PositiveInfinity,float.PositiveInfinity);
			PhysicsState state = new PhysicsState();
			RigidBody platform;
			
			
			state.Position.Linear = new Vector2D(500,500);
			//state.Position.Angular = .3;
			platform = new RigidBody(MI, state, rect);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            float Width = 600;
            float Length = 1200;
            Polygon2D rect2 = (Polygon2D)Polygon2D.FromRectangle(Length, Width);


            state.Position.Linear = new Vector2D(500 + Width / 2, 500 - Length / 2);
			
			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            state.Position.Linear = new Vector2D(500 - Width / 2, 500 - Length / 2);

			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);


			PhysicsState body1state = new PhysicsState();
			body1state.Position.Linear = new Vector2D(-300,100);
			body1state.Position.Angular = -.01f;
			body1state.Velocity.Linear = new Vector2D(2000,0);

            RigidBody body1 = new RigidBody(MassInertia.FromRectangle(700, 90, 500), body1state, (Polygon2D)Polygon2D.FromRectangle(90, 500));
			world.AddICollidableBody(body1);



			
		}


        public static void BalancingStackTest(Physics2D.World2D world)
        {
            StartingLocation = new Vector2D(0, 0);
            StaticGravityField Field = new StaticGravityField(new Vector2D(0, 900));
            world.AddIGravitySource(Field);
            world.CalcGravity = true;
            float Width = 8000;
            float Length = 600;
            Polygon2D rect = (Polygon2D)Polygon2D.FromRectangle(Length, Width);
            MassInertia MI = new MassInertia(float.PositiveInfinity, float.PositiveInfinity);
            PhysicsState state = new PhysicsState();
            RigidBody platform;
            state.Velocity.Linear = new Vector2D();

            state.Position.Linear = new Vector2D(500, 500);
            platform = new RigidBody(MI, state, rect);
            platform.Flags = platform.Flags | BodyFlags.IgnoreGravity;
            world.AddICollidableBody(platform);

            float Width2 = 600;
            float Length2 = 1200;
            Polygon2D rect2 = (Polygon2D)Polygon2D.FromRectangle(Length2, Width2);


            state.Position.Linear = new Vector2D(500 + Width / 2, 500 - Length / 2);

            platform = new RigidBody(MI, state, rect2);
            platform.Flags = platform.Flags | BodyFlags.IgnoreGravity;
            world.AddICollidableBody(platform);

            state.Position.Linear = new Vector2D(500 - Width / 2, 500 - Length / 2);

            platform = new RigidBody(MI, state, rect2);
            platform.Flags = platform.Flags | BodyFlags.IgnoreGravity;
            world.AddICollidableBody(platform);


            world.AddICollidableBody(
                new RigidBody(
                    MassInertia.FromRectangle(4000, 200, 200), 
                    new PhysicsState(new ALVector2D(0, new Vector2D(500, 100))),
                    (Polygon2D)Polygon2D.FromRectangle(200, 200)));
            world.AddICollidableBody(
                new RigidBody(
                    MassInertia.FromRectangle(2000, 50, 800), 
                    new PhysicsState(new ALVector2D(0, new Vector2D(600, -26))),
                    (Polygon2D)Polygon2D.FromRectangle(50, 800)));
            world.AddICollidableBody(
                new RigidBody(
                    MassInertia.FromRectangle(4000, 200, 200), 
                    new PhysicsState(new ALVector2D(0, new Vector2D(400, -158))),
                    (Polygon2D)Polygon2D.FromRectangle(200, 200)));
            world.AddICollidableBody(
                new RigidBody(
                    MassInertia.FromRectangle(3500, 190, 190), 
                    new PhysicsState(new ALVector2D(0, new Vector2D(400, -359))),
                    (Polygon2D)Polygon2D.FromRectangle(190, 190)));
            world.AddICollidableBody(
                new RigidBody(
                    MassInertia.FromRectangle(3000, 180, 180), 
                    new PhysicsState(new ALVector2D(0, new Vector2D(400, -560))),
                    (Polygon2D)Polygon2D.FromRectangle(180, 180)));
        }
        public static void DominoesTest(Physics2D.World2D world)
		{
			StartingLocation = new Vector2D(0,0);
			StaticGravityField Field = new StaticGravityField(new Vector2D(0,900));
			world.AddIGravitySource(Field);
			world.CalcGravity = true;
            float Width = 8000;
            float Length = 600;
            Polygon2D rect = (Polygon2D)Polygon2D.FromRectangle(Length, Width);
			MassInertia MI = new MassInertia(float.PositiveInfinity,float.PositiveInfinity);
			PhysicsState state = new PhysicsState();
			RigidBody platform;
			state.Velocity.Linear = new Vector2D();
			
			state.Position.Linear = new Vector2D(500,500);
			platform = new RigidBody(MI, state, rect);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            float Width2 = 600;
            float Length2 = 1200;
            Polygon2D rect2 = (Polygon2D)Polygon2D.FromRectangle(Length2, Width2);


            state.Position.Linear = new Vector2D(500 + Width / 2, 500 - Length / 2);
			
			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            state.Position.Linear = new Vector2D(500 - Width / 2, 500 - Length / 2);

			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            rect2 = (Polygon2D)Polygon2D.FromRectangle(200, 50);
			MassInertia MI2 = MassInertia.FromRectangle(2000,200,50);
			float range = 3034;
			float start = 300;
			int num  = 19;
			float inc = range/num;
			for(int pos = 0;pos != num; ++pos)
			{

				PhysicsState state2 = new PhysicsState();
				state2.Position.Linear = new Vector2D(start+inc*pos,100);
				RigidBody box = new RigidBody(MI2, state2, rect2);
				world.AddICollidableBody(box);
			}
			//200


		}
		
		public static  void DominoesStackTest(Physics2D.World2D world)
		{
			StartingLocation = new Vector2D(0,0);
			StaticGravityField Field = new StaticGravityField(new Vector2D(0,900));
			world.AddIGravitySource(Field);
			world.CalcGravity = true;
            Polygon2D rect = (Polygon2D)Polygon2D.FromRectangle(600, 8000);
			MassInertia MI = new MassInertia(float.PositiveInfinity,float.PositiveInfinity);
			PhysicsState state = new PhysicsState();
			RigidBody platform;
			
			state.Position.Linear = new Vector2D(500,500);
			platform = new RigidBody(MI, state, rect);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            float Width = 600;
            float Length = 1200;

            Polygon2D rect2 = (Polygon2D)Polygon2D.FromRectangle(Length, Width);


            state.Position.Linear = new Vector2D(500 + Width / 2, 500 - Length / 2);
			
			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            state.Position.Linear = new Vector2D(500 - Width / 2, 500 - Length / 2);

			platform = new RigidBody(MI, state, rect2);
			platform.Flags = platform.Flags|BodyFlags.IgnoreGravity;
			world.AddICollidableBody(platform);

            rect2 = (Polygon2D)Polygon2D.FromRectangle(200, 50);
			MassInertia MI2 = MassInertia.FromRectangle(1000,200,50);
			float range = 1450;
			float start = 300;
			int num  = 7;
			float inc = range/num;
			for(int pos = 0;pos != num; ++pos)
			{

				PhysicsState state2 = new PhysicsState();
				state2.Position.Linear = new Vector2D(start+inc*pos,100);
				RigidBody box = new RigidBody(MI2, state2, rect2);
				world.AddICollidableBody(box);

				PhysicsState state3 = new PhysicsState();
				state3.Position.Linear = new Vector2D(5+start+inc*pos,-25);
				state3.Position.Angular = (float)Math.PI/2;
				RigidBody box2 = new RigidBody(MI2, state3, rect2);
				world.AddICollidableBody(box2);

				PhysicsState state4 = new PhysicsState();
				state4.Position.Linear = new Vector2D(5+start+inc*pos,-150);
				//state3.Position.Angular = (float)Math.PI/2;
				RigidBody box3 = new RigidBody(MI2, state4, rect2);
				world.AddICollidableBody(box3);
				
				PhysicsState state5 = new PhysicsState();
				state5.Position.Linear = new Vector2D(5+start+inc*pos,-275);
				state5.Position.Angular = (float)Math.PI/2;
				RigidBody box4 = new RigidBody(MI2, state5, rect2);
				world.AddICollidableBody(box4);
			}
			//200


		}


        public static  void SetMultiPart(Physics2D.World2D world)
        {
            StartingLocation = new Vector2D(0, 300);
            PhysicsState bodystate = new PhysicsState();
            bodystate.Position.Linear = new Vector2D(0, 0);

            Coefficients coe = new Coefficients(0.8f, 0.4f, 0.1f);
            AdvanceMath.Geometry2D.Polygon2D bob = (Polygon2D)Polygon2D.FromSquare(100);

            RigidBodyPart[] parts = new RigidBodyPart[2];

            parts[0] = new RigidBodyPart(new ALVector2D(0, new Vector2D(-70, 0)), bob, coe);
            parts[1] = new RigidBodyPart(new ALVector2D(0,new Vector2D(70,0)), bob, coe);

            RigidBody body = new RigidBody(MassInertia.FromSquare(500, 100), new PhysicsState(), parts);

            world.AddICollidableBody(body);
        }
        public static  void SetChain(Physics2D.World2D world)
		{
			StartingLocation  = new Vector2D(0,300);
			PhysicsState bodystate = new PhysicsState();
			bodystate.Position.Linear = new Vector2D(0,0);

            RigidBody lastbody1 = new RigidBody(MassInertia.FromRectangle(700, 90, 500), bodystate, (Polygon2D)Polygon2D.FromRectangle(90, 200));
			float distance = 220;
			//world.AddICollidableBody(lastbody1);

			for(int pos = 0; pos != 40; ++pos)
			{
				bodystate.Position.Linear += new Vector2D(distance,0);
                RigidBody body1 = new RigidBody(MassInertia.FromRectangle(700, 90, 500), bodystate, (Polygon2D)Polygon2D.FromRectangle(90, 200));
				world.AddICollidableBody(body1);
				PinJoint joint = new PinJoint(new CollidablePair(lastbody1,body1),bodystate.Position.Linear - new Vector2D(distance/2,0),.1f,.06f);
				world.AddICollidableBody(body1);
				if(pos != 0)
				{
					world.AddIJoint(joint);
				}
				lastbody1 = body1;
			}
		}
		
		
		public static  void TrackedAnchorTest(Physics2D.World2D world)
		{
			//StartingLocation = new Vector2D(0,170);
            Polygon2D sq = (Polygon2D)Polygon2D.FromSquare(100);
			//MassInertia sqMI = MassInertia.FromSquare(3000,100);
			MassInertia sqMI = new  MassInertia(3000,float.PositiveInfinity);
			PhysicsState sqstate = new PhysicsState();
			sqstate.Position.Linear = new Vector2D(300,-150);
			RigidBody body = new RigidBody(sqMI, sqstate, sq); 
			world.AddICollidableBody(body);
			TrackedAnchor anchor = new TrackedAnchor(body,new Vector2D(300,-250),new Vector2D(0,1),.1f,.2f);
			world.AddIJoint(anchor);

		}
		public static  void AnchorTest(Physics2D.World2D world)
		{
			//StartingLocation = new Vector2D(0,170);
            Polygon2D sq = (Polygon2D)Polygon2D.FromSquare(100);
			MassInertia sqMI = MassInertia.FromSquare(3000,100);
			PhysicsState sqstate = new PhysicsState();
			sqstate.Position.Linear = new Vector2D(300,-350);
			RigidBody body = new RigidBody(sqMI, sqstate, sq); 
			world.AddICollidableBody(body);
			Anchor anchor = new Anchor(body,new Vector2D(300,-150),.1f,.02f);
			world.AddIJoint(anchor);
		}
		public static  void ImpulseWaveTest(Physics2D.World2D world)
		{
			PhysicsState bstate = new PhysicsState();
			ImpulseWave boom = new ImpulseWave(new Mortal(.15f), 900, bstate, 5, 3500);
			boom.CollisionState.GenerateCollisionEvents = true;
			world.AddICollidableBody(boom);
			
			//StartingLocation = new Vector2D(0,170);
			/*Square sq = new Square(100);
			MassInertia sqMI = MassInertia.FromSquare(3000,100);
			PhysicsState sqstate = new PhysicsState();
			sqstate.Position.Linear = new Vector2D(300,150);
			world.AddICollidableBody(new RigidBody(sq,sqMI,sqstate));
			sqstate.Position.Linear += new Vector2D(2,-103);
			world.AddICollidableBody(new RigidBody(sq,sqMI,sqstate));
			sqstate.Position.Linear += new Vector2D(2,-103);
			world.AddICollidableBody(new RigidBody(sq,sqMI,sqstate));*/
		}
		

		public static  void SetHumanRigidBody(Physics2D.World2D world, Vector2D pos )
		{

			float breakDistance = 35;//float.PositiveInfinity;
            float breakVelocity = 8000;//float.PositiveInfinity;
            float breakImpulse = 8000;//float.PositiveInfinity;
			float timescale = .5f;

			RigidBody Head = new RigidBody(MassInertia.FromSolidCylinder(120,20), 0, new Vector2D(0,0)+pos, new Circle2D(30));
            RigidBody Torso = new RigidBody(MassInertia.FromRectangle(2000, 150, 80), 0, new Vector2D(0, 108) + pos, (Polygon2D)Polygon2D.FromRectangle(150, 80));
			PinJoint Neck = new PinJoint(new CollidablePair(Head,Torso),new Vector2D(20,30)+pos,3,timescale,breakDistance,breakVelocity,breakImpulse);
			PinJoint Neck2 = new PinJoint(new CollidablePair(Head,Torso),new Vector2D(-20,30)+pos,3,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(-60, 80) + pos, (Polygon2D)Polygon2D.FromRectangle(75, 30));			
			PinJoint LSholder = new PinJoint(new CollidablePair(Torso,LArm),new Vector2D(-60,40)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LLArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(-60, 155) + pos, (Polygon2D)Polygon2D.FromRectangle(60, 27));			
			PinJoint LElbow= new PinJoint(new CollidablePair(LArm,LLArm),new Vector2D(-60,115)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);



            RigidBody RArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(60, 80) + pos, (Polygon2D)Polygon2D.FromRectangle(75, 30));			
			PinJoint RSholder = new PinJoint(new CollidablePair(Torso,RArm),new Vector2D(60,40)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);

            RigidBody RLArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(60, 155) + pos, (Polygon2D)Polygon2D.FromRectangle(60, 27));			
			PinJoint RElbow = new PinJoint(new CollidablePair(RArm,RLArm),new Vector2D(60,115)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);



            RigidBody RLeg = new RigidBody(MassInertia.FromRectangle(900, 250, 80), 0, new Vector2D(25, 265) + pos, (Polygon2D)Polygon2D.FromRectangle(150, 40));			
			PinJoint RHip = new PinJoint(new CollidablePair(Torso,RLeg),new Vector2D(25,190)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LLeg = new RigidBody(MassInertia.FromRectangle(900, 250, 80), 0, new Vector2D(-25, 265) + pos, (Polygon2D)Polygon2D.FromRectangle(150, 40));			
			PinJoint LHip = new PinJoint(new CollidablePair(Torso,LLeg),new Vector2D(-25,190)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);

			world.AddIJoint(Neck);
			world.AddIJoint(Neck2);
			world.AddICollidableBody(Head);
			world.AddICollidableBody(Torso);
			world.AddICollidableBody(LArm);
			world.AddIJoint(LSholder);
			world.AddICollidableBody(LLArm);
			world.AddIJoint(LElbow);
			
			world.AddICollidableBody(RArm);
			world.AddIJoint(RSholder);
			world.AddICollidableBody(RLArm);
			world.AddIJoint(RElbow);

			world.AddICollidableBody(RLeg);
			world.AddIJoint(RHip);

			world.AddICollidableBody(LLeg);
			world.AddIJoint(LHip);
		}
        public static void SetHumanRigidBodyV2(Physics2D.World2D world, Vector2D pos)
        {
            SetHumanRigidBodyV2(world, pos, 50, float.PositiveInfinity, 800000, .5f);
        }
        public static void SetHumanRigidBodyV2(Physics2D.World2D world, Vector2D pos, float breakDistance, float breakVelocity, float breakImpulse, float timescale)
        {
            SetHumanRigidBodyV2(world, pos,Vector2D.Zero,new Immortal(), breakDistance, breakVelocity, breakImpulse, timescale);
        }
        public static void SetHumanRigidBodyV2(Physics2D.World2D world, Vector2D pos,Vector2D velocity,ILifeTime lifetime, float breakDistance, float breakVelocity, float breakImpulse, float timescale)
		{
            

			RigidBody Head = new RigidBody(MassInertia.FromSolidCylinder(120,20), 0, new Vector2D(0,0)+pos, new Circle2D(30));
            Head.Current.Velocity.Linear = velocity;
            Head.LifeTime = (ILifeTime)lifetime.Clone();
            RigidBody Torso = new RigidBody(MassInertia.FromRectangle(2000, 150, 80), 0, new Vector2D(0, 108) + pos, (Polygon2D)Polygon2D.FromRectangle(150, 80));
            Torso.Current.Velocity.Linear = velocity;
            Torso.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint Neck = new PinJoint(new CollidablePair(Head,Torso),new Vector2D(20,30)+pos,3,timescale,breakDistance,breakVelocity,breakImpulse);
			PinJoint Neck2 = new PinJoint(new CollidablePair(Head,Torso),new Vector2D(-20,30)+pos,3,timescale,breakDistance,breakVelocity,breakImpulse);



            RigidBody LArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(-60, 80) + pos, (Polygon2D)Polygon2D.FromRectangle(75, 30));
            LArm.Current.Velocity.Linear = velocity;
            LArm.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint LSholder = new PinJoint(new CollidablePair(Torso,LArm),new Vector2D(-60,40)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LLArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(-60, 155) + pos, (Polygon2D)Polygon2D.FromRectangle(60, 27));
            LLArm.Current.Velocity.Linear = velocity;
            LLArm.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint LElbow= new PinJoint(new CollidablePair(LArm,LLArm),new Vector2D(-60,115)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);



            RigidBody RArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(60, 80) + pos, (Polygon2D)Polygon2D.FromRectangle(75, 30));
            RArm.Current.Velocity.Linear = velocity;
            RArm.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint RSholder = new PinJoint(new CollidablePair(Torso,RArm),new Vector2D(60,40)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody RLArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(60, 155) + pos, (Polygon2D)Polygon2D.FromRectangle(60, 27));
            RLArm.Current.Velocity.Linear = velocity;
            RLArm.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint RElbow = new PinJoint(new CollidablePair(RArm,RLArm),new Vector2D(60,115)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);





            RigidBody RLeg = new RigidBody(MassInertia.FromRectangle(500, 250, 80), 0, new Vector2D(25, 230) + pos, (Polygon2D)Polygon2D.FromRectangle(80, 40));
            RLeg.Current.Velocity.Linear = velocity;
            RLeg.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint RHip = new PinJoint(new CollidablePair(Torso,RLeg),new Vector2D(25,190)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LLeg = new RigidBody(MassInertia.FromRectangle(500, 250, 80), 0, new Vector2D(-25, 230) + pos, (Polygon2D)Polygon2D.FromRectangle(80, 40));
            LLeg.Current.Velocity.Linear = velocity;
            LLeg.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint LHip = new PinJoint(new CollidablePair(Torso,LLeg),new Vector2D(-25,190)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);


            RigidBody LRLeg = new RigidBody(MassInertia.FromRectangle(400, 250, 80), 0, new Vector2D(25, 312) + pos, (Polygon2D)Polygon2D.FromRectangle(80, 40));
            LRLeg.Current.Velocity.Linear = velocity;
            LRLeg.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint RKnee = new PinJoint(new CollidablePair(LRLeg,RLeg),new Vector2D(25,270)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LLLeg = new RigidBody(MassInertia.FromRectangle(400, 250, 80), 0, new Vector2D(-25, 312) + pos, (Polygon2D)Polygon2D.FromRectangle(80, 40));
            LLLeg.Current.Velocity.Linear = velocity;
            LLLeg.LifeTime = (ILifeTime)lifetime.Clone();
            PinJoint LKnee = new PinJoint(new CollidablePair(LLLeg,LLeg),new Vector2D(-25,270)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);


			world.AddIJoint(Neck);
			world.AddIJoint(Neck2);
			world.AddICollidableBody(Head);
			world.AddICollidableBody(Torso);
			world.AddICollidableBody(LArm);
			world.AddIJoint(LSholder);
			world.AddICollidableBody(LLArm);
			world.AddIJoint(LElbow);
			
			world.AddICollidableBody(RArm);
			world.AddIJoint(RSholder);
			world.AddICollidableBody(RLArm);
			world.AddIJoint(RElbow);




			world.AddICollidableBody(RLeg);
			world.AddIJoint(RHip);

			world.AddICollidableBody(LLeg);
			world.AddIJoint(LHip);

			world.AddICollidableBody(LRLeg);
			world.AddIJoint(RKnee);

			world.AddICollidableBody(LLLeg);
			world.AddIJoint(LKnee);
		}

        public static  void MassNumberOfUrQuanTest(Physics2D.World2D world)
        {
            Vector2D accel = Vector2D.Origin;
           // float gravity = 14.8f;
            //world.AddIGravitySource(new StaticGravityPoint(accel, gravity));
           // world.CalcGravity = true;
            StartingLocation = new Vector2D(2000, 2000);
            for (int pos1 = -1000; pos1 <= -400; pos1 += 300)
            {
                for (int pos2 = -2000; pos2 <= 800; pos2 += 300)
                {
                    RigidBody body = CreateUrQuan(world);
                    body.Current.Position.Linear = new Vector2D(pos1, pos2)+StartingLocation;
					body.Current.Velocity.Linear = new Vector2D(100,0);

                    world.AddICollidableBody(body);
                }
            }
            

        }
        public static  void EarthlingTest(Physics2D.World2D world)
        {
            //Vector2D accel = Vector2D.Origin;
            // float gravity = 14.8f;
            //world.AddIGravitySource(new StaticGravityPoint(accel, gravity));
            // world.CalcGravity = true;
            
                    RigidBody body = CreateEarthling(world);
                    body.Current.Position.Linear = new Vector2D(0, 0);
                    body.Current.Position.Angular = (float)Math.PI;
                    //collidable.Current.Velocity.Linear = new Vector2D(-100, 0);
                    //collidable.Current.Velocity.Angular = (float)rand.NextDouble();
                    world.AddICollidableBody(body);
               
            StartingLocation = new Vector2D(100, 300);

        }
        public static  void MassNumberOfEarthlingTest(Physics2D.World2D world)
		{
			Vector2D accel = Vector2D.Origin;
			// float gravity = 14.8f;
			//world.AddIGravitySource(new StaticGravityPoint(accel, gravity));
			// world.CalcGravity = true;
			for (int pos1 = 400; pos1 <=1000; pos1 += 300)
			{
				for (int pos2 = -1850; pos2 <= 800; pos2 += 300)
				{
					RigidBody body = CreateEarthling(world);
					body.Current.Position.Linear = new Vector2D(pos1, pos2);
					body.Current.Position.Angular = (float)Math.PI;
					body.Current.Velocity.Linear = new Vector2D(-100,0);
					//collidable.Current.Velocity.Angular = (float)rand.NextDouble();
					world.AddICollidableBody(body);
				}
			}
			StartingLocation = new Vector2D(1000, 1000);

		}
        public static  void MassNumberOfMTest(Physics2D.World2D world)
		{
			Vector2D accel= Vector2D.Origin;
			float gravity = 14.8f;
			world.AddIGravitySource(new StaticGravityPoint(accel,gravity));
			world.CalcGravity = true;

			for(int pos1 = -100;pos1 <= 1000;pos1+=300)
			{
				for(int pos2 = -400;pos2 <= 900;pos2+= 300)
				{
					SetM(world,new Vector2D(pos1,pos2));
				}
			}									  
		}
		
		public static  void MassNumberOfHumanBodiesTest(Physics2D.World2D world)
		{
			//Vector2D accel= Vector2D.Origin;
			//float gravity = 1.8f;
			//world.AddIGravitySource(new StaticGravityPoint(accel,gravity));
			//world.CalcGravity = true;
			for(int pos1 = -4000;pos1 <= -0000;pos1+=700)
			{
				for(int pos2 = -2000;pos2 <= -500;pos2+= 700)
				{
					SetHumanRigidBodyV2(world,new Vector2D(pos1,pos2));
				}
			}									  
		}
		public static  void SetJim(Physics2D.World2D world)
		{
			SetJ(world,new Vector2D(100,150));
            RigidBody I = new RigidBody(MassInertia.FromRectangle(300, 100, 30), 0, new Vector2D(220, 150), (Polygon2D)Polygon2D.FromRectangle(100, 30));
			world.AddICollidableBody(I);
			SetM(world,new Vector2D(280,150));
		}
		public static  void SetMom(Physics2D.World2D world)
		{
			SetM(world,new Vector2D(0,0));
			SetO(world,new Vector2D(150,0));
			SetM(world,new Vector2D(270,0));
		}
		public static  void SetPinJointTest(Physics2D.World2D world)
		{
            RigidBody body1 = new RigidBody(MassInertia.FromSquare(600, 50), 0, new Vector2D(-25, 0), (Polygon2D)Polygon2D.FromRectangle(100, 50));
            RigidBody body2 = new RigidBody(MassInertia.FromSquare(600, 50), 0, new Vector2D(25, 0), (Polygon2D)Polygon2D.FromRectangle(100, 50));
            RigidBody body3 = new RigidBody(MassInertia.FromSquare(600, 50), 0, new Vector2D(75, 0), (Polygon2D)Polygon2D.FromRectangle(100, 50));
			PinJoint joint1 = new PinJoint(new CollidablePair(body1,body2),new Vector2D(0,-25),0,.05f);
			PinJoint joint2 = new PinJoint(new CollidablePair(body1,body2),new Vector2D(0,25),0,.05f);
			PinJoint joint3 = new PinJoint(new CollidablePair(body3,body2),new Vector2D(25,-25),0,.05f);
			PinJoint joint4 = new PinJoint(new CollidablePair(body3,body2),new Vector2D(25,25),0,.05f);
			body1.Current.Velocity.Angular = 1;
			
			world.AddIJoint(joint1);
			world.AddIJoint(joint2);
			world.AddIJoint(joint3);
			world.AddIJoint(joint4);
			world.AddICollidableBody(body1);
			world.AddICollidableBody(body2);
			world.AddICollidableBody(body3);
		}
		public static  void SetM(Physics2D.World2D world,Vector2D Mpos)
		{
            RigidBody M1 = new RigidBody(MassInertia.FromRectangle(600, 100, 30), 0, new Vector2D(0, 0) + Mpos, (Polygon2D)Polygon2D.FromRectangle(100, 30));
			
			Polygon2D MP2 = new Polygon2D(new Vector2D[]{
														new Vector2D(15,50),
														new Vector2D(-15,-25),
														new Vector2D(-15,-50),
														new Vector2D(0,-50),
														new Vector2D(15,25),
													});
			RigidBody M2 = new RigidBody(MassInertia.FromRectangle(600,100,30), 0, new Vector2D(30,0)+Mpos, MP2);

			Polygon2D MP3 = new Polygon2D(new Vector2D[]{
														new Vector2D(-15,50),
														new Vector2D(-15,25),
														new Vector2D(0,-50),
														new Vector2D(15,-50),
														new Vector2D(15,-25),

			});
			RigidBody M3 = new RigidBody(MassInertia.FromRectangle(600,100,30), 0, new Vector2D(60,0)+Mpos, MP3);
            RigidBody M4 = new RigidBody(MassInertia.FromRectangle(600, 100, 30), 0, new Vector2D(90, 0) + Mpos, (Polygon2D)Polygon2D.FromRectangle(100, 30));
			
			PinJoint MJ1a = new PinJoint(new CollidablePair(M1,M2),new Vector2D(15,-50)+Mpos,0,.05f);
			PinJoint MJ1b = new PinJoint(new CollidablePair(M1,M2),new Vector2D(15,-25)+Mpos,0,.05f);
			PinJoint MJ2a = new PinJoint(new CollidablePair(M2,M3),new Vector2D(45,50)+Mpos,0,.05f);
			PinJoint MJ2b = new PinJoint(new CollidablePair(M2,M3),new Vector2D(45,25)+Mpos,0,.05f);
			PinJoint MJ3a = new PinJoint(new CollidablePair(M3,M4),new Vector2D(75,-50)+Mpos,0,.05f);
			PinJoint MJ3b = new PinJoint(new CollidablePair(M3,M4),new Vector2D(75,-25)+Mpos,0,.05f);
			world.AddIJoint(MJ1a);
			world.AddIJoint(MJ1b);
			world.AddIJoint(MJ2a);
			world.AddIJoint(MJ2b);
			world.AddIJoint(MJ3a);
			world.AddIJoint(MJ3b);
			world.AddICollidableBody(M1);
			world.AddICollidableBody(M2);
			world.AddICollidableBody(M3);
			world.AddICollidableBody(M4);
		}
		public static  void SetO(Physics2D.World2D world,Vector2D Opos)
		{
            RigidBody O1 = new RigidBody(MassInertia.FromRectangle(600, 100, 30), 0, new Vector2D(0, 0) + Opos, (Polygon2D)Polygon2D.FromRectangle(100, 30));
            RigidBody O2 = new RigidBody(MassInertia.FromRectangle(600, 30, 30), 0, new Vector2D(30, 35) + Opos, (Polygon2D)Polygon2D.FromRectangle(30, 30));
            RigidBody O3 = new RigidBody(MassInertia.FromRectangle(600, 100, 30), 0, new Vector2D(60, 0) + Opos, (Polygon2D)Polygon2D.FromRectangle(100, 30));
            RigidBody O4 = new RigidBody(MassInertia.FromRectangle(600, 30, 30), 0, new Vector2D(30, -35) + Opos, (Polygon2D)Polygon2D.FromRectangle(30, 30));

			PinJoint OJ1a = new PinJoint(new CollidablePair(O1,O2),new Vector2D(15,50)+Opos,0,.05f);
			PinJoint OJ1b = new PinJoint(new CollidablePair(O1,O2),new Vector2D(15,15)+Opos,0,.05f);
			PinJoint OJ2a = new PinJoint(new CollidablePair(O1,O4),new Vector2D(15,-50)+Opos,0,.05f);
			PinJoint OJ2b = new PinJoint(new CollidablePair(O1,O4),new Vector2D(15,-15)+Opos,0,.05f);

			PinJoint OJ3a = new PinJoint(new CollidablePair(O3,O2),new Vector2D(45,50)+Opos,0,.05f);
			PinJoint OJ3b = new PinJoint(new CollidablePair(O3,O2),new Vector2D(45,15)+Opos,0,.05f);
			PinJoint OJ4a = new PinJoint(new CollidablePair(O3,O4),new Vector2D(45,-50)+Opos,0,.05f);
			PinJoint OJ4b = new PinJoint(new CollidablePair(O3,O4),new Vector2D(45,-15)+Opos,0,.05f);

			world.AddIJoint(OJ1a);
			world.AddIJoint(OJ1b);
			world.AddIJoint(OJ2a);
			world.AddIJoint(OJ2b);

			world.AddIJoint(OJ3a);
			world.AddIJoint(OJ3b);
			world.AddIJoint(OJ4a);
			world.AddIJoint(OJ4b);

			world.AddICollidableBody(O1);
			world.AddICollidableBody(O2);
			world.AddICollidableBody(O3);
			world.AddICollidableBody(O4);
		}
		public static  void SetJ(Physics2D.World2D world,Vector2D Opos)
		{
            RigidBody O1 = new RigidBody(MassInertia.FromRectangle(600, 100, 30), 0, new Vector2D(10, 25) + Opos, (Polygon2D)Polygon2D.FromRectangle(50, 20));
            RigidBody O2 = new RigidBody(MassInertia.FromRectangle(600, 30, 30), 0, new Vector2D(30, 35) + Opos, (Polygon2D)Polygon2D.FromRectangle(30, 30));
            RigidBody O3 = new RigidBody(MassInertia.FromRectangle(600, 100, 30), 0, new Vector2D(60, 0) + Opos, (Polygon2D)Polygon2D.FromRectangle(100, 30));
			
			PinJoint OJ1a = new PinJoint(new CollidablePair(O1,O2),new Vector2D(15,50)+Opos,0,.05f);
			PinJoint OJ1b = new PinJoint(new CollidablePair(O1,O2),new Vector2D(15,15)+Opos,0,.05f);


			PinJoint OJ2a = new PinJoint(new CollidablePair(O3,O2),new Vector2D(45,50)+Opos,0,.05f);
			PinJoint OJ2b = new PinJoint(new CollidablePair(O3,O2),new Vector2D(45,15)+Opos,0,.05f);


			world.AddIJoint(OJ1a);
			world.AddIJoint(OJ1b);
			world.AddIJoint(OJ2a);
			world.AddIJoint(OJ2b);


			world.AddICollidableBody(O1);
			world.AddICollidableBody(O2);
			world.AddICollidableBody(O3);
		}
		public static  void SetILTest(Physics2D.World2D world)
		{
            RigidBody I1 = new RigidBody(MassInertia.FromSquare(600, 50), 0, new Vector2D(-25, 0), (Polygon2D)Polygon2D.FromRectangle(100, 50));
            RigidBody I2 = new RigidBody(MassInertia.FromSquare(600, 50), 0, new Vector2D(25, 0), (Polygon2D)Polygon2D.FromRectangle(50, 50));
            RigidBody I3 = new RigidBody(MassInertia.FromSquare(600, 50), 0, new Vector2D(75, 0), (Polygon2D)Polygon2D.FromRectangle(100, 50));
			PinJoint Ijoint1 = new PinJoint(new CollidablePair(I1,I2),new Vector2D(0,-25),0,.05f);
			PinJoint Ijoint2 = new PinJoint(new CollidablePair(I1,I2),new Vector2D(0,25),0,.05f);
			PinJoint Ijoint3 = new PinJoint(new CollidablePair(I3,I2),new Vector2D(25,-25),0,.05f);
			PinJoint Ijoint4 = new PinJoint(new CollidablePair(I3,I2),new Vector2D(25,25),0,.05f);
			world.AddIJoint(Ijoint1);
			world.AddIJoint(Ijoint2);
			world.AddIJoint(Ijoint3);
			world.AddIJoint(Ijoint4);
			world.AddICollidableBody(I1);
			world.AddICollidableBody(I2);
			world.AddICollidableBody(I3);

            RigidBody L1 = new RigidBody(MassInertia.FromSquare(600, 50), 0, new Vector2D(150, 0), (Polygon2D)Polygon2D.FromRectangle(50, 150));
            RigidBody L2 = new RigidBody(MassInertia.FromSquare(600, 50), 0, new Vector2D(200, 50), (Polygon2D)Polygon2D.FromRectangle(50, 50));
			PinJoint Ljoint1 = new PinJoint(new CollidablePair(L1,L2),new Vector2D(175,50),0,.05f);
			PinJoint Ljoint2 = new PinJoint(new CollidablePair(L1,L2),new Vector2D(175,100),0,.05f);
			world.AddIJoint(Ljoint1);
			world.AddIJoint(Ljoint2);
			world.AddICollidableBody(L1);
			world.AddICollidableBody(L2);
		}
		public static  void SetForceTest(Physics2D.World2D world)
		{
            RigidBody body1 = new RigidBody(MassInertia.FromSquare(6000, 30), 0, new Vector2D(-16, 0), (Polygon2D)Polygon2D.FromSquare(30));
			//collidable1.Current.Velocity.Angular = .1f;
			
			world.AddICollidableBody(body1);
			body1.ApplyForce(new ForceInfo(new Vector2D(600000,0),new Vector2D(0,-50)));
			body1.Update(.01f);
		}
		/*public static  void SetPoolTable(Physics2D.World2D world)
		{
			RigidBody wall1 = new RigidBody(new Rectangle(100,1500),MassInertia.FromRectangle(float.PositiveInfinity,500,100),0,new Vector2D(0,-500));
			RigidBody wall2 = new RigidBody(new Rectangle(100,1500),MassInertia.FromRectangle(float.PositiveInfinity,500,100),0,new Vector2D(0,500));
			RigidBody wall3 = new RigidBody(new Rectangle(1100,100),MassInertia.FromRectangle(float.PositiveInfinity,500,100),0,new Vector2D(800,0));
			RigidBody wall4 = new RigidBody(new Rectangle(1100,100),MassInertia.FromRectangle(float.PositiveInfinity,500,100),0,new Vector2D(-800,0));
			wall1.Coefficients.Restitution = .5f;
			world.AddICollidableBody(wall1);
			wall2.Coefficients.Restitution = .5f;
			world.AddICollidableBody(wall2);
			wall3.Coefficients.Restitution = .5f;
			world.AddICollidableBody(wall3);
			wall4.Coefficients.Restitution = .5f;
			world.AddICollidableBody(wall4);
			RigidBody queueball = new RigidBody(new Circle(20),MassInertia.FromSolidCylinder(200,20),0, new  Vector2D(-300,0));
			queueball.Current.Velocity.Linear = new Vector2D(500,(float)(float)rand.NextDouble()*6-3);
			queueball.Current.Velocity.Angular = (float)((float)rand.NextDouble()*4)-2;
			world.AddICollidableBody(queueball);
			Vector2D[] balls = new Vector2D[]{
											   new  Vector2D(200,0),
											   new  Vector2D(236,20),
											   new  Vector2D(236,-20),
											   new  Vector2D(272,0),
											   new  Vector2D(272,-40),
											   new  Vector2D(272,40),
											   new  Vector2D(308,20),
											   new  Vector2D(308,-20),
											   new  Vector2D(308,60),
											   new  Vector2D(308,-60),
											};
			foreach(Vector2D point in balls)
			{
				RigidBody ball = new RigidBody(new Circle(20),MassInertia.FromSolidCylinder(200,20),0,point);
				ball.Coefficients.Restitution = .8f;
				world.AddICollidableBody(ball);
			}
			StartingLocation = new Vector2D(0,200);
			
		}
        */
		public static  void SetVerySpecialBallTest(Physics2D.World2D world)
		{
			float CageMaxX = 600;
			float CageMaxY = 600;
			float CageMinX = -600;
			float CageMinY = -600;
			float CageRadius = 25;
			
			float CoreMaxX = 400;
			float CoreMaxY = 400;
			float CoreMinX = -300;
			float CoreMinY = -300;
			float Spacing = 50;
			float CoreRadius = 25;


			// make cage
			bool CageXisGood;
			bool CageYisGood;
			bool CoreXisGood;
			bool CoreYisGood;
			RigidBody body2 = new RigidBody(MassInertia.FromSolidCylinder(50,CoreRadius), 0, new Vector2D(500,25), new Circle2D(CoreRadius));
			body2.Current.Velocity.Linear = new Vector2D(-220,6);
			body2.Current.Velocity.Angular = 5;
			
			world.AddICollidableBody(body2);
			RigidBody body3 = new RigidBody(MassInertia.FromSolidCylinder(50,CoreRadius), 0, new Vector2D(500,85), new Circle2D(CoreRadius));
			body3.Current.Velocity.Linear = new Vector2D(-220,-6);
			body3.Current.Velocity.Angular = 5;
			world.AddICollidableBody(body3);

			for(float pos1 = CageMinX;pos1 <=CageMaxX;pos1+=Spacing)
			{
				CoreXisGood = pos1 >= CoreMinX && pos1 <= CoreMaxX;
				CageXisGood = pos1 == CageMaxX || pos1 == CageMinX;

				if(CoreXisGood)
				{
					//RigidBody collidable2 = new RigidBody(new Circle(CoreRadius),MassInertia.FromSolidCylinder(50,CoreRadius),0,new Vector2D(pos1,CoreMaxY+((CageMaxY -CoreMaxY)/2)));
					//collidable2.Current.Velocity.Linear = new Vector2D(0,-120);
					//world.AddICollidableBody(collidable2);
				}
				for(float pos2 = CageMinY;pos2 <=CageMaxY;pos2+=Spacing)
				{
					CageYisGood = pos2 == CageMaxY || pos2 == CageMinY;
					CoreYisGood = pos2 >= CoreMinY && pos2 <= CoreMaxY;

					if(CageXisGood||CageYisGood)
					{
						RigidBody body = new RigidBody(MassInertia.FromSolidCylinder(float.PositiveInfinity,CageRadius), 0, new Vector2D(pos1,pos2), new Circle2D(CageRadius));
						body.Flags = body.Flags|BodyFlags.IgnoreGravity;
						world.AddICollidableBody(body);
					}
					if(CoreXisGood&&CoreYisGood)
					{
						RigidBody body = new RigidBody(MassInertia.FromSolidCylinder(50,CoreRadius), 0, new Vector2D(pos1,pos2), new Circle2D(CoreRadius));
						world.AddICollidableBody(body);
					}
				}
			}
		}
		public static  void SetGravityTest(Physics2D.World2D world)
		{
			world.CalcGravity =true;
			world.AddIGravitySource(new StaticGravityPoint(new Vector2D(1000,1000),200));
			world.AddIGravitySource(new StaticGravityPoint(new Vector2D(-1000,1000),200));
			world.AddIGravitySource(new StaticGravityPoint(new Vector2D(-1000,-1000),200));
			world.AddIGravitySource(new StaticGravityPoint(new Vector2D(1000,-1000),200));
		}
		public static  void SetSuperPolygon2DTest(Physics2D.World2D world)
		{
			RigidBody test = new RigidBody(MassInertia.FromSolidCylinder(400,50), 0, new Vector2D(400,400), new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 5,50)));
			//test.Current.Velocity.Linear = new Vector2D(-40,0);
			world.AddICollidableBody(test);
		}

		public static  void SetPlanet(Physics2D.World2D world)
		{
			//Vector2D accel= Vector2D.Origin;
			//float gravity = 9.8;
			//world.gravity.AddGravityPoint(new StaticGravityPoint(accel,gravity));
			//world.CalcGravity = true;
			RigidBody body = new RigidBody(MassInertia.FromSolidCylinder(float.PositiveInfinity,180), 0, Vector2D.Origin , new Circle2D(180));
			body.Flags = BodyFlags.InfiniteMass;
			body.Current.Velocity.Angular = 1;
			world.AddICollidableBody(body);
		}
		public static  void MassNumberOfCirclesTest(Physics2D.World2D world)
		{

			Vector2D accel= Vector2D.Origin;
			float gravity = 9.8f;
			world.AddIGravitySource(new StaticGravityPoint(accel,gravity));
			world.CalcGravity = true;
			for(int pos1 =-400;pos1 <= 400;pos1+=50)
			{
				for(int pos2 = -400;pos2 <= 400;pos2+= 50)
				{
					float r = (float)(float)rand.NextDouble()*3;
					RigidBody body = new RigidBody(MassInertia.FromSolidCylinder(90,15+r), 0, new Vector2D(pos1,pos2), new Circle2D(15+r));
					body.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-body.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-body.Current.Position.Linear).Magnitude)).RightHandNormal;
					world.AddICollidableBody(body);
				}
			}									  
		}
		
		public static  void MassNumberOfSquaresAndCirclesTest(Physics2D.World2D world)
		{
			Vector2D accel= new  Vector2D(500,500);
			float gravity = 9.8f;
			world.AddIGravitySource(new StaticGravityPoint(accel,gravity));
			world.CalcGravity = true;
			for(int pos1 = 200;pos1 <= 700;pos1+=50)
			{
				for(int pos2 = 200;pos2 <= 600;pos2+= 50)
				{
					if(rand.Next(2)==0)
					{
                        RigidBody body = new RigidBody(MassInertia.FromSquare(90, 30), 0, new Vector2D(pos1, pos2), (Polygon2D)Polygon2D.FromSquare(30));
						body.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-body.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-body.Current.Position.Linear).Magnitude)).RightHandNormal;
						world.AddICollidableBody(body);
					}
					else
					{
						float r = (float)(float)rand.NextDouble()*3;
						RigidBody body = new RigidBody(MassInertia.FromSolidCylinder(90,15+r), 0, new Vector2D(pos1,pos2), new Circle2D(15+r));
						//collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(world.accel-collidable.Current.Position.Linear,(float)Math.Sqrt(2*(world.gravity*0.63661977236758134307553505349036)*(world.accel-collidable.Current.Position.Linear).Magnitude)).GetRightHandNormal();
						world.AddICollidableBody(body);
					}
				}
			}
												  
		}
		public static  void MassNumberOfBodiesTest(Physics2D.World2D world)
		{
			Vector2D accel= Vector2D.Origin;
			//float gravity = 9.8;
			//world.gravity.AddGravitySource(new StaticGravityPoint(accel,gravity));
			world.CalcGravity = false;

			for(int pos1 = -1000;pos1 <= 1000;pos1+=200)
			{
				for(int pos2 = -600;pos2 <= 600;pos2+= 200)
				{
					switch(rand.Next(0,3))
					{
						case 0:
							RigidBody body = new RigidBody(MassInertia.FromSquare(400,30), 0, new Vector2D(pos1,pos2), new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( rand.Next(5,9),30)));
							//collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-collidable.Current.Position.Linear,(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-collidable.Current.Position.Linear).Magnitude)).RightHandNormal;
							world.AddICollidableBody(body);
							break;
						case 1:
							float r = (float)(float)rand.NextDouble()*3;
							RigidBody body1 = new RigidBody(MassInertia.FromSolidCylinder(90,15+r), 0, new Vector2D(pos1,pos2), new Circle2D(15+r));
							//collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-collidable.Current.Position.Linear,(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-collidable.Current.Position.Linear).Magnitude)).RightHandNormal;
							world.AddICollidableBody(body1);
							break;
						case 2:
                            RigidBody body2 = new RigidBody(MassInertia.FromSquare(90, 30), 0, new Vector2D(pos1, pos2), (Polygon2D)Polygon2D.FromSquare(30));
							//collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-collidable.Current.Position.Linear,(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-collidable.Current.Position.Linear).Magnitude)).RightHandNormal;
							world.AddICollidableBody(body2);
							break;
						default:
							break;
					}
				}
			}
												  
		}
        public static void MassNumberOfMinesTest(Physics2D.World2D world)
        {
            //Vector2D accel= Vector2D.Origin;
            //float gravity = 9.8f;
            //world.AddIGravitySource(new StaticGravityPoint(accel,gravity));
            //world.CalcGravity = true;

            for (int pos1 = -8000; pos1 <= 8000; pos1 += 450 + rand.Next(0, 100))
            {
                for (int pos2 = -13200; pos2 <= -200; pos2 += 450 + rand.Next(0, 100))
                {
                    RigidBody body = new RigidBody(MassInertia.FromSquare(6000, 90), 0, new Vector2D(pos1 + rand.Next(-450, 450), pos2 + rand.Next(-450, 450)), new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( rand.Next(3, 9), 30)));
                    body.CollisionState.GenerateCollisionEvents = true;
                    body.LifeTime = new Mortal(999999);
                    //collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-collidable.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-collidable.Current.Position.Linear).Magnitude)).RightHandNormal;
                    //collidable.Current.Velocity.Angular = 1;
                    world.AddICollidableBody(body);
                }
            }

        }
        public static void MassNumberOfPolygon2DsTest(Physics2D.World2D world)
		{
			Vector2D accel= Vector2D.Origin;
			float gravity = 50.8f;
			world.AddIGravitySource(new StaticGravityPoint(accel,gravity));
			world.CalcGravity = true;
			for(int pos1 = -21000;pos1 <= 21000;pos1+=300)
			{
				for(int pos2 = -2000;pos2 <= -000;pos2+= 300)
				{
					RigidBody body = new RigidBody(MassInertia.FromSquare(600,90), 0, new Vector2D(pos1,pos2), new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( rand.Next( 3,9),30)));
					body.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-body.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-body.Current.Position.Linear).Magnitude)).RightHandNormal;
					//collidable.Current.Velocity.Angular = 1;
					world.AddICollidableBody(body);
				}
			}
												  
		}
		public static  void MassNumberOfSquaresTest(Physics2D.World2D world)
		{
			//Vector2D accel= Vector2D.Origin;
			//float gravity = 9.8;
			//world.gravity.AddGravitySource(new StaticGravityPoint(accel,gravity));
			world.CalcGravity = false;

			StartingLocation = new Vector2D(900,900);
			for(int pos1 = -500;pos1 <= 1400;pos1+=90)
			{
				for(int pos2 = -800;pos2 <= 800;pos2+= 90)
				{
                    RigidBody body = new RigidBody(MassInertia.FromSquare(90, 30), 0, new Vector2D(pos1, pos2), (Polygon2D)Polygon2D.FromSquare(30));
					//collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-collidable.Current.Position.Linear,(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-collidable.Current.Position.Linear).Magnitude)).RightHandNormal;
					world.AddICollidableBody(body);
				}
			}								  
		}
        public static void MassNumberOfRectTest(Physics2D.World2D world)
        {
            //Vector2D accel= Vector2D.Origin;
            //float gravity = 9.8;
            //world.gravity.AddGravitySource(new StaticGravityPoint(accel,gravity));
            world.CalcGravity = false;

            StartingLocation = new Vector2D(900, 900);
            for (int pos1 = -2000; pos1 <= 2000; pos1 += 130 + rand.Next(300))
            {
                for (int pos2 = -2000; pos2 <= 2000; pos2 += 130 + rand.Next(300))
                {
                    float l = 30 + (float)(rand.NextDouble() * 150);
                    float w = 30 + (float)(rand.NextDouble() * 150);

                    RigidBody body = new RigidBody(MassInertia.FromRectangle(l * w/10, l, w), 0, new Vector2D(pos1, pos2), (Polygon2D)Polygon2D.FromRectangle(l, w));
                    //collidable.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-collidable.Current.Position.Linear,(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-collidable.Current.Position.Linear).Magnitude)).RightHandNormal;
                    world.AddICollidableBody(body);
                }
            }
        }
		public static  void SetTest(Physics2D.World2D world)
		{
			world.CalcGravity = false;
            RigidBody test = new RigidBody(MassInertia.FromRectangle(900, 50, 100), 0, new Vector2D(540, 600), (Polygon2D)Polygon2D.FromRectangle(50, 100));
			//test.Current.Velocity.Linear = new Vector2D(0,-40);
			world.AddICollidableBody(test);
            RigidBody test2 = new RigidBody(MassInertia.FromRectangle(900, 50, 100), 0, new Vector2D(500, 400), (Polygon2D)Polygon2D.FromRectangle(50, 90));
			//test2.Current.Velocity.Linear = new Vector2D(1,40);
			world.AddICollidableBody(test2);
		}
		public static  void Set3SquareTest(Physics2D.World2D world)
		{
			world.CalcGravity = false;
            RigidBody test = new RigidBody(MassInertia.FromSquare(100, 50), 0, new Vector2D(500, 600), (Polygon2D)Polygon2D.FromSquare(50));
			test.Current.Velocity.Linear = new Vector2D(0,-50);
            RigidBody test2 = new RigidBody(MassInertia.FromSquare(100, 50), 0, new Vector2D(500, 500), (Polygon2D)Polygon2D.FromSquare(50));//
            RigidBody test3 = new RigidBody(MassInertia.FromSquare(100, 50), 0, new Vector2D(500, 400), (Polygon2D)Polygon2D.FromSquare(50));//
            RigidBody test4 = new RigidBody(MassInertia.FromSquare(100, 50), 0, new Vector2D(500, 300), (Polygon2D)Polygon2D.FromSquare(50));//
			test3.Current.Velocity.Linear = new Vector2D(0,40);
			test4.Current.Velocity.Linear = new Vector2D(0,40);
			world.AddICollidableBody(test2);
			world.AddICollidableBody(test);
			world.AddICollidableBody(test3);
			world.AddICollidableBody(test4);
		}
		public static  void Set2SquareTest(Physics2D.World2D world)
		{
			world.CalcGravity = false;
            RigidBody test = new RigidBody(MassInertia.FromSquare(300, 100), 0, new Vector2D(300, 600), (Polygon2D)Polygon2D.FromSquare(100));
			test.Current.Velocity.Linear = new Vector2D(0,-40);
			//test.Current.Velocity.Angular  = -23;

            RigidBody test2 = new RigidBody(MassInertia.FromSquare(300, 100), 4, new Vector2D(300, 400), (Polygon2D)Polygon2D.FromSquare(100));//
			test2.Current.Velocity.Linear = new Vector2D(0,40);
			world.AddICollidableBody(test2);
			world.AddICollidableBody(test);
		}
		public static  void SetFrictionTest(Physics2D.World2D world)
		{
			world.CalcGravity = false;
            RigidBody test = new RigidBody(MassInertia.FromRectangle(300, 50, 100), (float)5 - (float)(float)Math.PI / 2, new Vector2D(500, 530), (Polygon2D)Polygon2D.FromRectangle(50, 100));
			test.Current.Velocity.Linear = new Vector2D(-80,-40);
			test.Current.Velocity.Angular  = .5f;
			world.AddICollidableBody(test);
            RigidBody test2 = new RigidBody(MassInertia.FromRectangle(3000, 50, 800), (float)(float)Math.PI / 2, new Vector2D(400, 400), (Polygon2D)Polygon2D.FromRectangle(800, 50));
			test2.Current.Velocity.Linear = new Vector2D(-80,0);
			world.AddICollidableBody(test2);
		}
		public static  void Set2RectagleTest(Physics2D.World2D world)
		{
			world.CalcGravity = false;
            RigidBody test = new RigidBody(MassInertia.FromRectangle(300, 50, 100), 0, new Vector2D(500, 600), (Polygon2D)Polygon2D.FromRectangle(50, 100));
			test.Current.Velocity.Linear = new Vector2D(0,-40);
			//test.Current.Velocity.Angular  = -.5f;
			world.AddICollidableBody(test);

            RigidBody test2 = new RigidBody(MassInertia.FromRectangle(300, 50, 800), 0, new Vector2D(560, 400), (Polygon2D)Polygon2D.FromRectangle(50, 800));
			test2.Current.Velocity.Linear = new Vector2D(0,40);
			world.AddICollidableBody(test2);
		}
		public static  void Set2RotatingRectagleTest(Physics2D.World2D world)
		{
			world.CalcGravity = false;
            RigidBody test = new RigidBody(MassInertia.FromRectangle(300, 50, 100), 0, new Vector2D(500, 530), (Polygon2D)Polygon2D.FromRectangle(50, 100));
			test.Current.Velocity.Linear = new Vector2D(0,-40);
			test.Current.Velocity.Angular  = 1;
			world.AddICollidableBody(test);
            RigidBody test2 = new RigidBody(MassInertia.FromRectangle(300, 50, 100), 0, new Vector2D(580, 470), (Polygon2D)Polygon2D.FromRectangle(50, 100));
			// test2.Current.Velocity.Linear = new Vector2D(0,40);
			world.AddICollidableBody(test2);
		}
		public static  void SetCircleRectagleTest(Physics2D.World2D world)
		{
            RigidBody test2 = new RigidBody(MassInertia.FromRectangle(300, 50, 100), (float)(float)Math.PI * 2, new Vector2D(535, -400), (Polygon2D)Polygon2D.FromRectangle(50, 150));
			test2.Current.Velocity.Linear = new Vector2D(0,40);
			world.AddICollidableBody(test2);
			//world.CalcGravity = false;
			RigidBody test = new RigidBody(MassInertia.FromRectangle(300,50,100), 0, new Vector2D(485,-200), new Circle2D(25));
			test.Current.Velocity.Linear = new Vector2D(0,-40);
			//test.Current.Velocity.Angular  = -.5f;
			world.AddICollidableBody(test);


		}
		public static  void SetCircleSquareTest(Physics2D.World2D world)
		{
			//world.CalcGravity = false;
			RigidBody test = new RigidBody(MassInertia.FromRectangle(300,50,100), (float)(float)Math.PI/4, new Vector2D(485,0), new Circle2D(25));
			test.Current.Velocity.Linear = new Vector2D(0,40);
			//test.Current.Velocity.Angular  = -.5f;
			world.AddICollidableBody(test);


            RigidBody test3 = new RigidBody(MassInertia.FromSquare(300, 100), (float)(float)Math.PI / 4, new Vector2D(485, 200), (Polygon2D)Polygon2D.FromSquare(50));
			test3.Current.Velocity.Linear = new Vector2D(0,-40);
			world.AddICollidableBody(test3);
		}
		public static  void SetWalls(Physics2D.World2D world)
		{
			world.CalcGravity = false;
            RigidBody test = new RigidBody(MassInertia.FromRectangle(float.PositiveInfinity, 50, 500), 0, new Vector2D(700, 0), (Polygon2D)Polygon2D.FromRectangle(50, 1000));
			test.Flags = BodyFlags.InfiniteMass;
			
			//test.Current.Velocity.Linear = new Vector2D(0,-40);
			world.AddICollidableBody(test);

            RigidBody test2 = new RigidBody(MassInertia.FromRectangle(float.PositiveInfinity, 50, 500), 0, new Vector2D(700, 1100), (Polygon2D)Polygon2D.FromRectangle(50, 1000));
			test2.Flags = BodyFlags.InfiniteMass;
			//test2.Current.Velocity.Linear = new Vector2D(1,40);
			world.AddICollidableBody(test2);
            RigidBody test3 = new RigidBody(MassInertia.FromRectangle(float.PositiveInfinity, 500, 50), 0, new Vector2D(0, 0), (Polygon2D)Polygon2D.FromRectangle(1000, 50));
			test3.Flags = BodyFlags.InfiniteMass;
			
			//test3.Current.Velocity.Linear = new Vector2D(0,-40);C:\programming\MyGame\Physics2D\Geometry\Circle.cs
			world.AddICollidableBody(test3);

            RigidBody test4 = new RigidBody(MassInertia.FromRectangle(float.PositiveInfinity, 500, 50), 0, new Vector2D(1100, 700), (Polygon2D)Polygon2D.FromRectangle(1000, 50));
			test4.Flags = BodyFlags.InfiniteMass;
			
			//test4.Current.Velocity.Linear = new Vector2D(1,40);
			world.AddICollidableBody(test4);
		}
		
		public static  void SetTriangleTest(Physics2D.World2D world)
		{
			world.CalcGravity = false;
			RigidBody test = new RigidBody(MassInertia.FromSolidCylinder(400,50), 0, new Vector2D(900,900), new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 3,50)));
			//test.Current.Velocity.Linear = new Vector2D(-40,0);
			world.AddICollidableBody(test);

		}
		public static  void SetPolygon2DTest(Physics2D.World2D world)
		{
			Vector2D[] dud = new Vector2D[]{
											
											 
											 new Vector2D(20,20),
											 new Vector2D(-20,20),
											 new Vector2D(-20,-20),
											 new Vector2D(20,-20),
											 
											new Vector2D(40,0)
										 };
			Polygon2D bob = new Polygon2D(dud);
			RigidBody test = new RigidBody(MassInertia.FromRectangle(900,40,40), 0, new Vector2D(600,800), bob);
			//test.Current.Velocity.Linear = new Vector2D(-40,0);
			world.AddICollidableBody(test);

		}
		public static  void SetOneRectagle(Physics2D.World2D world)
		{


            RigidBody test2 = new RigidBody(MassInertia.FromRectangle(500, 45, 45), (float)(float)Math.PI / 4, new Vector2D(1000, 1000), (Polygon2D)Polygon2D.FromRectangle(30, 30));
			//test2.Current.Velocity.Linear = new Vector2D(0,40);
			test2.Current.Velocity.Angular = 30;
			world.AddICollidableBody(test2);
		}

		public static  void SetCircleTest(Physics2D.World2D world)
		{																													
			world.CalcGravity = false;																														
																																								
			RigidBody test = new RigidBody(MassInertia.FromSolidCylinder(50,20), 2, new Vector2D(650,300), new Circle2D(20));	
			test.Current.Velocity.Linear = new Vector2D(-120,0);																																							
			//test.Current.Velocity.Angular = -1;																														
			world.AddICollidableBody(test);
			RigidBody test2 = new RigidBody(MassInertia.FromSolidCylinder(50,20), 0, new Vector2D(550,300), new Circle2D(20));
			world.AddICollidableBody(test2);
			RigidBody test3 = new RigidBody(MassInertia.FromSolidCylinder(50,20), 0, new Vector2D(510,300), new Circle2D(20));
			world.AddICollidableBody(test3);																													
		}

		public static  void SetTest6(Physics2D.World2D world)
		{
			world.CalcGravity = false;
			
			RigidBody test = new RigidBody(MassInertia.FromCylindricalShell(50,20), (float)(float)Math.PI/4, new Vector2D(650,500), new Circle2D(20));
			//test.Current.Velocity.Linear = new Vector2D(-120,0);
			world.AddICollidableBody(test);
			RigidBody test2 = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity,20), 0, new Vector2D(250,500), new Circle2D(20));
			world.AddICollidableBody(test2);
			RigidBody test3 = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity,20), 0, new Vector2D(800,500), new Circle2D(20));
			world.AddICollidableBody(test3);
			for(int pos = 0; pos < 250;pos+=50)
			{
				RigidBody body = new RigidBody(MassInertia.FromCylindricalShell(50,20), 0, new Vector2D(300+pos,500), new Circle2D(20) );
				world.AddICollidableBody(body);
			}
			
		}

		public static  void SetSpecialBalls(Physics2D.World2D world)
		{
			world.CalcGravity = false;
			for(int pos0 = 0; pos0 <400;pos0+=100)
			{
				RigidBody test = new RigidBody(MassInertia.FromCylindricalShell(50,20), (float)(float)Math.PI/4, new Vector2D(750,500-pos0), new Circle2D(20));
				test.Current.Velocity.Linear = new Vector2D(-120,0);
				test.Coefficients.Restitution = 1;
				world.AddICollidableBody(test);
				RigidBody test2 = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity,20), 0, new Vector2D(250,500-pos0), new Circle2D(20));
				test2.Flags = BodyFlags.InfiniteMass|BodyFlags.IgnoreGravity;
				test2.Coefficients.Restitution = 1;
				world.AddICollidableBody(test2);
				RigidBody test3 = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity,20), 0, new Vector2D(800,500-pos0), new Circle2D(20));
				test3.Flags = BodyFlags.InfiniteMass|BodyFlags.IgnoreGravity;
				test3.Coefficients.Restitution = 1;
				world.AddICollidableBody(test3);
				for(int pos = 300; pos <= 650;pos+=40+pos0/6)
				{
					RigidBody body = new RigidBody(MassInertia.FromCylindricalShell(50,20), 0, new Vector2D(50+pos,500-pos0), new Circle2D(20) );
					body.Coefficients.Restitution = 1;
					world.AddICollidableBody(body);
				}
			}
		}
		public static  void SetBoxCage(Physics2D.World2D world)
		{
			//world.CalcGravity = false;
			int minx = -5000;
			int maxx = 5000;
			int miny = -5000;
			int maxy = 5000;
			
			
			for(int pos = minx; pos <= maxx;pos+=200)
			{
				for(int pos0 = miny ; pos0 <=maxy;pos0+=200)
				{
					bool x = pos==minx||pos==maxx;
					bool y = pos0==miny ||pos0==maxy;
					if(x||y)
					{
                        RigidBody body = new RigidBody(MassInertia.FromSquare(float.PositiveInfinity, 150), 0, new Vector2D(pos, pos0), (Polygon2D)Polygon2D.FromSquare(150));
						body.Flags = BodyFlags.InfiniteMass|BodyFlags.IgnoreGravity|BodyFlags.NoUpdate;
						world.AddICollidableBody(body);
					}
				}
			}
		}
		public static  void SetCage(Physics2D.World2D world)
		{
			world.CalcGravity = false;
			for(int pos0 = 0; pos0 <=1000;pos0+=50)
			{
			
				for(int pos = 0; pos <= 1000;pos+=50)
				{
					bool x = pos==0||pos==1000;
					bool y = pos0==0||pos0==1000;
					if(x||y)
					{
						RigidBody body = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity,20), 0, new Vector2D(pos,pos0), new Circle2D(20) );
						body.Flags = BodyFlags.InfiniteMass;						
						world.AddICollidableBody(body);
					}
				}
			}
		}

		public static  void SetSol(Physics2D.World2D world)
		{
			Vector2D accel= Vector2D.Origin;
			float gravity = 9.8f;
			world.AddIGravitySource(new StaticGravityPoint(accel,gravity));
			world.CalcGravity = true;
			RigidBody mars = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity, 15), 0, new Vector2D(0,2000), new Circle2D(42));
			mars.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-mars.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-mars.Current.Position.Linear).Magnitude)).RightHandNormal;
			mars.Flags = BodyFlags.InfiniteMass;
			world.AddICollidableBody(mars);
			RigidBody earth = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity, 20), 0, new Vector2D(0,1400), new Circle2D(50));
			earth.Flags = BodyFlags.InfiniteMass;
			
			earth.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-earth.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-earth.Current.Position.Linear).Magnitude)).RightHandNormal;
			world.AddICollidableBody(earth);
			RigidBody venus = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity, 18), 0, new Vector2D(0,950), new Circle2D(44));
			venus.Flags = BodyFlags.InfiniteMass;
			venus.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-venus.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-venus.Current.Position.Linear).Magnitude)).RightHandNormal;
			world.AddICollidableBody(venus);
			RigidBody mercury = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity, 5), 0, new Vector2D(0,650), new Circle2D(15));
			mercury.Flags = BodyFlags.InfiniteMass;
			mercury.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-mercury.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-mercury.Current.Position.Linear).Magnitude)).RightHandNormal;
			world.AddICollidableBody(mercury);
			RigidBody sun = new RigidBody(MassInertia.FromCylindricalShell(float.PositiveInfinity, 75), 0, new Vector2D(0,0), new Circle2D(100));
			sun.Flags = BodyFlags.InfiniteMass;
			world.AddICollidableBody(sun);
			for(float angle = 0; angle <= (float)Math.PI*2;angle+=(float).08)
			{
				int r = rand.Next(4000);
				RigidBody body = new RigidBody(MassInertia.FromSolidCylinder(350,30), 0, Vector2D.FromLengthAndAngle(r+5000,angle), new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( rand.Next(5,9),30)));
				
				//RigidBody collidable = new RigidBody(new Circle(lightninglenght),MassInertia.FromSolidCylinder(20+lightninglenght-10,lightninglenght),(float)r/2,new Vector2D(point+rand.Next(20),pos2+rand.Next(20)));
				body.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-body.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-body.Current.Position.Linear).Magnitude)).RightHandNormal;
				//collidable.Current.Velocity.Angular = .3;
				if((accel-body.Current.Position.Linear).Magnitude>500)
				{											
					world.AddICollidableBody(body);
				}
			}
			
			for(int pos = -2500; pos <= 2500;pos+=700)
			{
				
				for(int pos2 = -2500; pos2 <= 2500;pos2+=700)
				{
					int r = rand.Next(5);
					int width = rand.Next(7)+7;
					int lenght = rand.Next(7)+7;
                    RigidBody body = new RigidBody(MassInertia.FromRectangle(20 + width, lenght, width), ((float)(r % 10)) / 10, new Vector2D(pos + rand.Next(350), pos2 + rand.Next(350)), (Polygon2D)Polygon2D.FromRectangle(lenght, width));
					
					//RigidBody collidable = new RigidBody(new Circle(lightninglenght),MassInertia.FromSolidCylinder(20+lightninglenght-10,lightninglenght),(float)r/2,new Vector2D(point+rand.Next(20),pos2+rand.Next(20)));
					body.Current.Velocity.Linear = Vector2D.SetMagnitude(accel-body.Current.Position.Linear,(float)(float)Math.Sqrt(2*(gravity*0.63661977236758134307553505349036)*(accel-body.Current.Position.Linear).Magnitude)).RightHandNormal;
					//collidable.Current.Velocity.Angular = .3;
					if((accel-body.Current.Position.Linear).Magnitude>1000)
					{											
						world.AddICollidableBody(body);
					}
				}
			}
		}
        public static Vector2D GetOrbitVelocity(Vector2D PosOfAccelPoint, Vector2D PosofShip, float AccelDoToGravity)
		{
			float MyOrbitConstant = 0.63661977236758134307553505349036f; //(area under 1/4 of a sin wave)/(PI/2)

			Vector2D distance = PosOfAccelPoint - PosofShip;
			float distanceMag = distance.Magnitude;
			Vector2D distanceNorm = distance*(1/distanceMag);

			float VelocityForOrbit = (float)(float)Math.Sqrt(2*MyOrbitConstant*AccelDoToGravity*distanceMag);
			Vector2D orbitvelocity = distanceNorm.RightHandNormal*VelocityForOrbit;
			return orbitvelocity;
		}
		public static  void SetEarthMoon(Physics2D.World2D world)
		{
			world.CalcGravity = true;
			//RigidBody earth = new RigidBody(new Circle(6371.01f),MassInertia.FromSolidCylinder(float.Parse("5.9736e24"),6371.01f),0,new Vector2D(-6000,-6000));
			RigidBody earth = new RigidBody(MassInertia.FromSolidCylinder(5.9736e26f,6371.01f), 0, new Vector2D(-6000,0), new Circle2D(6371.01f));
			//earth.Flags = BodyFlags.GravityWell;
            earth.Current.Velocity.Angular = 0.011574074074074074074074074074074f; ;//0.000011574074074074074074074074074074f;
			world.AddICollidableBody(earth);
			world.AddIGravitySource(earth);
			RigidBody luna = new RigidBody(MassInertia.FromSolidCylinder(7.349e24f,1737.4f), 0, new Vector2D(384403 ,0), new Circle2D(1737.4f));
			luna.Current.Velocity.Linear = new Vector2D(0,1.022f);
			//luna.Flags = BodyFlags.GravityWell;
			world.AddICollidableBody(luna);
			world.AddIGravitySource(luna);
		}
		public static  void SetRealisticSol(Physics2D.World2D world)
		{
			world.CalcGravity = true;
			//RigidBody earth = new RigidBody(new Circle(6371.01f),MassInertia.FromSolidCylinder(float.Parse("5.9736e24"),6371.01f),0,new Vector2D(-6000,-6000));
			
			
			Vector2D EarthsDistanceFromSun = new Vector2D(152097701,0);
			Vector2D MoonsDistanceFromEarth = new Vector2D(384403,0);
			StartingLocation = EarthsDistanceFromSun+new Vector2D(7000,0);
			Vector2D EarthsOrbitalVelocity = new Vector2D(0,29.783f);
			Vector2D MoonsOrbitalVelocity = new Vector2D(0,1.022f);
			
			

			StartingVelocity = EarthsOrbitalVelocity+new Vector2D(0,40);

			RigidBody earth = new RigidBody(MassInertia.FromSolidCylinder(float.Parse("5.9736e24"),6371.01f), 0, EarthsDistanceFromSun, new Circle2D(6371.01f));
			earth.Current.Velocity.Linear = EarthsOrbitalVelocity;
			earth.Current.Velocity.Angular =  0.000011574074074074074074074074074074f;
			earth.Flags = BodyFlags.GravityWell;
			
			world.AddICollidableBody(earth);
			world.AddIGravitySource(earth);
			RigidBody luna = new RigidBody(MassInertia.FromSolidCylinder(float.Parse("7.349e22"),1737.4f), 0, EarthsDistanceFromSun+MoonsDistanceFromEarth, new Circle2D(1737.4f));
			luna.Current.Velocity.Linear = EarthsOrbitalVelocity + MoonsOrbitalVelocity;
			luna.Flags = BodyFlags.GravityWell;
			world.AddICollidableBody(luna);
			world.AddIGravitySource(luna);
			RigidBody sun = new RigidBody(MassInertia.FromSolidCylinder(float.Parse("1.9891e30"),696000), 0, new Vector2D(0,0), new Circle2D(696000));
			sun.Flags = BodyFlags.GravityWell;			
			world.AddICollidableBody(sun);
			world.AddIGravitySource(sun);
			StartingVelocity += Vector2D.SetMagnitude(EarthsDistanceFromSun-StartingLocation,(float)(float)Math.Sqrt(2*(world.Gravity.GetGravityPullAt(StartingLocation).Magnitude*0.63661977236758134307553505349036)*(EarthsDistanceFromSun-StartingLocation).Magnitude)).RightHandNormal;
		}

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

                if(!info.AtContactStage&&info.Step == 0)
                {
                    
				//if(collidables.Collidable1.Geometry.UseCircleCollision||collidables.Collidable2.Geometry.UseCircleCollision)
				//{
                    if (!(info.ICollidableBody1 is ImpulseWave) || !(info.ICollidableBody2 is ImpulseWave))
					{
						//if()
						//{
							World2D world = sender as World2D;
							PhysicsState bob;// 
                            if (info.ICollidableBody1.CollisionState.GenerateCollisionEvents)
						{
                            bob = info.ICollidableBody1After;// new PhysicsState(collidables.Collidable1.Current);
						}
						else
						{
                            bob = info.ICollidableBody2After;// new PhysicsState(collidables.Collidable2.Current);
						}

						
						//rand.Position.Linear += collidables.Distance*.5f;
                        if (!info.ICollidableBody1.LifeTime.IsExpired)
                        {
                            if (info.ICollidableBody1.LifeTime is Mortal)
                            {
                                if (!(info.ICollidableBody1 is ImpulseWave))
                                {
                                    world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.ICollidableBody1.MassInertia.Mass, bob, info.ICollidableBody1.BoundingRadius, 5500));
                                    info.ICollidableBody1.LifeTime.IsExpired = true;
                                }
                            }
                        }
                        if (!info.ICollidableBody2.LifeTime.IsExpired)
                        {
                            if (info.ICollidableBody2.LifeTime is Mortal)
							{
                                if (!(info.ICollidableBody2 is ImpulseWave))
								{
                                    world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.ICollidableBody2.MassInertia.Mass, bob, info.ICollidableBody2.BoundingRadius, 5500));
                                    info.ICollidableBody2.LifeTime.IsExpired = true;
								}
							}
						}
					}
				}
			}
        }

        #endregion
        #region newer stuff
        public OldDirectXTests():base()
        {

        }
        public override void InitObjects()
        {
            
            base.InitObjects();
        }
        public override void AddObjects()
        {
            OldDirectXTests.SetEvents(world);

            OldDirectXTests.Set(world);
            this.InitialShipState = OldDirectXTests.staticInitialShipState;
            this.mainship = CreateUserRigidBody(world);
            base.AddObjects();
        }
        public override string Name
        {
            get
            {
                return "Old Direct X Tests";
            }
        }
        public override string Description
        {
            get
            {
                return "All the Tests from the the old DirectXTest Driver. (hardcoded configurations)" ;
            }
        }
        public override IDemo CreateNew()
        {
            return new OldDirectXTests();
        }
        #endregion
    }
}
