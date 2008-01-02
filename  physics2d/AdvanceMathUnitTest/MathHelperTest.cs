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
    public class MathHelperTest
    {
        [SetUp]
        public void Init()
        {

        }
        [Test]
        public void Clamp()
        {
            Assert.AreEqual(0, MathHelper.Clamp(-3, 0, 1),"1");
            Assert.AreEqual(0, MathHelper.Clamp(90, -9, 0), "2");
            Assert.AreEqual(0, MathHelper.Clamp(0, -9, 6), "3");
        }
        [Test]
        public void WrapClamp()
        {
            Assert.AreEqual(1, MathHelper.WrapClamp(-3, 0, 4), "1");
            Assert.AreEqual(-10, MathHelper.WrapClamp(20, -10, 0), "2");
            Assert.AreEqual(0, MathHelper.WrapClamp(0, -9, 6), "3");
            Assert.AreEqual(0, MathHelper.WrapClamp(10, 0, 10), "4");
            Assert.AreEqual(0, MathHelper.WrapClamp(-10, 0, 10), "5");
            Assert.AreEqual(0, MathHelper.WrapClamp(2, -1, 1), "6");
            Assert.AreEqual(0, MathHelper.WrapClamp(2 * 10, -1, 1), "7");
            Assert.AreEqual(-.5f, MathHelper.WrapClamp(1.5f, -1, 1), "8");
        }
        [Test]
        public void ClampAngle()
        {
            Assert.AreEqual(0, MathHelper.ClampAngle(MathHelper.TwoPi), "1");
            Assert.AreEqual(MathHelper.Pi, MathHelper.ClampAngle(MathHelper.Pi * 3), UnitHelper.Tolerance, "2");
            Assert.AreEqual(MathHelper.PiOver2, MathHelper.ClampAngle(MathHelper.TwoPi + MathHelper.PiOver2), UnitHelper.Tolerance, "3");
            Assert.AreEqual(-MathHelper.PiOver2, MathHelper.ClampAngle(MathHelper.ThreePiOver2), UnitHelper.Tolerance, "4");
        }
        [Test]
        public void Lerp()
        {
            Assert.AreEqual(1, MathHelper.Lerp(0, 1, 1));
            Assert.AreEqual(0, MathHelper.Lerp(0, 1, 0));
            Assert.AreEqual(.5f, MathHelper.Lerp(0, 1, .5f));
        }
        [Test]
        public void TrySolveQuadratic()
        {
            Scalar a = 1;
            Scalar b = 0;
            Scalar c = -1;
            Scalar minus, plus;
            if (MathHelper.TrySolveQuadratic(a, b, c, out plus,out minus))
            {
                Assert.AreEqual(1, plus, "1 plus");
                Assert.AreEqual(-1, minus, "1 minus");
            }
            else
            {
                Assert.Fail("1");
            }
            a = 0;
            b = 1;
            c = -1;
            if (MathHelper.TrySolveQuadratic(a, b, c, out plus, out minus))
            {
                Assert.AreEqual(1, plus, "2 plus");
                Assert.AreEqual(1, minus, "2 minus");
            }
            else
            {
                Assert.Fail("2");
            }
            a = 1;
            b = 0;
            c = 0;
            if (MathHelper.TrySolveQuadratic(a, b, c, out plus, out minus))
            {
                Assert.AreEqual(0, plus, "3 plus");
                Assert.AreEqual(0, minus, "3 minus");
            }
            else
            {
                Assert.Fail("3");
            }
            a = 0;
            b = 1;
            c = 0;
            if (MathHelper.TrySolveQuadratic(a, b, c, out plus, out minus))
            {
                Assert.AreEqual(0, plus, "4 plus");
                Assert.AreEqual(0, minus, "4 minus");
            }
            else
            {
                Assert.Fail("4");
            }
        }
        [Test]
        public void Min()
        {
            Assert.AreEqual(1, MathHelper.Min(8, 5, 1, 8, 3, 4, 6, 2), "1");
            Assert.IsNaN(MathHelper.Min(8, 5, 1, Scalar.NaN, 3, 4, 6, 2), "2");
        }
        [Test]
        public void Max()
        {
            Assert.AreEqual(8, MathHelper.Max(8, 5, 1, 8, 3, 4, 6, 2), "1");
            Assert.IsNaN(MathHelper.Max(8, 5, 1, Scalar.NaN, 3, 4, 6, 2), "2");
        }
    }
}