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
using AdvanceMath.Geometry2D;
using AdvanceMath;

namespace Physics2D
{

    public interface IMassive : IRemovable, ITimed, ICloneable
    {
        #region properties
        /// <summary>
        /// Gets the MassInertia describing the objects mass.
        /// </summary>
        MassInertia MassInertia { get;}
        /// <summary>
        /// Gets and Sets the BodyFlags of the object.
        /// </summary>
        BodyFlags Flags { get;set;}
        /// <summary>
        /// Gets and Sets the the PhysicsState describing what it was at the last time SaveState() was called.
        /// </summary>
        PhysicsState Initial { get;set;}
        /// <summary>
        /// Gets and Sets the last good PhysicsState (last time Update() was called.).
        /// </summary>
        PhysicsState Good { get;set;}
        /// <summary>
        /// Gets the current PhysicsState.
        /// </summary>
        PhysicsState Current { get;set;}
        /// <summary>
        /// Gets a The Matrix2D that describes the current transformations.
        /// </summary>
        Matrix2D Matrix { get;}
        #endregion
        #region methods
        /// <summary>
        /// Tells the Object to Calculate Current.Acceleration.
        /// </summary>
        void CalcAccelerations(Vector2D accelerationDueToGravity);
        /// <summary>
        /// Clears all the forces acting on the object.
        /// </summary>
        void ClearForces();

        /// <summary>
        /// Tells the object to calculate its new velocity based on the amount of time passed.
        /// </summary>
        /// <param name="dt">change in time.</param>
        void UpdateVelocity(float dt);
        /// <summary>
        /// Tells the object to calculate its new position based on the amount of time passed. 
        /// </summary>
        /// <param name="dt">change in time.</param>
        void UpdatePosition(float dt);
        /// <summary>
        /// Calculates the Velocity of the object at the position relative to the world.
        /// </summary>
        /// <param name="pointRelativeToWorld">point to get the velocity at.</param>
        /// <returns>The velocty of the object relative to the world.</returns>
        Vector2D GetVelocityAtWorld(Vector2D pointRelativeToWorld);
        /// <summary>
        /// Calculates the Velocity of the object at the position relative to the collidable.
        /// </summary>
        /// <param name="pointRelativeToBody">point to get the velocity at.</param>
        /// <returns>The velocty of the object relative to the world.</returns>
        Vector2D GetVelocityAtRelative(Vector2D pointRelativeToBody);
        /// <summary>
        /// Sets Current Velocity to Initial.
        /// </summary>
        void LoadVelocityState();
        /// <summary>
        /// Sets Current Position to Initial.
        /// </summary>
        void LoadPositionState();
        /// <summary>
        /// Sets Current to Good.
        /// </summary>
        void SaveGood();
        /// <summary>
        /// Sets Current to Initial.
        /// </summary>
        void SaveState();
        /// <summary>
        /// Sets Initial  to Current.
        /// </summary>
        void LoadState();
        /// <summary>
        /// Applies Torque to the Body.
        /// </summary>
        /// <param name="torque">the ammount of torque to apply</param>
        void ApplyTorque(float torque);
        /// <summary>
        /// Applies Force to the collidable.
        /// </summary>
        /// <param name="forceInfo">The info about the force to be applied</param>
        void ApplyForce(ForceInfo forceInfo);
        /// <summary>
        /// Transforms the Vector.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="convertType">The type of conversion</param>
        /// <returns></returns>
        Vector2D Vector2DTransform(Vector2D source, Vector2DTransformType convertType);
        #endregion
    }
}