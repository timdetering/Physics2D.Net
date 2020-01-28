#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
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
using Physics2D.Collision;
using Physics2D.Geometry2D;
using Physics2D.AdvMath;
namespace Physics2D
{
    public class ImpulseAppliedInfo 
    {
        public readonly bool AtContactStage;
        public readonly int Step;
        public readonly int Interation;
        public readonly double dt;
        public readonly CollisionInfo Info;
        public readonly PhysicsState ICollidable1Before;
        public readonly PhysicsState ICollidable2Before;
        public readonly PhysicsState ICollidable1After;
        public readonly PhysicsState ICollidable2After;
        public ICollidable ICollidable1;
        public ICollidable ICollidable2;
        public ImpulseAppliedInfo(
          bool AtContactStage,
          int Step,
          int Interation,
          double dt,
          CollisionInfo Info,
          PhysicsState ICollidable1Before,
          PhysicsState ICollidable2Before,
          PhysicsState ICollidable1After,
          PhysicsState ICollidable2After,
          ICollidable ICollidable1,
          ICollidable ICollidable2)
        {
            this.AtContactStage = AtContactStage;
            this.Step = Step;
            this.Interation = Interation;
            this.dt = dt;
            this.Info = Info;
            this.ICollidable1Before = ICollidable1Before;
            this.ICollidable2Before = ICollidable2Before;
            this.ICollidable1After = ICollidable1After;
            this.ICollidable2After = ICollidable2After;
            this.ICollidable1 = ICollidable1;
            this.ICollidable2 = ICollidable2;
        }
    }
}
