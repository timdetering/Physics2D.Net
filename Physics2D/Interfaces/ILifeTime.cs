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
using System.ComponentModel;
using System.Xml.Serialization;
namespace Physics2D
{

	/// <summary>
	/// Describes the lifeTime of an object.
	/// </summary>
    [XmlInclude(typeof(BaseLifeTime)), XmlInclude(typeof(ChildLifeTime))]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]

    public interface ILifeTime : ICloneable
    {
        /// <summary>
        /// Resets the Time Left.
        /// </summary>
        void Reset();
        /// <summary>
        /// Gets or Sets if the Objects life is expired.
        /// </summary>
        bool IsExpired { get;set;}
        /// <summary>
        /// Gets the amount of time left in seconds.
        /// </summary>
        [XmlIgnore(), Browsable(false)]
        float TimeLeft { get;}
        /// <summary>
        /// Gets the Age in seconds.
        /// </summary>
        float Age { get;set;}
        /// <summary>
        /// Informs the lifeTime instance that a certain amount of time has passed.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called.</param>
        void Update(float dt);
    }
}
