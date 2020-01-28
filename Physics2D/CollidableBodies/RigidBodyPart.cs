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
using AdvanceMath;
using AdvanceMath.Geometry2D;
namespace Physics2D.CollidableBodies
{
    [Serializable]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public class RigidBodyPart : ICollidableBodyPart,IDeserializationCallback
    {
        [AdvanceSystem.ComponentModel.DefaultObjectValue]
        public static RigidBodyPart Empty
        {
            get
            {
                return new RigidBodyPart(ALVector2D.Zero, new Polygon2D(), new Coefficients(0, 0, 0));
            }
        }
        #region fields
        protected IGeometry2D baseGeometry = null;
        protected Coefficients coefficients = null;
        protected ALVector2D offset;
        protected ALVector2D position;
        [NonSerialized]
        protected Matrix2D matrix;
        [NonSerialized]
        protected Matrix2D offsetMatrix;
        [NonSerialized]
        protected ALVector2D initialPosition;
        [NonSerialized]
        protected ALVector2D goodPosition;
        [NonSerialized]
        protected BoundingBox2D boundingBox2D = null;
        [NonSerialized]
        protected Polygon2D polygon2D = null;
        [NonSerialized]
        protected Polygon2D initialPolygon2D = null;
        [NonSerialized]
        protected Polygon2D goodPolygon2D = null;
        protected bool useCircleCollision;
        #endregion
        #region constructors
        public RigidBodyPart(ALVector2D offset, ALVector2D position, IGeometry2D geometry, Coefficients coefficients)
        {
            this.offset = offset;
            this.position = new ALVector2D(offset.Angular, Vector2D.Rotate(position.Angular, offset.Linear)) + position;
            this.coefficients = coefficients;
            this.BaseGeometry = geometry;
            Init();
        }
        public RigidBodyPart(ALVector2D offset, IGeometry2D geometry, Coefficients coefficients)
        {
            this.offset = offset;
            this.position = offset;
            this.coefficients = coefficients;
            this.BaseGeometry = geometry;
            Init();
        }
        protected void Init()
        {
            this.initialPosition = this.position;
            this.goodPosition = this.position;
            this.offsetMatrix = offset.ToMatrix2D();
        }
        protected RigidBodyPart(RigidBodyPart copy)
        {
            this.coefficients = copy.coefficients;
            this.offset = copy.offset;
            this.position = copy.position;
            this.initialPosition = copy.initialPosition;
            this.goodPosition = copy.goodPosition;
            this.baseGeometry = copy.baseGeometry;
            this.useCircleCollision = copy.useCircleCollision;
            if (!useCircleCollision)
            {
                this.polygon2D = new Polygon2D(copy.polygon2D);
                this.initialPolygon2D = new Polygon2D(copy.polygon2D);
                this.goodPolygon2D = new Polygon2D(copy.polygon2D);
            }
            this.offsetMatrix = copy.offsetMatrix;
        }
        #endregion
        #region properties
        [System.ComponentModel.Browsable(false)]
        public Matrix2D Matrix
        {
            get { return matrix; }
        }
        [AdvanceSystem.ComponentModel.UTCConstructorParameter("geometry")]
        public IGeometry2D BaseGeometry
        {
            get
            {
                return baseGeometry;
            }
            set
            {
                this.baseGeometry = value;
                this.useCircleCollision = !(value is Polygon2D);
                if (!useCircleCollision)
                {
                    this.polygon2D =  new Polygon2D((Polygon2D)value);
                    this.polygon2D.Position = position;
                    this.initialPolygon2D = new Polygon2D(polygon2D);
                    this.goodPolygon2D = new Polygon2D(polygon2D);
                }
            }
        }
        [System.ComponentModel.Browsable(false)]
        public BoundingBox2D SweepBoundingBox2D
        {
            get
            {
                if (this.useCircleCollision)
                {
                    return boundingBox2D;
                }
                else
                {
                    return BoundingBox2D.From2BoundingBox2Ds(boundingBox2D,this.initialPolygon2D.BoundingBox2D);
                }
            }
        }
        [System.ComponentModel.Browsable(false)]
        public BoundingBox2D BoundingBox2D
        {
            get
            {
                return boundingBox2D;
            }
        }
        [AdvanceSystem.ComponentModel.UTCConstructorParameter("coefficients")]
        public Coefficients Coefficients
        {
            get
            {
                return coefficients;
            }
            set { }
        }
        [AdvanceSystem.ComponentModel.UTCConstructorParameter("offset")]
        public ALVector2D Offset
        {
            get { return offset; }
            set 
            {
                offset = value;
            }
        }
        [System.ComponentModel.Browsable(false)]
        public Polygon2D Polygon2D
        {
            get { return polygon2D; }
        }
        [System.ComponentModel.Browsable(false)]
        public ALVector2D Position
        {
            get
            {
                return position;
            }
            set
            {
                SetPosition(value);
            }
        }
        [System.ComponentModel.Browsable(false)]
        public ALVector2D InitialPosition
        {
            get { return initialPosition; }
        }
        [System.ComponentModel.Browsable(false)]
        public Polygon2D InitialPolygon2D
        {
            get { return initialPolygon2D; }
        }
        [System.ComponentModel.Browsable(false)]
        public ALVector2D GoodPosition
        {
            get { return goodPosition; }
        }
        [System.ComponentModel.Browsable(false)]
        public Polygon2D GoodPolygon2D
        {
            get { return goodPolygon2D; }
        }
        [System.ComponentModel.Browsable(false)]
        public Vector2D[] DisplayVertices
        {
            get
            {
                if (!this.useCircleCollision)
                {
                    return Vertex2D.PositionToVector2DArray(this.goodPolygon2D.Vertices);
                }
                else
                {
                    return null;
                }
            }
        }
        [System.ComponentModel.Browsable(false)]
        public bool UseCircleCollision
        {
            get { return useCircleCollision; }
        }
        #endregion
        #region methods
        public void Save()
        {
            if (!useCircleCollision)
            {
				initialPolygon2D.Set(polygon2D);
            }
            initialPosition = position;
        }
        public void Load()
        {
            if (!useCircleCollision)
            {
				polygon2D.Set(initialPolygon2D);
            }
            position = initialPosition;
        }
        public void SaveGood()
        {
            if (!useCircleCollision)
            {
                goodPolygon2D.SetPosition(position, matrix);
            }
            goodPosition = position;
        }
        public void CalcBoundingBox2D()
        {
            if (useCircleCollision)
            {
                float OuterRadius = baseGeometry.BoundingRadius;
                float UX = position.Linear.X + OuterRadius;
                float UY = position.Linear.Y + OuterRadius;
                float LX = position.Linear.X - OuterRadius;
                float LY = position.Linear.Y - OuterRadius;
                boundingBox2D = new BoundingBox2D(UX, UY, LX, LY);
            }
            else
            {
                polygon2D.CalcBoundingBox2D();
                boundingBox2D = polygon2D.BoundingBox2D;
            }
        }
        public void SetPosition(ALVector2D Position, Matrix2D matrix)
        {
            this.matrix = matrix * offsetMatrix;
            position.Linear = this.matrix.VertexMatrix * Vector2D.Zero;
            position.Angular = offset.Angular + Position.Angular;
            if (!useCircleCollision)
            {
                polygon2D.SetPosition(position, this.matrix);
            }
        }
        public void SetPosition(ALVector2D Position)
        {
            SetPosition(Position, Position.ToMatrix2D());
        }
        public void OnDeserialization(object sender)
        {
            this.initialPosition = this.position;
            this.goodPosition = this.position;
            if (baseGeometry is Polygon2D)
            {
                ((Polygon2D)baseGeometry).OnDeserialization(null);
            }
            this.BaseGeometry = baseGeometry;

            this.offsetMatrix = offset.ToMatrix2D();
        }
        public virtual object Clone()
        {
            return new RigidBodyPart(this);
        }
        #endregion
    }
}
