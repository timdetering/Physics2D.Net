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



// because this code was basically copied from Box2D
// Copyright (c) 2006 Erin Catto http://www.gphysics.com
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
    public sealed class PivotJoint : Joint, Solvers.ISequentialImpulsesJoint
    {
        static ALVector2D Zero1 = ALVector2D.Zero;
        static Vector2D Zero2 = Vector2D.Zero;
        Solvers.SequentialImpulsesSolver solver;
        Body body1;


        Matrix2x2 M;
        Vector2D localAnchor1;
        Vector2D anchor;
        Vector2D r1;
        Vector2D bias;
        Vector2D accumulatedImpulse;
        Scalar biasFactor;
        Scalar softness;


        public PivotJoint(Body body1, Vector2D anchor, Lifespan lifetime)
            : base(lifetime)
        {
            if (body1 == null) { throw new ArgumentNullException("body1"); }
            this.body1 = body1;
            this.anchor = anchor;
            body1.ApplyMatrix();
            Matrix3x3 matrix1 = body1.Shape.MatrixInv.VertexMatrix;
            Vector2D.Transform(ref matrix1, ref anchor, out localAnchor1);
            softness = 0.001f;
            biasFactor = 0.2f;
        }
        public Vector2D Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }

        public Scalar BiasFactor
        {
            get { return biasFactor; }
            set { biasFactor = value; }
        }
        public Scalar Softness
        {
            get { return softness; }
            set { softness = value; }
        }
        public override Body[] Bodies
        {
            get { return new Body[1] { body1 }; }
        }
        protected override void OnAdded()
        {
            this.solver = (Solvers.SequentialImpulsesSolver)Engine.Solver;
        }
        void Solvers.ISequentialImpulsesJoint.PreStep(Scalar dtInv)
        {

            Scalar mass1Inv = body1.Mass.MassInv;
            Scalar inertia1Inv = body1.Mass.MomentofInertiaInv;

            // Pre-compute anchors, mass matrix, and bias.
            Matrix2x2 matrix1 = body1.Shape.Matrix.NormalMatrix;

            Vector2D.Transform(ref matrix1, ref localAnchor1, out r1);

            // deltaV = deltaV0 + K * impulse
            // invM = [(1/m1 + 1/m2) * eye(2) - skew(r1) * invI1 * skew(r1) - skew(r2) * invI2 * skew(r2)]
            //      = [1/m1+1/m2     0    ] + invI1 * [r1.y*r1.y -r1.x*r1.y] + invI2 * [r1.y*r1.y -r1.x*r1.y]
            //        [    0     1/m1+1/m2]           [-r1.x*r1.y r1.x*r1.x]           [-r1.x*r1.y r1.x*r1.x]

            Matrix2x2 K;
            K.m00 = mass1Inv;
            K.m11 = mass1Inv;

            K.m00 += inertia1Inv * r1.Y * r1.Y;
            K.m01 = -inertia1Inv * r1.X * r1.Y;
            K.m10 = -inertia1Inv * r1.X * r1.Y;
            K.m11 += inertia1Inv * r1.X * r1.X;

            K.m00 += softness;
            K.m11 += softness;

            Matrix2x2.Invert(ref K, out M);


            Vector2D dp;
            Vector2D.Add(ref body1.State.Position.Linear, ref r1, out dp);
            Vector2D.Subtract(ref anchor, ref dp, out dp);


            if (solver.PositionCorrection)
            {
                //bias = -0.1f * dtInv * dp;
                Scalar flt = -biasFactor * dtInv;
                Vector2D.Multiply(ref dp, ref flt, out bias);
            }
            else
            {
                bias = Vector2D.Zero;
            }
            if (solver.WarmStarting)
            {
                PhysicsHelper.SubtractImpulse(
                    ref body1.State.Velocity, ref accumulatedImpulse,
                    ref r1, ref mass1Inv, ref inertia1Inv);
            }
            else
            {
                accumulatedImpulse = Vector2D.Zero;
            }
        }
        void Solvers.ISequentialImpulsesJoint.ApplyImpulse()
        {
            Scalar mass1Inv = body1.Mass.MassInv;
            Scalar inertia1Inv = body1.Mass.MomentofInertiaInv;

            Vector2D dv;
            PhysicsHelper.GetRelativeVelocity(
                ref body1.State.Velocity, ref Zero1,
                ref r1, ref Zero2, out dv);

            Vector2D impulse;
            Vector2D vect1;
            Vector2D.Multiply(ref softness, ref accumulatedImpulse, out vect1);
            Vector2D.Subtract(ref bias, ref dv, out impulse);
            Vector2D.Subtract(ref impulse, ref vect1, out impulse);
            Vector2D.Transform(ref  M, ref impulse, out impulse);
            //impulse = M * (bias - dv - softness * P);

            PhysicsHelper.SubtractImpulse(
                ref body1.State.Velocity, ref impulse,
                ref r1, ref mass1Inv, ref inertia1Inv);


            Vector2D.Add(ref accumulatedImpulse, ref impulse, out accumulatedImpulse);
        }
    }
}