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
using System.Collections.Generic;

using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    [Serializable]
    public sealed class GravityPointMass : PhysicsLogic
    {
        static Lifespan GetLifeSpan(Body source, Lifespan lifetime)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (lifetime == null) { throw new ArgumentNullException("lifeTime"); }
            return new Lifespan(lifetime.Age, lifetime.MaxAge, source.Lifetime);
        }

        Scalar metersPerDistanceUnit;
        Body source;
        public GravityPointMass(Body source, Scalar metersPerDistanceUnit, Lifespan lifetime)
            : base(GetLifeSpan(source, lifetime))
        {
            if (metersPerDistanceUnit <= 0) { throw new ArgumentOutOfRangeException("metersPerDistanceUnit"); }
            this.source = source;
            this.metersPerDistanceUnit = metersPerDistanceUnit;
        }
        protected internal override void RunLogic(Scalar dt)
        {
            foreach (Body e in Bodies)
            {
                if (e != source && !e.IgnoresGravity)
                {
                    Scalar magnitude;
                    Vector2D gravity;
                    Vector2D.Subtract(ref source.State.Position.Linear, ref e.State.Position.Linear, out gravity);
                    Vector2D.Normalize(ref gravity, out magnitude, out gravity);
                    magnitude = (source.Mass.AccelerationDueToGravity /
                            (magnitude * magnitude * metersPerDistanceUnit * metersPerDistanceUnit));
                    Vector2D.Multiply(ref gravity, ref magnitude, out gravity);
                    Vector2D.Add(ref e.State.Acceleration.Linear, ref gravity, out e.State.Acceleration.Linear);
                }
            }
        }

        void OnSourceLifetimeChanged(object sender, EventArgs e)
        {
            this.Lifetime = GetLifeSpan(source, this.Lifetime);
        }
        protected override void OnAdded()
        {
            this.Lifetime = GetLifeSpan(source, this.Lifetime);
            this.source.LifetimeChanged += OnSourceLifetimeChanged;
        }
        protected override void OnRemoved(PhysicsEngine engine, bool wasPending)
        {
            if (!wasPending)
            {
                this.source.LifetimeChanged -= OnSourceLifetimeChanged;
            }
        }
    }

}