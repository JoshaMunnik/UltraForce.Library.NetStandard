// <copyright file="UFModelStorageWrapper.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (c) 2018 Ultra Force Development
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
using System.Reflection;
using System.Runtime.CompilerServices;
using UltraForce.Library.NetStandard.Events;
using UltraForce.Library.NetStandard.Interfaces;
using UltraForce.Library.NetStandard.Storage;

namespace UltraForce.Library.NetStandard.Models
{
  /// <summary>
  /// <see cref="UFModelStorageWrapper" /> can be used to create models that
  /// map their properties directly to a storage.
  /// <para>
  /// Use <see cref="Get{T}"/> to get a property value from the storage and
  /// use <see cref="Set"/> to store a property value to the storage.
  /// </para>
  /// <para>
  /// The class handles properties that implement
  /// <see cref="IUFNotifyDataChanged"/>. The class uses 
  /// <see cref="UFModel.Option.TrackChildChange"/> and
  /// will update the object in storage whenever it changes.
  /// </para>
  /// <para>
  /// If a property is a (sub)class of <see cref="UFModelStorageWrapper"/> the
  /// class will call <see cref="SetParentKeyPrefix"/> whenever a new instance
  /// is created or assigned. The class will never store the value itself in
  /// the storage. The class assumes the property value will take care of
  /// storing/retrieving itself. This allows for a
  /// <see cref="UFModelStorageWrapper"/> subclass to be used for multiple
  /// properties without key name conflicts.
  /// </para>
  /// <para>
  /// Properties that implement <see cref="IUFStorableObject"/> are treated
  /// slightly different. The first time a property's value is referenced
  /// and none exists, a new instance is created and its data is loaded
  /// from the wrapped <see cref="UFKeyedStorage"/> using
  /// <see cref="UFKeyedStorage.GetStorableObject(string,IUFStorableObject)"/>.
  /// </para>
  /// </summary>
  public class UFModelStorageWrapper : UFModel
  {
    #region private variables

    /// <summary>
    /// Prefix to apply to key names
    /// </summary>
    private readonly string m_keyPrefix;

    /// <summary>
    /// Prefix set by parent instance
    /// </summary>
    private string m_keyParentPrefix;

    /// <summary>
    /// Storage for settings.
    /// </summary>
    private readonly UFKeyedStorage m_storage;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance <see cref="UFModelStorageWrapper"/>.
    /// </summary>
    /// <param name="aStorage">
    /// <see cref="UFKeyedStorage"/> instance to wrap
    /// </param>
    /// <param name="aPrefix">
    /// Prefix used to create storage key names from property names.
    /// </param>
    protected UFModelStorageWrapper(
      UFKeyedStorage aStorage,
      string aPrefix = ""
    ) : base(Option.TrackChildChange)
    {
      this.m_keyParentPrefix = "";
      this.m_keyPrefix = aPrefix;
      this.m_storage = aStorage;
    }

    #endregion

    #region public methods

    /// <inheritdoc />
    public override void Clear(bool aCallChanged)
    {
      this.m_storage.DeleteAll();
      if (aCallChanged)
      {
        this.Changed();
      }
    }

    #endregion

    #region protected methods

    /// <summary>
    /// Gets a value from storage.
    /// </summary>
    /// <remarks>
    /// The first time this method is called for a type that is implementing
    /// <see cref="IUFStorableObject"/> the method will create the object
    /// with <c>aFactory</c> and then will load it from the storage.
    /// <para>
    /// Subsequent calls just return the stored instance.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">
    /// Type of property
    /// </typeparam>
    /// <param name="aFactory">
    /// Default value to use when the value is not available in the storage
    /// </param>
    /// <param name="aPropertyName">
    /// Name of property used as storage key
    /// </param>
    /// <returns>Value from storage or aDefault</returns>
    protected override T Get<T>(
      Func<T> aFactory,
      [CallerMemberName] string aPropertyName = ""
    )
    {
      string key = this.GetStorageKey(aPropertyName);
      TypeInfo typeInfo = typeof(T).GetTypeInfo();
      // UFModalStorageWrapper (sub)classes are assumed to store/load
      // themselves so just create but do not retrieve from the storage.
      if (
        typeof(UFModelStorageWrapper).GetTypeInfo().IsAssignableFrom(typeInfo)
      )
      {
        // use the storage property name as key prefix for the instance
        return base.Get(
          () =>
          {
            object result = aFactory()!;
            ((UFModelStorageWrapper)result).SetParentKeyPrefix(key + ".");
            return (T)result;
          },
          aPropertyName
        );
      }
      // IUFStorableObject are created and then 'loaded' from the storage
      if (
        typeof(IUFStorableObject).GetTypeInfo().IsAssignableFrom(typeInfo)
      )
      {
        return base.Get(
          () =>
          {
            T result = aFactory()!;
            this.m_storage.GetStorableObject(
              key,
              (IUFStorableObject)result
            );
            return result;
          },
          aPropertyName
        );
      }
      if (!this.m_storage.HasKey(key))
      {
        return aFactory();
      }
      object? value = this.m_storage.GetObject(key, typeof(T), type => aFactory()!);
      return value == null ? aFactory() : (T)value;
    }

