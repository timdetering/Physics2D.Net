// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Design.ParseMethodAttribute
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;
using System.Reflection;

namespace AdvanceMath.Design
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class ParseMethodAttribute : Attribute
  {
    public static MethodInfo GetParseMethod(Type t)
    {
      foreach (MethodInfo method in t.GetMethods())
      {
        if (method.IsStatic && method.GetCustomAttributes(typeof (ParseMethodAttribute), true).Length > 0 && (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof (string)) && method.ReturnType == t)
          return method;
      }
      return (MethodInfo) null;
    }
  }
}
