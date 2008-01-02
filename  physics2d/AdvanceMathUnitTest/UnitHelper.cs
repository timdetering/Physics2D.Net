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
using NUnit.Framework;



namespace AdvanceMath.UnitTest
{
    public static class UnitHelper
    {
        public const Scalar Tolerance = .000001f;


        public static void RefOperationTester<T>(T left, T right, T expected, RefOperation<T, T, T> op, string description)
        {
            RefOperationTester<T, T, T>(left, right, expected, op, description);
            RefOperationTesterLeftSameInternal<T, T>(left, right, expected, op, description);
            RefOperationTesterRightSameInternal<T, T>(left, right, expected, op, description);
        }
        public static void RefOperationTesterLeftSame<TRight, TResult>(TResult left, TRight right, TResult expected, RefOperation<TResult, TRight, TResult> op, string description)
        {
            RefOperationTester<TResult, TRight, TResult>(left, right, expected, op, description);
            RefOperationTesterLeftSameInternal<TRight, TResult>(left, right, expected, op, description);
        }
        public static void RefOperationTesterRightSame<TLeft, TResult>(TLeft left, TResult right, TResult expected, RefOperation<TLeft, TResult, TResult> op, string description)
        {
            RefOperationTester<TLeft, TResult, TResult>(left, right, expected, op, description);
            RefOperationTesterRightSameInternal<TLeft, TResult>(left, right, expected, op, description);
        }
        public static void RefOperationTester<TLeft, TRight, TResult>(TLeft left, TRight right, TResult expected, RefOperation<TLeft, TRight, TResult> op, string description)
        {
            TLeft op1 = left;
            TRight op2 = right;
            TResult result;
            op(ref op1, ref op2, out result);
            Assert.AreEqual(op1, left, description + " RefOp: left unchanged");
            Assert.AreEqual(op2, right, description + " RefOp: right unchanged");
            Assert.AreEqual(result, expected, description + " RefOp: expected");
        }
        static void RefOperationTesterLeftSameInternal<TRight, TResult>(TResult left, TRight right, TResult expected, RefOperation<TResult, TRight, TResult> op, string description)
        {
            TRight op2 = right;
            TResult result = left;
            op(ref result, ref op2, out result);
            Assert.AreEqual(op2, right, description + " RefOp: right unchanged in left same");
            Assert.AreEqual(result, expected, description + " RefOp: expected in left same");
        }
        static void RefOperationTesterRightSameInternal<TLeft, TResult>(TLeft left, TResult right, TResult expected, RefOperation<TLeft, TResult, TResult> op, string description)
        {
            TLeft op1 = left;
            TResult result = right;
            op(ref op1, ref result, out result);
            Assert.AreEqual(op1, left, description + " RefOp: left unchanged in right same");
            Assert.AreEqual(result, expected, description + " RefOp: expected in right same");
        }
    }
}