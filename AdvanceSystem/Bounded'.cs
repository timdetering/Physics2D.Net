#region LGPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
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
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace AdvanceSystem
{
    [Serializable, StructLayout(LayoutKind.Sequential)]
    [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
    public sealed class Bounded<T> : IXmlSerializable
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        [ComponentModel.UTCParser]
        public static Bounded<T> Parse(string text)
        {
            string[] vals = text.Split(new string[] { "(", "<=", ")" }, StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length != 4)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 4 parts.", text));
            }
            else
            {
                try
                {
                    Type t = typeof(T);
                    return new Bounded<T>(
                        (T)Convert.ChangeType(vals[0].Trim(), t),
                        (T)Convert.ChangeType(vals[1].Trim(), t),
                        (T)Convert.ChangeType(vals[2].Trim(), t),
                        Convert.ToBoolean(vals[3].Trim()));
                }
                catch (Exception ex)
                {
                    throw new FormatException("The parts of the vectors must be decimal numbers", ex);
                }
            }
        }
        private T current;
        private NumberBinder<T> binder;
        public Bounded() { }
        [ComponentModel.UTCConstructor]
        public Bounded(T current, NumberBinder<T> binder)
        {
            this.current = current;
            this.Binder = binder;
        }
        public Bounded(T lower, T current, T upper, Boolean Wrap)
            : this(current, NumberBinder<T>.GetBinder(lower, upper, Wrap))
        { }
        public Bounded(T upper)
            : this(upper, NumberBinder<T>.GetBinder(upper))
        { }
        public Bounded(Bounded<T> copy)
        {
            this.current = copy.current;
            this.binder = copy.binder;
        }
        [XmlIgnore]
        [ComponentModel.UTCConstructorParameter("current")]
        public T Value
        {
            get
            {
                return current;
            }
            set
            {
                current = binder.Bind(value);
            }
        }
        [XmlIgnore]
        [ComponentModel.UTCConstructorParameter("binder")]
        public NumberBinder<T> Binder
        {
            get
            {
                return binder;
            }
            set
            {
                binder = value;
                if (binder == null)
                {
                    throw new NullReferenceException("Binder cannot be null");
                }
                current = binder.Bind(current);
            }
        }
        [Browsable(false)]
        public T EmptyValue
        {
            get
            {
                return binder.GetEmptyValue(current);
            }
        }
        [XmlIgnore, Browsable(false)]
        public double Percent
        {
            get
            {
                return binder.GetPercent(current);
            }
            set
            {
                current = binder.SetPercent(value);
            }
        }
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return current.CompareTo(binder.lower) == 0;
            }
        }
        [Browsable(false)]
        public bool IsFull
        {
            get
            {
                return current.CompareTo(binder.upper) == 0;
            }
        }
        public bool HasRoomFor(T value)
        {
            return binder.CanAddUnBounded(current, value);
        }
        public T ChangeValue(T dv)
        {
            return binder.GetOverflow(ref current, dv);
        }
        public void Fill()
        {
            current = binder.upper;
        }
        public void Empty()
        {
            current = binder.lower;
        }
        public static implicit operator T(Bounded<T> value)
        {
            return value.current;
        }
        [ComponentModel.UTCFormater]
        public override string ToString()
        {
            return string.Format("( {0} <= {1} <= {2} ) {3}", binder.lower, current, binder.upper, binder.wrap);
        }
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(System.Xml.XmlReader reader)
        {
            Bounded<T> self = Parse(reader.ReadElementContentAsString().Replace("LT", "<="));
            this.binder = self.binder;
            this.current = self.current;
        }
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteValue(this.ToString().Replace("<=", "LT"));
        }
        #endregion
    }
    [Serializable, StructLayout(LayoutKind.Sequential)]
    [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
    public abstract class NumberBinder<T> : ICloneable
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        [ComponentModel.UTCParser]
        public static NumberBinder<T> Parse(string text)
        {
            string[] vals = text.Split(new string[] { "(", "<=", ")" }, StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length != 3)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 3 parts.", text));
            }
            else
            {
                try
                {
                    Type t = typeof(T);
                    return GetBinder(
                        (T)Convert.ChangeType(vals[0].Trim(), t),
                        (T)Convert.ChangeType(vals[1].Trim(), t),
                        Convert.ToBoolean(vals[2].Trim()));
                }
                catch (Exception ex)
                {
                    throw new FormatException("The parts of the vectors must be decimal numbers", ex);
                }
            }
        }
        
        public static NumberBinder<T> GetBinder(T lower, T upper, Boolean wrap)
        {
            switch (upper.GetType().Name)
            {
                case "Byte":
                    return (NumberBinder<T>)(object)new ByteBinder(Convert.ToByte(lower), Convert.ToByte(upper), wrap);
                case "UInt16":
                    return (NumberBinder<T>)(object)new UInt16Binder(Convert.ToUInt16(lower), Convert.ToUInt16(upper), wrap);
                case "UInt32":
                    return (NumberBinder<T>)(object)new UInt32Binder(Convert.ToUInt32(lower), Convert.ToUInt32(upper), wrap);
                case "UInt64":
                    return (NumberBinder<T>)(object)new UInt64Binder(Convert.ToUInt64(lower), Convert.ToUInt64(upper), wrap);
                case "SByte":
                    return (NumberBinder<T>)(object)new SByteBinder(Convert.ToSByte(lower), Convert.ToSByte(upper), wrap);
                case "Int16":
                    return (NumberBinder<T>)(object)new Int16Binder(Convert.ToInt16(lower), Convert.ToInt16(upper), wrap);
                case "Int32":
                    return (NumberBinder<T>)(object)new Int32Binder(Convert.ToInt32(lower), Convert.ToInt32(upper), wrap);
                case "Int64":
                    return (NumberBinder<T>)(object)new Int64Binder(Convert.ToInt64(lower), Convert.ToInt64(upper), wrap);
                case "Single":
                    return (NumberBinder<T>)(object)new SingleBinder(Convert.ToSingle(lower), Convert.ToSingle(upper), wrap);
                case "Double":
                    return (NumberBinder<T>)(object)new DoubleBinder(Convert.ToDouble(lower), Convert.ToDouble(upper), wrap);
                case "Decimal":
                    return (NumberBinder<T>)(object)new DecimalBinder(Convert.ToDecimal(lower), Convert.ToDecimal(upper), wrap);
                default:
                    return null;
            }
        }
        public static NumberBinder<T> GetBinder(T upper, Boolean wrap)
        {
            return GetBinder(new T(), upper, wrap);
        }
        public static NumberBinder<T> GetBinder(T upper)
        {
            return GetBinder(new T(), upper, false);
        }
        protected internal T upper;
        protected internal T lower;
        protected T range;
        protected internal Boolean wrap;

        [ComponentModel.UTCConstructor]
        protected NumberBinder(T lower, T upper, Boolean wrap)
        {
            this.upper = upper;
            this.lower = lower;
            this.wrap = wrap;
        }
        protected NumberBinder(NumberBinder<T> copy)
        {
            this.upper = copy.upper;
            this.lower = copy.lower;
            this.range = copy.range;
            this.wrap = copy.wrap;
        }
        [ComponentModel.UTCConstructorParameter("lower")]
        public T Lower
        {
            get
            {
                return lower;
            }
            set
            {
                lower = value;
            }
        }
        [ComponentModel.UTCConstructorParameter("upper")]
        public T Upper
        {
            get
            {
                return upper;
            }
            set
            {
                upper = value;
            }
        }
        [ComponentModel.UTCConstructorParameter("wrap")]
        public Boolean Wrap
        {
            get
            {
                return wrap;
            }
            set
            {
                wrap = value;
            }
        }
        public abstract T Bind(T value);
        public abstract double GetPercent(T value);
        public abstract T SetPercent(double percent);
        public abstract bool CanAddUnBounded(T left, T right);
        public abstract T GetOverflow(ref T value, T dv);
        public T GetOverflow(T value, T dv)
        {
            return GetOverflow(ref value, dv);
        }
        public abstract T GetEmptyValue(T value);
        [ComponentModel.UTCFormater]
        public override string ToString()
        {
            return string.Format("( {0} <= {1} ) {2}", lower, upper, wrap);
        }
        public abstract object Clone();
        #region sub-classes
        #region unsigned
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class ByteBinder : NumberBinder<Byte>
        {
            [ComponentModel.UTCConstructor]
            public ByteBinder(Byte lower, Byte upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = (Byte)((upper - lower) + 1);
            }
            public ByteBinder(NumberBinder<Byte> copy) : base(copy) { }
            public override object Clone()
            {
                return new ByteBinder(this);
            }
            public override Byte Bind(Byte value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(Byte value)
            {
                return (value - lower) / (range - 1);
            }
            public override Byte SetPercent(double percent)
            {
                return (Byte)(((double)range - 1) * (double)percent + (double)lower);
            }
            public override bool CanAddUnBounded(Byte value, Byte value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override Byte GetOverflow(ref Byte current, Byte dv)
            {
                current += dv;
                Byte boundedCurrent = current;
                Byte leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = (Byte)(current - boundedCurrent);
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = (Byte)(current - boundedCurrent);
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = (Byte)(current - boundedCurrent);
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override Byte GetEmptyValue(Byte value)
            {
                return (Byte)(lower - value);
            }


        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class UInt16Binder : NumberBinder<UInt16>
        {
            [ComponentModel.UTCConstructor]
            public UInt16Binder(UInt16 lower, UInt16 upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = (UInt16)((upper - lower) + 1);
            }
            public UInt16Binder(NumberBinder<UInt16> copy) : base(copy) { }
            public override object Clone()
            {
                return new UInt16Binder(this);
            }
            public override UInt16 Bind(UInt16 value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(UInt16 value)
            {
                return (value - lower) / (range - 1);
            }
            public override UInt16 SetPercent(double percent)
            {
                return (UInt16)(((double)range - 1) * (double)percent + (double)lower);
            }
            public override bool CanAddUnBounded(UInt16 value, UInt16 value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override UInt16 GetOverflow(ref UInt16 current, UInt16 dv)
            {
                current += dv;
                UInt16 boundedCurrent = current;
                UInt16 leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = (UInt16)(current - boundedCurrent);
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = (UInt16)(current - boundedCurrent);
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = (UInt16)(current - boundedCurrent);
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override UInt16 GetEmptyValue(UInt16 value)
            {
                return (UInt16)(lower - value);
            }
        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class UInt32Binder : NumberBinder<UInt32>
        {
            [ComponentModel.UTCConstructor]
            public UInt32Binder(UInt32 lower, UInt32 upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = upper - lower + 1;
            }
            public UInt32Binder(NumberBinder<UInt32> copy) : base(copy) { }
            public override object Clone()
            {
                return new UInt32Binder(this);
            }
            public override UInt32 Bind(UInt32 value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(UInt32 value)
            {
                return (value - lower) / (range - 1);
            }
            public override UInt32 SetPercent(double percent)
            {
                return (UInt32)(((double)range - 1) * (double)percent + (double)lower);
            }
            public override bool CanAddUnBounded(UInt32 value, UInt32 value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override UInt32 GetOverflow(ref UInt32 current, UInt32 dv)
            {
                current += dv;
                UInt32 boundedCurrent = current;
                UInt32 leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = current - boundedCurrent;
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = current - boundedCurrent;
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = current - boundedCurrent;
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override UInt32 GetEmptyValue(UInt32 value)
            {
                return lower - value;
            }
        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class UInt64Binder : NumberBinder<UInt64>
        {
            [ComponentModel.UTCConstructor]
            public UInt64Binder(UInt64 lower, UInt64 upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = upper - lower + 1;
            }
            public UInt64Binder(NumberBinder<UInt64> copy) : base(copy) { }
            public override object Clone()
            {
                return new UInt64Binder(this);
            }
            public override UInt64 Bind(UInt64 value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(UInt64 value)
            {
                return (value - lower) / (range - 1);
            }
            public override UInt64 SetPercent(double percent)
            {
                return (UInt64)(((double)range - 1) * (double)percent + (double)lower);
            }
            public override bool CanAddUnBounded(UInt64 value, UInt64 value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override UInt64 GetOverflow(ref UInt64 current, UInt64 dv)
            {
                current += dv;
                UInt64 boundedCurrent = current;
                UInt64 leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = current - boundedCurrent;
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = current - boundedCurrent;
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = current - boundedCurrent;
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override UInt64 GetEmptyValue(UInt64 value)
            {
                return lower - value;
            }
        }
        #endregion
        #region signed
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class SByteBinder : NumberBinder<SByte>
        {
            [ComponentModel.UTCConstructor]
            public SByteBinder(SByte lower, SByte upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = (SByte)((upper - lower) + 1);
            }
            public SByteBinder(NumberBinder<SByte> copy) : base(copy) { }
            public override object Clone()
            {
                return new SByteBinder(this);
            }
            public override SByte Bind(SByte value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(SByte value)
            {
                return (value - lower) / (range - 1);
            }
            public override SByte SetPercent(double percent)
            {
                return (SByte)(((double)range - 1) * (double)percent + (double)lower);
            }
            public override bool CanAddUnBounded(SByte value, SByte value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override SByte GetOverflow(ref SByte current, SByte dv)
            {
                current += dv;
                SByte boundedCurrent = current;
                SByte leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = (SByte)(current - boundedCurrent);
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = (SByte)(current - boundedCurrent);
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = (SByte)(current - boundedCurrent);
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override SByte GetEmptyValue(SByte value)
            {
                return (SByte)(lower - value);
            }
        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class Int16Binder : NumberBinder<Int16>
        {
            [ComponentModel.UTCConstructor]
            public Int16Binder(Int16 lower, Int16 upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = (Int16)((upper - lower) + 1);
            }
            public Int16Binder(NumberBinder<Int16> copy) : base(copy) { }
            public override object Clone()
            {
                return new Int16Binder(this);
            }
            public override Int16 Bind(Int16 value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(Int16 value)
            {
                return (value - lower) / (range - 1);
            }
            public override Int16 SetPercent(double percent)
            {
                return (Int16)(((double)range - 1) * (double)percent + (double)lower);
            }
            public override bool CanAddUnBounded(Int16 value, Int16 value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override Int16 GetOverflow(ref Int16 current, Int16 dv)
            {
                current += dv;
                Int16 boundedCurrent = current;
                Int16 leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = (Int16)(current - boundedCurrent);
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = (Int16)(current - boundedCurrent);
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = (Int16)(current - boundedCurrent);
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override Int16 GetEmptyValue(Int16 value)
            {
                return (Int16)(lower - value);
            }
        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class Int32Binder : NumberBinder<Int32>
        {
            [ComponentModel.UTCConstructor]
            public Int32Binder(Int32 lower, Int32 upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = upper - lower + 1;
            }
            public Int32Binder(NumberBinder<Int32> copy) : base(copy) { }
            public override object Clone()
            {
                return new Int32Binder(this);
            }
            public override Int32 Bind(Int32 value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(Int32 value)
            {
                return (value - lower) / (range - 1);
            }
            public override Int32 SetPercent(double percent)
            {
                return (Int32)(((double)range - 1) * (double)percent + (double)lower);
            }
            public override bool CanAddUnBounded(Int32 value, Int32 value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override Int32 GetOverflow(ref Int32 current, Int32 dv)
            {
                current += dv;
                Int32 boundedCurrent = current;
                Int32 leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = current - boundedCurrent;
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = current - boundedCurrent;
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = current - boundedCurrent;
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override Int32 GetEmptyValue(Int32 value)
            {
                return lower - value;
            }
        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class Int64Binder : NumberBinder<Int64>
        {
            [ComponentModel.UTCConstructor]
            public Int64Binder(Int64 lower, Int64 upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = upper - lower + 1;
            }
            public Int64Binder(NumberBinder<Int64> copy) : base(copy) { }
            public override object Clone()
            {
                return new Int64Binder(this);
            }
            public override Int64 Bind(Int64 value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(Int64 value)
            {
                return (value - lower) / (range - 1);
            }
            public override Int64 SetPercent(double percent)
            {
                return (Int64)(((double)range - 1) * (double)percent + (double)lower);
            }
            public override bool CanAddUnBounded(Int64 value, Int64 value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override Int64 GetOverflow(ref Int64 current, Int64 dv)
            {
                current += dv;
                Int64 boundedCurrent = current;
                Int64 leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = current - boundedCurrent;
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = current - boundedCurrent;
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = current - boundedCurrent;
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override Int64 GetEmptyValue(Int64 value)
            {
                return lower - value;
            }
        }
        #endregion
        #region floating point
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class SingleBinder : NumberBinder<Single>
        {
            [ComponentModel.UTCConstructor]
            public SingleBinder(Single lower, Single upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = upper - lower + 1;
            }
            public SingleBinder(NumberBinder<Single> copy) : base(copy) { }
            public override object Clone()
            {
                return new SingleBinder(this);
            }
            public override Single Bind(Single value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(Single value)
            {
                return (value - lower) / (range - 1);
            }
            public override Single SetPercent(double percent)
            {
                return (Single)((range - 1) * percent + lower);
            }
            public override bool CanAddUnBounded(Single value, Single value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override Single GetOverflow(ref Single current, Single dv)
            {
                current += dv;
                Single boundedCurrent = current;
                Single leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = current - boundedCurrent;
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = current - boundedCurrent;
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = current - boundedCurrent;
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override Single GetEmptyValue(Single value)
            {
                return lower - value;
            }

        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class DoubleBinder : NumberBinder<Double>
        {
            [ComponentModel.UTCConstructor]
            public DoubleBinder(Double lower, Double upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = upper - lower + 1;
            }
            public DoubleBinder(NumberBinder<Double> copy) : base(copy) { }
            public override object Clone()
            {
                return new DoubleBinder(this);
            }
            public override Double Bind(Double value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(Double value)
            {
                return (value - lower) / (range - 1);
            }
            public override Double SetPercent(double percent)
            {
                return ((range - 1) * percent + lower);
            }
            public override bool CanAddUnBounded(Double value, Double value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override Double GetOverflow(ref Double current, Double dv)
            {
                current += dv;
                Double boundedCurrent = current;
                Double leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = current - boundedCurrent;
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = current - boundedCurrent;
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = current - boundedCurrent;
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override Double GetEmptyValue(Double value)
            {
                return lower - value;
            }
        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        [System.ComponentModel.TypeConverter(typeof(ComponentModel.UniversalTypeConvertor)), ComponentModel.UTCPropertiesSupported]
        sealed class DecimalBinder : NumberBinder<Decimal>
        {
            [ComponentModel.UTCConstructor]
            public DecimalBinder(Decimal lower, Decimal upper, Boolean wrap)
                : base(lower, upper, wrap)
            {
                this.range = upper - lower + 1;
            }
            public DecimalBinder(NumberBinder<Decimal> copy) : base(copy) { }
            public override object Clone()
            {
                return new DecimalBinder(this);
            }
            public override Decimal Bind(Decimal value)
            {
                if (wrap)
                {
                    while (lower > value)
                    {
                        value += range;
                    }
                    while (upper < value)
                    {
                        value -= range;
                    }
                }
                else
                {
                    if (lower > value)
                    {
                        value = lower;
                    }
                    else if (upper < value)
                    {
                        value = upper;
                    }
                }
                return value;
            }
            public override double GetPercent(Decimal value)
            {
                return (double)((value - lower) / (range - 1));
            }
            public override Decimal SetPercent(double percent)
            {
                return (Decimal)((range - 1) * (Decimal)percent + lower);
            }
            public override bool CanAddUnBounded(Decimal value, Decimal value2)
            {
                value += value2;
                return value >= lower && value <= upper;
            }
            public override Decimal GetOverflow(ref Decimal current, Decimal dv)
            {
                current += dv;
                Decimal boundedCurrent = current;
                Decimal leftover = 0;
                if (wrap)
                {
                    while (lower > boundedCurrent)
                    {
                        boundedCurrent += range;
                    }
                    while (upper < boundedCurrent)
                    {
                        boundedCurrent -= range;
                    }
                    leftover = current - boundedCurrent;
                }
                else
                {
                    if (lower > boundedCurrent)
                    {
                        boundedCurrent = lower;
                        leftover = current - boundedCurrent;
                    }
                    else if (upper < boundedCurrent)
                    {
                        boundedCurrent = upper;
                        leftover = current - boundedCurrent;
                    }
                }
                current = boundedCurrent;
                return leftover;
            }
            public override Decimal GetEmptyValue(Decimal value)
            {
                return lower - value;
            }
        }
        #endregion
        #endregion


    }
}
