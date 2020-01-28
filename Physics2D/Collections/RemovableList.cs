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
namespace Physics2D.Collections
{
    [Serializable]
    public class RemovableList<T> : UpdatableList<T>, ICleanable
        where T : IRemovable
    {
        public RemovableList()
            : base()
        { }
        public RemovableList(IEnumerable<T> collection)
            : base(collection)
        { }
        public RemovableList(int capacity)
            : base(capacity)
        { }
        /// <summary>
        /// Removes all objects that are expired.
        /// </summary>
        /// <returns>true if the list is empty; otherwise false. </returns>
        /// <remarks>
        /// This is different then the RemoveExpired for <see cref="TimedList< T >"/>. 
        /// In that it checks for object being expired through the <see cref="ICleanable.RemoveExpired"/> method,
        /// and that it generates the <see cref="IRemovable.Removed"/>  event.
        /// </remarks>
        public bool RemoveExpired()
        {
            base.RemoveAll(delegate(T item) { if (item.RemoveExpired()) { item.OnRemoved(); return true; } return false; });
            return base.Count == 0;
        }
    }
}