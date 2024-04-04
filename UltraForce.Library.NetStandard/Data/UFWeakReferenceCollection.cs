// <copyright file="UFWeakReferenceCollection.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (C) 2018 Ultra Force Development
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
// </license>

using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraForce.Library.NetStandard.Data
{
  /// <summary>
  /// This collection can be used to track a collection of objects that might get garbage collected at one point.
  /// <para>
  /// Use <see cref="Add(T)" /> or <see cref="Add(IEnumerable{T})"/> to add items. If possible an object can
  /// call <see cref="Disposing" /> from its destructor or <see cref="IDisposable.Dispose"/> method to clean up
  /// dead references.
  /// </para>
  /// <para>
  /// Use <see cref="GetExistingItems" /> to get a list of all items that are still existing.
  /// </para>
  /// <para>
  /// The class can also act as a pool of objects, use the <see cref="Pop"/> to get an existing item.
  /// </para>
  /// </summary>
  /// <typeparam name="T">Type to track references to</typeparam>
  public class UFWeakReferenceCollection<T> where T : class
  {
    #region private variables

    /// <summary>
    /// List of weak references to added items
    /// </summary>
    private readonly List<WeakReference> m_items = new List<WeakReference>();

    /// <summary>
    /// Threshold determines how often to clean up when objects are capable
    /// of running Disposing
    /// </summary>
    private readonly int m_disposeThreshold;

    /// <summary>
    /// Number of Disposing calls since last clean up.
    /// </summary>
    private int m_disposeCount;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFWeakReferenceCollection{T}" />.
    /// <para>
    /// The threshold indicates after how many <see cref="Disposing" /> calls
    /// the class will clean up the internally managed list by removing all
    /// references to objects that are no longer existing.
    /// </para>
    /// </summary>
    /// <param name="aThreshold">Clean up threshold</param>
    public UFWeakReferenceCollection(int aThreshold = 100)
    {
      this.m_disposeThreshold = aThreshold;
      this.m_disposeCount = 0;
    }

    #endregion

    #region public methods

    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    /// <param name="anItem">Item to add</param>
    public void Add(T anItem)
    {
      // when capacity is reached remove dead references first
      if (this.m_items.Capacity == this.m_items.Count)
      {
        this.RemoveDeadReferences();
      }
      this.m_items.Add(new WeakReference(anItem));
    }

    /// <summary>
    /// Adds multiple items to the collection.
    /// </summary>
    /// <param name="anItems">Items to add</param>
    public void Add(IEnumerable<T> anItems)
    {
      foreach (T item in anItems)
      {
        this.Add(item);
      }
    }

    /// <summary>
    /// Removes an item from the collection.
    /// </summary>
    /// <param name="anItem">Item to remove</param>
    public void Remove(T anItem)
    {
      WeakReference? item = this.Find(anItem);
      if (item != null)
      {
        this.m_items.Remove(item);
      }
    }

    /// <summary>
    /// Gets and removes an existing item from the end of the list. 
    /// </summary>
    /// <returns>Instance or null if there are no existing items</returns>
    public T? Pop()
    {
      // process from end to start
      for (int index = this.m_items.Count - 1; index >= 0; index--)
      {
        // shortcut and remove it (either it is used or the target is no longer available)
        WeakReference? reference = this.m_items[index];
        this.m_items.RemoveAt(index);
        if (reference.IsAlive)
        {
          return (T)reference.Target;
        }
      }
      return null;
    }

    /// <summary>
    /// Checks if the collection already contains a reference to a certain item.
    /// </summary>
    /// <param name="anItem">Item to check</param>
    /// <returns><c>True</c> if item is in the collection</returns>
    public bool Contains(T anItem)
    {
      return this.m_items.Select(weakItem => weakItem.Target as T).Any(item => item == anItem);
    }

    /// <summary>
    /// An object that is part of the collection can call this method from its destructor or
    /// <see cref="IDisposable.Dispose" /> method.
    /// <para>
    /// The method will keep track of the number of calls and cleans up dead references once the threshold has been
    /// reached.
    /// </para>
    /// </summary>
    /// <param name="anItem">Item being disposed</param>
    public void Disposing(T anItem)
    {
      this.m_disposeCount++;
      if (this.m_disposeCount > this.m_disposeThreshold)
      {
        this.RemoveDeadReferences();
      }
    }

    /// <summary>
    /// Clears all referenced items.
    /// </summary>
    public void Clear()
    {
      this.m_items.Clear();
      this.m_disposeCount = 0;
    }

    /// <summary>
    /// Gets all items that are still existing. Take note that the items will not be garbage collected until the
    /// returned list instance is garbage collected.
    /// </summary>
    /// <returns>List containing all existing items</returns>
    public List<T> GetExistingItems()
    {
      return this.m_items.Select(weakItem => weakItem.Target).OfType<T>().ToList();
    }

    #endregion

    #region private methods

    /// <summary>
    /// Remove all items that are no longer alive.
    /// </summary>
    private void RemoveDeadReferences()
    {
      this.m_items.RemoveAll(w => !w.IsAlive);
      this.m_disposeCount = 0;
    }

    /// <summary>
    /// Finds an weak reference to a certain instance.
    /// </summary>
    /// <param name="anItem">
    /// Item to find reference for
    /// </param>
    /// <returns>
    /// <see cref="WeakReference"/> instance or <c>null</c> if none could be
    /// found.
    /// </returns>
    private WeakReference? Find(T anItem)
    {
      return this.m_items.FirstOrDefault(item => item.Target == anItem);
    }

    #endregion
  }
}