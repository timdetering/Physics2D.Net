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
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
namespace AdvanceMath
{
    /// <summary>
    /// Class Used to store a Linear Value along with an Angular Value. Like Position and Orientation. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public struct ALVector2D
    {
        [AdvanceSystem.ComponentModel.UTCParser]
        public static ALVector2D Parse(string text)
        {
            string[] vals = text.Trim(' ', '(', '[', '<', ')', ']', '>').Split(new char[] { ',' }, 2);
            if (vals.Length != 2)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 2 parts separated by commas in the form (x,y) with optional parenthesis.", text));
            }
            else
            {
                try
                {
                    ALVector2D returnvalue;
                    returnvalue.angular = Scalar.Parse(vals[0]);
                    returnvalue.Linear = Vector2D.Parse(vals[1]);
                    return returnvalue;
                }
                catch (Exception ex)
                {
                    throw new FormatException("The parts of the vectors must be decimal numbers", ex);
                }
            }
        }

        /// <summary>
        /// ALVector2D(0,Vector2D.Zero)
        /// </summary>
        public static readonly ALVector2D Zero = new ALVector2D(0, Vector2D.Zero);
        /// <summary>
        /// This is the Angular value of this ALVector2D. 
        /// </summary>
        /// <remarks>Example: Angular would be Orientation and Linear would be Location Completly describing a Position.</remarks>
        Scalar angular;
        /// <summary>
        /// This is the Linear value of this ALVector2D. 
        /// </summary>
        /// <remarks>Example: Angular would be Orientation and Linear would be Location Completly describing a Position.</remarks>
        public Vector2D Linear;

        /// <summary>
        /// Creates a new ALVector2D instance on the stack.
        /// </summary>
        /// <param name="Angular">The Angular value.</param>
        /// <param name="Linear">The Linear value.</param>
        public ALVector2D(Scalar Angular, Vector2D Linear)
        {
            this.angular = Angular;
            this.Linear = Linear;
        }
        [AdvanceSystem.ComponentModel.UTCConstructor]
        public ALVector2D(Scalar Angular, Scalar X, Scalar Y)
        {
            this.angular = Angular;
            this.Linear = new Vector2D(X, Y);
        }
        /// <summary>
        /// This is the Angular value of this ALVector2D. 
        /// </summary>
        /// <remarks>Example: Angular would be Orientation and Linear would be Location Completly describing a Position.</remarks>
        public Scalar Angular
        {
            get { return angular; }
            set { angular = value; }
        }
        [XmlIgnore]
        public Scalar X
        {
            get { return Linear.x; }
            set { Linear.x = value; }
        }
        [XmlIgnore]
        public Scalar Y
        {
            get { return Linear.y; }
            set { Linear.y = value; }
        }
        /// <summary>
        /// Does Addition of 2 ALVector2Ds.
        /// </summary>
        /// <param name="left">The left ALVector2D operand.</param>
        /// <param name="right">The right ALVector2D operand.</param>
        /// <returns>The Sum of the ALVector2Ds.</returns>
        public static ALVector2D operator +(ALVector2D left, ALVector2D right)
        {
            return new ALVector2D(left.angular + right.angular, left.Linear + right.Linear);
        }
        /// <summary>
        /// Does Subtraction of 2 ALVector2Ds.
        /// </summary>
        /// <param name="left">The left ALVector2D operand.</param>
        /// <param name="right">The right ALVector2D operand.</param>
        /// <returns>The Difference of the ALVector2Ds.</returns>
        public static ALVector2D operator -(ALVector2D left, ALVector2D right)
        {
            return new ALVector2D(left.angular - right.angular, left.Linear - right.Linear);
        }
        /// <summary>
        /// Does Multiplication of 2 ALVector2Ds 
        /// </summary>
        /// <param name="source">The ALVector2D to be Multiplied.</param>
        /// <param name="scalar">The Scalar multiplier.</param>
        /// <returns>The Product of the ALVector2Ds.</returns>
        /// <remarks>It does normal Multiplication of the Angular value but does Scalar Multiplication of the Linear value.</remarks>
        public static ALVector2D operator *(ALVector2D source, Scalar scalar)
        {
            return new ALVector2D(source.angular * scalar, source.Linear * scalar);
        }
        /// <summary>
        /// Does Multiplication of 2 ALVector2Ds 
        /// </summary>
        /// <param name="scalar">The Scalar multiplier.</param>
        /// <param name="source">The ALVector2D to be Multiplied.</param>
        /// <returns>The Product of the ALVector2Ds.</returns>
        /// <remarks>It does normal Multiplication of the Angular value but does Scalar Multiplication of the Linear value.</remarks>
        public static ALVector2D operator *(Scalar scalar, ALVector2D source)
        {
            return new ALVector2D(scalar * source.angular, scalar * source.Linear);
        }
        public static bool operator ==(ALVector2D left, ALVector2D right)
        {
            return left.angular == right.angular && left.Linear == right.Linear;
        }
        public static bool operator !=(ALVector2D left, ALVector2D right)
        {
            return left.angular != right.angular && left.Linear != right.Linear;
        }
        public override bool Equals(object obj)
        {
            if (obj is ALVector2D)
            {
                ALVector2D other = (ALVector2D)obj;
                return this == other;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return angular.GetHashCode() ^ Linear.GetHashCode();
        }
        [AdvanceSystem.ComponentModel.UTCFormater]
        public override string ToString()
        {
            return string.Format("({0}, {1})", angular, Linear);
        }
        public Matrix4x4 ToMatrix4x4()
        {
            return Matrix4x4.FromTranslation((Vector3D)Linear) * Matrix3x3.FromRotationZ(angular);
        }
        public Matrix2D ToMatrix2D()
        {
            Matrix2x2 rotation = Matrix2x2.FromRotation(angular);
            return new Matrix2D(rotation, Matrix3x3.FromTranslate2D(Linear) * rotation);
        }
    }
}
