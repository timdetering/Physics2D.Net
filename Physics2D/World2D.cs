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
using System.Runtime.Serialization;
using System.Collections.Generic;
using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath;
using Physics2D.Collections;

namespace Physics2D
{

    [Serializable]
    public class World2D : World2D<ICollidableBody, ICollidableArea, IJoint>
    {
        public World2D() : base() { }
        public World2D(GravitySourceList gravitySourceList) : base(gravitySourceList) { }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="CO">A type that Impliments <see cref="ICollidableBody"/></typeparam>
    /// <typeparam name="CA">A type that Impliments <see cref="ICollidableArea"/></typeparam>
    /// <typeparam name="JO">A type that Impliments <see cref="IJoint"/></typeparam>
    [Serializable]
    public class World2D<CB, CA, JO> : IUpdatable, IDeserializationCallback
        where CB : class, ICollidableBody
        where CA : class, ICollidableArea
        where JO : class, IJoint
    {
        #region events
        [field: NonSerialized]
        public event CollisionEventDelegate Collision;
        #endregion
        #region fields
        protected bool enabled = false;
        protected bool doShock = true;
        protected bool calcGravity = false;
        protected bool contactBroadPhaseOnlyOnce = false;
        protected bool doSeperateIntersecting = true;
        protected float seperatingFactor = (float)1;
        /// <summary>
        /// The number of times the contact detection routine will be ran each timestep.
        /// </summary>
        protected int contactStepsCount = 10;
        /// <summary>
        /// The number of times the collision detection routine will be ran each timestep.
        /// </summary>
        protected int collisionStepsCount = 5;
        /// <summary>
        /// The number of times the seperate routine will be ran each timestep.
        /// </summary>
        protected int seperateStepsCount = 5;
        /// <summary>
        /// The collision detector used to detect collisionFilter
        /// </summary>
        [NonSerialized]
        protected CollisionDetector<CB, CA> detector;
        /// <summary>
        /// manages all the gravity
        /// </summary>
        protected GravitySourceList gravitySourceList;
        /// <summary>
        /// grouppairs that were added while the world is running. they are added after collissoin detection is ran.
        /// </summary>
        protected List<CB> pendingCollidableBodies = new List<CB>();
        /// <summary>
        /// grouppairs that are currently in the world.
        /// </summary>
        protected CollidableBodyList<CB> collidableBodies = new CollidableBodyList<CB>();
        /// <summary>
        /// the joints in the world.
        /// </summary>
        protected JointList<JO> joints = new JointList<JO>();
        /// <summary>
        /// joints that were added while the world is running. they are added after collissoin detection is ran.
        /// </summary>
        protected List<JO> pendingJoints = new List<JO>();
        protected BaseCollidableList<CA> collidableAreas = new BaseCollidableList<CA>();
        protected List<CA> pendingCollidableAreas = new List<CA>();
        protected float rayTraceVelocityTolerance = 400;
        #endregion
        #region constructors

        public World2D()
        {
            this.gravitySourceList = new GravitySourceList();
            this.detector = new CollisionDetector<CB, CA>();
        }
        public World2D(GravitySourceList gravitySourceCollection)
        {
            this.gravitySourceList = gravitySourceCollection;
            this.calcGravity = gravitySourceCollection.Count > 0;
            this.detector = new CollisionDetector<CB, CA>();
        }
        public World2D(CollisionDetector<CB, CA> detector)
        {
            this.gravitySourceList = new GravitySourceList();
            this.detector = detector;
        }
        #endregion
        #region properties
        public float RayTraceVelocityTolerance
        {
            get { return rayTraceVelocityTolerance; }
            set { rayTraceVelocityTolerance = value; }
        }
        public float SeperatingFactor
        {
            get { return seperatingFactor; }
            set { seperatingFactor = value; }
        }
        public bool DoSeperateIntersecting
        {
            get { return doSeperateIntersecting; }
            set { doSeperateIntersecting = value; }
        }
        public bool DoShock
        {
            get { return doShock; }
            set { doShock = value; }
        }
        public int CollisionStepsCount
        {
            get { return collisionStepsCount; }
            set { collisionStepsCount = value; }
        }
        public int ContactStepsCount
        {
            get { return contactStepsCount; }
            set { contactStepsCount = value; }
        }
        public List<CB> Collidables
        {
            get
            {
                return new List<CB>(collidableBodies);
            }
        }
        public List<JO> Joints
        {
            get
            {
                return new List<JO>(joints);
            }
        }
        public List<CA> Ray2DEffectGroups
        {
            get
            {
                return new List<CA>(collidableAreas);
            }
        }
        public List<IRay2DEffect> Ray2DEffects
        {
            get
            {
                List<IRay2DEffect> returnvalue = new List<IRay2DEffect>();
                foreach (CA area in collidableAreas)
                {
                    IRay2DEffectGroup group = area as IRay2DEffectGroup;
                    if (group != null)
                    {
                        List<IRay2DEffect> tmp = group.Ray2DEffects;
                        if (tmp != null)
                        {
                            returnvalue.AddRange(tmp);
                        }
                    }
                }
                return returnvalue;
            }
        }
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                lock (this)
                {
                    enabled = value;
                }
            }
        }
        public bool CalcGravity
        {
            get
            {
                return calcGravity;
            }
            set
            {
                calcGravity = value;
            }
        }
        public GravitySourceList Gravity
        {
            get
            {
                return gravitySourceList;
            }
        }
        /// <summary>
        /// Increases speed but Decreases Accuracy.
        /// </summary>
        public bool ContactBroadPhaseOnlyOnce
        {
            get { return contactBroadPhaseOnlyOnce; }
            set { contactBroadPhaseOnlyOnce = value; }
        }
        #endregion
        #region methods
        #region public
        /// <summary>
        /// Adds a <typeparamref name="CO"/> to the World2D and its <see cref="detector"/>.
        /// </summary>
        /// <param name="collidable">A <typeparamref name="CO"/> that needs to be added.</param>
        /// <remarks>
        /// This does not add the object to the <see cref="collidableBodies"/> list instantly unless the world is not enabled.  
        /// The object is added to the <see cref="pendingcollidables"/> list and added to the World2D when <see cref="AddPending()"/> is called. 
        /// </remarks>
        public virtual void AddICollidableBody(CB collidableBody)
        {
            if (collidableBody != null)
            {
                if (this.enabled)
                {
                    lock (pendingCollidableBodies)
                    {
                        pendingCollidableBodies.Add(collidableBody);
                    }
                }
                else
                {
                    collidableBodies.Add(collidableBody);
                    detector.AddICollidableBody(collidableBody);
                }
            }
        }
        /// <summary>
        /// Adds a <typeparamref name="CA"/> to the World2D and its <see cref="detector"/>.
        /// </summary>
        /// <param name="area">A <typeparamref name="CA"/> that needs to be added.</param>
        /// <remarks>
        /// This does not add the object to the <see cref="collidableAreas"/> list instantly unless the world is not enabled.  
        /// The object is added to the <see cref="pendingCollidableAreas"/> list and added to the World2D when <see cref="AddPending()"/>is called. 
        /// </remarks>
        public virtual void AddICollidableArea(CA area)
        {
            if (area != null)
            {
                if (this.enabled)
                {
                    lock (pendingCollidableAreas)
                    {
                        pendingCollidableAreas.Add(area);
                    }
                }
                else
                {
                    collidableAreas.Add(area);
                    detector.AddICollidableArea(area);
                }
            }
        }
        /// <summary>
        /// Adds a <typeparamref name="JO"/> to the World2D.
        /// </summary>
        /// <param name="joint">A <typeparamref name="JO"/> that needs to be added.</param>
        /// <remarks>
        /// This does not add the object to the <see cref="joints"/> list  instantly unless the world is not enabled.  
        /// The object is added to the <see cref="pendingjoints"/> list and added to the World2D when <see cref="AddPending()"/> is called. 
        /// </remarks>
        public virtual void AddIJoint(JO joint)
        {
            if (joint != null)
            {
                if (this.enabled)
                {
                    lock (pendingJoints)
                    {
                        pendingJoints.Add(joint);
                    }
                }
                else
                {
                    joints.Add(joint);
                }
            }
        }
        /// <summary>
        /// Adds a gravity source to the World2D.
        /// </summary>
        /// <param name="source"></param>
        /// <remarks>
        /// this adds the object instantly, but it wont effects things untill the next call to <see cref="CalcAcceleration()"/>
        /// </remarks>
        public void AddIGravitySource(IGravitySource source)
        {
            if (source != null)
            {
                gravitySourceList.Add(source);
                CB body = source as CB;
                if (body != null)
                {
                    body.Flags = body.Flags | BodyFlags.GravityWell;
                }
            }
        }
        /// <summary>
        /// Updates the world by a amount of time.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        public void Update(float dt)
        {
            lock (this)
            {
                if (enabled)
                {
                    RemoveExpiredAndAddPending();
                    if (this.calcGravity)
                    {
                        collidableBodies.CalcAcceleration(this.gravitySourceList);
                    }
                    else
                    {
                        collidableBodies.CalcAcceleration(null);
                    }

                    collidableBodies.ResetEventInfo();
                    bool doContacts = ProcessAllCollisions(dt);
                    collidableBodies.UpdateVelocity(dt);
                    collidableBodies.ClearForces();
                    if (doContacts)
                    {
                        ProcessAllContacts(dt);
                    }
                    joints.PreCalc(dt);
                    joints.CalcAndApply(dt);
                    collidableBodies.UpdatePosition(dt);
                    ProcessAllCollidableAreas(dt);

                    this.detector.ClearTemporaryObjects();
                    collidableBodies.ResetCollisionStates();



                    UpdateIUpdatables(dt);
                    GenerateCollisionEvents();
                }
            }
        }
        /// <summary>
        /// Reconstructs the world after it has been serialized.
        /// </summary>
        /// <param name="sender"></param>
        public virtual void OnDeserialization(object sender)
        {
            detector = new CollisionDetector<CB, CA>();
            detector.AddICollidableBodyRange(collidableBodies);
            detector.AddICollidableAreaRange(collidableAreas);
        }
        #endregion
        #region protected
        /// <summary>
        /// Removes objects that have <see cref="ITimed.IsExpired"/> set to true and adds pending objects.
        /// </summary>
        protected void RemoveExpiredAndAddPending()
        {
            RemoveExpired();
            AddPending();
        }
        /// <summary>
        /// Removes objects that have <see cref="ITimed.IsExpired"/> set to true.
        /// </summary>
        protected virtual void RemoveExpired()
        {
            collidableBodies.RemoveExpired();
            collidableAreas.RemoveExpired();
            joints.RemoveExpired();
            gravitySourceList.RemoveExpired();
            detector.RemoveExpired();
        }
        /// <summary>
        /// Moves objects form the pending list the the actual lists.
        /// </summary>
        protected virtual void AddPending()
        {
            lock (pendingCollidableBodies)
            {
                collidableBodies.AddRange(pendingCollidableBodies);
                detector.AddICollidableBodyRange(pendingCollidableBodies);
                pendingCollidableBodies.Clear();
            }
            lock (pendingCollidableAreas)
            {
                collidableAreas.AddRange(pendingCollidableAreas);
                detector.AddICollidableAreaRange(pendingCollidableAreas);
                pendingCollidableAreas.Clear();
            }
            lock (pendingJoints)
            {
                joints.AddRange(pendingJoints);
                pendingJoints.Clear();
            }
        }
        /// <summary>
        /// Calls <see cref="IUpdatable.Update"/> for all of the objects in the World2D.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        protected virtual void UpdateIUpdatables(float dt)
        {
            collidableBodies.Update(dt);
            joints.Update(dt);
            collidableAreas.Update(dt);
        }
        /// <summary>
        /// Generates events for the <see cref="Collision"/> event.
        /// </summary>
        protected void GenerateCollisionEvents()
        {
            if (Collision != null)
            {
                int length = collidableBodies.Count;
                for (int pos = 0; pos < length; ++pos)
                {
                    if (collidableBodies[pos].CollisionState.GenerateContactEvents || collidableBodies[pos].CollisionState.GenerateCollisionEvents || collidableBodies[pos].CollisionState.GenerateRayEvents)
                    {
                        if (collidableBodies[pos].CollisionState.InterferenceInfos.Count != 0)
                        {
                            Collision(this, new CollisionEventArgs(collidableBodies[pos].CollisionState.InterferenceInfos, collidableBodies[pos].CollisionState.CollisionImpulseApplied, collidableBodies[pos].CollisionState.ContactImpulseApplied));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Runs collision detection for all of the <see cref="ICollidableArea"/> in the <see cref="CollisionDetector.collidableAreas"/> of <see cref="detector"/>.
        /// </summary>
        /// <param name="dt">The change in time in seconds, from the last time this method was called. </param>
        protected void ProcessAllCollidableAreas(float dt)
        {
            if (detector.CollidableAreasNeedProcessing)
            {
                detector.CollisionPairParameters.TestEdgeonEdge = false;
                detector.ApplyFilter(BodyFlags.None, true);
                detector.ProcessCollidableAreas(dt);
            }
        }
        protected bool ProcessAllCollisions(float dt)
        {
            bool returnvalue = false;
            //	PreCalcJoints(dt);

            collidableBodies.ResetCollisionStates();// ResetCollisionStates();
            collidableBodies.ResetImpulseApplied();// ResetImulseAppliedForAll();

            collidableBodies.SaveState();// SaveStates();

            collidableBodies.UpdateVelocity(dt);// UpdateVelocities(dt);
            collidableBodies.UpdatePosition(dt);// UpdatePositions(dt);
            detector.CollisionPairParameters.DoRayTrace = true;
            detector.CollisionPairParameters.RayTraceVelocityTolerance = rayTraceVelocityTolerance;
            detector.ApplyFilter(BodyFlags.NoCollision, true);
            detector.CalcBroadPhasedCollisions();
            detector.CalcNarrowPhasedCollisions(dt);
            detector.CollisionPairParameters.DoRayTrace = false;

            collidableBodies.LoadVelocityState();// LoadVelocityStates();
            joints.PreCalc(dt);//PreCalcJoints(dt);
            List<CollisionPair> pairs = detector.CollisionPairs;
            int pairCount = pairs.Count;
            if (pairCount == 0)
            {
                return returnvalue;
            }
            int direction = -1;
            int endpos;
            int pos;
            for (int step = 0; step < collisionStepsCount; ++step)
            {
                if (direction == -1)
                {
                    direction = 1;
                    endpos = pairCount;
                    pos = 0;
                }
                else
                {
                    direction = -1;
                    endpos = -1;
                    pos = pairCount - 1;
                }
                bool CollisionInThisStep = false;
                for (; pos != endpos; pos += direction)
                {
                    float Restitution = pairs[pos].Coefficients.Restitution;
                    CollisionInThisStep = this.HandleCollision(pairs[pos], dt, Restitution, step, false) || CollisionInThisStep;
                    returnvalue = CollisionInThisStep || returnvalue;
                }
                joints.CalcAndApply(dt); //CalcAndApplyJoints(dt);
                if (CollisionInThisStep && ((step + 1) < collisionStepsCount))
                {
                    detector.CalcNarrowPhasedCollisions(dt);
                    pairs = detector.CollisionPairs;
                    pairCount = pairs.Count;
                }
                if (!CollisionInThisStep)
                {
                    break;
                }
            }
            collidableBodies.LoadPositionState();// LoadPositionStates();


            return returnvalue;
        }
        protected void ProcessAllContacts(float dt)
        {

            if (contactStepsCount > 0)
            {
                float CoefficientofRestitution = -1;
                float mcr = -1 / (float)contactStepsCount;
                joints.PreCalc(dt); //PreCalcJoints(dt);
                for (int step = 0; step < contactStepsCount; ++step)
                {
                    CoefficientofRestitution = (contactStepsCount - step - 1) * mcr;

                    collidableBodies.SaveState();// SaveStates();
                    collidableBodies.UpdatePosition(dt);// UpdatePositions(dt);

                    collidableBodies.ResetCollisionStates();// ResetCollisionStates();
                    if ((!contactBroadPhaseOnlyOnce) || step == 0)
                    {
                        detector.CollisionPairParameters.TestEdgeonEdge = false;
                        detector.ApplyFilter(BodyFlags.NoContact | BodyFlags.NoCollision, false);
                        detector.CalcBroadPhasedCollisions();
                    }
                    detector.CalcNarrowPhasedCollisions(dt);

                    collidableBodies.LoadState();

                    if (detector.CollisionPairs.Count == 0)
                    {
                        joints.CalcAndApply(dt); 
                        return;
                    }
                    detector.CalcCollisionLevels();
                    bool ContactInThisStep = false;
                    for (int level = 0; level < detector.LevelCount; ++level)
                    {
                        List<CollisionPair> currentLevel = detector.GetCollisionPairsByLevel(level);
                        int pairCount = currentLevel.Count;
                        if (doShock)
                        {
                            for (int pos = 0; pos != pairCount; ++pos)
                            {
                                currentLevel[pos].FreezeLowerLevel();
                            }
                        }
                        for (int pos = 0; pos != pairCount; ++pos)
                        {
                            ContactInThisStep = HandleCollision(currentLevel[pos], dt, CoefficientofRestitution, step, true) || ContactInThisStep;
                        }
                        if (doShock)
                        {
                            for (int pos = 0; pos != pairCount; ++pos)
                            {
                                currentLevel[pos].Collidable1.CollisionState.Frozen = false;
                                currentLevel[pos].Collidable2.CollisionState.Frozen = false;
                            }
                        }
                    }
                    joints.CalcAndApply(dt); //CalcAndApplyJoints(dt);
                    if (!ContactInThisStep)
                    {
                        break;
                    }
                }
                if (doSeperateIntersecting)
                {
                    detector.CalcNarrowPhasedCollisions(dt);
                    detector.ResetCollisionPairs();
                    bool DidSeperation = true;
                    for (int pos = 0; pos < this.seperateStepsCount && DidSeperation; ++pos)
                    {
                        DidSeperation = false;
                        foreach (CollisionPair pair in detector.CollisionPairs)
                        {
                            if (pair.BestCollisionInfo.Distance < -Physics.Tolerance)
                            {
                                SeperateIntersecting(dt, pair, seperatingFactor);
                                DidSeperation = true;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// This does not work yet.
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="factor"></param>
        protected void SeperateIntersecting(float dt, CollisionPair pair, float factor)
        {
            if (pair.BestCollisionInfo == null)
            {
                return;
            }
            CollisionPairParameters parameters = new CollisionPairParameters();
            parameters.DoInnerRadiusTests = true;
            CollisionPair pair2 = new CollisionPair(pair.Collidable1, pair.Collidable2, parameters);
            if (!pair2.TestBoundingBox2Ds())
            {
                pair.BestCollisionInfo.Distance = 1;
                return;
            }
            pair2.CalcDistance();
            if (!pair2.TestBoundingRadius())
            {
                pair.BestCollisionInfo.Distance = 1;
                return;
            }
            pair2.TestCollisions(dt);
            if (!pair2.IsValid)
            {
                pair.BestCollisionInfo.Distance = 1;
                return;
            }
            CollisionInfo info = pair2.BestCollisionInfo;
            Vector2D Impulse;

            if (!ApplyImpulse(pair2, factor, dt, true, out Impulse))
            {
                pair.BestCollisionInfo.Distance = 1;
            }
        }
        protected void SeperateIntersectingOLD(CollisionPair pair, float factor)
        {
            CollisionInfo info = pair.BestCollisionInfo;
            if (info == null)
            {
                return;
            }
            ICollidableBody collidable1 = pair.Collidable1;
            ICollidableBody collidable2 = pair.Collidable2;


            Vector2D delta = (factor * info.Distance) * info.CollisionNormal;
            Vector2D dist = (collidable1.Current.Position.Linear - collidable2.Current.Position.Linear);
            if (dist * delta < 0)
            {
                delta = -delta;
            }

            if (((collidable1.Flags & (BodyFlags.NoImpulse | BodyFlags.InfiniteMass | BodyFlags.GravityWell)) != 0))
            {
                if (((collidable2.Flags & (BodyFlags.NoImpulse | BodyFlags.InfiniteMass | BodyFlags.GravityWell)) != 0))
                {
                    return;
                }
                collidable2.Current.Position.Linear += delta;
                foreach (CollisionPair pair2 in collidable2.CollisionState.CollisionPairs)
                {
                    if (pair2.Collidable1 == collidable2)
                    {
                        pair2.BestCollisionInfo.Distance -= delta * pair2.BestCollisionInfo.CollisionNormal;
                    }
                    else
                    {
                        pair2.BestCollisionInfo.Distance += delta * pair2.BestCollisionInfo.CollisionNormal;
                    }
                }
            }
            else if (((collidable2.Flags & (BodyFlags.NoImpulse | BodyFlags.InfiniteMass | BodyFlags.GravityWell)) != 0))
            {
                collidable1.Current.Position.Linear -= delta;
                foreach (CollisionPair pair2 in collidable1.CollisionState.CollisionPairs)
                {
                    if (pair2.Collidable1 == collidable1)
                    {
                        pair2.BestCollisionInfo.Distance += delta * pair2.BestCollisionInfo.CollisionNormal;
                    }
                    else
                    {
                        pair2.BestCollisionInfo.Distance -= delta * pair2.BestCollisionInfo.CollisionNormal;
                    }
                }
            }
            else
            {
                delta *= .5f;
                collidable1.Current.Position.Linear -= delta;
                collidable2.Current.Position.Linear += delta;
                foreach (CollisionPair pair2 in collidable1.CollisionState.CollisionPairs)
                {
                    if (pair2.Collidable1 == collidable1)
                    {
                        pair2.BestCollisionInfo.Distance -= delta * pair2.BestCollisionInfo.CollisionNormal;
                    }
                    else
                    {
                        pair2.BestCollisionInfo.Distance += delta * pair2.BestCollisionInfo.CollisionNormal;
                    }
                }
                foreach (CollisionPair pair2 in collidable2.CollisionState.CollisionPairs)
                {
                    if (pair2.Collidable1 == collidable2)
                    {
                        pair2.BestCollisionInfo.Distance += delta * pair2.BestCollisionInfo.CollisionNormal;
                    }
                    else
                    {
                        pair2.BestCollisionInfo.Distance -= delta * pair2.BestCollisionInfo.CollisionNormal;
                    }
                }
            }
        }

        protected virtual bool HandleCollision(CollisionPair pair, float dt, float CoefficientofRestitution, int step, bool InContact)
        {
            PhysicsState Body1Before = null;
            PhysicsState Body2Before = null;
            bool GenerateEventFor1;
            bool GenerateEventFor2;
            bool GenerateEvent;
            Vector2D Impulse;
            if (!pair.IsValid)
            {
                return false;
            }
            if (InContact)
            {
                GenerateEventFor1 = pair.Collidable1.CollisionState.GenerateContactEvents;
                GenerateEventFor2 = pair.Collidable2.CollisionState.GenerateContactEvents;
            }
            else
            {
                GenerateEventFor1 = pair.Collidable1.CollisionState.GenerateCollisionEvents;
                GenerateEventFor2 = pair.Collidable2.CollisionState.GenerateCollisionEvents;
            }
            GenerateEvent = GenerateEventFor1 || GenerateEventFor2;
            if (GenerateEvent)
            {
                Body1Before = new PhysicsState(pair.Collidable1.Current);
                Body2Before = new PhysicsState(pair.Collidable2.Current);
            }
            if (ApplyImpulse(pair, CoefficientofRestitution, dt, InContact, out Impulse))
            {
                if (InContact)
                {
                    pair.Collidable1.CollisionState.ContactImpulseApplied = true;
                    pair.Collidable2.CollisionState.ContactImpulseApplied = true;
                }
                else
                {
                    pair.Collidable1.CollisionState.CollisionImpulseApplied = true;
                    pair.Collidable2.CollisionState.CollisionImpulseApplied = true;
                }
                if (GenerateEvent)
                {
                    if (GenerateEventFor1)
                    {
                        InterferenceInfo info = new InterferenceInfo(new CollidablePairInterferenceInfo(
                        InContact,
                        step,
                        dt,
                        Impulse,
                        pair.BestCollisionInfo,
                        Body1Before,
                        Body2Before,
                        new PhysicsState(pair.Collidable1.Current),
                        new PhysicsState(pair.Collidable2.Current),
                        pair.Collidable1,
                        pair.Collidable2));
                        pair.Collidable1.CollisionState.InterferenceInfos.Add(info);
                    }
                    if (GenerateEventFor2)
                    {
                        InterferenceInfo info = new InterferenceInfo(new CollidablePairInterferenceInfo(
                        InContact,
                        step,
                        dt,
                        Impulse,
                        pair.BestCollisionInfo,
                        Body2Before,
                        Body1Before,
                        new PhysicsState(pair.Collidable2.Current),
                        new PhysicsState(pair.Collidable1.Current),
                        pair.Collidable2,
                        pair.Collidable1
                        ));
                        pair.Collidable2.CollisionState.InterferenceInfos.Add(info);
                    }
                }
                return true;
                /*if (InContact)
                {
                    return true;
                }
                Vector2D acc = Impulse * (pair.Collidable1.MassInertia.MassInv + pair.Collidable2.MassInertia.MassInv);
                Vector2D avgACC = Vector2D.Zero;
                bool test1 = (pair.Collidable1.Flags & BodyFlags.InfiniteMass) != BodyFlags.InfiniteMass;
                if (test1)
                {
                    avgACC = pair.Collidable1.Current.Acceleration.Linear;
                }
                if ((pair.Collidable2.Flags & BodyFlags.InfiniteMass) != BodyFlags.InfiniteMass)
                {
                    avgACC += pair.Collidable2.Current.Acceleration.Linear;
                    if (test1)
                    {
                        avgACC *= .5f;
                    }
                }
                bool rv =  MathAdv.Abs(avgACC.X) < MathAdv.Abs(acc.X) || MathAdv.Abs(avgACC.Y) < MathAdv.Abs(acc.Y);
                return rv;*/
                // return (acc > acccaaalll);
               // return (collisionThreashold > acccaaalll);
                //return true;
            }
            return false;
        }
        
        //protected float collisionThreashold = 90;
        protected bool ApplyImpulse(CollisionPair pair, float CoefficientofRestitution, float dt, bool InContact, out Vector2D Impulse)
        {
            if (pair.Collidable1.CollisionState.Frozen && pair.Collidable2.CollisionState.Frozen)
            {
                Impulse = Vector2D.Zero;
                return false;
            }
            bool ApplyToBody1 = !pair.Collidable1.CollisionState.Frozen && ((pair.Collidable1.Flags & BodyFlags.InfiniteMass) != BodyFlags.InfiniteMass);
            bool ApplyToBody2 = !pair.Collidable2.CollisionState.Frozen && ((pair.Collidable2.Flags & BodyFlags.InfiniteMass) != BodyFlags.InfiniteMass);
            CollisionInfo info = pair.BestCollisionInfo;
            Vector2D CollisionPointReletaveToICollidableBody1 = info.CollisionPoint - pair.Collidable1.Current.Position.Linear;
            Vector2D CollisionPointReletaveToICollidableBody2 = info.CollisionPoint - pair.Collidable2.Current.Position.Linear;


            Vector2D RelativeVelocity = pair.Collidable1.GetVelocityAtRelative(CollisionPointReletaveToICollidableBody1) - pair.Collidable2.GetVelocityAtRelative(CollisionPointReletaveToICollidableBody2);
            float RelativeVelocityAlongNormal = RelativeVelocity * info.CollisionNormal;

            float K = pair.Collidable1.GetK(CollisionPointReletaveToICollidableBody1, info.CollisionNormal) + pair.Collidable2.GetK(CollisionPointReletaveToICollidableBody2, info.CollisionNormal);

            if (K < Physics.Tolerance)
            {
                Impulse = Vector2D.Zero;
                return false;
            }
            float NewRelativeVelocityAlongNormal = -CoefficientofRestitution * RelativeVelocityAlongNormal - RelativeVelocityAlongNormal;
            if (dt != 0 && InContact)
            {
                float extra_vel = -info.Distance / (5 * dt);
                if (Math.Abs(extra_vel) > Physics.MaxDeltaVelocity)
                {
                    extra_vel = Math.Sign(extra_vel) * Physics.MaxDeltaVelocity;
                }
                float extra = extra_vel - NewRelativeVelocityAlongNormal;
                if (extra > 0.0f)
                {
                    NewRelativeVelocityAlongNormal += extra;
                }
            }
            if (Math.Abs(NewRelativeVelocityAlongNormal) <= 0)
            {
                Impulse = Vector2D.Zero;
                return false;
            }
            float ImpulseAlongNormal = (-1 / K) * NewRelativeVelocityAlongNormal;

            RelativeVelocity = pair.Collidable1.Current.GetVelocityAtRelative(CollisionPointReletaveToICollidableBody1) - pair.Collidable2.Current.GetVelocityAtRelative(CollisionPointReletaveToICollidableBody2);
            Vector2D RelativeVelocityAlongTangent = (RelativeVelocity - (RelativeVelocity * info.CollisionNormal) * info.CollisionNormal);
            float RelativeVelocityAlongTangentMag = RelativeVelocityAlongTangent.Magnitude;

            if (RelativeVelocityAlongTangentMag > Physics.Tolerance)
            {
                Vector2D Tangent = RelativeVelocityAlongTangent * (-1 / RelativeVelocityAlongTangentMag);
                K = pair.Collidable1.GetK(CollisionPointReletaveToICollidableBody1, Tangent) + pair.Collidable2.GetK(CollisionPointReletaveToICollidableBody2, Tangent);
                if (K > 0.0f)
                {
                    float ImpulseAlongTangent = RelativeVelocityAlongTangentMag / K;
                    float StaticImpulseAlongNormal = pair.Coefficients.StaticFriction * ImpulseAlongNormal;
                    float FrictionImpulse;
                    bool DoStatic = ImpulseAlongTangent < StaticImpulseAlongNormal;
                    if (DoStatic)
                    {
                        FrictionImpulse = ImpulseAlongTangent;
                    }
                    else
                    {
                        FrictionImpulse = pair.Coefficients.DynamicFriction * ImpulseAlongTangent;//ImpulseAlongNormal;
                    }
                    Impulse = ((ImpulseAlongNormal) * info.CollisionNormal) + ((-FrictionImpulse) * Tangent);
                }
                else
                {
                    Impulse = ((ImpulseAlongNormal) * info.CollisionNormal);
                }
            }
            else
            {
                Impulse = ((ImpulseAlongNormal) * info.CollisionNormal);
            }
            if (ApplyToBody1)
            {
                pair.Collidable1.ApplyImpulse(CollisionPointReletaveToICollidableBody1, -Impulse);
            }
            if (ApplyToBody2)
            {
                pair.Collidable2.ApplyImpulse(CollisionPointReletaveToICollidableBody2, Impulse);
            }
            return true;
        }
        #endregion
        #endregion
    }
}
