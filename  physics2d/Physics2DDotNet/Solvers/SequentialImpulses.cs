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
using System.Collections.Generic;

using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet.Math2D;


namespace Physics2DDotNet.Solvers
{

    sealed class SequentialImpulsesTag
    {
        public ALVector2D biasVelocity;
        public Body body;
        public SequentialImpulsesTag(Body body)
        {
            this.body = body;
        }
    }


    public sealed class SequentialImpulsesSolver : CollisionSolver
    {
        sealed class Contact : IContactInfo
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
            public Scalar restitution;


            public Vector2D r1;
            public Vector2D r2;


            Vector2D IContactInfo.Position
            {
                get { return position; }
            }
            Vector2D IContactInfo.Normal
            {
                get { return normal; }
            }
            Scalar IContactInfo.Distance
            {
                get { return distance; }
            }
        }
        sealed class Arbiter : ICollisionInfo
        {
            static void Collide(List<Contact> contacts, Body body1, Body body2)
            {
                BoundingRectangle bb1 = body1.Shape.Rectangle;
                BoundingRectangle bb2 = body2.Shape.Rectangle;
                BoundingRectangle targetArea;
                BoundingRectangle.FromIntersection(ref bb1, ref bb2, out targetArea);
                Collide(contacts, body1, body2, ref targetArea);
                Collide(contacts, body2, body1, ref targetArea);
            }
            static void Collide(List<Contact> contacts, Body body1, Body body2, ref BoundingRectangle targetArea)
            {
                if (!body1.Shape.CanGetIntersection)
                {
                    return;
                }
                Vector2D[] vertexes = body2.Shape.Vertices;
                for (int index = 0; index < vertexes.Length; ++index)
                {
                    Vector2D vector = vertexes[index];
                    ContainmentType contains;
                    targetArea.Contains(ref vector, out contains);
                    if (contains == ContainmentType.Contains)
                    {
                        IntersectionInfo info;
                        if (body1.Shape.TryGetIntersection(vector, out info))
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
                            if (body1.ID < body2.ID)
                            {
                                contact.id = vertexes.Length - index;
                            }
                            else
                            {
                                contact.id = index;
                                Vector2D.Negate(ref contact.normal, out contact.normal);
                            }
                            contacts.Add(contact);
                        }
                    }
                }
            }
            static Scalar ZeroClamp(Scalar value)
            {
                return ((value < 0) ? (0) : (value));
            }
            static readonly Contact[] Empty = new Contact[0];

            Contact[] contacts;
            Body body1;
            Body body2;
            SequentialImpulsesTag tag1;
            SequentialImpulsesTag tag2;

            SequentialImpulsesSolver parent;
            Scalar restitution;

            Scalar friction;
            bool updated = false;
            public Arbiter(SequentialImpulsesSolver parent, Body body1, Body body2)
            {
                if (body1.ID < body2.ID)
                {
                    this.body1 = body1;
                    this.body2 = body2;
                    this.tag1 = (SequentialImpulsesTag)body1.SolverTag;
                    this.tag2 = (SequentialImpulsesTag)body2.SolverTag;
                }
                else
                {
                    this.body1 = body2;
                    this.body2 = body1;
                    this.tag1 = (SequentialImpulsesTag)body2.SolverTag;
                    this.tag2 = (SequentialImpulsesTag)body1.SolverTag;
                }
                this.friction = MathHelper.Sqrt(
                    this.body1.Coefficients.DynamicFriction *
                    this.body2.Coefficients.DynamicFriction);
                this.restitution = Math.Min(body1.Coefficients.Restitution, body2.Coefficients.Restitution);
                this.parent = parent;
            }
            public bool Updated
            {
                get { return updated; }
                set { updated = value; }
            }

