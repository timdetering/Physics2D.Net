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
    public class StringCollectionEditorUI : RichTextBox
    {
        static string newLineChar = "\n";
        static string newLineString = @"\n";

        private ToolTip toolTip;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem cutStripMenuItem;
        private ToolStripMenuItem copyStripMenuItem;
        private ToolStripMenuItem pasteStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem selectAlltoolStripMenuItem;
        private ToolStripMenuItem selectLineStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem makeSingleStringtoolStripMenuItem;
        private ToolStripMenuItem makeMultipleStringstoolStripMenuItem;
        private IContainer components;
    
        public StringCollectionEditorUI()
        {
            InitializeComponent();
            this.BorderStyle = BorderStyle.None;
            //tb.AcceptsReturn = true;
            this.Height = 150;
            this.AcceptsTab = true;
            this.Multiline = true;
            this.ScrollBars = RichTextBoxScrollBars.Both;
            this.WordWrap = false;
            this.MinimumSize = new Size(100, 20);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.selectLineStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAlltoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.makeSingleStringtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeMultipleStringstoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutStripMenuItem,
            this.copyStripMenuItem,
            this.pasteStripMenuItem,
            this.toolStripSeparator1,
            this.selectLineStripMenuItem,
            this.selectAlltoolStripMenuItem,
            this.toolStripSeparator2,
            this.makeSingleStringtoolStripMenuItem,
            this.makeMultipleStringstoolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(180, 170);
            // 
            // cutStripMenuItem
            // 
            this.cutStripMenuItem.Name = "cutStripMenuItem";
            this.cutStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.cutStripMenuItem.Text = "Cut";
            this.cutStripMenuItem.Click += new System.EventHandler(this.cutStripMenuItem_Click);
            // 
            // copyStripMenuItem
            // 
            this.copyStripMenuItem.Name = "copyStripMenuItem";
            this.copyStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.copyStripMenuItem.Text = "Copy";
            this.copyStripMenuItem.Click += new System.EventHandler(this.copyStripMenuItem_Click);
            // 
            // pasteStripMenuItem
            // 
            this.pasteStripMenuItem.Name = "pasteStripMenuItem";
            this.pasteStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.pasteStripMenuItem.Text = "Paste";
            this.pasteStripMenuItem.Click += new System.EventHandler(this.pasteStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(176, 6);
            // 
            // selectLineStripMenuItem
            // 
            this.selectLineStripMenuItem.Name = "selectLineStripMenuItem";
            this.selectLineStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.selectLineStripMenuItem.Text = "Select Line";
            this.selectLineStripMenuItem.Click += new System.EventHandler(this.selectLineStripMenuItem_Click);
            // 
            // selectAlltoolStripMenuItem
            // 
            this.selectAlltoolStripMenuItem.Name = "selectAlltoolStripMenuItem";
            this.selectAlltoolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.selectAlltoolStripMenuItem.Text = "Select All";
            this.selectAlltoolStripMenuItem.Click += new System.EventHandler(this.selectAlltoolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(176, 6);
            // 
            // makeSingleStringtoolStripMenuItem
            // 
            this.makeSingleStringtoolStripMenuItem.Name = "makeSingleStringtoolStripMenuItem";
            this.makeSingleStringtoolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.makeSingleStringtoolStripMenuItem.Text = "Make Single String";
            this.makeSingleStringtoolStripMenuItem.Click += new EventHandler(makeSingleStringtoolStripMenuItem_Click);
            // 
            // makeMultipleStringstoolStripMenuItem
            // 
            this.makeMultipleStringstoolStripMenuItem.Name = "makeMultipleStringstoolStripMenuItem";
            this.makeMultipleStringstoolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.makeMultipleStringstoolStripMenuItem.Text = "Make Multiple Strings";
            this.makeMultipleStringstoolStripMenuItem.Click += new EventHandler(makeMultipleStringstoolStripMenuItem_Click);
            // 
            // StringCollectionUIEditor
            // 
            this.ContextMenuStrip = this.contextMenuStrip;
            this.toolTip.SetToolTip(this, "One line per string. Use “\\n” to represent a new line in a string.");
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        public void SelectedReplace(string oldValue, string newValue)
        {
            string newstring = this.SelectedText.Replace(oldValue, newValue);
            int start = SelectionStart;
            this.SelectedText = newstring;
            Select(start, newstring.Length);
        }
        public void SetText(IEnumerable collection)
        {
            bool afterfirst = false;
            StringBuilder rv = new StringBuilder();
            foreach (string s in collection)
            {
                if (afterfirst)
                {
                    rv.Append(Environment.NewLine);
                }
                afterfirst = true;
                int index = rv.Length;
                rv.Append(s);
                if (s.Length > 0)
                {
                    rv.Replace(Environment.NewLine, newLineString, index, rv.Length - index);
                    rv.Replace(newLineChar, newLineString, index, rv.Length - index);
                }
            }
            Text = rv.ToString();
        }
        public new string[] Lines
        {
            get
            {
                string[] result = base.Lines;
                int Length = result.Length;
                for (int pos = 0; pos < Length; ++pos)
                {
                    result[pos] = result[pos].Replace(newLineString, Environment.NewLine);
                }
                return result;
            }
        }


        void makeMultipleStringstoolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedReplace(newLineString,newLineChar);
        }


        void makeSingleStringtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedReplace(newLineChar, newLineString);
        }

        
        public void SelectLine()
        {
            string text = this.Text;
            int Length = text.Length;
            int start = this.SelectionStart-1;
            if (start == -1)
            {
                start = 0;
            }
            
            start = text.LastIndexOf(newLineChar, start);
            if(start == -1)
            {
                start = 0;
            }
            else 
            {
                start += newLineChar.Length;
            }
            int end = text.IndexOf(newLineChar, start);
            if (end == -1)
            {
                if (start == 0)
                {
                    this.SelectAll();
                }
                else
                {
                    this.Select(start, Length - start);
                }
            }
            else
            {
                this.Select(start, end - start);
            }
            
        }
        void selectLineStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectLine();
        }



        void selectAlltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectAll();
        }

        void pasteStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Paste();
        }

        void cutStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cut();
        }

        void copyStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Copy();
        }


    }
}