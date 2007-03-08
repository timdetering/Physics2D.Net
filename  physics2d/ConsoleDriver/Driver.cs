#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

//using Physics2D.CollisionDetection;
using Physics2DDotNet.Math2D;
using AdvanceMath;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Security;
using System.Security.AccessControl;

//using ExceptionEventHandler = System.EventHandler<ConsoleDriver.ExceptionEventArgs>;
using System.Reflection;
using System.Diagnostics;

using AdvanceMath.Design;
using Physics2DDotNet;
namespace ConsoleDriver
{
    public class TimeTester
    {
        Stopwatch stopwatch = new Stopwatch();
        int loops;
        ThreadStart method1;
        ThreadStart method2;
        ThreadStart reset;
        TimeSpan looptime;
        TimeSpan time1;
        TimeSpan time2;


        public TimeTester(int loops, ThreadStart method1, ThreadStart method2)
            : this(loops, method1, method2, null)
        { }
        public TimeTester(int loops, ThreadStart method1, ThreadStart method2, ThreadStart reset)
        {
            this.loops = loops;
            this.method1 = method1;
            this.method2 = method2;
            this.reset = reset;
        }

        public TimeSpan EmptyLoop1
        {
            get { return looptime; }
        }
        public TimeSpan Loop1
        {
            get { return time1; }
        }
        public TimeSpan Loop2
        {
            get { return time2; }
        }

        public void Run()
        {
            method1();
            method2();
            if (reset != null) { reset(); }
            GC.Collect();
            Thread.Sleep(15);
            looptime = RunLoop(EmptyLoop);
            GC.Collect();
            Thread.Sleep(15);
            time1 = RunLoop(method1);
            if (reset != null) { reset(); }
            GC.Collect();
            Thread.Sleep(15);
            time2 = RunLoop(method2);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("LoopCount: {0}\n", loops);
            builder.AppendFormat("Empty Loop: {0}\n", looptime);
            builder.AppendFormat("Method1: {0}\n", time1);
            builder.AppendFormat("Method2: {0}\n", time2);
            if (time1.Ticks < time2.Ticks)
            {
                builder.AppendLine("Method1 is faster.");
                builder.AppendFormat("{0:#.} Percent of Method2\n", Math.Round(((double)time1.Ticks / (double)time2.Ticks) * 100, 2));
            }
            else
            {
                builder.AppendLine("Method2 is faster.");
                builder.AppendFormat("{0:#.} Percent of Method1\n", Math.Round(((double)time2.Ticks / (double)time1.Ticks) * 100, 2));
            }
            builder.AppendFormat("{0:#.} Percent of Method1 is loop\n", (Math.Round(((double)looptime.Ticks / (double)time1.Ticks) * 100, 2)));
            builder.AppendFormat("{0:#.} Percent of Method2 is loop\n", (Math.Round(((double)looptime.Ticks / (double)time2.Ticks) * 100, 2)));
            return builder.ToString();
        }

        private void EmptyLoop() { }
        private TimeSpan RunLoop(ThreadStart method)
        {
            method();
            stopwatch.Reset();
            stopwatch.Start();
            for (int loop = 0; loop < loops; ++loop)
            {
                method();
            }
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }

    static class Driver
    {
        static long RoundUpToPowerOfTwo(long value)
        {
            long result = 1;
            while (result < value) { result <<= 1; }
            return result;
        }
        static int RoundUpToPowerOfTwo(int value)
        {
            int result = 1;
            while (result < value) { result <<= 1; }
            return result;
        }


        static Random rand = new Random();
        static Stopwatch stopwatch = new Stopwatch();

        static void nothering(object obj) { }
        [STAThread]
        static void Main(string[] args)
        {


            //Console.WriteLine(Math.Floor(1.1));

            float angle = -MathHelper.PI *3;
            angle = MathHelper.ClampAngle(angle);
            Console.WriteLine(angle / MathHelper.TWO_PI);


             angle = MathHelper.AngleSubtract(MathHelper.PI, 0);

           // Console.WriteLine(angle / MathHelper.TWO_PI);



            //int value = 87000;
            //Console.WriteLine(RoundUpToPowerOfTwo(value));


           /* TimeTester test = new TimeTester(30000000, TEST1, TEST2);
            test.Run();
            Console.WriteLine(test);*/

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
