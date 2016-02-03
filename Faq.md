# Frequently Asked Questions #


## I have my own renderer, what assemblies do I need to reference just for the engine? ##

Just Physics2DDotNet and AdvanceMath


## I try to get the project to compile but I get compile errors about Unit Tests. What’s going on? ##


The AdvanceMathUnitTest part of the Project is purely for testing and it requires a library to compile called NUnit. It is not necessary to run the Demo or Compile the library you can do two things to solve this (in Visual Studios 2005):

Right click on AdvanceMathUnitTest and the select remove. Then it
should compile.

Or you could right click on Physics2DDemo and then select debug->start
new instance.



## Where is the Documentation? ##

The library itself is well commented using C#’s XML comments these comments are easily accessed via intellisense. But I do plan to add tutorials. You can specifically ask for a tutorial on the Discussion page and I will try to make one for you.

## How do I remove a Body from the engine? ##

Set its Lifetime.IsExpired to true. It will be removed in PhysicsEngine.Update

## How do I Make an immoveable platform? ##

Set Body.IgnoresGravity to true and Body.Mass = new MassInfo(float.PositiveInfinity, float.PositiveInfinity).

## My Polygons don’t collide what is happening? ##

You most likely have your grid spacing set too large. It’s a parameter in the Polygon’s constructor. The grid spacing is used to create the CollisionGrid which is how Polygons do collision detection. The smaller the value the more accurate it will be. I found having it about 1/4 the size of the smallest feature of a polygon works well.

## What do all those assemblies do? ##

**AdvanceMath**
> This is the basic math classes like vectors and such. They are separate in case someone
> wishes to use them without the engine.

**AdvanceMathUnitTest**
> This is some unit tests for the AdvanceMath lib. they are far from complete.

**ConsoleDriver**
> This is just a console app. only there for my convenience to test stuff without running a
> complete demo

**DemoRunner**
> This just starts the Physics2DDotNet.Demo just there to organize all the external dlls
> needed to run the demo.

**Graphics2D.Net**
> This is a Graphical library to help with the rendering of demos. It was written to have
> the Demos up to the same code quality of Physics2D.Net. Since many complained that the
> demo was confusing and ugly.

**Physics2DDemo**
> This is the old demo. I plan to eventually delete it.

**Physics2DDotNet**
> This is the actual Physics Engine Library in all its glory.

**Physics2DDotNet.Demo**
> This holds the demos it uses Graphics2D.Net for rendering.