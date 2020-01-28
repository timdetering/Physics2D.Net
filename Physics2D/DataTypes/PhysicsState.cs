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
using AdvanceMath.Geometry2D;
using AdvanceMath;
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace Physics2D
{


    /// <summary>
    /// This class holds the variables usually changed mulitple times  each update like the postion of an object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    public sealed class PhysicsState
    {
        /// <summary>
        /// This is Position and Orientation.
        /// </summary>
        /// <remarks>
        /// <seealso href="http://en.wikipedia.org/wiki/Position"/>
        /// </remarks>
        public ALVector2D Position;

        /// <summary>
        /// Angular and Linear Velocity.
        /// </summary>
        /// <remarks>
        /// <seealso href="http://en.wikipedia.org/wiki/Velocity"/>
        /// </remarks>
        public ALVector2D Velocity;

        /// <summary>
        /// Angular and Linear Acceleration.
        /// </summary>
        /// <remarks>
        /// <seealso href="http://en.wikipedia.org/wiki/Acceleration"/>
        /// </remarks>
        
        [NonSerialized]
        public ALVector2D Acceleration;
        /// <summary>
        /// Torque and Force
        /// </summary>
        /// <remarks>
        /// <seealso href="http://en.wikipedia.org/wiki/Torque"/>
        /// <seealso href="http://en.wikipedia.org/wiki/Force"/>
        /// </remarks>
        
        [NonSerialized]
        public ALVector2D ForceAccumulator;
        public Vector2D GetVelocityAtWorld(Vector2D pointRelativeToWorld)
        {
            return Velocity.Linear + (Velocity.Angular ^ (pointRelativeToWorld - Position.Linear));
        }
        public Vector2D GetVelocityAtRelative(Vector2D pointRelativeToBody)
        {
            return Velocity.Linear + (Velocity.Angular ^ pointRelativeToBody);
        }
        public PhysicsState()
        {
            this.Position = ALVector2D.Zero;
            this.Velocity = ALVector2D.Zero;
            this.Acceleration = ALVector2D.Zero;
            this.ForceAccumulator = ALVector2D.Zero;
        }
        public PhysicsState(ALVector2D Position)
        {
            this.Position = Position;
            this.Velocity = ALVector2D.Zero;
            this.Acceleration = ALVector2D.Zero;
            this.ForceAccumulator = ALVector2D.Zero;
        }
        public PhysicsState(ALVector2D Position, ALVector2D Velocity)
        {
            this.Position = Position;
            this.Velocity = Velocity;
            this.Acceleration = ALVector2D.Zero;
            this.ForceAccumulator = ALVector2D.Zero;
        }
        public PhysicsState(PhysicsState state)
        {
            this.Position = state.Position;
            this.Velocity = state.Velocity;
            this.Acceleration = state.Acceleration;
            this.ForceAccumulator = state.ForceAccumulator;
        }
        public void Set(PhysicsState state)
        {
            this.Position = state.Position;
            this.Velocity = state.Velocity;
            this.Acceleration = state.Acceleration;
            this.ForceAccumulator = state.ForceAccumulator;
        }
    }
}
