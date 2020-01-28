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
using System.Runtime.Serialization;
using System.Collections.Generic;
using Physics2D.CollisionDetection;
namespace Physics2D
{
    /// <summary>
    /// A very ugly class at the moment needs major refactoring and redesigning.
    /// </summary>
    [Serializable]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public sealed class CollisionState : ICloneable, IDeserializationCallback 
	{
		public bool Frozen = false;
		public int CollisionLevel = -1;
        [NonSerialized]
        public List<ICollidableBody> Colliders = new List<ICollidableBody>();
        [NonSerialized]
        public List<CollisionPair> CollisionPairs = new List<CollisionPair>();
		public bool CollisionImpulseApplied = false;
		public bool ContactImpulseApplied = false;
        [NonSerialized]
        public List<InterferenceInfo> InterferenceInfos = null;
        private bool generateContactEvents = false;

        public bool GenerateContactEvents
        {
            get { return generateContactEvents; }
            set { generateContactEvents = value; }
        }
        private bool generateCollisionEvents = false;

        public bool GenerateCollisionEvents
        {
            get { return generateCollisionEvents; }
            set { generateCollisionEvents = value; }
        }
        private bool generateRayEvents = false;

        public bool GenerateRayEvents
        {
            get { return generateRayEvents; }
            set { generateRayEvents = value; }
        }
		public CollisionState(){}
		public void Reset()
		{
			Frozen = false;
			CollisionLevel = -1;
			Colliders.Clear();
            CollisionPairs.Clear();
		}
		public void ResetEventInfo()
		{
            if (generateCollisionEvents || generateContactEvents || generateRayEvents)
			{
                if (InterferenceInfos == null)
				{
                    InterferenceInfos = new List<InterferenceInfo>();
				}
				else
				{
                    InterferenceInfos.Clear();
				}
			}
		}
        public object Clone()
        {
            CollisionState clone = new CollisionState();
            clone.CollisionImpulseApplied = CollisionImpulseApplied;
            clone.ContactImpulseApplied = ContactImpulseApplied;
            clone.generateCollisionEvents = generateCollisionEvents;
            clone.generateContactEvents = generateContactEvents;
            clone.generateRayEvents = generateRayEvents;
            return clone;
        }
        #region IDeserializationCallback Members
        public void OnDeserialization(object sender)
        {
            Colliders = new List<ICollidableBody>();
            CollisionPairs = new List<CollisionPair>();
            ResetEventInfo();
        }
        #endregion
    }
}
