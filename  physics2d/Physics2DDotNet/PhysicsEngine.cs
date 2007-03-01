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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

using AdvanceMath;
using Physics2DDotNet.Math2D;
using Physics2DDotNet.Solvers;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.Collections;

namespace Physics2DDotNet
{

    /// <summary>
    /// The Engine that will Apply Physics to object added to it.
    /// </summary>
    [Serializable]
    public sealed class PhysicsEngine
    {
        #region static methods
        private static void CheckChild(IPhysicsEntity child)
        {
            if (child.Engine != null) { throw new InvalidOperationException("The IPhysicsEntity cannot be added to more then one engine or added twice."); }
        }
        #endregion
        #region events
        /// <summary>
        /// Generated when Bodies are truly added to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<Body>> BodiesAdded;
        /// <summary>
        /// Generated when Joints are truly added to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<Joint>> JointsAdded;
        /// <summary>
        /// Generated when PhysicsLogics are truly added to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<PhysicsLogic>> LogicsAdded;

        /// <summary>
        /// Generated when a Bodies are removed to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<Body>> BodiesRemoved;
        /// <summary>
        /// Generated when a Joints are removed to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<Joint>> JointsRemoved;
        /// <summary>
        /// Generated when a PhysicsLogics are removed to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<PhysicsLogic>> LogicsRemoved; 
        #endregion
        #region fields
        private int nextBodyID;

        [NonSerialized]
        AdvReaderWriterLock rwLock;
        [NonSerialized]
        internal bool inUpdate;

        private List<PhysicsLogic> logics;
        internal List<Body> bodies;
        internal List<Joint> joints;

        private List<PhysicsLogic> pendingLogics;
        private List<Joint> pendingJoints;
        private List<Body> pendingBodies;

        private List<PhysicsLogic> removedLogics;
        private List<Joint> removedJoints;
        private List<Body> removedBodies;

        private CollisionSolver solver;
        private BroadPhaseCollisionDetector broadPhase;

