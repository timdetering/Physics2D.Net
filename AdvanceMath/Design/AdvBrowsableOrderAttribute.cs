// Decompiled with JetBrains decompiler
// Type: AdvanceMath.Design.AdvBrowsableOrderAttribute
// Assembly: AdvanceMath, Version=1.2.2578.26001, Culture=neutral, PublicKeyToken=null
// MVID: 17DABC3B-E4D5-4350-A4D6-6E3CBC05820C
// Assembly location: C:\Users\TimD\Desktop\ReMasters-0.1.10-binary\bin\AdvanceMath.dll

using System;

namespace AdvanceMath.Design
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
  public sealed class AdvBrowsableOrderAttribute : Attribute
  {
    private string[] order;

    public AdvBrowsableOrderAttribute(string order)
    {
      this.order = order.Split(',');
    }

    public string[] Order
    {
      get
      {
        return this.order;
      }
    }

    public static string[] GetOrder(Type t)
    {
      object[] customAttributes = t.GetCustomAttributes(typeof (AdvBrowsableOrderAttribute), false);
      return customAttributes.Length > 0 ? ((AdvBrowsableOrderAttribute) customAttributes[0]).Order : (string[]) null;
    }
  }
}
