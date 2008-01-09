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
using System.Drawing;
using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Collections;
using Tao.OpenGl;
using SdlDotNet.Graphics;
using SdlDotNet.Core;
using SdlDotNet.Input;

namespace Graphics2DDotNet
{


    public class PendableCollection<TParent, TChild>
        where TParent : class
        where TChild : Pendable<TParent>
    {
        class ZOrderComparer : IComparer<TChild>
        {
            public int Compare(TChild x, TChild y)
            {
                int result = x.ZOrder.CompareTo(y.ZOrder);
                if (result == 0)
                {
                    result = x.ID.CompareTo(y.ID);
                }
                return result;
            }
        }
        static ZOrderComparer zOrderComparer = new ZOrderComparer();
        public event EventHandler<CollectionEventArgs<TChild>> ItemsAdded;
        public event EventHandler<CollectionEventArgs<TChild>> ItemsRemoved;
        const int firstID = 1;
        private int idCounter = firstID;
        private bool zOrderChanged;
        List<TChild> pendingItems;
        List<TChild> items;
        List<TChild> removedItems;
        TParent parent;
        object sender;
        public PendableCollection(TParent parent)
            : this(parent, parent)
        { }
        public PendableCollection(TParent parent, object sender)
        {
            this.idCounter = firstID;
            this.parent = parent;
            this.pendingItems = new List<TChild>();
            this.items = new List<TChild>();
            this.removedItems = new List<TChild>();
            this.sender = sender;
        }
        public List<TChild> Items { get { return items; } }

        public void Add(TChild item)
        {
            item.PreCheckInternal();
            item.CheckInternal();
            item.OnPendingInternal(parent);
            pendingItems.Add(item);
        }
        public void AddRange(ICollection<TChild> collection)
        {
            if (collection == null) { throw new ArgumentNullException("collection"); }
            if (collection.Count == 0) { return; }
            foreach (TChild item in collection)
            {
                item.PreCheckInternal();
            }
            foreach (TChild item in collection)
            {
                item.CheckInternal();
            }
            foreach (TChild item in collection)
            {
                item.OnPendingInternal(parent);
            }
            pendingItems.AddRange(collection);
        }
        public void AddPending()
        {
            items.AddRange(pendingItems);
            for (int index = 0; index < pendingItems.Count; ++index)
            {
                pendingItems[index].OnAddedInternal(++idCounter);
                pendingItems[index].ZOrderChanged += OnZOrderChanged;
            }
            if (ItemsAdded != null) { ItemsAdded(sender, new CollectionEventArgs<TChild>(pendingItems.AsReadOnly())); }
            pendingItems.Clear();
            zOrderChanged = true;
        }
        public void CheckZOrder()
        {
            if (zOrderChanged)
            {
                zOrderChanged = false;
                items.Sort(zOrderComparer);
            }
        }

        void OnZOrderChanged(object sender, EventArgs e)
        {
            zOrderChanged = true;
        }
        private bool IsItemExpired(TChild item)
        {
            if (!item.Lifetime.IsExpired) { return false; }
            if (ItemsRemoved != null) { removedItems.Add(item); }
            item.OnRemovedInternal();
            item.ZOrderChanged -= OnZOrderChanged;
            return true;
        }
        public void RemoveExpired()
        {
            if (items.RemoveAll(IsItemExpired) == 0) { return; }
            if (ItemsRemoved != null)
            {
                ItemsRemoved(sender, new CollectionEventArgs<TChild>(removedItems.AsReadOnly()));
                removedItems.Clear();
            }
        }
        public void Clear()
        {
            ClearPending();
            ClearAdded();
            this.idCounter = firstID;
        }
        private void ClearPending()
        {
            if (this.pendingItems.Count == 0) { return; }
            List<TChild> pendingItems = this.pendingItems;
            this.pendingItems = new List<TChild>();
            for (int index = 0; index < pendingItems.Count; ++index)
            {
                pendingItems[index].OnRemovedInternal();
            }
            pendingItems.Clear();
        }
        private void ClearAdded()
        {
            if (items.Count == 0) { return; }
            for (int index = 0; index < items.Count; ++index)
            {
                items[index].OnRemovedInternal();
            }
            if (ItemsRemoved != null)
            {
                ItemsRemoved(sender, new CollectionEventArgs<TChild>(items.AsReadOnly()));
            }
            items.Clear();
        }
    }
}