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
using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath;
using Physics2D.Collections;

namespace Physics2D.CollidableAreas
{
    [Serializable]
    public class LightningRayGroup : BaseRay2DEffectGroup
    {
        float detail;
        static Random rand = new Random();
        ICollidableBody collidable;
        float impulse;
        public LightningRayGroup(Vector2D first, Vector2D last, float displace, float detail,ICollidableBody collidable,float impulse,ILifeTime lifeTime):base(lifeTime)
        {
            this.detail = detail;
            this.collidable = collidable;
            this.impulse = impulse;
            LinkedList<Vector2D> points = new LinkedList<Vector2D>();
            points.AddFirst(first);
            points.AddLast(last);
            Lightning(points.First, points.Last, displace, points);
            CalcEffects(points);
        }
        private void Lightning(LinkedListNode<Vector2D> first, LinkedListNode<Vector2D> last, float displace, LinkedList<Vector2D> points)
        {
            if (displace < detail)
            {
                return; 
            }
            else
            {
                Vector2D mid = (first.Value + last.Value)*.5f;
                mid.X += ((float)rand.NextDouble() - .5f) * displace;
                mid.Y += ((float)rand.NextDouble() - .5f) * displace;
                LinkedListNode<Vector2D> midnode = new LinkedListNode<Vector2D>(mid);
                points.AddAfter(first, midnode);
                displace *= .5f;
                Lightning(first, midnode, displace, points);
                Lightning(midnode, last, displace, points);
            }
        }
        private void CalcEffects(LinkedList<Vector2D> points)
        {

            ray2DEffects = new TimedList<IRay2DEffect>(points.Count - 1);
            Vector2D last = Vector2D.Zero;
            bool good = false;
            foreach (Vector2D point in points)
            {
                if (good)
                {
                    Vector2D Direction = point - last;
                    float Distance = Direction.Magnitude;
                    Direction = Direction*(1 / Distance);
                    RaySegment2D segment = new RaySegment2D(last, Direction, Distance);
                    ray2DEffects.Add(new AttachedImpulseRay(lifeTime, segment, impulse, collidable));
                }
                good = true;
                last = point;
            }
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
                pairs.Remove(collidable);
                int length = ray2DEffects.Count;
                if (length > 0)
                {
                    bool contact = false;
                    int pos = 0;
                    for (; pos < length; ++pos)
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
                            contact = true;
                            break;
                        }
                    }
                    if (contact)
                    {
                        pos++;
                        if (pos < length)
                        {
                            ray2DEffects.RemoveRange(pos, length - pos);
                        }
                    }
                }
            }
        }
    }
}
