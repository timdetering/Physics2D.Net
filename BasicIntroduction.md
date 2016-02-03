You first need to understand how initialize and use the engine.
This page is meant to give you a general overview on how to do a basic setup of the engine.


# Initializing The Engine #
```

            PhysicsEngine engine = new PhysicsEngine();
            engine.BroadPhase = new Physics2DDotNet.Detectors.SelectiveSweepDetector();
            engine.Solver = new Physics2DDotNet.Solvers.SequentialImpulsesSolver();
```

This shows an instance of the Physics Engine being declared and initialized.
The physics engine has 2 properties that must be set before the engine can run. The BroadPhase and the Solver. These are very important but we can ignore them for now.

Once this is done you can do calls to Update without exceptions being thrown, but nothing will happen if there are no bodies in the engine.

# Running Engine #
To use the engine you must call PhysicsEngine.Update at regular intervals the library has a class called PhysicsTimer to do this for you.
```

            PhysicsTimer timer = new PhysicsTimer(engine.Update, .01f);
            timer.IsRunning = true;
```
As you see it is simple to set up and use. You just pass a delegate (which does not have to be engine.Update) for the timer to call at regular intervals. The second parameter is the amount of time in seconds between the calls.
Then setting IsRunning to true will start the timer.

# Using the Engine #
To use the engine you first need to add bodies and other things to it for anything to happen.

To add a body you first must create one.
To create one you have to describe exactly so that the engine will know how to represent it.
## Coefficients ##
First we start with the Coefficients which describes it’s restitution and friction.
The restitution is how bouncy something is 1 is very bouncy 0 is not bouncy.
The friction is how much sliding is allowed 1 is none 0 is all.
```

            Coefficients coffecients = new Coefficients(/*restitution*/1, /*friction*/.5f);
```
## Shapes ##
The engine allows for multiple shapes to be represented in the engine from circles to Ray Segments.
Each Body has a shape which must be defined. Here we create 2 shapes a circle and a Polygon (which is actually a rectangle)
```

            IShape shape1 = new CircleShape(8, 7);
            IShape shape2 = new PolygonShape(VertexHelper.CreateRectangle(10, 20),3);
```

The first parameter for a circle is its radius the second is how many vertices will be created along its perimeter to represent it.

The first parameter for the polygon is the list of vertices for the polygon (this can be both concave and convex) the second is the spacing for the distance grid. This values should be about a third the size of the smallest part of the polygon.

  * NOTE: There are more values specified for both the Polygon and the Circle then probably expected, because the engine uses a special system for narrow phased collision detection, which I call a distance grid. The circle needs a list of vertices to compare against a polygon and a polygon must have the spacing of the grid defined.
## Bodies ##
### Creating ###

Then we combine all the parameters and create the Bodies.
```

            float mass = 5;
            Body body1 = new Body(new PhysicsState(), shape1, mass, coffecients, new Lifespan());
            Body body2 = new Body(new PhysicsState(), shape2, mass, coffecients, new Lifespan());
```
The PhysicsState object describes the Body’s position, velocity and acceleration.
The default constructor creates a motionless body and the origin.
The mass is just how hard it is to change a body's velocity.
The Lifespan object describes how long the body will remain in the engine. The default construct creates one that will remain in the engine until the it's IsExpired property is set to true. You can set or modify both of these after the body has be created.
  * NOTE: all object that are added to the engine have a Lifespan object.



The Bodies have been created but this does nothing until they are added to the engine.


### Adding ###

To add the object is rather simple, just call the corresponding method on the engine instance.
```

            engine.AddBody(body1);
            engine.AddBody(body2);
```
A body is first added to a pending list and then actually added to the engine on the next call to update.

### Removing ###

To Remove a object just set it’s lifetime object to expired. It will be removed on the next call to the engine Update method.
```
            body1.Lifetime.IsExpired = true;
```

## Joints ##
Joints are connections between 2 objects this could be between 2 bodies or between a body and some arbitrary point. This connection causes bodies to affect each other. Like a Hinge joint will cause 2 bodies to act as if they have a door like hinge between them.
### Creating ###
To create a joint you will use that joints constructor.
```
            Joint joint = new HingeJoint(body1, body2, new Vector2D(100,0), new Lifespan());
```
The third parameter is the location of the hinge.
### Adding ###
Naturally before the joint has any effect it must be added to the engine.
```
            engine.AddJoint(joint);
```
### Removing ###
There are 2 ways to remove a joint.
One is the same as for a body just set it Is expired property to true or remove one of the bodies that is affected by the joint.
```
            joint.Lifetime.IsExpired = true;
```

## Physics Logics ##
They are logic that is ran on the engine every update. This can be gravity or a velocity limiter. They are created, added, and removed just like Bodies and Joints.