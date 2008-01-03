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
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Drawing;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Graphics2DDotNet;
using Physics2DDotNet;
using Physics2DDotNet.Ignorers;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.PhysicsLogics;
using SdlDotNet;
using SdlDotNet.Input;
using SdlDotNet.Graphics;
namespace Physics2DDotNet.Demo.Demos
{
    [PhysicsDemo("Simple", "Ragdoll's", "TODO")]
    public class RagdollDemo : BaseDemo
    {
        DisposeCallback dispose;
        protected override void Open()
        {
            dispose += DemoHelper.BasicDemoSetup(DemoInfo);
            dispose += DemoHelper.CreateTank(DemoInfo, new Vector2D(300, 0));
            Layer.Engine.AddLogic(new GravityField(new Vector2D(0, 1000), new Lifespan()));
            DemoHelper.AddLine(DemoInfo, new Vector2D(0, 700), new Vector2D(300, 700), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(300, 700), new Vector2D(400, 650), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(400, 650), new Vector2D(500, 650), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(500, 650), new Vector2D(500, 500), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(500, 500), new Vector2D(900, 550), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddLine(DemoInfo, new Vector2D(400, 400), new Vector2D(600, 300), 30, Scalar.PositiveInfinity).IgnoresGravity = true;
            DemoHelper.AddRagDoll(DemoInfo, new Vector2D(200, 400));
            DemoHelper.AddRagDoll(DemoInfo, new Vector2D(300, 300));
            DemoHelper.AddRagDoll(DemoInfo, new Vector2D(400, 200));
            DemoHelper.AddRagDoll(DemoInfo, new Vector2D(500, 100));
            DemoHelper.AddRagDoll(DemoInfo, new Vector2D(600, 0));
            DemoHelper.AddRagDoll(DemoInfo, new Vector2D(700, -100));
        }
        protected override void Dispose(bool disposing)
        {
            if (dispose != null) { dispose(); }
        }
    }
}