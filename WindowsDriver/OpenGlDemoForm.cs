#region LGPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Physics2D;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using System.Media;
using SdlDotNet;
using Tao.OpenGl;
using Tao.Sdl;
using AdvanceSystem;
using AdvanceSystem.Forms;
using WindowsDriver.Demos;
namespace WindowsDriver
{



    /// <summary>
    /// This a converted nehe example.
    /// </summary>
    public class OpenGlDemoForm
    {
        #region Fields
        WindowsDriver.Demos.IDemo demo;


        //private MusicDictionary music = new MusicDictionary();
        //Width of screen
        int width = 640;
        //Height of screen
        int height = 640;
        // Bits per pixel of screen
        int bpp = 16;
        // Surface to render on
        Surface screen;

        float timeScale = 1;
        float extraDT;
        float targetDT;
        bool allowSmallerThenTarget = false;
        KeyboardState state = new KeyboardState();
        Thread intergrateThread;
        public static DateTime lastTime = DateTime.Now;
        /// <summary>
        /// Width of window
        /// </summary>
        protected int Width
        {
            get
            {
                return width;
            }
        }

        /// <summary>
        /// Height of window
        /// </summary>
        protected int Height
        {
            get
            {
                return height;
            }
        }

        /// <summary>
        /// Bits per pixel of surface
        /// </summary>
        protected int BitsPerPixel
        {
            get
            {
                return this.bpp;
            }
        }

        #endregion Fields
        #region Constructors

        /// <summary>
        /// Basic constructor
        /// </summary>
        public OpenGlDemoForm(IDemo demo)
        {
            Initialize(demo);
        }

        #endregion Constructors
        /// <summary>
        /// Initializes methods common to all NeHe lessons
        /// </summary>
        protected void Initialize(WindowsDriver.Demos.IDemo demo)
        {
#if Release
            try
            {
#endif
            //Mixer.Open(22050, AudioFormat.Default, 2, 2048);
            demo.InitObjects();
            //demo.AddObjects();
            
            // Sets keyboard events
            // Sets the ticker to update OpenGL Context
            Events.Tick += new TickEventHandler(this.Tick);
            // Sets the resize window event
            Events.VideoResize += new VideoResizeEventHandler(Events_VideoResize);
            Events.Quit += new QuitEventHandler(Events_Quit);
            // Set the Frames per second.
            Events.Fps = 60;
            Events.KeyboardDown += new KeyboardEventHandler(Events_KeyboardDown);
            targetDT = (float)1 / (float)Events.Fps;
            // Creates SDL.NET Surface to hold an OpenGL scene

            double percentOfScreen = .85;

            width = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * percentOfScreen);
            height = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * percentOfScreen);
            //screen = Video.SetVideoModeWindowOpenGL(width, height, true);
            //Environment.
            //screen = Video.SetVideoModeOpenGL(width, height, 16);
            screen = Video.SetVideoModeWindowOpenGL(width, height, true);
            //Video.WindowIcon();
            // Video.
            //Video.SetVideoModeWindowOpenGL(Video.Screen.Width,Video.Screen.Height,true);
            //Tao.Sdl.Sdl.SDL_WM_IconifyWindow();


            // Sets Window icon and title
            this.WindowAttributes();

            this.demo = demo;
            this.demo.World2D.Enabled = true;

#if Release
            }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
                throw;
            }
