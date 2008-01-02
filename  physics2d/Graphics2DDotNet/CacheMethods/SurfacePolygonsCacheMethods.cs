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
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Tao.OpenGl;
using SdlDotNet;
using SdlDotNet.Graphics;
using Color = System.Drawing.Color;
namespace Graphics2DDotNet
{
    public class SurfacePolygonsCacheMethods : ICacheMethods<SurfacePolygons>
    {
        public static Predicate<Color> DefaultIsOpaque = IsOpaqueInternal;
        private static bool IsOpaqueInternal(Color color)
        {
            return color.A != 0;
        }
        public SurfacePolygons LoadItem(string name, object[] loadArgs)
        {
            Predicate<Color> isOpaque = null;
            if (loadArgs != null && loadArgs.Length == 1)
            {
                isOpaque = loadArgs[0] as Predicate<Color>;
                if (isOpaque == null)
                {
                    if (loadArgs[0] is Color)
                    {
                        Color c = (Color)loadArgs[0];
                        isOpaque = delegate(Color color)
                        {
                            return (c.ToArgb() != color.ToArgb());
                        };
                    }
                }
            }
            if (isOpaque == null)
            {
                isOpaque = DefaultIsOpaque;
            }
            Surface surface = Cache<Surface>.GetItem(name);
            Vector2D[][] polygons = MultiPolygonShape.CreateFromBitmap(new SurfaceBitmap(surface, isOpaque));
            polygons = MultiPolygonShape.Reduce(polygons);
            Vector2D offset = MultiPolygonShape.GetCentroid(polygons);
            polygons = MultiPolygonShape.MakeCentroidOrigin(polygons);
            return new SurfacePolygons(surface, polygons, offset);
        }
        public void DisposeItem(SurfacePolygons item)
        { }
    }

}