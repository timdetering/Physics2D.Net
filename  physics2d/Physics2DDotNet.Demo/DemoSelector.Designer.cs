namespace Physics2DDotNet.Demo
{
    partial class DemoSelector
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
            this.lDemos = new System.Windows.Forms.Label();
            this.bStart = new System.Windows.Forms.Button();
            this.rtbDescription = new System.Windows.Forms.RichTextBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.bSettings = new System.Windows.Forms.Button();
            this.lvDemos = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lDemos
            // 
            this.lDemos.AutoSize = true;
            this.lDemos.Location = new System.Drawing.Point(12, 9);
            this.lDemos.Name = "lDemos";
            this.lDemos.Size = new System.Drawing.Size(40, 13);
            this.lDemos.TabIndex = 1;
            this.lDemos.Text = "Demos";
            // 
            // bStart
            // 
            this.bStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bStart.Enabled = false;
            this.bStart.Location = new System.Drawing.Point(136, 426);
            this.bStart.Name = "bStart";
            this.bStart.Size = new System.Drawing.Size(75, 23);
            this.bStart.TabIndex = 2;
            this.bStart.Text = "Start";
            this.bStart.UseVisualStyleBackColor = true;
            this.bStart.Click += new System.EventHandler(this.OnStartClick);
            // 
            // rtbDescription
            // 
            this.rtbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbDescription.Location = new System.Drawing.Point(15, 242);
            this.rtbDescription.Name = "rtbDescription";
            this.rtbDescription.ReadOnly = true;
            this.rtbDescription.Size = new System.Drawing.Size(196, 178);
            this.rtbDescription.TabIndex = 3;
            this.rtbDescription.Text = "";
            // 
            // lDescription
            // 
            this.lDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lDescription.AutoSize = true;
            this.lDescription.Location = new System.Drawing.Point(12, 226);
            this.lDescription.Name = "lDescription";
            this.lDescription.Size = new System.Drawing.Size(60, 13);
            this.lDescription.TabIndex = 4;
            this.lDescription.Text = "Description";
            // 
            // bSettings
            // 
            this.bSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSettings.Enabled = false;
            this.bSettings.Location = new System.Drawing.Point(12, 426);
            this.bSettings.Name = "bSettings";
            this.bSettings.Size = new System.Drawing.Size(75, 23);
            this.bSettings.TabIndex = 5;
            this.bSettings.Text = "Settings";
            this.bSettings.UseVisualStyleBackColor = true;
            // 
            // lvDemos
            // 
            this.lvDemos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvDemos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvDemos.FullRowSelect = true;
            listViewGroup1.Header = "ListViewGroup";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "ListViewGroup";
            listViewGroup2.Name = "listViewGroup2";
            this.lvDemos.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.lvDemos.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listViewItem1.Group = listViewGroup1;
            listViewItem2.Group = listViewGroup1;
            this.lvDemos.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.lvDemos.Location = new System.Drawing.Point(12, 25);
            this.lvDemos.MultiSelect = false;
            this.lvDemos.Name = "lvDemos";
            this.lvDemos.Size = new System.Drawing.Size(199, 198);
            this.lvDemos.TabIndex = 6;
            this.lvDemos.UseCompatibleStateImageBehavior = false;
            this.lvDemos.View = System.Windows.Forms.View.Details;
            this.lvDemos.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.OnSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 115;
            // 
            // DemoSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 459);
            this.Controls.Add(this.lvDemos);
            this.Controls.Add(this.bSettings);
            this.Controls.Add(this.rtbDescription);
            this.Controls.Add(this.bStart);
            this.Controls.Add(this.lDemos);
            this.Controls.Add(this.lDescription);
            this.Name = "DemoSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Demo Selector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lDemos;
        private System.Windows.Forms.Button bStart;
        private System.Windows.Forms.RichTextBox rtbDescription;
        private System.Windows.Forms.Label lDescription;
        private System.Windows.Forms.Button bSettings;
        private System.Windows.Forms.ListView lvDemos;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}