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
    public class BoundingRectangleTest
    {
        BoundingCircle circle1;
        BoundingCircle circle2;
        BoundingCircle circle3;

        BoundingRectangle rect0;
        BoundingRectangle rect1;
        BoundingRectangle rect2;

        BoundingPolygon polygon1;
       // BoundingPolygon polygon2;

        [SetUp]
        public void Init()
        {
            circle1 = new BoundingCircle(Vector2D.YAxis, 2);
            circle2 = new BoundingCircle(Vector2D.XAxis, 2);
            rect1 = BoundingRectangle.FromCircle(circle1);
            circle3 = BoundingCircle.FromRectangle(rect1);
            rect2 = BoundingRectangle.FromCircle(circle3);
            polygon1 = new BoundingPolygon(rect1.Corners());
            rect0 = new BoundingRectangle(-1, 3.01f, 1, 6);
        }
        [Test]
        public void ContainsCircle()
        {
            Assert.AreEqual(ContainmentType.Contains, rect2.Contains(circle3), "1");
            Assert.AreEqual(ContainmentType.Intersects, rect1.Contains(circle3), "2");
            Assert.AreEqual(ContainmentType.Intersects, rect1.Contains(circle2), "3");
            Assert.AreEqual(ContainmentType.Disjoint, rect0.Contains(circle1), "4");
        }
        [Test]
        public void ContainsPoint()
        {
            BoundingRectangle rect = new BoundingRectangle(0, 0, 2, 2);
            Assert.AreEqual(ContainmentType.Contains, rect.Contains(new Vector2D(1, 1)), "1");
            Assert.AreEqual(ContainmentType.Contains, rect.Contains(new Vector2D(2, 2)), "2");
            Assert.AreEqual(ContainmentType.Contains, rect.Contains(new Vector2D(0, 2)), "3");
            Assert.AreEqual(ContainmentType.Contains, rect.Contains(new Vector2D(0, 0)), "4");
            Assert.AreEqual(ContainmentType.Disjoint, rect.Contains(new Vector2D(2, 3)), "5");
            Assert.AreEqual(ContainmentType.Disjoint, rect.Contains(new Vector2D(-1, 0)), "6");
            Assert.AreEqual(ContainmentType.Disjoint, rect.Contains(new Vector2D(-.0001f, 0)), "7");
            Assert.AreEqual(ContainmentType.Disjoint, rect.Contains(new Vector2D(3, 1)), "8");
            Assert.AreEqual(ContainmentType.Disjoint, rect.Contains(new Vector2D(1, -1)), "9");
        }
        [Test]
        public void ContainsRectangle()
        {
            Assert.AreEqual(ContainmentType.Contains, rect0.Contains(rect0), "1");
            Assert.AreEqual(ContainmentType.Contains, rect1.Contains(rect1), "2");
            Assert.AreEqual(ContainmentType.Contains, rect2.Contains(rect2), "3");
            Assert.AreEqual(ContainmentType.Contains, rect2.Contains(rect1), "4");
            Assert.AreEqual(ContainmentType.Disjoint, rect0.Contains(rect1), "5");
            Assert.AreEqual(ContainmentType.Intersects, rect1.Contains(rect2), "6");
        }
        [Test]
        public void ContainsPolygon()
        {
            Assert.Fail("Not Implimented");
        }
        [Test]
        public void IntersectsRectangle()
        {
            Assert.Fail("Not Implimented");
        }
        [Test]
        public void IntersectsCircle()
        {
            Assert.Fail("Not Implimented");
        }
        [Test]
        public void IntersectsPolygon()
        {
            Assert.Fail("Not Implimented");
        }
    }

}