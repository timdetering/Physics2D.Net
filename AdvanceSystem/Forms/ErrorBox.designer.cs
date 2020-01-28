namespace AdvanceSystem.Forms
{
    partial class ErrorBox
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
            this.components = new System.ComponentModel.Container();
            this.rtbMessage = new System.Windows.Forms.RichTextBox();
            this.CopyContextStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bOK = new System.Windows.Forms.Button();
            this.bCopy = new System.Windows.Forms.Button();
            this.bQuit = new System.Windows.Forms.Button();
            this.CopyContextStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbMessage
            // 
            this.rtbMessage.ContextMenuStrip = this.CopyContextStrip;
            this.rtbMessage.Location = new System.Drawing.Point(12, 12);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.ReadOnly = true;
            this.rtbMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.rtbMessage.Size = new System.Drawing.Size(368, 205);
            this.rtbMessage.TabIndex = 0;
            this.rtbMessage.Text = "";
            this.rtbMessage.WordWrap = false;
            // 
            // CopyContextStrip
            // 
            this.CopyContextStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.CopyContextStrip.Name = "CopyContextStrip";
            this.CopyContextStrip.Size = new System.Drawing.Size(174, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.copyToolStripMenuItem.Text = "Copy To Clipboard";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(299, 223);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(81, 31);
            this.bOK.TabIndex = 1;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bCopy
            // 
            this.bCopy.Location = new System.Drawing.Point(12, 223);
            this.bCopy.Name = "bCopy";
            this.bCopy.Size = new System.Drawing.Size(110, 31);
            this.bCopy.TabIndex = 3;
            this.bCopy.Text = "Copy To Clipboard";
            this.bCopy.UseVisualStyleBackColor = true;
            this.bCopy.Click += new System.EventHandler(this.bCopy_Click);
            // 
            // bQuit
            // 
            this.bQuit.Location = new System.Drawing.Point(219, 223);
            this.bQuit.Name = "bQuit";
            this.bQuit.Size = new System.Drawing.Size(74, 31);
            this.bQuit.TabIndex = 4;
            this.bQuit.Text = "Quit";
            this.bQuit.UseVisualStyleBackColor = true;
            this.bQuit.Click += new System.EventHandler(this.bQuit_Click);
            // 
            // ErrorBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 266);
            this.Controls.Add(this.bQuit);
            this.Controls.Add(this.rtbMessage);
            this.Controls.Add(this.bCopy);
            this.Controls.Add(this.bOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorBox";
            this.Text = "ErrorBox";
            this.CopyContextStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbMessage;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.ContextMenuStrip CopyContextStrip;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.Button bCopy;
        private System.Windows.Forms.Button bQuit;
    }
}