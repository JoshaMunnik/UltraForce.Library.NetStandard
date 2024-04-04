// <copyright file="UFDictionaryTools.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// <see cref="IDictionary{TKey,TValue}"/> related utility methods.
  /// </summary>
  public static class UFDictionaryTools
  {
    #region public methods

    /// <summary>
    /// Tries to get value for a key, if not found returns a default value.
    /// </summary>
    /// <typeparam name="K">The type for key</typeparam>
    /// <typeparam name="V">The type for value</typeparam>
    /// <param name="aDictionary">
    /// A dictionary to get value from
    /// </param>
    /// <param name="aKey">
    /// Key to get value for
    /// </param>
    /// <param name="aDefault">
    /// Default value to return if value could not be obtained
    /// </param>
    /// <returns>Value for the key or default value</returns>
    public static V GetValue<K, V>(
      IDictionary<K, V> aDictionary,
      K aKey,
      V aDefault)
    {
      V result;
      if ((aDictionary != null) && aDictionary.TryGetValue(aKey, out result))
      {
        return result;
      }
      else
      {
        return aDefault;
      }
    }

    /// <summary>
    /// Tries to get value for a key, if not found returns a default value.
    /// <para>
    /// Uses <see cref="object.ToString" /> to get the string from the value.
    /// </para>
    /// </summary>
    /// <typeparam name="TKey">The type for key</typeparam>
    /// <typeparam name="TValue">The type for value</typeparam>
    /// <param name="aDictionary">A dictionary to get value from</param>
    /// <param name="aKey">Key to get value for</param>
    /// <param name="aDefault">
    /// Default value to return if value could not be obtained
    /// </param>
    /// <returns>Value for the key or default value</returns>
    public static string GetValueAsString<TKey, TValue>(
      IDictionary<TKey, TValue>? aDictionary,
      TKey aKey,
      string aDefault)
    {
      if (
        (aDictionary != null) && 
        aDictionary.TryGetValue(aKey, out TValue result) && 
        (result != null)
      )
      {
        return result.ToString();
      }
      return aDefault;
    }

    /// <summary>
    /// Tries to get value for a key, if not found returns a default value.
    /// <para>
    /// Returns true if the <see cref="object.ToString" /> equals "test" (case
    /// insensitive compare).
    /// </para>
    /// </summary>
    /// <typeparam name="TKey">The type for key</typeparam>
    /// <typeparam name="TValue">The type for value</typeparam>
    /// <param name="aDictionary">A dictionary to get value from</param>
    /// <param name="aKey">Key to get value for</param>
    /// <param name="aDefault">
    /// Default value to return if value could not be obtained
    /// </param>
    /// <returns>Value for the key or default value</returns>
    public static bool GetValueAsBoolean<TKey, TValue>(
      IDictionary<TKey, TValue>? aDictionary,
      TKey aKey,
      bool aDefault)
    {
      if ((aDictionary != null) && aDictionary.TryGetValue(aKey, out TValue result))
      {
        return result!.ToString()
          .Equals(
            "true",
            StringComparison.OrdinalIgnoreCase
          );
      }
      else
      {
        return aDefault;
      }
    }

    /// <summary>
    /// Tries to get value for a key, if not found returns a default value.
    /// <para>
    /// Uses the <see cref="int.Parse(string)" /> on the result of
    /// <see cref="object.ToString" /> of the value.
    /// </para>
    /// </summary>
    /// <typeparam name="TKey">The type for key</typeparam>
    /// <typeparam name="TValue">The type for value</typeparam>
    /// <param name="aDictionary">A dictionary to get value from</param>
    /// <param name="aKey">Key to get value for</param>
    /// <param name="aDefault">
    /// Default value to return if value could not be obtained
    /// </param>
    /// <returns>Value for the key or default value</returns>
    public static int GetValueAsInt<TKey, TValue>(
      IDictionary<TKey, TValue>? aDictionary,
      TKey aKey,
      int aDefault)
    {
      if ((aDictionary != null) && aDictionary.TryGetValue(aKey, out TValue result))
      {
        return int.Parse(result!.ToString());
      }
      else
      {
        return aDefault;
      }
    }

    /// <summary>
    /// Tries to get value for a key, if not found returns a default value.
    /// <para>
    /// Uses the <see cref="float.Parse(string)" /> on the result of
    /// <see cref="object.ToString" /> of the value.
    /// </para>
    /// </summary>
    /// <typeparam name="TKey">The type for key</typeparam>
    /// <typeparam name="TValue">The type for value</typeparam>
    /// <param name="aDictionary">A dictionary to get value from</param>
    /// <param name="aKey">Key to get value for</param>
    /// <param name="aDefault">
    /// Default value to return if value could not be obtained
    /// </param>
    /// <returns>Value for the key or default value</returns>
    public static float GetValueAsFloat<TKey, TValue>(
      IDictionary<TKey, TValue>? aDictionary,
      TKey aKey,
      float aDefault)
    {
      if ((aDictionary != null) && aDictionary.TryGetValue(aKey, out TValue result))
      {
        return float.Parse(result!.ToString());
      }
      else
      {
        return aDefault;
      }
    }

    /// <summary>
    /// Tries to get value for a key, if not found returns a default value.
    /// <para>
    /// Uses the <see cref="double.Parse(string)" /> on the result of
    /// <see cref="object.ToString" /> of the value.
    /// </para>
    /// </summary>
    /// <typeparam name="TKey">The type for key</typeparam>
    /// <typeparam name="TValue">The type for value</typeparam>
    /// <param name="aDictionary">A dictionary to get value from</param>
    /// <param name="aKey">Key to get value for</param>
    /// <param name="aDefault">
    /// Default value to return if value could not be obtained
    /// </param>
    /// <returns>Value for the key or default value</returns>
    public static double GetValueAsDouble<TKey, TValue>(
      IDictionary<TKey, TValue>? aDictionary,
      TKey aKey,
      double aDefault)
    {
      if ((aDictionary != null) && aDictionary.TryGetValue(aKey, out TValue result))
      {
        return double.Parse(result!.ToString());
      }
      return aDefault;
    }

    /// <summary>
    /// Merges multiple dictionaries into a single dictionary. The first non
    /// null entry will be used to merge the others into. If multiple
    /// dictionary contain the same key, the value of the last dictionary with
    /// that key will be used.
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Value type</typeparam>
    /// <param name="aDictionaries">Dictionaries to merge (can be null)</param>
    /// <returns>
    /// The first non null parameter with the values of all others merged
    /// into it. If all parameters are null, the method will return null.
    /// </returns>
    public static IDictionary<TKey, TValue>? Merge<TKey, TValue>(
      params IDictionary<TKey, TValue>?[] aDictionaries
    )
    {
      IDictionary<TKey, TValue>? result = null;
      foreach (IDictionary<TKey, TValue>? dictionary in aDictionaries)
      {
        if (result == null)
        {
          result = dictionary;
        }
        else if (dictionary != null)
        {
          foreach (KeyValuePair<TKey, TValue> item in dictionary)
          {
            result[item.Key] = item.Value;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Dumps a dictionary
    /// </summary>
    /// <typeparam name="TKey">type for key</typeparam>
    /// <typeparam name="TValue">type for value</typeparam>
    /// <param name="aDictionary">Dictionary to dump</param>
    /// <param name="aSeparator">separator to use between values</param>
    /// <returns>
    /// string in the form of 'k=v'[+ aSeparator + 'k=v' ...]
    /// </returns>
    public static string Dump<TKey, TValue>(
      IDictionary<TKey, TValue> aDictionary,
      string aSeparator = "\n"
    )
    {
      return string.Join(
        aSeparator,
        aDictionary.Select(x => x.Key + "=" + x.Value)
      );
    }

    #endregion
  }
}