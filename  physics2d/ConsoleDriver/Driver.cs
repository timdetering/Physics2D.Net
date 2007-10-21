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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Physics2DDotNet.Math2D;
using AdvanceMath;
using System.Runtime.InteropServices;

using System.Security;
using System.Security.AccessControl;

using System.Reflection;

using System.Diagnostics;

using AdvanceMath.Design;
using AdvanceMath.Geometry2D;
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
        public static void TwoPassFileErase(string path, int blockSize, Random random)
        {
            if (path == null) { throw new ArgumentNullException("path"); }
            if (blockSize <= 0) { throw new ArgumentOutOfRangeException("blockSize"); }
            if (random == null) { throw new ArgumentNullException("random"); }
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Write))
            {
                Byte[] buffer = new byte[blockSize];
                long length = stream.Length;
                for (long index = 0; index < length; index += buffer.Length)
                {
                    random.NextBytes(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                }
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                Array.Clear(buffer, 0, buffer.Length);
                for (long index = 0; index < length; index += buffer.Length)
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                stream.Flush();
            }
            string newPath;
            do
            {
                newPath = Path.Combine(Path.GetDirectoryName(path), Path.GetRandomFileName());
            } while (File.Exists(newPath));
            File.Move(path, newPath);
            File.Delete(newPath);
        }


        static void TestMethod(int c, int d)
        {
            int a,b;
            a = 40;
            b = (c+ d )*a;
            a = rand.Next();
            a = b+ d *a+a;
        }


        static Random rand = new Random();
        static Stopwatch stopwatch = new Stopwatch();

        static void nothering(object obj) { }


        [STAThread]
        static void Main(string[] args)
        {




            Vector2D[] vertexes1 = new Vector2D[]
            {
                new Vector2D(-1,1),
                new Vector2D(-3,1),
                new Vector2D(-3,-1),
                new Vector2D(-1,-1),
            };
            Vector2D[] vertexes2 = new Vector2D[]
            {
                new Vector2D(1,-1),
                new Vector2D(3,-1),
                new Vector2D(3,1),
                new Vector2D(1,1),
            };
            Vector2D[][] polygons = new Vector2D[2][];
            polygons[0] = vertexes1;
            polygons[1] = vertexes2;
            Console.WriteLine(MultiPartPolygon.GetCentroid(polygons));
            Console.WriteLine(MultiPartPolygon.GetArea(polygons));

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
