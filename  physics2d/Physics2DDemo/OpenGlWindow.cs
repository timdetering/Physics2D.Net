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




using System;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using Tao.OpenGl;

namespace Physics2DDemo
{
    public delegate void DrawDelegate(int width, int height);

    public class OpenGlWindow
    {
        public DrawDelegate DrawCallback;

        Surface screen;
        int width;
        int height;

        public OpenGlWindow(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();
            if (DrawCallback != null) { DrawCallback(width, height); }
            Video.GLSwapBuffers();
        }
        void Reshape()
        {
            width = screen.Width;
            height = screen.Height;
            Gl.glViewport(0, 0, width, height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, width, 0, height, -1, 1);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }

        void Init()
        {
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glClearColor(0, 0, 0, 0.5f);
            Gl.glClearDepth(1);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
        }

        public void Run()
        {

            Video.WindowIcon();
            Video.WindowCaption = "Physics2D.Net Demo";
            screen = Video.SetVideoMode(width, height, true, true);
            Events.Tick += this.Tick;
            Events.Quit += this.Quit;
            Events.VideoResize += this.Resize;
            Events.KeyboardDown += this.KeyboardDown;

            Events.Fps = 60;

            Init();
            Reshape();
            Events.Run();
        }

        private void Resize(object sender, VideoResizeEventArgs e)
        {
            screen = Video.SetVideoMode(e.Width, e.Height, true, true);
        }

        private void KeyboardDown(
            object sender,
            KeyboardEventArgs e)
        {
            if (e.Key == Key.Escape ||
                e.Key == Key.Q)
            {
                Events.QuitApplication();
            }
        }

        private void Tick(object sender, TickEventArgs e)
        {
            if (screen.Width != width || screen.Height != height)
            {
                Init();
                Reshape();
            }
            Draw();
        }

        private void Quit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }
    }
}

