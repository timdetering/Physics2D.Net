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
using System.Windows.Forms;
using WindowsDriver.Demos;
using DM = WindowsDriver.Demos;
namespace WindowsDriver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {


                IDemo[] demos = new IDemo[]
                {
                    new DM.BaseDisplayDemo(),
                    new DM.BaseControlDemo(),
                    new DM.OldDirectXTests(),
                    new DM.BoundedOldTests(),
                    new DM.ChainTest(),
                    new DM.DominoesTest(),
                    new DM.GravityFieldTest(),
                    new DM.MassNumberOfHumanBodiesTest(),
                    new DM.MassNumberTestWithRestPlace(),
                    new DM.MassNumberOfMinesTest(),
                    new DM.TwoPlayerGame(),
                    new DM.Pong(),
                    new DM.SpecialBalls(),
                    
                    new DM.HyperMeleeDemo.MissileTest(),
                    new DM.HyperMeleeDemo.UrQuanRammingTest(),
                    new DM.HyperMeleeDemo.EarthlingMissileTest(),
                    new  DM.TankDemo()
                            };









                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    Application.Run(new MainMenu(demos));

                    //Application.Run(new DemoForm(new DM.GravityFieldTest()));
                }
                catch
                {

                }
            }
            else if(args.Length == 1)
            {
                Type t = Type.GetType(args[0]);
                IDemo demo = (IDemo)t.GetConstructor(Type.EmptyTypes).Invoke(null);
                OpenGlDemoForm form = new OpenGlDemoForm(demo);
                form.Run();
            }
        }
    }
}