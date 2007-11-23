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
using System.Collections.ObjectModel;

using AdvanceMath;






namespace Physics2DDotNet
{

    /// <summary>
    /// A Joint Between 2 Bodies that will pivot around an Anchor.
    /// </summary>
    [Serializable]
    public sealed class HingeJoint : Joint, Solvers.ISequentialImpulsesJoint
    {
        Solvers.SequentialImpulsesSolver solver;
        Body body1;
        Body body2;
        Matrix2x2 M;
        Vector2D localAnchor1, localAnchor2;
        Vector2D r1, r2;
        Vector2D bias;
        Vector2D accumulatedImpulse;
        Scalar biasFactor;
        Scalar softness;

        /// <summary>
        /// Creates a new HingeJoint Instance.
        /// </summary>
        /// <param name="body1">One of the bodies to be Jointed.</param>
        /// <param name="body2">One of the bodies to be Jointed.</param>
        /// <param name="anchor">The location of the Hinge.</param>
        /// <param name="lifeTime">A object Describing how long the object will be in the engine.</param>
        public HingeJoint(Body body1, Body body2, Vector2D anchor, Lifespan lifetime)
            : base(lifetime)
        {
            if (body1 == null) { throw new ArgumentNullException("body1"); }
            if (body2 == null) { throw new ArgumentNullException("body2"); }
            if (body1 == body2) { throw new ArgumentException("You cannot add a joint to a body to itself"); }
            this.body1 = body1;
            this.body2 = body2;
            body1.ApplyMatrix();
            body2.ApplyMatrix();

            Matrix3x3 matrix1 = body1.Shape.MatrixInv.VertexMatrix;
            Matrix3x3 matrix2 = body2.Shape.MatrixInv.VertexMatrix;
            Vector2D.Transform(ref matrix1, ref anchor, out localAnchor1);
            Vector2D.Transform(ref matrix2, ref anchor, out localAnchor2);

            softness = 0.001f;
            biasFactor = 0.2f;
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
        public override ReadOnlyCollection<Body> Bodies
        {
            get { return new ReadOnlyCollection<Body>(new Body[2] { body1, body2 }); }
        }
        protected override void OnAdded()
        {
            this.solver = (Solvers.SequentialImpulsesSolver)Engine.Solver;
        }
        void Solvers.ISequentialImpulsesJoint.PreStep(Scalar dtInv)
        {

            Scalar mass1Inv = body1.Mass.MassInv;
            Scalar mass2Inv = body2.Mass.MassInv;
            Scalar inertia1Inv = body1.Mass.MomentOfInertiaInv;
            Scalar inertia2Inv = body2.Mass.MomentOfInertiaInv;

            // Pre-compute anchors, mass matrix, and bias.
            Matrix2x2 matrix1 = body1.Shape.Matrix.NormalMatrix;
            Matrix2x2 matrix2 = body2.Shape.Matrix.NormalMatrix;


            Vector2D.Transform(ref matrix1, ref localAnchor1, out r1);
            Vector2D.Transform(ref matrix2, ref localAnchor2, out r2);

            // deltaV = deltaV0 + K * impulse
            // invM = [(1/m1 + 1/m2) * eye(2) - skew(r1) * invI1 * skew(r1) - skew(r2) * invI2 * skew(r2)]
            //      = [1/m1+1/m2     0    ] + invI1 * [r1.y*r1.y -r1.x*r1.y] + invI2 * [r1.y*r1.y -r1.x*r1.y]
            //        [    0     1/m1+1/m2]           [-r1.x*r1.y r1.x*r1.x]           [-r1.x*r1.y r1.x*r1.x]

            Matrix2x2 K;
            K.m00 = mass1Inv + mass2Inv;
            K.m11 = mass1Inv + mass2Inv;

            K.m00 += inertia1Inv * r1.Y * r1.Y;
            K.m01 = -inertia1Inv * r1.X * r1.Y;
            K.m10 = -inertia1Inv * r1.X * r1.Y;
            K.m11 += inertia1Inv * r1.X * r1.X;

            K.m00 += inertia2Inv * r2.Y * r2.Y;
            K.m01 -= inertia2Inv * r2.X * r2.Y;
            K.m10 -= inertia2Inv * r2.X * r2.Y;
            K.m11 += inertia2Inv * r2.X * r2.X;


            K.m00 += softness;
            K.m11 += softness;

            Matrix2x2.Invert(ref K, out M);


            Vector2D dp, vect1, vect2;
            Vector2D.Add(ref body1.State.Position.Linear, ref r1, out vect1);
            Vector2D.Add(ref body2.State.Position.Linear, ref r2, out vect2);
            Vector2D.Subtract(ref vect2, ref vect1, out dp);


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

                PhysicsHelper.AddImpulse(
                    ref body2.State.Velocity, ref accumulatedImpulse,
                    ref r2, ref mass2Inv, ref inertia2Inv);
            }
            else
            {
                accumulatedImpulse = Vector2D.Zero;
            }
            body1.ApplyProxy();
            body2.ApplyProxy();
        }
        void Solvers.ISequentialImpulsesJoint.ApplyImpulse()
        {

            Scalar mass1Inv = body1.Mass.MassInv;
            Scalar mass2Inv = body2.Mass.MassInv;
            Scalar inertia1Inv = body1.Mass.MomentOfInertiaInv;
            Scalar inertia2Inv = body2.Mass.MomentOfInertiaInv;


            Vector2D dv;
            PhysicsHelper.GetRelativeVelocity(
                ref body1.State.Velocity, ref body2.State.Velocity,
                ref r1, ref r2, out dv);



            Vector2D impulse, vect1;
            Vector2D.Multiply(ref softness, ref accumulatedImpulse, out vect1);
            Vector2D.Subtract(ref bias, ref dv, out impulse);
            Vector2D.Subtract(ref impulse, ref vect1, out impulse);
            Vector2D.Transform(ref  M, ref impulse, out impulse);
            //impulse = M * (bias - dv - softness * P);

            PhysicsHelper.SubtractImpulse(
                ref body1.State.Velocity, ref impulse,
                ref r1, ref mass1Inv, ref inertia1Inv);

            PhysicsHelper.AddImpulse(
                ref body2.State.Velocity, ref impulse,
                ref r2, ref mass2Inv, ref inertia2Inv);

            Vector2D.Add(ref accumulatedImpulse, ref impulse, out accumulatedImpulse);

            body1.ApplyProxy();
            body2.ApplyProxy();
        }
    }
}