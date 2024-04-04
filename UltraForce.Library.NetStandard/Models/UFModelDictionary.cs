// <copyright file="UFModelDictionary.cs" company="Ultra Force Development">
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
using System.Text;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Events;
using UltraForce.Library.NetStandard.Interfaces;
using UltraForce.Library.NetStandard.Storage;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Models
{
  /// <summary>
  /// Implements a generic <see cref="IDictionary"/> adding support for certain
  /// <see cref="UFModel.Option"/> values and implementing various 
  /// <see cref="UFModel"/> methods. 
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="UFModelDictionary{TKey,TValue}"/> will fire a 
  /// <see cref="UFModel.DataChanged"/> event whenever the contents of 
  /// the dictionary changes.
  /// </para>
  /// <para>
  /// If <see cref="UFModel.Option.LockChildren" /> option is set and TValue 
  /// implements the <see cref="IUFLockable"/> interface, <see cref="Lock"/>
  /// will also lock all items within the list.
  /// </para>
  /// <para>
  /// If <see cref="UFModel.Option.TrackChildChange" /> option is set and 
  /// TValue implements <see cref="IUFNotifyDataChanged"/>,  
  /// <see cref="UFModelDictionary{TKey,TValue}" /> will install change 
  /// listeners with every stored value and will fire 
  /// <see cref="UFModel.DataChanged" />, 
  /// <see cref="UFModel.ChildChanged" /> and 
  /// <see cref="UFModel.PropertyChanged" /> events if a value changes. 
  /// </para>
  /// <para>
  /// <see cref="UFModelDictionary{TKey,TValue}" /> will make sure to install
  /// only one listener, even if the same value instance is added for multiple
  /// keys.
  /// </para>
  /// </remarks>
  public class UFModelDictionary<TKey, TValue> : UFModel, IDictionary<TKey, TValue>
    where TKey : struct
    where TValue : struct
  {
    #region private consts

    /// <summary>
    /// Key used to store and retrieve count value with
    /// </summary>
    private const string CountKey = "count";

    /// <summary>
    /// Key used to store retrieve item value with (index will be concatenated to it)
    /// </summary>
    private const string ItemKey = "item_";

    /// <summary>
    /// Key used to store retrieve key value with (index will be concatenated to it)
    /// </summary>
    private const string KeyKey = "key_";

    #endregion

    #region private vars

    /// <summary>
    /// UFModelDictionary uses a generic Dictionary to contain the data.
    /// </summary>
    private readonly Dictionary<TKey, TValue> m_dictionary;

    /// <summary>
    /// Lookup is used when tracking child changes in case a value is 
    /// assigned to multiple keys.
    /// </summary>
    private readonly Dictionary<TValue, List<TKey>>? m_keyLookup;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFModelDictionary{TKey,TValue}"/> class.
    /// </summary>
    /// <param name='anOptions'>
    /// The options <see cref="UFModel.Option" />
    /// </param>
    public UFModelDictionary(params Option[] anOptions) : base(anOptions)
    {
      this.m_dictionary = new Dictionary<TKey, TValue>();
      // create lookup table
      if (
        this.HasOption(Option.TrackChildChange)
        && UFObjectTools.Implements<TValue, IUFNotifyDataChanged>()
      )
      {
        this.m_keyLookup = new Dictionary<TValue, List<TKey>>();
      }
    }

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFModelDictionary{TKey,TValue}"/> class.
    /// </summary>
    /// <param name='aCapacity'>
    /// Initial capacity.
    /// </param>
    /// <param name='anOptions'>
    /// An options.
    /// </param>
    public UFModelDictionary(int aCapacity, params Option[] anOptions)
      : base(anOptions)
    {
      this.m_dictionary = new Dictionary<TKey, TValue>(aCapacity);
      if (
        this.HasOption(Option.TrackChildChange)
        && UFObjectTools.Implements<TValue, IUFNotifyDataChanged>()
      )
      {
        this.m_keyLookup = new Dictionary<TValue, List<TKey>>();
      }
    }

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFModelDictionary{TKey,TValue}"/> class.
    /// </summary>
    /// <param name='aDictionary'>
    /// Initial dictionary values.
    /// </param>
    /// <param name='anOptions'>
    /// An options.
    /// </param>
    public UFModelDictionary(
      IDictionary<TKey, TValue> aDictionary,
      params Option[] anOptions
    ) : base(anOptions)
    {
      this.m_dictionary = new Dictionary<TKey, TValue>(aDictionary);
      if (
        this.HasOption(Option.TrackChildChange)
        && UFObjectTools.Implements<TValue, IUFNotifyDataChanged>()
      )
      {
        this.m_keyLookup = new Dictionary<TValue, List<TKey>>();
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

    #endregion

    #region iuflockable

    /// <summary>
    /// If <see cref="UFModel.Option.LockChildren"/> is set, call 
    /// <see cref="IUFLockable.Lock"/> on all non-null items that implement
    /// <see cref="IUFLockable"/>.
    /// </summary>
    public override int Lock()
    {
      int result = base.Lock();
      if (
        !this.HasOption(Option.LockChildren) || !UFObjectTools.Implements<TValue, IUFLockable>()
      )
      {
        return result;
      }
      foreach (TValue? item in this.m_dictionary.Values)
      {
        ((IUFLockable)item.Value).Lock();
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
      if (
        !this.HasOption(Option.LockChildren) || !UFObjectTools.Implements<TValue, IUFLockable>()
      )
      {
        return result;
      }
      foreach (TValue? item in this.m_dictionary.Values)
      {
        ((IUFLockable)item.Value).Unlock();
      }
      return result;
    }

    #endregion

    #region iufstorableobject

    /// <summary>
    /// Saves the keys and values in the storage.
    /// </summary>
    /// <param name="aStorage">Storage to save the data to</param>
    public override void SaveToStorage(UFKeyedStorage aStorage)
    {
      base.SaveToStorage(aStorage);
      int index = 0;
      foreach (KeyValuePair<TKey, TValue> item in this.m_dictionary)
      {
        aStorage.SetObject<TKey>(KeyKey + index, item.Key);
        aStorage.SetObject<TValue>(ItemKey + index, item.Value!);
        index++;
      }
      aStorage.SetInt(CountKey, index);
    }

    /// <summary>
    /// Loads the keys and values from the storage.
    /// </summary>
    /// <param name="aStorage">Storage to save the data to</param>
    public override void LoadFromStorage(UFKeyedStorage aStorage)
    {
      // load will also call Clear
      base.LoadFromStorage(aStorage);
      int count = aStorage.GetInt(CountKey);
      for (int index = 0; index < count; index++)
      {
        TKey? key = aStorage.GetObject<TKey>(KeyKey + index);
        TValue? value = aStorage.GetObject<TValue>(ItemKey + index);
        if ((key != null) && (value != null))
        {
          this.Add(key.Value, value.Value);
        }
      }
      this.Unlock();
    }

    #endregion

    #region IUFJsonExport

    /// <summary>
    /// Save data as object definition.
    /// </summary>
    /// <param name="aBuilder">A builder to add data to.</param>
    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    public override void SaveJson(StringBuilder aBuilder)
    {
      UFJsonTools.SaveDictionary(aBuilder, (IDictionary)this);
    }

    #endregion

    #region idictionary[tkey,tvalue]

    /// <inheritdoc />
    public bool ContainsKey(TKey aKey)
    {
      return this.m_dictionary.ContainsKey(aKey);
    }

    /// <inheritdoc />
    public void Add(TKey aKey, TValue aValue)
    {
      this.m_dictionary.Add(aKey, aValue);
      this.ValueAdded(aKey, aValue, true);
    }

    /// <inheritdoc />
    public bool Remove(TKey aKey)
    {
      if (this.m_dictionary.ContainsKey(aKey))
      {
        TValue value = this.m_dictionary[aKey];
        bool result = this.m_dictionary.Remove(aKey);
        if (result)
        {
          this.ValueRemoved(aKey, value, true);
        }
        return result;
      }
      else
      {
        return this.m_dictionary.Remove(aKey);
      }
    }

    /// <inheritdoc />
    public bool TryGetValue(TKey aKey, out TValue aValue)
    {
      return this.m_dictionary.TryGetValue(aKey, out aValue);
    }

    /// <inheritdoc />
    [UFIgnore]
    public TValue this[TKey aKey]
    {
      get { return this.m_dictionary[aKey]; }
      set
      {
        if (this.m_dictionary.ContainsKey(aKey))
        {
          this.ValueRemoved(aKey, this.m_dictionary[aKey], false);
        }
        this.m_dictionary[aKey] = value;
        this.ValueAdded(aKey, value, true);
      }
    }

    /// <inheritdoc />
    [UFIgnore]
    public ICollection<TKey> Keys
    {
      get { return this.m_dictionary.Keys; }
    }

    /// <inheritdoc />
    [UFIgnore]
    public ICollection<TValue> Values
    {
      get { return this.m_dictionary.Values; }
    }

    #endregion

    #region ienumerable

    /// <inheritdoc />
    public IEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion

    #region ienumerable[keyvaluepair[tkey,tvalue]]

    /// <inheritdoc />
    IEnumerator<KeyValuePair<TKey, TValue>>
      IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
      return this.m_dictionary.GetEnumerator();
    }

    #endregion

    #region icollection[keyvaluepair[tkey,tvalue]]

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> anItem)
    {
      this.m_dictionary.Add(anItem.Key, anItem.Value);
      this.ValueAdded(anItem.Key, anItem.Value, true);
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> anItem)
    {
      return this.m_dictionary.ContainsKey(anItem.Key) &&
             this.m_dictionary[anItem.Key!]!.Equals(anItem.Value);
    }

    /// <inheritdoc />
    public void CopyTo(
      KeyValuePair<TKey, TValue>[] anArray,
      int anArrayIndex
    )
    {
      foreach (KeyValuePair<TKey, TValue> item in this.m_dictionary)
      {
        anArray[anArrayIndex++] = item;
      }
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> anItem)
    {
      if (this.m_dictionary.ContainsKey(anItem.Key) &&
          this.m_dictionary[anItem.Key!]!.Equals(anItem.Value)
         )
      {
        this.Remove(anItem.Key);
        return true;
      }
      return false;
    }

    /// <inheritdoc />
    [UFIgnore]
    public int Count
    {
      get { return this.m_dictionary.Count; }
    }

    /// <inheritdoc />
    [UFIgnore]
    public bool IsReadOnly
    {
      get { return false; }
    }

    #endregion

    #region private methods

    /// <summary>
    /// A value has been added for a specific key.
    /// </summary>
    /// <param name='aKey'>
    /// The key the item has been added for.
    /// </param>
    /// <param name='aValue'>
    /// An item.
    /// </param>
    /// <param name='aFireChanged'>
    /// <c>true</c> fire DataChanged event, <c>false</c> not
    /// </param>
    private void ValueAdded(TKey aKey, TValue aValue, bool aFireChanged)
    {
      // process value when using TrackChildChange
      if (this.m_keyLookup != null)
      {
        if (!this.m_keyLookup.ContainsKey(aValue))
        {
          this.m_keyLookup[aValue] = new List<TKey>();
          ((IUFNotifyDataChanged)aValue!).DataChanged += this.HandleItemDataChanged;
        }
        this.m_keyLookup[aValue].Add(aKey);
      }
      if (aFireChanged)
      {
        this.Changed();
      }
    }

    /// <summary>
    /// A value has been removed for a specific key.
    /// </summary>
    /// <param name='aKey'>
    /// The key the item has been removed for.
    /// </param>
    /// <param name='aValue'>
    /// An item.
    /// </param>
    /// <param name='aFireChanged'>
    /// <c>true</c> fire DataChanged event, <c>false</c> not
    /// </param>
    private void ValueRemoved(TKey aKey, TValue aValue, bool aFireChanged)
    {
      // process value when using TrackChildChange
      if (this.m_keyLookup != null)
      {
        if (this.m_keyLookup.ContainsKey(aValue))
        {
          this.m_keyLookup[aValue].Remove(aKey);
          if (this.m_keyLookup[aValue].Count == 0)
          {
            ((IUFNotifyDataChanged)aValue!).DataChanged -= this.HandleItemDataChanged;
            this.m_keyLookup.Remove(aValue);
          }
        }
      }
      if (aFireChanged)
      {
        this.Changed();
      }
    }

    /// <summary>
    /// Processes the values in the dictionary and call ValueAdded for every 
    /// value.
    /// </summary>
    private void ProcessValues()
    {
      if (this.m_keyLookup == null)
      {
        return;
      }
      foreach (TKey key in this.m_dictionary.Keys)
      {
        this.ValueAdded(key, this.m_dictionary[key], false);
      }
    }

    /// <summary>
    /// Remove the values from the dictionary, call ValueRemoved for every 
    /// value.
    /// </summary>
    private void RemoveValues()
    {
      if (this.m_keyLookup != null)
      {
        foreach (TKey key in this.m_dictionary.Keys)
        {
          this.ValueRemoved(key, this.m_dictionary[key], false);
        }
      }
      this.m_dictionary.Clear();
    }

    #endregion

    #region event handlers

    /// <summary>
    /// Handles changes to an item. It fires a 
    /// <see cref="UFModel.DataChanged"/>, 
    /// <see cref="UFModel.PropertyChanged"/> and
    /// <see cref="UFModel.ChildChanged"/> events using <c>aSender</c> as
    /// sender.
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