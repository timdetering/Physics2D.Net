using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Diagnostics;
using AdvanceSystem.ComponentModel.Design;

namespace AdvanceSystem.ComponentModel.GUI
{
    public partial class XmlObjectListEditor : Form
    {
        static BinaryFormatter formater = new BinaryFormatter();
        public static XmlObjectListEditor FromTypes(Type baseType, params Type[] subtypes)
        {
            XmlObjectListEditor oObjectListEditor = new XmlObjectListEditor();
            oObjectListEditor.SetUp(baseType, subtypes);
            oObjectListEditor.Loaded = false;
            return oObjectListEditor;
        }
        public static XmlObjectListEditor FromFile(string FileName, Type baseType, params Type[] subtypes)
        {
            XmlObjectListEditor form = new XmlObjectListEditor();
            form.SetUp(baseType, subtypes);
            form.LoadFromFile(FileName);
            return form;
        }
        public static XmlObjectListEditor FromFile(string FileName, TypeRelations[] typeRelations)
        {
            XmlObjectListEditor form = new XmlObjectListEditor();
            string line2;
            using (System.IO.StreamReader reader = new StreamReader(FileName))
            {
                reader.ReadLine();
                line2 = reader.ReadLine();
            }
            foreach (TypeRelations rel in typeRelations)
            {
                if (Regex.IsMatch(line2, "<ArrayOf" + rel.Basetype.Name + "( |>)"))
                {
                    form.SetUp(rel.Basetype, rel.SubTypes);
                    break;
                }
            }
            if (form.subtypes == null)
            {
                throw new Exception("File is invalid");
            }
            form.LoadFromFile(FileName);
            return form;
        }
        XmlSerializer serializer;
        Type[] subtypes;
        bool loaded;
        private XmlObjectListEditor()
        {
            InitializeComponent();
            Loaded = false;
            Saved = true;
        }
        public bool Loaded
        {
            get { return loaded; }
            private set { loaded = value; }
        }
        public bool Saved
        {
            get
            {
                return !saveToolStripMenuItem.Enabled;
            }
            private set
            {
                saveToolStripMenuItem.Enabled = !value;
            }
        }
        public Array ObjectValues
        {
            get
            {
                Array arr = Array.CreateInstance((Type)bs.DataSource, bs.Count);
                bs.CopyTo(arr, 0);
                return arr;
            }
            set
            {
                bs.Clear();
                foreach (object o in value)
                {
                    bs.Add(o);
                }
            }
        }
        public string FileName
        {
            get { return this.saveFileDialog.FileName; }
            private set
            {
                Loaded = true;
                this.saveFileDialog.FileName = value;
                this.Text = Path.GetFileName(value);
            }
        }
        public void SetUp(Type baseType, params Type[] subtypes)
        {
            this.bs.Clear();
            this.bs.DataSource = baseType;
            this.cbTypes.Items.Clear();
            this.subtypes = subtypes;
            foreach (Type subtype in subtypes)
            {
                this.cbTypes.Items.Add(subtype.Name);
            }
            this.cbTypes.SelectedIndex = subtypes.Length - 1;
            FileName = baseType.Name + ".XML";
            try
            {
                serializer = new XmlSerializer(baseType.MakeArrayType());//, subtypes);
            }
            catch (Exception ex)
            {
                /*AdvanceSystem.Forms.ErrorBox.DisplayError(
                            string.Format("An Error occured While trying to Create the XmlSerializer of type: {0}", baseType.Name),
                            ex);*/
                serializer = null;
            }
            this.bs.AllowNew = true;
        }
        Type GetNewObjectType()
        {
            if (this.cbTypes.SelectedIndex > -1)
            {
                return (subtypes[this.cbTypes.SelectedIndex]);
            }
            else if (subtypes.Length > 0)
            {
                return (subtypes[0]);
            }
            else
            {
                return ((Type)bs.DataSource);
            }
        }
        object GetNewObject()
        {
            Type itemType = GetNewObjectType();
            object rv = DefaultObjectValueAttribute.GetCustomDefaultValue(itemType);
            if (rv == null)
            {
                rv = itemType.GetConstructor(new Type[0]).Invoke(null);
            }
            return rv;
        }
        public void LoadFromFile(string FileName)
        {
            this.FileName = FileName;
            using (FileStream stream = File.OpenRead(FileName))
            {
                if (serializer != null)
                {
                    ObjectValues = (Array)serializer.Deserialize(stream);
                }
                else
                {
                    ObjectValues = (Array)formater.Deserialize(stream);
                }
                Saved = true;
            }
        }
        public void SaveAsFile()
        {
            Loaded = false;
            SaveToFile();
        }
        public void SaveToFile()
        {
            if (serializer != null&&( Loaded || saveFileDialog.ShowDialog(this) == DialogResult.OK))
            {
                using (Stream stream = saveFileDialog.OpenFile())
                {
                    if (serializer != null)
                    {
                        serializer.Serialize(stream, ObjectValues);
                    }
                    else
                    {
                        formater.Serialize(stream, ObjectValues);
                    }
                    Loaded = true;
                    Saved = true;
                }
            }
        }
        private void bs_PositionChanged(object sender, EventArgs e)
        {
            this.propertyGrid.SelectedObject = bs.Current;
        }
        private void propertyGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            bs.EndEdit();
            bs.ResetBindings(false);
            if (bs.Current != null)
            {
                string name = bs.Current.GetType().Name;
                this.cbTypes.SelectedIndex = Array.FindIndex<Type>(subtypes, delegate(Type t) { return t.Name == name; });
            }
        }
        private void bs_AddingNew(object sender, AddingNewEventArgs e)
        {
            e.NewObject = GetNewObject();
        }
        private void SomethingChanged(object sender, EventArgs e)
        {
            Saved = false;
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFromFile(saveFileDialog.FileName);
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsFile();
        }
        private void SomethingChanged(object sender, ListChangedEventArgs e)
        {

        }
        private void ObjectListEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (!Saved)
            {
                DialogResult result = MessageBox.Show(this,
                    "Do you want to save the changes to " + this.Text + ".",
                    this.MdiParent.Text,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation);
                switch (result)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        SaveToFile();
                        break;
                }
            }
        }
        private void lbNamesCopy_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(lbNames.GetItemText(lbNames.SelectedValue));
        }
    }
}