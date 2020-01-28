#region LGPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AdvanceSystem.Forms
{



    public partial class ErrorBox : Form
    {
        static object syncroot = new object();
        public static void DisplayError(Exception ex)
        {
            DisplayError(null, ex);
        }
        public static void DisplayError(string message, Exception ex)
        {
            lock (syncroot)
            {
                StringBuilder sb = new StringBuilder();
                if (message != null)
                {
                    sb.AppendLine(message);
                }
                AppendMessage(sb, ex);
                while ((ex = ex.InnerException) != null)
                {
                    sb.AppendLine("InnerException:");
                    AppendMessage(sb, ex);
                }
                using (ErrorBox box = new ErrorBox(sb.ToString()))
                {
                    box.ShowDialog();
                }
            }
        }
        static void AppendMessage(StringBuilder sb, Exception ex)
        {
            sb.AppendFormat("{1}{0}Target: {2}{0}Source: {3}{0}",
                Environment.NewLine,
                ex.ToString(),
                ex.TargetSite,
                ex.Source);
        }
        protected ErrorBox(string error)
        {
            InitializeComponent();
            this.rtbMessage.Text = error;
        }
        private void bOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(this.rtbMessage.Text);
            }
            catch { }
        }

        private void bCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(this.rtbMessage.Text);
            }
            catch { }
        }

        private void bQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(-1);
        }
    }
}