            public void Update()
            {
                updated = true;
                List<Contact> newContacts;
                bool isEmpty = null == contacts || Empty == contacts;
                if (isEmpty)
                {
                    newContacts = new List<Contact>();
                }
                else
                {
                    newContacts = new List<Contact>(contacts.Length);
                }
                Collide(newContacts, body1, body2);
                if (newContacts.Count == 0)
                {
                    this.contacts = Empty;
                }
                else if (isEmpty)
                {
                    this.contacts = newContacts.ToArray();
                }
                else
                {
                    Contact[] mergedContacts = newContacts.ToArray();
                    if (parent.warmStarting)
                    {
                        for (int index = 0, oldIndex = 0; index < mergedContacts.Length && oldIndex < contacts.Length; )
                        {
                            Contact oldC = contacts[oldIndex];
                            Contact newC = mergedContacts[index];
                            if (oldC.id == newC.id)
                            {
                                newC.Pn = oldC.Pn;
                                newC.Pt = oldC.Pt;
                                newC.Pnb = oldC.Pnb;
                                ++oldIndex;
                                ++index;
                            }
                            else if (oldC.id < newC.id) { ++oldIndex; }
                            else { ++index; }
                        }
                    }
                    this.contacts = mergedContacts;
                }
            }
            static Random rand = new Random();
            public void PreApply(Scalar dtInv)
            {


                Scalar mass1Inv = body1.Mass.MassInv;
                Scalar I1Inv = body1.Mass.MomentofInertiaInv;
                Scalar mass2Inv = body2.Mass.MassInv;
                Scalar I2Inv = body2.Mass.MomentofInertiaInv;

                for (int index = 0; index < contacts.Length; ++index)
                {
                    Contact c = contacts[index];
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

                    if (parent.positionCorrection)
                    {
                        c.bias = -parent.biasFactor * dtInv * Math.Min(0.0f, c.distance + parent.allowedPenetration);
                    }
                    else
                    {
                        c.bias = 0;
                    }
                    if (parent.accumulateImpulses)
                    {
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
                    }

                    if (this.restitution == 0)
                    {
                        c.restitution = 0;
                    }
                    else
                    {
                        // sets up the restitution
                        Scalar vn;
                        Vector2D rv;
                        PhysicsHelper.GetRelativeVelocity(
                            ref body1.State.Velocity, ref body2.State.Velocity,
                            ref c.r1, ref c.r2, out rv);
                        Vector2D.Dot(ref c.normal, ref rv, out vn);
                        c.restitution = -vn * this.restitution;
                    }
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

                PhysicsState state1 = b1.State;
                PhysicsState state2 = b2.State;

                for (int index = 0; index < contacts.Length; ++index)
                {
                    Contact c = contacts[index];

                    // Relative velocity at contact
                    Vector2D dv;
                    PhysicsHelper.GetRelativeVelocity(
                        ref state1.Velocity,
                        ref state2.Velocity,
                        ref c.r1, ref c.r2, out dv);

                    // Compute normal impulse
                    Scalar vn;
                    Vector2D.Dot(ref dv, ref c.normal, out vn);
                    //Scalar vn = Vector2D.Dot(dv, c.normal);

                    Scalar dPn;
                    if (parent.splitImpulse)
                    {
                        dPn = c.massNormal * (c.restitution - vn);
                    }
                    else
                    {
                        dPn = c.massNormal * (c.restitution - vn + c.bias);
                    }


                    if (parent.accumulateImpulses)
                    {
                        // Clamp the accumulated impulse
                        Scalar Pn0 = c.Pn;
                        c.Pn = ZeroClamp(Pn0 + dPn);
                        //c.Pn = Math.Max(Pn0 + dPn, 0.0f);
                        dPn = c.Pn - Pn0;
                    }
                    else
                    {
                        //dPn = Math.Max(dPn, 0.0f);
                        dPn = ZeroClamp(dPn);
                    }

                    // Apply contact impulse
                    Vector2D Pn;
                    Vector2D.Multiply(ref  c.normal, ref dPn, out Pn);
                    //Vector2D Pn = dPn * c.normal;

                    PhysicsHelper.SubtractImpulse(
                        ref state1.Velocity,
                        ref Pn,
                        ref c.r1,
                        ref mass1Inv,
                        ref I1Inv);

                    PhysicsHelper.AddImpulse(
                        ref state2.Velocity,
                        ref Pn,
                        ref c.r2,
                        ref mass2Inv,
                        ref I2Inv);


                    if (parent.splitImpulse)
                    {
                        // Compute bias impulse
                        PhysicsHelper.GetRelativeVelocity(
                            ref tag1.biasVelocity,
                            ref tag2.biasVelocity,
                            ref c.r1, ref c.r2, out dv);



                        Scalar vnb;
                        Vector2D.Dot(ref dv, ref c.normal, out vnb);
                        //Scalar vnb = Vector2D.Dot(dv, c.normal);

                        Scalar dPnb = c.massNormal * (c.bias - vnb);
                        Scalar Pnb0 = c.Pnb;
                        c.Pnb = ZeroClamp(Pnb0 + dPnb);
                        // c.Pnb = Math.Max(Pnb0 + dPnb, 0.0f);
                        dPnb = c.Pnb - Pnb0;

                        Vector2D Pb;
                        Vector2D.Multiply(ref dPnb, ref c.normal, out Pb);
                        //Vector2D Pb = dPnb * c.normal;


                        PhysicsHelper.SubtractImpulse(
                            ref tag1.biasVelocity,
                            ref Pb,
                            ref c.r1,
                            ref mass1Inv,
                            ref I1Inv);

                        PhysicsHelper.AddImpulse(
                            ref tag2.biasVelocity,
                            ref Pb,
                            ref c.r2,
                            ref mass2Inv,
                            ref I2Inv);
                    }

                    // Relative velocity at contact

                    PhysicsHelper.GetRelativeVelocity(
                        ref state1.Velocity,
                        ref state2.Velocity,
                        ref c.r1, ref c.r2, out dv);


                    Vector2D tangent;
                    PhysicsHelper.GetTangent(ref c.normal, out tangent);

                    Scalar vt;
                    Vector2D.Dot(ref dv, ref tangent, out vt);
                    //Scalar vt = Vector2D.Dot(dv, tangent);
                    Scalar dPt = c.massTangent * (-vt);




                    if (parent.accumulateImpulses)
                    {
                        // Compute friction impulse
                        Scalar maxPt = friction * c.Pn;
                        // Clamp friction
                        Scalar oldTangentImpulse = c.Pt;
                        c.Pt = MathHelper.Clamp(oldTangentImpulse + dPt, -maxPt, maxPt);
                        dPt = c.Pt - oldTangentImpulse;
                    }
                    else
                    {
                        // Compute friction impulse
                        Scalar maxPt = friction * dPn;
                        dPt = MathHelper.Clamp(dPt, -maxPt, maxPt);
                    }


                    // Apply contact impulse
                    Vector2D Pt;
                    Vector2D.Multiply(ref tangent, ref dPt, out Pt);

                    //Vector2D Pt = dPt * tangent;

                    PhysicsHelper.SubtractImpulse(
                        ref state1.Velocity,
                        ref Pt,
                        ref c.r1,
                        ref mass1Inv,
                        ref I1Inv);

                    PhysicsHelper.AddImpulse(
                        ref state2.Velocity,
                        ref Pt,
                        ref c.r2,
                        ref mass2Inv,
                        ref I2Inv);
                }
            }
            public bool Collided
            {
                get { return contacts.Length > 0; }
            }
            ReadOnlyCollection<IContactInfo> ICollisionInfo.Contacts
            {
                get
                {
                    return new ReadOnlyCollection<IContactInfo>(
                        new Physics2DDotNet.Collections.ImplicitCastCollection<IContactInfo, Contact>(contacts));
                }
            }
        }

