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
//using Physics2D.Collision;
using Physics2D.CollidableBodies;
using Physics2D.Joints;
using AdvanceMath.Geometry2D;
using AdvanceSystem;
using AdvanceMath;
using global::SdlDotNet;
namespace WindowsDriver.Demos
{



	public class RandomGravity : IGravitySource
	{
		#region IGravitySource Members
		Random rand;

		public int changemax = 50;
		public int changemin = -50;
        public Bounded<float> xPull = new Bounded<float>(-200, 0, 200, false);
        public Bounded<float> yPull = new Bounded<float>(-300, 0, 300, false);

		public RandomGravity()
		{
			rand = new Random(DateTime.Now.Millisecond);

		}

		public Vector2D GetGravityPullAt(Vector2D position)
		{
			// TODO:  Add RandomGravity.GetGravityPullAt implementation
			yPull.Value += rand.Next(changemin,changemax+1);
            xPull.Value += rand.Next(changemin, changemax + 1) / 2;


			return new Vector2D (xPull,yPull);
		}

		#endregion
        bool isExpired;
        public bool IsExpired
        {
            get
            {
                return isExpired;
            }
            set
            {
                isExpired = value;
            }
        }
	}

    public class Pong : BaseDisplayDemo
	{
		//World2D world;
		RigidBody player1;
		RigidBody player2;
		RigidBody UVWall;
		RigidBody LVWall;
		RigidBody ball;
        bool reseted = false;
		RandomGravity gravity;
		float platformForce = 20000000;
		float timesince;
		private Polygon2D MakePaddle()
		{
			int sides = 6;
			Polygon2D paddle = new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( sides,200));
			int maxsize = 100;
			Vector2D[] verticies = new Vector2D[sides];
			float max = 0;
			foreach(Vertex2D vect in paddle.Vertices)
			{
				max = (float)Math.Max(max,vect.OriginalPosition.Y);
			}
			float scale = maxsize/max;

			for(int pos = 0; pos != sides; ++pos)
			{
				Vector2D tmp = paddle.Vertices[pos].OriginalPosition;
				tmp.Y *= scale;
				verticies[pos] = tmp;
			}
			return new Polygon2D(verticies);

		}
		public Pong()
		{
			
		}
        public override void InitObjects()
        {
            Init();
        }
		private void Init()
		{
            this.timesince = 0;
            this.player1 = null;
            this.player2 = null;
            this.UVWall = null;
            this.LVWall = null;
            this.ball = null;
            this.world = null;
            this.gravity = null;



			world = new World2D();
			gravity = new RandomGravity();
			world.AddIGravitySource( gravity);
			//world.CalcGravity = true;
			//Polygon2D paddle = MakePaddle();
            Polygon2D paddle = (Polygon2D)Polygon2D.FromRectangle(150, 400);
			MassInertia PaddleMI = new MassInertia(6000,float.PositiveInfinity);
			PhysicsState state = new PhysicsState();
			state.Position.Linear = new Vector2D(0,801);
			player1 = new RigidBody(PaddleMI, state, paddle);
			player1.Flags = player1.Flags |BodyFlags.IgnoreGravity;
			
			state.Position.Linear = new Vector2D(0,-801);
			player2 = new RigidBody(PaddleMI, state, paddle);

			player2.Flags = player2.Flags |BodyFlags.IgnoreGravity;

			world.AddICollidableBody(player1);
			world.AddICollidableBody(player2);
			TrackedAnchor trackedAnchor1 = new TrackedAnchor(player1,player1.Current.Position.Linear,new Vector2D(0,1),.1f,.2f);
			TrackedAnchor trackedAnchor2 = new TrackedAnchor(player2,player2.Current.Position.Linear,new Vector2D(0,1),.1f,.2f);
			world.AddIJoint(trackedAnchor1);
			world.AddIJoint(trackedAnchor2);

            Polygon2D HWAllRect = (Polygon2D)Polygon2D.FromRectangle(2000, 2200);
			MassInertia WallMI = new MassInertia(float.PositiveInfinity,float.PositiveInfinity);
			state.Position.Linear = new Vector2D(2300,0);
			RigidBody RHWall = new RigidBody(WallMI, state, HWAllRect);
			RHWall.Flags = RHWall.Flags |BodyFlags.IgnoreGravity;

			world.AddICollidableBody(RHWall);
			state.Position.Linear = new Vector2D(-2300,0);
			RigidBody LHWall = new RigidBody(WallMI, state, HWAllRect);
			LHWall.Flags = LHWall.Flags |BodyFlags.IgnoreGravity;


			world.AddICollidableBody(LHWall);


            Polygon2D VWAllRect = (Polygon2D)Polygon2D.FromRectangle(200, 2600);


			state.Position.Linear = new Vector2D(0,-900);
			UVWall = new RigidBody(WallMI, state, VWAllRect);

			UVWall.Flags = UVWall.Flags |BodyFlags.IgnoreGravity;


			world.AddICollidableBody(UVWall);
			state.Position.Linear = new Vector2D(0,900);
			LVWall = new RigidBody(WallMI, state, VWAllRect);

			LVWall.Flags = LVWall.Flags |BodyFlags.IgnoreGravity;

			world.AddICollidableBody(LVWall);


			PhysicsState ballstate = new PhysicsState();
			Random rand = new Random();
			ballstate.Velocity.Linear = (float)Math.Sign(rand.Next(0,2)-.5f)*Vector2D.FromLengthAndAngle(900,(float)rand.NextDouble()*(float)Math.PI*.5f+(float)Math.PI*.25f);// new Vector2D(rand.Next(-200,200),rand.Next(-900,900));

			ball = new RigidBody(MassInertia.FromSolidCylinder(20,50), ballstate, new Circle2D(50));
			//ball = new RigidBody(new Circle(50),new MassInertia(2000,float.PositiveInfinity,Vector2D.Zero),ballstate);
			ball.CollisionState.GenerateCollisionEvents = true;
			
			world.AddICollidableBody(ball);
			
			//UVWall.Coefficients.Restitution = 0;			
			//LVWall.Coefficients.Restitution = 0;

			//LHWall.Coefficients.Restitution = 1.0;
			//RHWall.Coefficients.Restitution = 1.0;


			ball.Coefficients.Restitution = 1;
			//ball.CoefficientOfStaticFriction = -.1f;
			//ball.CoefficientOfDynamicFriction  = -.1f;

		
			player1.Coefficients.Restitution = 1;
			//player1.CoefficientOfStaticFriction = 1.01f;
			//player1.CoefficientOfDynamicFriction  = 1.01f;
			player2.Coefficients.Restitution = 1;
			//player2.CoefficientOfStaticFriction = 1.01f;
			//player2.CoefficientOfDynamicFriction  = 1.01f;
			world.Collision +=new CollisionEventDelegate(world_Collision);
			timesince = 0;
			float x = ball.Current.Velocity.Linear.X;
			this.player1.Current.Velocity.Linear.X = x;
			this.player2.Current.Velocity.Linear.X = x;

		}
        public override bool Update(float dt)
		{
            reseted = false;

			if(timesince>1)
			{
				this.gravity.xPull.Binder.Upper += 10;
                this.gravity.xPull.Binder.Lower -= 10;
                this.gravity.yPull.Binder.Upper += 5;
                this.gravity.yPull.Binder.Lower -= 5;
				this.gravity.changemin -= 3;
				this.gravity.changemax += 3;
				timesince = 0;
			}
			//ball.Current.Velocity.Linear.Magnitude = (dt*1.1*ball.Current.Velocity.Linear.Magnitude );
		

			if(dt != 0)
			{
				float y = ball.Current.Velocity.Linear.Y;
				//ball.Current.Velocity.Linear.Y += (dt*.5f*y );
			}
			timesince += dt;
			world.Update(dt);
            return reseted;
		}
        public override void UpdateKeyBoard(KeyboardState keys, float dt)
		{
			if (keys[Key.A])
			{
				player1.ApplyForce(new ForceInfo(new Vector2D(-platformForce,0)));
			}
			if (keys[Key.D])
			{
				player1.ApplyForce(new ForceInfo(new Vector2D(platformForce,0)));
			}
			if (keys[Key.S])
			{
				player1.Current.Velocity.Linear = Vector2D.Zero;// .ApplyForce(new ForceInfo(true,new Vector2D(-Settings.ShipForce,0)));
			}
			if (keys[Key.J])
			{
				player2.ApplyForce(new ForceInfo(new Vector2D(-platformForce,0)));
			}
			if (keys[Key.L])
			{
				player2.ApplyForce(new ForceInfo(new Vector2D(platformForce,0)));
			}
			if (keys[Key.K])
			{
				player2.Current.Velocity.Linear = Vector2D.Zero;// .ApplyForce(new ForceInfo(true,new Vector2D(-Settings.ShipForce,0)));
			}
		}

