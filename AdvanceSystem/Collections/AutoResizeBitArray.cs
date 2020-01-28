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
using System.Text;
using System.Collections;
using System.Collections.Generic;
namespace AdvanceSystem.Collections
{
    /// <summary>
    /// A Bit array that has no fixed size and rarly throws out of bounds.
    /// </summary>
    [Serializable]
    public sealed class AutoResizeBitArray : ICloneable, IEnumerable<bool>
    {
        #region static stuff

        public static readonly AutoResizeBitArray True = new AutoResizeBitArray(true);
        public static readonly AutoResizeBitArray False = new AutoResizeBitArray(false);

        public static void Copy(AutoResizeBitArray source, int sourceBitIndex, AutoResizeBitArray dest, int destBitIndex, int length)
        {
            //TODO: Make a Less wasteful implementation.
            for (int pos = length-1; pos > -1; --pos)
            {
                dest[destBitIndex + pos] = source[sourceBitIndex + pos];
            }
        }


        #region Highest and Lowest Stuff
        static readonly byte[] highestLookUpTable = GenerateHighestLookUpTable();
        static readonly byte[] lowestLookUpTable = GenerateLowestLookUpTable();
        static byte[] GenerateHighestLookUpTable()
        {
            List<byte> lookup = new List<byte>(byte.MaxValue);
            lookup.Add(0);
            lookup.Add(1);
            for (byte i = 2; i != 0; i *= i)
            {
                for (byte j = 0; j < i; ++j)
                {
                    lookup.Add(i);
                }
            }
            return lookup.ToArray();
        }
        static byte[] GenerateLowestLookUpTable()
        {
            List<byte> lookup = new List<byte>(byte.MaxValue);
            lookup.Add(0);
            for (int i = 1; i < byte.MaxValue; i++)
            {
                for (int j = 0; j < 8; ++j)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        lookup.Add((byte)(j + 1));
                        break;
                    }
                }
            }
            return lookup.ToArray();
        }
        public static int FindHighestTrueBitIndex(int x)
        {
            if ((x & 0xff00) != 0)
            {
                return (highestLookUpTable[x >> 8] << 8) - 1;
            }
            else if ((x & 0x00ff) != 0)
            {
                return highestLookUpTable[x] - 1;
            }
            else
            {
                return -1;
            }
        }
        public static int FindHighestFalseBitIndex(int x)
        {
            return FindHighestTrueBitIndex(~x);
        }
        public static int FindLowestTrueBitIndex(int x)
        {
            if ((x & 0x00ff) != 0)
            {
                return lowestLookUpTable[x] - 1;
            }
            else if ((x & 0xff00) != 0)
            {
                return (lowestLookUpTable[x >> 8] << 8) - 1;
            }
            else
            {
                return -1;
            }
        }
        public static int FindLowestFalseBitIndex(int x)
        {
            return FindLowestTrueBitIndex(~x);
        } 
        #endregion
        #region Operators
        private static void Copy(int[] sourceArray, int sourceIndex, int[] destArray, int destIndex, int length, bool negate)
        {
            if (negate)
            {
                for (int index = 0; index < length; ++index)
                {
                    destArray[sourceIndex + index] = ~sourceArray[destIndex+index];
                }

                /*unsafe
                {
                    fixed (int* sptr = &sourceArray[sourceIndex], dptr = &destArray[destIndex])
                    {
                        for (int index = 0; index < length; ++index)
                        {
                            dptr[index] = ~sptr[index];
                        }
                    }
                }*/
            }
            else
            {
                Array.Copy(sourceArray, sourceIndex, destArray, destIndex, length);
            }
        }

        private static AutoResizeBitArray NullOr(AutoResizeBitArray left, AutoResizeBitArray right)
        {
            AutoResizeBitArray result = new AutoResizeBitArray();
            if (right.array == null ^ left.array == null)
            {
                if (left.array == null)
                {
                    if (left.offValue)
                    {
                        result.offValue = left.offValue;
                    }
                    else
                    {
                        result.array = (int[])right.array.Clone();
                        result.offValue = right.offValue;
                    }
                }
                else
                {
                    if (right.offValue)
                    {
                        result.offValue = right.offValue;
                    }
                    else
                    {
                        result.array = (int[])left.array.Clone();
                        result.offValue = left.offValue;
                    }
                }
            }
            else
            {
                result.offValue = left.offValue || right.offValue;
            }
            return result;
        }
        private static AutoResizeBitArray NotNullOr(AutoResizeBitArray left, AutoResizeBitArray right)
        {
            AutoResizeBitArray result = new AutoResizeBitArray();
            int minLength = left.array.Length;
            int maxLength = right.array.Length;
            if (minLength != maxLength)
            {
                Functions.Sort<int, AutoResizeBitArray>(ref minLength, ref maxLength, ref left, ref  right);
                result.array = new int[maxLength];
                Copy(right.array, minLength, result.array, minLength, maxLength - minLength, left.offValue && right.offValue);
            }
            else
            {
                result.array = new int[minLength];
            }
            if (left.offValue ^ right.offValue)
            {
                result.offValue = true;
                if (right.offValue)
                {
                    Functions.Swap<AutoResizeBitArray>(ref left, ref  right);
                }
                for (int index = 0; index < minLength; ++index)
                {
                    result.array[index] = left.array[index] & ~right.array[index];
                }
            }
            else
            {
                if (left.offValue)
                {
                    result.offValue = true;
                    for (int index = 0; index < minLength; ++index)
                    {
                        result.array[index] = left.array[index] & right.array[index];
                    }
                }
                else
                {
                    for (int index = 0; index < minLength; ++index)
                    {
                        result.array[index] = left.array[index] | right.array[index];
                    }
                }
            }
            return result;
        }
        public static AutoResizeBitArray operator |(AutoResizeBitArray left, AutoResizeBitArray right)
        {
            if (right.array == null || left.array == null)
            {
                return NullOr(left, right);
            }
            else
            {
                return NotNullOr(left, right);
            }
        }

        private static AutoResizeBitArray NullXor(AutoResizeBitArray left, AutoResizeBitArray right)
        {
            AutoResizeBitArray result = new AutoResizeBitArray();
            result.offValue = left.offValue ^ right.offValue;
            if (right.array == null ^ left.array == null)
            {
                if (left.array == null)
                {
                    result.array = (int[])right.array.Clone();
                }
                else
                {
                    result.array = (int[])left.array.Clone();
                }
            }
            return result;
        }
        private static AutoResizeBitArray NotNullXor(AutoResizeBitArray left, AutoResizeBitArray right)
        {
            AutoResizeBitArray result = new AutoResizeBitArray();
            result.offValue = left.offValue ^ right.offValue;
            int minLength = left.array.Length;
            int maxLength = right.array.Length;

            if (minLength != maxLength)
            {
                Functions.Sort<int,AutoResizeBitArray>(ref minLength, ref maxLength, ref left, ref right);
                result.array = new int[maxLength];
                Copy(right.array, minLength, result.array, minLength, maxLength - minLength, result.offValue);
            }
            else
            {
                result.array = new int[minLength];
            }
            if (result.offValue)
            {
                if (left.offValue)
                {
                    for (int index = 0; index < minLength; ++index)
                    {
                        result.array[index] = ~(~left.array[index] ^ right.array[index]);
                    }
                }
                else
                {
                    for (int index = 0; index < minLength; ++index)
                    {
                        result.array[index] = ~(left.array[index] ^ ~right.array[index]);
                    }
                }
            }
            else
            {
                if (left.offValue)
                {
                    for (int index = 0; index < minLength; ++index)
                    {
                        result.array[index] = (~left.array[index] ^ ~right.array[index]);
                    }
                }
                else
                {
                    for (int index = 0; index < minLength; ++index)
                    {
                        result.array[index] = left.array[index] ^ right.array[index];
                    }
                }
            }
            return result;
        }
        public static AutoResizeBitArray operator ^(AutoResizeBitArray left, AutoResizeBitArray right)
        {
            if (right.array == null || left.array == null)
            {
                return NullXor(left, right);
            }
            else
            {
                return NotNullXor(left, right);
            }
        }
        public static AutoResizeBitArray operator &(AutoResizeBitArray left, AutoResizeBitArray right)
        {
            //Decided to do demorgan's!
            AutoResizeBitArray result = new AutoResizeBitArray(left.array, !left.offValue) | new AutoResizeBitArray(right.array, !right.offValue);
            result.Not();
            return result;
        }
        public static AutoResizeBitArray operator ~(AutoResizeBitArray value)
        {
            AutoResizeBitArray result = new AutoResizeBitArray();
            result.offValue = value.offValue;
            if (value.array != null)
            {
                result.array = (int[])value.array.Clone();
            }
            return result;
        }
        private static bool ContainsOnBit(int[] array)
        {
            int length = array.Length;
            for (int index = 0; index < length; ++index)
            {
                if (array[index] != 0)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool ContainsOnBit(int[] array, int index, int count)
        {
            int endindex = index + count;
            for (; index < endindex; ++index)
            {
                if (array[index] != 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static explicit operator bool(AutoResizeBitArray value)
        {
            if (value.array != null && !value.offValue)
            {
                return ContainsOnBit(value.array);
            }
            return value.offValue;
        }
        public static bool AndAnd(AutoResizeBitArray left, AutoResizeBitArray right)
        {
            if (left.offValue && right.offValue)
            {
                return true;
            }
            if (right.array == null || left.array == null)
            {
                if (right.array == null ^ left.array == null)
                {
                    if (right.array == null)
                    {
                        if (right.offValue)
                        {
                            return (bool)left;
                        }
                    }
                    else
                    {
                        if (left.offValue)
                        {
                            return (bool)right;
                        }
                    }
                }
                return false;
            }
            else
            {
                int leftlength = left.array.Length;
                int rightlength = right.array.Length;
                int minLength = leftlength;
                if (leftlength != rightlength)
                {
                    if (leftlength < rightlength)
                    {
                        minLength = leftlength;
                        if (left.offValue && ContainsOnBit(right.array, minLength, rightlength - minLength))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        minLength = rightlength;
                        if (right.offValue && ContainsOnBit(left.array, minLength, leftlength - minLength))
                        {
                            return true;
                        }
                    }
                }
                if (left.offValue)
                {
                    for (int index = 0; index < minLength; ++index)
                    {
                        if ((~left.array[index] & right.array[index]) != 0)
                        {
                            return true;
                        }
                    }
                }
                else if (right.offValue)
                {
                    for (int index = 0; index < minLength; ++index)
                    {
                        if ((left.array[index] & ~right.array[index]) != 0)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < minLength; ++index)
                    {
                        if ((left.array[index] & right.array[index]) != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion
        #endregion
        #region Fields
        int[] array;
        /// <summary>
        /// The value an off bit has.
        /// </summary>
        bool offValue = false;
        #endregion
        #region Constructors
        public AutoResizeBitArray()
        { }
        public AutoResizeBitArray(bool offValue)
        {
            this.offValue = offValue;
        }
        public AutoResizeBitArray(int[] values, bool offValue)
        {
            this.array = values;
            this.offValue = offValue;
        }
        public AutoResizeBitArray(AutoResizeBitArray copy)
        {
            if (copy.array != null)
            {
                this.array = (int[])copy.array.Clone();
            }
            this.offValue = copy.offValue;
        }
        public AutoResizeBitArray(BitArray bitArray)
        {
            this.array = new int[bitArray.Count / 32];
            bitArray.CopyTo(array, 0);
            this.offValue = false;
        }
        #endregion
        #region Properties
        private int FindFirstUsedIndex
        {
            get
            {
                if (array != null)
                {
                    for (int index = array.Length - 1; index > -1; --index)
                    {
                        if (array[index] != 0)
                        {
                            return index;
                        }
                    }
                }
                return -1;
            }
        }
        private int HighestOnBitIndex
        {
            get
            {
                if (array != null)
                {
                    for (int index = array.Length - 1; index > -1; --index)
                    {
                        if (array[index] != 0)
                        {
                            return FindHighestTrueBitIndex(array[index]) + index * 32;
                        }
                    }
                }
                return -1;
            }
        }
        private int HighestOffBitIndex
        {
            get
            {
                if (array != null)
                {
                    for (int index = array.Length - 1; index > -1; --index)
                    {
                        if (array[index] != 0xffff)
                        {
                            return FindHighestFalseBitIndex(array[index]) + index * 32;
                        }
                    }
                }
                return -1;
            }
        }
        private int LowestOnBitIndex
        {
            get
            {
                if (array != null)
                {
                    int length = array.Length;
                    for (int index = 0; index < length; ++index)
                    {
                        if (array[index] != 0)
                        {
                            return FindLowestTrueBitIndex(array[index]) + index * 32;
                        }
                    }
                    return length * 32;
                }
                return -1;
            }
        }
        private int LowestOffBitIndex
        {
            get
            {
                if (array != null)
                {
                    int length = array.Length;
                    for (int index = 0; index < length; ++index)
                    {
                        if (array[index] != 0xffff)
                        {
                            return FindLowestFalseBitIndex(array[index]) + index * 32;
                        }
                    }
                    return length * 32;
                }
                return 0;
            }
        }

        public int HighestTrueBitIndex
        {
            get
            {
                if (offValue)
                {
                    return HighestOffBitIndex;
                }
                else
                {
                    return HighestOnBitIndex;
                }
            }
        }
        public int LowestFalseBitIndex
        {
            get
            {
                if (offValue)
                {
                    return LowestOnBitIndex;
                }
                else
                {
                    return LowestOffBitIndex;
                }
            }
        }
        /// <summary>
        /// Will be a multiple of 32.
        /// </summary>
        public int Capacity
        {
            get
            {
                return array.Length * 32;
            }
            set
            {
                GetIndex(value, true);
            }
        }
        public bool AllTrue
        {
            get
            {
                return this.array == null && offValue;
            }
        }
        public bool AllFalse
        {
            get
            {
                return this.array == null && !offValue;
            }
        }
        #endregion
        #region Methods
        public void SetAll(bool value)
        {
            this.array = null;
            this.offValue = value;
        }
        int GetIndex(int bitIndex, bool resize)
        {
            int index = bitIndex / 32;
            if (array == null)
            {
                if (resize)
                {
                    array = new int[index + 1];
                }
                else
                {
                    index = -1;
                }
            }
            else
            {
                if (index >= array.Length)
                {
                    if (resize)
                    {
                        Array.Resize<int>(ref array, index + 1);
                    }
                    else
                    {
                        index = -1;
                    }
                }
            }
            return index;
        }
        public bool Get(int bitIndex)
        {
            int index = GetIndex(bitIndex, false);
            if (index > -1)
            {
                return offValue ^ ((array[index] & (1 << (bitIndex % 32))) != 0);
            }
            else
            {
                return offValue;
            }
        }
        public void Set(int bitIndex, bool value)
        {
            int index = GetIndex(bitIndex, offValue ^ value);
            if (index > -1)
            {
                if (offValue ^ value)
                {
                    array[index] |= (1 << (bitIndex % 32));
                }
                else
                {
                    array[index] &= (~(1 << (bitIndex % 32)));
                }
            }
            else
            {
                if (offValue ^ value)
                {
                    throw new ArgumentOutOfRangeException("bitIndex", "The AutoResizeBitArray does not handle setting to negative indexes.");
                }
            }
        }
        public void Toggle(int bitIndex)
        {
            int index = GetIndex(bitIndex, true);
            if (index > -1)
            {
                array[index] ^= 1 << (bitIndex % 32);
            }
            else
            {
                throw new ArgumentOutOfRangeException("bitIndex", "The AutoResizeBitArray does not handle setting to negative indexes.");
            }
        }
        public void Not()
        {
            this.offValue = !this.offValue;
        }
        public void Clear()
        {
            this.array = null;
            this.offValue = false;
        }
        public void TrimExcess()
        {
            int length = FindFirstUsedIndex + 1;
            if (length == 0)
            {
                array = null;
            }
            else
            {
                Array.Resize<int>(ref this.array, length);
            }
        }
        public object Clone()
        {
            return new AutoResizeBitArray(this);
        }
        public int[] ToArray()
        {
            int[] result = null;
            if (this.array != null)
            {
                int length = this.array.Length;
                result = new int[length];
                Copy(array, 0, result, 0, length, offValue);
            }
            return result;
        }
        public string ToBinaryString()
        {
            return ToBaseString(2);
        }
        public string ToBaseString(int toBase)
        {
            if (array != null)
            {
                int length = array.Length;
                int size = 64 / toBase;
                StringBuilder builder = new StringBuilder(length * 64 / size);
                if (offValue)
                {
                    for (int pos = length - 1; pos > -1; --pos)
                    {
                        builder.Append(Convert.ToString(~array[pos], toBase).PadLeft(size, '0'));
                    }
                }
                else
                {
                    for (int pos = length - 1; pos > -1; --pos)
                    {
                        builder.Append(Convert.ToString(array[pos], toBase).PadLeft(size, '0'));
                    }
                }
                return builder.ToString();
            }
            else
            {
                if (offValue)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
        }
        IEnumerator<bool> PrivateGetEnumerator()
        {
            for (int index = array.Length - 1; index > -1; --index)
            {
                int value = array[index];
                for (int bitindex = 0; bitindex < 32; ++index)
                {
                    yield return ((value & (1 << bitindex)) != 0) ^ offValue;
                }
            }
            yield break;
        }
        public IEnumerator<bool> GetEnumerator()
        {
            return PrivateGetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return PrivateGetEnumerator();
        }
        #endregion
        #region Indexors
        public bool this[int bitIndex]
        {
            get
            {
                return Get(bitIndex);
            }
            set
            {
                Set(bitIndex, value);
            }
        }
        #endregion
    }
}