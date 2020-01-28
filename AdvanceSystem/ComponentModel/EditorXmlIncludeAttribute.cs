using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
namespace AdvanceSystem.ComponentModel
{
    /// <summary>Allows the <see cref="T:System.Xml.Serialization.XmlSerializer"></see> to recognize a type when it serializes or deserializes an object.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    public class EditorXmlIncludeAttribute : XmlIncludeAttribute
    {
        /// <summary>Initializes a new instance of the XmlEditorIncludeAttribute class.</summary>
        /// <param name="type">The <see cref="T:System.Type"></see> of the object to include. </param>
        public EditorXmlIncludeAttribute(Type type)
            : base(type)
        { }
    }
    [global::System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DefaultObjectValueAttribute : Attribute
    {
        public DefaultObjectValueAttribute() { }
        public static object GetCustomDefaultValue(Type itemType)
        {
            foreach (FieldInfo finfo in itemType.GetFields())
            {
                if (finfo.IsStatic && finfo.IsDefined(typeof(DefaultObjectValueAttribute), false))
                {
                    return finfo.GetValue(null);
                }
            }
            foreach (PropertyInfo pinfo in itemType.GetProperties())
            {
                if ( pinfo.CanRead  && pinfo.IsDefined(typeof(DefaultObjectValueAttribute), false))
                {
                    return pinfo.GetValue(null,null);
                }
            }
            return null;
        }
    }
}