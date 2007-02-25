#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion



using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace AdvanceMath.Design
{
    public class AdvPropertyDescriptor : PropertyDescriptor, IEquatable<AdvPropertyDescriptor>
    {
        MemberInfo info;
        FieldInfo field;
        PropertyInfo property;
        string description;
        public AdvPropertyDescriptor(FieldInfo field)
            : this(field.Name, field)
        { }
        public AdvPropertyDescriptor(string name, FieldInfo field)
            : base(name, (Attribute[])field.GetCustomAttributes(typeof(Attribute), true))
        {
            this.info = field;
            this.field = field;
            this.description = base.Description;
        }
        public AdvPropertyDescriptor(PropertyInfo property)
            : this(property.Name, property)
        { }
        public AdvPropertyDescriptor(string name, PropertyInfo property)
            : base(name, (Attribute[])property.GetCustomAttributes(typeof(Attribute), true))
        {
            this.info = property;
            this.property = property;
            this.property = property;
            this.description = base.Description;
        }
        public override Type ComponentType
        {
            get { return info.DeclaringType; }
        }
        public override object GetValue(object component)
        {
            if (field == null)
            {
                return property.GetValue(component, null);
            }
            return field.GetValue(component);
        }
        public override bool IsReadOnly
        {
            get { return !(property == null || !property.CanWrite); }
        }
        public override Type PropertyType
        {
            get
            {
                if (field == null)
                {
                    return property.PropertyType;
                }
                return field.FieldType;
            }
        }
        public override bool CanResetValue(object component) { return false; }
        public override void ResetValue(object component)
        {
            throw new NotSupportedException();
        }
        public override void SetValue(object component, object value)
        {
            if (field == null)
            {
                property.SetValue(component, value, null);
            }
            else
            {
                field.SetValue(component, value);
            }
            this.OnValueChanged(component, EventArgs.Empty);
        }
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return info.GetHashCode();
        }
        public override string Description
        {
            get
            {
                return description;
            }
        }
        public void SetDescription(string value)
        {
            description = value;
        }
        public override bool Equals(object obj)
        {
            return obj is AdvPropertyDescriptor && Equals((AdvPropertyDescriptor)obj);
        }
        public bool Equals(AdvPropertyDescriptor other)
        {
            return info.Equals(other.info);
        }
    }
}
