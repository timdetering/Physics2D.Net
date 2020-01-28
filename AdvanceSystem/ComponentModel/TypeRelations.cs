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
    public class TypeRelations
    {



        private static void SetSubTypes(Type Basetype, List<Type> SubTypes)
        {
            if (!SubTypes.Contains(Basetype) && CheckType(Basetype))
            {
                SubTypes.Add(Basetype);
            }
            foreach (EditorXmlIncludeAttribute att in GetEditorXmlIncludeAttributes(Basetype))
            {
                SetSubTypes(att.Type, SubTypes);
            }
        }
        private static EditorXmlIncludeAttribute[] GetEditorXmlIncludeAttributes(Type Basetype)
        {
            return Array.ConvertAll<object, EditorXmlIncludeAttribute>(
                Basetype.GetCustomAttributes(typeof(EditorXmlIncludeAttribute), false),
                delegate(object type)
                {
                    return (EditorXmlIncludeAttribute)type;
                });
        }

        private static bool CheckType(Type type)
        {
            return !type.IsAbstract &&
                !type.IsArray &&
                type.IsClass &&
                !type.IsInterface &&
                (type.GetConstructor(Type.EmptyTypes) != null||
                DefaultObjectValueAttribute.GetCustomDefaultValue(type) != null);
        }

        Type basetype;
        Type[] subTypes;
        public Type Basetype
        {
            get { return basetype; }
        }
        public Type[] SubTypes
        {
            get { return subTypes; }
        }
        public Type[] ValidTypes
        {
            get
            {
                if (basetype.IsAbstract || basetype.IsInterface)
                {
                    return subTypes;
                }
                else
                {
                    Type[] rv = new Type[subTypes.Length + 1];
                    rv[0] = basetype;
                    subTypes.CopyTo(rv, 1);
                    return rv;
                }
            }
        }
        public TypeRelations(Type Basetype, params Type[] SubTypes)
        {
            this.basetype = Basetype;
            this.subTypes = SubTypes;
        }
        public TypeRelations(Type Basetype)
        {
            this.basetype = Basetype;
            List<Type> SubTypes = new List<Type>();
            SetSubTypes(basetype, SubTypes);
            this.subTypes = SubTypes.ToArray();
        }
    }
}