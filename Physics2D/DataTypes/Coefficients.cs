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
    /// Describes the Coefficients of a surface.
    /// </summary>
    [Serializable]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public sealed class Coefficients
    {
        /// <summary>
        /// Logically Merges 2 Coefficients.
        /// </summary>
        /// <param name="first">The first Coefficients. </param>
        /// <param name="second">The second Coefficients. </param>
        /// <returns>The Result of the merger.</returns>
        //public static Coefficients Merge(Coefficients first,Coefficients second)
        //{
        //    return new Coefficients(first.Restitution*second.Restitution*.5f, first.StaticFriction* second.StaticFriction*.5f, first.DynamicFriction* second.DynamicFriction*.5f);
        //}
        public static Coefficients Merge(Coefficients first, Coefficients second)
        {
            return new Coefficients(Math.Min(first.restitution, second.restitution), Math.Max(first.staticFriction, second.staticFriction), Math.Max(first.dynamicFriction, second.dynamicFriction));
        }
        private float restitution;
        private float staticFriction;
        private float dynamicFriction;
        public Coefficients(float Restitution, float StaticFriction, float DynamicFriction)
        {
            this.restitution = Restitution;
            this.staticFriction = StaticFriction;
            this.dynamicFriction = DynamicFriction;
        }
        /// <summary>
        /// AKA Bounciness. This is how much energy is kept as kinetic energy after a collision.
        /// </summary>
        public float Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }
        /// <summary>
        /// http://en.wikipedia.org/wiki/Friction
        /// </summary>
        public float StaticFriction
        {
            get { return staticFriction; }
            set { staticFriction = value; }
        }
        /// <summary>
        /// http://en.wikipedia.org/wiki/Friction
        /// </summary>
        public float DynamicFriction
        {
            get { return dynamicFriction; }
            set { dynamicFriction = value; }
        }
    }
}
