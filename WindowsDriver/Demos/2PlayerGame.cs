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
using Physics2D.Joints;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using System.Drawing;

using global::SdlDotNet;
namespace WindowsDriver.Demos
{
	/// <summary>
	/// Summary description for BasicCollisions.
	/// </summary>
    public class TwoPlayerGame : BaseDisplayDemo
	{
		public static ArrayList SetHumanRigidBodyV2(Physics2D.World2D world, Vector2D pos )
		{

			float breakDistance = 100;
            float breakVelocity = float.PositiveInfinity;
            float breakImpulse = 400000;//float.PositiveInfinity;

			float timescale = .1f;

			RigidBody Head = new RigidBody(MassInertia.FromSolidCylinder(120,20), 0, new Vector2D(0,0)+pos, new Circle2D(30));
            RigidBody Torso = new RigidBody(MassInertia.FromRectangle(2000, 150, 80), 0, new Vector2D(0, 108) + pos, (Polygon2D)Polygon2D.FromRectangle(150, 80));
			PinJoint Neck = new PinJoint(new CollidablePair(Head,Torso),new Vector2D(20,30)+pos,3,timescale,breakDistance,breakVelocity,breakImpulse);
			PinJoint Neck2 = new PinJoint(new CollidablePair(Head,Torso),new Vector2D(-20,30)+pos,3,timescale,breakDistance,breakVelocity,breakImpulse);

            RigidBody LArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(-60, 80) + pos, (Polygon2D)Polygon2D.FromRectangle(75, 30));			
			PinJoint LSholder = new PinJoint(new CollidablePair(Torso,LArm),new Vector2D(-60,40)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LLArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(-60, 155) + pos, (Polygon2D)Polygon2D.FromRectangle(60, 27));			
			PinJoint LElbow= new PinJoint(new CollidablePair(LArm,LLArm),new Vector2D(-60,115)+pos,1.5f,timescale,breakDistance,breakVelocity,breakImpulse);

