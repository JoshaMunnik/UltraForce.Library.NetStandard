// <copyright file="UFArrayTools.cs" company="Ultra Force Development">
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
using System.Linq;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Array related utility methods.
  /// </summary>
  public static class UFArrayTools
  {
    #region public methods

    /// <summary>
    /// Shuffles an array.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='anArray'>
    /// The array to shuffle.
    /// </param>
    /// <returns>The value of the anArray parameter</returns>
    public static T[] Shuffle<T>(T[] anArray)
    {
      int n = anArray.Length;
      while (n > 1)
      {
        int k = UFRandomTools.Next(n--);
        Swap(anArray, k, n);
      }
      return anArray;
    }

    /// <summary>
    /// Swaps two elements in an array.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='anArray'>
    /// An array to update
    /// </param>
    /// <param name='anIndex0'>
    /// First element.
    /// </param>
    /// <param name='anIndex1'>
    /// Second element.
    /// </param>
    /// <returns>The value of the anArray parameter</returns>
    public static T[] Swap<T>(T[] anArray, int anIndex0, int anIndex1)
    {
      (anArray[anIndex0], anArray[anIndex1]) = (anArray[anIndex1], anArray[anIndex0]);
      return anArray;
    }

    /// <summary>
    /// Returns a random item from an array.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='anArray'>
    /// The array to get a value from.
    /// </param>
    /// <returns>
    /// An item from the array.
    /// </returns>
    public static T RandomItem<T>(T[] anArray)
    {
      return anArray[UFRandomTools.Next(anArray.Length)];
    }

    /// <summary>
    /// Returns a random item from a part of an array.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='anArray'>
    /// The array to get a value from.
    /// </param>
    /// <param name='aStart'>
    /// Start index
    /// </param>
    /// <param name='aCount'>
    /// Number of elements
    /// </param>
    /// <returns>
    /// An item from the list.
    /// </returns>
    public static T RandomItem<T>(T[] anArray, int aStart, int aCount)
    {
      return anArray[aStart + UFRandomTools.Next(aCount)];
    }

    /// <summary>
    /// Fills an array with new instances of a certain type.
    /// </summary>
    /// <remarks>
    /// Type <c>T</c> must have a parameterless constructor.
    /// </remarks>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="anArray">Array to set items in</param>
    /// <returns>Value of <c>anArray</c></returns>
    public static T[] Fill<T>(T[] anArray)
    {
      return Fill(anArray, 0, anArray.Length);
    }

    /// <summary>
    /// Fills a part of an array with new instances of a certain type.
    /// </summary>
    /// <remarks>
    /// Type <c>T</c> must have a parameterless constructor.
    /// </remarks>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="anArray">Array to set items in</param>
    /// <param name="aStart">Starting index</param>
    /// <param name="aCount">Number of items</param>
    /// <returns>Value of <c>anArray</c></returns>
    public static T[] Fill<T>(T[] anArray, int aStart, int aCount)
    {
      for (int index = aStart + aCount - 1; index >= aStart; index--)
      {
        anArray[index] = Activator.CreateInstance<T>();
      }
      return anArray;
    }
    
    /// <summary>
    /// Checks if an array contains any of the values.
    /// </summary>
    /// <param name="array">Array to check</param>
    /// <param name="values">Value to check</param>
    /// <typeparam name="T">Type of the elements inside the array</typeparam>
    /// <returns><c>true</c> if any of the values are found in the array</returns>
    public static bool ContainsAny<T>(T[] array, params T[] values)
    {
      return values.Any(array.Contains);
    }
    
    /// <summary>
    /// Checks if a array contains all the values. 
    /// </summary>
    /// <param name="array">Array to check</param>
    /// <param name="values">Value to check</param>
    /// <typeparam name="T">Type of the elements inside the array</typeparam>
    /// <returns><c>true</c> if all values are found in the array</returns>
    public static bool ContainsAll<T>(T[] array, params T[] values)
    {
      return values.All(array.Contains);
    }

    #endregion
  }
}