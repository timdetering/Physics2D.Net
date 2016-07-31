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
    public class Vector4DTest
    {
        static Random rand = new Random();
        const Scalar Value = 600;
        Vector4D op1;
        Vector4D op2;
        [SetUp]
        public void Init()
        {
            op1 = new Vector4D(Value, -Value, Value, -Value);
            op2 = new Vector4D(-Value, Value, -Value, Value);
        }
        [Test]
        public void Equals()
        {
            Assert.IsTrue(Vector4D.Zero != Vector4D.YAxis);
            Assert.IsFalse(Vector4D.Zero == Vector4D.YAxis);
            Assert.AreEqual(Vector4D.Zero, Vector4D.Zero);
            Assert.AreNotEqual(Vector4D.Zero, Vector4D.YAxis);
        }
        [Test]
        public void Addition()
        {
            Assert.AreEqual(Vector4D.Zero, op1 + op2, "Operator");
            Assert.AreEqual(Vector4D.Zero, Vector4D.Add(op1, op2), "Method");
            UnitHelper.RefOperationTester<Vector4D>(op1, op2, Vector4D.Zero, Vector4D.Add, "Addition");
        }
        [Test]
        public void Subtraction()
        {
            Assert.AreEqual(op1 - op1, Vector4D.Zero, "Operator");
            Assert.AreEqual(Vector4D.Zero, Vector4D.Subtract(op1, op1), "Method");
            UnitHelper.RefOperationTester<Vector4D>(op1, op2, op1 - op2, Vector4D.Subtract,"Subtraction");
        }
        [Test]
        public void Dot()
        {
            Scalar expected = -(Value * Value * 4);
            UnitHelper.RefOperationTester<Vector4D, Vector4D, Scalar>(op1, op2, expected, Vector4D.Dot,"Dot");
            Assert.AreEqual(expected,op1 * op2,"Operator" );
            Assert.AreEqual(expected,Vector4D.Dot(op1, op2),"Method" );
        }
        [Test]
        public void Magnitude()
        {
            Scalar expected = MathHelper.Sqrt(Value * Value * 4);
            Assert.AreEqual(expected, op1.Magnitude, UnitHelper.Tolerance, "magnitude 1");
            Vector4D test = op1;
            expected = 6;
            test = Vector4D.SetMagnitude(test, expected);
            Assert.AreEqual(expected, test.Magnitude, UnitHelper.Tolerance, "magnitude 2");
            expected = 1;
            Vector4D.Normalize(ref test, out test);
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
            Assert.AreEqual(new Vector4D(-Value, -Value, -Value, -Value), Vector4D.Min(op1, op2), "1");
            UnitHelper.RefOperationTester<Vector4D>(op1, op2, new Vector4D(-Value, -Value, -Value, -Value), Vector4D.Min, "2");
        }
        [Test]
        public void Max()
        {
            Assert.AreEqual(new Vector4D(Value, Value, Value, Value), Vector4D.Max(op1, op2), "1");
            UnitHelper.RefOperationTester<Vector4D>(op1, op2, new Vector4D(Value, Value, Value, Value), Vector4D.Max, "2");
        }
        [Test]
        public void Multiplication()
        {
            Vector4D expected = op1;
            expected.X *= 2;
            expected.Y *= 2;
            expected.Z *= 2;
            expected.W *= 2;
            UnitHelper.RefOperationTesterLeftSame<Scalar, Vector4D>(op1, 2, expected, Vector4D.Multiply, "1");
            UnitHelper.RefOperationTesterRightSame<Scalar, Vector4D>(2, op1, expected, Vector4D.Multiply, "2");
            Assert.AreEqual(expected, Vector4D.Multiply(2, op1), "3");
            Assert.AreEqual(expected, Vector4D.Multiply(op1, 2), "4");
            Assert.AreEqual(expected, 2 * op1, "5");
            Assert.AreEqual(expected, op1 * 2, "6");
        }
    }
}
