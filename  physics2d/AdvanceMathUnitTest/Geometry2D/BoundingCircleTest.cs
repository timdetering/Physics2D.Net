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



namespace AdvanceMath.Geometry2D.UnitTest
{
    [TestFixture]
    public class BoundingCircleTest
    {
        BoundingCircle circle1;
        BoundingCircle circle2;
        BoundingCircle circle3;

        BoundingRectangle rect1;
        BoundingRectangle rect2;

        BoundingPolygon polygon1;
        BoundingPolygon polygon2;

        [SetUp]
        public void Init()
        {
            circle1 = new BoundingCircle(Vector2D.YAxis, 2);
            circle2 = new BoundingCircle(Vector2D.XAxis, 2);
            rect1 = BoundingRectangle.FromCircle(circle1);
            circle3 = BoundingCircle.FromRectangle(rect1);
            rect2 = BoundingRectangle.FromCircle(circle3);
            polygon1 = new BoundingPolygon(rect1.Corners());
        }
        [Test]
        public void ContainsCircle()
        {
            Assert.IsTrue(circle1.Contains(circle1) == ContainmentType.Contains,"1");
            Assert.IsFalse(circle1.Contains(circle2) == ContainmentType.Contains, "2");
            Assert.IsTrue(circle3.Contains(circle1) == ContainmentType.Contains, "3");
            Assert.IsFalse(circle1.Contains(circle3) == ContainmentType.Contains, "4");
        }
        [Test]
        public void ContainsPoint()
        {
            Assert.IsTrue(circle1.Contains(Vector2D.YAxis + new Vector2D(2, 0)) == ContainmentType.Contains, "1");
            Assert.IsTrue(circle1.Contains(Vector2D.YAxis + new Vector2D(0, 2)) == ContainmentType.Contains, "2");
            Assert.IsFalse(circle1.Contains(Vector2D.YAxis + new Vector2D(1, 2)) == ContainmentType.Contains, "3");
            Assert.IsTrue(circle1.Contains(Vector2D.YAxis) == ContainmentType.Contains, "4");
        }
        [Test]
        public void ContainsRectangle()
        {
            Assert.IsTrue(circle3.Contains(rect1) == ContainmentType.Contains, "1");
            Assert.IsFalse(circle1.Contains(rect1) == ContainmentType.Contains, "2");
        }
        [Test]
        public void ContainsPolygon()
        {
            Assert.IsTrue(circle3.Contains(polygon1) == ContainmentType.Contains, "1");
            Assert.IsFalse(circle1.Contains(polygon1) == ContainmentType.Contains, "2");
        }
        [Test]
        public void IntersectsRectangle()
        {
            Assert.IsTrue(circle3.Intersects(rect1), "1");
            Assert.IsTrue(circle2.Intersects(rect1), "2");
            BoundingCircle c = new BoundingCircle(5, 5, 1);
            Assert.IsFalse(c.Intersects(rect2), "1");
            Assert.IsFalse(c.Intersects(rect2), "2");
        }
        [Test]
        public void IntersectsCircle()
        {
            Assert.IsTrue(circle1.Intersects(circle1),"1");
            Assert.IsTrue(circle1.Intersects(circle2), "2");
            Assert.IsFalse(circle1.Intersects(new BoundingCircle(0,6,3)), "3");
        }
        [Test]
        public void IntersectsPolygon()
        {
            Assert.Fail("Not Implimented");
        }
    }

}