using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AdvanceSystem.ComponentModel.Design;
namespace AdvanceSystem.ComponentModel.GUI
{
    public partial class SerializedXmlEditor : Form
    {
        TypeRelations[] allowedTypes;

        public SerializedXmlEditor()
        {
            InitializeComponent();
            AllowedTypes = new TypeRelations[]{};
        }



        public TypeRelations[] AllowedTypes
        {
            get
            {
                return allowedTypes;
            }
            set
            {
                this.allowedTypes = value;
                
                
                //ToolStripMenuItem[] menuItems = Array.ConvertAll<TypeRelations,ToolStripMenuItem>(value,delegate(TypeRelations t){ return GetToolStripMenuItemFromType(t);});


                this.menuStrip.SuspendLayout();
                this.toolStrip.SuspendLayout();
                this.SuspendLayout();

                this.newWindowToolStripMenuItem.DropDownItems.Clear();
                this.newToolStripMenuItem.DropDownItems.Clear();
                this.newToolStripButton.DropDownItems.Clear();


                this.newWindowToolStripMenuItem.DropDownItems.AddRange(GetMenuItems(value));
                this.newToolStripButton.DropDownItems.AddRange(GetMenuItems(value));
                this.newToolStripMenuItem.DropDownItems.AddRange(GetMenuItems(value));

                
                this.menuStrip.ResumeLayout(false);
                this.menuStrip.PerformLayout();
                this.toolStrip.ResumeLayout(false);
                this.toolStrip.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();
            }
        }
        private ToolStripMenuItem[] GetMenuItems(TypeRelations[] value)
        {
            return Array.ConvertAll<TypeRelations, ToolStripMenuItem>(value, delegate(TypeRelations t) { return GetToolStripMenuItemFromType(t); });
        }
        private ToolStripMenuItem GetToolStripMenuItemFromType(TypeRelations t)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Text = t.Basetype.Name;
            menuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            menuItem.Tag = t;
            menuItem.Click += new EventHandler(ShowNewFormofTag);
            menuItem.Size = new System.Drawing.Size(152, 22);
            return menuItem;
        }
        private void ShowNewFormofTag(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                TypeRelations t = menuItem.Tag as TypeRelations;
                if (t != null)
                {
                    XmlObjectListEditor childForm = XmlObjectListEditor.FromTypes(t.Basetype, t.SubTypes);
                    // Make it a child of this MDI form before showing it.
                    childForm.MdiParent = this;
                    childForm.Show();
                }
            }
        }

        /*private void ShowNewForm(object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            int index = ComboBoxDialog.ShowOptions(this,"New Editor", Array.ConvertAll<TypeRelations, object>(allowedTypes, delegate(TypeRelations type) { return type.Basetype.Name; }));

            if (index > -1)
            {
                ObjectListEditor childForm = ObjectListEditor.FromTypes(allowedTypes[index].Basetype, allowedTypes[index].SubTypes);
                // Make it a child of this MDI form before showing it.
                childForm.MdiParent = this;
                childForm.Show();
            }
        }*/

        private void OpenFile(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    try
                    {

                        XmlObjectListEditor childForm = XmlObjectListEditor.FromFile(filename, allowedTypes);
                        //ObjectListEditor childForm = ObjectListEditor.FromFile(openFileDialog.FileName, allowedTypes);
                        // Make it a child of this MDI form before showing it.
                        childForm.MdiParent = this;
                        childForm.Show();

                    }
                    catch (Exception ex)
                    {
                        AdvanceSystem.Forms.ErrorBox.DisplayError(
                            string.Format("An Error occured While trying to Load: {0}",Path.GetFileName(filename)),
                            ex);
                    }
                }
            }
        }
        private void SaveFile(object sender, EventArgs e)
        {
            XmlObjectListEditor childForm = this.ActiveMdiChild as XmlObjectListEditor;
            if (childForm != null)
            {
                childForm.SaveToFile();
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XmlObjectListEditor childForm = this.ActiveMdiChild as XmlObjectListEditor;
            if (childForm != null)
            {
                childForm.SaveAsFile();
            }
        }



        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not Supported");
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not Supported");
            // TODO: Use System.Windows.Forms.Clipboard to insert the selected text or images into the clipboard
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not Supported");
            // TODO: Use System.Windows.Forms.Clipboard.GetText() or System.Windows.Forms.GetData to retrieve information from the clipboard.
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void SaveAll(object sender, EventArgs e)
        {
            Form start = this.ActiveMdiChild;
            foreach (XmlObjectListEditor child in this.MdiChildren)
            {
                if (!child.Saved)
                {
                    if (!child.Loaded)
                    {
                        child.Activate();
                    }
                    child.SaveToFile();
                }
            }
            start.Activate();
        }
    }
}
