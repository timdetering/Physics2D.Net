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
using AdvanceSystem;
using Physics2D;

using AdvanceMath.Geometry2D;
using AdvanceMath;
using Color = System.Drawing.Color;
namespace WindowsDriver.Demos
{
    public class BoundedOldTests : OldDirectXTests 
    {
        public Bounded<float> XAxis = new Bounded<float>(-8000, 0, 8000, true);
        public Bounded<float> YAxis = new Bounded<float>(-8000, 0, 8000, true);
        public Bounded<float> TXAxis;
        public Bounded<float> TYAxis;
        public override string Name
        {
            get
            {
                return "Bounded Old Direct X Tests";
            }
        }
        public override string Description
        {
            get
            {
                return "All the Tests from the the old DirectXTest Driver. (hardcoded configurations) But Bounded to a certain area";
            }
        }
        public override void AddObjects()
        {
            base.AddObjects();

            /*BoundingBox2D gamearea = new BoundingBox2D(0, 0, 0, 0);

            foreach (ICollidableBody body in this.world.Collidables)
            {
                if ((body.Flags & BodyFlags.InfiniteMass) != BodyFlags.InfiniteMass)
                {
                    body.CalcBoundingBox2D();
                    gamearea = BoundingBox2D.From2BoundingBox2Ds(gamearea, body.BoundingBox2D);
                }
            }
            XAxis = new Bounded(gamearea.Lower.X, 0, gamearea.Upper.X, true);
            YAxis = new Bounded(gamearea.Lower.Y, 0, gamearea.Upper.Y, true);*/

        }
        public override bool Update(float dt)
        {
            Vector2D campos = this.CameraPosition;
            TXAxis = new Bounded<float>(campos.X + XAxis.Binder.Lower, 0, campos.X + XAxis.Binder.Upper, true);
            TYAxis = new Bounded<float>(campos.Y + YAxis.Binder.Lower, 0, campos.Y + YAxis.Binder.Upper, true);
            foreach (ICollidableBody body in this.world.Collidables)
            {
                if ((body.Flags & BodyFlags.InfiniteMass) != BodyFlags.InfiniteMass)
                {
                    Vector2D tmp = body.Current.Position.Linear;
                    tmp.X = TXAxis.Binder.Bind(tmp.X);
                    tmp.Y = TYAxis.Binder.Bind(tmp.Y);
                    body.Current.Position.Linear = tmp;
                }
            }
            return base.Update(dt);
        }
        public override IDemo CreateNew()
        {
            return new BoundedOldTests();
        }
    }
}