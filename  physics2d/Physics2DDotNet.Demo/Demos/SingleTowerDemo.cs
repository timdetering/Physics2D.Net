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
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Drawing;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Graphics2DDotNet;
using Physics2DDotNet;
using Physics2DDotNet.Ignorers;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.PhysicsLogics;
using SdlDotNet;
using SdlDotNet.Input;
using SdlDotNet.Graphics;
namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Simple", "Single Tower", "There is a single Tower of stacking blocks")]
    public class SingleTowerDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
           






            dispose += DemoHelper.BasicDemoSetup(DemoInfo);
            dispose += DemoHelper.CreateTank(DemoInfo, new Vector2D(50, 0));

            Scene.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));


            DemoHelper.AddFloor(DemoInfo, new ALVector2D(0, new Vector2D(700, 750)));

            IShape shape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("block.png"), 3, 7, 4);


            DemoHelper.AddGrid(
                DemoInfo, shape, 20,
                new BoundingRectangle(440, 450, 500, 730),
                1, 1);
            Body ball = DemoHelper.AddCircle(DemoInfo, 80, 20, 4000, new ALVector2D(0, 1028, 272));
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
    [PhysicsDemo("Simple", "MultiPolygon Demo", "There is a single Tower of stacking blocks")]
    public class MultiPolygonDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {







            dispose += DemoHelper.BasicDemoSetup(DemoInfo);
            dispose += DemoHelper.CreateTank(DemoInfo, new Vector2D(50, 0));

            Scene.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));


            DemoHelper.AddFloor(DemoInfo, new ALVector2D(0, new Vector2D(700, 750)));

            IShape shape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("block.png"), 3, 7, 4);


            DemoHelper.AddGrid(
                DemoInfo, shape, 20,
                new BoundingRectangle(440, 450, 500, 730),
                1, 1);
            Body ball = DemoHelper.AddCircle(DemoInfo, 80, 20, 4000, new ALVector2D(0, 1028, 272));

            Vector2D[][] polygons1 = new Vector2D[2][];
            polygons1[0] = VertexHelper.Subdivide(VertexHelper.CreateRectangle(50, 50), 16);
            polygons1[1] = VertexHelper.CreateCircle(30, 20);
            Matrix2x3 matrix = Matrix2x3.FromTransformation(1, new Vector2D(30, 50));

            polygons1[0] = VertexHelper.ApplyMatrix(ref matrix, polygons1[0]);

            IShape shape1 = ShapeFactory.CreateColoredMultiPolygon(polygons1, 3);
            DemoHelper.AddShape(DemoInfo, shape1, 50, new ALVector2D(0, 300, 300));


            Vector2D[][] polygons = new Vector2D[3][];
            polygons[0] = VertexHelper.Subdivide(VertexHelper.CreateRectangle(30, 50), 16);
            polygons[1] = VertexHelper.Subdivide(VertexHelper.CreateRectangle(50, 70), 16);
            polygons[2] = VertexHelper.CreateCircle(30, 20);
             matrix = Matrix2x3.FromTransformation(6, new Vector2D(36, 50));

            polygons[2] = VertexHelper.ApplyMatrix(ref matrix, polygons[2]);
            matrix = Matrix2x3.FromTransformation(-6, new Vector2D(-36, 50));

            polygons[1] = VertexHelper.ApplyMatrix(ref matrix, polygons[1]);
            IShape shape2 = ShapeFactory.CreateColoredMultiPolygon(polygons, 3);
            DemoHelper.AddShape(DemoInfo, shape2, 50, new ALVector2D(0, 400, 300));
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }


    [PhysicsDemo("111", "FreezingDemo", "There is a single Tower of stacking blocks")]
    public class FreezingDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {







            dispose += DemoHelper.BasicDemoSetup(DemoInfo);
          //  dispose += DemoHelper.CreateTank(DemoInfo, new Vector2D(50, 0));

            Scene.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));


            DemoHelper.AddFloor(DemoInfo, new ALVector2D(0, new Vector2D(700, 750)));

            IShape shape = ShapeFactory.CreateSprite(Cache<SurfacePolygons>.GetItem("block.png"), 3, 7, 4);

            DemoHelper.AddShape(DemoInfo, shape, 20, new ALVector2D(0,300, 700));

            DemoHelper.AddGrid(
                DemoInfo, shape, 20,
                new BoundingRectangle(440, 450, 500, 730),
                1, 1);


          //  Body ball = DemoHelper.AddCircle(DemoInfo, 80, 20, 4000, new ALVector2D(0, 1028, 272));
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}