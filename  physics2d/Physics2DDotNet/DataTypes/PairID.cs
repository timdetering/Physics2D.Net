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
using System.Runtime.InteropServices;

using AdvanceMath;
using Physics2DDotNet.Math2D;

namespace Physics2DDotNet
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PairID
    {
        public static long GetId(int ID1, int ID2)
        {
            PairID result;
            result.ID = 0;
            if (ID1 > ID2)
            {
                result.lowID = ID2;
                result.highID = ID1;
            }
            else
            {
                result.lowID = ID1;
                result.highID = ID2;
            }
            return result.ID;
        }
        public static void GetIds(long id,out int ID1,out  int ID2)
        {
            PairID result;
            result.lowID = 0;
            result.highID = 0;
            result.ID = id;
            ID1 = result.lowID;
            ID2 = result.highID;
        }
        [FieldOffset(0)]
        long ID;
        [FieldOffset(0)]
        int lowID;
        [FieldOffset(sizeof(int))]
        int highID;
    }
}