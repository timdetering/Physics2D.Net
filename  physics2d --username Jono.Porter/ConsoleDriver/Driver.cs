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
        TimeSpan looptime;
        TimeSpan time1;
        TimeSpan time2;



        public TimeTester(int loops, ThreadStart method1, ThreadStart method2)
        {
            this.loops = loops;
            this.method1 = method1;
            this.method2 = method2;
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
            looptime = RunLoop(EmptyLoop);
            time1 = RunLoop(method1);
            time2 = RunLoop(method2);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
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
        private  TimeSpan RunLoop(ThreadStart method)
        {
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

        static Vector2D testv = new Vector2D();
        static Vector2D testr = new Vector2D();
        static float scal = 2;
        static void TEST1()
        {
            testr = Vector2D.Multiply(testv, scal);
        }
        static void TEST2()
        {
            //testr = Vector2D.Multiply2(testv, scal);
        }



        static Random rand = new Random();
        static Stopwatch stopwatch = new Stopwatch();

        static void nothering(object obj) { }

        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            float x = 2;
            float y = 2;
            float z = +x * +y;
            Console.WriteLine(z);






            Vector2D[] loc = new Vector2D[] { new Vector2D(0, 0), new Vector2D(10, 0) };
            Vector2D[] loc2 = Polygon.Subdivide(loc, 2);


            ALVector2D alv = new ALVector2D(MathHelper.PI / 2, new Vector2D(50, 100));
            Matrix2D matrix;
            Matrix2D.FromALVector2D(ref alv, out matrix);

            Vector2D vector = new Vector2D(0,10);
            Console.WriteLine(vector);
            Vector2D r1 = matrix.NormalMatrix * vector;
            Vector2D r2 = matrix.VertexMatrix * vector;
            Console.WriteLine(r1);
            Console.WriteLine(r2);
            Matrix2D matrixInv;
            Matrix2D.Invert(ref matrix, out matrixInv);
            Console.WriteLine(matrixInv.NormalMatrix * r1);
            Console.WriteLine(matrixInv.VertexMatrix * r2);


            Polygon polygon = new Polygon(Polygon.CreateRectangle(20, 20), 2);
            Circle circle = new Circle(10,8);
            IntersectionInfo info;
            if (circle.TryGetIntersection(new Vector2D(0,10), out info) || info != null)
            {
                Console.WriteLine("circle");
                Console.WriteLine(info.Normal);
                Console.WriteLine(info.Distance);
            }
            if (polygon.TryGetIntersection(new Vector2D(3, 5), out info) || info != null)
            {
                Console.WriteLine("polygon");
                Console.WriteLine(info.Normal);
                Console.WriteLine(info.Distance);
            }

            Console.ReadLine();


/*
            int[,,] arr = new int[,,] 
{ 
{ { 1, 2 }, { 3, 4 } }, 
{ { 5, 6}, { 7,8 } }
};

            int[,,] arr3 = new int[2, 2,3];
            Array.Copy(arr, arr3, 4);
            //Functions.ArrayCopy(arr, new int[] { 0, 0,0 }, arr3, new int[] { 0, 0,0 }, new int[] { 2, 2,2 });
            int[,,] arr2 = (int[,,])Functions.ArrayRemoveRange(arr, 0, 1, 0);


            return;*/


          /*  Form1 gggg = new Form1();
            gggg.propertyGrid1.SelectedObject = new TESTObj();
            Application.Run(gggg);
            return;
            TimeTester test = new TimeTester(100000000, TEST1, TEST2);
            test.Run();
            Console.WriteLine(test);

            Polygon firstOld, firstNew, secondOld, secondNew;

            Vector2D[] shape = new Vector2D[]{new Vector2D(1,1),new Vector2D(1,-1),new Vector2D(-1,-1),new Vector2D(-1,1)};
            //Array.Reverse(shape);
            firstOld = new Polygon(shape);
            firstNew = (Polygon)firstOld.Clone();

            secondOld = new Polygon(shape);
            secondNew = (Polygon)secondOld.Clone();

            ALVector2D pos = new ALVector2D();

            pos.Linear = new Vector2D(20, 0);
            firstOld.ApplyMatrix(pos.ToMatrix2D());
           // pos.Angular = .01f;
            secondNew.ApplyMatrix(pos.ToMatrix2D());
            pos.Linear = new Vector2D(-20, 0);
            firstNew.ApplyMatrix(pos.ToMatrix2D());
            //pos.Angular = .02f;
            secondOld.ApplyMatrix(pos.ToMatrix2D());
            HorrableNarrowPhase phase = new HorrableNarrowPhase();
            Matrix2D g = pos.ToMatrix2D();
            Vector2D tt = new Vector2D();
            Vector2D.Multiply(ref g.VertexMatrix, ref tt, out tt);

            CollisionInfo info = phase.TestCollision(1, firstOld, firstNew, secondOld, secondNew);
            nothering(info);
            nothering(tt);*/
            Console.WriteLine("Finished");
            Console.ReadLine();
        }


    }
}
