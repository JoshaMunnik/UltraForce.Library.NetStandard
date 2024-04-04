// <copyright file="UFRandomTools.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// UFRandomTools defines static methods to obtain a random value of certain 
  /// kind. It maps the methods to an internally managed <see cref="Random"/>
  /// instance.
  /// </summary>
  public static class UFRandomTools
  {
    #region private vars

    /// <summary>
    /// The random value generator.
    /// </summary>
    private static readonly Random s_random = new Random();

    #endregion

    #region public methods

    /// <summary>
    /// Returns the next random integer.
    /// </summary>
    /// <returns>Random integer.</returns>
    public static int Next()
    {
      return s_random.Next();
    }

    /// <summary>
    /// Returns the next random integer.
    /// </summary>
    /// <param name="aMax">The maximum value (exclusive).</param>
    /// <returns>Random integer.</returns>
    public static int Next(int aMax)
    {
      return s_random.Next(aMax);
    }

    /// <summary>
    /// Fills a buffer with random bytes.
    /// </summary>
    /// <param name="aBuffer">A buffer to fill.</param>
    public static void NextBytes(byte[] aBuffer)
    {
      s_random.NextBytes(aBuffer);
    }

    /// <summary>
    /// Returns the next random float.
    /// </summary>
    /// <returns>Random float.</returns>
    public static float NextFloat()
    {
      return (float)s_random.NextDouble();
    }

    /// <summary>
    /// Returns the next random double.
    /// </summary>
    /// <returns>Random double.</returns>
    public static double NextDouble()
    {
      return s_random.NextDouble();
    }

    /// <summary>
    /// Returns a random integer within a range.
    /// </summary>
    /// <param name="aMin">The minimum value (inclusive).</param>
    /// <param name="aMax">The maximum value (exclusive).</param>
    /// <returns>A random integer &gt;= aMin and &lt; aMax</returns>
    public static int Range(int aMin, int aMax)
    {
      return s_random.Next(aMin, aMax);
    }

    #endregion
  }
}