_It is recommended to first read the BasicIntroduction page._


Author: madpew


# Introduction #
In this tutorial you will:
  * Creating the physics engine.
  * Setup the physics engine.
  * Add a GravityLogic.
  * Add two Bodies (one static, while the other dynamic)


**Note:**
> _C# is not my "native" language, and may find VB like characteristics in the code.
> This code was originally programmed in VB.NET 2005 and can be download from the [discussion](http://groups.google.com/group/physics2ddotnet) downloads area.
> Also the "rdrRenderer" (IRenderer) object is not in Physics2D.Net
> and only used to take focus out of the graphics rendering area._



# Creating An Instance #

```
using AdvanceMath;
using Physics2DDotNet.Math2D;
//import the namespace physics 2D .Net uses for math calculations
using Physics2DDotNet;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.Solvers;
//import the physics 2d .Net namespace

namespace Tutorial01_CSharp
{
	public partial class frmTutorial : Form
	{

		public frmTutorial()
		{
			InitializeComponent();
			this.Load += new EventHandler(this.Tutorial_Load);
			this.FormClosing += new FormClosingEventHandler(this.Tutorial_FormClosing);
			this.RendererWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RendererWorker_DoWork);
		}

		IRenderer rdrRenderer;
		//renderer that will handle a backbuffer and such for me using the GDI+ api

		PhysicsEngine phsEngine;
		//engine for the physics, handles just about everything
		PhysicsTimer phsTimer;
		//timer to accurately raise timestep events 

		Body bdyBox;
		//this is going to be our lonely box that is physically enabled

		bool blnExitRendererLoop;

		private delegate void TutorialClose_Callback();

		private void Tutorial_Load(object sender, System.EventArgs e)
		{
			this.RendererWorker.WorkerSupportsCancellation = true;
			this.ClientSize = new Size(800, 600);
			this.DoubleBuffered = true;

			rdrRenderer = new GdiRenderer();
			//create the renderer
			rdrRenderer.Target = this.Handle;
			//set the target where the scene will be renderer
			rdrRenderer.Initialize();
			//initialize the renderer (creates the image buffers)

			phsEngine = new PhysicsEngine();

			phsEngine.BroadPhase = (BroadPhaseCollisionDetector)new SweepAndPruneDetector();
			//pick a board phaser type...

			phsEngine.Solver = (CollisionSolver)new SequentialImpulsesSolver();
			//pick a solver type...
			SequentialImpulsesSolver phsSolver = (SequentialImpulsesSolver)phsEngine.Solver;
			phsSolver.Iterations = 12;
			phsSolver.SplitImpulse = true;
			phsSolver.BiasFactor = 0.7f;
			phsSolver.AllowedPenetration = 0.1f;
			//fine tune solver to your liking, this seems to be popular

			phsTimer = new PhysicsTimer(this.Physics_Timestep, 0.01f);
			//create the physics timer

			phsTimer.IsRunning = true;
			//start it up!

			Tutorial_CreateGravity();
			//need gravity or you just got a white box that "floats"
			Tutorial_CreateFloor();
			//create the floor, a white box that just falls isn't fun
			Tutorial_CreateBox();
			//the action character

			this.RendererWorker.RunWorkerAsync();
		}

		private void Tutorial_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			//clean up time...
			if (this.RendererWorker.IsBusy)
			{
				e.Cancel = true;
				blnExitRendererLoop = true;
			}
			else
			{
				phsTimer.IsRunning = false;
				phsTimer.Dispose();
				phsTimer = null;

				bdyBox = null;

				phsEngine.Clear();
				phsEngine = null;

				rdrRenderer.Release();
				rdrRenderer = null;
			}
		}

		private void Physics_Timestep(float timeStep)
		{
			phsEngine.Update(timeStep);
			//like "bullet-time"? maybe you should try "timeStep*fractionAmount", where fractionAmount is like 0.25
		}

		private void RendererWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			while (!(blnExitRendererLoop))
			{
				try
				{
					//this is just the renderer starting, clearing, drawing, finishing, and presenting the scene.
					rdrRenderer.Start();
					rdrRenderer.Clear(Color.Black);

					rdrRenderer.DrawRectangle(new Point(Convert.ToInt32(bdyBox.State.Position.Linear.X), Convert.ToInt32(bdyBox.State.Position.Linear.Y)), new Size(64, 64), Color.White, DrawStyleMode.Fill, new Point(Convert.ToInt32(bdyBox.State.Position.Linear.X) + 32, Convert.ToInt32(bdyBox.State.Position.Linear.Y) + 32), (bdyBox.State.Position.Angular * (float)57.2957795));

					rdrRenderer.Finish();
					rdrRenderer.Present();

					Thread.Sleep(33);
				//we don't really need advanced FPS control, so this will just do
				}
				catch (ThreadAbortException ex)
				{
					break;
				}
				catch (Exception ex)
				{
				}
			}
		}

		private void RendererWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			Tutorial_Close();
		}

	}
}

```
_The above code creates the necessary objects, initializes them, and handles the disposables._

When creating the PhysicsEngine instance you also need to setup a BroadPhaser for detecting collisions and a CollisionSolver for responding to a collision. Each type have their own namespace (Physics2DNet.Detectors, Physics2DDotNet.Solvers) respectively. _Currently Physics2D.Net only has one solver then multiple detectors_

# The Tree Apple Effect #

```
		private void Tutorial_CreateGravity()
		{
			PhysicsLogic logGravity;

			logGravity = (PhysicsLogic)new GravityField(new Vector2D(0f, 200f), new Lifespan());
			//pretty basic, create a downward force

			phsEngine.AddLogic(logGravity);
		}
```

Physics2D.Net allows to create custom PhysicsLogic objects that will apply forces to bodies in the world. Physics2D.Net also already has a PhysicsLogic object for gravity characteristics. You can simply apply gravity in your world by creating the GravityField with X and Y axis force amounts. Then add the base PhysicsLogic class to the PhysicsEngine collection.



# Adding A Floor #



```
		private void Tutorial_CreateFloor()
		{
			Body bdyFloor;
			PhysicsState flrState;
			Shape flrShape;
			Coefficients flrCoff;
			Lifespan flrLife;

			flrState = new PhysicsState(new ALVector2D((float)0.0, ((float)this.ClientSize.Width) * ((float)0.5), (float)this.ClientSize.Height));
			//create the state, centering the x-axis on screen and bottom of the y-axis

			flrShape = new Polygon(Polygon.CreateRectangle(64, this.ClientSize.Width), 2);
			//create form.widthX64 rectangle (sq) with grid spacing at 2

			flrCoff = new Coefficients(0.5f, 0.4f, 0.4f);
			//might require tuning to your liking...

			flrLife = new Lifespan();
			//forever and ever

			bdyFloor = new Body(flrState, flrShape, float.PositiveInfinity, flrCoff, flrLife);
			//never ending mass means it isn't going to move on impact

			bdyFloor.IgnoresGravity = true;
			//make sure the floor stays

			phsEngine.AddBody(bdyFloor);

		}
```

Since this is only a simple program, we will be creating a floor just under the bottom of the screen. This will create the illulsion that the box hit the bottom of the form window.

The floor is actually a physics Body, but ignores the gravity logic (so it doesn't fall) and the mass of the Body is at an infinite state (so no body can move it from collision responses). Also each Body in the PhysicsEngine collection holds a Lifespan. Simply creating a Lifespan with no arguments makes it have an infinite life.



# The Star Of The Show #


```
		private void Tutorial_CreateBox()
		{
			PhysicsState boxState;
			//the state holds its position, velocity and angle
			Shape boxShape;
			//the shape is the area for a collision bounding polygon
			Coefficients boxCoff;
			//the coefficients is the box's friction characteristics, i.e. a wood box and metal box would have different coefficients do to the box's surface texture
			Lifespan boxLife;
			//how long the box will live until it is destoried. this is also the only way to remove bodies, to make sure the bodies collection is synchronized to the physics engine.

			boxState = new PhysicsState(new ALVector2D(0, this.ClientSize.Width * 0.5f, 0));
			//create the box state, centering the x-axis on screen

			boxShape = new Polygon(Polygon.CreateRectangle(64, 64), 2);
			//create 64x64 rectangle (sq) with grid spacing at 2

			boxCoff = new Coefficients(0.5f, 0.4f, 0.4f);
			//might require tuning to your liking...

			boxLife = new Lifespan();
			//forever and ever

			bdyBox = new Body(boxState, boxShape, 30, boxCoff, boxLife);

			phsEngine.AddBody(bdyBox);
			//add the body to the physics engine collection

			bdyBox.ApplyTorque(900000f);
			//make it spin to see the collision reaction better when it hits the floor
		}
```

We create one more Body, but this time for a box that is up in the air. The box also has torque applied to itself so when the PhysicsEngine starts it will cause the box to spin, making the collision response when it hits the floor more dramatic.