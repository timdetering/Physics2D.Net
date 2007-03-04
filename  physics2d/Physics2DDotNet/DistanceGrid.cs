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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;

using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    /// <summary>
    /// A class used by some Shape Objects for Narrow Phased collision.
    /// </summary>
    [Serializable]
    public sealed class DistanceGrid
    {
        Scalar gridSpacing;
        Scalar gridSpacingInv;
        BoundingBox2D box;
        Scalar[][] nodes;
        public DistanceGrid(Shape shape, Scalar spacing)
        {
            if (shape == null) { throw new ArgumentNullException("shape"); }
            if (spacing <= 0) { throw new ArgumentOutOfRangeException("spacing"); }
            //prepare the shape.
            Matrix2D old = shape.Matrix;
            Matrix2D ident = Matrix2D.Identity;
            shape.ApplyMatrix(ref ident);
            shape.CalcBoundingBox2D();

            this.box = shape.BoundingBox2D; 
            this.gridSpacing = spacing;
            this.gridSpacingInv = 1 / spacing;
            int xSize = (int)Math.Ceiling((box.Upper.X - box.Lower.X) * gridSpacingInv) + 2;
            int ySize = (int)Math.Ceiling((box.Upper.Y - box.Lower.Y) * gridSpacingInv) + 2;

            this.nodes = new Scalar[xSize][];
            for (int index = 0; index < xSize; ++index)
            {
                this.nodes[index] = new Scalar[ySize];
            }
            Vector2D vector;
            vector.X = box.Lower.X;
            for (int x = 0; x < xSize; ++x, vector.X += spacing)
            {
                vector.Y = box.Lower.Y;
                for (int y = 0; y < ySize; ++y, vector.Y += spacing)
                {
                    nodes[x][ y] = shape.GetDistance(vector);
                }
            }
            //restore the shape
            shape.ApplyMatrix(ref old);
            shape.CalcBoundingBox2D();
        }
        public bool TryGetIntersection(Vector2D vector, out IntersectionInfo result)
        {
            if (BoundingBox2D.TestIntersection(ref box, ref vector))
            {
                int x = (int)Math.Floor((vector.X - box.Lower.X) * gridSpacingInv);
                int y = (int)Math.Floor((vector.Y - box.Lower.Y) * gridSpacingInv);

                Scalar bottomLeft = nodes[x][ y];
                Scalar bottomRight = nodes[x + 1][ y];
                Scalar topLeft = nodes[x][ y + 1];
                Scalar topRight = nodes[x + 1][ y + 1];

                if (bottomLeft <= 0 ||
                    bottomRight <= 0 ||
                    topLeft <= 0 ||
                    topRight <= 0)
                {
                    Scalar xPercent = (vector.X - (gridSpacing * x + box.Lower.X)) * gridSpacingInv;
                    Scalar yPercent = (vector.Y - (gridSpacing * y + box.Lower.Y)) * gridSpacingInv;

                    Scalar top, bottom, distance;

                    MathHelper.Lerp(ref topLeft, ref topRight, ref xPercent, out top);
                    MathHelper.Lerp(ref bottomLeft, ref bottomRight, ref xPercent, out bottom);
                    MathHelper.Lerp(ref bottom, ref top, ref yPercent, out distance);

                    if (distance <= 0)
                    {
                        Scalar right, left;

                        MathHelper.Lerp(ref bottomRight, ref topRight, ref yPercent, out right);
                        MathHelper.Lerp(ref bottomLeft, ref topLeft, ref yPercent, out left);

                        Vector2D normal;
                        normal.X = right - left;
                        normal.Y = top - bottom;
                        Vector2D.Normalize(ref normal, out normal);
                        result = new IntersectionInfo(vector, normal, distance);
                        return true;
                    }
                }
            }
            result = null;
            return false;
        }
    }
}