// <copyright file="UFCache.cs" company="Ultra Force Development">
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
using System.Collections.Generic;
using System.Linq;

namespace UltraForce.Library.NetStandard.Services
{
  /// <summary>
  /// <see cref="UFCache{TKey, TValue}" /> is a base class to create a simple memory cache for a certain type and
  /// storing it for a certain key type.
  /// <para>
  /// The methods are thread safe (using the `lock` construct).
  /// </para>
  /// </summary>
  /// <remarks>
  /// Subclasses must override the <see cref="GetSize"/> method.
  /// </remarks>
  /// <typeparam name="TKey">Type of the key</typeparam>
  /// <typeparam name="TValue">Type of the value</typeparam>
  public abstract class UFCache<TKey, TValue> 
  {

  #region private variables

  /// <summary>
  /// Maximum size of cache
  /// </summary>
  private readonly long m_capacity;

  /// <summary>
  /// Total size used
  /// </summary>
  private long m_totalSize;

  /// <summary>
  /// Entries in the cache
  /// </summary>
  private readonly Dictionary<TKey, UFCacheEntry> m_entries;

  #endregion

  #region constructors

  /// <summary>
  /// Constructs an instance of <see cref="UFCache{TKey,TValue}"/>.
  /// </summary>
  /// <param name="aCapacity">
  /// Maximums size of cache
  /// </param>
  /// <param name="anUnknown">
  /// Value to return when trying to get a value that is not in the cache.
  /// </param>
  protected UFCache(long aCapacity, TValue anUnknown)
  {
    this.m_capacity = aCapacity;
    this.m_entries = new Dictionary<TKey, UFCacheEntry>(100);
    this.Unknown = anUnknown;
  }

  #endregion

  #region public methods

  /// <summary>
  /// Adds a value to the cache.
  /// <para>
  /// If the size of the value is bigger then the cache capacity, nothing happens and the value is not cached.
  /// </para>
  /// <para>
  /// The method will remove the oldest cached items (access times most in the past) if the cache would exceed
  /// its capacity.
  /// </para>
  /// </summary>
  /// <param name="aKey">Key of value to add</param>
  /// <param name="aValue">Value to add</param>
  public void Add(TKey aKey, TValue aValue)
  {
    lock (this.m_entries)
    {
      long size = this.GetSize(aValue);
      // exit if data is to big for the cache
      if (size > this.m_capacity)
      {
        return;
      }
      // new item would exceed max cache size?
      if (this.m_totalSize + size > this.m_capacity)
      {
        // remove oldest items until enough space becomes available
        IEnumerable<UFCacheEntry>
          entries = this.m_entries.Values.OrderBy(entry => entry.AccessedOn);
        foreach (UFCacheEntry entry in entries)
        {
          this.m_totalSize -= entry.Size;
          this.m_entries.Remove(entry.Key);
          if (this.m_totalSize + size <= this.m_capacity)
          {
            break;
          }
        }
      }
      this.m_entries[aKey] = new UFCacheEntry(aKey, aValue, size);
      this.m_totalSize += size;
    }
  }

  /// <summary>
  /// Gets a value for a key from the cache. If the value can not be found, the method will return the value
  /// of <see cref="Unknown"/>.
  /// <para>
  /// Accessing a value will also update its access time.
  /// </para>
  /// </summary>
  /// <param name="aKey">Key to get value for</param>
  /// <returns>Value or <see cref="Unknown"/></returns>
  public TValue Get(TKey aKey)
  {
    lock (this.m_entries)
    {
      if (!this.m_entries.ContainsKey(aKey))
      {
        return this.Unknown;
      }
      UFCacheEntry entry = this.m_entries[aKey];
      entry.AccessedOn = DateTime.Now;
      return entry.Value;
    }
  }

  /// <summary>
  /// Removes a value for a key from the cache. If the value can not be found, the method will return the value
  /// of <see cref="Unknown"/>.
  /// </summary>
  /// <param name="aKey">Key to get value for</param>
  /// <returns>Removed value or <see cref="Unknown"/></returns>
  public TValue Remove(TKey aKey)
  {
    lock (this.m_entries)
    {
      if (!this.m_entries.ContainsKey(aKey))
      {
        return this.Unknown;
      }
      UFCacheEntry entry = this.m_entries[aKey];
      this.m_entries.Remove(aKey);
      this.m_totalSize -= entry.Size;
      return entry.Value;
    }
  }

  /// <summary>
  /// Checks if there is a value for a key.
  /// </summary>
  /// <param name="aKey">Key to check</param>
  /// <returns>True if there is a value</returns>
  public bool Has(TKey aKey)
  {
    return this.m_entries.ContainsKey(aKey);
  }

  /// <summary>
  /// Clears the whole cache.
  /// </summary>
  public void Clear()
  {
    lock (this.m_entries)
    {
      this.m_entries.Clear();
      this.m_totalSize = 0;
    }
  }

  #endregion

  #region public properties

  /// <summary>
  /// This value is returned by <see cref="Get"/> when there is no value stored for a certain key.
  /// </summary>
  public TValue Unknown { get; }

  #endregion

  #region protected abstract methods

  /// <summary>
  /// Gets the size of a value.
  /// </summary>
  /// <param name="aValue">Value to get a size for</param>
  /// <returns>Size of value</returns>
  protected abstract long GetSize(TValue aValue);

  #endregion

  #region private classes

  /// <summary>
  /// CacheEntry stores information about a value in the cache
  /// </summary>
  private class UFCacheEntry
  {
    /// <summary>
    /// Key the value is stored for, it is also stored here so that the entry can still be found when reordering
    /// the values.
    /// </summary>
    public TKey Key { get; }

    /// <summary>
    /// Value stored
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// Last time the value was accessed.
    /// </summary>
    public DateTime AccessedOn { get; set; }

    /// <summary>
    /// Size of value
    /// </summary>
    public long Size { get; }

    /// <summary>
    /// Constructs an instance of CacheEntry
    /// </summary>
    /// <param name="aKey"></param>
    /// <param name="aValue"></param>
    /// <param name="aSize"></param>
    public UFCacheEntry(TKey aKey, TValue aValue, long aSize)
    {
      this.Key = aKey;
      this.Value = aValue;
      this.AccessedOn = DateTime.Now;
      this.Size = aSize;
    }
  }

  #endregion

  }
}