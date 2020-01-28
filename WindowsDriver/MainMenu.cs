#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1fof the License, or (at your option) any later version.
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
using WindowsDriver.Demos;
using System.Threading;
namespace WindowsDriver
{
    public partial class MainMenu : Form
    {
        IDemo currentDemo = null;
        public MainMenu(IDemo[] demos)
        {
            InitializeComponent();
            foreach (IDemo demo in demos)
            {
                this.lbDemos.Items.Add(demo);
            }
        }
        private void lbDemos_SelectedValueChanged(object sender, EventArgs e)
        {
            currentDemo = (IDemo)lbDemos.SelectedItem;
            if (currentDemo != null)
            {
                rtbDemoDescription.Text = currentDemo.Description;
                rtbInstructions.Text = currentDemo.Instructions;
            }
        }
        OpenGlDemoForm form;

        private void bRunDemo_Click(object sender, EventArgs e)
        {
            if (currentDemo != null)
            {
                try
                {
                    AppDomain domain = AppDomain.CreateDomain("demoDomain");
                    domain.ExecuteAssembly("WindowsDriver.exe",
                        new string[] { currentDemo.GetType().FullName });
                    AppDomain.Unload(domain);
                    //form = new OpenGlDemoForm(currentDemo.CreateNew());
                    //form.Run();
                    //form.ShowDialog();
                }
                catch(Exception ex) 
                {
                    AdvanceSystem.Forms.ErrorBox.DisplayError(ex);
                    /*MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
                    if (form != null)
                    {
                        form.Close();
                        form = null;
                    }
                    while ((ex = ex.InnerException) != null)
                    {
                        MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
                    }*/
                }
            }
        }
    }
}