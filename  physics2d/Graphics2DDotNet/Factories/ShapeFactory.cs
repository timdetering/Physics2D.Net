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
using System.Collections.ObjectModel;
using System.Text;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Tao.OpenGl;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.OpenGl;
namespace Graphics2DDotNet
{
    public static class ShapeFactory
    {
        public static ScalarColor3[] CreateColor3Array(int count)
        {
            return CreateColor3Array(new ScalarColor3(1, .5f, 0), new ScalarColor3(1, 1, 1), count);
        }
        public static ScalarColor3[] CreateColor3Array(ScalarColor3 first, ScalarColor3 rest, int count)
        {
            ScalarColor3[] result = new ScalarColor3[count];
            result[0] = first;
            for (int index = 1; index < count; ++index)
            {
                result[index] = rest;
            }
            return result;
        }
        public static ScalarColor4[] CreateColor4Array(int count)
        {
            return CreateColor4Array(new ScalarColor4(1, .5f, 0, 1), new ScalarColor4(1, 1, 1, 1), count);
        }
        public static ScalarColor4[] CreateColor4Array(ScalarColor4 first, ScalarColor4 rest, int count)
        {
            ScalarColor4[] result = new ScalarColor4[count];
            result[0] = first;
            for (int index = 1; index < count; ++index)
            {
                result[index] = rest;
            }
            return result;
        }

        public static PolygonShape CreatePolygon(Vector2D[] vertexes, Scalar gridSpacing)
        {
            PolygonShape shape = new PolygonShape(vertexes, gridSpacing);
            shape.Tag = DrawableFactory.CreatePolygon(vertexes);
            return shape;
        }
        public static PolygonShape CreatePolygon(Vector2D[] vertexes, Scalar gridSpacing, ScalarColor3[] colors)
        {
            PolygonShape shape = new PolygonShape(vertexes, gridSpacing);
            shape.Tag = DrawableFactory.CreatePolygon(vertexes, colors);
            return shape;
        }
        public static PolygonShape CreatePolygon(Vector2D[] vertexes, Scalar gridSpacing, ScalarColor4[] colors)
        {
            PolygonShape shape = new PolygonShape(vertexes, gridSpacing);
            shape.Tag = DrawableFactory.CreatePolygon(vertexes, colors);
            return shape;
        }
        public static PolygonShape CreateColoredPolygon(Vector2D[] vertexes, Scalar gridSpacing)
        {
            PolygonShape shape = new PolygonShape(vertexes, gridSpacing);
            Vector2D[] reduced = PolygonShape.Reduce(vertexes);
            shape.Tag = DrawableFactory.CreatePolygon(reduced, CreateColor3Array(reduced.Length));
            return shape;
        }

        public static CircleShape CreateCircle(Scalar radius, int vertexCount)
        {
            CircleShape shape = new CircleShape(radius, vertexCount);
            shape.Tag = DrawableFactory.CreatePolygon(shape.Vertexes);
            return shape;
        }
        public static CircleShape CreateCircle(Scalar radius, int vertexCount, ScalarColor3[] colors)
        {
            CircleShape shape = new CircleShape(radius, vertexCount);
            shape.Tag = DrawableFactory.CreatePolygon(shape.Vertexes, colors);
            return shape;
        }
        public static CircleShape CreateCircle(Scalar radius, int vertexCount, ScalarColor4[] colors)
        {
            CircleShape shape = new CircleShape(radius, vertexCount);
            shape.Tag = DrawableFactory.CreatePolygon(shape.Vertexes, colors);
            return shape;
        }
        public static CircleShape CreateColoredCircle(Scalar radius, int vertexCount)
        {
            CircleShape shape = new CircleShape(radius, vertexCount);
            shape.Tag = DrawableFactory.CreatePolygon(shape.Vertexes, CreateColor3Array(vertexCount));
            return shape;
        }

        public static Shape CreateMultiPolygon(Vector2D[][] polygons, Scalar gridSpacing)
        {
            if (polygons.Length == 1)
            {
                return CreatePolygon(polygons[0], gridSpacing);
            }
            else
            {
                MultiPolygonShape shape = new MultiPolygonShape(polygons, gridSpacing);
                shape.Tag = DrawableFactory.CreateMultiPolygon(polygons);
                return shape;
            }
        }
        public static Shape CreateMultiPolygon(Vector2D[][] polygons, Scalar gridSpacing, ScalarColor3[][] colors)
        {
            if (polygons.Length == 1)
            {
                return CreatePolygon(polygons[0], gridSpacing, colors[0]);
            }
            else
            {
                MultiPolygonShape shape = new MultiPolygonShape(polygons, gridSpacing);
                shape.Tag = DrawableFactory.CreateMultiPolygon(polygons, colors);
                return shape;
            }
        }
        public static Shape CreateMultiPolygon(Vector2D[][] polygons, Scalar gridSpacing, ScalarColor4[][] colors)
        {
            if (polygons.Length == 1)
            {
                return CreatePolygon(polygons[0], gridSpacing, colors[0]);
            }
            else
            {
                MultiPolygonShape shape = new MultiPolygonShape(polygons, gridSpacing);
                shape.Tag = DrawableFactory.CreateMultiPolygon(polygons, colors);
                return shape;
            }
        }
        public static Shape CreateColoredMultiPolygon(Vector2D[][] polygons, Scalar gridSpacing)
        {
            if (polygons.Length == 1)
            {
                return CreateColoredPolygon(polygons[0], gridSpacing);
            }
            else
            {
                MultiPolygonShape shape = new MultiPolygonShape(polygons, gridSpacing);
                ScalarColor3[][] colors = new ScalarColor3[polygons.Length][];
                Vector2D[][] reduced = new Vector2D[polygons.Length][];
                for (int index = 0; index < polygons.Length; ++index)
                {
                    reduced[index] = PolygonShape.Reduce(polygons[index]);
                    colors[index] = CreateColor3Array(reduced[index].Length);
                }
                shape.Tag = DrawableFactory.CreateMultiPolygon(reduced, colors);
                return shape;
            }
        }

        public static Shape CreateSprite(SurfacePolygons surfacePolygons, int reduce, Scalar subdivide, Scalar gridSpacing)
        {
            Vector2D[][] polygons = surfacePolygons.Polygons;
            for (int index = 1; index < reduce; index++)
            {
                polygons = MultiPolygonShape.Reduce(polygons, index);
            }
            polygons = MultiPolygonShape.Subdivide(polygons, subdivide);
            Vector2D centroid = surfacePolygons.Offset;
            Shape shape;
            if (polygons.Length == 1)
            {
                shape = new PolygonShape(polygons[0], gridSpacing);
            }
            else
            {
                shape = new MultiPolygonShape(polygons, gridSpacing);
            }
            shape.Tag = DrawableFactory.CreateSprite(surfacePolygons.Surface, centroid);
            return shape;
        }

        public static RaySegmentsShape CreateRays(RaySegment[] raySegments)
        {
            RaySegmentsShape rayShape = new RaySegmentsShape(raySegments);
            rayShape.Tag = new RaysSegmentsDrawable(rayShape);
            return rayShape;
        }
    }

}