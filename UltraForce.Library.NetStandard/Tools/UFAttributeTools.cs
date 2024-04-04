// <copyright file="UFAttributeTools.cs" company="Ultra Force Development">
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
using System.Reflection;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// A static class with methods related to <see cref="Attribute"/> subclasses
  /// </summary>
  public static class UFAttributeTools
  {
    /// <summary>
    /// Finds an instance of a certain attribute class.
    /// </summary>
    /// <typeparam name="T">
    /// A subclass of Attribute
    /// </typeparam>
    /// <param name="aType">
    ///   Type to search attribute for.
    /// </param>
    /// <param name="anInherited">
    ///   When <c>true</c> search also inherited attributes.
    /// </param>
    /// <returns>
    /// The first attribute of type <c>T</c> or <c>null</c> if none could be found.
    /// </returns>
    public static T? Find<T>(Type aType, bool anInherited = true) where T : Attribute
    {
      return (T?) aType.GetTypeInfo().GetCustomAttributes(typeof(T), anInherited).FirstOrDefault();
    }

    /// <summary>
    /// Finds an instance of a certain attribute class.
    /// </summary>
    /// <typeparam name="T">
    /// A subclass of Attribute
    /// </typeparam>
    /// <param name="anInfo">
    /// Property to search attribute for.
    /// </param>
    /// <param name="anInherited">
    /// When <c>true</c> search also inherited attributes.
    /// </param>
    /// <returns>
    /// The first attribute of type <c>T</c> or <c>null</c> if none could  be found.
    /// </returns>
    public static T? Find<T>(PropertyInfo anInfo, bool anInherited = true) where T : Attribute
    {
      return (T?) anInfo.GetCustomAttributes(typeof(T), anInherited).FirstOrDefault();
    }

    /// <summary>
    /// Finds an instance of a certain attribute class.
    /// </summary>
    /// <typeparam name="T">
    /// A subclass of Attribute
    /// </typeparam>
    /// <param name="anInfo">
    /// Field to search attribute for.
    /// </param>
    /// <param name="anInherited">
    /// When <c>true</c> search also inherited attributes.
    /// </param>
    /// <returns>
    /// The first attribute of type <c>T</c> or <c>null</c> if none could be found.
    /// </returns>
    public static T? Find<T>(FieldInfo anInfo, bool anInherited = true) where T : Attribute
    {
      return (T?) anInfo.GetCustomAttributes(typeof(T), anInherited).FirstOrDefault();
    }

    /// <summary>
    /// Finds an instance of a certain attribute class.
    /// </summary>
    /// <typeparam name="T">
    /// A subclass of Attribute
    /// </typeparam>
    /// <param name="anInfo">
    /// Method to search attribute for.
    /// </param>
    /// <param name="anInherited">
    /// When <c>true</c> search also inherited attributes.
    /// </param>
    /// <returns>
    /// The first attribute of type <c>T</c> or <c>null</c> if none could be found.
    /// </returns>
    public static T? Find<T>(MethodInfo anInfo, bool anInherited = true) where T : Attribute
    {
      return (T?) anInfo.GetCustomAttributes(typeof(T), anInherited).FirstOrDefault();
    }
  }
}