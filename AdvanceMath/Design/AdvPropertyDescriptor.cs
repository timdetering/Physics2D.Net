// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Design.AdvPropertyDescriptor
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.ComponentModel;
using System.Reflection;

namespace AdvanceMath.Design
{
  public class AdvPropertyDescriptor : PropertyDescriptor, IEquatable<AdvPropertyDescriptor>
  {
    private MemberInfo info;
    private FieldInfo field;
    private PropertyInfo property;
    private string description;

    public AdvPropertyDescriptor(FieldInfo field)
      : this(field.Name, field)
    {
    }

    public AdvPropertyDescriptor(string name, FieldInfo field)
      : base(name, (Attribute[]) field.GetCustomAttributes(typeof (Attribute), true))
    {
      this.info = (MemberInfo) field;
      this.field = field;
      this.description = base.Description;
    }

    public AdvPropertyDescriptor(PropertyInfo property)
      : this(property.Name, property)
    {
    }

    public AdvPropertyDescriptor(string name, PropertyInfo property)
      : base(name, (Attribute[]) property.GetCustomAttributes(typeof (Attribute), true))
    {
      this.info = (MemberInfo) property;
      this.property = property;
      this.property = property;
      this.description = base.Description;
    }

    public override Type ComponentType
    {
      get
      {
        return this.info.DeclaringType;
      }
    }

    public override object GetValue(object component)
    {
      return this.field == null ? this.property.GetValue(component, (object[]) null) : this.field.GetValue(component);
    }

    public override bool IsReadOnly
    {
      get
      {
        return this.property != null && this.property.CanWrite;
      }
    }

    public override Type PropertyType
    {
      get
      {
        return this.field == null ? this.property.PropertyType : this.field.FieldType;
      }
    }

    public override bool CanResetValue(object component)
    {
      return false;
    }

    public override void ResetValue(object component)
    {
      throw new NotSupportedException();
    }

    public override void SetValue(object component, object value)
    {
      if (this.field == null)
        this.property.SetValue(component, value, (object[]) null);
      else
        this.field.SetValue(component, value);
      this.OnValueChanged(component, EventArgs.Empty);
    }

    public override bool ShouldSerializeValue(object component)
    {
      return true;
    }

    public override int GetHashCode()
    {
      return this.info.GetHashCode();
    }

    public override string Description
    {
      get
      {
        return this.description;
      }
    }

    public void SetDescription(string value)
    {
      this.description = value;
    }

    public override bool Equals(object obj)
    {
      return obj is AdvPropertyDescriptor && this.Equals((AdvPropertyDescriptor) obj);
    }

    public bool Equals(AdvPropertyDescriptor other)
    {
      return this.info.Equals((object) other.info);
    }
  }
}
