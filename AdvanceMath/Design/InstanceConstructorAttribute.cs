// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Design.InstanceConstructorAttribute
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.Reflection;

namespace AdvanceMath.Design
{
  [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
  public sealed class InstanceConstructorAttribute : Attribute
  {
    private string[] parameterNames;

    public InstanceConstructorAttribute(string parameterNames)
    {
      this.parameterNames = parameterNames.Split(',');
    }

    public string[] ParameterNames
    {
      get
      {
        return this.parameterNames;
      }
    }

    public static ConstructorInfo GetConstructor(Type t, out string[] paramNames)
    {
      foreach (ConstructorInfo constructor in t.GetConstructors())
      {
        object[] customAttributes = constructor.GetCustomAttributes(typeof (InstanceConstructorAttribute), true);
        if (customAttributes.Length > 0)
        {
          InstanceConstructorAttribute constructorAttribute = (InstanceConstructorAttribute) customAttributes[0];
          if (constructor.GetParameters().Length == constructorAttribute.ParameterNames.Length)
          {
            paramNames = constructorAttribute.ParameterNames;
            return constructor;
          }
        }
      }
      paramNames = (string[]) null;
      return (ConstructorInfo) null;
    }
  }
}
