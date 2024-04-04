// <copyright file="UFProgressTools.cs" company="Ultra Force Development">
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

using UltraForce.Library.NetStandard.Interfaces;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Support methods for progress.
  /// </summary>
  public static class UFProgressTools
  {
    /// <summary>
    /// Checks if an object implements the <see cref="IUFWeightedProgress" /> and calls the appropriate method.
    /// </summary>
    /// <param name="anObject">
    /// Object to get progress weight from
    /// </param>
    /// <param name="aDefault">
    /// Value to use if the object does not implement <see cref="IUFWeightedProgress"/>.
    /// </param>
    /// <returns>
    /// Result from <see cref="IUFWeightedProgress.ProgressWeight" /> or <c>aDefault</c> if anObject does not implement
    /// the specified interface.
    /// </returns>
    public static double GetProgressWeight(object anObject, double aDefault = 1.0)
    {
      return (anObject is IUFWeightedProgress weightedProgress) ? weightedProgress.ProgressWeight : aDefault;
    }

    /// <summary>
    /// Checks if an object implements the <see cref="IUFProgress" /> and calls the appropriate method.
    /// </summary>
    /// <param name="anObject">
    /// Object to get progress from
    /// </param>
    /// <param name="aDefault">
    /// Value to use if the object does not implement <see cref="IUFProgress"/>.
    /// </param>
    /// <returns>
    /// Result from <see cref="IUFProgress.Progress" /> or <c>aDefault</c> if anObject does not implement the
    /// specified interface.
    /// </returns>
    public static double GetProgress(object anObject, double aDefault = 0.0)
    {
      return (anObject is IUFProgress progress) ? progress.Progress : aDefault;
    }
  }
}