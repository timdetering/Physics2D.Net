// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Design.AdvBrowsableAttribute
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace AdvanceMath.Design
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class AdvBrowsableAttribute : Attribute
  {
    private string name;

    public AdvBrowsableAttribute()
      : this((string) null)
    {
    }

    public AdvBrowsableAttribute(string name)
    {
      this.name = name;
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public static PropertyDescriptorCollection GetDispMembers(Type t)
    {
      string[] order = AdvBrowsableOrderAttribute.GetOrder(t);
      List<PropertyDescriptor> propertyDescriptorList = new List<PropertyDescriptor>();
      foreach (PropertyInfo property in t.GetProperties())
      {
        object[] customAttributes1 = property.GetCustomAttributes(typeof (AdvBrowsableAttribute), true);
        if (customAttributes1.Length > 0)
        {
          AdvBrowsableAttribute browsableAttribute = (AdvBrowsableAttribute) customAttributes1[0];
          AdvPropertyDescriptor propertyDescriptor = browsableAttribute.Name == null ? new AdvPropertyDescriptor(property) : new AdvPropertyDescriptor(browsableAttribute.Name, property);
          object[] customAttributes2 = property.GetCustomAttributes(typeof (DescriptionAttribute), true);
          if (customAttributes2.Length > 0)
          {
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute) customAttributes2[0];
            propertyDescriptor.SetDescription(descriptionAttribute.Description);
          }
          propertyDescriptorList.Add((PropertyDescriptor) propertyDescriptor);
        }
      }
      foreach (FieldInfo field in t.GetFields())
      {
        object[] customAttributes1 = field.GetCustomAttributes(typeof (AdvBrowsableAttribute), true);
        if (customAttributes1.Length > 0)
        {
          AdvBrowsableAttribute browsableAttribute = (AdvBrowsableAttribute) customAttributes1[0];
          AdvPropertyDescriptor propertyDescriptor = browsableAttribute.Name == null ? new AdvPropertyDescriptor(field) : new AdvPropertyDescriptor(browsableAttribute.Name, field);
          object[] customAttributes2 = field.GetCustomAttributes(typeof (DescriptionAttribute), true);
          if (customAttributes2.Length > 0)
          {
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute) customAttributes2[0];
            propertyDescriptor.SetDescription(descriptionAttribute.Description);
          }
          propertyDescriptorList.Add((PropertyDescriptor) propertyDescriptor);
        }
      }
      if (propertyDescriptorList.Count == 0)
        return (PropertyDescriptorCollection) null;
      return order != null ? new PropertyDescriptorCollection(propertyDescriptorList.ToArray()).Sort(order) : new PropertyDescriptorCollection(propertyDescriptorList.ToArray());
    }
  }
}
