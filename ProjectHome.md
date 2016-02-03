![http://physics2d.googlepages.com/Physics2DDotNetLogo.png](http://physics2d.googlepages.com/Physics2DDotNetLogo.png)Physics2D.Net is 2 dimensional rigid body physics engine written in C#. To work with XNA, Silverlight and .Net.

> It is a complete rewrite of [my original physics 2D found on source forge](http://sourceforge.net/projects/physics2d).    It is CLS compliant. Currently it has only one collision solver. The narrow phase collision detector is considered to be part of the solver.  The Solver is Sequential impulses with a distance grid used for narrow phase detection. There are 4 Broad phase detectors to choose from.

Features:
  * Uses the split impulses solver from [Box2D](http://www.gphysics.com/).
  * This 2D Physics Engine is written entirely in C# and is optimized for C#.
  * It has a wide range of Shapes and supports concave and convex polygons.
  * It has a large number of Methods to manipulate Polygons such as generating a polygon from a bitmap (sprite).
  * The math library it uses (AdvanceMath) is written entirely in C# and is optimized for C#.
  * It uses a class called PhysicsLogic to implement a wide Range of effects from gravity to Maximum velocities.
  * Joints allow bodies to be tied together to make complex systems like ropes and rag dolls.
  * A large amount of parameter checking is done to reduce buggy implementations.
  * Has a simple and elegant design.
  * It is Renderer independent, so it can be used with either Managed Direct X (MDX), XNA, OpenGl, WPF, SilverLight or another Renderer of your choice. (The Demo uses OpenGl)
  * A very unrestrictive license (MIT) so you can use it for any kind of project.
  * The code is well commented using C#’s XML comments.