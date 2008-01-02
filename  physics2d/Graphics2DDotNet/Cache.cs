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
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Tao.OpenGl;
using SdlDotNet;
using SdlDotNet.Graphics;
using Color = System.Drawing.Color;
namespace Graphics2DDotNet
{
    public static class Cache<TValue>
    {
        sealed class TypeComparer : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
        static TypeComparer typeComparer = new TypeComparer();


        static object syncRoot;
        static ICacheMethods<TValue> methods;

        static bool IsCacheLoaderInterface(Type type, object filterCriteria)
        {
            return type == typeof(ICacheMethods<TValue>);
        }
        static bool IsCacheLoader(Type type, object filterCriteria)
        {
            return type.FindInterfaces(IsCacheLoaderInterface, null).Length > 0;
        }
        private static Dictionary<string, WeakReference> cache;
        static Cache()
        {


            List<Type> types = new List<Type>();
            foreach (Module module in Assembly.GetCallingAssembly().GetLoadedModules(true))
            {
                types.AddRange(module.FindTypes(IsCacheLoader, null));
            }
            foreach (Module module in Assembly.GetEntryAssembly().GetLoadedModules(true))
            {
                types.AddRange(module.FindTypes(IsCacheLoader, null));
            }
            foreach (Module module in Assembly.GetExecutingAssembly().GetLoadedModules(true))
            {
                types.AddRange(module.FindTypes(IsCacheLoader, null));
            }
            types.Sort(typeComparer);
            Type last = null;
            types.RemoveAll(delegate(Type t)
            {
                bool result = t == last;
                last = t;
                return result;
            });
            if (types.Count != 1)
            {
                throw new Exception("TODO");
            }
            methods = (ICacheMethods<TValue>) types[0].GetConstructor(Type.EmptyTypes).Invoke(null);
            cache = new Dictionary<string, WeakReference>();
            syncRoot = new object();
        }

        public static TValue GetItem(string name,params object[] loadArgs)
        {
            if (name == null) { throw new ArgumentNullException("name"); }
            lock (syncRoot)
            {
                WeakReference weakReference;
                if (cache.TryGetValue(name, out weakReference))
                {
                    if (weakReference.IsAlive)
                    {
                        return (TValue)weakReference.Target;
                    }
                }
                TValue result = methods.LoadItem(name, loadArgs);
                cache[name] = new WeakReference(result);
                return result;
            }
        }
        public static bool Remove(string name)
        {
            if (name == null) { throw new ArgumentNullException("name"); }
            lock (syncRoot)
            {
                bool result = false;
                WeakReference weakReference;
                if (cache.TryGetValue(name, out weakReference))
                {
                    if (weakReference.IsAlive)
                    {
                        methods.DisposeItem((TValue)weakReference.Target);
                        weakReference.Target = null;
                        result = true;
                    }
                    cache.Remove(name);
                }
                return result;
            }
        }
        public static void Clear()
        {
            lock (syncRoot)
            {
                foreach (WeakReference weakReference in cache.Values)
                {
                    if (weakReference.IsAlive)
                    {
                        methods.DisposeItem((TValue)weakReference.Target);
                        weakReference.Target = null;
                    }
                }
                cache.Clear();
            }
        }
    }
}