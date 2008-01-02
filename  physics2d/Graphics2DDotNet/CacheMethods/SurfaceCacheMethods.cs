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
    public class SurfaceCacheMethods : ICacheMethods<Surface>
    {
        public Surface LoadItem(string name, object[] loadArgs)
        {
            int index = name.LastIndexOf('|');
            if (index == -1)
            {
                return new Surface(Path.Combine(Settings.ImageDir, name));
            }
            else
            {
                string text = name.Substring(0, index);
                string fontname = name.Substring(index + 1);
                Font font = Cache<Font>.GetItem(fontname);
                if (loadArgs == null)
                {
                    return font.Render(text, Color.White, Color.Black);
                }
                switch (loadArgs.Length)
                {
                    case 0:
                        return font.Render(text, Color.White, Color.Black);
                    case 1:
                        return font.Render(text, (Color)loadArgs[0], Color.Black);
                    case 2:
                        return font.Render(text, (Color)loadArgs[0], (Color)loadArgs[1]);
                    case 3:
                        return font.Render(text, (Color)loadArgs[0], (Color)loadArgs[1], (bool)loadArgs[2]);
                    case 4:
                        return font.Render(text, (Color)loadArgs[0], (bool)loadArgs[1], (int)loadArgs[2], (int)loadArgs[3]);
                    case 5:
                        return font.Render(text, (Color)loadArgs[0], (Color)loadArgs[1], (bool)loadArgs[2], (int)loadArgs[3], (int)loadArgs[4]);
                    default:
                        throw new Exception();
                }
            }
        }
        public void DisposeItem(Surface item)
        {
            item.Dispose();
        }
    }

}