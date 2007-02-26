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
    public sealed class Lifespan : IDuplicateable<Lifespan>
    {
        #region fields
        Scalar age = 0;
        Scalar maxAge;
        Lifespan master;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a new Lifespan Instance that is Immortal.
        /// </summary>
        public Lifespan()
            : this(0, -1, null)
        { }
        /// <summary>
        /// Creates a new Lifespan Instance that is mortal.
        /// </summary>
        /// <param name="maxAge">How long the item will stay in the engine. (in seconds)</param>
        public Lifespan(Scalar maxAge)
            : this(0, maxAge, null)
        { }
        /// <summary>
        /// Creates a new Lifespan Instance that dies when it's master dies.
        /// </summary>
        /// <param name="master">The Master of this new Instance</param>
        public Lifespan(Lifespan master)
            : this(0, -1, master)
        { }
        /// <summary>
        /// Creates a new Lifespan Instance that dies when it's master dies or of old age.
        /// </summary>
        /// <param name="maxAge">How long the item will stay in the engine. (in seconds)</param>
        /// <param name="master">The Master of this new Instance</param>
        public Lifespan(Scalar maxAge, Lifespan master)
            : this(0, maxAge, master)
        { }
        /// <summary>
        /// Creates a new Lifespan Instance that dies when it's master dies or of old age that is already aged.
        /// </summary>
        /// <param name="age">the current age of the new Lifespan.</param>
        /// <param name="maxAge">How long the item will stay in the engine. (in seconds)</param>
        /// <param name="master">The Master of this new Instance</param>
        public Lifespan(Scalar age, Scalar maxAge, Lifespan master)
        {
            this.age = age;
            this.maxAge = maxAge;
            this.master = master;
        }
        #endregion
        #region properties
        /// <summary>
        /// Gets and Sets The Master of this Instance. This instance IsExpired the master is.
        /// </summary>
        public Lifespan Master
        {
            get { return master; }
            set { master = value; }
        }
        /// <summary>
        /// Gets and Sets if it IsExpired and should be removed from the engine.
        /// </summary>
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
        /// <summary>
        /// Gets and Sets if the Master IsExpired and should be removed from the engine.
        /// </summary>
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
        /// <summary>
        /// Gets if the only way for the object to leave the engine is for it to be set to expired.
        /// </summary>
        public bool IsImmortal
        {
            get
            {
                return maxAge < 0;
            }
        }
        /// <summary>
        /// Gets if it is expired becuase of old age.
        /// </summary>
        public bool OverAged
        {
            get
            {
                return maxAge >= 0 && age > maxAge || age < 0;
            }
        }
        /// <summary>
        /// Gets and Sets how long the object will stay in the engine.
        /// </summary>
        public Scalar MaxAge
        {
            get { return maxAge; }
            set { maxAge = value; }
        }
        /// <summary>
        /// Gets how much time the object has left.
        /// </summary>
        public Scalar TimeLeft
        {
            get
            {
                return (maxAge > 0) ? (maxAge - age) : (Scalar.PositiveInfinity);
            }
        }
        /// <summary>
        /// Gets and Sets The current age of the object.
        /// </summary>
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
        /// <summary>
        /// Increases the Age of object by a change in time.
        /// </summary>
        /// <param name="dt">the amount of time passed since the last call.</param>
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
