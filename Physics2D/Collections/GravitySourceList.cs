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
using AdvanceMath;
namespace Physics2D.Collections
{
    [Serializable]
    public class GravitySourceList : List<IGravitySource>, ICleanable
	{
		public GravitySourceList():base()
		{}
        public GravitySourceList(IEnumerable<IGravitySource> collection)
            : base(collection)
        {}
        public GravitySourceList(int capacity)
            : base(capacity)
        {}
        public Vector2D GetGravityPullAt(Vector2D pointRelativeToWorld)
        {
            Vector2D returnvalue = Vector2D.Origin;
            int count = base.Count;
            for (int pos = 0; pos < count; ++pos)
            {
                returnvalue += base[pos].GetGravityPullAt(pointRelativeToWorld);
            }
            return returnvalue;
        }
        public bool RemoveExpired()
        {
            base.RemoveAll(delegate(IGravitySource item) { return item.IsExpired; });
            return base.Count == 0;
        }
    }
}
