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
    [Serializable]
    public abstract class BaseTimed : ITimed
    {
        protected ILifeTime lifeTime;
        public BaseTimed() { }
        public BaseTimed(ILifeTime lifeTime)
        {
            this.lifeTime = lifeTime;
        }
        public BaseTimed(BaseTimed copy)
        {
            if (copy.lifeTime != null)
            {
                this.lifeTime = (ILifeTime)copy.lifeTime.Clone();
            }
        }
        public ILifeTime LifeTime
        {
            get
            {
                return lifeTime;
            }
            set
            {
                lifeTime = value;
            }
        }
        [XmlIgnore(),Browsable(false)]
        public bool IsExpired
        {
            get
            {
                return lifeTime != null && lifeTime.IsExpired;
            }
            set
            {
                lifeTime.IsExpired = value;
            }
        }
        public virtual void Update(float dt)
        {
            lifeTime.Update(dt);
        }
    }
}