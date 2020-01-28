#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
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

using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath;
namespace Physics2D
{
    [Serializable]
    public enum InterferenceType
    {
        RayCollidable,
        CollidablePair
    }
    [Serializable]
    public sealed class InterferenceInfo
    {
        public readonly InterferenceType InterferenceType;
        public CollidablePairInterferenceInfo CollidablePairInfo;
        public RayCollidableInterferenceInfo RayCollidableInfo;
        public InterferenceInfo(CollidablePairInterferenceInfo CollidablePairInfo)
        {
            this.InterferenceType = InterferenceType.CollidablePair;
            this.CollidablePairInfo = CollidablePairInfo;
            this.RayCollidableInfo = null;
        }
        public InterferenceInfo(RayCollidableInterferenceInfo RayCollidableInfo)
        {
            this.InterferenceType = InterferenceType.RayCollidable;
            this.CollidablePairInfo = null;
            this.RayCollidableInfo = RayCollidableInfo;
        }
    }
    [Serializable]
    public sealed class RayCollidableInterferenceInfo
    {
        public readonly ICollidableArea Area;
        public readonly RaySegment2D RaySegment2D;
        public readonly Ray2DIntersectInfo Info;
        public readonly PhysicsState Before;
        public readonly PhysicsState After;
        public ICollidableBody Collidable;
        public RayCollidableInterferenceInfo(
            ICollidableArea Area,
            RaySegment2D RaySegment2D,
            Ray2DIntersectInfo Info,
            PhysicsState Before,
            PhysicsState After,
            ICollidableBody Collidable)
        {
            this.Area = Area;
            this.RaySegment2D = RaySegment2D;
            this.Info = Info;
            this.Before = Before;
            this.After = After;
            this.Collidable = Collidable;
        }
    }

    [Serializable]
    public sealed class CollidablePairInterferenceInfo 
    {
        public readonly bool AtContactStage;
        public readonly int Step;
        public readonly float dt;
        public readonly Vector2D Impulse;
        public readonly CollisionInfo Info;
        public readonly PhysicsState ICollidableBody1Before;
        public readonly PhysicsState ICollidableBody2Before;
        public readonly PhysicsState ICollidableBody1After;
        public readonly PhysicsState ICollidableBody2After;
        public ICollidableBody ICollidableBody1;
        public ICollidableBody ICollidableBody2;
        public CollidablePairInterferenceInfo(
          bool AtContactStage,
          int Step,
          float dt,
          Vector2D Impulse,
          CollisionInfo Info,
          PhysicsState ICollidableBody1Before,
          PhysicsState ICollidableBody2Before,
          PhysicsState ICollidableBody1After,
          PhysicsState ICollidableBody2After,
          ICollidableBody ICollidableBody1,
          ICollidableBody ICollidableBody2)
        {
            this.AtContactStage = AtContactStage;
            this.Step = Step;
            this.dt = dt;
            this.Info = Info;
            this.Impulse = Impulse;
            this.ICollidableBody1Before = ICollidableBody1Before;
            this.ICollidableBody2Before = ICollidableBody2Before;
            this.ICollidableBody1After = ICollidableBody1After;
            this.ICollidableBody2After = ICollidableBody2After;
            this.ICollidableBody1 = ICollidableBody1;
            this.ICollidableBody2 = ICollidableBody2;
        }
    }
}
