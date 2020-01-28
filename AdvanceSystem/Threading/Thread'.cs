namespace AdvanceSystem.Threading
{
    using System;
    using System.Globalization;
    using System.Security;
    using System.Security.Permissions;
    using System.Runtime.ConstrainedExecution;
    using System.Threading;
    /// <summary>Represents the method that executes on a <see cref="T:AdvanceSystem.Threading.Thread`1"></see>.</summary>
    /// <filterpriority>1</filterpriority>
    public delegate void ParameterizedThreadStart<T>(T parameter);
    /// <summary>Creates and controls a thread, sets its priority, and gets its status.</summary>
    /// <filterpriority>1</filterpriority>
    public sealed class Thread<T>
    {
        #region Fields
        private ParameterizedThreadStart<T> start;
        private Thread truethread;
        #endregion
        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="T:AdvanceSystem.Threading.Thread`1"></see> class, specifying a delegate that allows an object to be passed to the thread when the thread is started.</summary>
        /// <param name="start">A <see cref="T:AdvanceSystem.Threading.ParameterizedThreadStart`1"></see> delegate that represents the methods to be invoked when this thread begins executing.</param>
        /// <exception cref="T:System.ArgumentNullException">start is null. </exception>
        public Thread(ParameterizedThreadStart<T> start)
        {
            this.start = start;
            this.truethread = new Thread(new ParameterizedThreadStart(PrivateStart));
        }
        /// <summary>Initializes a new instance of the <see cref="T:AdvanceSystem.Threading.Thread`1"></see> class, specifying a delegate that allows an object to be passed to the thread when the thread is started and specifying the maximum stack size for the thread.</summary>
        /// <param name="maxStackSize">The maximum stack size to be used by the thread.</param>
        /// <param name="start">A <see cref="T:AdvanceSystem.Threading.ParameterizedThreadStart`1"></see> delegate that represents the methods to be invoked when this thread begins executing.</param>
        /// <exception cref="T:System.ArgumentNullException">start is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">maxStackSize is less than 128K (131,072).</exception>
        public Thread(ParameterizedThreadStart<T> start, int maxStackSize)
        {
            this.start = start;
            truethread = new Thread(new ParameterizedThreadStart(PrivateStart), maxStackSize);
        }
        #endregion
        #region Methods
        private void PrivateStart(object parameter)
        {
            start((T)parameter);
        }
        /// <summary>Raises a <see cref="T:AdvanceSystem.Threading.Thread`1AbortException"></see> in the thread on which it is invoked, to begin the process of terminating the thread. Calling this method usually terminates the thread.</summary>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The thread that is being aborted is currently suspended.</exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlThread" /></PermissionSet>
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public void Abort() { truethread.Abort(); }
        /// <summary>Raises a <see cref="T:AdvanceSystem.Threading.Thread`1AbortException"></see> in the thread on which it is invoked, to begin the process of terminating the thread while also providing exception information about the thread termination. Calling this method usually terminates the thread.</summary>
        /// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The thread that is being aborted is currently suspended.</exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlThread" /></PermissionSet>
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public void Abort(object stateInfo) { truethread.Abort(stateInfo); }
        /// <summary>Returns an <see cref="T:System.Threading.ApartmentState"></see> value indicating the apartment state.</summary>
        /// <returns>One of the <see cref="T:System.Threading.ApartmentState"></see> values indicating the apartment state of the managed thread. The default is <see cref="F:System.Threading.ApartmentState.Unknown"></see>.</returns>
        /// <filterpriority>1</filterpriority>
        public ApartmentState GetApartmentState() { return truethread.GetApartmentState(); }
        /// <summary>Returns a <see cref="T:System.Threading.CompressedStack"></see> object that can be used to capture the stack for the current thread.</summary>
        /// <returns>A <see cref="T:System.Threading.CompressedStack"></see> object that can be used to capture the stack for the current thread.</returns>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /><IPermission class="System.Security.Permissions.StrongNameIdentityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PublicKeyBlob="00000000000000000400000000000000" /></PermissionSet>
        [Obsolete("Thread.GetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class"), SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode), StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0x00000000000000000400000000000000")]
        public CompressedStack GetCompressedStack() { return truethread.GetCompressedStack(); }
        /// <summary>Returns a hash code for the current thread.</summary>
        /// <returns>An integer hash code value.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() { return truethread.GetHashCode(); }
        /// <summary>Interrupts a thread that is in the WaitSleepJoin thread state.</summary>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the appropriate <see cref="T:System.Security.Permissions.SecurityPermission"></see>. </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlThread" /></PermissionSet>
        [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public void Interrupt() { truethread.Interrupt(); }
        /// <summary>Blocks the calling thread until a thread terminates, while continuing to perform standard COM and SendMessage pumping.</summary>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The caller attempted to join a thread that is in the <see cref="F:System.Threading.ThreadState.Unstarted"></see> state. </exception>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1InterruptedException">The thread is interrupted while waiting. </exception>
        /// <filterpriority>1</filterpriority>
        [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        public void Join() { truethread.Join(); }
        /// <summary>Blocks the calling thread until a thread terminates or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.</summary>
        /// <returns>true if the thread has terminated; false if the thread has not terminated after the amount of time specified by the millisecondsTimeout parameter has elapsed.</returns>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait for the thread to terminate. </param>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The thread has not been started. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The value of millisecondsTimeout is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite"></see> in milliseconds. </exception>
        /// <filterpriority>1</filterpriority>
        [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        public bool Join(int millisecondsTimeout) { return truethread.Join(millisecondsTimeout); }
        /// <summary>Blocks the calling thread until a thread terminates or the specified time elapses, while continuing to perform standard COM and SendMessage pumping.</summary>
        /// <returns>true if the thread terminated; false if the thread has not terminated after the amount of time specified by the timeout parameter has elapsed.</returns>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"></see> set to the amount of time to wait for the thread to terminate. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The value of timeout is negative and is not equal to <see cref="F:System.Threading.Timeout.Infinite"></see> in milliseconds, or is greater than <see cref="F:System.Int32.MaxValue"></see> milliseconds. </exception>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The caller attempted to join a thread that is in the <see cref="F:System.Threading.ThreadState.Unstarted"></see> state. </exception>
        /// <filterpriority>1</filterpriority>
        [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        public bool Join(TimeSpan timeout) { return truethread.Join(timeout); }
        /// <summary>Resumes a thread that has been suspended.</summary>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The thread has not been started, is dead, or is not in the suspended state. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the appropriate <see cref="T:System.Security.Permissions.SecurityPermission"></see>. </exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlThread" /></PermissionSet>
        [Obsolete("Thread.Resume has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202", false), SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public void Resume() { truethread.Resume(); }
        /// <summary>Sets the apartment state of a thread before it is started.</summary>
        /// <param name="state">The new apartment state.</param>
        /// <exception cref="T:System.InvalidOperationException">The apartment state has already been initialized.</exception>
        /// <exception cref="T:System.ArgumentException">state is not a valid apartment state.</exception>
        /// <exception cref="T:System.ThreadStateException">The thread has already been started.</exception>
        /// <filterpriority>1</filterpriority>
        [HostProtection(SecurityAction.LinkDemand, Synchronization = true, SelfAffectingThreading = true)]
        public void SetApartmentState(ApartmentState state) { truethread.SetApartmentState(state); }
        /// <summary>Applies a captured <see cref="T:System.Threading.CompressedStack"></see> to the current thread.</summary>
        /// <param name="stack">The <see cref="T:System.Threading.CompressedStack"></see> object to be applied to the current thread.</param>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /><IPermission class="System.Security.Permissions.StrongNameIdentityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PublicKeyBlob="00000000000000000400000000000000" /></PermissionSet>
        [Obsolete("Thread.SetCompressedStack is no longer supported. Please use the System.Threading.CompressedStack class"), SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode), StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0x00000000000000000400000000000000")]
        public void SetCompressedStack(CompressedStack stack) { truethread.SetCompressedStack(stack); }
        /// <summary>Causes the operating system to change the state of the current instance to <see cref="F:System.Threading.ThreadState.Running"></see>, and optionally supplies an object containing data to be used by the method the thread executes.</summary>
        /// <param name="parameter">An object that contains data to be used by the method the thread executes.</param>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The thread has already been started. </exception>
        /// <exception cref="T:System.InvalidOperationException">This thread was created using a <see cref="T:AdvanceSystem.Threading.Thread`1Start"></see> delegate instead of a <see cref="T:AdvanceSystem.Threading.ParameterizedThreadStart`1"></see> delegate.</exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the appropriate <see cref="T:System.Security.Permissions.SecurityPermission"></see>. </exception>
        /// <exception cref="T:System.OutOfMemoryException">There is not enough memory available to start this thread. </exception>
        /// <filterpriority>1</filterpriority>
        [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        public void Start(T parameter)
        {
            truethread.Start(parameter);
        }
        /// <summary>Either suspends the thread, or if the thread is already suspended, has no effect.</summary>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The thread has not been started or is dead. </exception>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the appropriate <see cref="T:System.Security.Permissions.SecurityPermission"></see>. </exception>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlThread" /></PermissionSet>
        [Obsolete("Thread.Suspend has been deprecated.  Please use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.  http://go.microsoft.com/fwlink/?linkid=14202", false), SecurityPermission(SecurityAction.Demand, ControlThread = true), SecurityPermission(SecurityAction.Demand, ControlThread = true)]
        public void Suspend() { truethread.Suspend(); }
        /// <summary>Sets the apartment state of a thread before it is started.</summary>
        /// <returns>true if the apartment state is set; otherwise, false.</returns>
        /// <param name="state">The new apartment state.</param>
        /// <exception cref="T:System.ArgumentException">state is not a valid apartment state.</exception>
        /// <exception cref="T:System.ThreadStateException">The thread has already been started.</exception>
        /// <filterpriority>1</filterpriority>
        [HostProtection(SecurityAction.LinkDemand, Synchronization = true, SelfAffectingThreading = true)]
        public bool TrySetApartmentState(ApartmentState state) { return truethread.TrySetApartmentState(state); }
        #endregion
        #region Properties
        /// <summary>Gets or sets the apartment state of this thread.</summary>
        /// <returns>One of the <see cref="T:System.Threading.ApartmentState"></see> values. The initial value is Unknown.</returns>
        /// <exception cref="T:System.ArgumentException">An attempt is made to set this property to a state that is not a valid apartment state (a state other than single-threaded apartment (STA) or multithreaded apartment (MTA)). </exception>
        /// <filterpriority>2</filterpriority>
        [Obsolete("The ApartmentState property has been deprecated.  Use GetApartmentState, SetApartmentState or TrySetApartmentState instead.", false)]
        public ApartmentState ApartmentState
        {
            get { return truethread.ApartmentState; }
            [HostProtection(SecurityAction.LinkDemand, Synchronization = true, SelfAffectingThreading = true)]
            set { truethread.ApartmentState = value; }
        }
        /// <summary>Gets or sets the culture for the current thread.</summary>
        /// <returns>A <see cref="T:System.Globalization.CultureInfo"></see> representing the culture for the current thread.</returns>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlThread" /></PermissionSet>
        public CultureInfo CurrentCulture
        {
            get { return truethread.CurrentCulture; }
            [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
            set { truethread.CurrentCulture = value; }
        }
        /// <summary>Gets or sets the current culture used by the Resource Manager to look up culture-specific resources at run time.</summary>
        /// <returns>A <see cref="T:System.Globalization.CultureInfo"></see> representing the current culture.</returns>
        /// <exception cref="T:System.ArgumentNullException">The property value is null. </exception>
        /// <filterpriority>2</filterpriority>
        /// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /></PermissionSet>
        public CultureInfo CurrentUICulture
        {
            get { return truethread.CurrentUICulture; }
            [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
            set { truethread.CurrentUICulture = value; }
        }
        /// <summary>Gets an <see cref="T:System.Threading.ExecutionContext"></see> object that contains information about the various contexts of the current thread. </summary>
        /// <returns>An <see cref="T:System.Threading.ExecutionContext"></see> object that consolidates context information for the current thread.</returns>
        /// <filterpriority>2</filterpriority>
        public ExecutionContext ExecutionContext
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            get { return truethread.ExecutionContext; }
        }
        /// <summary>Gets a value indicating the execution status of the current thread.</summary>
        /// <returns>true if this thread has been started and has not terminated normally or aborted; otherwise, false.</returns>
        /// <filterpriority>1</filterpriority>
        public bool IsAlive { get { return truethread.IsAlive; } }
        /// <summary>Gets or sets a value indicating whether or not a thread is a background thread.</summary>
        /// <returns>true if this thread is or is to become a background thread; otherwise, false.</returns>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The thread is dead. </exception>
        /// <filterpriority>1</filterpriority>
        public bool IsBackground
        {
            get { return truethread.IsBackground; }
            [HostProtection(SecurityAction.LinkDemand, SelfAffectingThreading = true)]
            set { truethread.IsBackground = value; }
        }
        /// <summary>Gets a value indicating whether or not a thread belongs to the managed thread pool.</summary>
        /// <returns>true if this thread belongs to the managed thread pool; otherwise, false.</returns>
        /// <filterpriority>2</filterpriority>
        public bool IsThreadPoolThread { get { return truethread.IsThreadPoolThread; } }
        /// <summary>Gets a unique identifier for the current managed thread.</summary>
        /// <returns>An integer that represents a unique identifier for this managed thread.</returns>
        /// <filterpriority>1</filterpriority>
        public int ManagedThreadId
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get { return truethread.ManagedThreadId; }
        }
        /// <summary>Gets or sets the name of the thread.</summary>
        /// <returns>A string containing the name of the thread, or null if no name was set.</returns>
        /// <exception cref="T:System.InvalidOperationException">A set operation was requested, and the Name property has already been set. </exception>
        /// <filterpriority>1</filterpriority>
        public string Name
        {
            get { return truethread.Name; }
            [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
            set { truethread.Name = value; }
        }
        /// <summary>Gets or sets a value indicating the scheduling priority of a thread.</summary>
        /// <returns>One of the <see cref="T:AdvanceSystem.Threading.Thread`1Priority"></see> values. The default value is Normal.</returns>
        /// <exception cref="T:System.ArgumentException">The value specified for a set operation is not a valid ThreadPriority value. </exception>
        /// <exception cref="T:AdvanceSystem.Threading.Thread`1StateException">The thread has reached a final state, such as <see cref="F:System.Threading.ThreadState.Aborted"></see>. </exception>
        /// <filterpriority>1</filterpriority>
        public ThreadPriority Priority
        {
            get { return truethread.Priority; }
            [HostProtection(SecurityAction.LinkDemand, SelfAffectingThreading = true)]
            set { truethread.Priority = value; }
        }
        /// <summary>Gets a value containing the states of the current thread.</summary>
        /// <returns>One of the <see cref="T:AdvanceSystem.Threading.Thread`1State"></see> values indicating the state of the current thread. The initial value is Unstarted.</returns>
        /// <filterpriority>2</filterpriority>
        public ThreadState ThreadState
        {
            get { return truethread.ThreadState; }
        }
        #endregion
    }
}