// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Design.AdvTypeConverter`1
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace AdvanceMath.Design
{
  public class AdvTypeConverter<TType> : ExpandableObjectConverter
  {
    private ConstructorInfo instanceCtor;
    private MethodInfo parse;
    private PropertyDescriptorCollection descriptions;
    private string[] instanceCtorParamNames;

    public AdvTypeConverter()
    {
      Type t = typeof (TType);
      this.parse = ParseMethodAttribute.GetParseMethod(t);
      this.descriptions = AdvBrowsableAttribute.GetDispMembers(t);
      if (this.descriptions != null)
      {
        this.instanceCtor = InstanceConstructorAttribute.GetConstructor(t, out this.instanceCtorParamNames);
        if (this.instanceCtor != null)
        {
          ParameterInfo[] parameters = this.instanceCtor.GetParameters();
          if (parameters.Length == this.instanceCtorParamNames.Length)
          {
            for (int index = 0; index < this.instanceCtorParamNames.Length; ++index)
            {
              PropertyDescriptor propertyDescriptor = this.descriptions.Find(this.instanceCtorParamNames[index], false);
              if (propertyDescriptor == null || propertyDescriptor.PropertyType != parameters[index].ParameterType)
              {
                this.instanceCtor = (ConstructorInfo) null;
                break;
              }
            }
          }
          else
            this.instanceCtor = (ConstructorInfo) null;
        }
      }
      Console.WriteLine();
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return this.parse != null && sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return this.instanceCtor != null && destinationType == typeof (InstanceDescriptor) || base.CanConvertTo(context, destinationType);
    }

    public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
    {
      return this.instanceCtor != null || base.GetCreateInstanceSupported(context);
    }

    public override bool GetPropertiesSupported(ITypeDescriptorContext context)
    {
      return this.descriptions != null || base.GetPropertiesSupported(context);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      if (this.parse != null)
      {
        if (value is string)
        {
          try
          {
            return this.parse.Invoke((object) null, new object[1]
            {
              value
            });
          }
          catch (TargetInvocationException ex)
          {
            throw ex.InnerException;
          }
        }
      }
      return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return this.instanceCtor != null && value is TType && destinationType == typeof (InstanceDescriptor) ? (object) new InstanceDescriptor((MemberInfo) this.instanceCtor, (ICollection) this.GetInstanceDescriptorObjects(value)) : base.ConvertTo(context, culture, value, destinationType);
    }

    public override object CreateInstance(
      ITypeDescriptorContext context,
      IDictionary propertyValues)
    {
      if (this.instanceCtor == null)
        return base.CreateInstance(context, propertyValues);
      try
      {
        return this.instanceCtor.Invoke(this.GetInstanceDescriptorObjects(propertyValues));
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    public override PropertyDescriptorCollection GetProperties(
      ITypeDescriptorContext context,
      object value,
      Attribute[] attributes)
    {
      return this.descriptions != null ? this.descriptions : base.GetProperties(context, value, attributes);
    }

    private object[] GetInstanceDescriptorObjects(IDictionary propertyValues)
    {
      object[] objArray = new object[this.instanceCtorParamNames.Length];
      for (int index = 0; index < this.instanceCtorParamNames.Length; ++index)
        objArray[index] = propertyValues[(object) this.instanceCtorParamNames[index]];
      return objArray;
    }

    private object[] GetInstanceDescriptorObjects(object value)
    {
      object[] objArray = new object[this.instanceCtorParamNames.Length];
      for (int index = 0; index < this.instanceCtorParamNames.Length; ++index)
      {
        PropertyDescriptor propertyDescriptor = this.descriptions.Find(this.instanceCtorParamNames[index], false);
        objArray[index] = propertyDescriptor.GetValue(value);
      }
      return objArray;
    }
  }
}
