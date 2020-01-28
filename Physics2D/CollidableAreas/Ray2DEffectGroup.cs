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
using System.Collections.Generic;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2D.CollisionDetection;
using Physics2D.Collections;

namespace Physics2D.CollidableAreas
{
    public abstract class BaseRay2DEffectGroup : BaseCollidable, IRay2DEffectGroup
    {
        protected TimedList<IRay2DEffect> ray2DEffects;
        public BaseRay2DEffectGroup() { this.ray2DEffects = new TimedList<IRay2DEffect>(); }
        public BaseRay2DEffectGroup(ILifeTime lifeTime)
            : base(lifeTime)
        {
            this.ray2DEffects = new TimedList<IRay2DEffect>();
        }
        public BaseRay2DEffectGroup(BaseRay2DEffectGroup copy)
            : base(copy)
        {
            this.ray2DEffects = new TimedList<IRay2DEffect>(copy.ray2DEffects);
        }
        public List<IRay2DEffect> Ray2DEffects
        {
            get { return new List<IRay2DEffect>(ray2DEffects); }
        }
        public abstract void HandlePossibleIntersections(float dt, List<ICollidableBody> collidables);
        public override bool RemoveExpired()
        {
            return ray2DEffects.RemoveExpired() && base.RemoveExpired();
        }
        public override void Update(float dt)
        {
            ray2DEffects.Update(dt);
            base.Update(dt);
        }
        protected static RayICollidableBodyPair GetIntersection(RaySegment2D raySegment, List<ICollidableBody> pairs)
        {
            int collidablesCount = pairs.Count;
            if (collidablesCount == 0)
            {
                return null;
            }
            float Smallestdistance = raySegment.Length;
            RayICollidableBodyPair returnvalue = null;
            RayICollidableBodyPair pair;
            for (int pos = 0; pos < collidablesCount; ++pos)
            {
                pair = new RayICollidableBodyPair(raySegment, pairs[pos]);// new RayICollidableBodyPair(raySegment, grouppairs[pos]);
                if (pair.TestIntersection())
                {
                    if (pair.BestIntersectInfo != null && Smallestdistance > pair.BestIntersectInfo.DistanceFromOrigin)
                    {
                        Smallestdistance = pair.BestIntersectInfo.DistanceFromOrigin;
                        returnvalue = pair;
                    }
                }
            }
            return returnvalue;
        }

    }
    [Serializable]
    public class Ray2DEffectGroup : BaseRay2DEffectGroup
    {
        public Ray2DEffectGroup()
        { }
        public Ray2DEffectGroup(List<IRay2DEffect> effects)
        {
            this.ray2DEffects.AddRange(effects);
            if (this.ray2DEffects.Count > 0)
            {
                lifeTime = new ChildLifeTime(effects[0].LifeTime);
            }
        }
        public void Add(IRay2DEffect effect)
        {
            if (lifeTime == null || lifeTime.TimeLeft < effect.LifeTime.TimeLeft)
            {
                this.lifeTime = new ChildLifeTime(effect.LifeTime);
            }
            ray2DEffects.Add(effect);
        }
        public override void CalcBoundingBox2D()
        {
            int length = ray2DEffects.Count;
            if (length > 0)
            {
                this.boundingBox2D = ray2DEffects[0].RaySegment.BoundingBox2D;
                for (int pos = 1; pos < length; ++pos)
                {
                    boundingBox2D = BoundingBox2D.From2BoundingBox2Ds(boundingBox2D, ray2DEffects[pos].RaySegment.BoundingBox2D);
                }
            }
        }
        static PhysicsState before;
        public override void HandlePossibleIntersections(float dt, List<ICollidableBody> pairs)
        {
            if (pairs != null)
            {
                int length = ray2DEffects.Count;
                for (int pos = 0; pos < length; ++pos)
                {
                    RayICollidableBodyPair pair = GetIntersection(ray2DEffects[pos].RaySegment, pairs);
                    if (pair != null && pair.BestIntersectInfo.Intersects)
                    {
                        if (pair.ICollidableBody.CollisionState.GenerateRayEvents)
                        {
                            before = new PhysicsState(pair.ICollidableBody.Current);
                        }
                        ray2DEffects[pos].ApplyEffect(dt, pair);
                        if (pair.ICollidableBody.CollisionState.GenerateRayEvents)
                        {
                            pair.ICollidableBody.CollisionState.InterferenceInfos.Add(
                                new InterferenceInfo(new RayCollidableInterferenceInfo(this,
                                pair.RaySegment2D,
                                pair.BestIntersectInfo,
                                before,
                                new PhysicsState(pair.ICollidableBody.Current),
                                pair.ICollidableBody)));
                        }
                    }
                }
            }
        }
    }
}