    /// <summary>
    /// Sets a value in storage. Will fire a changed event if value is new.
    /// </summary>
    /// <remarks>
    /// If the value assigned is a subclass of
    /// <see cref="UFModelStorageWrapper"/>, a call will be made to
    /// <see cref="SetParentKeyPrefix"/>.
    /// </remarks>
    /// <param name="aValue">
    /// Value to store. If a value is <c>null</c>, it will get deleted from
    /// the storage.
    /// </param>
    /// <param name="aPropertyName">
    /// Name of property used as storage key
    /// </param>
    /// <returns>
    /// <c>True</c> if new value got stored; <c>false</c> if the value already
    /// stored is equal to <c>aValue</c>. 
    /// </returns>
    protected override bool Set(
      object? aValue,
      [CallerMemberName] string aPropertyName = ""
    )
    {
      string key = this.GetStorageKey(aPropertyName);
      bool result = base.Set(aValue, aPropertyName);
      // delete from storage if new value is null
      if (aValue == null)
      {
        if (this.m_storage.HasKey(key))
        {
          this.m_storage.DeleteKey(key);
        }
        return result;
      }
      // UFModalStorageWrapper (sub)classes are assumed to store/load
      // themselves so do nothing with storage
      if (aValue is UFModelStorageWrapper wrapper)
      {
        // set parent key prefix if new value was assigned
        if (result)
        {
          wrapper.SetParentKeyPrefix(key);
        }
        return result;
      }
      // if the value does not exist in the storage, it always has changed
      result = result || !this.m_storage.HasKey(key);
      // always update value in storage
      this.m_storage.SetObject(key, aValue, aValue.GetType());
      return result;
    }

    /// <summary>
    /// Removes a value from the settings. It assigns <c>null</c> as new
    /// property value.
    /// </summary>
    /// <param name="aPropertyName">Name of property to remove</param>
    protected void DeleteFromStorage(string aPropertyName)
    {
      this.Set(null, aPropertyName);
    }

    /// <inheritdoc />
    protected override void ChildHasChanged(
      object aValue,
      string[] aPropertyNames,
      UFDataChangedEventArgs anEvent
    )
    {
      base.ChildHasChanged(aValue, aPropertyNames, anEvent);
      // UFModelStorageWrapper instances will take care of storing themselves
      // so just exit
      if (aValue is UFModelStorageWrapper)
      {
        return;
      }
      // update the value in storage
      foreach (string propertyName in aPropertyNames)
      {
        string key = this.GetStorageKey(propertyName);
        this.m_storage.SetObject(key, aValue);
      }
    }

    /// <summary>
    /// This method is called when this instance is assigned to a property
    /// in a parent <see cref="UFModelStorageWrapper"/> instance.
    /// <para>
    /// It stores a prefix that is used when getting the storage key via
    /// <see cref="GetStorageKey"/>.
    /// </para>
    /// <para>
    /// The default implementation just stores the key.
    /// </para>
    /// </summary>
    /// <param name="aKeyPrefix">Prefix to use</param>
    protected virtual void SetParentKeyPrefix(string aKeyPrefix)
    {
      this.m_keyParentPrefix = aKeyPrefix;
    }

    /// <summary>
    /// Gets key for storage.
    /// <para>
    /// The default implementation prefixes the key with the value
    /// from <see cref="SetParentKeyPrefix"/> and the prefix passed with the
    /// constructor.
    /// </para>
    /// </summary>
    /// <param name="aName">Name to convert to unique key</param>
    /// <returns>Unique key</returns>
    protected virtual string GetStorageKey(string aName)
    {
      return this.m_keyParentPrefix + this.m_keyPrefix + aName;
    }

    #endregion
  }
}