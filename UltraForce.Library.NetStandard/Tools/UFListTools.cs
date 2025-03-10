// <copyright file="UFListTools.cs" company="Ultra Force Development">
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
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// IList related utility methods.
  /// </summary>
  public static class UFListTools
  {
    /// <summary>
    /// Shuffles a list.
    /// </summary>
    /// <param name='aList'>
    /// The list to shuffle.
    /// </param>
    /// <returns>The value of the anArray parameter</returns>
    public static IList<T> Shuffle<T>(IList<T> aList)
    {
      int n = aList.Count;
      while (n > 1)
      {
        int k = UFRandomTools.Next(n--);
        UFListTools.Swap(aList, k, n);
      }
      return aList;
    }

    /// <summary>
    /// Shuffles a part of a list.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='aList'>
    /// The list to shuffle.
    /// </param>
    /// <param name='aStart'>
    /// Start index
    /// </param>
    /// <param name='aCount'>
    /// Number of elements
    /// </param>
    /// <returns>The value of the anArray parameter</returns>
    public static IList<T> Shuffle<T>(IList<T> aList, int aStart, int aCount)
    {
      int n = aCount;
      while (n > 1)
      {
        int k = UFRandomTools.Next(n--);
        UFListTools.Swap(aList, aStart + k, aStart + n);
      }
      return aList;
    }

    /// <summary>
    /// Swaps two elements in a list.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='aList'>
    /// An array to update
    /// </param>
    /// <param name='anIndex0'>
    /// First element.
    /// </param>
    /// <param name='anIndex1'>
    /// Second element.
    /// </param>
    /// <returns>The value of the anArray parameter</returns>
    public static IList<T> Swap<T>(
      IList<T> aList,
      int anIndex0,
      int anIndex1
    )
    {
      (aList[anIndex0], aList[anIndex1]) = (aList[anIndex1], aList[anIndex0]);
      return aList;
    }

    /// <summary>
    /// Returns random item from a list.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='aList'>
    /// The list to get a value from.
    /// </param>
    /// <returns>
    /// An item from the list.
    /// </returns>
    public static T RandomItem<T>(IList<T> aList)
    {
      return aList[UFRandomTools.Next(aList.Count)];
    }

    /// <summary>
    /// Returns random item from a part of a list.
    /// </summary>
    /// <typeparam name='T'>
    /// Item type.
    /// </typeparam>
    /// <param name='aList'>
    /// The list to get a value from.
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
    public static T RandomItem<T>(IList<T> aList, int aStart, int aCount)
    {
      return aList[aStart + UFRandomTools.Next(aCount)];
    }

    /// <summary>
    /// Removes and returns random item from a list.
    /// </summary>
    /// <typeparam name='T'>Item type.</typeparam>
    /// <param name='aList'>The list to get a value from.</param>
    /// <returns>An item from the list (which is also removed).</returns>
    public static T RemoveRandomItem<T>(IList<T> aList)
    {
      return RemoveRandomItem(aList, 0, aList.Count);
    }

    /// <summary>
    /// Removes and returns random item from a part of a list.
    /// </summary>
    /// <typeparam name='T'>Item type.</typeparam>
    /// <param name='aList'>The list to get a value from.</param>
    /// <param name='aStart'>Start index</param>
    /// <param name='aCount'>Number of elements</param>
    /// <returns>An item from the list.</returns>
    public static T RemoveRandomItem<T>(IList<T> aList, int aStart, int aCount)
    {
      int index = aStart + UFRandomTools.Next(aCount);
      T result = aList[index];
      aList.RemoveAt(index);
      return result;
    }

    /// <summary>
    /// Fills a list by adding a number of new instances of a certain type.
    /// </summary>
    /// <remarks>
    /// Type <c>T</c> must have a parameterless constructor.
    /// </remarks>
    /// <typeparam name="T">Type to add</typeparam>
    /// <param name="aList">List to add new instances to</param>
    /// <param name="aCount">Number of new instances to add</param>
    /// <returns>Value of <c>aList</c></returns>
    public static IList<T> Fill<T>(IList<T> aList, int aCount)
    {
      for (int index = 0; index < aCount; index++)
      {
        aList.Add(Activator.CreateInstance<T>());
      }
      return aList;
    }

    /// <summary>
    /// Fills a list by adding <see cref="List{T}.Capacity" /> new instances 
    /// of a certain type. 
    /// </summary>
    /// <remarks>
    /// Type <c>T</c> must have a parameterless constructor.
    /// </remarks>
    /// <typeparam name="T">Type to add</typeparam>
    /// <param name="aList">List to add new instances to</param>
    /// <returns>Value of <c>aList</c></returns>
    public static List<T> Fill<T>(List<T> aList)
    {
      return (List<T>)Fill(aList, aList.Capacity);
    }

    /// <summary>
    /// Compacts a list by removing items that do not pass a test function.
    /// </summary>
    /// <typeparam name="T">Type of list items</typeparam>
    /// <param name="aList">List to compact</param>
    /// <param name="aTest">Test function</param>
    /// <returns>value of <c>aList</c></returns>
    public static List<T> Compact<T>(List<T> aList, Func<T, bool> aTest)
    {
      // first copy the items that pass the test
      int targetIndex = 0;
      for (int sourceIndex = 0; sourceIndex < aList.Count; sourceIndex++)
      {
        T item = aList[sourceIndex];
        if (aTest(item))
        {
          aList[targetIndex++] = item;
        }
      }
      // remove unused items (they have been copied to positions before target)
      if (targetIndex < aList.Count)
      {
        aList.RemoveRange(targetIndex, aList.Count - targetIndex);
      }
      return aList;
    }

    /// <summary>
    /// Creates a list of items by calling a factory function a number of times.
    /// </summary>
    /// <param name="aCount"></param>
    /// <param name="aFactory"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<List<T>> FillAsync<T>(int aCount, Func<Task<T>> aFactory)
    {
      List<T> items = new List<T>(aCount);
      for (int index = 0; index < aCount; index++)
      {
        items.Add(await aFactory());
      }
      return items;
    }

    /// <summary>
    /// Creates a list of items by calling a factory function a number of times.
    /// </summary>
    /// <param name="aCount"></param>
    /// <param name="aFactory"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> Fill<T>(int aCount, Func<T> aFactory)
    {
      List<T> items = new List<T>(aCount);
      for (int index = 0; index < aCount; index++)
      {
        items.Add(aFactory());
      }
      return items;
    }

    /// <summary>
    /// Checks if two lists are equal, in that they contain the same items. The order of the items
    /// does not have to be the same.
    /// </summary>
    /// <param name="aFirst"></param>
    /// <param name="aSecond"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool EqualContent<T>(IList<T> aFirst, IList<T> aSecond)
    {
      return aFirst.Count == aSecond.Count && aFirst.All(aSecond.Contains);
    }
    
    /// <summary>
    /// Finds the index of the first element that passes a test.
    /// </summary>
    /// <param name="aList">List to process</param>
    /// <param name="aTest">Test to pass</param>
    /// <param name="aStart">Optional starting index</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Index of -1 if no item passes the test</returns>
    public static int IndexOf<T>(IList<T> aList, Func<T, bool> aTest, int aStart = 0)
    {
      for (int index = aStart; index < aList.Count; index++)
      {
        if (aTest(aList[index]))
        {
          return index;
        }
      }
      return -1;
    }
    
    /// <summary>
    /// Checks if a list contains any of the values.
    /// </summary>
    /// <param name="list">List to check</param>
    /// <param name="values">Value to check</param>
    /// <typeparam name="T">Type of the elements inside the list</typeparam>
    /// <returns><c>true</c> if any of the values are found in the list</returns>
    public static bool ContainsAny<T>(IList<T> list, params T[] values)
    {
      return values.Any(list.Contains);
    }
    
    /// <summary>
    /// Checks if a list contains all the values. 
    /// </summary>
    /// <param name="list">List to check</param>
    /// <param name="values">Value to check</param>
    /// <typeparam name="T">Type of the elements inside the list</typeparam>
    /// <returns><c>true</c> if all values are found in the list</returns>
    public static bool ContainsAll<T>(IList<T> list, params T[] values)
    {
      return values.All(list.Contains);
    }
  }
}