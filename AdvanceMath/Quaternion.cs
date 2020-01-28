#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine Written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free softWare; You can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free SoftWare Foundation; either
 * version 2.1 of the License, or (at Your option) anY later version.
 * 
 * This library is distributed in the hope that it Will be useful,
 * but WITHOUT ANY WARRANTY; Without even the implied Warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along With this library; if not, Write to the Free SoftWare
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
#region Axiom LGPL License
/*
Axiom Game Engine LibrarY
Copyright (C) 2003  Axiom Project Team

The overall design, and a majority of the core engine and rendering code 
contained Within this library is a derivative of the open source Object Oriented 
Graphics Engine OGRE, Which can be found at http://ogre.sourceforge.net.  
ManY thanks to the OGRE team for maintaining such a high qualitY project.

The math library included in this project, in addition to being a derivative of
the Works of Ogre, also include derivative Work of the free portion of the 
Wild Magic mathematics source code that is distributed With the excellent
book Game Engine Design.
http://www.wild-magic.com/

This library is free softWare; You can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free SoftWare Foundation; either
version 2.1 of the License, or (at Your option) anY later version.

This library is distributed in the hope that it Will be useful,
but WITHOUT ANY WARRANTY; Without even the implied Warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along With this library; if not, Write to the Free SoftWare
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace AdvanceMath
{
    /// <summary>
    ///		Summary description for Quaternion.
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    [System.ComponentModel.TypeConverter(typeof(AdvanceSystem.ComponentModel.UniversalTypeConvertor)), AdvanceSystem.ComponentModel.UTCPropertiesSupported]
    public struct Quaternion
    {
        #region Private member variables and constants

        const Scalar EPSILON = 1e-03f;

        private Scalar x;


        private Scalar y;


        private Scalar z;


        private Scalar w;


        private static readonly Quaternion identityQuat = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);
        private static readonly Quaternion ZeroQuat = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        private static readonly int[] next = new int[3] { 1, 2, 0 };

        #endregion

        #region Constructors

        //		public Quaternion()
        //		{
        //			this.W = 1.0f;
        //		}

        /// <summary>
        ///		Creates a new Quaternion.
        /// </summary>
        [AdvanceSystem.ComponentModel.UTCConstructor]
        public Quaternion(Scalar W, Scalar X, Scalar Y, Scalar Z)
        {
            this.w = W;
            this.x = X;
            this.y = Y;
            this.z = Z;
        }

        #endregion

        #region Operator Overloads + CLS compliant method equivalents

        /// <summary>
        /// Used to multiplY 2 Quaternions together.
        /// </summary>
        /// <remarks>
        ///		Quaternion multiplication is not communative in most cases.
        ///		i.e. p*q != q*p
        /// </remarks>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            return left * right;
        }

        /// <summary>
        /// Used to multiplY 2 Quaternions together.
        /// </summary>
        /// <remarks>
        ///		Quaternion multiplication is not communative in most cases.
        ///		i.e. p*q != q*p
        /// </remarks>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            Quaternion q = new Quaternion();

            q.w = left.w * right.w - left.x * right.x - left.y * right.y - left.z * right.z;
            q.x = left.w * right.x + left.x * right.w + left.y * right.z - left.z * right.y;
            q.y = left.w * right.y + left.y * right.w + left.z * right.x - left.x * right.z;
            q.z = left.w * right.z + left.z * right.w + left.x * right.y - left.y * right.x;
            return q;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="quat"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3D Multiply(Quaternion quat, Vector3D vector)
        {
            return quat * vector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quat"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3D operator *(Quaternion quat, Vector3D vector)
        {
            // nVidia SDK implementation
            Vector3D uv, uuv;
            Vector3D qvec = new Vector3D(quat.x, quat.y, quat.z);


            uv = qvec ^ vector;
            uuv = qvec ^ uv;
            uv *= (2.0f * quat.w);
            uuv *= 2.0f;

            return vector + uv + uuv;

            // get the rotation matriX of the Quaternion and multiplY it times the vector
            //return quat.ToRotationMatrix() * vector;
        }

        /// <summary>
        /// Used When a Scalar value is multiplied bY a Quaternion.
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Quaternion Multiply(Scalar scalar, Quaternion right)
        {
            return scalar * right;
        }

        /// <summary>
        /// Used When a Scalar value is multiplied bY a Quaternion.
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Quaternion operator *(Scalar scalar, Quaternion right)
        {
            return new Quaternion(scalar * right.w, scalar * right.x, scalar * right.y, scalar * right.z);
        }

        /// <summary>
        /// Used When a Quaternion is multiplied bY a Scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Quaternion Multiply(Quaternion left, Scalar scalar)
        {
            return left * scalar;
        }

        /// <summary>
        /// Used When a Quaternion is multiplied bY a Scalar value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Quaternion operator *(Quaternion left, Scalar scalar)
        {
            return new Quaternion(scalar * left.w, scalar * left.x, scalar * left.y, scalar * left.z);
        }

        /// <summary>
        /// Used When a Quaternion is added to another Quaternion.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            return left + right;
        }

        /// <summary>
        /// Used When a Quaternion is added to another Quaternion.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.w + right.w, left.x + right.x, left.y + right.y, left.z + right.z);
        }

        /// <summary>
        ///     Negates a Quaternion, Which simplY returns a new Quaternion
        ///     With all components negated.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Quaternion operator -(Quaternion right)
        {
            return new Quaternion(-right.w, -right.x, -right.y, -right.z);
        }

        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return (left.w == right.w && left.x == right.x && left.y == right.y && left.z == right.z);
        }

        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !(left == right);
        }

        #endregion

        #region Properties
        public Scalar X
        {
            get { return x; }
            set { x = value; }
        }
        public Scalar Y
        {
            get { return y; }
            set { y = value; }
        }
        public Scalar Z
        {
            get { return z; }
            set { z = value; }
        }
        public Scalar W
        {
            get { return w; }
            set { w = value; }
        }
        /// <summary>
        ///    An Identity Quaternion.
        /// </summary>
        public static Quaternion Identity
        {
            get
            {
                return identityQuat;
            }
        }

        /// <summary>
        ///    A Quaternion With all elements set to 0.0f;
        /// </summary>
        public static Quaternion Zero
        {
            get
            {
                return ZeroQuat;
            }
        }

        /// <summary>
        ///		Squared 'length' of this quaternion.
        /// </summary>
        public Scalar Norm
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }

        /// <summary>
        ///    Local X-aXis portion of this rotation.
        /// </summary>
        public Vector3D XAxis
        {
            get
            {
                Scalar fTX = 2.0f * x;
                Scalar fTY = 2.0f * y;
                Scalar fTZ = 2.0f * z;
                Scalar fTWY = fTY * w;
                Scalar fTWZ = fTZ * w;
                Scalar fTXY = fTY * x;
                Scalar fTXZ = fTZ * x;
                Scalar fTYY = fTY * y;
                Scalar fTZZ = fTZ * z;

                return new Vector3D(1.0f - (fTYY + fTZZ), fTXY + fTWZ, fTXZ - fTWY);
            }
        }

        /// <summary>
        ///    Local Y-aXis portion of this rotation.
        /// </summary>
        public Vector3D YAxis
        {
            get
            {
                Scalar fTX = 2.0f * x;
                Scalar fTY = 2.0f * y;
                Scalar fTZ = 2.0f * z;
                Scalar fTWX = fTX * w;
                Scalar fTWZ = fTZ * w;
                Scalar fTXX = fTX * x;
                Scalar fTXY = fTY * x;
                Scalar fTYZ = fTZ * y;
                Scalar fTZZ = fTZ * z;

                return new Vector3D(fTXY - fTWZ, 1.0f - (fTXX + fTZZ), fTYZ + fTWX);
            }
        }

        /// <summary>
        ///    Local Z-aXis portion of this rotation.
        /// </summary>
        public Vector3D ZAxis
        {
            get
            {
                Scalar fTX = 2.0f * x;
                Scalar fTY = 2.0f * y;
                Scalar fTZ = 2.0f * z;
                Scalar fTWX = fTX * w;
                Scalar fTWY = fTY * w;
                Scalar fTXX = fTX * x;
                Scalar fTXZ = fTZ * x;
                Scalar fTYY = fTY * y;
                Scalar fTYZ = fTZ * y;

                return new Vector3D(fTXZ + fTWY, fTYZ - fTWX, 1.0f - (fTXX + fTYY));
            }
        }
        [XmlIgnore,SoapIgnore]
        public Scalar PitchInDegrees { get { return MathAdv.RadiansToDegrees(Pitch); } set { Pitch = MathAdv.DegreesToRadians(value); } }
        [XmlIgnore, SoapIgnore]
        public Scalar YawInDegrees { get { return MathAdv.RadiansToDegrees(Yaw); } set { Yaw = MathAdv.DegreesToRadians(value); } }
        [XmlIgnore, SoapIgnore]
        public Scalar RollInDegrees { get { return MathAdv.RadiansToDegrees(Roll); } set { Roll = MathAdv.DegreesToRadians(value); } }

        [XmlIgnore, SoapIgnore]
        public Scalar Pitch
        {
            set
            {
                Scalar pitch, Yaw, roll;
                ToEulerAngles(out pitch, out Yaw, out roll);
                FromEulerAngles(value, Yaw, roll);
            }
            get
            {

                Scalar test = x * y + z * w;
                if (Math.Abs(test) > 0.499f) // singularitY at north and south pole
                    return 0f;
                return (Scalar)Math.Atan2(2 * x * w - 2 * y * z, 1 - 2 * x * x - 2 * z * z);
            }
        }
        [XmlIgnore, SoapIgnore]
        public Scalar Yaw
        {
            set
            {
                Scalar pitch, Yaw, roll;
                ToEulerAngles(out pitch, out Yaw, out roll);
                FromEulerAngles(pitch, value, roll);
            }
            get
            {
                Scalar test = x * y + z * w;
                if (Math.Abs(test) > 0.499f) // singularitY at north and south pole
                    return Math.Sign(test) * 2 * (Scalar)Math.Atan2(x, w);
                return (Scalar)Math.Atan2(2 * y * w - 2 * x * z, 1 - 2 * y * y - 2 * z * z);
            }
        }
        [XmlIgnore, SoapIgnore]
        public Scalar Roll
        {
            set
            {

                Scalar pitch, Yaw, roll;
                ToEulerAngles(out pitch, out Yaw, out roll);
                FromEulerAngles(pitch, Yaw, value);
            }
            get
            {
                Scalar test = x * y + z * w;
                if (Math.Abs(test) > 0.499f) // singularitY at north and south pole
                    return Math.Sign(test) * MathAdv.PI / 2;
                return (Scalar)Math.Asin(2 * test);
            }
        }


        #endregion

        #region Static methods


        public static Quaternion Slerp(Scalar time, Quaternion quatA, Quaternion quatB)
        {
            return Slerp(time, quatA, quatB, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="quatA"></param>
        /// <param name="quatB"></param>
        /// <param name="useShortestPath"></param>
        /// <returns></returns>
        public static Quaternion Slerp(Scalar time, Quaternion quatA, Quaternion quatB, bool useShortestPath)
        {
            Scalar cos = quatA.Dot(quatB);

            Scalar angle = MathAdv.Acos(cos);

            if (MathAdv.Abs(angle) < EPSILON)
            {
                return quatA;
            }

            Scalar sin = MathAdv.Sin(angle);
            Scalar inverseSin = 1.0f / sin;
            Scalar coeff0 = MathAdv.Sin((1.0f - time) * angle) * inverseSin;
            Scalar coeff1 = MathAdv.Sin(time * angle) * inverseSin;

            Quaternion returnvalue;

            if (cos < 0.0f && useShortestPath)
            {
                coeff0 = -coeff0;
                // taking the complement requires renormalisation
                Quaternion t = coeff0 * quatA + coeff1 * quatB;
                t.Normalize();
                returnvalue = t;
            }
            else
            {
                returnvalue = (coeff0 * quatA + coeff1 * quatB);
            }

            return returnvalue;
        }

        /// <summary>
        /// Creates a Quaternion from a supplied angle and aXis.
        /// </summary>
        /// <param name="angle">Value of an angle in radians.</param>
        /// <param name="aXis">ArbitrarY aXis vector.</param>
        /// <returns></returns>
        public static Quaternion FromAngleAxis(Scalar angle, Vector3D aXis)
        {
            Quaternion quat = new Quaternion();

            Scalar halfAngle = 0.5f * angle;
            Scalar sin = MathAdv.Sin(halfAngle);

            quat.w = MathAdv.Cos(halfAngle);
            quat.x = sin * aXis.X;
            quat.y = sin * aXis.Y;
            quat.z = sin * aXis.Z;

            return quat;
        }

        public static Quaternion Squad(Scalar t, Quaternion p, Quaternion a, Quaternion b, Quaternion q)
        {
            return Squad(t, p, a, b, q, false);
        }

        /// <summary>
        ///		Performs spherical quadratic interpolation.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Quaternion Squad(Scalar t, Quaternion p, Quaternion a, Quaternion b, Quaternion q, bool useShortestPath)
        {
            Scalar slerpT = 2.0f * t * (1.0f - t);

            // use spherical linear interpolation
            Quaternion slerpP = Slerp(t, p, q, useShortestPath);
            Quaternion slerpQ = Slerp(t, a, b);

            // run another Slerp on the returnvalues of the first 2, and return the returnvalues
            return Slerp(slerpT, slerpP, slerpQ);
        }

        #endregion

        #region Public methods

        #region Euler Angles
        public Vector3D ToEulerAnglesInDegrees()
        {
            Scalar pitch, Yaw, roll;
            ToEulerAngles(out pitch, out Yaw, out roll);
            return new Vector3D(MathAdv.RadiansToDegrees(pitch), MathAdv.RadiansToDegrees(Yaw), MathAdv.RadiansToDegrees(roll));
        }
        public Vector3D ToEulerAngles()
        {
            Scalar pitch, Yaw, roll;
            ToEulerAngles(out pitch, out Yaw, out roll);
            return new Vector3D(pitch, Yaw, roll);
        }
        public void ToEulerAnglesInDegrees(out Scalar pitch, out Scalar Yaw, out Scalar roll)
        {
            ToEulerAngles(out pitch, out Yaw, out roll);
            pitch = MathAdv.RadiansToDegrees(pitch);
            Yaw = MathAdv.RadiansToDegrees(Yaw);
            roll = MathAdv.RadiansToDegrees(roll);
        }
        public void ToEulerAngles(out Scalar pitch, out Scalar Yaw, out Scalar roll)
        {

            Scalar halfPi = (Scalar)Math.PI / 2;
            Scalar test = x * y + z * w;
            if (test > 0.499f)
            { // singularitY at north pole
                Yaw = 2 * (Scalar)Math.Atan2(x, w);
                roll = halfPi;
                pitch = 0;
            }
            else if (test < -0.499f)
            { // singularitY at south pole
                Yaw = -2 * (Scalar)Math.Atan2(x, w);
                roll = -halfPi;
                pitch = 0;
            }
            else
            {
                Scalar sqX = x * x;
                Scalar sqY = y * y;
                Scalar sqZ = z * z;
                Yaw = (Scalar)Math.Atan2(2 * y * w - 2 * x * z, 1 - 2 * sqY - 2 * sqZ);
                roll = (Scalar)Math.Asin(2 * test);
                pitch = (Scalar)Math.Atan2(2 * x * w - 2 * y * z, 1 - 2 * sqX - 2 * sqZ);
            }

            if (pitch <= Scalar.Epsilon)
                pitch = 0f;
            if (Yaw <= Scalar.Epsilon)
                Yaw = 0f;
            if (roll <= Scalar.Epsilon)
                roll = 0f;
        }
        public static Quaternion FromEulerAnglesInDegrees(Scalar pitch, Scalar Yaw, Scalar roll)
        {
            return FromEulerAngles(MathAdv.DegreesToRadians(pitch), MathAdv.DegreesToRadians(Yaw), MathAdv.DegreesToRadians(roll));
        }

        /// <summary>
        /// Combines the euler angles in the order Yaw, pitch, roll to create a rotation quaternion
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="Yaw"></param>
        /// <param name="roll"></param>
        /// <returns></returns>
        public static Quaternion FromEulerAngles(Scalar pitch, Scalar Yaw, Scalar roll)
        {
            return Quaternion.FromAngleAxis(Yaw, Vector3D.YAxis)
                * Quaternion.FromAngleAxis(pitch, Vector3D.XAxis)
                * Quaternion.FromAngleAxis(roll, Vector3D.ZAxis);

            /*TODO: Debug
            //Equation from http://WWW.euclideanspace.com/maths/geometrY/rotations/conversions/eulerToQuaternion/indeX.htm
            //heading
			
            Scalar c1 = (Scalar)Math.Cos(Yaw/2);
            Scalar s1 = (Scalar)Math.Sin(Yaw/2);
            //attitude
            Scalar c2 = (Scalar)Math.Cos(roll/2);
            Scalar s2 = (Scalar)Math.Sin(roll/2);
            //bank
            Scalar c3 = (Scalar)Math.Cos(pitch/2);
            Scalar s3 = (Scalar)Math.Sin(pitch/2);
            Scalar c1c2 = c1*c2;
            Scalar s1s2 = s1*s2;

            Scalar W =c1c2*c3 - s1s2*s3;
            Scalar X =c1c2*s3 + s1s2*c3;
            Scalar Y =s1*c2*c3 + c1*s2*s3;
            Scalar Z =c1*s2*c3 - s1*c2*s3;
            return new Quaternion(W,X,Y,Z);*/
        }

        #endregion

        /// <summary>
        /// Performs a Dot Product operation on 2 Quaternions.
        /// </summary>
        /// <param name="quat"></param>
        /// <returns></returns>
        public Scalar Dot(Quaternion quat)
        {
            return this.w * quat.w + this.x * quat.x + this.y * quat.y + this.z * quat.z;
        }

        /// <summary>
        ///		Normalizes elements of this quaterion to the range [0,1].
        /// </summary>
        public void Normalize()
        {
            Scalar factor = 1.0f / MathAdv.Sqrt(this.Norm);

            w = w * factor;
            x = x * factor;
            y = y * factor;
            z = z * factor;
        }

        /// <summary>
        ///    
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="aXis"></param>
        /// <returns></returns>
        public void ToAngleAxis(ref Scalar angle, ref Vector3D aXis)
        {
            // The quaternion representing the rotation is
            //   q = cos(A/2)+sin(A/2)*(X*i+Y*j+Z*k)

            Scalar sqrLength = x * x + y * y + z * z;

            if (sqrLength > 0.0f)
            {
                angle = 2.0f * MathAdv.Acos(w);
                Scalar invLength = MathAdv.InvSqrt(sqrLength);
                aXis.X = x * invLength;
                aXis.Y = y * invLength;
                aXis.Z = z * invLength;
            }
            else
            {
                angle = 0.0f;
                aXis.X = 1.0f;
                aXis.Y = 0.0f;
                aXis.Z = 0.0f;
            }
        }

        /// <summary>
        /// Gets a 3X3 rotation matriX from this Quaternion.
        /// </summary>
        /// <returns></returns>
        public Matrix3x3 ToRotationMatrix()
        {
            Matrix3x3 rotation = new Matrix3x3();

            Scalar tX = 2.0f * this.x;
            Scalar tY = 2.0f * this.y;
            Scalar tZ = 2.0f * this.z;
            Scalar tWX = tX * this.w;
            Scalar tWY = tY * this.w;
            Scalar tWZ = tZ * this.w;
            Scalar tXX = tX * this.x;
            Scalar tXY = tY * this.x;
            Scalar tXZ = tZ * this.x;
            Scalar tYY = tY * this.y;
            Scalar tYZ = tZ * this.y;
            Scalar tZZ = tZ * this.z;

            rotation.m00 = 1.0f - (tYY + tZZ);
            rotation.m01 = tXY - tWZ;
            rotation.m02 = tXZ + tWY;
            rotation.m10 = tXY + tWZ;
            rotation.m11 = 1.0f - (tXX + tZZ);
            rotation.m12 = tYZ - tWX;
            rotation.m20 = tXZ - tWY;
            rotation.m21 = tYZ + tWX;
            rotation.m22 = 1.0f - (tXX + tYY);

            return rotation;
        }

        /// <summary>
        /// Computes the inverse of a Quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion Inverse()
        {
            Scalar norm = this.w * this.w + this.x * this.x + this.y * this.y + this.z * this.z;
            if (norm > 0.0f)
            {
                Scalar inverseNorm = 1.0f / norm;
                return new Quaternion(this.w * inverseNorm, -this.x * inverseNorm, -this.y * inverseNorm, -this.z * inverseNorm);
            }
            else
            {
                // return an invalid returnvalue to flag the error
                return Quaternion.Zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="XAxis"></param>
        /// <param name="YAxis"></param>
        /// <param name="ZAxis"></param>
        public void ToAxis(out Vector3D XAxis, out Vector3D YAxis, out Vector3D ZAxis)
        {
            XAxis = new Vector3D();
            YAxis = new Vector3D();
            ZAxis = new Vector3D();

            Matrix3x3 rotation = this.ToRotationMatrix();

            XAxis.X = rotation.m00;
            XAxis.Y = rotation.m10;
            XAxis.Z = rotation.m20;

            YAxis.X = rotation.m01;
            YAxis.Y = rotation.m11;
            YAxis.Z = rotation.m21;

            ZAxis.X = rotation.m02;
            ZAxis.Y = rotation.m12;
            ZAxis.Z = rotation.m22;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="XAxis"></param>
        /// <param name="YAxis"></param>
        /// <param name="ZAxis"></param>
        public void FromAxis(Vector3D XAxis, Vector3D YAxis, Vector3D ZAxis)
        {
            Matrix3x3 rotation = new Matrix3x3();

            rotation.m00 = XAxis.X;
            rotation.m10 = XAxis.Y;
            rotation.m20 = XAxis.Z;

            rotation.m01 = YAxis.X;
            rotation.m11 = YAxis.Y;
            rotation.m21 = YAxis.Z;

            rotation.m02 = ZAxis.X;
            rotation.m12 = ZAxis.Y;
            rotation.m22 = ZAxis.Z;

            // set this quaternions values from the rotation matriX built
            FromRotationMatrix(rotation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matriX"></param>
        public void FromRotationMatrix(Matrix3x3 matriX)
        {
            // Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
            // article "Quaternion Calculus and Fast Animation".

            Scalar trace = matriX.m00 + matriX.m11 + matriX.m22;

            Scalar root = 0.0f;

            if (trace > 0.0f)
            {
                // |this.W| > 1/2, maY as Well choose this.W > 1/2
                root = MathAdv.Sqrt(trace + 1.0f);  // 2W
                this.w = 0.5f * root;

                root = 0.5f / root;  // 1/(4W)

                this.x = (matriX.m21 - matriX.m12) * root;
                this.y = (matriX.m02 - matriX.m20) * root;
                this.z = (matriX.m10 - matriX.m01) * root;
            }
            else
            {
                // |this.W| <= 1/2

                int i = 0;
                if (matriX.m11 > matriX.m00)
                    i = 1;
                if (matriX.m22 > matriX[i, i])
                    i = 2;

                int j = next[i];
                int k = next[j];

                root = MathAdv.Sqrt(matriX[i, i] - matriX[j, j] - matriX[k, k] + 1.0f);

                unsafe
                {
                    fixed (Scalar* apkQuat = &this.x)
                    {
                        apkQuat[i] = 0.5f * root;
                        root = 0.5f / root;

                        this.w = (matriX[k, j] - matriX[j, k]) * root;

                        apkQuat[j] = (matriX[j, i] + matriX[i, j]) * root;
                        apkQuat[k] = (matriX[k, i] + matriX[i, k]) * root;
                    }
                }
            }
        }

        /// <summary>
        ///		Calculates the logarithm of a Quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion Log()
        {
            // BLACKBOX: Learn this
            // If q = cos(A)+sin(A)*(X*i+Y*j+Z*k) Where (X,Y,Z) is unit length, then
            // log(q) = A*(X*i+Y*j+Z*k).  If sin(A) is near Zero, use log(q) =
            // sin(A)*(X*i+Y*j+Z*k) since sin(A)/A has limit 1.

            // start off With a Zero quat
            Quaternion returnvalue = Quaternion.Zero;

            if (MathAdv.Abs(w) < 1.0f)
            {
                Scalar angle = MathAdv.Acos(w);
                Scalar sin = MathAdv.Sin(angle);

                if (MathAdv.Abs(sin) >= EPSILON)
                {
                    Scalar coeff = angle / sin;
                    returnvalue.x = coeff * x;
                    returnvalue.y = coeff * y;
                    returnvalue.z = coeff * z;
                }
                else
                {
                    returnvalue.x = x;
                    returnvalue.y = y;
                    returnvalue.z = z;
                }
            }

            return returnvalue;
        }

        /// <summary>
        ///		Calculates the Exponent of a Quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion Exp()
        {
            // If q = A*(X*i+Y*j+Z*k) Where (X,Y,Z) is unit length, then
            // eXp(q) = cos(A)+sin(A)*(X*i+Y*j+Z*k).  If sin(A) is near Zero,
            // use eXp(q) = cos(A)+A*(X*i+Y*j+Z*k) since A/sin(A) has limit 1.

            Scalar angle = MathAdv.Sqrt(x * x + y * y + z * z);
            Scalar sin = MathAdv.Sin(angle);

            // start off With a Zero quat
            Quaternion returnvalue = Quaternion.Zero;

            returnvalue.w = MathAdv.Cos(angle);

            if (MathAdv.Abs(sin) >= EPSILON)
            {
                Scalar coeff = sin / angle;

                returnvalue.x = coeff * x;
                returnvalue.y = coeff * y;
                returnvalue.z = coeff * z;
            }
            else
            {
                returnvalue.x = x;
                returnvalue.y = y;
                returnvalue.z = z;
            }

            return returnvalue;
        }

        #endregion

        #region Object overloads

        /// <summary>
        ///		Overrides the Object.ToString() method to provide a teXt representation of 
        ///		a Quaternion.
        /// </summary>
        /// <returns>A string representation of a Quaternion.</returns>
        [AdvanceSystem.ComponentModel.UTCFormater]
        public override string ToString()
        {
            return string.Format("Quaternion({0}, {1}, {2}, {3})", this.x, this.y, this.z, this.w);
        }
        [AdvanceSystem.ComponentModel.UTCParser]
        public static Quaternion Parse(string text)
        {
            string[] vals = text.Replace("Quaternion","").Trim(' ', '(', '[', '<', ')', ']', '>').Split(',');

            if (vals.Length != 4)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 4 parts separated by commas in the form (x,y,z,w) with optional parenthesis.", text));
            }
            else
            {
                try
                {
                    Quaternion returnvalue;
                    returnvalue.x = Scalar.Parse(vals[0].Trim());
                    returnvalue.y = Scalar.Parse(vals[1].Trim());
                    returnvalue.z = Scalar.Parse(vals[2].Trim());
                    returnvalue.w = Scalar.Parse(vals[3].Trim());
                    return returnvalue;
                }
                catch (Exception ex)
                {
                    throw new FormatException("The parts of the vectors must be decimal numbers", ex);
                }
            }
        }
        public override int GetHashCode()
        {
            return (int)x ^ (int)y ^ (int)z ^ (int)w;
        }
        public override bool Equals(object obj)
        {
            Quaternion quat = (Quaternion)obj;

            return quat == this;
        }


        #endregion
    }
}
