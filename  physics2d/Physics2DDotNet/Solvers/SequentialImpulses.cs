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


namespace Physics2DDotNet.Solvers
{



    public sealed class SequentialImpulsesSolver : CollisionSolver
    {
        sealed class SITag
        {
            public int CollisionCount;
        }
        sealed class Contact
        {
            public int id;
            public Vector2D position;
            public Vector2D normal;
            public Scalar distance;
            public Scalar Pn;
            public Scalar Pt;
            public Scalar Pnb;	// accumulated normal impulse for position bias
            public Scalar massNormal;
            public Scalar massTangent;
            public Scalar bias;


            public Vector2D r1;
            public Vector2D r2;
        }
        sealed class Arbiter
        {
            static void Collide(List<Contact> contacts, Body entity1, Body entity2)
            {
                BoundingBox2D bb1 = entity1.Shape.BoundingBox2D;
                BoundingBox2D bb2 = entity2.Shape.BoundingBox2D;
                BoundingBox2D targetArea;
                BoundingBox2D.FromIntersection(ref bb1, ref bb2, out targetArea);
                Collide(contacts, entity1, entity2, ref targetArea);
                Collide(contacts, entity2, entity1, ref targetArea);
            }
            static void Collide(List<Contact> contacts, Body entity1, Body entity2, ref BoundingBox2D targetArea)
            {
                if (!entity1.Shape.CanGetIntersection)
                {
                    return;
                }
                Vector2D[] vertexes = entity2.Shape.Vertices;
                Array.Resize<Vector2D>(ref vertexes, vertexes.Length + 2);
                vertexes[vertexes.Length - 2] = Vector2D.Zero;
                vertexes[vertexes.Length - 1] = Vector2D.XAxis;
                for (int index = 0; index < vertexes.Length; ++index)
                {
                    Vector2D vector = vertexes[index];
                    if (BoundingBox2D.TestIntersection(ref targetArea, ref vector))
                    {
                        IntersectionInfo info;
                        if (entity1.Shape.TryGetIntersection(vector, out info))
                        {

                            Contact contact = new Contact();
                            contact.normal = info.Normal;
                            if (Scalar.IsNaN(contact.normal.X) ||
                                Scalar.IsNaN(contact.normal.Y))
                            {
                                continue;
                            }
                            contact.distance = info.Distance;
                            contact.position = vector;
                            contact.id = index + 1;
                            if (entity1.ID < entity2.ID)
                            {
                                contact.id = -contact.id;
                            }
                            else
                            {
                                Vector2D.Negate(ref contact.normal, out contact.normal);
                            }
                            contacts.Add(contact);
                        }
                    }
                }
            }
            List<Contact> contacts;
            Body body1;
            Body body2;
            bool biasPreservesMomentum;
            Scalar biasFactor;
            Scalar allowedPenetration;
            Scalar friction;
            bool updated = false;
            public Arbiter(Body entity1, Body entity2, bool biasPreservesMomentum, Scalar biasFactor, Scalar allowedPenetration)
            {
                if (entity1.ID < entity2.ID)
                {
                    this.body1 = entity1;
                    this.body2 = entity2;
                }
                else
                {
                    this.body1 = entity2;
                    this.body2 = entity1;
                }
                this.friction = MathHelper.Sqrt(
                    this.body1.Coefficients.DynamicFriction *
                    this.body2.Coefficients.DynamicFriction);

                this.biasPreservesMomentum = biasPreservesMomentum;
                this.biasFactor = biasFactor;
                this.allowedPenetration = allowedPenetration;
            }
            public bool Updated
            {
                get { return updated; }
                set { updated = value; }
            }
            public int Count { get { return contacts.Count; } }
            int ContactCompare(Contact c1, Contact c2)
            {
                return c2.distance.CompareTo(c1.distance);
            }
            public void Update()
            {
                updated = true;
                List<Contact> newContacts = new List<Contact>();
                Collide(newContacts, body1, body2);
                if (contacts == null)
                {
                    this.contacts = newContacts;
                }
                else
                {
                    if (newContacts.Count == 0)
                    {
                        contacts.Clear();
                        return;
                    }
                    List<Contact> mergedContacts = new List<Contact>();
                    foreach (Contact contact in newContacts)
                    {
                        Contact oldcontact = contacts.Find(delegate(Contact old) { return old.id == contact.id; });
                        if (oldcontact != null)
                        {
                            contact.Pn = oldcontact.Pn;
                            contact.Pt = oldcontact.Pt;
                        }
                        mergedContacts.Add(contact);
                    }
                    contacts.Clear();
                    contacts = mergedContacts;
                }
                contacts.Sort(ContactCompare);
            }
            public void PreApply(Scalar dtInv)
            {

                Scalar biasMultiplier = 1 / MathHelper.Pow(
                    Math.Max(Math.Min(
                    ((SITag)body1.SolverTag).CollisionCount,
                    ((SITag)body2.SolverTag).CollisionCount) - 1,
                    1),
                    2);


                Scalar mass1Inv = body1.Mass.MassInv;
                Scalar I1Inv = body1.Mass.MomentofInertiaInv;
                Scalar mass2Inv = body2.Mass.MassInv;
                Scalar I2Inv = body2.Mass.MomentofInertiaInv;

                foreach (Contact c in contacts)
                {
                    Vector2D.Subtract(ref c.position, ref body1.State.Position.Linear, out c.r1);
                    Vector2D.Subtract(ref c.position, ref body2.State.Position.Linear, out c.r2);

                    // Precompute normal mass, tangent mass, and bias.
                    PhysicsHelper.GetMassNormal(
                        ref c.r1, ref c.r2,
                        ref c.normal,
                        ref mass1Inv, ref I1Inv,
                        ref mass2Inv, ref I2Inv,
                        out c.massNormal);

                    Vector2D tangent;
                    PhysicsHelper.GetTangent(ref c.normal, out tangent);

                    PhysicsHelper.GetMassNormal(
                        ref c.r1, ref c.r2,
                        ref tangent,
                        ref mass1Inv, ref I1Inv,
                        ref mass2Inv, ref I2Inv,
                        out c.massTangent);

                    c.bias = -(biasFactor * biasMultiplier) * dtInv * MathHelper.Min(0.0f, c.distance + allowedPenetration);


                    // Apply normal + friction impulse
                    Vector2D vect1, vect2, P;
                    Vector2D.Multiply(ref c.normal, ref c.Pn, out vect1);
                    Vector2D.Multiply(ref tangent, ref c.Pt, out vect2);
                    Vector2D.Add(ref vect1, ref vect2, out P);

                    PhysicsHelper.SubtractImpulse(
                        ref body1.State.Velocity,
                        ref P,
                        ref c.r1,
                        ref mass1Inv,
                        ref I1Inv);

                    PhysicsHelper.AddImpulse(
                        ref body2.State.Velocity,
                        ref P,
                        ref c.r2,
                        ref mass2Inv,
                        ref I2Inv);

                    // Initialize bias impulse to zero.
                    c.Pnb = 0;
                }
            }
            public void Apply()
            {
                Body b1 = body1;
                Body b2 = body2;

                Scalar mass1Inv = b1.Mass.MassInv;
                Scalar I1Inv = b1.Mass.MomentofInertiaInv;
                Scalar mass2Inv = b2.Mass.MassInv;
                Scalar I2Inv = b2.Mass.MomentofInertiaInv;

                foreach (Contact c in contacts)
                {

                    // Relative velocity at contact
                    Vector2D dv;
                    PhysicsHelper.GetRelativeVelocity(
                        ref b1.State.Velocity,
                        ref b2.State.Velocity,
                        ref c.r1, ref c.r2, out dv);

                    // Compute normal impulse
                    Scalar vn;
                    Vector2D.Dot(ref dv, ref c.normal, out vn);
                    //Scalar vn = Vector2D.Dot(dv, c.normal);

                    Scalar dPn;
                    if (biasPreservesMomentum)
                    {
                        dPn = c.massNormal * (-vn);
                    }
                    else
                    {
                        dPn = c.massNormal * (-vn + c.bias) ;
                    }

                    // Clamp the accumulated impulse
                    Scalar Pn0 = c.Pn;
                    c.Pn = MathHelper.Max(Pn0 + dPn, 0.0f);
                    dPn = c.Pn - Pn0;

                    // Apply contact impulse
                    Vector2D Pn;
                    Vector2D.Multiply(ref  c.normal, ref dPn, out Pn);
                    //Vector2D Pn = dPn * c.normal;


                    PhysicsHelper.SubtractImpulse(
                        ref b1.State.Velocity,
                        ref Pn,
                        ref c.r1,
                        ref mass1Inv,
                        ref I1Inv);

                    PhysicsHelper.AddImpulse(
                        ref b2.State.Velocity,
                        ref Pn,
                        ref c.r2,
                        ref mass2Inv,
                        ref I2Inv);


                    if (biasPreservesMomentum)
                    {
                        // Compute bias impulse
                        PhysicsHelper.GetRelativeVelocity(
                            ref b1.State.SolverVelocity,
                            ref b2.State.SolverVelocity,
                            ref c.r1, ref c.r2, out dv);


                        Scalar vnb;
                        Vector2D.Dot(ref dv, ref c.normal, out vnb);
                        //Scalar vnb = Vector2D.Dot(dv, c.normal);

                        Scalar dPnb = c.massNormal * (-vnb + c.bias);
                        Scalar Pnb0 = c.Pnb;
                        c.Pnb = MathHelper.Max(Pnb0 + dPnb, 0.0f);
                        dPnb = c.Pnb - Pnb0;

                        Vector2D Pb;
                        Vector2D.Multiply(ref dPnb, ref c.normal, out Pb);
                        //Vector2D Pb = dPnb * c.normal;


                        PhysicsHelper.SubtractImpulse(
                            ref b1.State.SolverVelocity,
                            ref Pb,
                            ref c.r1,
                            ref mass1Inv,
                            ref I1Inv);

                        PhysicsHelper.AddImpulse(
                            ref b2.State.SolverVelocity,
                            ref Pb,
                            ref c.r2,
                            ref mass2Inv,
                            ref I2Inv);
                    }

                    // Relative velocity at contact

                    PhysicsHelper.GetRelativeVelocity(
                        ref b1.State.Velocity,
                        ref b2.State.Velocity,
                        ref c.r1, ref c.r2, out dv);



                    // Compute friction impulse
                    Scalar maxPt = friction * c.Pn;
                    Vector2D tangent;
                    PhysicsHelper.GetTangent(ref c.normal, out tangent);

                    Scalar vt;
                    Vector2D.Dot(ref dv, ref tangent, out vt);
                    //Scalar vt = Vector2D.Dot(dv, tangent);
                    Scalar dPt = c.massTangent * (-vt);

                    // Clamp friction
                    Scalar oldTangentImpulse = c.Pt;
                    c.Pt = MathHelper.Clamp(oldTangentImpulse + dPt, -maxPt, maxPt);
                    dPt = c.Pt - oldTangentImpulse;

                    // Apply contact impulse
                    Vector2D Pt;
                    Vector2D.Multiply(ref tangent, ref dPt, out Pt);

                    //Vector2D Pt = dPt * tangent;

                    PhysicsHelper.SubtractImpulse(
                        ref b1.State.Velocity,
                        ref Pt,
                        ref c.r1,
                        ref mass1Inv,
                        ref I1Inv);

                    PhysicsHelper.AddImpulse(
                        ref b2.State.Velocity,
                        ref Pt,
                        ref c.r2,
                        ref mass2Inv,
                        ref I2Inv);
                }
            }
        }