#endif
        }

        void Events_KeyboardDown(object sender, KeyboardEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Events.QuitApplication();
            }
        }
        void Events_Quit(object sender, QuitEventArgs e)
        {
            Events.QuitApplication();
        }
        void Events_MusicFinished(object sender, MusicFinishedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }
        void Events_VideoResize(object sender, VideoResizeEventArgs e)
        {

            //width = screen.Size.Width;
            //height = screen.Size.Height;
            width = e.Width;
            height = e.Height;
            screen = Video.SetVideoModeWindowOpenGL(width, height, true);
            this.InitGL();
            //Reshape();
        }
        /// <summary>
        /// Sets Window icon and caption
        /// </summary>
        protected void WindowAttributes()
        {
            Video.WindowIcon();
            Video.WindowCaption = "The Ur-Quan Remasters";
        }
        /// <summary>
        /// Resizes window
        /// </summary>
        protected virtual void Reshape()
        {
            this.Reshape(1.0F);
        }
        /// <summary>
        /// Resizes window
        /// </summary>
        /// <param name="distance"></param>
        protected virtual void Reshape(float distance)
        {
            // Reset The current Viewport
            Gl.glViewport(0, 0, width, height);
            // Select The Projection Matrix
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // Reset The Projection Matrix
            Gl.glLoadIdentity();
            // Calculate The Aspect Ratio Of The Window
            //Glu.gluPerspective(45.0F, (width / (float)height), 0.1F, distance);
            Glu.gluPerspective(45.0, (width / (float)height), 0, distance);

            //Gl.glTranslatef(0, 0, 1);
            //Gl.glTranslatef(width / 2, height / 2, 0);
            //Gl.glScalef(width,height, 1);
            // Select The Modelview Matrix
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            // Reset The Modelview Matrix
            Gl.glLoadIdentity();
        }

        /// <summary>
        /// Initializes the OpenGL system
        /// </summary>
        protected virtual void InitGL()
        {
            // Enable Smooth Shading
            Gl.glShadeModel(Gl.GL_SMOOTH);
            // Black Background
            Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.5f);
            // Depth Buffer Setup
            Gl.glClearDepth(1.0F);
            // Enables Depth Testing
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            // The Type Of Depth Testing To Do
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            // Really Nice Perspective Calculations
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
            Gl.glViewport(0, 0, width, height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0, width, 0, height, -100, 100);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }
        Random rand = new Random();


        /// <summary>
        /// Renders the scene
        /// </summary>
        protected virtual void DrawGLScene()
        {

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            //Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.Texture[0]);
            Gl.glLoadIdentity();
            Gl.glTranslatef(width / 2, height / 2, 0);
            Gl.glRotatef(180, 0, 0, 1);
            Gl.glRotatef(180, 0, 1, 0);
            Gl.glTranslatef(-width / 2, -height / 2, 0);

            demo.DrawGraphics(screen.Size);

            int error = Gl.glGetError();
            if (error != 0)
            {
                throw new Exception("OpenGL had an error");
            }
        }

        bool first = true;
        private void Tick(object sender, TickEventArgs e)
        {
#if Release
		try
            {  
#endif
            if (first)
            {
                first = false;
                demo.AddObjects();
                lastTime = DateTime.Now;
            }
            state.Update();
            if (demo.World2D.Enabled)
            {
                if ((intergrateThread == null || intergrateThread.ThreadState == ThreadState.Stopped))
                {
                    DateTime now = DateTime.Now;
                    TimeSpan diff = now.Subtract(lastTime);
                    lastTime = now;
                    float dt = (float)diff.TotalSeconds;
                    intergrateThread = new Thread(new ParameterizedThreadStart(Integrate));
                    intergrateThread.Start(dt);
                    demo.UpdateAI(dt);

                    lock (this.demo.World2D)
                    {
                        DrawGLScene();
                    }
                }
                else
                {

                    lock (this.demo.World2D)
                    {
                        DrawGLScene();
                    }
                    if (!intergrateThread.Join(1000))
                    {
                        intergrateThread.Abort();
                        intergrateThread = null;
                    }
                }
            }
            else
            {
                lastTime = DateTime.Now;
                demo.UpdateKeyBoard(state, .0001f);
                lock (this.demo.World2D)
                {
                    DrawGLScene();
                }
            }
            Video.GLSwapBuffers();

#if Release
		            }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
            }  
#endif
        }

        private void Integrate(object dt)
        {

#if Release
		 try
            {  
#endif
            Integrate((float)dt);
#if Release
		}
            catch (ThreadAbortException) { Console.WriteLine("ThreadAbortException in Integrate"); }
            catch (ThreadInterruptedException) { Console.WriteLine("ThreadInterruptedException in Integrate"); }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
            }  
#endif
        }
        private bool Integrate(float dt)
        {
            float trueDT = dt + extraDT;
            int intergrations = (int)(trueDT / targetDT);
            float timestep = targetDT;
            if (allowSmallerThenTarget)
            {
                if (intergrations > 0)
                {
                    timestep = trueDT / intergrations;
                }
                else
                {
                    intergrations = 1;
                    timestep = trueDT;
                }
            }
            else
            {
                if (trueDT < targetDT || !(intergrations > 0))
                {
                    extraDT += dt;
                    return false;
                }
            }
            //intergrations = 1;
            extraDT = trueDT - intergrations * timestep;
            bool returnvalue = false;
            for (int pos = 0; pos < intergrations; ++pos)
            {
                state.Update();
                demo.UpdateKeyBoard(state, timestep * timeScale);
                returnvalue = demo.Update(timestep * timeScale) || returnvalue;
            }
            return returnvalue;
        }
        //		private void Resize (object sender, VideoResizeEventArgs e)
        //		{
        //			screen = Video.SetVideoModeWindowOpenGL(e.Width, e.Height, true);
        //			if (screen.Width != e.Width || screen.Height != e.Height)
        //			{
        //				//this.InitGL();
        //				this.Reshape();
        //			}
        //		}


        /// <summary>
        /// Starts lesson
        /// </summary>
        public void Run()
        {
#if Release
            try
            { 
#endif
            Reshape();
            InitGL();
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Events.Run();

#if Release
		 }
            catch (Exception ex)
            {
                ErrorBox.DisplayError(ex);
            }  
#endif

        }
    }


}
