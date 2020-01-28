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
    public class JointList<T> : RemovableList<T>
        where T : IJoint
    {
        public JointList()
            : base()
        { }
        public JointList(IEnumerable<T> collection)
            : base(collection)
        { }
        public JointList(int capacity)
            : base(capacity)
        { }
        /// <summary>
        /// Calls <see cref="IJoint.PreCalc"/> for all the <typeparamref name="T"/> in the list.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        public void PreCalc(float dt)
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base[pos].PreCalc(dt);
            }
        }
        /// <summary>
        /// Calls <see cref="IJoint.CalcAndApply"/> for all the <typeparamref name="T"/> in the list.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        public void CalcAndApply(float dt)
        {
            count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                base [pos].CalcAndApply(dt);
            }
        }
    }
}