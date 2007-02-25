#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion



#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
namespace Physics2DDotNet
{
    /// <summary>
    /// A object that describes the time a object will remain in the Physics engine.
    /// </summary>
    [Serializable]
    public sealed class Lifespan 
    {
        #region fields
        Scalar age = 0;
        Scalar maxAge;
        Lifespan master; 
        #endregion
        #region constructors
        public Lifespan()
            : this(0, -1, null)
        { }
        public Lifespan(Scalar maxAge)
            : this(0, maxAge, null)
        { }
        public Lifespan(Lifespan master)
            : this(0, -1, master)
        { }
        public Lifespan(Scalar maxAge, Lifespan master)
            : this(0, maxAge, master)
        { }
        public Lifespan(Scalar age, Scalar maxAge, Lifespan master)
        {
            this.age = age;
            this.maxAge = maxAge;
            this.master = master;
        } 
        #endregion
        #region properties
        public Lifespan Master
        {
            get { return master; }
            set { master = value; }
        }
        public bool IsExpired
        {
            get
            {
                return OverAged || IsMasterExpired;
            }
            set
            {
                if (value ^ IsExpired)
                {
                    if (value)
                    {
                        age = -1;
                    }
                    else
                    {
                        if (OverAged)
                        {
                            age = 0;
                        }
                        if (IsMasterExpired)
                        {
                            IsMasterExpired = false;
                        }
                    }
                }
            }
        }
        public bool IsMasterExpired
        {
            get
            {
                return (master != null && master.IsExpired);
            }
            set
            {
                if (master != null)
                {
                    master.IsExpired = value;
                }
            }
        }
        public bool IsImmortal
        {
            get
            {
                return maxAge < 0;
            }
        }
        public bool OverAged
        {
            get
            {
                return maxAge >= 0 && age > maxAge || age < 0;
            }
        }
        public Scalar MaxAge
        {
            get { return maxAge; }
            set { maxAge = value; }
        }
        public Scalar TimeLeft
        {
            get
            {
                return (maxAge > 0) ? (maxAge - age) : (Scalar.PositiveInfinity);
            }
        }
        public Scalar Age
        {
            get
            {
                return age;
            }
            set
            {
                age = value;
            }
        } 
        #endregion
        #region methods
        public void Update(Scalar dt)
        {
            age += dt;
        }
        public Lifespan Duplicate()
        {
            return new Lifespan(this.age, this.maxAge, this.master);
        }
        #endregion
    }
}