        static bool IsJointRemoved(ISequentialImpulsesJoint joint)
        {
            return joint.Engine == null;
        }

        Dictionary<long, Arbiter> arbiters;
        List<ISequentialImpulsesJoint> siJoints;
        bool biasPreservesMomentum = true;
        Scalar biasFactor = 0.8f;
        Scalar allowedPenetration = 0.01f;
        int iterations = 10;

        public SequentialImpulsesSolver()
        {
            arbiters = new Dictionary<long, Arbiter>();
            siJoints = new List<ISequentialImpulsesJoint>();
        }


        public bool BiasPreservesMomentum
        {
            get { return biasPreservesMomentum; }
            set { biasPreservesMomentum = value; }
        }
        public Scalar BiasFactor
        {
            get { return biasFactor; }
            set { biasFactor = value; }
        }
        public Scalar AllowedPenetration
        {
            get { return allowedPenetration; }
            set { allowedPenetration = value; }
        }
        public int Iterations
        {
            get { return iterations; }
            set { iterations = value; }
        }

        protected internal override bool HandleCollision(Scalar dt, Body first, Body second)
        {
            Body entity1 = first;
            Body entity2 = second;
            long id = PairID.GetId(entity1.ID, entity2.ID);
            Arbiter arbiter;
            if (arbiters.TryGetValue(id, out arbiter))
            {
                arbiter.Update();
                if (arbiter.Count == 0)
                {

                    arbiters.Remove(id);
                }
            }
            else
            {
                arbiter = new Arbiter(entity1, entity2, biasPreservesMomentum, biasFactor, allowedPenetration);
                arbiter.Update();
                if (!entity1.IgnoresCollisionResponse &&
                    !entity2.IgnoresCollisionResponse &&
                    arbiter.Count > 0)
                {
                    arbiters.Add(id, arbiter);
                }
            }
            bool rv = arbiter.Count > 0;
            if (rv)
            {
                ((SITag)first.SolverTag).CollisionCount++;
                ((SITag)second.SolverTag).CollisionCount++;
            }
            return rv;
        }
        void RemoveEmpty()
        {
            List<long> empty = new List<long>();
            foreach (KeyValuePair<long, Arbiter> pair in arbiters)
            {
                if (pair.Value.Count == 0 || !pair.Value.Updated)
                {
                    empty.Add(pair.Key);
                }
            }
            foreach (long key in empty)
            {
                arbiters.Remove(key);
            }
        }

