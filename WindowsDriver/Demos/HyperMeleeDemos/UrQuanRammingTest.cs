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
    public class UrQuanRammingTest : BaseControlDemo
    {
        #region newer stuff
        public UrQuanRammingTest()
            : base()
        {
            this.lightningimpulse *= 10;
            this.forwardimpulse *= -1;
        }
        public override void InitObjects()
        {
            
            base.InitObjects();
        }
        Random rand = new Random();
        Missile[] missiles;
        RigidBody[] bodies;
        public override void AddObjects()
        {
            int count = 20;
            bodies = new RigidBody[count];
            missiles = new Missile[count];
            for (int pos = 0; pos != count; ++pos)
            {
                bodies[pos] = OldDirectXTests.CreateUrQuan(world);
                bodies[pos].Current.Position.Linear = new Vector2D(rand.Next(-500,500)+ 200 - 10 * pos, 200 + 300 * pos);
                //grouppairs[point] = new RigidBody(Polygon2D.FromRectangle(20, 100), MassInertia.FromRectangle(200, 20, 100), new PhysicsState(new ALVector2D(0, new Vector2D(200 - 10 * point, 200 + 200 * point))));
                world.AddICollidableBody(bodies[pos]);
            }
            //OldDirectXTests.SetEarthMoon(world);
            OldDirectXTests.SetEvents(world);
            //this.InitialShipState = OldDirectXTests.staticInitialShipState;
            this.mainship = OldDirectXTests.CreateUserRigidBody(world);
            base.AddObjects();
            for (int pos = 0; pos != count; ++pos)
            {
                missiles[pos] = new Missile(this.mainship, bodies[pos], 4000000000, 1, 40000000, 200);
            }
        }
        public override void UpdateAI(float dt)
        {
            foreach (Missile miss in missiles)
            {
                miss.Update(dt);
            }
        }
        public override string Name
        {
            get
            {
                return "Ur-Quan Ramming Test";
            }
        }
        public override string Description
        {
            get
            {
                return "A bunch of Ur-Quan Vessels try to Ram your Ship";
            }
        }
        public override IDemo CreateNew()
        {
            return new UrQuanRammingTest();
        }
        #endregion
    }
}
