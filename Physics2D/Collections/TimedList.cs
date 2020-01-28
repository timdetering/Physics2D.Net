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
    public class TimedList<T> : UpdatableList<T>, ICleanable
        where T : ITimed
    {
        public TimedList()
            : base()
        { }
        public TimedList(IEnumerable<T> collection)
            : base(collection)
        { }
        public TimedList(int capacity)
            : base(capacity)
        { }
        /// <summary>
        /// Removes all objects that are expired.
        /// </summary>
        /// <returns>true if the list is empty; otherwise false. </returns>
        public bool RemoveExpired()
        {
            base.RemoveAll(delegate(T item) { return item.IsExpired; });
            return base.Count == 0;
        }
    }
}