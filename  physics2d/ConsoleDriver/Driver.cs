#region MIT License
/*
 * Copyright (c) 2005-2008 Jonathan Mark Porter. http://physics2d.googlepages.com/
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


using AdvanceMath;
using System.Runtime.InteropServices;

using System.Security;
using System.Security.AccessControl;

using System.Reflection;

using System.Diagnostics;

using AdvanceMath.Design;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Joints;

#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
namespace ConsoleDriver
{
    [global::System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class OperationFactoryAttribute : Attribute
    {
        readonly Type operation;
        readonly Type left;
        readonly Type right;
        public OperationFactoryAttribute(Type operation, Type left, Type right)
        {
            this.operation = operation;
            this.left = left;
            this.right = right;
        }
    }
    public interface IOperationSubFactory<TInput, TOuput>
    {
        IOperation<TOuput> Create(TInput left,TInput right);
    }
    public interface IOperation<TOuput>
    {
        TOuput Execute();
    }
    public static class OperationFactory<TInput, TOuput>
    {
        static Dictionary<Type, Dictionary<Type, Dictionary<Type, IOperationSubFactory<TInput, TOuput>>>> cache;
        static OperationFactory()
        {
            cache = new Dictionary<Type, Dictionary<Type, Dictionary<Type, IOperationSubFactory<TInput, TOuput>>>>();
        }
        public static SpecificOperationFactory<TInput, TOuput> CreateFactory(Type operation)
        {
            Dictionary<Type, Dictionary<Type, IOperationSubFactory<TInput, TOuput>>> cache2;
            if (cache.TryGetValue(operation, out cache2))
            {
                return new SpecificOperationFactory<TInput, TOuput>(operation,cache2);
            }
            else
            {
                return null;
            }
        }
        public static bool TryCreate(Type operation, TInput left, TInput right,out IOperation<TOuput> result)
        {
            Dictionary<Type, Dictionary<Type, IOperationSubFactory<TInput, TOuput>>> cache2;
            Dictionary<Type, IOperationSubFactory<TInput, TOuput>> cache3;
            IOperationSubFactory<TInput, TOuput> factory;
            result = default(IOperation<TOuput>);
            if (cache.TryGetValue(operation, out cache2))
            {
                if (cache2.TryGetValue(left.GetType(), out cache3))
                {
                    if (cache3.TryGetValue(right.GetType(), out factory))
                    {
                        result = factory.Create(left, right);
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public class SpecificOperationFactory<TInput, TOuput>
    {
        Type operation;
        Dictionary<Type, Dictionary<Type, IOperationSubFactory<TInput, TOuput>>> cache;
        internal SpecificOperationFactory(Type operation, Dictionary<Type, Dictionary<Type, IOperationSubFactory<TInput, TOuput>>> cache)
        {
            this.operation = operation;
            this.cache = cache;
        }
        public Type Operation
        {
            get { return operation; }
        }
        public bool TryCreate(Type operation, TInput left, TInput right, out IOperation<TOuput> result)
        {
            Dictionary<Type, IOperationSubFactory<TInput, TOuput>> cache2;
            IOperationSubFactory<TInput, TOuput> factory;
            result = default(IOperation<TOuput>);
            if (cache.TryGetValue(left.GetType(), out cache2))
            {
                if (cache2.TryGetValue(right.GetType(), out factory))
                {
                    result = factory.Create(left, right);
                    return true;
                }
            }
            return false;
        }
    }



    public class TestSubClass
    {
        public Scalar member;
    }
    public class TestClass
    {
        public Scalar local;
        public TestSubClass B;

        public Scalar Test(Scalar arg, TestSubClass C)
        {
            Scalar stack = 1;
            return stack * arg * local * B.member * C.member;
        }

        public Scalar Test2(Scalar arg, TestSubClass C)
        {
            Scalar stack = 1;
            Scalar temp1 = arg;
            Scalar temp2 = local;
            Scalar temp3 = B.member;
            Scalar temp4 = C.member;
            return stack * temp1 * temp2 * temp3 * temp4;
        }
    }


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


        static void TestMethod(int c, int d)
        {
            int a, b;
            a = 40;
            b = (c + d) * a;
            a = rand.Next();
            a = b + d * a + a;
        }


        static Random rand = new Random();
        static Stopwatch stopwatch = new Stopwatch();

        static void nothering(object obj) { }


        [STAThread]
        static void Main(string[] args)
        {



            PhysicsEngine engine = new PhysicsEngine();
            engine.BroadPhase = new Physics2DDotNet.Detectors.SelectiveSweepDetector();
            engine.Solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();

            PhysicsTimer timer = new PhysicsTimer(engine.Update, .01f);
            timer.IsRunning = true;



            Coefficients coffecients = new Coefficients(/*restitution*/1, /*friction*/.5f);


            IShape shape1 = new CircleShape(8, 7);
            IShape shape2 = new PolygonShape(VertexHelper.CreateRectangle(20, 10), 3);

            Scalar mass = 5;
            Body body1 = new Body(new PhysicsState(), shape1, mass, coffecients, new Lifespan());
            Body body2 = new Body(new PhysicsState(), shape2, mass, coffecients, new Lifespan());

            engine.AddBody(body1);
            engine.AddBody(body2);
            Joint joint = new HingeJoint(body1, body2, Vector2D.Zero, new Lifespan());
            engine.AddJoint(joint);
            joint.Lifetime.IsExpired = true;

            engine.AddJoint(new HingeJoint(body1, body2, Vector2D.Zero, new Lifespan()));
            engine.Update(0);

            body1.Lifetime.IsExpired = true;

            timer.IsRunning = false;
            engine.AddProxy(body1, body2, Matrix2x2.Identity);
            //  b1.RemoveFromProxy();



            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, engine);
            stream.Seek(0, SeekOrigin.Begin);
            PhysicsEngine engine2 = (PhysicsEngine)formatter.Deserialize(stream);





            Console.WriteLine();

            /*

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
            */
            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
