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
using global::SdlDotNet;
namespace WindowsDriver.Demos
{
	/// <summary>
	/// Summary description for BasicCollisions.
	/// </summary>
    public class SpecialBalls : BaseDisplayDemo
	{
		public SpecialBalls()
		{

			world = new World2D();
			world.CalcGravity = false;
			for(int pos0 = 0; pos0 <400;pos0+=100)
			{
				RigidBody test = new RigidBody(MassInertia.FromCylindricalShell(50,20), 0, new Vector2D(750,500-pos0), new Circle2D(20));
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
				for(int pos = 300; pos <= 650;pos+=40+(pos0)/6)
				{
					RigidBody body = new RigidBody(MassInertia.FromCylindricalShell(50,20), 0, new Vector2D(50+pos,500-pos0), new Circle2D(20) );
					body.Coefficients.Restitution = 1;
					world.AddICollidableBody(body);
				}
			}
			foreach(ICollidableBody body in world.Collidables)
			{
				body.Current.Position.Linear -= new Vector2D(500,400);

			}


		}
		public override void UpdateKeyBoard(KeyboardState keys,float dt)
		{
		}
		public override float  Scale
		{
			get
			{
				return 1.5f;
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
                return "Special Balls";
            }
        }
        public override string Description
        {
            get
            {
                return "bunch of balls bounding arround";
            }
        }
        public override IDemo CreateNew()
        {
            return new SpecialBalls();
        }

	}
}
