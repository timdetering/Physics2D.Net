#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion




#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using Physics2DDotNet.Math2D;
using AdvanceMath;
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
namespace Physics2DDotNet
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
        public PhysicsState()
        {
            this.Position = ALVector2D.Zero;
            this.Velocity = ALVector2D.Zero;
            this.Acceleration = ALVector2D.Zero;
            this.ForceAccumulator = ALVector2D.Zero;
        }
        public PhysicsState(ALVector2D position)
        {
            this.Position = position;
            this.Velocity = ALVector2D.Zero;
            this.Acceleration = ALVector2D.Zero;
            this.ForceAccumulator = ALVector2D.Zero;
        }
        public PhysicsState(ALVector2D position, ALVector2D velocity)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Acceleration = ALVector2D.Zero;
            this.ForceAccumulator = ALVector2D.Zero;
        }
        public PhysicsState(PhysicsState state)
        {
            if (state == null) { throw new ArgumentNullException("state"); }
            this.Position = state.Position;
            this.Velocity = state.Velocity;
            this.Acceleration = state.Acceleration;
            this.ForceAccumulator = state.ForceAccumulator;
        }
        public void Set(PhysicsState state)
        {
            if (state == null) { throw new ArgumentNullException("state"); }
            this.Position = state.Position;
            this.Velocity = state.Velocity;
            this.Acceleration = state.Acceleration;
            this.ForceAccumulator = state.ForceAccumulator;
        }
    }
}
