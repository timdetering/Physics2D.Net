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

namespace Physics2D
{
    /// <summary>
    /// the interface that describes the methods and properties required for an object to be a Joint.
    /// </summary>
    public interface IJoint : IRemovable, ITimed
    {
        /// <summary>
        /// Tells the object to calculate the variables that wont change till the next call to UpdatePosition.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        void PreCalc(float dt);

        /// <summary>
        /// Calculates and applies impulse if required.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        /// <returns>true if impulse was applied; otherwise false.</returns>
        bool CalcAndApply(float dt);
    }
}