            RigidBody RArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(60, 80) + pos, (Polygon2D)Polygon2D.FromRectangle(75, 30));			
			PinJoint RSholder = new PinJoint(new CollidablePair(Torso,RArm),new Vector2D(60,40)+pos,2,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody RLArm = new RigidBody(MassInertia.FromRectangle(200, 75, 30), 0, new Vector2D(60, 155) + pos, (Polygon2D)Polygon2D.FromRectangle(60, 27));			
			PinJoint RElbow = new PinJoint(new CollidablePair(RArm,RLArm),new Vector2D(60,115)+pos,1.5f,timescale,breakDistance,breakVelocity,breakImpulse);

            RigidBody RLeg = new RigidBody(MassInertia.FromRectangle(500, 250, 80), 0, new Vector2D(25, 230) + pos, (Polygon2D)Polygon2D.FromRectangle(80, 40));			
			PinJoint RHip = new PinJoint(new CollidablePair(Torso,RLeg),new Vector2D(25,190)+pos,1,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LLeg = new RigidBody(MassInertia.FromRectangle(500, 250, 80), 0, new Vector2D(-25, 230) + pos, (Polygon2D)Polygon2D.FromRectangle(80, 40));			
			PinJoint LHip = new PinJoint(new CollidablePair(Torso,LLeg),new Vector2D(-25,190)+pos,3,timescale,breakDistance,breakVelocity,breakImpulse);


            RigidBody LRLeg = new RigidBody(MassInertia.FromRectangle(400, 250, 80), 0, new Vector2D(25, 310) + pos, (Polygon2D)Polygon2D.FromRectangle(80, 40));			
			PinJoint RKnee = new PinJoint(new CollidablePair(LRLeg,RLeg),new Vector2D(25,270)+pos,1,timescale,breakDistance,breakVelocity,breakImpulse);
            RigidBody LLLeg = new RigidBody(MassInertia.FromRectangle(400, 250, 80), 0, new Vector2D(-25, 310) + pos, (Polygon2D)Polygon2D.FromRectangle(80, 40));			
			PinJoint LKnee = new PinJoint(new CollidablePair(LLLeg,LLeg),new Vector2D(-25,270)+pos,3,timescale,breakDistance,breakVelocity,breakImpulse);

			ArrayList list = new ArrayList();

			world.AddIJoint(Neck);
			world.AddIJoint(Neck2);
			
			list.Add(Torso);
			//world.AddICollidableBody(Torso);
			list.Add(Head);
			//world.AddICollidableBody(Head);
			
			



			list.Add(RLeg);
			//world.AddICollidableBody(RLeg);
			world.AddIJoint(RHip);
			list.Add(LLeg);

			//world.AddICollidableBody(LLeg);
			world.AddIJoint(LHip);
			list.Add(LRLeg);
			//world.AddICollidableBody(LRLeg);
			world.AddIJoint(RKnee);
			//world.AddICollidableBody(LLLeg);
			list.Add(LLLeg);
			world.AddIJoint(LKnee);


			list.Add(LArm);

			//world.AddICollidableBody(LArm);
			world.AddIJoint(LSholder);
			list.Add(LLArm);
			//world.AddICollidableBody(LLArm);
			world.AddIJoint(LElbow);
			list.Add(RArm);
			
			//world.AddICollidableBody(RArm);
			
			
			world.AddIJoint(RSholder);
			list.Add(RLArm);
			//world.AddICollidableBody(RLArm);
			world.AddIJoint(RElbow);
			return list;
		}

		public  void SetBoxCage(Physics2D.World2D world)
		{
			//world.CalcGravity = false;
			int minx = -1700;
			int maxx = 1700;
			int miny = -1400;
			int maxy = 1300;
			int squaresize = 50;
			
			float sign = -1;
			for(int pos = minx; pos <= maxx;pos+=squaresize*2)
			{
				for(int pos0 = miny ; pos0 <=maxy;pos0+=squaresize*2)
				{
					bool x = pos==minx||pos==maxx;
					bool y = pos0==miny ||pos0==maxy;
					if(x||y)
					{			
						sign = -sign;	
						if(rand.Next(2)==0)
						{
							sign = -sign;
						}
                        RigidBody body = new RigidBody(MassInertia.FromSquare(float.PositiveInfinity, squaresize), 0, new Vector2D(pos, pos0), (Polygon2D)Polygon2D.FromSquare(squaresize));
						body.Current.Velocity.Angular = sign*90;
						body.Flags = BodyFlags.InfiniteMass|BodyFlags.IgnoreGravity|BodyFlags.NoUpdate;
						world.AddICollidableBody(body);
					}
				}
			}
		}
		public static void SetCircleBoxCage(Physics2D.World2D world)
		{
			//world.CalcGravity = false;
			/*int minx = -1300;
			int maxx = 1300;
			int miny = -1300;
			int maxy = 1300;*/
			float BoxSideSize = 50;
			
			float radius = 1500;

			float circ = 2*(float)Math.PI*radius;
			float NumberofBoxes = circ/(BoxSideSize*2);
			float Spacing = (NumberofBoxes/(circ/((float)Math.PI*2)));

			float sign = -1;
			for(float ra = 0; ra<= 2*(float)Math.PI;ra+=Spacing)
			{
				sign = -sign;
                RigidBody body = new RigidBody(MassInertia.FromSquare(float.PositiveInfinity, BoxSideSize), ra, Vector2D.FromLengthAndAngle(radius, ra), (Polygon2D)Polygon2D.FromSquare(BoxSideSize));
				body.Current.Velocity.Angular = sign*90;
				body.Flags = BodyFlags.InfiniteMass|BodyFlags.IgnoreGravity|BodyFlags.NoUpdate;
				world.AddICollidableBody(body);
			}
			
			
			/*for(int point = minx; point <= maxx;point+=100)
			{
				for(int pos0 = miny ; pos0 <=maxy;pos0+=100)
				{
					bool x = point==minx||point==maxx;
					bool y = pos0==miny ||pos0==maxy;
					if(x||y)
					{																							
						RigidBody collidable = new RigidBody(new Square(50) ,MassInertia.FromSquare(float.PositiveInfinity,40),0,new Vector2D(point,pos0));
						collidable.Current.Velocity.Angular = 110;
						collidable.Flags = BodyFlags.InfiniteMass|BodyFlags.IgnoreGravity|BodyFlags.NoUpdate;
						world.AddICollidableBody(collidable);
					}
				}
			}*/
		}
		public static Vector2D StartingVelocity = Vector2D.Zero;
		public static Vector2D StartingLocation = Vector2D.Zero;
		public static float StartingOrientation = 0;
		public static float ShipForce = 6000000;
		public static float ShipTorque = 200000000;
		public static int ProjectilesVelocity = 900;	
		float scale = .2f;
		public static RigidBody CreateUserRigidBody(Physics2D.World2D world)
		{
			Vector2D[] vertexlist = new Vector2D[6];
			vertexlist[0] = new Vector2D(50,30);
			vertexlist[1] = new Vector2D(-50,30);
			vertexlist[2] = new Vector2D(-50,-30);
			vertexlist[3] = new Vector2D(50,-30);
			vertexlist[4] = new Vector2D(60,-10);
			vertexlist[5] = new Vector2D(60,10);
			Polygon2D mainshape = new Polygon2D(vertexlist);
			//Circle mainshape = new Circle(50);
	
			RigidBody mainship = new RigidBody(MassInertia.FromRectangle(60000, 30,100), StartingOrientation, StartingLocation, mainshape);
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

		//World2D world;
		RigidBody player1;
		RigidBody arm1;
		RigidBody head1;
		RigidBody player2;
		RigidBody arm2;
		RigidBody head2;

		Random rand = new Random();
		public TwoPlayerGame()
		{
		}
        public override void InitObjects()
        {
            System.Windows.Forms.MessageBox.Show("The Objective of this game is to keep your head in the Arena longer then your opponent keeps his/hers. The Most advisable way of doing this would be to keep it firmly attached to your torso and make sure your opponent does the opposite. Also the \"Walls\" will tear you appart so try and stay away from them.");
            System.Windows.Forms.MessageBox.Show("The Controls are Simple \"P\" is Pause/UnPause. The movement controls for the left player are {\"A\",\"D\",\"S\",\"W\"} And the Controls for the Right Player are the arrow keys. The Ctrls closest to the cluster of moment keys is the player’s fire keys. The Fire key fires an explosive projective out of the arm that starts farthest away from the opponent.");
            INIT();
        }

		private void INIT()
		{
			world = new World2D();
			ArrayList parts1  = SetHumanRigidBodyV2(world,new Vector2D(300,0));//CreateUserRigidBody(world);
			player1 = (RigidBody)parts1[0];
			head1 = (RigidBody)parts1[1];
			arm1 = (RigidBody)parts1[parts1.Count-1];

			foreach(RigidBody part in parts1)
			{
				world.AddICollidableBody(part);
			}
			ArrayList parts2 = SetHumanRigidBodyV2(world,new Vector2D(-300,0));//CreateUserRigidBody(world);
			player2 = (RigidBody)parts2[0];
			head2 = (RigidBody)parts2[1];
			arm2 = (RigidBody)parts2[parts2.Count-3];

			
			foreach(RigidBody part in parts2)
			{
				world.AddICollidableBody(part);
			}
			//world.AddICollidableBody(player1);
			//world.AddICollidableBody(player2);
			if(rand.Next(2)==0)
			{
				SetBoxCage(world);
			}
			else
			{
				SetCircleBoxCage(world);
			}
			world.Collision+=new CollisionEventDelegate(world_Collision);
		}

        public override bool Update(float dt)
		{
			if(dt>.3f)
			{
				dt = .3f;
			}
			world.Update(dt);
			bool player1loses = head1.Good.Position.Linear.Magnitude >3000;
			bool player2loses = head2.Good.Position.Linear.Magnitude >3000;
			if(player1loses&&player2loses)
			{
				System.Windows.Forms.MessageBox.Show("YOU GUYS BOTH LOSE\n Press P to UnPause");
			}
			else if(player1loses)
			{
				System.Windows.Forms.MessageBox.Show("The Right Handed Player Loses\n Press P to UnPause");
			}
			else if(player2loses)
			{
				System.Windows.Forms.MessageBox.Show("The Left Handed Player Loses\n Press P to UnPause");
			}
			if(player1loses||player2loses)
			{
				this.INIT();
                this.world.Enabled = true;
                return true;
			}
            return false;

		
		}


        public override float Scale
		{
			get
			{
				return scale;
			}
		}
	
		DateTime lastFired  = DateTime.Now;
		DateTime lastFired2  = DateTime.Now;
		DateTime paused  = DateTime.Now;
		
		
		
		float radius = 20;
		float mass = 200;
		float lifetime = 3;
		float velocity = ProjectilesVelocity;
        public override void UpdateKeyBoard(KeyboardState keys, float dt)
        {
         
			DateTime now = DateTime.Now;
			
			

			TimeSpan timesincefired = now-lastFired;
			TimeSpan timesincefired2 = now-lastFired;
				

					
			if(keys[Key.LeftControl])
			{
				if(timesincefired.TotalMilliseconds>200)
				{
					lastFired = now;
					Vector2D position = this.arm2.Current.Position.Linear+Vector2D.FromLengthAndAngle(this.arm2.BoundingRadius+radius+10,arm2.Current.Position.Angular+(float)Math.PI*.5f);
					RigidBody projectile = new RigidBody(MassInertia.FromSolidCylinder(mass*3,radius), this.arm2.Current.Position.Angular, position, new Polygon2D(Polygon2D.FromNumberofSidesAndRadius( 7,radius)));
					Vector2D LV = Vector2D.FromLengthAndAngle(velocity,arm2.Current.Position.Angular+(float)Math.PI*.5f);
					projectile.Current.Velocity.Linear = arm2.Current.Velocity.Linear +  LV;
					projectile.LifeTime = new Mortal(lifetime);
						
					projectile.CollisionState.GenerateCollisionEvents = true;
						
					world.AddICollidableBody(projectile);
					arm2.Current.Velocity.Linear -= LV*(mass/arm2.MassInertia.Mass);
				}
			}
			if(keys[Key.RightControl])
			{
				if(timesincefired2.TotalMilliseconds>200)
				{
					lastFired = now;
					Vector2D position = this.arm1.Current.Position.Linear+Vector2D.FromLengthAndAngle(this.arm1.BoundingRadius+radius+10,arm1.Current.Position.Angular+(float)Math.PI*.5f);
                    RigidBody projectile = new RigidBody(MassInertia.FromSolidCylinder(mass * 3, radius), this.arm1.Current.Position.Angular, position, new Polygon2D(Polygon2D.FromNumberofSidesAndRadius(7, radius)));
					Vector2D LV = Vector2D.FromLengthAndAngle(velocity,arm1.Current.Position.Angular+(float)Math.PI*.5f);
					projectile.Current.Velocity.Linear = arm1.Current.Velocity.Linear +  LV;
					projectile.LifeTime = new Mortal(lifetime);
						
					projectile.CollisionState.GenerateCollisionEvents = true;
						
					world.AddICollidableBody(projectile);
					arm1.Current.Velocity.Linear -= LV*(mass/arm1.MassInertia.Mass);
				}
			}
			if(now .Subtract(keypressed).TotalSeconds<Physics2D.Physics.Tolerance)
			{
				return;
			}
			if (keys[Key.P])
			{
				if(now.Subtract(paused).TotalSeconds >.35)
				{
                    this.world.Enabled = !this.world.Enabled;
				}
				paused = now;
			}
			if (keys[Key.K])
			{
				foreach(IJoint joint in world.Joints)
				{
					joint.IsExpired = true;
				}
			} 

				
				
			if (keys[Key.W])
			{
				this.player2.ApplyForce(new ForceInfo(new Vector2D(0,-ShipForce),Vector2DTransformType.ToWorldAngular));
			}
			if (keys[Key.S])
			{
				this.player2.ApplyForce(new ForceInfo(new Vector2D(0,ShipForce),Vector2DTransformType.ToWorldAngular));
			}
			if (keys[Key.D])
			{
				this.player2.ApplyTorque(ShipTorque);
				
			} 
			else if (keys[Key.A])
			{
				this.player2.ApplyTorque(-ShipTorque);
			} 
			else 
			{
				float dv = ShipTorque*this.player2.MassInertia.MomentofInertiaInv*dt;
				if((float)Math.Abs(this.player2.Current.Velocity.Angular)<Physics2D.Physics.Tolerance)
				{
					this.player2.Current.Velocity.Angular = 0;
				}
				else if(this.player2.Current.Velocity.Angular>0)//Physics2D.Physics.Tolerance)
				{
					
					if(dv>this.player2.Current.Velocity.Angular)
					{
						this.player2.ApplyTorque(-this.player2.Current.Velocity.Angular*this.player2.MassInertia.MomentofInertiaInv);
					}
					else
					{
						this.player2.ApplyTorque(-ShipTorque);
					}
				}
				else if(this.player2.Current.Velocity.Angular<0)//-Physics2D.Physics.Tolerance)
				{
					if(-dv<this.player2.Current.Velocity.Angular)
					{
						this.player2.ApplyTorque(this.player2.Current.Velocity.Angular*this.player2.MassInertia.MomentofInertiaInv);
					}
					else
					{
						this.player2.ApplyTorque(ShipTorque);
					}
				}
			} 



			if (keys[Key.UpArrow])
			{
				this.player1.ApplyForce(new ForceInfo(new Vector2D(0,-ShipForce),Vector2DTransformType.ToWorldAngular));
			}
			if (keys[Key.DownArrow])
			{
				this.player1.ApplyForce(new ForceInfo(new Vector2D(0,ShipForce),Vector2DTransformType.ToWorldAngular));
			}
			if (keys[Key.RightArrow])
			{
				this.player1.ApplyTorque(ShipTorque);
				
			} 
			else if (keys[Key.LeftArrow])
			{
				this.player1.ApplyTorque(-ShipTorque);
			} 
			else 
			{
				float dv = ShipTorque*this.player1.MassInertia.MomentofInertiaInv*dt;
				if((float)Math.Abs(this.player1.Current.Velocity.Angular)<Physics2D.Physics.Tolerance)
				{
					this.player1.Current.Velocity.Angular = 0;
				}
				else if(this.player1.Current.Velocity.Angular>0)//Physics2D.Physics.Tolerance)
				{
					
					if(dv>this.player1.Current.Velocity.Angular)
					{
						this.player1.ApplyTorque(-this.player1.Current.Velocity.Angular*this.player1.MassInertia.MomentofInertiaInv);
					}
					else
					{
						this.player1.ApplyTorque(-ShipTorque);
					}
				}
				else if(this.player1.Current.Velocity.Angular<0)//-Physics2D.Physics.Tolerance)
				{
					if(-dv<this.player1.Current.Velocity.Angular)
					{
						this.player1.ApplyTorque(this.player1.Current.Velocity.Angular*this.player1.MassInertia.MomentofInertiaInv);
					}
					else
					{
						this.player1.ApplyTorque(ShipTorque);
					}
				}
			} 
			keypressed = now;
		}
		public void UpdateKeyboardOld(KeyboardState keys,float dt)
		{

			DateTime now = DateTime.Now;
			if(now .Subtract(keypressed).TotalSeconds<Physics2D.Physics.Tolerance)
			{
				return;
			}
			keypressed = now;
			if (keys[Key.P])
			{
				if(now.Subtract(paused).TotalSeconds >.35)
				{
					this.world.Enabled = false;
				}
				paused = now;
			}
			if (keys[Key.W])
			{
				this.player2.ApplyForce(new ForceInfo(new Vector2D(ShipForce,0),Vector2DTransformType.ToWorldAngular));
			}
			if (keys[Key.S])
			{
				this.player2.ApplyForce(new ForceInfo(new Vector2D(-ShipForce,0),Vector2DTransformType.ToWorldAngular));
			}
			if (keys[Key.D])
			{
				this.player2.ApplyTorque(ShipTorque);
				
			} 
			else if (keys[Key.A])
			{
				this.player2.ApplyTorque(-ShipTorque);
			} 
			else 
			{
				float dv = ShipTorque*this.player2.MassInertia.MomentofInertiaInv*dt;
				if((float)Math.Abs(this.player2.Current.Velocity.Angular)<Physics2D.Physics.Tolerance)
				{
					this.player2.Current.Velocity.Angular = 0;
				}
				else if(this.player2.Current.Velocity.Angular>0)//Physics2D.Physics.Tolerance)
				{
					
					if(dv>this.player2.Current.Velocity.Angular)
					{
						this.player2.ApplyTorque(-this.player2.Current.Velocity.Angular*this.player2.MassInertia.MomentofInertiaInv);
					}
					else
					{
						this.player2.ApplyTorque(-ShipTorque);
					}
				}
				else if(this.player2.Current.Velocity.Angular<0)//-Physics2D.Physics.Tolerance)
				{
					if(-dv<this.player2.Current.Velocity.Angular)
					{
						this.player2.ApplyTorque(this.player2.Current.Velocity.Angular*this.player2.MassInertia.MomentofInertiaInv);
					}
					else
					{
						this.player2.ApplyTorque(ShipTorque);
					}
				}
			} 



			if (keys[Key.UpArrow])
			{
				this.player1.ApplyForce(new ForceInfo(new Vector2D(ShipForce,0),Vector2DTransformType.ToWorldAngular));
			}
			if (keys[Key.DownArrow])
			{
				this.player1.ApplyForce(new ForceInfo(new Vector2D(-ShipForce,0),Vector2DTransformType.ToWorldAngular));
			}
			if (keys[Key.RightArrow])
			{
				this.player1.ApplyTorque(ShipTorque);
				
			} 
			else if (keys[Key.LeftArrow])
			{
				this.player1.ApplyTorque(-ShipTorque);
			} 
			else 
			{
				float dv = ShipTorque*this.player1.MassInertia.MomentofInertiaInv*dt;
				if((float)Math.Abs(this.player1.Current.Velocity.Angular)<Physics2D.Physics.Tolerance)
				{
					this.player1.Current.Velocity.Angular = 0;
				}
				else if(this.player1.Current.Velocity.Angular>0)//Physics2D.Physics.Tolerance)
				{
					
					if(dv>this.player1.Current.Velocity.Angular)
					{
						this.player1.ApplyTorque(-this.player1.Current.Velocity.Angular*this.player1.MassInertia.MomentofInertiaInv);
					}
					else
					{
						this.player1.ApplyTorque(-ShipTorque);
					}
				}
				else if(this.player1.Current.Velocity.Angular<0)//-Physics2D.Physics.Tolerance)
				{
					if(-dv<this.player1.Current.Velocity.Angular)
					{
						this.player1.ApplyTorque(this.player1.Current.Velocity.Angular*this.player1.MassInertia.MomentofInertiaInv);
					}
					else
					{
						this.player1.ApplyTorque(ShipTorque);
					}
				}
			} 
		}
		
		DateTime keypressed = DateTime.Now;
		DateTime lastFiredSecondary = DateTime.Now;
		public override Vector2D CameraPosition
		{
			get
			{
				//return (this.player1.Good.Position.Linear+this.player2.Good.Position.Linear)*.5f;	
				return Vector2D.Zero;
			}
		}
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
                        PhysicsState bob;// 
                        if (info.ICollidableBody1.CollisionState.GenerateCollisionEvents)
                        {
                            bob = info.ICollidableBody1Before;// new PhysicsState(collidables.Collidable1.Current);
                        }
                        else
                        {
                            bob = info.ICollidableBody2Before;// new PhysicsState(collidables.Collidable2.Current);
                        }


                        //rand.Position.Linear += collidables.Distance*.5f;

                        if (info.ICollidableBody1.LifeTime is Mortal)
                        {
                            if (!(info.ICollidableBody1 is ImpulseWave))
                            {
                                world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.ICollidableBody1.MassInertia.Mass*10, bob, info.ICollidableBody1.BoundingRadius, 5500));
                                info.ICollidableBody1.LifeTime.IsExpired = true;
                            }
                        }
                        if (info.ICollidableBody2.LifeTime is Mortal)
                        {
                            if (!(info.ICollidableBody2 is ImpulseWave))
                            {
                                world.AddICollidableBody(new ImpulseWave(new Mortal(.17f), info.ICollidableBody2.MassInertia.Mass*10, bob, info.ICollidableBody2.BoundingRadius, 5500));
                                info.ICollidableBody2.LifeTime.IsExpired = true;
                            }
                        }
                        //}
                    }
                }
            }
        }
        public override string Name
        {
            get
            {
                return "2 Player Game";
            }
        }
        public override string Description
        {
            get
            {
                return "A simple game where 2 players control rgdolls";
            }
        }
        public override string Instructions
        {
            get
            {
                return "Left Player:\n"+
                    "Movment keys: a,w,s,d\n"+
                    "weapon: left ctrl\n\n"+
                    "Right Player:\n"+
                    "Movment keys: arrow keys\n"+
                    "weapon: right ctrl\n";
            }
        }
        public override IDemo CreateNew()
        {
            return new TwoPlayerGame();
        }
        static int[] ExposionColors = new int[] { Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Yellow.ToArgb() };
        static int ExplosionPrimaryColor = Color.Orange.ToArgb();

	}

}
