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
using System.Drawing;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Physics2D;
using Physics2D.CollidableAreas;
using Physics2D.Collections;
using Physics2D.CollidableBodies;
using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Security;
using System.Security.AccessControl;


using AdvanceSystem;
using AdvanceSystem.ComponentModel;
using AdvanceSystem.ComponentModel.GUI;
using AdvanceSystem.ComponentModel.Design;
//using ExceptionEventHandler = System.EventHandler<ConsoleDriver.ExceptionEventArgs>;
using System.Reflection;





namespace ConsoleDriver
{
    public class Test
    {
        //[AdvanceSystem.ComponentModel.DefaultObjectValue]
        //public static Test Zero = new Test(99);
        public Test() { }
        public Test(float bl) { bounded = new Bounded<float>(bl); }
        Bounded<float> bounded = new Bounded<float>(8);

        public Bounded<float> Bounded
        {
            get { return bounded; }
            set { bounded = value; }
        }
        Point pointf;

        public Point Pointf
        {
            get { return pointf; }
            set { pointf = value; }
        }

        Vector2D vector2D;

        public Vector2D Vector2D
        {
            get { return vector2D; }
            set { vector2D = value; }
        }
        Vector3D vector3D;

        public Vector3D Vector3D
        {
            get { return vector3D; }
            set { vector3D = value; }
        }
        Vector4D vector4D;

        [System.ComponentModel.DisplayName("FFF:")]
        public Vector4D Vector4D
        {
            get { return vector4D; }
            set { vector4D = value; }
        }
        PhysicsState state = new PhysicsState();

        public PhysicsState State
        {
            get { return state; }
            set { state = value; }
        }


        public virtual void EventHandler(object sender, EventArgs e)
        {
        }
    }




}
namespace ConsoleDriver
{



    static class Driver
    {
        static char[] array1 = "test1".ToCharArray();
        static char[] array2 = "test2".ToCharArray();
        /*static Test[] t = new Test[0];
        static event EventHandler Event;*/
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            

            /*RigidBody box = new RigidBody(MassInertia.FromSolidCylinder(1000, 100), new PhysicsState(), new Polygon2D(Polygon2D.FromNumberofSidesAndRadius(10, 100)));

            MemoryStream stream = new MemoryStream();
            BinaryFormatter formater = new BinaryFormatter();
            formater.Serialize(stream, box);
            stream.Position = 0;

            RigidBody box2 = (RigidBody)formater.Deserialize(stream);

            Nothering(box);
            Nothering(box2);*/
            SerializedXmlEditor g = new SerializedXmlEditor();
            g.Text = "Physics 2D World Editor";
            g.Width = 640;
            g.Height = 480;

            g.AllowedTypes = new TypeRelations[] { new TypeRelations(typeof(RigidBody)) };
            System.Windows.Forms.Application.Run(g);

            //TimeTester(9002);
            Console.WriteLine("Finished");
            Console.ReadLine();

            /*while (true)
            {
                Nothering();
                Thread.Sleep(1000);
            }*/
        }

        static void g2_Event(object sender, EventArgs e)
        {
            Console.WriteLine("G2");
        }

        static void g_Event(object sender, EventArgs e)
        {
            Console.WriteLine("G");
            
        }
        static void Nothering(RigidBody box2) { }
        public static void TimeTester(long loops)
        {
            DateTime start;
            DateTime end;
            Console.WriteLine("Starting: ");
            start = DateTime.Now;
            for (long pos = 0; pos < loops; ++pos)
            {
                EmptyTimeTestCall();
            }
            end = DateTime.Now;
            TimeSpan looptime = (TimeSpan)end.Subtract(start);
            Console.WriteLine("Empty loop took: " + looptime.TotalSeconds + " TotalSeconds");
            start = DateTime.Now;
            for (long pos = 0; pos < loops; ++pos)
            {
                TimeTesterMethod1();
            }
            end = DateTime.Now;
            TimeSpan time1 = (TimeSpan)end.Subtract(start);
            Console.WriteLine("Method1 took: " + time1.TotalSeconds + " TotalSeconds");
            start = DateTime.Now;
            for (long pos = 0; pos < loops; ++pos)
            {
                TimeTesterMethod2();
            }
            end = DateTime.Now;
            TimeSpan time2 = (TimeSpan)end.Subtract(start);
            Console.WriteLine("Method2 took: " + time2.TotalSeconds + " TotalSeconds");
            if (time2.Ticks > time1.Ticks)
            {
                Console.WriteLine("Method1 is faster.");
                Console.WriteLine(Math.Round(((double)time1.Ticks / (double)time2.Ticks) * 100, 2) + " Percent of Method2");
            }
            else
            {
                Console.WriteLine("Method2 is faster.");
                Console.WriteLine(Math.Round(((double)time2.Ticks / (double)time1.Ticks) * 100, 2) + " Percent of Method1");
            }
            Console.WriteLine(Math.Round(((double)looptime.Ticks / (double)time1.Ticks) * 100, 2) + " Percent of Method1 is loop");
            Console.WriteLine(Math.Round(((double)looptime.Ticks / (double)time2.Ticks) * 100, 2) + " Percent of Method2 is loop");
        }
        public static void EmptyTimeTestCall()
        { }

        static List<int> list = new List<int>();
        public static void TimeTesterMethod1()
        {
            for (int pos = 0; pos < 1000; ++pos)
            {
                list.Add(0);
                list.Add(0);
                list.RemoveAt(0);
            }
            list.Clear();
            /*int count = t.Length;
            for (int pos = 0; pos < count; pos++)
            {
                t[pos].EventHandler(.5f, null);
            }*/
            /*Test2 t2 = new Test2();
            for (int pos = 0; pos < 900; pos++)
            {
                Test nt = new Test();
                t2.Event += new EventHandler(nt.EventHandler);
            }*/
        }
        static LinkedList<int> list2 = new LinkedList<int>();
        public static void TimeTesterMethod2()
        {
            for (int pos = 0; pos < 1000; ++pos)
            {
                list2.AddLast(0);
                list2.AddLast(0);
                list2.RemoveFirst();
            }
            list2.Clear();
            //Event(.5f, null);
            /*List<Test> t2 = new List<Test>();
            for (int pos = 0; pos < 900; pos++)
            {
                Test nt = new Test();
                t2.Add(nt);
            }*/
        }
    }
}
