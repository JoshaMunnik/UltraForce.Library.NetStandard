// <copyright file="UFJsonTools.cs" company="Ultra Force Development">
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
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
// </license>

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UltraForce.Library.NetStandard.Interfaces;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// UFJsonTools can be used to create JSON formatted data. It supports 
  /// <see cref="IUFJsonExport" />.
  /// </summary>
  /// <remarks>
  /// The code is based upon MiniJSONs code (added support for IUFJsonExport):
  /// <para>
  /// MiniJSON Copyright (c) 2013 Calvin Rien
  /// </para>
  /// <para>
  /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
  /// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
  /// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT.
  /// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
  /// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
  /// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
  /// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  /// </para>
  /// </remarks>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public static class UFJsonTools
  {
    /// <summary>
    /// Adds a string to <see cref="StringBuilder"/> using JON formatting.
    /// </summary>
    /// <param name="aBuilder">A builder to add string to.</param>
    /// <param name="aValue">A value to add.</param>
    public static void SaveString(StringBuilder aBuilder, string aValue)
    {
      aBuilder.Append('\"');
      char[] charArray = aValue.ToCharArray();
      foreach (char character in charArray)
      {
        switch (character)
        {
          case '"':
            aBuilder.Append("\\\"");
            break;
          case '\\':
            aBuilder.Append("\\\\");
            break;
          case '\b':
            aBuilder.Append("\\b");
            break;
          case '\f':
            aBuilder.Append("\\f");
            break;
          case '\n':
            aBuilder.Append("\\n");
            break;
          case '\r':
            aBuilder.Append("\\r");
            break;
          case '\t':
            aBuilder.Append("\\t");
            break;
          default:
            int charCode = Convert.ToInt32(character);
            if ((charCode >= 32) && (charCode <= 126))
            {
              aBuilder.Append(character);
            }
            else
            {
              aBuilder.Append("\\u");
              aBuilder.Append(charCode.ToString("x4"));
            }
            break;
        }
      }
      aBuilder.Append('\"');
    }

    /// <summary>
    /// Saves a value as JSON structure.
    /// </summary>
    /// <param name="aValue">A value to save.</param>
    /// <returns>JSON formatted string</returns>
    public static string SaveValue(object aValue)
    {
      StringBuilder builder = new StringBuilder();
      UFJsonTools.SaveValue(builder, aValue);
      return builder.ToString();
    }

    /// <summary>
    /// Adds a value to <see cref="StringBuilder"/> using JSON formatting.
    /// <para>
    /// This method tries to create the correct formatted JSON based on the 
    /// type of <c>aValue</c>. 
    /// </para>
    /// <para>
    /// The method supports objects implementing <see cref="IUFJsonExport" />.
    /// </para>
    /// </summary>
    /// <param name="aBuilder">A builder to add value to.</param>
    /// <param name="aValue">A value to add.</param>
    public static void SaveValue(StringBuilder aBuilder, object? aValue)
    {
      switch (aValue)
      {
        case null:
          aBuilder.Append("null");
          break;
        case string stringValue:
          UFJsonTools.SaveString(aBuilder, stringValue);
          break;
        case IUFJsonExport exportValue:
          exportValue.SaveJson(aBuilder);
          break;
        case IList listValue:
          UFJsonTools.SaveList(aBuilder, listValue);
          break;
        case IDictionary dictionaryValue:
          UFJsonTools.SaveDictionary(aBuilder, dictionaryValue);
          break;
        case char charValue:
          UFJsonTools.SaveString(aBuilder, new string(charValue, 1));
          break;
        case int _:
        case uint _:
        case long _:
        case sbyte _:
        case byte _:
        case short _:
        case ushort _:
        case ulong _:
        case double _:
        case float _:
        case decimal _:
          aBuilder.Append(aValue);
          break;
        default:
          UFJsonTools.SaveString(aBuilder, aValue.ToString());
          break;
      }
    }

    /// <summary>
    /// Saves an <see cref="IList"/> as JSON structure.
    /// </summary>
    /// <param name="aList">A list to add.</param>
    /// <returns>JSON formatted string</returns>
    public static string SaveList(IList aList)
    {
      StringBuilder builder = new StringBuilder();
      UFJsonTools.SaveList(builder, aList);
      return builder.ToString();
    }

    /// <summary>
    /// Adds an <see cref="IList"/> as JSON array to 
    /// <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="aBuilder">A builder to add data to.</param>
    /// <param name="aList">A list to add.</param>
    public static void SaveList(StringBuilder aBuilder, IList aList)
    {
      aBuilder.Append('[');
      for (int index = 0; index < aList.Count; index++)
      {
        if (index > 0)
        {
          aBuilder.Append(',');
        }
        UFJsonTools.SaveValue(aBuilder, aList[index]);
      }
      aBuilder.Append(']');
    }

    /// <summary>
    /// Saves an <see cref="IDictionary"/> as JSON structure.
    /// </summary>
    /// <param name="aDictionary">A dictionary.</param>
    /// <returns>JSON formatted string</returns>
    public static string SaveDictionary(IDictionary aDictionary)
    {
      StringBuilder builder = new StringBuilder();
      UFJsonTools.SaveDictionary(builder, aDictionary);
      return builder.ToString();
    }

    /// <summary>
    /// Adds an <see cref="IDictionary"/> as JSON object to 
    /// <see cref="StringBuilder"/>.
    /// <para>
    /// The keys are used for property names, the values as property values.
    /// </para>
    /// </summary>
    /// <param name="aBuilder">A builder to add data to.</param>
    /// <param name="aDictionary">A dictionary to add.</param>
    public static void SaveDictionary(
      StringBuilder aBuilder,
      IDictionary aDictionary
    )
    {
      bool firstValue = true;
      aBuilder.Append('{');
      foreach (object key in aDictionary.Keys)
      {
        if (!firstValue)
        {
          aBuilder.Append(',');
        }
        UFJsonTools.SaveString(aBuilder, key.ToString());
        aBuilder.Append(':');
        UFJsonTools.SaveValue(aBuilder, aDictionary[key]);
        firstValue = false;
      }
      aBuilder.Append('}');
    }
  }
}