        static bool IsJointRemoved(ISequentialImpulsesJoint joint)
        {
            return !joint.IsAdded;
        }
        static bool IsTagRemoved(SequentialImpulsesTag tag)
        {
            return !tag.body.IsAdded;
        }





        Dictionary<long, Arbiter> arbiters;
        List<ISequentialImpulsesJoint> siJoints;
        List<SequentialImpulsesTag> tags;
        bool splitImpulse = true;
        bool accumulateImpulses = true;
        bool warmStarting = true;
        bool positionCorrection = true;




        Scalar biasFactor = 0.8f;
        Scalar allowedPenetration = 0.1f;
        int iterations = 10;




        public SequentialImpulsesSolver()
        {
            arbiters = new Dictionary<long, Arbiter>();
            siJoints = new List<ISequentialImpulsesJoint>();
            tags = new List<SequentialImpulsesTag>();
        }

        public bool PositionCorrection
        {
            get { return positionCorrection; }
            set { positionCorrection = value; }
        }
        public bool AccumulateImpulses
        {
            get { return accumulateImpulses; }
            set { accumulateImpulses = value; }
        }
        public bool SplitImpulse
        {
            get { return splitImpulse; }
            set { splitImpulse = value; }
        }
        public bool WarmStarting
        {
            get { return warmStarting; }
            set { warmStarting = value; }
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

        protected internal override ICollisionInfo HandleCollision(Scalar dt, Body first, Body second)
        {
            long id = PairID.GetId(first.ID, second.ID);
            Arbiter arbiter;
            if (arbiters.TryGetValue(id, out arbiter))
            {
                arbiter.Update();
                if (!arbiter.Collided)
                {
                    arbiters.Remove(id);
                }
            }
            else
            {
                arbiter = new Arbiter(this, first, second);
                arbiter.Update();
                if (!first.IgnoresCollisionResponse &&
                    !second.IgnoresCollisionResponse &&
                    arbiter.Collided)
                {
                    arbiters.Add(id, arbiter);
                }
            }
            return arbiter;
        }
        void RemoveEmpty()
        {
            List<long> empty = new List<long>();
            foreach (KeyValuePair<long, Arbiter> pair in arbiters)
            {
                Arbiter value = pair.Value;
                if (!value.Collided || !value.Updated)
                {
                    empty.Add(pair.Key);
                }
            }
            for (int index = 0; index < empty.Count; ++index)
            {
                arbiters.Remove(empty[index]);
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
            for (int index = 0; index < tags.Count; ++index)
            {
                SequentialImpulsesTag tag = tags[index];
                tag.biasVelocity = ALVector2D.Zero;
                tag.body.UpdateVelocity(dt);
            }

            Arbiter[] arbs = new Arbiter[arbiters.Count];
            arbiters.Values.CopyTo(arbs, 0);
            for (int index = 0; index < arbs.Length; ++index)
            {
                arbs[index].PreApply(dtInv);
            }
            for (int index = 0; index < siJoints.Count; ++index)
            {
                siJoints[index].PreStep(dtInv);
            }
            for (int i = 0; i < iterations; ++i)
            {
                for (int index = 0; index < arbs.Length; ++index)
                {
                    arbs[index].Apply();
                }
                for (int index = 0; index < siJoints.Count; ++index)
                {
                    siJoints[index].ApplyImpulse();
                }
            }
            for (int index = 0; index < tags.Count; ++index)
            {
                SequentialImpulsesTag tag = tags[index];
                if (splitImpulse)
                {
                    tag.body.UpdatePosition(dt, ref tag.biasVelocity);
                }
                else
                {
                    tag.body.UpdatePosition(dt);
                }
                tag.body.ClearForces();
            }
        }
        protected internal override void AddBodyRange(List<Body> collection)
        {
            foreach (Body item in collection)
            {
                if (item.SolverTag == null)
                {
                    SequentialImpulsesTag tag = new SequentialImpulsesTag(item);
                    SetTag(item, tag);
                    tags.Add(tag);
                }
                else
                {
                    tags.Add((SequentialImpulsesTag)item.SolverTag);
                }
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
            tags.Clear();
        }
        protected internal override void RemoveExpiredJoints()
        {
            siJoints.RemoveAll(IsJointRemoved);
        }
        protected internal override void RemoveExpiredBodies()
        {
            tags.RemoveAll(IsTagRemoved);
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