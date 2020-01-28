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
using AdvanceMath;
using AdvanceMath.Geometry2D;
namespace Physics2D.CollidableBodies
{

    [Serializable]
    public class RigidBodyTemplate
    {
        #region fields
        protected float mass;
        protected float inertiaMultiplier;
        protected IGeometry2D[] geometries;
        protected Coefficients[] coefficients;

        protected float boundingRadius;
        #endregion
        #region constructors
        public RigidBodyTemplate(
            MassInertia massInertia,
            IGeometry2D[] geometries,
            Coefficients[] coefficients):this(
            massInertia.Mass,
            massInertia.MassInv * massInertia.MomentofInertia,
            geometries,
            coefficients)
        {}

        public RigidBodyTemplate(
            float mass,
            float inertiaMultiplier,
            IGeometry2D[] geometries,
            Coefficients[] coefficients
            //ColoredTemplate[] colors,
            //bool[] collidable
            )
        {
            this.mass = mass;
            this.inertiaMultiplier = inertiaMultiplier;
            this.geometries = geometries;
            this.coefficients = coefficients;
            this.boundingRadius = CalcBoundingRadius();
        }

        #endregion
        #region properties
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        public float InertiaMultiplier
        {
            get { return inertiaMultiplier; }
            set { inertiaMultiplier = value; }
        }
        public IGeometry2D[] Geometries
        {
            get { return geometries; }
            set { geometries = value; }
        }
        public Coefficients[] Coefficients
        {
            get { return coefficients; }
            set { coefficients = value; }
       }
        /*public ColoredTemplate[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }*/
        public float BoundingRadius
        {
            get
            {
                return boundingRadius;
            }
        }

        #endregion
        #region methods
        public ICollidableBodyPart[] GetCollidableParts(ALVector2D position)
        {
            int count = geometries.Length;
            ICollidableBodyPart[] returnvalue = new ICollidableBodyPart[count];
            for (int pos = 0; pos != count; ++pos)
            {
                ALVector2D Offset = geometries[pos].Position;
                //if (colors == null || colors[pos] == null)
                //{
                    returnvalue[pos] = new RigidBodyPart(Offset, position, geometries[pos], coefficients[pos]);
                //}
                //else
                //{
                    //returnvalue[pos] = new ColoredRigidBodyPart(Offset, position, geometries[pos], coefficients[pos], colors[pos].Colors, colors[pos].PrimaryColor);
                //}
            }
            return returnvalue;
        }
        public MassInertia GetMassInertia()
        {
            return new MassInertia(mass, mass * inertiaMultiplier);
        }
        public void BalanceBody()
        {
            int polyCount = geometries.Length;
            float[] areas = new float[polyCount];
            Vector2D[] positions = new Vector2D[polyCount];
            for (int pos = 0; pos < polyCount; ++pos)
            {
                Vector2D cog = geometries[pos].Centroid;
                areas[pos] = geometries[pos].Area;
                positions[pos] = geometries[pos].Position.Linear+ cog;
                geometries[pos].Shift(-cog);
            }
            Vector2D offset = GetCenterOfGravity(areas, positions);
            for (int pos = 0; pos < polyCount; ++pos)
            {
                geometries[pos].Position= new ALVector2D(geometries[pos].Position.Angular,geometries[pos].Position.Linear - offset);
            }
            this.boundingRadius = CalcBoundingRadius();
        }
        /// <remarks>Warning Very Slow!</remarks>
        public void CalcInertiaMultiplier(float incriment)
        {
            inertiaMultiplier = MassInertia.CalcMomentofInertia(geometries, incriment);
        }
        /// <remarks>Warning Very Slow!</remarks>
        /*public static void CalculateCollidable(ref IGeometry[] geometries, float incriment, out float InertiaMultiplyer)
        {
            int polyCount = geometries.Length;
            float[] areas = new float[polyCount];
            Vector2D[] positions = new Vector2D[polyCount];
            for (int pos = 0; pos != polyCount; ++pos)
            {
                Vector2D cog = geometries[pos].Centroid;
                areas[pos] = geometries[pos].Area;
                positions[pos] = geometries[pos].Position.Linear - cog;
                geometries[pos].Shift(cog);
            }
            Vector2D offset = GetCenterOfGravity(areas, positions);
            for (int pos = 0; pos != polyCount; ++pos)
            {
                geometries[pos].Center = geometries[pos].Center - offset;
            }
            InertiaMultiplyer = MassInertia.CalcMomentofInertia(geometries, incriment);
        }*/
        public static Vector2D GetCenterOfGravity(float[] masses, Vector2D[] positions)
        {
            int Length = masses.Length;
            if (Length != positions.Length)
            {
                throw new ArgumentException("Lengths of arrays passed do not match");
            }
            float Totalmass = 0;
            Vector2D returnvalue = Vector2D.Zero;
            for (int pos = 0; pos != Length; ++pos)
            {
                returnvalue += positions[pos] * (masses[pos]);
                Totalmass += masses[pos];
            }
            returnvalue *= (1 / Totalmass);
            return returnvalue;
        }
        protected float CalcBoundingRadius()
        {
            int partcount = geometries.Length;
            float returnvalue = 0;
            for (int pos = 0; pos < partcount; ++pos)
            {
                if (geometries.Length == 1 && geometries[pos].Position.Linear == Vector2D.Zero)
                {
                    returnvalue = Math.Max(returnvalue, geometries[pos].BoundingRadius);
                }
                else
                {
                    Polygon2D poly = geometries[pos] as Polygon2D;
                    if (poly != null)
                    {
                        foreach (Vertex2D Vertex in poly.Vertices)
                        {
                            float distance = Vertex.Position.Magnitude;
                            returnvalue = Math.Max(returnvalue, distance);
                        }
                    }
                    else
                    {
                        Vector2D distance = geometries[pos].Position.Linear.Normalized * geometries[pos].BoundingRadius + geometries[pos].Position.Linear;
                        returnvalue = Math.Max(returnvalue, distance.Magnitude);
                    }
                }
            }
            return returnvalue;
        }
        #endregion
    }
}
