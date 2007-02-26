#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion




using System;
using System.Collections.Generic;
using System.Text;

namespace Physics2DDemo
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Physics2D Demo");

            Console.WriteLine("In the demo pressing the number keys will load different demos.");
            Console.WriteLine("Left clicking on the screen will launch a projectile where you click.");
            Console.WriteLine("Right clicking and holding will shoot out particles where you click.");
            Console.WriteLine("The arrow keys will control one of the objects on the screen");
            Console.WriteLine("In the upper left corner a small colored box will appear.");
            Console.WriteLine("If it is green then the engine has too little to do and is skipping a timestep");
            Console.WriteLine("If it is red then the engine has too much to do.");
            Console.WriteLine("After each demo loads it prints to the console how many objects are in the demo.");
            Console.WriteLine("Press Enter To Start");
            Console.ReadLine();
            Demo demo = new Demo();
            OpenGlWindow g = new OpenGlWindow(1024,768);
            g.DrawCallback  = demo.Draw;
            g.Run();
        }
    }
}
