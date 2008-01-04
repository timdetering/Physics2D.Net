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
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Collections;
using Tao.OpenGl;


namespace Graphics2DDotNet
{
    public sealed class DrawInfo
    {
        private Scalar trueDt;
        private Scalar trueDtInv;
        private Scalar dt;
        private Scalar dtInv;
        private int drawCount;
        private int refreshCount;
        /// <summary>
        /// Creates a new DrawInfo Instance.
        /// </summary>
        /// <param name="dt"> The current change in time. (seconds)</param>
        /// <param name="updateCount"></param>
        /// <param name="refreshCount"></param>
        public DrawInfo(Scalar dt, Scalar trueDt, int drawCount, int refreshCount)
        {
            this.dt = dt;
            this.dtInv = (dt > 0) ? (1 / dt) : (0);
            this.trueDt = trueDt;
            this.trueDtInv = (trueDt > 0) ? (1 / trueDt) : (0);
            this.drawCount = drawCount;
            this.refreshCount = refreshCount;
        }
        /// <summary>
        /// The number of times the Gl has been refreshed.
        /// </summary>
        public int RefreshCount { get { return refreshCount; } }
        /// <summary>
        /// The current change in time. (seconds)
        /// </summary>
        public Scalar Dt { get { return dt; } }
        /// <summary>
        /// The inverse of the change in time. (0 if dt is 0)
        /// </summary>
        public Scalar DtInv { get { return dtInv; } }
        /// <summary>
        /// The actaul change in time. (seconds)
        /// </summary>
        public Scalar TrueDt { get { return trueDt; } }
        /// <summary>
        /// The inverse of the actaul change in time. (0 if dt is 0)
        /// </summary>
        public Scalar TrueDtInv { get { return trueDtInv; } }
        /// <summary>
        /// The number for the current draw.
        /// </summary>
        public int DrawCount { get { return drawCount; } }
    }

}