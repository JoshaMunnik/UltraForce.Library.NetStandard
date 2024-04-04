// <copyright file="UFJavaScriptTools.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Static support methods related to JavaScript
  /// </summary>
  public static class UFJavaScriptTools
  {
    #region private constants

    /// <summary>
    /// Defines the date/time: 1970-1-1 0:0:0.0
    /// </summary>
    private static readonly DateTime s_epochDateTime = new DateTime(
      1970,
      1,
      1,
      0,
      0,
      0,
      0,
      System.DateTimeKind.Utc
    );

    /// <summary>
    /// Quote character to use with string.
    /// </summary>
    private const string StringQuote = "'";

    #endregion

    #region public methods

    /// <summary>
    /// Returns a value which can be used as Time (number of milliseconds 
    /// since 1970-1-1) parameter to get the date/time in javascript.
    /// </summary>
    /// <param name="aDateTime">DateTime value to get Time value for</param>
    /// <returns>Time value</returns>
    public static long GetTime(DateTime aDateTime)
    {
      return (long)(aDateTime - s_epochDateTime).TotalMilliseconds;
    }

    /// <summary>
    /// Returns a string value for use within javascript. It surrounds the
    /// text with the javascript string quotes and escapes any occurrence
    /// of the quote within aText.
    /// <para>
    /// If <c>aText</c> is null the method will return "null"
    /// </para>
    /// </summary>
    /// <param name="aText">Text to convert</param>
    /// <returns>javascript string specification</returns>
    public static string GetString(string? aText)
    {
      return aText == null
        ? "null"
        : StringQuote +
        aText
          .Replace(StringQuote, "\\" + StringQuote)
          .Replace("\r", "")
          .Replace("\n", "\\n")
          .Replace("\t", "\\t")
        + StringQuote;
    }

    /// <summary>
    /// Joins the strings in aList and calls <see cref="GetString(string)"/>.
    /// <para>
    /// If <c>aList</c> is null, the method will return "null"
    /// </para>
    /// </summary>
    /// <param name="aSeparator">Separator to join with</param>
    /// <param name="aList">List of strings to join</param>
    /// <returns>javascript string specification</returns>
    public static string GetString(
      string aSeparator,
      IEnumerable<string>? aList
    )
    {
      return aList == null ? "null" : GetString(string.Join(aSeparator, aList));
    }

    /// <summary>
    /// Joins the strings in aList by calling
    /// <see cref="GetString(string,IEnumerable{string})"/> using ', ' as
    /// separator value.
    /// </summary>
    /// <param name="aList">List of strings to join</param>
    /// <returns>javascript string specification</returns>
    public static string GetString(IEnumerable<string> aList)
    {
      return GetString(", ", aList);
    }

    #endregion
  }
}