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
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2D.Collections;
namespace Physics2D.CollisionDetection
{





    /// <summary>
    /// handles all the Collision classes to detect collisionFilter.
    /// </summary>
    [Serializable]
    public sealed class CollisionDetector<CB, CA> : ICleanable
        where CB : class, ICollidableBody
        where CA : class, ICollidableArea
    {
        #region feilds
        /// <summary>
        /// List of grouppairs that are to have collision detection ran on.
        /// </summary>
        CollidableBodyList<CB> collidables;
        /// <summary>
        /// List of collidableArea that are to have collision detection ran on.
        /// </summary>
        BaseCollidableList<CA> collidableAreas;
        /// <summary>
        /// The collision grid that does the broadest phased collision detection.
        /// </summary>
        ICollisionFilter<CB> collisionFilter;
        /// <summary>
        /// a collection of CollisionPairs that the collisionFilter returned
        /// </summary>
        public List<CollisionPair> CollisionFilterPairs;
        /// <summary>
        /// a collection of grouppairs that passed the narow phased detection.
        /// </summary>
        public List<CollisionPair> CollisionPairs;
        /// <summary>
        /// holds a list of positions in the CollisionPairs List of where each collision lvl starts
        /// </summary>
        List<int> collisionLevelsPosition = new List<int>();
        #endregion
        #region constructors
        /// <summary>
        /// creates a new CollisionDetector instance.
        /// </summary>
        /// <param name="cellsize">The size of each cell. (for the SweepAndPruneCollisionFilter)</param>
        /// <param name="gridsize">The total size of the grid. (for the SweepAndPruneCollisionFilter)</param>
        public CollisionDetector()
        {
            this.collisionFilter = new SweepAndPruneCollisionFilter<CB>();
            collidables = new CollidableBodyList<CB>();
            CollisionPairs = new List<CollisionPair>();
            collidableAreas = new BaseCollidableList<CA>();
        }
        public CollisionDetector(ICollisionFilter<CB> collisionFilter)
        {
            this.collisionFilter = collisionFilter;
            collidables = new CollidableBodyList<CB>();
            CollisionPairs = new List<CollisionPair>();
            collidableAreas = new BaseCollidableList<CA>();
        }
        #endregion
        #region properties
        public bool CollidableAreasNeedProcessing
        {
            get
            {
                return collidableAreas.Count > 0;
            }
        }
        /// <summary>
        /// Gets the Number of Collision levels
        /// </summary>
        public int LevelCount
        {
            get
            {
                return collisionLevelsPosition.Count;
            }
        }
        public CollisionPairParameters CollisionPairParameters
        {
            get
            {
                return collisionFilter.CollisionPairParameters;
            }
            set
            {
                collisionFilter.CollisionPairParameters = value;
            }
        }
        #endregion
        #region methods
        /// <summary>
        /// adds grouppairs to the list of grouppairs to have collision detection ran on.
        /// </summary>
        /// <param name="candidates">The list of Candidates.</param>
        /// <param name="IncludeNonImpulseApplied">states if grouppairs that not had impulse applied yet should be added.</param>
        /// <param name="filter">A BodyFlags that contains flags that should be filtered out.</param>
        public void AddICollidableBody(CB candidate)
        {
            if (candidate.IgnoreInfo.IsCollidable)
            {
                this.collisionFilter.AddICollidableBody(candidate);
                this.collidables.Add(candidate);
            }
        }
        public void AddICollidableArea(CA area)
        {
            if (area.IgnoreInfo.IsCollidable)
            {
                this.collidableAreas.Add(area);
            }
        }
        public void AddICollidableBodyRange(ICollection<CB> candidates)
        {
            
            int count = candidates.Count;
            if (count > 0)
            {
                int newCapacity = count + collidables.Count;
                if (collidables.Capacity < newCapacity)
                {
                    collidables.Capacity = newCapacity;
                }
                foreach (CB candidate in candidates)
                {
                    AddICollidableBody(candidate);
                }
            }
        }
        public void AddICollidableAreaRange(ICollection<CA> areas)
        {
            int count = areas.Count;
            if (count > 0)
            {
                int newCapacity = count + collidableAreas.Count;
                if (collidableAreas.Capacity < (newCapacity))
                {
                    collidableAreas.Capacity = newCapacity;
                }
                foreach (CA area in areas)
                {
                    AddICollidableArea(area);
                }
            }
        }
        public void ApplyFilter(BodyFlags filter, bool IncludeNonImpulseApplied)
        {
            collisionFilter.ApplyFilter(filter, IncludeNonImpulseApplied);
        }
        /// <summary>
        /// runs thought the narrow phased collision detection.
        /// </summary>
        /// <param name="dt">change in time.</param>
        public void CalcNarrowPhasedCollisions(float dt)
        {
            CollisionPairs.Clear();
            int count = CollisionFilterPairs.Count;
            CollisionPair pair;
            for (int pos = 0; pos != count; ++pos)
            {
                pair = CollisionFilterPairs[pos];
                if (pair.TestBoundingBox2Ds())
                {
                    pair.CalcDistance();
                    if (pair.TestBoundingRadius())
                    {
                        pair.TestCollisions(dt);
                        if (pair.IsValid)
                        {
                            CollisionPairs.Add(pair);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// runs through the brad phased detection
        /// </summary>
        public void CalcBroadPhasedCollisions()
        {
            CollisionFilterPairs = collisionFilter.GetPossibleCollisions();
        }
        /// <summary>
        /// resets the colliders list in the grouppairs CollisionState to be current.
        /// </summary>
        public void ResetColliders()
        {
            int count = collidables.Count;
            for (int pos = 0; pos != count; ++pos)
            {
                collidables[pos].CollisionState.Colliders.Clear();
            }
            count = CollisionPairs.Count;
            for (int pos = 0; pos != count; ++pos)
            {
                CollisionPair pair = CollisionPairs[pos];
                pair.Collidable1.CollisionState.Colliders.Add(pair.Collidable2);
                pair.Collidable2.CollisionState.Colliders.Add(pair.Collidable1);
            }
        }
        public void ResetCollisionPairs()
        {
            int count = collidables.Count;
            for (int pos = 0; pos != count; ++pos)
            {
                collidables[pos].CollisionState.CollisionPairs.Clear();
            }
            count = CollisionPairs.Count;
            for (int pos = 0; pos != count; ++pos)
            {
                CollisionPair pair = CollisionPairs[pos];
                pair.Collidable1.CollisionState.CollisionPairs.Add(pair);
                pair.Collidable2.CollisionState.CollisionPairs.Add(pair);
            }
        }
        /// <summary>
        /// claculates the collision levels of objects.
        /// </summary>
        public void CalcCollisionLevels()
        {
            ResetColliders();
            foreach (ICollidableBody body in collidables)
            {
                if ((body.Flags & BodyFlags.GravityWell) == BodyFlags.GravityWell || (body.Flags & BodyFlags.InfiniteMass) == BodyFlags.InfiniteMass)
                {

                    body.CollisionState.CollisionLevel = 0;
                    foreach (ICollidableBody collider in body.CollisionState.Colliders)
                    {
                        collider.CollisionState.CollisionLevel = 1;
                    }
                }
            }
            bool AllLevelsSet = false;
            for (int level = 0; !AllLevelsSet && level < 30; ++level)
            {
                AllLevelsSet = true;
                foreach (ICollidableBody body in collidables)
                {
                    if (body.CollisionState.CollisionLevel == level)
                    {
                        AllLevelsSet = false;
                        foreach (ICollidableBody collider in body.CollisionState.Colliders)
                        {
                            if (collider.CollisionState.CollisionLevel == -1)//collidable.CollisionState.CollisionLevel)
                            {
                                collider.CollisionState.CollisionLevel = body.CollisionState.CollisionLevel + 1;
                            }
                        }
                    }
                }
            }
            foreach (CollisionPair pair in CollisionPairs)
            {
                pair.CalcCollisionLevel();
            }
            CollisionPairs.Sort(new CollisionPairLvlComparer());
            CalcCollisionLevelsPosition();
        }
        /// <summary>
        /// populates the collisionLevelsPosition list.
        /// </summary>
        private void CalcCollisionLevelsPosition()
        {
            collisionLevelsPosition.Clear();
            int level = -1;
            for (int pos = 0; pos != CollisionPairs.Count; ++pos)
            {
                CollisionPair pair = CollisionPairs[pos];
                if (pair.CollisionLevel != level)
                {
                    collisionLevelsPosition.Add(pos);
                    level = pos;
                }
            }
        }
        /// <summary>
        /// gets a List of CollisionPairs by the collision level it is at.
        /// </summary>
        /// <param name="level">the level to get.</param>
        /// <returns>the CollisionPairs at that level if the level exists; otherwise null.</returns>
        public List<CollisionPair> GetCollisionPairsByLevel(int level)
        {
            if (CollisionPairs.Count == 0)
            {
                return null;
            }
            if (collisionLevelsPosition.Count == 0)//||CollisionPairs.Count==0)
            {
                if (level == 0)
                {
                    return CollisionPairs;
                }
                return null;
            }
            if (collisionLevelsPosition.Count - 1 < level)
            {
                return null;
            }
            int startpos = collisionLevelsPosition[level];
            int endpos = collisionLevelsPosition.Count - 1;
            if (collisionLevelsPosition.Count - 1 <= level)
            {
                endpos = CollisionPairs.Count;
            }
            else
            {
                endpos = collisionLevelsPosition[level + 1];
            }
            return CollisionPairs.GetRange(startpos, endpos - startpos);
        }
        public void ProcessCollidableAreas(float dt)
        {
            int length = collidableAreas.Count;
            List<ICollidableBody>[] grouppairs = collisionFilter.GetIntersections<CA>(this.collidableAreas);
            for (int pos = 0; pos < length; ++pos)
            {
                List<ICollidableBody> pairs = grouppairs[pos];
                if (pairs != null)
                {
                    collidableAreas[pos].HandlePossibleIntersections(dt, pairs);
                }
            }
        }
        public bool RemoveExpired()
        {
            collisionFilter.RemoveExpired();
            collidables.RemoveNonCollidables();
            collidableAreas.RemoveNonCollidables();
            return false;
        }
        public void ClearTemporaryObjects()
        {
            collisionLevelsPosition.Clear();
            CollisionFilterPairs.Clear();
            CollisionPairs.Clear();
        }
        public void Clear()
        {
            collidables.Clear();
            collidableAreas.Clear();
            collisionFilter.ClearICollidableBodys();
        }
        #endregion
        #region subclasses
        /// <summary>
        /// A Comparer to sort the CollisionPairs by level.
        /// </summary>
        [Serializable]
        class CollisionPairLvlComparer : System.Collections.Generic.IComparer<CollisionPair>
        {
            #region IComparer<CollisionPair> Members
            public int Compare(CollisionPair x, CollisionPair y)
            {
                return x.CollisionLevel.CompareTo(y.CollisionLevel);
            }
            #endregion
        }
        #endregion
    }
}
