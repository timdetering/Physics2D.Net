
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif

using System;
using NUnit.Framework;



namespace AdvanceMath.UnitTest
{



    [TestFixture]
    public class Vector3DTest
    {
        static Random rand = new Random();
        const Scalar Value = 600;
        Vector3D op1;
        Vector3D op2;
        [SetUp]
        public void Init()
        {
            op1 = new Vector3D(Value, -Value, Value);
            op2 = new Vector3D(-Value, Value, -Value);
        }
        [Test]
        public void Equals()
        {
            Assert.IsTrue(Vector3D.Zero != Vector3D.YAxis);
            Assert.IsFalse(Vector3D.Zero == Vector3D.YAxis);
            Assert.AreEqual(Vector3D.Zero, Vector3D.Zero);
            Assert.AreNotEqual(Vector3D.Zero, Vector3D.YAxis);
        }
        [Test]
        public void Addition()
        {
            Assert.AreEqual(Vector3D.Zero, op1 + op2, "Operator");
            Assert.AreEqual(Vector3D.Zero, Vector3D.Add(op1, op2), "Method");
            UnitHelper.RefOperationTester<Vector3D>(op1, op2, Vector3D.Zero, Vector3D.Add, "Addition");
        }
        [Test]
        public void Subtraction()
        {
            Assert.AreEqual(op1 - op1, Vector3D.Zero, "Operator");
            Assert.AreEqual(Vector3D.Zero, Vector3D.Subtract(op1, op1), "Method");
            UnitHelper.RefOperationTester<Vector3D>(op1, op2, op1 - op2, Vector3D.Subtract,"Subtraction");
        }
        [Test]
        public void Dot()
        {
            Scalar expected = -(Value * Value * 3);
            UnitHelper.RefOperationTester<Vector3D, Vector3D, Scalar>(op1, op2, expected, Vector3D.Dot,"Dot");
            Assert.AreEqual(expected,op1 * op2,"Operator" );
            Assert.AreEqual(expected,Vector3D.Dot(op1, op2),"Method" );
        }
        [Test]
        public void Magnitude()
        {
            Scalar expected = MathHelper.Sqrt(Value * Value * 3);
            Assert.AreEqual(expected, op1.Magnitude, UnitHelper.Tolerance, "magnitude 1");
            Vector3D test = op1;
            expected = 6;
            test = Vector3D.SetMagnitude(test, expected);
            Assert.AreEqual(expected, test.Magnitude, UnitHelper.Tolerance, "magnitude 2");
            expected = 1;
            Vector3D.Normalize(ref test, out test);
            Assert.AreEqual(expected, test.Magnitude, UnitHelper.Tolerance, "magnitude 3");
        }
        [Test]
        public void Transform()
        {
            Assert.Fail("Not Implimented");
        }
        [Test]
        public void Min()
        {
            Assert.AreEqual(new Vector3D(-Value, -Value, -Value), Vector3D.Min(op1, op2), "1");
            UnitHelper.RefOperationTester<Vector3D>(op1, op2, new Vector3D(-Value, -Value, -Value), Vector3D.Min, "2");
        }
        [Test]
        public void Max()
        {
            Assert.AreEqual(new Vector3D(Value, Value, Value), Vector3D.Max(op1, op2), "1");
            UnitHelper.RefOperationTester<Vector3D>(op1, op2, new Vector3D(Value, Value, Value), Vector3D.Max, "2");
        }

        [Test]
        public void Multiplication()
        {
            Vector3D expected = op1;
            expected.X *= 2;
            expected.Y *= 2;
            expected.Z *= 2;
            UnitHelper.RefOperationTesterLeftSame<Scalar, Vector3D>(op1, 2, expected, Vector3D.Multiply, "1");
            UnitHelper.RefOperationTesterRightSame<Scalar, Vector3D>(2, op1, expected, Vector3D.Multiply, "2");
            Assert.AreEqual(expected, Vector3D.Multiply(2, op1), "3");
            Assert.AreEqual(expected, Vector3D.Multiply(op1, 2), "4");
            Assert.AreEqual(expected, 2 * op1, "5");
            Assert.AreEqual(expected, op1 * 2, "6");
        }
    }
}
