// <copyright file="UFEnum.cs" company="Ultra Force Development">
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Extensions
{
  /// <summary>
  /// Defines extension method for use with <see cref="Enum"/> type.
  /// </summary>
  public static class UFEnumExtensions
  {
    /// <summary>
    /// Get the value of a <see cref="UFDescriptionAttribute"/> used with an enum value.
    /// If no value is found, the value of the <see cref="DescriptionAttribute"/> is
    /// used. If that one also cannot be found, the enum value is converted to a string using
    /// <see cref="object.ToString"/>.
    /// </summary>
    /// <remarks>
    /// Based on code from: 
    /// http://stackoverflow.com/questions/18912697/system-componentmodel-descriptionattribute-in.NetStandard1-class-library
    /// <para>
    /// Usage:
    /// <code>
    ///   ... 
    ///   [UFDescription("Some text")]
    ///   FirstEnumValue
    ///   ...
    ///   // returns "Some text"
    ///   FirstEnumValue.GetDescription()
    ///   ... 
    ///   [Description("other text")]
    ///   SecondEnumValue
    ///   ... 
    ///   // returns "Other text"
    ///   SecondEnumValue.GetDescription()
    ///   ... 
    ///   ThirdEnumValue
    ///   ... 
    ///   // returns "ThirdEnumValue" 
    ///   ThirdEnumValue.GetDescription()
    /// </code>
    /// </para>
    /// </remarks>
    /// <param name="anEnumerationValue">Enumeration value.</param>
    /// <returns>
    /// The value of the description attribute or enum value converted to 
    /// string.
    /// </returns>
    public static string GetDescription(this Enum anEnumerationValue)
    {
      return UFStringTools.SelectString(
        anEnumerationValue.GetAttribute<UFDescriptionAttribute>()?.Description,
        anEnumerationValue.GetAttribute<DescriptionAttribute>()?.Description,
        anEnumerationValue.ToString()
      );
    }

    /// <summary>
    /// Get the value of a <see cref="UFDescriptionAttribute.ShortDescription"/> used with an enum value.
    /// </summary>
    /// <param name="anEnumerationValue">Enumeration value.</param>
    /// <returns>
    /// The short description value of the description attribute or enum value converted to string.
    /// </returns>
    public static string GetShortDescription(this Enum anEnumerationValue)
    {
      return UFStringTools.SelectString(
        anEnumerationValue.GetAttribute<UFDescriptionAttribute>()?.ShortDescription,
        anEnumerationValue.ToString()
      );
    }

    /// <summary>
    /// Get the value of a <see cref="UFDescriptionAttribute.ShortDescription"/> used with an enum value.
    /// </summary>
    /// <param name="anEnumerationValue">Enumeration value.</param>
    /// <returns>
    /// The short description value of the description attribute or enum value converted to string.
    /// </returns>
    public static string GetName(this Enum anEnumerationValue)
    {
      return UFStringTools.SelectString(
        anEnumerationValue.GetAttribute<UFDescriptionAttribute>()?.Name,
        anEnumerationValue.ToString()
      );
    }

    /// <summary>
    /// Get the next value in an enum; if the value is the last value in 
    /// the enum, the first value is returned.
    /// </summary>
    /// <remarks>
    /// Source: 
    /// http://stackoverflow.com/questions/642542/how-to-get-next-or-previous-enum-value-in-c-sharp
    /// </remarks>
    /// <param name="aSource">Source.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    /// <returns>The next value (or first value)</returns>
    public static T Next<T>(this T aSource) where T : struct, Enum
    {
      T[] values = (T[])Enum.GetValues(aSource.GetType());
      int nextIndex = Array.IndexOf<T>(values, aSource) + 1;
      return (values.Length == nextIndex) ? values.First() : values[nextIndex];
    }

    /// <summary>
    /// Get the next value in an enum; if the value is the last value in 
    /// the enum, the first value is returned.
    /// </summary>
    /// <remarks>
    /// Source: 
    /// http://stackoverflow.com/questions/642542/how-to-get-next-or-previous-enum-value-in-c-sharp
    /// </remarks>
    /// <param name="aSource">Source.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    /// <returns>The next value (or first value)</returns>
    public static T Previous<T>(this T aSource) where T : struct, Enum
    {
      T[] values = (T[])Enum.GetValues(aSource.GetType());
      int previousIndex = Array.IndexOf<T>(values, aSource) - 1;
      return (previousIndex < 0) ? values.Last() : values[previousIndex];
    }

    /// <summary>
    /// Gets an attribute type for an enum value.
    /// </summary>
    /// <param name="anEnumerationValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetAttribute<T>(this Enum anEnumerationValue) where T : class
    {
      if (
        anEnumerationValue
          .GetType()
          .GetRuntimeField(anEnumerationValue.ToString())
          .GetCustomAttributes(typeof(T), false)
          .SingleOrDefault() is T result
      )
      {
        return result;
      }
      return null;
    }
  }
}