        #endregion
        #region constructors
        public PhysicsEngine()
        {
            this.rwLock = new AdvReaderWriterLock();

            this.joints = new List<Joint>();
            this.bodies = new List<Body>();
            this.logics = new List<PhysicsLogic>();

            this.pendingBodies = new List<Body>();
            this.pendingJoints = new List<Joint>();
            this.pendingLogics = new List<PhysicsLogic>();

            this.removedBodies = new List<Body>();
            this.removedJoints = new List<Joint>();
            this.removedLogics = new List<PhysicsLogic>();
        }
        #endregion
        #region properties
        /// <summary>
        /// Gets A threadSafe List of Joints (You wont get the "The collection has changed" Exception with this)
        /// </summary>
        public ThreadSafeList<Joint> Joints
        {
            get
            {
                ThreadSafeList<Joint> result =
                    new ThreadSafeList<Joint>(
                    joints,
                    rwLock);
                result.MakeReadOnly();
                return result;
            }
        }
        /// <summary>
        /// Gets A threadSafe List of Bodies (You wont get the "The collection has changed" Exception with this)
        /// </summary>
        public ThreadSafeList<Body> Bodies
        {
            get
            {
                ThreadSafeList<Body> result =
                    new ThreadSafeList<Body>(
                    bodies,
                    rwLock);
                result.MakeReadOnly();
                return result;
            }
        }
        /// <summary>
        /// Gets A threadSafe List of PhysicsLogics (You wont get the "The collection has changed" Exception with this)
        /// </summary>
        public ThreadSafeList<PhysicsLogic> Logics
        {
            get
            {
                ThreadSafeList<PhysicsLogic> result =
                    new ThreadSafeList<PhysicsLogic>(
                    logics,
                    rwLock);
                result.MakeReadOnly();
                return result;
            }
        }
        /// <summary>
        /// Gets and Sets The BroadPhase collision Detector. (This must be Set to a non-Null value before any calls to Update)
        /// </summary>
        public BroadPhaseCollisionDetector BroadPhase
        {
            get { return broadPhase; }
            set
            {
                using (rwLock.Write)
                {
                    if (broadPhase != value)
                    {
                        if (broadPhase != null) { broadPhase.OnRemovedInternal(); }
                        if (value != null) { value.OnAddedInternal(this); }
                        broadPhase = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets and Sets the Collision Solver (This must be Set to a non-Null value before any calls to Update)
        /// </summary>
        public CollisionSolver Solver
        {
            get
            {
                return solver;
            }
            set
            {
                using (rwLock.Write)
                {
                    if (solver != value)
                    {
                        if (solver != null) { solver.OnRemovedInternal(); }
                        if (value != null) { value.OnAddedInternal(this); }
                        solver = value;
                    }
                }
            }
        }
        #endregion
        #region methods
        /// <summary>
        /// Adds a Body to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="item">The Body to be added.</param>
        public void AddBody(Body item)
        {
            CheckChild(item);
            lock (pendingBodies)
            {
                pendingBodies.Add(item);
            }
        }
        /// <summary>
        /// Adds a collection of Bodies to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="collection">The collection to be Added</param>
        public void AddBodyRange(ICollection<Body> collection)
        {
            foreach (Body item in collection)
            {
                CheckChild(item);
            }
            lock (pendingBodies)
            {
                pendingBodies.AddRange(collection);
            }
        }

        /// <summary>
        /// Adds a Joint to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="item">The Joint to be added.</param>
        public void AddJoint(Joint item)
        {
            CheckChild(item);
            solver.CheckJoint(item);
            lock (pendingJoints)
            {
                pendingJoints.Add(item);
            }
        }
        /// <summary>
        /// Adds a collection of Joints to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="collection">The collection to be Added</param>
        public void AddJointRange(ICollection<Joint> collection)
        {
            CheckState();
            foreach (Joint item in collection)
            {
                CheckChild(item);
                solver.CheckJoint(item);
            }
            lock (pendingJoints)
            {
                pendingJoints.AddRange(collection);
            }
        }

        /// <summary>
        /// Adds a PhysicsLogic to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="item">The PhysicsLogic to be added.</param>
        public void AddLogic(PhysicsLogic item)
        {
            CheckChild(item);
            lock (pendingLogics)
            {
                pendingLogics.Add(item);
            }
        }
        /// <summary>
        /// Adds a collection of PhysicsLogics to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="collection">The collection to be Added</param>
        public void AddLogicRange(ICollection<PhysicsLogic> collection)
        {
            foreach (PhysicsLogic item in collection)
            {
                CheckChild(item);
            }
            lock (pendingLogics)
            {
                pendingLogics.AddRange(collection);
            }
        }

        /// <summary>
        /// Updates the Engine with a change in time. This call wil block all access to the engine while it is running.
        /// </summary>
        /// <param name="dt">the change in time</param>
        public void Update(Scalar dt)
        {
            if (dt < 0) { throw new ArgumentOutOfRangeException("dt"); }
            CheckState();
            WriterLock wLock = rwLock.Write;
            inUpdate = true;
            try
            {
                AddPending();

                UpdateTime(dt);
                solver.Solve(dt);
                OnStateChanged();

                RemoveExpired();
            }
            finally
            {
                inUpdate = false;
                wLock.Release();
            }
        }

        /// <summary>
        /// Clears the Engine of all objects. Also clears the Detector and Solver.
        /// </summary>
        public void Clear()
        {
            using (rwLock.Write)
            {
                nextBodyID = 0;
                solver.Clear();
                broadPhase.Clear();
                foreach (Body body in bodies)
                {
                    body.OnRemoved();
                }
                foreach (Joint joint in joints)
                {
                    joint.OnRemovedInternal();
                }
                foreach (PhysicsLogic logic in logics)
                {
                    logic.OnRemovedInternal();
                }
                if (BodiesRemoved != null && bodies.Count>0)
                {
                    BodiesRemoved(this, new CollectionEventArgs<Body>(bodies.AsReadOnly()));
                }
                if (JointsRemoved != null && joints.Count > 0)
                {
                    JointsRemoved(this, new CollectionEventArgs<Joint>(joints.AsReadOnly()));
                }
                if (LogicsRemoved != null && logics.Count > 0)
                {
                    LogicsRemoved(this, new CollectionEventArgs<PhysicsLogic>(logics.AsReadOnly()));
                }
                bodies.Clear();
                joints.Clear();
                logics.Clear();
                lock (pendingBodies)
                {
                    pendingBodies.Clear();
                }
                lock (pendingJoints)
                {
                    pendingJoints.Clear();
                }
                lock (pendingLogics)
                {
                    pendingLogics.Clear();
                }
            }
        }

        private void UpdateTime(Scalar dt)
        {
            int count = bodies.Count;
            for (int index = 0; index < count; ++index)
            {
                bodies[index].UpdateTime(dt);
            }
            count = joints.Count;
            for (int index = 0; index < count; ++index)
            {
                joints[index].UpdateTime(dt);
            }
            count = logics.Count;
            for (int index = 0; index < count; ++index)
            {
                logics[index].UpdateTime(dt);
            }
        }
        private void OnStateChanged()
        {
            int count = bodies.Count;
            for (int index = 0; index < count; ++index)
            {
                bodies[index].OnStateChanged();
            }
        }

        private void RemoveExpired()
        {
            RemoveExpiredBodies();
            RemoveExpiredJoints();
            RemoveExpiredLogics();
        }
        private void RemoveExpiredBodies()
        {
            if (bodies.RemoveAll(IsBodyExpired) == 0) { return; }
            solver.RemoveExpiredBodies();
            broadPhase.RemoveExpiredBodies();
            if (BodiesRemoved != null)
            {
                BodiesRemoved(this, new CollectionEventArgs<Body>(removedBodies.AsReadOnly()));
                removedBodies.Clear();
            }
        }
        private void RemoveExpiredJoints()
        {
            if (joints.RemoveAll(IsJointExpired) == 0) { return; }
            solver.RemoveExpiredJoints();
            if (JointsRemoved != null)
            {
                JointsRemoved(this, new CollectionEventArgs<Joint>(removedJoints.AsReadOnly()));
                removedJoints.Clear();
            }
        }
        private void RemoveExpiredLogics()
        {
            if (logics.RemoveAll(IsLogicExpired) == 0) { return; }
            if (LogicsRemoved != null)
            {
                LogicsRemoved(this, new CollectionEventArgs<PhysicsLogic>(removedLogics.AsReadOnly()));
                removedLogics.Clear();
            }
        }

        private bool IsBodyExpired(Body body)
        {
            if (!body.Lifetime.IsExpired) { return false; }
            if (BodiesRemoved != null) { removedBodies.Add(body); }
            body.OnRemoved();
            return true;
        }
        private bool IsJointExpired(Joint joint)
        {
            if (!joint.Lifetime.IsExpired) { return false; }
            if (JointsRemoved != null) { removedJoints.Add(joint); }
            foreach (Body body in joint.Bodies)
            {
                body.jointCount--;
            }
            joint.OnRemovedInternal();
            return true;
        }
        private bool IsLogicExpired(PhysicsLogic logic)
        {
            if (!logic.Lifetime.IsExpired) { return false; }
            if (LogicsRemoved != null) { removedLogics.Add(logic); }
            logic.OnRemovedInternal();
            return true;
        }

        private void AddPending()
        {
            AddPendingBodies();
            AddPendingJoints();
            AddPendingLogics();
        }
        private void AddPendingBodies()
        {
            lock (this.pendingBodies)
            {
                if (pendingBodies.Count == 0) { return; }
                int count = pendingBodies.Count;
                for (int index = 0; index < count; ++index)
                {
                    Body item = pendingBodies[index];
                    item.ID = nextBodyID++;
                    item.ApplyMatrix();
                    item.OnAdded(this);
                }
                bodies.AddRange(pendingBodies);
                solver.AddBodyRange(pendingBodies);
                broadPhase.AddBodyRange(pendingBodies);
                if (BodiesAdded != null) { BodiesAdded(this, new CollectionEventArgs<Body>(pendingBodies.AsReadOnly())); }
                pendingBodies.Clear();
            }
        }
        private void AddPendingJoints()
        {
            lock (this.pendingJoints)
            {
                if (pendingJoints.Count == 0) { return; }
                int count = pendingJoints.Count;
                for (int index = 0; index < count; ++index)
                {
                    Joint item = pendingJoints[index];
                    item.OnAddedInternal(this);
                    foreach (Body body in item.Bodies)
                    {
                        body.jointCount++;
                    }
                }
                joints.AddRange(pendingJoints);
                solver.AddJointRange(pendingJoints);
                if (JointsAdded != null) { JointsAdded(this, new CollectionEventArgs<Joint>(pendingJoints.AsReadOnly())); }
                pendingJoints.Clear();
            }
        }
        private void AddPendingLogics()
        {
            lock (this.pendingLogics)
            {
                if (pendingLogics.Count == 0) { return; }
                int count = pendingLogics.Count;
                for (int index = 0; index < count; ++index)
                {
                    pendingLogics[index].OnAddedInternal(this);
                }
                logics.AddRange(pendingLogics);
                if (LogicsAdded != null) { LogicsAdded(this, new CollectionEventArgs<PhysicsLogic>(pendingLogics.AsReadOnly())); }
                pendingLogics.Clear();
            }
        }

        private void CheckState()
        {
            if (this.broadPhase == null) { throw new InvalidOperationException("The BroadPhase property must be set."); }
            if (this.solver == null) { throw new InvalidOperationException("The Solver property must be set."); }
        }

        internal void RunLogic(Scalar dt)
        {
            int count = logics.Count;
            for (int index = 0; index < count; ++index)
            {
                logics[index].RunLogic(dt);
            }
        }
        internal void HandleCollision(Scalar dt, Body first, Body second)
        {
            if (first.Mass.MassInv == 0 && second.Mass.MassInv == 0) { return; }

            if (first.BroadPhaseDetectionOnly || second.BroadPhaseDetectionOnly)
            {
                if (first.BroadPhaseDetectionOnly)
                {
                    first.OnCollision(second, null);
                }
                if (second.BroadPhaseDetectionOnly)
                {
                    second.OnCollision(first, null);
                }
            }
            else
            {
                ICollisionInfo info = solver.HandleCollision(dt, first, second);
                if (info.Collided)
                {
                    first.OnCollision(second, info);
                    second.OnCollision(first, info);
                }
            }

        }
        #endregion
    }
}