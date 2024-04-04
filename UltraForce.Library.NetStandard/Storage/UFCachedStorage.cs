// <copyright file="UFCachedStorage.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Storage
{
  /// <summary>
  /// <see cref="UFCachedStorage"/> by caching values from another
  /// <see cref="UFKeyedStorage"/>.
  /// </summary>
  public class UFCachedStorage : UFKeyedStorage
  {
    #region private variables

    /// <summary>
    /// Encapsulated storage.
    /// </summary>
    private readonly UFKeyedStorage m_storage;

    /// <summary>
    /// Maximum time a value should be cached
    /// </summary>
    private readonly TimeSpan m_cacheLife;

    /// <summary>
    /// Cache of values
    /// </summary>
    private readonly Dictionary<string, CachedValue> m_cache;

    /// <summary>
    /// When <c>true</c> if a cached value is accessed before it expires,
    /// its time is expanded from the moment it was last accessed.
    /// </summary>
    private readonly bool m_keepAlive;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFCachedStorage"/>.
    /// </summary>
    /// <param name="aCacheLife">Maximum time a value is cached</param>
    /// <param name="aStorage">Storage to encapsulate</param>
    /// <param name="aKeepAlive">
    /// When <c>true</c> if a cached value is accessed before it expires,
    /// its life time is expanded from the moment it was last accessed.
    /// </param>
    protected UFCachedStorage(
      TimeSpan aCacheLife,
      UFKeyedStorage aStorage,
      bool aKeepAlive = false
    )
    {
      this.m_cacheLife = aCacheLife;
      this.m_cache = new Dictionary<string, CachedValue>();
      this.m_storage = aStorage;
      this.m_keepAlive = aKeepAlive;
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFCachedStorage"/> if a maximum
    /// cache life time of 1 minute .
    /// </summary>
    /// <param name="aStorage">Storage to encapsulate</param>
    /// <param name="aKeepAlive">
    /// When <c>true</c> if a cached value is accessed before it expires,
    /// its life time is expanded from the moment it was last accessed.
    /// </param>
    protected UFCachedStorage(
      UFKeyedStorage aStorage,
      bool aKeepAlive = false
    ) : this(TimeSpan.FromMinutes(1), aStorage, aKeepAlive)
    {
    }

    #endregion

    #region UFKeyedStorage

    /// <summary>
    /// Gets a string from the cache if any, else get it from the
    /// wrapped <see cref="UFKeyedStorage"/> and store it in the cache.
    /// </summary>
    /// <param name="aKey">Key to get value for</param>
    /// <param name="aDefault">Default value to use if none exists</param>
    /// <returns>Value for <c>aKey</c></returns>
    public override string GetString(string aKey, string aDefault)
    {
      lock (this.m_cache)
      {
        // cache contains the value, return it if the value is still alive.
        if (this.m_cache.TryGetValue(aKey, out CachedValue? value))
        {
          if (DateTime.Now - value.Time < this.m_cacheLife)
          {
            if (this.m_keepAlive)
            {
              value.Time = DateTime.Now;
            }
            return value.Value;
          }
        }
        // either used stored entry or create new cache entry if there was not 
        // one
        if (value == null)
        {
          value = new CachedValue();
          this.m_cache.Add(aKey, value);
        }
        value.Value = this.m_storage.GetString(aKey, aDefault);
        value.Time = DateTime.Now;
        return value.Value;
      }
    }

    /// <summary>
    /// Sets a string in the wrapped <see cref="UFKeyedStorage"/> and store or
    /// update in the cache.
    /// </summary>
    /// <param name="aKey">Key to store the value for</param>
    /// <param name="aValue">Value to store</param>
    public override void SetString(string aKey, string aValue)
    {
      lock (this.m_cache)
      {
        CachedValue value;
        if (this.m_cache.ContainsKey(aKey))
        {
          value = this.m_cache[aKey];
        }
        else
        {
          value = new CachedValue();
          this.m_cache.Add(aKey, value);
        }
        value.Value = aValue;
        value.Time = DateTime.Now;
      }
      this.m_storage.SetString(aKey, aValue);
    }

    /// <summary>
    /// Calls <see cref="UFKeyedStorage.DeleteAll"/> of the wrapped
    /// <see cref="UFKeyedStorage"/> and removes all cached values.
    /// </summary>
    public override void DeleteAll()
    {
      lock (this.m_cache)
      {
        this.m_cache.Clear();
      }
      this.m_storage.DeleteAll();
    }

    /// <summary>
    /// Calls <see cref="UFKeyedStorage.DeleteKey"/> of the wrapped
    /// <see cref="UFKeyedStorage"/> and removes the cached value (if any).
    /// </summary>
    public override void DeleteKey(string aKey)
    {
      lock (this.m_cache)
      {
        if (this.m_cache.ContainsKey(aKey))
        {
          this.m_cache.Remove(aKey);
        }
      }
      this.m_storage.DeleteKey(aKey);
    }

    /// <summary>
    /// Checks if a valid entry exists in the cache, else calls
    /// <see cref="UFKeyedStorage.HasKey"/> of the wrapped
    /// <see cref="UFKeyedStorage"/>.
    /// </summary>
    public override bool HasKey(string aKey)
    {
      lock (this.m_cache)
      {
        if (!this.m_cache.ContainsKey(aKey))
        {
          return this.m_storage.HasKey(aKey);
        }
        if (DateTime.Now - this.m_cache[aKey].Time < this.m_cacheLife)
        {
          return true;
        }
        // clean up value, it is no longer valid
        this.m_cache.Remove(aKey);
      }
      return this.m_storage.HasKey(aKey);
    }

    #endregion

    #region private class cachedvalue

    /// <summary>
    /// Simple class that contains a value and a date/time value.
    /// </summary>
    private class CachedValue
    {
      public string Value { get; set; } = null!;
      public DateTime Time { get; set; }
    }

    #endregion
  }
}