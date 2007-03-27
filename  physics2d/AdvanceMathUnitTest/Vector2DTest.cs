
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
    public class Vector2DTest
    {
        static Random rand = new Random();
        const Scalar Value = 600;
        Vector2D op1;
        Vector2D op2;
        [SetUp]
        public void Init()
        {
            op1 = new Vector2D(Value, -Value);
            op2 = new Vector2D(-Value, Value);
        }
        [Test]
        public void Equals()
        {
            Assert.IsTrue(Vector2D.Zero != Vector2D.YAxis);
            Assert.IsFalse(Vector2D.Zero == Vector2D.YAxis);
            Assert.AreEqual(Vector2D.Zero, Vector2D.Zero);
            Assert.AreNotEqual(Vector2D.Zero, Vector2D.YAxis);
        }
        [Test]
        public void Addition()
        {
            Assert.AreEqual(Vector2D.Zero, op1 + op2, "Operator");
            Assert.AreEqual(Vector2D.Zero, Vector2D.Add(op1, op2), "Method");
            UnitHelper.RefOperationTester<Vector2D>(op1, op2, Vector2D.Zero, Vector2D.Add, "Addition");
        }
        [Test]
        public void Subtraction()
        {
            Assert.AreEqual(op1 - op1, Vector2D.Zero, "Operator");
            Assert.AreEqual(Vector2D.Zero, Vector2D.Subtract(op1, op1), "Method");
            UnitHelper.RefOperationTester<Vector2D>(op1, op2, op1 - op2, Vector2D.Subtract,"Subtraction");
        }
        [Test]
        public void Dot()
        {
            Scalar expected = -(Value * Value * 2);
            UnitHelper.RefOperationTester<Vector2D, Vector2D, Scalar>(op1, op2, expected, Vector2D.Dot,"Dot");
            Assert.AreEqual(expected,op1 * op2,"Operator" );
            Assert.AreEqual(expected,Vector2D.Dot(op1, op2),"Method" );
        }
        [Test]
        public void Angle()
        {
            Scalar angle = MathHelper.PI;
            Scalar magnitude = 2;
            Vector2D test = Vector2D.FromLengthAndAngle(magnitude, angle);
            Assert.AreEqual(angle, test.Angle, "angle");
            angle = MathHelper.HALF_PI;
            test = Vector2D.SetAngle(test, angle);
            Assert.AreEqual(angle, test.Angle, .000001f, "angle2");
            Assert.AreEqual(magnitude, test.Magnitude, "mag");
        }
        [Test]
        public void Magnitude()
        {
            Scalar expected = MathHelper.Sqrt(Value * Value * 2);
            Assert.AreEqual(expected, op1.Magnitude, UnitHelper.Tolerance, "magnitude 1");
            Vector2D test = op1;
            expected = 6;
            test = Vector2D.SetMagnitude(test, expected);
            Assert.AreEqual(expected, test.Magnitude, UnitHelper.Tolerance, "magnitude 2");
            expected = 1;
            Vector2D.Normalize(ref test, out test);
            Assert.AreEqual(expected, test.Magnitude, UnitHelper.Tolerance, "magnitude 3");
        }
        [Test]
        public void Transform()
        {
            Matrix2x2 matrix2x2 = Matrix2x2.FromRotation(MathHelper.PI);
            Assert.AreEqual(matrix2x2 * Vector2D.XAxis, Vector2D.Transform(matrix2x2, Vector2D.XAxis), "matrix2x2 rotation");
            UnitHelper.RefOperationTesterRightSame<Matrix2x2, Vector2D>(matrix2x2, Vector2D.XAxis, matrix2x2 * Vector2D.XAxis, Vector2D.Transform, "matrix2x2 rotation");
            Matrix3x3 matrix3x3 = Matrix3x3.FromTranslate2D(-op1);
            Assert.AreEqual((matrix3x3 * op1), Vector2D.Transform(matrix3x3, op1), "matrix3x3 Translate");
            UnitHelper.RefOperationTesterRightSame<Matrix3x3, Vector2D>(matrix3x3, op1, matrix3x3 * op1, Vector2D.Transform, "matrix3x3 Translate");
            matrix3x3 = Matrix3x3.FromRotationZ(MathHelper.PI);
            Assert.AreEqual((matrix3x3 * op1), Vector2D.Transform(matrix3x3, op1), "matrix3x3 rotation");
            UnitHelper.RefOperationTesterRightSame<Matrix3x3, Vector2D>(matrix3x3, op1, matrix3x3 * op1, Vector2D.Transform, "matrix3x3 rotation");
        }
        [Test]
        public void Min()
        {
            Assert.AreEqual(new Vector2D(-Value, -Value), Vector2D.Min(op1, op2),"1");
            UnitHelper.RefOperationTester<Vector2D>(op1, op2, new Vector2D(-Value, -Value), Vector2D.Min, "2");
        }
        [Test]
        public void Max()
        {
            Assert.AreEqual(new Vector2D(Value, Value), Vector2D.Max(op1, op2),"1");
            UnitHelper.RefOperationTester<Vector2D>(op1, op2, new Vector2D(Value, Value), Vector2D.Max, "2");
        }
        [Test]
        public void ZCross()
        {
            Assert.AreEqual(0, Vector2D.ZCross(op1, op2), "1");
            Assert.AreEqual(Vector2D.Zero, Vector2D.ZCross(op1, 0), "2");
            Assert.AreEqual(Vector2D.Zero, Vector2D.ZCross(0, op2), "3");
            Assert.AreEqual(new Vector2D(-Value, -Value), Vector2D.ZCross(1, op2), "4");
            Assert.AreEqual(new Vector2D(Value, Value), Vector2D.ZCross(1, op1), "5");
            Assert.AreEqual(new Vector2D(-Value, -Value), Vector2D.ZCross(op1, 1), "6");
            Assert.AreEqual(new Vector2D(Value, Value), Vector2D.ZCross(op2, 1), "7");

            UnitHelper.RefOperationTester<Vector2D, Vector2D, Scalar>(op1, op2, 0, Vector2D.ZCross, "8");
            UnitHelper.RefOperationTester<Vector2D, Scalar, Vector2D>(op1, 1, new Vector2D(-Value, -Value), Vector2D.ZCross, "9");
            UnitHelper.RefOperationTester<Scalar, Vector2D, Vector2D>(1, op2, new Vector2D(-Value, -Value), Vector2D.ZCross, "10");
        }
        [Test]
        public void Multiplication()
        {
            Vector2D expected = op1;
            expected.X *= 2;
            expected.Y *= 2;
            UnitHelper.RefOperationTesterLeftSame<Scalar, Vector2D>(op1, 2, expected, Vector2D.Multiply, "1");
            UnitHelper.RefOperationTesterRightSame<Scalar, Vector2D>(2, op1, expected, Vector2D.Multiply, "2");
            Assert.AreEqual(expected, Vector2D.Multiply(2, op1), "3");
            Assert.AreEqual(expected, Vector2D.Multiply(op1, 2), "4");
            Assert.AreEqual(expected, 2 * op1, "5");
            Assert.AreEqual(expected, op1 * 2, "6");
        }
    }
}
