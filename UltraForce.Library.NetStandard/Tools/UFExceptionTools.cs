// <copyright file="UFExceptionTools.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Support methods for <see cref="Exception"/>
  /// </summary>
  public static class UFExceptionTools
  {
    /// <summary>
    /// Checks if an exception has an inner exception, if it does
    /// return its message and call <see cref="GetInnerExceptionMessages"/> recursively
    /// for the inner exception.
    /// </summary>
    /// <param name="anException">
    /// Exception to check
    /// </param>
    /// <returns>
    /// the inner exception message and recursively its inner exception
    /// </returns>
    public static string GetInnerExceptionMessages(Exception anException)
    {
      if (anException.InnerException != null)
      {
        return string.Format(
          " (InnerException: {0} {2} {1})",
          anException.InnerException.Message,
          anException.InnerException.StackTrace,
          GetInnerExceptionMessages(anException.InnerException)
        );
      }
      return "";
    }
  }
}