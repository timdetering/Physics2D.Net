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
using System.Threading;
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
using System.Diagnostics;
#endif


namespace Physics2DDotNet
{
#if CompactFramework || WindowsCE || PocketPC || XBOX360 
    class Stopwatch
    {
        bool started = false;
        TimeSpan timeSpan = TimeSpan.Zero;
        DateTime time;
        public long ElapsedMilliseconds
        {
            get
            {
                if (started)
                {
                    TimeSpan result = DateTime.Now.Subtract(time);
                    return result.Milliseconds;
                }
                else
                {
                    return timeSpan.Milliseconds;
                }
            }
        }
        public void Reset()
        {
            started = false;
            timeSpan = TimeSpan.Zero;
        }
        public void Start()
        {
            if (!started)
            {
                started = true;
                time = DateTime.Now.Subtract(timeSpan);
            }
        }
        public void Stop()
        {
            if (started)
            {
                timeSpan += DateTime.Now.Subtract(time);
                started = false;
            }
        }
    }
#endif



    /// <summary>
    /// The State of a PhysicsTimer
    /// </summary>
    public enum TimerState
    {
        /// <summary>
        /// The PhysicsTimer is Paused.
        /// </summary>
        Paused,
        /// <summary>
        /// The PhysicsTimer's calls to the Callback are on time.
        /// </summary>
        Normal,
        /// <summary>
        /// The PhysicsTimer's calls to the Callback are behind schedule.
        /// </summary>
        Slow,
        /// <summary>
        /// The PhysicsTimer's calls to the Callback are delayed to be on time.
        /// </summary>
        Fast,
        /// <summary>
        /// The PhysicsTimer is Disposed.
        /// </summary>
        Disposed,
    }
    /// <summary>
    /// A Callback used by the PhysicsTimer
    /// </summary>
    /// <param name="dt">The change in time.</param>
    public delegate void PhysicsCallback(Scalar dt);
    /// <summary>
    /// A class to update the PhysicsEngine at regular intervals.
    /// </summary>
    public class PhysicsTimer : IDisposable 
    {
        TimerState state;
        bool isDisposed;
        static int threadCount;
        Scalar targetInterval;
        Stopwatch watch;
        PhysicsCallback callback;
        ManualResetEvent waitHandle;
        Thread engineThread;
        bool isRunning;
        /// <summary>
        /// Creates a new PhysicsTimer Instance.
        /// </summary>
        /// <param name="callback">The callback to call.</param>
        /// <param name="targetDt">The target change in time.</param>
        public PhysicsTimer(PhysicsCallback callback, Scalar targetInterval)
        {
            if (callback == null) { throw new ArgumentNullException("callback"); }
            if (targetInterval <= 0) { throw new ArgumentOutOfRangeException("targetDt"); }
            this.targetInterval = targetInterval;
            this.callback = callback;
            this.waitHandle = new ManualResetEvent(false);
            this.watch = new Stopwatch();
        }
        /// <summary>
        /// Gets and Sets if the PhysicsTimer is currently calling the Callback.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                if (isDisposed) { throw new ObjectDisposedException(this.ToString()); }
                if (isRunning ^ value)
                {
                    isRunning = value;
                    if (value)
                    {
                        watch.Start();
                        if (this.engineThread == null)
                        {
                            this.engineThread = new Thread(EngineProcess);
                            this.engineThread.IsBackground = true;
                            this.engineThread.Name = string.Format("PhysicsEngine Thread: {0}", threadCount++);
                            this.engineThread.Start();
                        }
                        else
                        {
                            waitHandle.Set();
                            waitHandle.Reset();
                        }
                    }
                    else
                    {
                        watch.Stop();
                    }
                }
            }
        }
        /// <summary>
        /// Gets and Sets the desired Interval between Callback calls.
        /// </summary>
        public Scalar TargetInterval
        {
            get { return targetInterval; }
            set
            {
                if (isDisposed) { throw new ObjectDisposedException(this.ToString()); }
                if (value <= 0) { throw new ArgumentOutOfRangeException("value"); }
                this.targetInterval = value;
            }
        }
        /// <summary>
        /// Gets the current State of the PhysicsTimer.
        /// </summary>
        public TimerState State
        {
            get { return state; }
        }
        /// <summary>
        /// Gets and Sets the current Callback that will be called.
        /// </summary>
        public PhysicsCallback Callback
        {
            get { return callback; }
            set
            {
                if (isDisposed) { throw new ObjectDisposedException(this.ToString()); }
                if (value == null) { throw new ArgumentNullException("value"); }
                callback = value;
            }
        }
        void EngineProcess()
        {
            Scalar extraDt, dt;
            extraDt = 0;
            while (!isDisposed)
            {
                if (!isRunning)
                {
                    state = TimerState.Paused;
                    watch.Reset();
                    waitHandle.WaitOne(Timeout.Infinite, false);
                    watch.Start();
                }
                else
                {
                    dt = watch.ElapsedMilliseconds / 1000f;
                    extraDt += dt;
                    if (extraDt < targetInterval)
                    {
                        state = TimerState.Fast;
                        int sleep = (int)((targetInterval - (extraDt + dt)) * 1000);
                        if (sleep < 0) { sleep = 0; }
                        waitHandle.WaitOne(sleep, false);
                    }
                    else
                    {
                        extraDt -= targetInterval;
                        if (extraDt > targetInterval)
                        {
                            extraDt = targetInterval;
                            state = TimerState.Slow;
                        }
                        else
                        {
                            state = TimerState.Normal;
                        }
                        watch.Reset();
                        watch.Start();
                        callback(targetInterval);
                    }
                }
            }
            state = TimerState.Disposed;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    isDisposed = true;
                    waitHandle.Set();
                    watch.Stop();
                    isRunning = false;
                    state = TimerState.Disposed;
                    waitHandle.Close();
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}