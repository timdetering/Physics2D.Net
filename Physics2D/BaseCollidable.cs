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
using AdvanceMath.Geometry2D;
namespace Physics2D
{
    [Serializable]
    public abstract class BaseCollidable : BaseRemovableObject, IBaseCollidable
    {
        [NonSerialized]
        protected BoundingBox2D boundingBox2D;
        protected CollisionIgnoreInfo ignoreInfo;
        public BaseCollidable()
            : base()
        {
            this.ignoreInfo = new CollisionIgnoreInfo();
        }
        public BaseCollidable(ILifeTime lifeTime)
            : base(lifeTime)
        {
            this.ignoreInfo = new CollisionIgnoreInfo();
        }
        protected BaseCollidable(BaseCollidable copy)
            : base(copy)
        {
            this.ignoreInfo = new CollisionIgnoreInfo(copy.ignoreInfo);
        }
        public CollisionIgnoreInfo IgnoreInfo
        {
            get { return ignoreInfo; }
        }
        [System.ComponentModel.Browsable(false)]
        public BoundingBox2D BoundingBox2D
        {
            get { return boundingBox2D; }
        }
        public abstract void CalcBoundingBox2D();
    }
}