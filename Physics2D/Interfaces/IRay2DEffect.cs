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
using System.Collections.Generic;
using System.Text;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2D.CollisionDetection;


namespace Physics2D
{

    public interface IRay2DEffectGroup : ICollidableArea
    {
        /// <summary>
        /// List of IRay2DEffects in the Group.
        /// </summary>
        List<IRay2DEffect> Ray2DEffects { get;}
    }
    public interface IRay2DEffect : IUpdatable, ITimed
    {
        /// <summary>
        /// The raySegment that makes up the Effect.
        /// </summary>
        RaySegment2D RaySegment { get;}
        /// <summary>
        /// Called when a intersection with a ICollidableBody is confirmed.
        /// </summary>
        /// <param name="dt">The time passed in the timestep.</param>
        /// <param name="pair">The RayICollidableBodyPair describing the intersection.</param>
        void ApplyEffect(float dt, RayICollidableBodyPair pair);
        /// <summary>
        /// The length of the IRay2DEffect that should be shown on screen.
        /// </summary>
        float DisplayLength { get;}
        /// <summary>
        /// Is true if intersection detection should be ran on it.
        /// </summary>
        /// <remarks>If it changes after added to the world it will have strange effects.</remarks>
        bool IsCollidable { get;}
    }
}
