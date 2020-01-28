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
using System.Xml;
using System.Xml.Serialization;
namespace Physics2D
{
    [Serializable]
    [XmlInclude(typeof(Mortal)), XmlInclude(typeof(Immortal))]
    [XmlInclude(typeof(SingleStep)), XmlInclude(typeof(MultiStep))]
    public abstract class BaseLifeTime : ILifeTime
    {
        protected bool isExpired = false;
        protected float age = 0;
        protected BaseLifeTime()
        { }
        protected BaseLifeTime(BaseLifeTime copy)
        {
            this.age = copy.age;
            this.isExpired = copy.isExpired;
        }
        public virtual bool IsExpired
        {
            get
            {
                return isExpired;
            }
            set
            {
                isExpired = value;
            }
        }
        public float Age
        {
            get { return age; }
            set { age = value; }
        }
        public virtual void Reset()
        {
            age = 0;
            isExpired = false;
        }
        public virtual void Update(float dt)
        {
            age += dt;
        }
        public abstract float TimeLeft { get;}
        public abstract object Clone();
    }
    /// <summary>
    /// class descries a Imortal lifespan that can only be killed by setting Expired to true.
    /// </summary>
    [Serializable]
    public sealed class Immortal : BaseLifeTime
    {
        public Immortal()
        { }
        public Immortal(Immortal copy)
            : base(copy)
        { }
        public override float TimeLeft
        {
            get
            {
                return float.PositiveInfinity;
            }
        }
        public override object Clone()
        {
            return new Immortal(this);
        }
    }
    /// <summary>
    /// class descibes the lifespan of a mere mortal that will die after a time.
    /// </summary>
    [Serializable]
    public sealed class Mortal : BaseLifeTime
    {
        float maxTime;
        public Mortal(float maxTime)
        {
            if (float.IsInfinity(maxTime))
            {
                throw new ArgumentException("A Mortal cannot have a Infinite lifespan. (Use Immortal instead)");
            }
            this.maxTime = maxTime;
        }
        public Mortal(Mortal copy)
            : base(copy)
        {
            this.maxTime = copy.maxTime;
        }
        public override bool IsExpired
        {
            get
            {
                return isExpired || age > maxTime;
            }
        }
        public override float TimeLeft
        {
            get
            {
                return maxTime - age;
            }
        }
        public float MaxTime
        {
            get { return maxTime; }
            set { maxTime = value; }
        }
        public override object Clone()
        {
            return new Mortal(this);
        }

    }
    [Serializable]
    public sealed class SingleStep : BaseLifeTime
    {
        static float lastdt = .34f;
        public SingleStep() { }
        public SingleStep(SingleStep copy)
            : base(copy)
        { }
        public override float TimeLeft
        {
            get { return lastdt; }
        }
        public override void Update(float dt)
        {
            base.Update(dt);
            lastdt = dt;
            isExpired = true;
        }
        public override object Clone()
        {
            return new SingleStep(this);
        }
    }
    [Serializable]
    public sealed class MultiStep : BaseLifeTime
    {
        static float lastdt = .34f;
        int stepsLeft;
        int maxSteps;
        public MultiStep(int maxSteps)
        {
            this.maxSteps = maxSteps;
            this.stepsLeft = maxSteps;
        }
        public MultiStep(MultiStep copy)
            : base(copy)
        {
            this.stepsLeft = copy.stepsLeft;
            this.maxSteps = copy.maxSteps;
        }
        public int MaxSteps
        {
            get { return maxSteps; }
            set { maxSteps = value; }
        }
        public int StepsLeft
        {
            get { return stepsLeft; }
            set { stepsLeft = value; }
        }
        public override float TimeLeft
        {
            get { return lastdt * stepsLeft; }
        }
        public override void Update(float dt)
        {
            base.Update(dt);
            lastdt = dt;
            stepsLeft--;
            if (stepsLeft <= 0)
            {
                isExpired = true;
            }
        }
        public override void Reset()
        {
            base.Reset();
            stepsLeft = maxSteps;
        }
        public override object Clone()
        {
            return new MultiStep(this);
        }
    }
    [Serializable]
    public sealed class ChildLifeTime : ILifeTime
    {
        private ILifeTime self;
        private ILifeTime parent;
        public ChildLifeTime(ILifeTime parent) : this(parent, (ILifeTime)parent.Clone()) { }
        public ChildLifeTime(ILifeTime parent, ILifeTime self)
        {
            this.parent = parent;
            this.self = self;
        }
        public ChildLifeTime(ChildLifeTime copy)
        {
            this.parent = (ILifeTime)copy.parent.Clone();
            this.self = (ILifeTime)copy.self.Clone();
        }

        public ILifeTime Self
        {
            get { return self; }
            set { self = value; }
        }
        public ILifeTime Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        [XmlIgnore(), Browsable(false)]
        public float Age
        {
            get
            {
                return self.Age;
            }
            set
            {
                self.Age = value;
            }
        }
        [XmlIgnore(), Browsable(false)]
        public bool IsExpired
        {
            get
            {
                return self.IsExpired || parent.IsExpired;
            }
            set
            {
                self.IsExpired = value;
            }
        }
        [XmlIgnore(), Browsable(false)]
        public float TimeLeft
        {
            get { return Math.Min(parent.TimeLeft, self.TimeLeft); }
        }
        public object Clone()
        {
            return new ChildLifeTime(this);
        }
        public void Reset()
        {
            self.Reset();
        }
        public void Update(float dt)
        {
            self.Update(dt);
        }
    }
}
