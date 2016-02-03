# Release Notes #
Since God Created me and Jesus Saved me All glory goes to God.

## Physics2D.NET ##
### 2.0.0.0 ###

Quite a few changes to this release. And quite a few of them breaking.

It is guaranteed any code that worked with this engine in the past will no longer. But don’t worry most of it can be easily fixed.

These are the ones that are more likely to get you:

All the shapes have been renamed.
A lot of classes have moved around to different namespaces.
The Lifespan class has been completely rewritten.

These are ones that might get you if you use any special features in the engine:

All Matrix3x3 in the engine have been changed to the new Matrix2x3. (You can no longer do perspective transformations)

Changed all Shapes to be stateless. This means all methods and properties will return data as if the shape has an identity matrix applied to it. It also means that multiple Bodies can share the exact same Shape instance.  Vertexes is now what original vertexes was. This means the Rectangle and Matrix properties have been moved to the Body. The body now has a Matrices class that hold the matrixs and instead of the confusing names of inverted and such I use the naming of ToBody(Normal)  and ToWorld(Normal).

Another big change is now the Body.Transform property should work in every condition. Earlier raysegments would not work against a transformed circle or even a polygon.

I’ve added a few logics to make certain things easier.
The GlobalFluidLogic applies buoyancy and drag to everything in the engine.
The LineFluidLogic Applys drag and buoyancy to items on one side of a line. (The water line)
The ExplosionLogic simulates a simple explosion.

Also Added Dampening to the Body.

The collision event has also changed. it will only be raised on the initial collision between 2 objects, but dont worry the Contact property in the EventArg has a event that will be raised every time step the collision persists.


### 1.5.0.0 ###

There were quite a few things renamed because FXcop did not like them or I did not like them, the renames where mostly just changing of the case so these should be easy to fix.

A few things in AdvanceMath were renamed to make them closer to XNA’s math lib.

There are some minor speed improvements done.

There are quite a few bug fixes.

There are 3 new Broad Phase Detectors: Brute force, Selective Sweep and Frame Coherent Sweep and Prune. The brute force is there for people to use as a baseline for comparisons. Selective Sweep is a very good broudphase that I would recommend. Frame Coherent Sweep and Prune is good for very static environments.

Compatibility with Xbox360: the library will now compile under the compact framework for the Xbox360.

Fixes for support of serialization. All Physics2D.Net class now should serialize properly. Serializing the PhysicsEngine during a call to Update is not supported.

Support for filtering collision events using ignorers via the Body.EventIgnorer property.

MultipartPolygon.CreateFromBitmap returns multiple polygons that are inside a bitmap.

Added something I am calling proxying. It’s a way to have objects either wrap around the screen with collision being done properly or doing a 2d version of portal.

### 1.4.0.0 ###
There are a few breaking changes. The StateChanged event was renamed and it logic was changed so that it would only be called when the postion of a object changed from the last time it was called.

I added a Transformation Property to Body this allows you to transform the shap of a body. The Transformation matrix is multiplied against the matrix that will be applied to the Shape on each Apply matrix call. It allows you to make ovals out of circles or even apply a skew to a polygon.

I changed some the Shape’s properties to allow for a few more shape types to be added.

One shape that was added is the MultiPartPolygon this class is similar to the normal Polygon class except that it allows you to pass multiple polygons to describe its shape. This means you can make shapes that have parts for example you could make a block a text where each polygon is a letter.

Another shape I added is the RaySegments shape. This shape is used only for collision detection and has a custom intersection info class that will be passed to the collision event. The ray segments shape allows you to put in multiple rays (think lasers)

I did remove the Line shape since it would not work with either the RaySegments class or The Transformation property on body, and it can be represented with a polygon rather easily.

I also fixed restitution to be a lot more stable. In terms that objects with restitution higher then zero can now stack and be stable in the stacking.

### 1.3.0.0 ###

Quite a few changes for this release and bug fixes. You can check the change log for a rather extensive list.

There is also a lot of speed / memory use improvements.

I added the ability to generate a polygon from a bitmap. (This is really fun to play with)

I started some nunit tests for AdvanceMath. A added a completely new Geometry2D namespace to AdvanceMath this contains a lot of geometry classes and quite a few methods for testing intersection and such similar to XNA’s design. The nunit tests for them are incomplete so no guaranties that they work the way they should.  I also implemented some collision ignorers for your consumption. I also fixed an annoying stability bug with Contacts. I’ve also wrote a better Polygon.Reduce method that weighs vertex removal based on how much they would change the polygons area and it works extremely well.

The tank in the demo is a lot of fun to play with even with the fact that it’s limited by the screen not moving.


### 1.2.0.0 ###

The largest noticeable changes are that I’ve implemented split impulses for the joints. I changed how a few events get generated and added some more events.

Here are a few others:

fixed possible errors with the AddRange methods in the PhysicsEngine.

removed things specific to the solver and put them in classes used only by the solver.

added more parameter checking.

Added a RemovedEventArg holding the engine and if the object was pending.

Changed the removed event to be generated when the engine was cleared and the object was  pending.

Added a Pending event for when a object was just added to the engine, but not yet in the  updated loop.

Changed how the ApplyMatrix affects the Body when a parameter is passed.

For everything else check the changelog.

In the end it was mainly a bugfix/refactoring release.

### 1.1.0.0 ###
Mostly this has been an optimization and bug fix release. The only major change was the update of the sequential impulses to used split impulses like the more recent version of Box2D.

You can check the change log for every detail but here are a few important changes:

A lot of renaming happened but mostly on parameter names and only changing the case to be acceptable to fxCop. Like LifeSpan and LifeTime became Lifespan and Lifetime respectively. A few event names changed like NewState became StateChanged. If your code breaks it should be rather easy to find the slightly renamed methods and such.

Added a new event to the Body called ShapeChanged which is called when the shape property is assigned to a different shape.

Changed the logic on how some event are called. StateChanged was only called inside the update method but now its called every time apply matrix is called. (the assumption is if you are calling apply matrix you’ve changed the state.)

I’ve been trying to refactor and rename my AdvanceMath lib so it can be interchangeable with XNA’s so once I get that done. I can switch back and forth with a few define and reference changes. Making it so it can be used with XNA without having to convert vectors and such back and forth. I’m still at the design stage of this idea but I have been making the math classes similar.

Though it’s not part of the engine I made the demo run faster by using display lists and added two more demos.


### 1.0.0.0 ###
This is a complete rewrite/redesign of Physics2D very few
classes where copied over like PhysicsState and MassInfo.
The rest were completely rewritten or brand new. I tried
the minimalist approach with this version. Allot of the
classes are just gone, since there were either confusing
or unnecessary bloat.