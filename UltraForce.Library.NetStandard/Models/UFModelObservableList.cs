// <copyright file="UFModelObservableList.cs" company="Ultra Force Development">
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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UltraForce.Library.NetStandard.Events;

namespace UltraForce.Library.NetStandard.Models
{
  /// <summary>
  /// Extends <see cref="UFModelList{T}"/> and implements the
  /// <see cref="INotifyCollectionChanged"/> event.
  /// </summary>
  /// <remarks>
  /// The <see cref="CollectionChanged"/> uses weak references to the instances
  /// implementing the handler methods.
  /// </remarks>
  public class UFModelObservableList<TValue> : UFModelList<TValue>, INotifyCollectionChanged
  {
    #region private variables

    /// <summary>
    /// Manager to manage <see cref="CollectionChanged"/>
    /// </summary>
    private readonly UFWeakReferencedNotifyCollectionChangedManager m_manager =
      new UFWeakReferencedNotifyCollectionChangedManager();

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="UFModelObservableList{TValue}"/>.
    /// class.
    /// </summary>
    /// <param name='anOptions'>
    /// <see cref="UFModel.Option"/>
    /// </param>
    public UFModelObservableList(params Option[] anOptions) : base(anOptions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFModelObservableList{TValue}"/> class.
    /// </summary>
    /// <param name='aCapacity'>
    /// Initial list capacity.
    /// </param>
    /// <param name='anOptions'>
    /// <see cref="UFModel.Option"/>
    /// </param>
    public UFModelObservableList(int aCapacity, params Option[] anOptions)
      : base(aCapacity, anOptions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFModelObservableList{TValue}"/> class.
    /// </summary>
    /// <param name='aCollection'>
    /// An initial collection to fill list with
    /// </param>
    /// <param name='anOptions'>
    /// <see cref="UFModel.Option"/>
    /// </param>
    public UFModelObservableList(
      IEnumerable<TValue> aCollection,
      params Option[] anOptions
    )
      : base(aCollection, anOptions)
    {
    }

    #endregion

    #region public methods

    /// <inheritdoc />
    public override void Shuffle(int aStart, int aCount)
    {
      base.Shuffle(aStart, aCount);
      this.OnCollectionChangedMove(
        aStart,
        aStart,
        this.GetRange(aStart, aCount)
      );
    }

    #endregion

    #region INotifycollectionChanged

    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add => this.m_manager.Add(value);
      remove => this.m_manager.Remove(value);
    }

    #endregion

    #region List[t] methods

    /// <inheritdoc />
    public override void AddRange(IEnumerable<TValue> aCollection)
    {
      IList<TValue> list = aCollection.ToList();
      int index = this.Count;
      base.AddRange(list);
      this.OnCollectionChangedAdd(index, list);
    }

    #endregion

    #region protected overridden methods

    /// <inheritdoc />
    protected override void ValueAdded(
      int anIndex,
      TValue aValue,
      bool aFireChanged,
      bool anAdded,
      TValue? anOldValue = default
    )
    {
      base.ValueAdded(anIndex, aValue, aFireChanged, anAdded, anOldValue);
      if (!aFireChanged)
      {
        return;
      }
      if (anAdded || (anOldValue == null))
      {
        this.OnCollectionChangedAdd(anIndex, aValue);
      }
      else
      {
        this.OnCollectionChangedReplace(anIndex, aValue, anOldValue);
      }
    }

    /// <inheritdoc />
    protected override void ValueRemoved(
      int anIndex,
      TValue aValue,
      bool aFireChanged
    )
    {
      base.ValueRemoved(anIndex, aValue, aFireChanged);
      if (aFireChanged)
      {
        this.OnCollectionChangedRemove(anIndex, aValue);
      }
    }

    /// <inheritdoc />
    protected override void RemoveValues()
    {
      IList<TValue> copy = new List<TValue>(this);
      base.RemoveValues();
      this.OnCollectionChangedReset(copy);
    }

    #endregion

    #region private methods

    /// <summary>
    /// Trigger <see cref="CollectionChanged"/> event for items that moved.
    /// </summary>
    /// <param name="aNewIndex"></param>
    /// <param name="anOldIndex"></param>
    /// <param name="anItems"></param>
    private void OnCollectionChangedMove(
      int aNewIndex,
      int anOldIndex,
      IList<TValue> anItems
    )
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        NotifyCollectionChangedAction.Move,
        anItems,
        aNewIndex,
        anOldIndex
      ));
    }

    /// <summary>
    /// Trigger <see cref="CollectionChanged"/> event for items that got
    /// replaced.
    /// </summary>
    /// <param name="anIndex"></param>
    /// <param name="aNewValue"></param>
    /// <param name="anOldValue"></param>
    private void OnCollectionChangedReplace(
      int anIndex,
      TValue aNewValue,
      TValue anOldValue
    )
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        NotifyCollectionChangedAction.Replace,
        aNewValue,
        anOldValue,
        anIndex
      ));
    }

    /// <summary>
    /// Trigger <see cref="CollectionChanged"/> event for items that got added.
    /// </summary>
    /// <param name="anIndex"></param>
    /// <param name="aValue"></param>
    private void OnCollectionChangedAdd(
      int anIndex,
      TValue aValue
    )
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        NotifyCollectionChangedAction.Add,
        aValue,
        anIndex
      ));
    }

    /// <summary>
    /// Trigger <see cref="CollectionChanged"/> event for items that got added.
    /// </summary>
    /// <param name="anIndex"></param>
    /// <param name="anItems"></param>
    private void OnCollectionChangedAdd(
      int anIndex,
      IList<TValue> anItems
    )
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        NotifyCollectionChangedAction.Add,
        anItems,
        anIndex
      ));
    }

    /// <summary>
    /// Trigger <see cref="CollectionChanged"/> event for items that got
    /// removed.
    /// </summary>
    /// <param name="anIndex"></param>
    /// <param name="aValue"></param>
    private void OnCollectionChangedRemove(
      int anIndex,
      TValue aValue
    )
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        NotifyCollectionChangedAction.Remove,
        aValue,
        anIndex
      ));
    }

    /// <summary>
    /// Trigger <see cref="CollectionChanged"/> event for collection that
    /// got reset.
    /// </summary>
    private void OnCollectionChangedReset(IList<TValue>? anOldItems = null)
    {
      if (anOldItems != null)
      {
        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
          NotifyCollectionChangedAction.Remove,
          anOldItems,
          0
        ));
      }
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(
        NotifyCollectionChangedAction.Reset
      ));
    }

    /// <summary>
    /// Trigger <see cref="CollectionChanged"/> event.
    /// </summary>
    /// <param name="anArguments">Arguments to use</param>
    private void OnCollectionChanged(
      NotifyCollectionChangedEventArgs anArguments
    )
    {
      this.m_manager.Invoke(this, anArguments);
    }

    #endregion
  }
}