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

namespace Physics2DDotNet
{

    public sealed class GlobalFluidLogic : PhysicsLogic
    {
        sealed class Wrapper : IDisposable
        {
            public Vector2D centroid;
            public Scalar area;
            public Body body;
            public bool valid;
            public Wrapper(Body body)
            {
                this.body = body;
                this.body.ShapeChanged += OnShapeChanged;
                this.Calculate();
            }
            void Calculate()
            {
                CalculatePart();
                if (Math.Abs(this.centroid.X) < .00001f)
                {
                    this.centroid.X = 0;
                }
                if (Math.Abs(this.centroid.Y) < .00001f)
                {
                    this.centroid.Y = 0;
                }
            }
            void CalculatePart()
            {
                Shape shape = body.Shape;
                if (!shape.CanGetDragInfo ||
                    !shape.CanGetArea ||
                    !shape.CanGetCentroid)
                {
                    valid = false;
                    area = 0;
                    centroid = Vector2D.Zero;
                    return;
                }
                valid = true;
                area = shape.GetArea();
                centroid = shape.GetCentroid();
            }
            void OnShapeChanged(object sender, EventArgs e)
            {
                Calculate();
            }
            public void Dispose()
            {
                this.body.ShapeChanged -= OnShapeChanged;
            }
        }
        static bool IsRemoved(Wrapper wrapper)
        {
            if (wrapper.body.IsAdded)
            {
                wrapper.Dispose();
                return true;
            }
            return false;
        }

        Scalar dragCoefficient;
        Scalar density;
        Vector2D fluidVelocity;
        List<Wrapper> items;

        public GlobalFluidLogic(
            Scalar dragCoefficient,
            Scalar density,
            Vector2D fluidVelocity,
            Lifespan lifetime)
            : base(lifetime)
        {
            this.dragCoefficient = dragCoefficient;
            this.density = density;
            this.fluidVelocity = fluidVelocity;
            this.items = new List<Wrapper>();
            this.Order = 1;
        }
        protected internal override void RunLogic(TimeStep step)
        {
            for (int index = 0; index < items.Count; ++index)
            {
                Wrapper wrapper = items[index];
                if (!wrapper.valid) { continue; }
                Body body = wrapper.body;

                Vector2D centroid = wrapper.body.Shape.Matrix.NormalMatrix * wrapper.centroid;
                Vector2D buoyancyForce = body.State.Acceleration.Linear * wrapper.area * -density;
                wrapper.body.ApplyForce(buoyancyForce, centroid);

                Vector2D relativeVelocity = body.State.Velocity.Linear - fluidVelocity;
                Vector2D velocityDirection = relativeVelocity.Normalized;
                if (velocityDirection == Vector2D.Zero) { continue; }
                Vector2D dragDirection = velocityDirection.LeftHandNormal;
                DragInfo dragInfo = body.Shape.GetDragInfo(dragDirection);
                if (dragInfo.Area < .01f) { continue; }
                Scalar speedSq = relativeVelocity.MagnitudeSq;
                Scalar dragForceMag = -.5f * density * speedSq * dragInfo.Area * dragCoefficient;
                Scalar maxDrag = -MathHelper.Sqrt(speedSq) * body.Mass.Mass * step.DtInv;
                if (dragForceMag < maxDrag)
                {
                    dragForceMag = maxDrag;
                }

                Vector2D dragForce = dragForceMag * velocityDirection;
                wrapper.body.ApplyForce(dragForce, dragInfo.Center);

                wrapper.body.ApplyTorque(
                   -body.Mass.MomentOfInertia *
                   (body.Coefficients.DynamicFriction + density + dragCoefficient) *
                   body.State.Velocity.Angular);
            }
        }
        protected internal override void RemoveExpiredBodies()
        {
            items.RemoveAll(IsRemoved);
        }
        protected internal override void AddBodyRange(List<Body> collection)
        {
            int newCapacity = collection.Count + items.Count;
            if (items.Capacity < newCapacity)
            {
                items.Capacity = newCapacity;
            }
            for (int index = 0; index < collection.Count; ++index)
            {
                items.Add(new Wrapper(collection[index]));
            }
        }
        protected internal override void Clear()
        {
            for(int index = 0; index < items.Count;++index)
            {
                items[index].Dispose();
            }
            items.Clear();
        }
    }
}