		private void world_Collision(object sender, CollisionEventArgs args)
		{
            foreach (Physics2D.InterferenceInfo ginfo in args.InterferenceInfos)
            {
                if (ginfo.InterferenceType != InterferenceType.CollidablePair)
                {
                    continue;
                }
                CollidablePairInterferenceInfo info = ginfo.CollidablePairInfo;

                if (info.ICollidableBody1 == this.player1 || info.ICollidableBody1 == this.player2 || info.ICollidableBody2 == this.player1 || info.ICollidableBody2 == this.player2)
				{
					this.gravity.yPull.Value = 0;
					this.gravity.xPull.Value = 0;
				}
                else if (info.ICollidableBody1 == this.LVWall || info.ICollidableBody2 == this.LVWall)
				{
                    world.Enabled = false;
					System.Windows.Forms.MessageBox.Show("Bottom Player you Suck!");
                    world.Collidables.Clear();
                    InitObjects();
                    world.Enabled = true;
                    reseted = true;
				}
                else if (info.ICollidableBody1 == this.UVWall || info.ICollidableBody2 == this.UVWall)
				{
                    world.Enabled = false;
					System.Windows.Forms.MessageBox.Show("Top Player you Suck!");
                    world.Collidables.Clear();
                   
                    InitObjects();
                    world.Enabled = true;
                    reseted = true;

				}
			}
		}
        public override float Scale
		{
			get
			{
				return .3f;
			}
		}
        public override Vector2D CameraPosition
		{
			get
			{
				return Vector2D.Zero;				
			}
		}
        public override string Name
        {
            get
            {
                return "Pong";
            }
        }
        public override string Description
        {
            get
            {
                return "A Simple Recreation of Pong";
            }
        }
        public override string Instructions
        {
            get
            {
                return
                    "top player:\n" +
                    "moveleft == j\n" +
                    "moveright == l\n" +
                    "stop == k\n\n" +
                    "bottom player:\n" +
                    "moveleft == a\n" +
                    "moveright == d\n" +
                    "stop == s\n";

            }
        }
        public override IDemo CreateNew()
        {
            return new Pong();
        }
	}
}
