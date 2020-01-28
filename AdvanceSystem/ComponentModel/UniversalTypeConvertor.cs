using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
namespace AdvanceSystem.ComponentModel
{
    public class UniversalTypeConvertor : TypeConverter
    {
        public UniversalTypeConvertor()
        {
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return CheckContext(context) && UTCParserAttribute.CanParse(context.PropertyDescriptor.PropertyType);
            }
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (CheckContext(context))
            {
                MethodInfo minfo = UTCParserAttribute.GetParser(context.PropertyDescriptor.PropertyType);
                if (minfo != null)
                {
                    return minfo.Invoke(null, new object[] { value });
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (CheckContext(context))
            {
                if (destinationType == typeof(string))
                {
                    return UTCFormaterAttribute.CanFormat(context.PropertyDescriptor.PropertyType);
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    return CheckContext(context);
                }
            }
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            Type t = value.GetType();
            if (CheckContext(context) && context.PropertyDescriptor.PropertyType == t)
            {
                if (destinationType == typeof(string))
                {
                    MethodInfo minfo = UTCFormaterAttribute.GetFormater(context.PropertyDescriptor.PropertyType);
                    if (minfo != null)
                    {
                        if (minfo.IsStatic)
                        {
                            
                                return minfo.Invoke(null, new object[] { value });
                            
                        }
                        else
                        {
                            
                                return minfo.Invoke(value, null);
                            
                        }
                    }
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    ConstructorInfo cinfo = UTCConstructorAttribute.GetConstructor(t);
                    if (cinfo != null)
                    {
                        return new InstanceDescriptor(cinfo, GetConstructorParameters(cinfo, value));
                    }
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues");
            }
            Type t = context.PropertyDescriptor.PropertyType;
            ConstructorInfo cinfo = UTCConstructorAttribute.GetConstructor(t);
            if (cinfo != null)
            {
                object[] pinfos = GetConstructorPropertyInfos(cinfo, t);
                object[] parameters = new object[pinfos.Length];
                for (int pos = 0; pos < pinfos.Length; pos++)
                {
                    if (pinfos[pos] is PropertyInfo)
                    {
                        PropertyInfo pinfo = (PropertyInfo)pinfos[pos];
                        object value = propertyValues[pinfo.Name];
                        
                            
                        if (value != null && !pinfo.PropertyType.IsAssignableFrom(value.GetType()))
                        {
                            throw new ArgumentException("PropertyValueInvalidEntry");
                        }
                        parameters[pos] = value;
                    }
                    else
                    {
                        parameters[pos] = pinfos[pos];
                    }
                }
                try
                {
                    return cinfo.Invoke(parameters);
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
            throw new ArgumentException("PropertyValueInvalidEntry");
        }
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            if (CheckContext(context))
            {
                Type t = context.PropertyDescriptor.PropertyType;
                ConstructorInfo cInfo = UTCConstructorAttribute.GetConstructor(t);
                if (cInfo != null)
                {
                    try
                    {
                        GetConstructorPropertyInfos(cInfo, t);
                        return true;
                    }
                    catch
                    {
                    }
                }
            }
            return false;
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {

            return TypeDescriptor.GetProperties(value.GetType(), attributes);
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return CheckContext(context) && context.PropertyDescriptor.PropertyType.IsDefined(typeof(UTCPropertiesSupportedAttribute), false);
        }
        static bool CheckContext(ITypeDescriptorContext context)
        {
            return context != null &&
                   context.PropertyDescriptor != null &&
                   context.PropertyDescriptor.PropertyType != null;
        }
        object Default(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.DBNull:
                    return DBNull.Value;
                case TypeCode.Empty:
                case TypeCode.String:
                    return null;
                case TypeCode.Object:
                    if (t.IsClass)
                    {
                        return null;
                    }
                    break;
            }
            return t.GetConstructor(Type.EmptyTypes).Invoke(null);
        }
        object[] GetConstructorParameters(ConstructorInfo cinfo, object value)
        {
            object[] proInfos = GetConstructorPropertyInfos(cinfo, value.GetType());

            int length = proInfos.Length;
            object[] rv = new object[length];
            for (int pos = 0; pos < length; ++pos)
            {
                if (proInfos[pos] is PropertyInfo)
                {
                    rv[pos] = ((PropertyInfo)proInfos[pos]).GetValue(value, null);
                }
                else
                {
                    rv[pos] = proInfos[pos];
                }
            }
            return rv;
        }
        object[] GetConstructorPropertyInfos(ConstructorInfo cinfo, Type t)
        {
            int length = cinfo.GetParameters().Length;
            string[] ParamatersOfDefaultValue = ((UTCConstructorAttribute)(cinfo.GetCustomAttributes(typeof(UTCConstructorAttribute), false)[0])).ParamatersOfDefaultValue;
            object[] rv = new object[length];
            ParameterInfo[] parinfos = cinfo.GetParameters();
            PropertyInfo[] proinfos = t.GetProperties();
            for (int pos = 0; pos < length; ++pos)
            {
                string name = parinfos[pos].Name;
                Type paramType = parinfos[pos].ParameterType;
                if (ParamatersOfDefaultValue != null && Array.IndexOf<string>(ParamatersOfDefaultValue, name) > -1)
                {
                    rv[pos] = Default(paramType);
                    continue;
                }
                rv[pos] = GetPropertyInfo(name, paramType, proinfos);
                if (rv[pos] == null)
                {
                    throw new Exception(string.Format("{0}s are screwed up. Dont have parameter {1}", typeof(UTCConstructorParameterAttribute).Name, name));
                }
            }
            return rv;
        }
        PropertyInfo GetPropertyInfo(string name, Type t, PropertyInfo[] proinfos)
        {
            PropertyInfo rv = null;
            foreach (PropertyInfo proinfo in proinfos)
            {
                if (proinfo.PropertyType == t)
                {
                    if (proinfo.Name == name)
                    {
                        rv = proinfo;
                    }
                    else
                    {
                        foreach (UTCConstructorParameterAttribute att in proinfo.GetCustomAttributes(typeof(UTCConstructorParameterAttribute), false))
                        {
                            if (att.ParameterName == name)
                            {
                                rv = proinfo;
                                break;
                            }
                        }
                    }
                }
            }
            return rv;
        }
    }
    [global::System.AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class UTCConstructorAttribute : Attribute
    {
        public static bool CanConstruct(Type t)
        {
            return GetConstructor(t) != null;
        }
        public static ConstructorInfo GetConstructor(Type t)
        {
            ConstructorInfo[] cinfos = t.GetConstructors();
            foreach (ConstructorInfo cinfo in cinfos)
            {
                if (cinfo.IsDefined(typeof(UTCConstructorAttribute), false))
                {
                    return cinfo;
                }
            }
            return null;
        }
        string[] _paramatersOfDefaultValue;
        public UTCConstructorAttribute() { }
        public UTCConstructorAttribute(string[] ParamatersOfDefaultValue)
        {
            this._paramatersOfDefaultValue = ParamatersOfDefaultValue;
        }
        public string[] ParamatersOfDefaultValue
        {
            get
            {
                return _paramatersOfDefaultValue;
            }
        }
    }
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class UTCConstructorParameterAttribute : Attribute
    {
        string _parameterName;
        public UTCConstructorParameterAttribute(string ParameterName)
        {
            this._parameterName = ParameterName;
        }
        public string ParameterName
        {
            get
            {
                return this._parameterName;
            }
        }
    }
    [global::System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class UTCParserAttribute : Attribute
    {
        public static bool CanParse(Type t)
        {
            return GetParser(t) != null;
        }
        public static MethodInfo GetParser(Type t)
        {
            MethodInfo[] minfos = t.GetMethods();
            foreach (MethodInfo minfo in minfos)
            {
                if ((minfo.IsConstructor || (minfo.IsStatic && t == minfo.ReturnType)) && minfo.IsDefined(typeof(UTCParserAttribute), false))
                {
                    ParameterInfo[] pinfos = minfo.GetParameters();
                    if (pinfos.Length == 1 && pinfos[0].ParameterType == typeof(string))
                    {
                        return minfo;
                    }
                }
            }
            return null;
        }
        public UTCParserAttribute() { }

    }
    [global::System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class UTCFormaterAttribute : Attribute
    {
        public static bool CanFormat(Type t)
        {
            return GetFormater(t) != null;
        }
        public static MethodInfo GetFormater(Type t)
        {
            MethodInfo[] minfos = t.GetMethods();
            foreach (MethodInfo minfo in minfos)
            {
                if (typeof(string) == minfo.ReturnType && minfo.IsDefined(typeof(UTCFormaterAttribute), false))
                {
                    ParameterInfo[] pinfos = minfo.GetParameters();
                    if ((!minfo.IsStatic && pinfos.Length == 0) || (minfo.IsStatic && pinfos.Length == 1 && pinfos[0].ParameterType == t))
                    {
                        return minfo;
                    }
                }
            }
            return null;
        }
        public UTCFormaterAttribute() { }

    }
    [global::System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct|AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class UTCPropertiesSupportedAttribute : Attribute
    {
        public UTCPropertiesSupportedAttribute() { }
    }
}