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
namespace AdvanceSystem.ComponentModel.Design
{
    public class EnumListBox : ListBox
    {
        delegate object MergeLogic(SelectedObjectCollection o);
        Type enumType;
        MergeLogic logic;
        private void SetUp(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("The enumType Must be a enum", "enumType");
            }
            this.enumType = enumType;
            this.SetItemsCore(Enum.GetValues(enumType));
            if (enumType.IsDefined(typeof(FlagsAttribute), false))
            {
                this.SelectionMode = SelectionMode.MultiExtended;
                #region MergeLogic declaration
                switch (((Enum)Enum.GetValues(enumType).GetValue(0)).GetTypeCode())
                {
                    case TypeCode.Int64:
                        logic = delegate(SelectedObjectCollection objs)
                        {
                            Int64 rv = 0;
                            foreach (object t in objs)
                            {
                                rv |= (Int64)t;
                            }
                            return Enum.ToObject(enumType, rv);
                        };
                        break;
                    case TypeCode.Int32:
                        logic = delegate(SelectedObjectCollection objs)
                        {
                            Int32 rv = 0;
                            foreach (object t in objs)
                            {
                                rv |= (Int32)t;
                            }
                            return Enum.ToObject(enumType, rv);
                        };
                        break;
                    case TypeCode.Int16:
                        logic = delegate(SelectedObjectCollection objs)
                        {
                            Int16 rv = 0;
                            foreach (object t in objs)
                            {
                                rv |= (Int16)t;
                            }
                            return Enum.ToObject(enumType, rv);
                        };
                        break;
                    case TypeCode.SByte:
                        logic = delegate(SelectedObjectCollection objs)
                        {
                            SByte rv = 0;
                            foreach (object t in objs)
                            {
                                rv |= (SByte)t;
                            }
                            return Enum.ToObject(enumType, rv);
                        };
                        break;

                    case TypeCode.UInt64:
                        logic = delegate(SelectedObjectCollection objs)
                        {
                            UInt64 rv = 0;
                            foreach (object t in objs)
                            {
                                rv |= (UInt64)t;
                            }
                            return Enum.ToObject(enumType, rv);
                        };
                        break;
                    case TypeCode.UInt32:
                        logic = delegate(SelectedObjectCollection objs)
                        {
                            UInt32 rv = 0;
                            foreach (object t in objs)
                            {
                                rv |= (UInt32)t;
                            }
                            return Enum.ToObject(enumType, rv);
                        };
                        break;
                    case TypeCode.UInt16:
                        logic = delegate(SelectedObjectCollection objs)
                        {
                            UInt16 rv = 0;
                            foreach (object t in objs)
                            {
                                rv |= (UInt16)t;
                            }
                            return Enum.ToObject(enumType, rv);
                        };
                        break;
                    case TypeCode.Byte:
                        logic = delegate(SelectedObjectCollection objs)
                        {
                            Byte rv = 0;
                            foreach (object t in objs)
                            {
                                rv |= (Byte)t;
                            }
                            return Enum.ToObject(enumType, rv);
                        };
                        break;
                }
                #endregion
            }
            else
            {
                this.SelectionMode = SelectionMode.One;
            }
        }

        public object SelectedFlags
        {
            get
            {
                object rv = logic(base.SelectedItems);
                return rv;
            }
            set
            {
                string[] tmp = Enum.Format(enumType, value, "G").Split(',');
                object[] vals = Array.ConvertAll<string, object>(tmp, delegate(string name) { return Enum.Parse(enumType, name); });
                lock (this.SelectedItems)
                {
                    this.SelectedItems.Clear();
                    foreach (object o in vals)
                    {
                        SelectedItems.Add(o);
                    }
                }
            }
        }
        public object SelectedEnum
        {
            get
            {
                if (this.SelectionMode == SelectionMode.One)
                {
                    return base.SelectedItem;
                }
                else
                {
                    return SelectedFlags;
                }
            }
            set
            {
                if (enumType == null || enumType != value.GetType())
                {
                    SetUp(value.GetType());
                }
                if (this.SelectionMode == SelectionMode.One)
                {
                    base.SelectedItem = value;
                }
                else
                {
                    SelectedFlags = value;
                }
            }
        }
        /*public override object SelectedValue
        {
            get
            {
                return SelectedEnum;
            }
            set
            {
                SelectedEnum = value;
            }
        }*/
    }

}