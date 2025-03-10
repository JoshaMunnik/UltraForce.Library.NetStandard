// <copyright file="UFEnumerableTools.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2025 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (C) 2025 Ultra Force Development
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
using System.Linq;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Support methods for <see cref="IEnumerable{T}" />.
  /// </summary>
  public static class UFEnumerableTools
  {
    /// <summary>
    /// Checks if a collection contains any of the values.
    /// </summary>
    /// <param name="collection">Collection to check</param>
    /// <param name="values">Value to check</param>
    /// <typeparam name="T">Type of the elements inside the collection</typeparam>
    /// <returns><c>true</c> if any of the values are found in the collection</returns>
    public static bool ContainsAny<T>(IEnumerable<T> collection, params T[] values)
    {
      return values.Any(collection.Contains);
    }
    
    /// <summary>
    /// Checks if a collection contains all the values. 
    /// </summary>
    /// <param name="collection">Collection to check</param>
    /// <param name="values">Value to check</param>
    /// <typeparam name="T">Type of the elements inside the collection</typeparam>
    /// <returns><c>true</c> if all values are found in the collection</returns>
    public static bool ContainsAll<T>(IEnumerable<T> collection, params T[] values)
    {
      return values.All(collection.Contains);
    }
    
    /// <summary>
    /// Returns a random item from some collection.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='collection'>
    /// The collection to get a value from.
    /// </param>
    /// <returns>
    /// An item from the collection.
    /// </returns>
    public static T RandomItem<T>(IEnumerable<T> collection)
    {
      return UFArrayTools.RandomItem(collection.ToArray()); 
    }

    /// <summary>
    /// Checks if two collections are equal, in that they contain the same items.
    /// </summary>
    /// <param name="first">First collection to check</param>
    /// <param name="second">Second collection to check</param>
    /// <typeparam name="T">Type of the elements inside the collection</typeparam>
    /// <returns>True if both collections contain the same items</returns>
    public static bool EqualContent<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
      return UFListTools.EqualContent(first.ToList(), second.ToList());
    }
  }
}