namespace WindowsDriver
{
    partial class MainMenu
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
            this.lbDemos = new System.Windows.Forms.ListBox();
            this.rtbDemoDescription = new System.Windows.Forms.RichTextBox();
            this.lDemoDescription = new System.Windows.Forms.Label();
            this.lDemoList = new System.Windows.Forms.Label();
            this.bRunDemo = new System.Windows.Forms.Button();
            this.lInstructions = new System.Windows.Forms.Label();
            this.rtbInstructions = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lbDemos
            // 
            this.lbDemos.FormattingEnabled = true;
            this.lbDemos.Location = new System.Drawing.Point(12, 29);
            this.lbDemos.Name = "lbDemos";
            this.lbDemos.Size = new System.Drawing.Size(188, 355);
            this.lbDemos.TabIndex = 0;
            this.lbDemos.SelectedValueChanged += new System.EventHandler(this.lbDemos_SelectedValueChanged);
            // 
            // rtbDemoDescription
            // 
            this.rtbDemoDescription.Location = new System.Drawing.Point(206, 29);
            this.rtbDemoDescription.Name = "rtbDemoDescription";
            this.rtbDemoDescription.ReadOnly = true;
            this.rtbDemoDescription.Size = new System.Drawing.Size(416, 154);
            this.rtbDemoDescription.TabIndex = 1;
            this.rtbDemoDescription.Text = "";
            // 
            // lDemoDescription
            // 
            this.lDemoDescription.AutoSize = true;
            this.lDemoDescription.Location = new System.Drawing.Point(203, 13);
            this.lDemoDescription.Name = "lDemoDescription";
            this.lDemoDescription.Size = new System.Drawing.Size(91, 13);
            this.lDemoDescription.TabIndex = 2;
            this.lDemoDescription.Text = "Demo Description";
            // 
            // lDemoList
            // 
            this.lDemoList.AutoSize = true;
            this.lDemoList.Location = new System.Drawing.Point(9, 13);
            this.lDemoList.Name = "lDemoList";
            this.lDemoList.Size = new System.Drawing.Size(54, 13);
            this.lDemoList.TabIndex = 3;
            this.lDemoList.Text = "Demo List";
            // 
            // bRunDemo
            // 
            this.bRunDemo.Location = new System.Drawing.Point(506, 344);
            this.bRunDemo.Name = "bRunDemo";
            this.bRunDemo.Size = new System.Drawing.Size(116, 40);
            this.bRunDemo.TabIndex = 4;
            this.bRunDemo.Text = "Run Demo";
            this.bRunDemo.UseVisualStyleBackColor = true;
            this.bRunDemo.Click += new System.EventHandler(this.bRunDemo_Click);
            // 
            // lInstructions
            // 
            this.lInstructions.AutoSize = true;
            this.lInstructions.Location = new System.Drawing.Point(206, 197);
            this.lInstructions.Name = "lInstructions";
            this.lInstructions.Size = new System.Drawing.Size(61, 13);
            this.lInstructions.TabIndex = 5;
            this.lInstructions.Text = "Instructions";
            // 
            // rtbInstructions
            // 
            this.rtbInstructions.Location = new System.Drawing.Point(206, 213);
            this.rtbInstructions.Name = "rtbInstructions";
            this.rtbInstructions.ReadOnly = true;
            this.rtbInstructions.Size = new System.Drawing.Size(416, 120);
            this.rtbInstructions.TabIndex = 6;
            this.rtbInstructions.Text = "";
            // 
            // MainMenu
            // 
            this.AcceptButton = this.bRunDemo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 399);
            this.Controls.Add(this.rtbInstructions);
            this.Controls.Add(this.lInstructions);
            this.Controls.Add(this.bRunDemo);
            this.Controls.Add(this.lDemoList);
            this.Controls.Add(this.lDemoDescription);
            this.Controls.Add(this.rtbDemoDescription);
            this.Controls.Add(this.lbDemos);
            this.Name = "MainMenu";
            this.Text = "Physics2D Windows Driver";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbDemos;
        private System.Windows.Forms.RichTextBox rtbDemoDescription;
        private System.Windows.Forms.Label lDemoDescription;
        private System.Windows.Forms.Label lDemoList;
        private System.Windows.Forms.Button bRunDemo;
        private System.Windows.Forms.Label lInstructions;
        private System.Windows.Forms.RichTextBox rtbInstructions;


    }
}

