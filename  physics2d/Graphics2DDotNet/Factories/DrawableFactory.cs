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
    public static class DrawableFactory
    {
        private static int GetPower(int value)
        {
            int index = 30;
            for (; (1 << index) >= value && index > -2; --index) { }
            return 1 << index + 1;
        }
        public static PolygonDrawable CreatePolygon(Vector2D[] vertexes)
        {
            return new PolygonDrawable(vertexes);
        }
        public static Colored3PolygonDrawable CreatePolygon(Vector2D[] vertexes, ScalarColor3[] colors)
        {
            return new Colored3PolygonDrawable(vertexes, colors);
        }
        public static Colored4PolygonDrawable CreatePolygon(Vector2D[] vertexes, ScalarColor4[] colors)
        {
            return new Colored4PolygonDrawable(vertexes, colors);
        }


        public static MultiPolygonDrawable CreateMultiPolygon(Vector2D[][] polygons)
        {
            return new MultiPolygonDrawable(polygons);
        }
        public static Colored3MultiPolygonDrawable CreateMultiPolygon(Vector2D[][] polygons, ScalarColor3[][] colors)
        {
            return new Colored3MultiPolygonDrawable(polygons, colors);
        }
        public static Colored4MultiPolygonDrawable CreateMultiPolygon(Vector2D[][] polygons, ScalarColor4[][] colors)
        {
            return new Colored4MultiPolygonDrawable(polygons, colors);
        }
        public static SpriteDrawable CreateSprite(Surface surface, Vector2D[] vertexes, Vector2D[] coordinates)
        {
            return new SpriteDrawable(surface, vertexes, coordinates);
        }
        public static SpriteDrawable CreateSprite(Surface surface, Vector2D offset)
        {
            Vector2D[] vertexes = new Vector2D[4];
            vertexes[0] = -offset;
            vertexes[1] = new Vector2D(-offset.X, surface.Height - offset.Y);
            vertexes[2] = new Vector2D(surface.Width - offset.X, surface.Height - offset.Y);
            vertexes[3] = new Vector2D(surface.Width - offset.X, -offset.Y);

            Scalar xScale = surface.Width / (Scalar)GetPower(surface.Width);
            Scalar yScale = surface.Height / (Scalar)GetPower(surface.Height);
            Vector2D[] coordinates = new Vector2D[4];
            coordinates[1] = new Vector2D(0, 0);
            coordinates[0] = new Vector2D(0, yScale);
            coordinates[3] = new Vector2D(xScale, yScale);
            coordinates[2] = new Vector2D(xScale, 0);
            return new SpriteDrawable(surface, vertexes, coordinates);
        }
    }

}