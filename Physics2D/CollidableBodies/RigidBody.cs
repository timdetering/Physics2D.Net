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

using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath;

namespace Physics2D.CollidableBodies
{

    /// <summary>
    /// This is the Rigid Body class.
    /// It Holds all the information about a collidable that is needed for collisions.
    /// </summary>
    [Serializable]
    public class RigidBody : FormlessBody, IGravitySource, ICollidableBody
    {
        [AdvanceSystem.ComponentModel.DefaultObjectValueAttribute]
        public static RigidBody Empty
        {
            get
            {
                return new RigidBody(new MassInertia(), new PhysicsState(), new ICollidableBodyPart[] { RigidBodyPart.Empty });
            }
        }

        #region fields
        protected float boundingRadius;
        [NonSerialized]
        protected BoundingBox2D initialBoundingBox2D;
        protected CollisionState collisionState = new CollisionState();
        protected ICollidableBodyPart[] collidableParts;
        protected int partcount;
        #endregion
        #region constructors
        public RigidBody(MassInertia massInertia, float orientation, Vector2D position, IGeometry2D geometry)
            : this(massInertia, new PhysicsState(new ALVector2D(orientation, position)), geometry) 
        {}
        public RigidBody(MassInertia massInertia, PhysicsState state, IGeometry2D geometry)
            : this(
            massInertia, 
            state, 
            new ICollidableBodyPart[] { new RigidBodyPart(ALVector2D.Zero, state.Position, geometry, new Coefficients(0.8f, 0.4f, 0.1f)) })
        {}
        public RigidBody(
            MassInertia massInertia, 
            PhysicsState state, 
            ICollidableBodyPart[] collidableParts)
            : this(new Immortal(), massInertia, state, BodyFlags.None, collidableParts)
        {}
        public RigidBody(
            ILifeTime lifeTime, 
            MassInertia massInertia, 
            PhysicsState state, 
            BodyFlags flags, 
            ICollidableBodyPart[] collidableParts)
            : base(
            lifeTime, flags, 
            massInertia, 
            state)
        {
            SetICollidableBodyParts(collidableParts, -1);
        }
        public RigidBody(
            ILifeTime lifeTime, 
            PhysicsState state, 
            BodyFlags flags, 
            RigidBodyTemplate template)
            : base(
            lifeTime, flags, 
            template.GetMassInertia(), 
            state)
        {
            SetICollidableBodyParts(template.GetCollidableParts(state.Position),template.BoundingRadius);
        }
        protected RigidBody(RigidBody copy)
            : base(copy)
        {
            this.boundingRadius = copy.boundingRadius;
            this.collisionState = (CollisionState)copy.collisionState.Clone();
            this.partcount = copy.partcount;
            this.collidableParts = new ICollidableBodyPart[copy.partcount];
            for (int pos = 0; pos < partcount; ++pos)
            {
                this.collidableParts[pos] = (ICollidableBodyPart)copy.collidableParts[pos].Clone();
            }
        }
        #endregion
        #region properties
        [System.ComponentModel.Browsable(false)]
        public BoundingBox2D SweepBoundingBox2D
        {
            get
            {
                if (initialBoundingBox2D == null)
                {
                    return boundingBox2D;
                }
                else
                {
                    return BoundingBox2D.From2BoundingBox2Ds(initialBoundingBox2D, boundingBox2D);
                }
            }
        }
        public CollisionState CollisionState
        {
            get
            {
                return collisionState;
            }
            set
            {
                collisionState = value;
            }
        }
        public Coefficients Coefficients { get { return this.collidableParts[0].Coefficients; } }
        [System.ComponentModel.Browsable(false)]
        public float BoundingRadius
        {
            get
            {
                return this.boundingRadius;
            }
        }
        [System.ComponentModel.Editor(typeof(AdvanceSystem.ComponentModel.Design.XmlArrayEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ICollidableBodyPart[] CollidableParts
        {
            get
            {
                return collidableParts;
            }
            set
            {
                collidableParts = value;
            }
        }
        #endregion
        #region methods
        private void CheckICollidableBodyParts()
        {
            if(collidableParts ==null)
            {
                throw new ArgumentNullException("collidableParts");
            }
            for (int pos = 0; pos < collidableParts.Length; ++pos)
            {
                if(collidableParts[pos] == null)
                {
                    throw new ArgumentNullException("collidableParts", string.Format("The {0} Index is null",pos));
                }
            }
        }
        protected void SetICollidableBodyParts(ICollidableBodyPart[] collidableParts, float boundingRadius)
        {
            this.collidableParts = collidableParts;
            CheckICollidableBodyParts();
            this.partcount = collidableParts.Length;
            if (boundingRadius < 0)
            {
                SetPartsPositions();
                this.boundingRadius = CalcBoundingRadius();
            }
            else
            {
                this.boundingRadius = boundingRadius;
            }
        }
        public void SetAllPositions()
        {
            matrix = current.Position.ToMatrix2D();
            for (int pos = 0; pos != partcount; ++pos)
            {
                collidableParts[pos].SetPosition(current.Position, matrix);
            }
            CalcBoundingBox2D();
            SaveState();
            SaveGood();
        }
        public void SetAllPositions(ALVector2D position)
        {
            current.Position = position;
            SetAllPositions();
        }
        public override void LoadState()
        {
            base.LoadState();
            for (int pos = 0; pos != partcount; ++pos)
            {
                collidableParts[pos].Load();
            }
        }
        public override void LoadPositionState()
        {
            base.LoadPositionState();
            for (int pos = 0; pos != partcount; ++pos)
            {
                collidableParts[pos].Load();
            }
        }
        public override void SaveState()
        {
            initialBoundingBox2D = boundingBox2D;
            base.SaveState();
            for (int pos = 0; pos != partcount; ++pos)
            {
                collidableParts[pos].Save();
            }
        }

        public override sealed void CalcBoundingBox2D()
        {

            for (int pos = 0; pos < partcount; ++pos)
            {
                collidableParts[pos].CalcBoundingBox2D();
            }
            boundingBox2D = collidableParts[0].BoundingBox2D;
            for (int pos = 1; pos < partcount; ++pos)
            {
                boundingBox2D = BoundingBox2D.From2BoundingBox2Ds(boundingBox2D, collidableParts[pos].BoundingBox2D);
            }

        }
        public override void SaveGood()
        {
            base.SaveGood();
            for (int pos = 0; pos != partcount; ++pos)
            {
                collidableParts[pos].SaveGood();
            }
        }
        public override void UpdatePosition(float dt)
        {
            base.UpdatePosition(dt);
            for (int pos = 0; pos != partcount; ++pos)
            {
                collidableParts[pos].SetPosition(current.Position, matrix);
            }
        }
        public float GetK(Vector2D pointRelativeToBody, Vector2D normal)
        {
            if (this.collisionState.Frozen || float.IsInfinity(massInertia.Mass))
            {
                return 0;
            }
            float massK = 0;
            float inertiaK = 0;
            if (!float.IsInfinity(massInertia.Mass))
            {
                massK = massInertia.MassInv;
            }
            if (!float.IsInfinity(massInertia.MomentofInertia))
            {
                inertiaK = (normal * ((massInertia.MomentofInertiaInv * ((pointRelativeToBody ^ normal))) ^ pointRelativeToBody)); ;
            }
            return massK + inertiaK;//massInertia.MassInv+(normal* ((massInertia.MomentofInertiaInv * ((pointRelativeToBody^normal)))^ pointRelativeToBody));
        }
        public void ApplyImpulse(Vector2D pointRelativeToBody, Vector2D Impulse)
        {
            if (this.collisionState.Frozen || float.IsInfinity(massInertia.Mass))
            {
                return;
            }
            current.Velocity.Linear += (Impulse * massInertia.MassInv);
            current.Velocity.Angular += (massInertia.MomentofInertiaInv * (pointRelativeToBody ^ Impulse));
        }
        protected float CalcBoundingRadius()
        {
            SetAllPositions();
            float returnvalue = 0;
            if (partcount == 1)
            {
                returnvalue = collidableParts[0].BaseGeometry.BoundingRadius;
            }
            else
            {
                for (int pos = 0; pos < partcount; ++pos)
                {
                    if (collidableParts[pos].Offset.Linear == Vector2D.Zero)
                    {
                        returnvalue = Math.Max(returnvalue, collidableParts[pos].BaseGeometry.BoundingRadius);
                    }
                    else
                    {
                        if (collidableParts[pos].UseCircleCollision)
                        {

                            Vector2D distance = collidableParts[pos].Offset.Linear.Normalized * collidableParts[pos].BaseGeometry.BoundingRadius + collidableParts[pos].Offset.Linear;
                            returnvalue = Math.Max(returnvalue, distance.Magnitude);
                        }
                        else
                        {
                            foreach (Vertex2D Vertex in collidableParts[pos].Polygon2D.Vertices)
                            {
                                float distance = (Vertex.Position - current.Position.Linear).Magnitude;
                                returnvalue = Math.Max(returnvalue, distance);
                            }
                        }
                    }
                }
            }
            return returnvalue;

        }
        private void SetPartsPositions()
        {
            matrix = current.Position.ToMatrix2D();
            for (int pos = 0; pos != partcount; ++pos)
            {
                collidableParts[pos].SetPosition(current.Position, matrix);
                collidableParts[pos].Save();
                collidableParts[pos].SaveGood();
            }
        }
        public Circle2D ToCircle2D()
        {
            return new Circle2D(boundingRadius, current.Position.Linear);
        }
        public override object Clone()
        {
            return new RigidBody(this);
        }
        #region IGravitySource Members
        public virtual Vector2D GetGravityPullAt(Vector2D position)
        {
            Vector2D diff = current.Position.Linear - position;
            float MagSq = diff.MagnitudeSq;
            if (MagSq > Physics.Tolerance)
            {
                Vector2D diffnormal = (diff * (float)(1 / Math.Sqrt(MagSq)));
                return (massInertia.AccelerationDueToGravity / (MagSq * Physics.MetersPerDistanceUnit * Physics.MetersPerDistanceUnit)) * diffnormal;
            }
            else
            {
                return Vector2D.Origin;
            }
        }
        #endregion
        #endregion
    }
}
