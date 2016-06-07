//---------------------------------------------------------------------------------------------------
// <copyright file="ListWithChangedEvent.cs" company="Alstom">
//          (c) Copyright ALSTOM 2013.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections;

namespace PIS.Ground.Core.Data
{
    /// <summary>A delegate type for hooking up change notifications.</summary>
    /// <param name="sender">Source of the event.</param>
    /// <param name="e">Event information.</param>
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// A class that works just like ArrayList, but sends event notifications whenever the list
    /// changes.
    /// </summary>
    class ListWithChangedEvent: ArrayList
    {
        /// <summary>Initializes a new instance of the ListWithChangedEvent class.</summary>
        public ListWithChangedEvent()
        {
            
        }

        /// <summary>Initializes a new instance of the ListWithChangedEvent class.</summary>
        /// <param name="capacity">The capacity of the list.</param>
        public ListWithChangedEvent(int capacity)
            : base(capacity)
        {
            
        }

        /// <summary>Initializes a new instance of the ListWithChangedEvent class.</summary>
        /// <param name="c">The ICollection to process as value for the list.</param>
        public ListWithChangedEvent(ICollection c)
            : base(c)
        {
            
        }

        /// <summary>
        /// An event that clients can use to be notified whenever the elements of the list change.
        /// </summary>
        public event ChangedEventHandler changed;

        /// <summary>Lock for list access.</summary>
        protected static object _lock = new object();           

        /// <summary>Invoke the Changed event; called whenever list changes.</summary>
        /// <param name="e">Event information to send to registered event handlers.</param>
        protected virtual void OnChanged(EventArgs e)
        {
            if (changed != null)
                changed(this, e);
        }

        /// <summary>
        /// Override some of the methods that can change the list;
        /// invoke event after each.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> to be added to the end of the
        /// <see cref="T:System.Collections.ArrayList" />The object to add to the list. The value can be null.</param>
        /// <returns>.</returns>
        public override int Add(object value)
        {
            int i = -1;
            lock (_lock)
            {
                i = base.Add(value);
                OnChanged(EventArgs.Empty);
            }
            return i;
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.ArrayList" />.
        /// </summary>
        public override void Clear()
        {
            lock (_lock)
            {
                base.Clear();
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>Indexer to set items within this collection using array index syntax.</summary>
        /// <param name="index">Zero-based index of the entry to access.</param>
        /// <returns>The indexed item.</returns>
        public override object this[int index]
        {
            set
            {
                lock (_lock)
                {
                    base[index] = value;
                    OnChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>Pops the first element without generating an event.</summary>
        /// <returns>The first element popped from the list.</returns>
        public object PopNoEvent()
        {
            object value = null;
            lock (_lock)
            {
                if (base.Count > 0)
                {
                    value = base[0];
                    base.RemoveAt(0);
                }
            }
            return value;
        }

    }
}
