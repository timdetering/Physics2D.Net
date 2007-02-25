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
        private static bool IsBodyExpired(Body collider)
        {
            if (collider.Lifetime.IsExpired)
            {
                collider.OnRemoved();
                return true;
            }
            return false;
        }
        private static bool IsJointExpired(Joint joint)
        {
            if (joint.Lifetime.IsExpired)
            {
                joint.OnRemovedInternal();
                return true;
            }
            return false;
        }
        private static bool IsLogicExpired(PhysicsLogic logic)
        {
            if (logic.Lifetime.IsExpired)
            {
                logic.OnRemovedInternal();
                return true;
            }
            return false;
        }
        private static void CheckChild(IPhysicsEntity child)
        {
            if (child.Engine != null) { throw new InvalidOperationException("The IPhysicsEntity cannot be added to more then one engine or added twice."); }
        }
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
            foreach (Body item in bodies)
            {
                item.UpdateTime(dt);
            }
            foreach (Joint item in joints)
            {
                item.UpdateTime(dt);
            }
            foreach (PhysicsLogic item in logics)
            {
                item.UpdateTime(dt);
            }
        }
        private void OnStateChanged()
        {
            foreach (Body item in bodies)
            {
                item.OnStateChanged();
            }
        }

        private void RemoveExpired()
        {
            bodies.RemoveAll(IsBodyExpired);
            solver.RemoveExpiredBodies();
            broadPhase.RemoveExpiredBodies();
            joints.RemoveAll(IsJointExpired);
            solver.RemoveExpiredJoints();
            logics.RemoveAll(IsLogicExpired);
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
                foreach (Body item in pendingBodies)
                {
                    item.ID = nextBodyID++;
                    item.ApplyMatrix();
                    item.OnAdded(this);
                }
                bodies.AddRange(pendingBodies);
                solver.AddBodyRange(pendingBodies);
                broadPhase.AddBodyRange(pendingBodies);
                pendingBodies.Clear();
            }
        }
        private void AddPendingJoints()
        {
            lock (this.pendingJoints)
            {
                foreach (Joint item in pendingJoints)
                {
                    item.OnAddedInternal(this);
                }
                joints.AddRange(pendingJoints);
                solver.AddJointRange(pendingJoints);
                pendingJoints.Clear();
            }
        }
        private void AddPendingLogics()
        {
            lock (this.pendingLogics)
            {
                for (int index = 0; index < pendingLogics.Count; ++index)
                {
                    pendingLogics[index].OnAddedInternal(this);
                }
                logics.AddRange(pendingLogics);
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
            foreach (PhysicsLogic logic in logics)
            {
                logic.RunLogic(dt);
            }
        }
        internal void HandleCollision(Scalar dt, Body first, Body second)
        {
            if (first.Mass.MassInv == 0 && second.Mass.MassInv == 0) { return; }

            if (first.BroadPhaseDetectionOnly || second.BroadPhaseDetectionOnly)
            {
                if (first.BroadPhaseDetectionOnly)
                {
                    first.OnCollision(second);
                }
                if (second.BroadPhaseDetectionOnly)
                {
                    second.OnCollision(first);
                }
            }
            else if (solver.HandleCollision(dt, first, second))
            {
                first.OnCollision(second);
                second.OnCollision(first);
            }
        }
        #endregion
    }
}