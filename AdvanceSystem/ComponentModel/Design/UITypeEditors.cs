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
namespace AdvanceSystem.ComponentModel.Design
{
    public class EnumEditor : UITypeEditor
    {
        private EnumListBox enumLB;
        public EnumEditor()
        {
            enumLB = new EnumListBox();
            enumLB.BorderStyle = BorderStyle.None;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null
                && context.Instance != null
                && provider != null)
            {

                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    //Enum e = (Enum)Convert.ChangeType(value, context.PropertyDescriptor.PropertyType);
                    enumLB.SelectedEnum = value;
                    edSvc.DropDownControl(enumLB);
                    return enumLB.SelectedEnum;
                }
            }
            return null;
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
    public class StringCollectionEditor : UITypeEditor
    {
        bool Gridunset = true;
        PropertyGrid propertyGrid;
        private StringCollectionEditorUI tb;
        public StringCollectionEditor()
        {
            tb = new StringCollectionEditorUI();
            tb.Enter += new EventHandler(SetSize);
        }
        void SetSize(object sender, EventArgs e)
        {
            if (propertyGrid != null)
            {
                tb.Width = propertyGrid.Width - (18);
            }

            // ResizeToContent();
            //tb.Width = tb.Parent.Parent.Width;
        }
        static Type[] ConstrutorParams = new Type[]
            {
                typeof(IList<string>),
                typeof(ICollection<string>),
                typeof(string[]),
                typeof(IList),
                typeof(Array),
            };
        static Type[] AddParams = new Type[]
            {
                typeof(string),
                typeof(object),
            };
        private ConstructorInfo GetConstructor(Type t)
        {
            ConstructorInfo result;
            foreach (Type con in ConstrutorParams)
            {
                result = t.GetConstructor(new Type[] { con });
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        private MethodInfo GetAddRange(Type t)
        {
            MethodInfo result;
            foreach (Type con in ConstrutorParams)
            {
                result = t.GetMethod("AddRange", new Type[] { con });
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        private MethodInfo GetAdd(Type t)
        {
            MethodInfo result;
            foreach (Type con in AddParams)
            {
                result = t.GetMethod("Add", new Type[] { con });
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        private object CreateReturnValue(Type t, string[] result)
        {

            ConstructorInfo cotr = GetConstructor(t);
            if (cotr != null)
            {
                return cotr.Invoke(new object[] { result });
            }
            else
            {
                cotr = t.GetConstructor(Type.EmptyTypes);
                if (cotr != null)
                {
                    object newvalue = cotr.Invoke(null);
                    MethodInfo add = GetAddRange(t);
                    if (add != null)
                    {
                        add.Invoke(newvalue, new object[] { result });
                        return newvalue;
                    }
                    else
                    {
                        add = GetAdd(t);
                        if (add != null)
                        {
                            foreach (string str in result)
                            {
                                add.Invoke(newvalue, new object[] { str });
                            }
                            return newvalue;
                        }
                    }
                }
            }
            throw new Exception("The collection Type is freaking stupid");
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (Gridunset)
            {
                Gridunset = false;
                try
                {
                    propertyGrid = (PropertyGrid)provider.GetType().GetProperty("OwnerGrid").GetValue(provider, null);
                    propertyGrid.Resize += new EventHandler(SetSize);

                }
                catch { }
            }
            if (context != null
                && context.Instance != null
                && provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    tb.SetText(value as IEnumerable);
                    string text = tb.Text;
                    edSvc.DropDownControl(tb);
                    if (text == tb.Text)
                    {
                        return value;
                    }
                    string[] result = tb.Lines;
                    if (value is string[])
                    {
                        return result;
                    }
                    return CreateReturnValue(value.GetType(), result);
                }
            }
            return null;
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
    public class XmlCollectionEditor : CollectionEditor
    {
        public XmlCollectionEditor(Type type) : base(type) { }
        protected override Type[] CreateNewItemTypes()
        {
            TypeRelations relations = new TypeRelations(this.CollectionItemType);
            if (relations.SubTypes.Length > 0)
            {
                return relations.ValidTypes;
            }
            return base.CreateNewItemTypes();
        }
        protected override object CreateInstance(Type itemType)
        {
            if (itemType == typeof(string))
            {
                return "";
            }
            object rv = DefaultObjectValueAttribute.GetCustomDefaultValue(itemType);
            if (rv != null)
            {
                return rv;
            }
            return base.CreateInstance(itemType);
        }
    }
    public class XmlArrayEditor : ArrayEditor
    {
        public XmlArrayEditor(Type type) : base(type) { }
        protected override Type[] CreateNewItemTypes()
        {
            TypeRelations relations = new TypeRelations(this.CollectionItemType);
            if (relations.SubTypes.Length > 0)
            {
                return relations.ValidTypes;
            }
            return base.CreateNewItemTypes();
        }
        protected override object CreateInstance(Type itemType)
        {
            if (itemType == typeof(string))
            {
                return "";
            }
            object rv = DefaultObjectValueAttribute.GetCustomDefaultValue(itemType);
            if (rv != null)
            {
                return rv;
            }
            return base.CreateInstance(itemType);
        }
    }
    public class BooleanEditor : UITypeEditor
    {
        private ListBox boolLB;
        public BooleanEditor()
        {
            boolLB = new ListBox();
            boolLB.BorderStyle = BorderStyle.None;
            boolLB.Items.Add("True");
            boolLB.Items.Add("False");
            boolLB.DoubleClick += new EventHandler(boolLB_DoubleClick);
            boolLB.Height /= 3;
        }

        void boolLB_DoubleClick(object sender, EventArgs e)
        {
            edSvc.CloseDropDown();
        }
        IWindowsFormsEditorService edSvc;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null
                && context.Instance != null
                && provider != null)
            {

                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    //Enum e = (Enum)Convert.ChangeType(value, context.PropertyDescriptor.PropertyType);
                    if ((bool)value)
                    {
                        boolLB.SelectedIndex = 0;
                    }
                    else
                    {
                        boolLB.SelectedIndex = 1;
                    }
                    edSvc.DropDownControl(boolLB);
                    return boolLB.SelectedIndex == 0;
                }
            }
            return null;
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}