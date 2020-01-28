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
using System.Collections.Generic;

namespace AdvanceMath
{
    public sealed partial class MathAdv
    {

        public const Scalar Tolerance = 0.000000001f;
        public static Scalar RadianMin(Scalar radianAngle)
        {
            if (Math.Abs(radianAngle) > TWO_PI)
            {
                while (Math.Abs(radianAngle) > PI)
                {
                    radianAngle -= Math.Sign(radianAngle) * TWO_PI;
                }
            }
            else if (Math.Abs(radianAngle) > PI)
            {
                radianAngle -= Math.Sign(radianAngle) * TWO_PI;
                if (Math.Abs(radianAngle) > PI)
                {
                    radianAngle -= Math.Sign(radianAngle) * TWO_PI;
                }
            }
            return radianAngle;
        }

        public static Scalar GetAngleDifference(Scalar radianAngle1, Scalar radianAngle2)
        {
            radianAngle1 = RadianMin(radianAngle1);
            radianAngle2 = RadianMin(radianAngle2);
            Scalar returnvalue = radianAngle1 - radianAngle2;
            if (Math.Abs(returnvalue) > Math.PI)
            {
                returnvalue -= (Scalar)(2 * Math.Sign(returnvalue) * Math.PI);
            }
            return returnvalue;
        }
        public static Scalar GetVelocity(Scalar velocity, Scalar accel, Scalar distance)
        {
            return velocity + 2 * accel * distance;
        }
        public static Scalar GetDistance(Scalar velocity, Scalar accel, Scalar time)
        {
            return (Scalar)(velocity * time + .5 * accel * time * time);
        }
        public static Scalar GetDistance2(Scalar initialvelocity, Scalar finalvelocity, Scalar time)
        {
            return (Scalar)(time * .5 * (initialvelocity - finalvelocity));
        }
        
        /// <summary>
        /// Trys to Solve for x in the equation: (a * (x * x) + b * x + c == 0)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="plus">The result of (b + Math.Sqrt((b * b) - (4 * a * c))) / (2 * a)</param>
        /// <param name="minus">The result of (b - Math.Sqrt((b * b) - (4 * a * c))) / (2 * a)</param>
        /// <returns><see langword="false" /> if an error would have been thrown; otherwise <see langword="true" />.</returns>
        public static bool TrySolveQuadratic(Scalar a, Scalar b, Scalar c, out Scalar plus, out Scalar minus)
        {
            if (a == 0)
            {
                plus = -c / b;
                minus = plus;
                return true;
            }
            c = (b * b) - (4 * a * c);
            if (0 <= c)
            {
                b = -b;
                c = Sqrt(c);
                a = .5f / a;
                plus = ((b + c) * a);
                minus = ((b - c) * a);
                return true;
            }
            plus = 0;
            minus = 0;
            return false;
        }
    }
    /// <summary>
    /// Generates prime numbers. Just felt like writting one.
    /// </summary>
    public sealed class PrimeNumberGenerator 
    {
        List<int> primes;
        int maxNumber;
        public PrimeNumberGenerator(int maxNumber)
        {
            if (maxNumber < 2)
            {
                throw new ArgumentOutOfRangeException("maxNumber");
            }
            primes = new List<int>(Math.Max((int)Math.Sqrt(maxNumber),10));
            this.maxNumber = maxNumber;
            
            
            for (int pos = 2; pos <= maxNumber; ++pos)
            {
                if (PrivateIsPrime(pos))
                {
                    primes.Add(pos);
                }
            }
        }
        public List<int> Primes
        {
            get { return primes; }
        }
        public int MaxNumber
        {
            get { return maxNumber; }
        }
        private bool PrivateIsPrime(int number)
        {
            int length = primes.Count;
            for (int pos = 0; pos < length; ++pos)
            {
                if (number % primes[pos] == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsPrime(int number)
        {
            if (number > maxNumber || number < 1)
            {
                throw new ArgumentOutOfRangeException("number");
            }
            return PrivateIsPrime(number);
        }
    }
}