        protected internal override void Solve(Scalar dt)
        {
            Scalar dtInv = (dt > 0.0f) ? (1.0f / dt) : (0.0f);
            foreach (Arbiter arb in arbiters.Values)
            {
                arb.Updated = false;
            }
            Detect(dt);
            RemoveEmpty();
            this.Engine.RunLogic(dt);
            foreach (Body item in this.Bodies)
            {
                item.UpdateVelocity(dt);
            }

            int ArbCount = arbiters.Count;
            Arbiter[] arbs = new Arbiter[ArbCount];
            arbiters.Values.CopyTo(arbs, 0);
            for (int index = 0; index < ArbCount; ++index)
            {
                arbs[index].PreApply(dtInv);
            }
            foreach (ISequentialImpulsesJoint joint in siJoints)
            {
                joint.PreApply(dtInv);
            }
            for (int i = 0; i < iterations; ++i)
            {
                for (int index = 0; index < ArbCount; ++index)
                {
                    arbs[index].Apply();
                }
                foreach (ISequentialImpulsesJoint joint in siJoints)
                {
                    joint.Apply();
                }
            }
            foreach (Body item in this.Bodies)
            {
                if (biasPreservesMomentum)
                {
                    item.UpdatePosition(dt, ref item.State.SolverVelocity);
                    item.State.SolverVelocity = ALVector2D.Zero;
                }
                else
                {
                    item.UpdatePosition(dt);
                }
                item.ClearForces();
                ((SITag)item.SolverTag).CollisionCount = 0;
            }
        }
        protected internal override void AddBodyRange(List<Body> collection)
        {
            foreach (Body item in collection)
            {
                SetTag(item, new SITag());
            }
        }
        protected internal override void AddJointRange(List<Joint> collection)
        {
            ISequentialImpulsesJoint[] newJoints = new ISequentialImpulsesJoint[collection.Count];
            for (int index = 0; index < newJoints.Length; ++index)
            {
                newJoints[index] = (ISequentialImpulsesJoint)collection[index];
            }
            siJoints.AddRange(newJoints);
        }
        protected internal override void Clear()
        {
            arbiters.Clear();
            siJoints.Clear();
        }
        protected internal override void RemoveExpiredJoints()
        {
            siJoints.RemoveAll(IsJointRemoved);
        }
        protected internal override void CheckJoint(Joint joint)
        {
            if (!(joint is ISequentialImpulsesJoint))
            {
                throw new ArgumentException("The joint must impliment ISequentialImpulsesJoint to be added to this solver.");
            }
        }
    }
}