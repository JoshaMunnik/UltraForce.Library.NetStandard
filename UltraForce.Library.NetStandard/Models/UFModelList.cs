// <copyright file="UFModelList.cs" company="Ultra Force Development">
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Events;
using UltraForce.Library.NetStandard.Interfaces;
using UltraForce.Library.NetStandard.Storage;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Models
{
  /// <summary>
  /// Implements a generic <see cref="IList" /> adding support for certain
  /// <see cref="UFModel.Option"/> values and implementing various 
  /// <see cref="UFModel"/> methods.
  /// </summary>
  /// <remarks>
  /// <see cref="UFModelList{TValue}" /> will fire a 
  /// <see cref="UFModel.DataChanged" /> event whenever the contents of the 
  /// list changes.
  /// <para>
  /// If <see cref="UFModel.Option.LockChildren" /> option is set and TValue 
  /// implements the <see cref="IUFLockable"/> interface, <see cref="Lock"/>
  /// will also lock all items within the list.
  /// </para>
  /// <para>
  /// If <see cref="UFModel.Option.TrackChildChange" /> option is set and 
  /// TValue implements <see cref="IUFNotifyDataChanged"/>,  
  /// <see cref="UFModelList{TValue}" /> will install change listeners with 
  /// every item and will fire <see cref="UFModel.DataChanged" />, 
  /// <see cref="UFModel.ChildChanged" /> and 
  /// <see cref="UFModel.PropertyChanged" /> events if an item changes. 
  /// </para>
  /// <para>
  /// <see cref="UFModelList{TValue}" /> will make sure to install only one 
  /// listener, even if the same instance is added multiple times.
  /// </para>
  /// </remarks>
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  public class UFModelList<TValue> : UFModel, IList<TValue>
  {
    #region private consts

    /// <summary>
    /// Key used to store and retrieve count value with
    /// </summary>
    private const string CountKey = "count";

    /// <summary>
    /// Key used to store retrieve item value with (index will be concatenated 
    /// to it)
    /// </summary>
    private const string ItemKey = "item_";

    #endregion

    #region private variables

    /// <summary>
    /// UFModelList uses a generic List to contain the data.
    /// </summary>
    private readonly List<TValue> m_list;

    /// <summary>
    /// Lookup is used when tracking child changes in case a value is 
    /// assigned to multiple indexes.
    /// </summary>
    private readonly Dictionary<TValue, List<int>>? m_indexLookup;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UFModelList{TValue}"/> 
    /// class.
    /// </summary>
    /// <param name='anOptions'>
    /// <see cref="UFModel.Option"/>
    /// </param>
    public UFModelList(params Option[] anOptions) : base(anOptions)
    {
      this.m_list = new List<TValue>();
      // create lookup table
      if (
        this.HasOption(Option.TrackChildChange)
        && UFObjectTools.Implements<TValue, IUFNotifyDataChanged>()
      )
      {
        this.m_indexLookup = new Dictionary<TValue, List<int>>();
      }
    }

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFModelList{TValue}"/> class.
    /// </summary>
    /// <param name='aCapacity'>
    /// Initial list capacity.
    /// </param>
    /// <param name='anOptions'>
    /// <see cref="UFModel.Option"/>
    /// </param>
    public UFModelList(int aCapacity, params Option[] anOptions)
      : base(anOptions)
    {
      this.m_list = new List<TValue>(aCapacity);
      // create lookup table
      if (
        this.HasOption(Option.TrackChildChange)
        && UFObjectTools.Implements<TValue, IUFNotifyDataChanged>()
      )
      {
        this.m_indexLookup = new Dictionary<TValue, List<int>>();
      }
    }

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFModelList{TValue}"/> class.
    /// </summary>
    /// <param name='aCollection'>
    /// An initial collection to fill list with
    /// </param>
    /// <param name='anOptions'>
    /// <see cref="UFModel.Option"/>
    /// </param>
    public UFModelList(IEnumerable<TValue> aCollection, params Option[] anOptions) : base(anOptions)
    {
      this.m_list = new List<TValue>(aCollection);
      // create lookup table
      if (
        this.HasOption(Option.TrackChildChange)
        && UFObjectTools.Implements<TValue, IUFNotifyDataChanged>()
      )
      {
        this.m_indexLookup = new Dictionary<TValue, List<int>>();
        // process internal list.
        this.ProcessValues();
      }
    }

    #endregion

    #region public methods

    /// <inheritdoc />
    public override void Clear(bool aCallChanged)
    {
      base.Clear(aCallChanged);
      this.RemoveValues();
    }

    /// <summary>
    /// Shuffle the elements inside the list.
    /// </summary>
    public void Shuffle()
    {
      this.Shuffle(0, this.Count);
    }

    /// <summary>
    /// Shuffle a part of the list.
    /// </summary>
    /// <param name='aStart'>
    /// Start index.
    /// </param>
    /// <param name='aCount'>
    /// Number of elements.
    /// </param>
    public virtual void Shuffle(int aStart, int aCount)
    {
      Random random = new Random();
      int n = aCount;
      while (n > 1)
      {
        int k = random.Next(n--);
        (this.m_list[n + aStart], this.m_list[k + aStart]) = 
          (this.m_list[k + aStart], this.m_list[n + aStart]);
      }
      this.Changed();
    }

    /// <summary>
    /// Find first value that matches aMatch.
    /// </summary>
    /// <returns>An instance or null if no match was found</returns>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    public TValue Find(Predicate<TValue> aMatch)
    {
      return this.m_list.Find(aMatch);
    }

    /// <summary>
    /// Find last value that matches aMatch.
    /// </summary>
    /// <returns>An instance or null if no match was found</returns>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    public TValue FindLast(Predicate<TValue> aMatch)
    {
      return this.m_list.FindLast(aMatch);
    }

    /// <summary>
    /// Find index of first value that matches aMatch.
    /// </summary>
    /// <returns>The index or -1 if not found</returns>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    public int FindIndex(Predicate<TValue> aMatch)
    {
      return this.m_list.FindIndex(aMatch);
    }

    /// <summary>
    /// Find index of first value that matches aMatch.
    /// </summary>
    /// <returns>The index or -1 if not found</returns>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    /// <param name='aStart'>
    /// Index to searching from.
    /// </param>
    public int FindIndex(int aStart, Predicate<TValue> aMatch)
    {
      return this.m_list.FindIndex(aStart, aMatch);
    }

    /// <summary>
    /// Find index of first value that matches aMatch.
    /// </summary>
    /// <returns>The index or -1 if not found</returns>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    /// <param name='aStart'>
    /// Index to searching from.
    /// </param>
    /// <param name='aCount'>
    /// Max number of values to test.
    /// </param>
    public int FindIndex(int aStart, int aCount, Predicate<TValue> aMatch)
    {
      return this.m_list.FindIndex(aStart, aCount, aMatch);
    }

    /// <summary>
    /// Find index of last value that matches aMatch.
    /// </summary>
    /// <returns>The index or -1 if not found</returns>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    public int FindLastIndex(Predicate<TValue> aMatch)
    {
      return this.m_list.FindLastIndex(aMatch);
    }

    /// <summary>
    /// Find index of last value that matches aMatch.
    /// </summary>
    /// <returns>The index or -1 if not found</returns>
    /// <param name='aStart'>
    /// Index to searching from (backwards, so Index, Index-1, Index-2, etc).
    /// </param>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    public int FindLastIndex(int aStart, Predicate<TValue> aMatch)
    {
      return this.m_list.FindLastIndex(aStart, aMatch);
    }

    /// <summary>
    /// Find index of last value that matches aMatch.
    /// </summary>
    /// <returns>The index or -1 if not found</returns>
    /// <param name='aStart'>
    /// Index to searching from (backwards, so Index, Index-1, Index-2, etc).
    /// </param>
    /// <param name='aCount'>
    /// Max number of values to test.
    /// </param>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    public int FindLastIndex(
      int aStart,
      int aCount,
      Predicate<TValue> aMatch
    )
    {
      return this.m_list.FindLastIndex(aStart, aCount, aMatch);
    }

    /// <summary>
    /// Find all values that match aMatch.
    /// </summary>
    /// <returns>A list instance containing all the values.</returns>
    /// <param name='aMatch'>
    /// Match predicate
    /// </param>
    public List<TValue> FindAll(Predicate<TValue> aMatch)
    {
      return this.m_list.FindAll(aMatch);
    }

    /// <summary>
    /// Return the first index of a value starting from a certain position.
    /// </summary>
    /// <returns>
    /// The index of the item or -1 if not found.
    /// </returns>
    /// <param name='aValue'>
    /// The value to get the index of
    /// </param>
    /// <param name='aStart'>
    /// Starting position
    /// </param>
    public int IndexOf(TValue aValue, int aStart)
    {
      return this.m_list.IndexOf(aValue, aStart);
    }

    /// <summary>
    /// Return the index of a value within a certain range.
    /// </summary>
    /// <returns>
    /// The index of the item or -1 if not found.
    /// </returns>
    /// <param name='aValue'>
    /// The value to get the index of
    /// </param>
    /// <param name='aStart'>
    /// A starting position.
    /// </param>
    /// <param name='aCount'>
    /// Number of items to check.
    /// </param>
    public int IndexOf(TValue aValue, int aStart, int aCount)
    {
      return this.m_list.IndexOf(aValue, aStart, aCount);
    }

    /// <summary>
    /// Perform action on each item.
    /// </summary>
    /// <param name='anAction'>
    /// An action for specific type.
    /// </param>
    public void ForEach(Action<TValue> anAction)
    {
      foreach (TValue value in this.m_list)
      {
        anAction(value);
      }
    }

    #endregion

    #region IUFLockable

    /// <summary>
    /// If <see cref="UFModel.Option.LockChildren"/> is set, call 
    /// <see cref="IUFLockable.Lock"/> on all non-null items that implement
    /// <see cref="IUFLockable"/>.
    /// </summary>
    public override int Lock()
    {
      int result = base.Lock();
      if (!this.HasOption(Option.LockChildren) || !UFObjectTools.Implements<TValue, IUFLockable>())
      {
        return result;
      }
      foreach (TValue item in this.m_list.Where(item => item != null))
      {
        ((IUFLockable)item!).Lock();
      }
      return result;
    }

    /// <summary>
    /// If <see cref="UFModel.Option.LockChildren"/> is set, call 
    /// <see cref="IUFLockable.Unlock"/> on all non-null items that implement
    /// <see cref="IUFLockable"/>.
    /// </summary>
    public override int Unlock()
    {
      int result = base.Unlock();
      if (!this.HasOption(Option.LockChildren) || !UFObjectTools.Implements<TValue, IUFLockable>())
      {
        return result;
      }
      foreach (TValue item in this.m_list.Where(item => item != null))
      {
        ((IUFLockable)item!).Unlock();
      }
      return result;
    }

    #endregion

    #region IUFStorableObject

    /// <summary>
    /// Stores the items in a keyed storage. 
    /// </summary>
    /// <param name="aStorage">A storage to store data in.</param>
    public override void SaveToStorage(UFKeyedStorage aStorage)
    {
      base.SaveToStorage(aStorage);
      aStorage.SetInt(CountKey, this.Count);
      for (int index = 0; index < this.Count; index++)
      {
        aStorage.SetObject(ItemKey + index, this[index]!, typeof(TValue));
      }
    }

    /// <summary>
    /// Gets the items from a keyed storage. 
    /// </summary>
    /// <param name="aStorage">A storage to get data from.</param>
    public override void LoadFromStorage(UFKeyedStorage aStorage)
    {
      // prevent events
      this.Lock();
      // load will also call Clear
      base.LoadFromStorage(aStorage);
      int count = aStorage.GetInt(CountKey);
      for (int index = 0; index < count; index++)
      {
        object? value = aStorage.GetObject(ItemKey + index, typeof(TValue));
        if (value != null)
        {
          this.Add((TValue)value);
        }
      }
      this.Unlock();
    }

    #endregion

    #region IUFJSONExport

    /// <summary>
    /// Save the items as an array. This method does not save any other
    /// property.
    /// </summary>
    /// <param name="aBuilder">A builder to add data to.</param>
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public override void SaveJson(StringBuilder aBuilder)
    {
      UFJsonTools.SaveList(aBuilder, (IList)this);
    }

    #endregion

    #region List[t] methods

    /// <summary>
    /// Copies list to an array.
    /// </summary>
    /// <param name='anArray'>
    /// An array to copy to.
    /// </param>
    public void CopyTo(TValue[] anArray)
    {
      this.m_list.CopyTo(anArray);
    }

    /// <summary>
    /// Copies part of the list to an array.
    /// </summary>
    /// <param name='anIndex'>
    /// Starting index in list
    /// </param>
    /// <param name='anArray'>
    /// An array to copy to.
    /// </param>
    /// <param name='anArrayIndex'>
    /// Starting index in the array.
    /// </param>
    /// <param name='aCount'>
    /// Number of elements.
    /// </param>
    public void CopyTo(
      int anIndex,
      TValue[] anArray,
      int anArrayIndex,
      int aCount
    )
    {
      this.m_list.CopyTo(anIndex, anArray, anArrayIndex, aCount);
    }

    /// <summary>
    /// Convert the list to an array.
    /// </summary>
    /// <returns>
    /// Array of objects of type <c>TValue</c> containing the items
    /// </returns>
    public TValue[] ToArray()
    {
      return this.m_list.ToArray();
    }

    /// <summary>
    /// Adds another collection to the list.
    /// </summary>
    /// <param name="aCollection">Collection to add</param>
    public virtual void AddRange(IEnumerable<TValue> aCollection)
    {
      int index = this.Count;
      IList<TValue> list = aCollection.ToList();
      this.m_list.AddRange(list);
      this.Changed();
    }

    /// <summary>
    /// Gets a shallow copy of part of the list.
    /// </summary>
    /// <param name="aStart">start of part</param>
    /// <param name="aCount">number of items</param>
    /// <returns>copy</returns>
    public IList<TValue> GetRange(int aStart, int aCount)
    {
      return this.m_list.GetRange(aStart, aCount);
    }

    #endregion

    #region list[t] properties

    /// <summary>
    /// The capacity of the list (see <see cref="List{T}.Capacity"/>)
    /// </summary>
    [UFIgnore]
    public int Capacity
    {
      get { return this.m_list.Capacity; }
      set { this.m_list.Capacity = value; }
    }

    #endregion

    #region ilist[t]

    /// <summary>
    /// Get first index of item.
    /// </summary>
    /// <returns>
    /// The index or -1 if not found.
    /// </returns>
    /// <param name='anItem'>
    /// The item to get the index for
    /// </param>
    public int IndexOf(TValue anItem)
    {
      return this.m_list.IndexOf(anItem);
    }

    /// <summary>
    /// Insert item at specified index. Will fire an DataChanged event.
    /// </summary>
    /// <param name='anIndex'>
    /// Index to insert at (must be a value between 0 and Count)
    /// </param>
    /// <param name='anItem'>
    /// Item to insert
    /// </param>
    public void Insert(int anIndex, TValue anItem)
    {
      this.m_list.Insert(anIndex, anItem);
      this.ValueAdded(anIndex, anItem, true, true);
    }

    /// <summary>
    /// Remove item at index. Will fire an DataChanged event.
    /// </summary>
    /// <param name='anIndex'>
    /// Index to remove item at.
    /// </param>
    public void RemoveAt(int anIndex)
    {
      TValue item = this.m_list[anIndex];
      this.m_list.RemoveAt(anIndex);
      this.ValueRemoved(anIndex, item, true);
    }

    /// <summary>
    /// Access an item at an index. When assigning a new value, the instance
    /// will fire an DataChanged event.
    /// </summary>
    /// <param name='anIndex'>
    /// Index in list.
    /// </param>
    [UFIgnore]
    public TValue this[int anIndex]
    {
      get { return this.m_list[anIndex]; }
      set
      {
        this.ValueRemoved(anIndex, this.m_list[anIndex], false);
        TValue old = this.m_list[anIndex];
        this.m_list[anIndex] = value;
        this.ValueAdded(anIndex, value, true, false, old);
      }
    }

    #endregion

    #region IEnumerable

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    public IEnumerator GetEnumerator()
    {
      return this.m_list.GetEnumerator();
    }

    #endregion

    #region IEnumerable[t]

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>
    /// The enumerator.
    /// </returns>
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
      return this.m_list.GetEnumerator();
    }

    #endregion

    #region ICollection[t]

    /// <summary>
    /// Add the item to the end of the list. Will fire an DataChanged event.
    /// </summary>
    /// <param name='anItem'>
    /// Item to add.
    /// </param>
    public void Add(TValue anItem)
    {
      this.m_list.Add(anItem);
      this.ValueAdded(this.m_list.Count - 1, anItem, true, true);
    }

    /// <summary>
    /// Check if the list contains the item.
    /// </summary>
    /// <param name='anItem'>
    /// <c>true</c> if the list contains the item, <c>false</c> if not.
    /// </param>
    public bool Contains(TValue anItem)
    {
      return this.m_list.Contains(anItem);
    }

    /// <summary>
    /// Copies list to an array.
    /// </summary>
    /// <param name='anArray'>
    /// Array.
    /// </param>
    /// <param name='anArrayIndex'>
    /// Start in anArray to copy to
    /// </param>
    public void CopyTo(TValue[] anArray, int anArrayIndex)
    {
      this.m_list.CopyTo(anArray, anArrayIndex);
    }

    /// <summary>
    /// Remove the first occurrence of an item.
    /// </summary>
    /// <returns>
    /// <c>true</c> if item was removed, <c>false</c> if item could not 
    /// be found.
    /// </returns>
    /// <param name='anItem'>
    /// Item to remove
    /// </param>
    public bool Remove(TValue anItem)
    {
      int index = this.m_list.IndexOf(anItem);
      if (index < 0)
      {
        return false;
      }
      this.RemoveAt(index);
      return true;
    }

    /// <summary>
    /// Get the number of items
    /// </summary>
    /// <value>
    /// The number of items
    /// </value>
    [UFIgnore]
    public int Count
    {
      get { return this.m_list.Count; }
    }

    /// <summary>
    /// Get a value indicating the list is read only; this property always 
    /// returns <c>false</c>.
    /// </summary>
    /// <value>
    /// <c>true</c> if the list is read only; otherwise, <c>false</c>.
    /// </value>
    [UFIgnore]
    public bool IsReadOnly
    {
      get { return false; }
    }

    #endregion

    #region protected overridable methods

    /// <summary>
    /// A value has been added for a specific key.
    /// </summary>
    /// <param name='anIndex'>
    /// The index the item has been added for.
    /// </param>
    /// <param name='aValue'>
    /// An item.
    /// </param>
    /// <param name='aFireChanged'>
    /// <c>true</c> fire changed events, <c>false</c> not
    /// </param>
    /// <param name="anAdded">
    /// True if value was added, false if value was replaced
    /// </param>
    /// <param name="anOldValue">
    /// Value that is getting replaced (only used if anAction is Replace). Can be null else.
    /// </param>
    protected virtual void ValueAdded(
      int anIndex,
      TValue aValue,
      bool aFireChanged,
      bool anAdded,
      TValue? anOldValue = default
    )
    {
      // only continue if TrackChildChange option is used
      if (this.m_indexLookup != null)
      {
        // create entry for value if it is not already in the list
        if (!this.m_indexLookup.ContainsKey(aValue))
        {
          this.m_indexLookup[aValue] = new List<int>();
          // direct typecast is safe, since lookup table is only created if
          // TValue supports IUFNotifyDataChanged 
          ((IUFNotifyDataChanged)aValue!).DataChanged +=
            this.HandleItemDataChanged;
        }
        // store index of the value
        this.m_indexLookup[aValue].Add(anIndex);
      }
      if (aFireChanged)
      {
        this.Changed();
      }
    }

    /// <summary>
    /// A value has been removed for a specific key.
    /// </summary>
    /// <param name='anIndex'>
    /// The index of the item has been removed for.
    /// </param>
    /// <param name='aValue'>
    /// An item.
    /// </param>
    /// <param name='aFireChanged'>
    /// <c>true</c> fire DataChanged event, <c>false</c> not
    /// </param>
    protected virtual void ValueRemoved(
      int anIndex,
      TValue aValue,
      bool aFireChanged
    )
    {
      // only continue if TrackChildChange option is used
      if (this.m_indexLookup != null)
      {
        // make sure value is present in the lookup table
        if (this.m_indexLookup.ContainsKey(aValue))
        {
          this.m_indexLookup[aValue].Remove(anIndex);
          if (this.m_indexLookup[aValue].Count == 0)
          {
            // direct typecast is safe, since lookup table is only created if
            // TValue supports IUFNotifyDataChanged 
            ((IUFNotifyDataChanged)aValue!).DataChanged -=
              this.HandleItemDataChanged;
            this.m_indexLookup.Remove(aValue);
          }
        }
      }
      if (aFireChanged)
      {
        this.Changed();
      }
    }

    /// <summary>
    /// Remove the values from internal list and call ValueRemoved in case 
    /// of children are being tracked.
    /// </summary>
    protected virtual void RemoveValues()
    {
      if (this.m_indexLookup != null)
      {
        for (int index = this.Count - 1; index >= 0; index--)
        {
          this.ValueRemoved(index, this.m_list[index], false);
        }
      }
      this.m_list.Clear();
    }

    #endregion

    #region private methods

    /// <summary>
    /// Processes the values currently in the internal m_list.
    /// </summary>
    private void ProcessValues()
    {
      // only continue if TrackChildChange option is used
      if (this.m_indexLookup == null)
      {
        return;
      }
      for (int index = this.m_list.Count - 1; index >= 0; index--)
      {
        if (this.m_list[index] != null)
        {
          this.ValueAdded(index, this.m_list[index], false, true);
        }
      }
    }

    #endregion

    #region event handlers

    /// <summary>
    /// Handles changes to an item. It fires a 
    /// <see cref="UFModel.DataChanged"/> event and using <c>aSender</c> as
    /// sender the <see cref="UFModel.PropertyChanged"/> and
    /// <see cref="UFModel.ChildChanged"/> events.
    /// </summary>
    /// <param name='aSender'>
    /// A sender.
    /// </param>
    /// <param name='anEvent'>
    /// An event.
    /// </param>
    private void HandleItemDataChanged(
      object aSender,
      UFDataChangedEventArgs anEvent
    )
    {
      this.Changed();
      this.OnChildChanged(aSender, anEvent);
      foreach (string propertyName in anEvent.GetPropertyNames())
      {
        this.OnPropertyChanged(
          aSender,
          new PropertyChangedEventArgs(propertyName)
        );
      }
    }

    #endregion
  }
}