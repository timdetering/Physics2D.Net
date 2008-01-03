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
using System.Collections.ObjectModel;
using System.Text;
using System.Runtime.InteropServices;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Physics2DDotNet.PhysicsLogics;
using Tao.OpenGl;

using SdlDotNet.Core;
using SdlDotNet.Graphics;
using SdlDotNet.Input;
using SdlDotNet.OpenGl;
namespace Graphics2DDotNet
{
    public class RaysSegmentsDrawable : IDrawable
    {
        class RaysSegmentsState : IDrawableState
        {
            public BodyGraphic parent;
            public RaySegmentsCollisionLogic logic;
            public Scalar[] lenghts;
            public RaysSegmentsState(int count)
            {
                lenghts = new Scalar[count];
            }
            public void OnPending(IGraphic parent)
            {
                this.parent = parent as BodyGraphic;
                logic = new RaySegmentsCollisionLogic(this.parent.Body);
                parent.Parent.Engine.AddLogic(logic);
                logic.NewInfo += new EventHandler(logic_NewInfo);
            }
            void logic_NewInfo(object sender, EventArgs e)
            {
                for (int index = 0; index < lenghts.Length; ++index)
                {
                    lenghts[index] = logic.Collisions[index].Distance;
                }
            }

        }

        object tag;
        RaySegmentsShape shape;
        Vector2D[] array;
        public RaysSegmentsDrawable(RaySegmentsShape shape)
        {
            this.shape = shape;
            this.array = new Vector2D[shape.Segments.Length * 2];
            for (int index = 0; index < shape.Segments.Length; ++index)
            {
                array[index * 2] = shape.Segments[index].RayInstance.Origin;
                array[index * 2 + 1] = shape.Segments[index].RayInstance.Origin + shape.Segments[index].RayInstance.Direction * shape.Segments[index].Length;
            }
        }
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public IDrawableState CreateState()
        {
            return new RaysSegmentsState(shape.Segments.Length);
        }

        public void Draw(DrawInfo drawInfo, IDrawableState state)
        {
            RaysSegmentsState st = state as RaysSegmentsState;
            for (int index = 0; index < st.lenghts.Length; ++index)
            {
                RaySegment segment = shape.Segments[index];
                Ray ray = segment.RayInstance;
                Scalar length = (st.lenghts[index] == -1) ? (segment.Length) : (st.lenghts[index]);
                int destIndex = index * 2 + 1;
                array[destIndex].X = ray.Origin.X + ray.Direction.X * length;
                array[destIndex].Y = ray.Origin.Y + ray.Direction.Y * length;
            }
            Gl.glLineWidth(2);
            Gl.glColor3f(1, 1, 1);
            Gl.glBegin(Gl.GL_LINES);
            for (int index = 0; index < array.Length; ++index)
            {
                GlHelper.GlVertex(array[index]);
            }
            Gl.glEnd();
        }
    }
}