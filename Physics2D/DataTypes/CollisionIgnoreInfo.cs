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
using AdvanceSystem.Collections;
namespace Physics2D
{
    [Serializable]
    public sealed class CollisionIgnoreInfo: IDeserializationCallback
    {
        public const int ImmovableGroup = -1;
        private const int ReservedIDs = 1;
        static AutoResizeBitArray usedGroups = new AutoResizeBitArray();
        public static int GetUnusedID()
        {
            return usedGroups.LowestFalseBitIndex;
        }
        public static void FreeAllUsedIDs()
        {
            usedGroups.Clear();
        }
        public static void FreeUsedID(int ID)
        {
            usedGroups.Set(ID, false);
        }
        AutoResizeBitArray groups;
        AutoResizeBitArray groupsToIgnore;
        public CollisionIgnoreInfo()
        {
            groups = new AutoResizeBitArray();
            groupsToIgnore = new AutoResizeBitArray();
        }
        public CollisionIgnoreInfo(CollisionIgnoreInfo copy)
        {
            this.groups = new AutoResizeBitArray(copy.groups);
            this.groupsToIgnore = new AutoResizeBitArray(copy.groupsToIgnore);
        }
        public bool IsCollidable
        {
            get
            {
                return !this.groupsToIgnore.AllTrue;
            }
            set
            {
                if (!this.groupsToIgnore.AllTrue ^ value)
                {
                    this.groupsToIgnore.SetAll(!value);
                }
            }
        }
        /// <summary>
        /// Checks to see if the object is in a Group.
        /// </summary>
        /// <param name="ID">The Int32 number that belongs to a group.</param>
        /// <returns>true if is is in the group; otherwise false.</returns>
        public bool IsInGroup(int ID)
        {
            return groups[ID + ReservedIDs];
        }
        /// <summary>
        /// Checks to see if the object is ignoring a Group.
        /// </summary>
        /// <param name="ID">The Int32 number that belongs to a group.</param>
        /// <returns>true if is is ignoring the group; otherwise false.</returns>
        public bool IsIgnoring(int ID)
        {
            return groupsToIgnore[ID + ReservedIDs];
        }
        /// <summary>
        /// Joins a Ignore Group.
        /// </summary>
        /// <param name="ID">The Int32 number that belongs to a group.</param>
        public void JoinGroup(int ID) 
        {
            usedGroups.Set(ID + ReservedIDs, true);
            groups.Set(ID + ReservedIDs, true);
        }
        /// <summary>
        /// Leaves a Ignore Group.
        /// </summary>
        /// <param name="ID">The Int32 number that belongs to a group.</param>
        public void LeaveGroup(int ID) 
        {
            groups.Set(ID + ReservedIDs, false);
        }
        /// <summary>
        /// Adds a Group that this object will ignore.
        /// </summary>
        /// <param name="ID">The Int32 number that belongs to a group.</param>
        public void AddGroupToIgnore(int ID)
        {
            groupsToIgnore.Set(ID + ReservedIDs, true);
        }
        /// <summary>
        /// Removes a Group that this object will ignore.
        /// </summary>
        /// <param name="ID">The Int32 number that belongs to a group.</param>
        public void RemoveGroupToIgnore(int ID) 
        {
            groupsToIgnore.Set(ID + ReservedIDs, false);
        }
        /// <summary>
        /// Adds a Group that this object will ignore And Joins it.
        /// </summary>
        /// <param name="ID">The Int32 number that belongs to a group.</param>
        public void JoinGroupToIgnore(int ID)
        {
            usedGroups.Set(ID + ReservedIDs, true);
            groups.Set(ID + ReservedIDs, true);
            groupsToIgnore.Set(ID + ReservedIDs, true);
        }
        /// <summary>
        /// Removes a Group that this object will ignore And Leaves it.
        /// </summary>
        /// <param name="ID">The Int32 number that belongs to a group.</param>
        public void LeaveGroupToIgnore(int ID)
        {
            groups.Set(ID + ReservedIDs, false);
            groupsToIgnore.Set(ID + ReservedIDs, false);
        }
        /// <summary>
        /// Causes it to leave all Groups.
        /// </summary>
        public void ClearGroups()
        {
            AutoResizeBitArray reserved = new AutoResizeBitArray();
            AutoResizeBitArray.Copy(groups, 0, reserved, 0, ReservedIDs);
            this.groups.Clear();
            AutoResizeBitArray.Copy(reserved, 0, groups, 0, ReservedIDs);
        }
        /// <summary>
        /// Causes it to stop ignoring any Groups.
        /// </summary>
        public void ClearGroupsToIgnore()
        {
            AutoResizeBitArray reserved = new AutoResizeBitArray();
            AutoResizeBitArray.Copy(groupsToIgnore, 0, reserved, 0, ReservedIDs);
            this.groupsToIgnore.Clear();
            AutoResizeBitArray.Copy(reserved, 0, groupsToIgnore, 0, ReservedIDs);
        }
        /// <summary>
        /// Checks to see if 2 objects can collide. by cheking there ignore info.
        /// </summary>
        /// <param name="other">the other's ignore info.</param>
        /// <returns>true if they can collide; otherwise false.</returns>
        public bool CanCollideWith(CollisionIgnoreInfo other)
        {
            return  !this.groupsToIgnore.AllTrue&&!other.groupsToIgnore.AllTrue&&
                !(AutoResizeBitArray.AndAnd(this.groups, other.groupsToIgnore)||
                AutoResizeBitArray.AndAnd(other.groups, this.groupsToIgnore));
        }
        #region IDeserializationCallback Members
        public void OnDeserialization(object sender)
        {
            usedGroups = groups | usedGroups;
        }
        #endregion
    }
}