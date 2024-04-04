// <copyright file="UFObjectTools.cs" company="Ultra Force Development">
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
using System.Reflection;
using UltraForce.Library.NetStandard.Interfaces;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// <see cref="object" /> related support methods.
  /// </summary>
  public static class UFObjectTools
  {
    #region public methods

    /// <summary>
    /// Checks if a nullable type is null, if so return a default value else 
    /// return the value converted to the underlying type.
    /// </summary>
    /// <typeparam name="T">type to return</typeparam>
    /// <param name="aValue">value to check</param>
    /// <param name="aDefault">default value to use if aValue is null</param>
    /// <returns>aValue converted to underlying type or aDefault if aValue is <c>null</c></returns>
    public static T SelectValue<T>(T? aValue, T aDefault) where T : struct
    {
      return aValue == null ? aDefault : (T)Convert.ChangeType(aValue, typeof(T));
    }

    /// <summary>
    /// Gets the owning object of a property using a path specification.
    /// </summary>
    /// <param name="aRoot">Root object to start at</param>
    /// <param name="aPath">Property names separated by aSeparator</param>
    /// <param name="aSeparator"> Separator character to use (default is '.')</param>
    /// <returns>owning object or null if none could be found</returns>
    public static object? GetOwner(object aRoot, string aPath, char aSeparator = '.')
    {
      string[] parts = aPath.Split(aSeparator);
      return GetValueWithPath(aRoot, parts, parts.Length - 1);
    }

    /// <summary>
    /// Gets the property value using a path specification.
    /// </summary>
    /// <param name="aRoot">Root object to start at</param>
    /// <param name="aPath">Property names separated by aSeparator</param>
    /// <param name="aSeparator">Separator character to use (default is '.')</param>
    /// <returns>value or null if none could be found</returns>
    public static object? GetValue(object aRoot, string aPath, char aSeparator = '.')
    {
      string[] parts = aPath.Split(aSeparator);
      return GetValueWithPath(aRoot, parts, parts.Length);
    }

    /// <summary>
    /// Gets the property name at the end of a path specification.
    /// </summary>
    /// <param name="aPath">Property names separated by aSeparator</param>
    /// <param name="aSeparator"> Separator character to use (default is '.')</param>
    /// <returns>Value of last part in the path</returns>
    public static string GetPropertyName(string aPath, char aSeparator = '.')
    {
      string[] parts = aPath.Split(aSeparator);
      return parts[parts.Length - 1];
    }

    /// <summary>
    /// Checks if two objects are equal. The method checks for <c>null</c> values and then
    /// uses <see cref="object.Equals(object)"/> to check for equality.
    /// </summary>
    /// <param name="anObject0">First object to check</param>
    /// <param name="anObject1">Second object to check</param>
    /// <returns>
    /// <c>True</c> if both objects are <c>null</c> or <c>Equals</c> returns
    /// <c>true</c>.
    /// </returns>
    public static bool AreEqual(object? anObject0, object? anObject1)
    {
      return (anObject0 == anObject1) || ((anObject0 != null) && (anObject0.Equals(anObject1)));
    }

    /// <summary>
    /// Checks if a type implements an interface.
    /// </summary>
    /// <typeparam name="TType">Type to check</typeparam>
    /// <typeparam name="TInterface">Interface to check</typeparam>
    /// <returns><c>True</c> if TType implements TInterface</returns>
    public static bool Implements<TType, TInterface>()
    {
      TypeInfo typeInfo = typeof(TType).GetTypeInfo();
      TypeInfo interfaceInfo = typeof(TInterface).GetTypeInfo();
      return interfaceInfo.IsAssignableFrom(typeInfo);
    }

    /// <summary>
    /// Swaps two object values.
    /// </summary>
    /// <typeparam name="T">Type of objects</typeparam>
    /// <param name="anObject0">First object to swap</param>
    /// <param name="anObject1">Second object to swap</param>
    public static void Swap<T>(ref T anObject0, ref T anObject1)
    {
      (anObject0, anObject1) = (anObject1, anObject0);
    }

    /// <summary>
    /// Gets the property value of an object.
    /// <para>
    /// The method checks if the object implements <see cref="IUFAccessProperty"/>, if it does the method will use
    /// <see cref="IUFAccessProperty.GetPropertyValue"/> to obtain the value. Else the method uses reflection to
    /// get the value.
    /// </para>
    /// </summary>
    /// <param name="anObject">Object to get the property value from</param>
    /// <param name="aPropertyName">Name of the property</param>
    /// <returns>Value of the property</returns>
    /// <exception cref="Exception">Thrown if no property with the specified name could not be found</exception>
    public static object? GetPropertyValue(object anObject, string aPropertyName)
    {
      if (anObject is IUFAccessProperty accessor)
      {
        return accessor.GetPropertyValue(aPropertyName);
      }
      PropertyInfo? info = anObject.GetType().GetProperty(aPropertyName);
      if (info == null)
      {
        throw new Exception(
          $"{anObject.GetType().Name} does not have a public property {aPropertyName}");
      }
      return info.GetValue(anObject);
    }

    /// <summary>
    /// Sets the property value of an object.
    /// <para>
    /// The method checks if the object implements <see cref="IUFAccessProperty"/>, if it does the method will use
    /// <see cref="IUFAccessProperty.SetPropertyValue"/> to set the value. Else the method uses reflection to
    /// set the value.
    /// </para>
    /// </summary>
    /// <param name="anObject">Object to set the property value at</param>
    /// <param name="aPropertyName">Name of the property</param>
    /// <param name="aValue">Value to set</param>
    /// <exception cref="Exception">Thrown if no property with the specified name could not be found</exception>
    public static void SetPropertyValue(object anObject, string aPropertyName, object? aValue)
    {
      if (anObject is IUFAccessProperty accessor)
      {
        accessor.SetPropertyValue(aPropertyName, aValue);
      }
      PropertyInfo? info = anObject.GetType().GetProperty(aPropertyName);
      if (info == null)
      {
        throw new Exception(
          $"{anObject.GetType().Name} does not have a public property {aPropertyName}");
      }
      info.SetValue(anObject, aValue);
    }

    /// <summary>
    /// Copies all the public properties of a source to a target.
    /// </summary>
    /// <param name="aSource">source to copy from</param>
    /// <param name="aTarget">target to copy to</param>
    /// <typeparam name="T">the type of object</typeparam>
    /// <returns>the value of aTarget</returns>
    public static T CopyProperties<T>(T aSource, T aTarget)
      where T : notnull
    {
      return CopyProperties(aSource, aTarget, info => true);
    }

    /// <summary>
    /// Copies certain public properties of a source to a target.
    /// </summary>
    /// <param name="aSource">Source to copy from</param>
    /// <param name="aTarget">target to copy to</param>
    /// <param name="anIsValid">a function that should return true if the property should be copied</param>
    /// <typeparam name="T">the type of object</typeparam>
    /// <returns>the value of aTarget</returns>
    public static T CopyProperties<T>(T aSource, T aTarget, Func<PropertyInfo, bool> anIsValid)
      where T : notnull
    {
      return (T)CopyProperties(aSource, aTarget, typeof(T), anIsValid);
    }

    /// <summary>
    /// Copies all the public properties of a source to a target.
    /// </summary>
    /// <param name="aSource">source to copy from</param>
    /// <param name="aTarget">target to copy to</param>
    /// <param name="aPropertyNames">Names of properties to copy or skip</param>
    /// <param name="aSkip">When true then skip the properties in aPropertyNames</param>
    /// <typeparam name="T">the type of object</typeparam>
    /// <returns>the value of aTarget</returns>
    public static T CopyProperties<T>(
      T aSource, T aTarget, ICollection<string> aPropertyNames, bool aSkip = false
    )
      where T : notnull
    {
      return (T)CopyProperties(aSource, aTarget, typeof(T), aPropertyNames, aSkip);
    }

    /// <summary>
    /// Copies all the public properties of a certain type of from source to a target.
    /// </summary>
    /// <param name="aSource">source to copy from</param>
    /// <param name="aTarget">target to copy to</param>
    /// <param name="aType">the type to get property info from</param>
    /// <returns>the value of aTarget</returns>
    public static object CopyProperties(object aSource, object aTarget, Type aType)
    {
      return CopyProperties(aSource, aTarget, aType, info => true);
    }

    /// <summary>
    /// Copies all the public properties of a certain type of from source to a target.
    /// </summary>
    /// <param name="aSource">source to copy from</param>
    /// <param name="aTarget">target to copy to</param>
    /// <param name="aType">the type to get property info from</param>
    /// <param name="aPropertyNames">Names of properties to copy or skip</param>
    /// <param name="aSkip">When true then skip the properties in aPropertyNames</param>
    /// <returns>the value of aTarget</returns>
    public static object CopyProperties(
      object aSource, object aTarget, Type aType, ICollection<string> aPropertyNames,
      bool aSkip = false
    )
    {
      return CopyProperties(
        aSource, aTarget, aType, info => aPropertyNames.Contains(info.Name) ^ aSkip
      );
    }

    /// <summary>
    /// Copies certain public properties of a certain type from a source to a target.
    /// <para>
    /// If the type of either aSource or aTarget is not the same as aType, the method will only copy the property
    /// if both aSource and aTarget have a property with the same name. 
    /// </para>
    /// </summary>
    /// <param name="aSource">source to copy from</param>
    /// <param name="aTarget">target to copy to</param>
    /// <param name="aType">the type to get property info from</param>
    /// <param name="anIsValid">a function that should return true if the property should be copied</param>
    /// <returns>the value of aTarget</returns>
    public static object CopyProperties(
      object aSource, object aTarget, Type aType, Func<PropertyInfo, bool> anIsValid
    )
    {
      Type sourceType = aSource.GetType();
      Type targetType = aTarget.GetType();
      bool isSource = sourceType == aType;
      bool isTarget = targetType == aType;
      foreach (PropertyInfo propertyInfo in aType.GetProperties())
      {
        if (!anIsValid(propertyInfo))
        {
          continue;
        }
        PropertyInfo? sourcePropertyInfo =
          isSource ? propertyInfo : sourceType.GetProperty(propertyInfo.Name);
        if (sourcePropertyInfo == null)
        {
          continue;
        }
        PropertyInfo? targetPropertyInfo =
          isTarget ? propertyInfo : targetType.GetProperty(propertyInfo.Name);
        if ((targetPropertyInfo != null) && targetPropertyInfo.CanWrite)
        {
          targetPropertyInfo.SetValue(aTarget, sourcePropertyInfo.GetValue(aSource));
        }
      }
      return aTarget;
    }

    #endregion

    #region private methods

    /// <summary>
    /// Gets a value from object chain using a path specification.
    /// </summary>
    /// <param name="aRoot">Root to start in</param>
    /// <param name="aParts">Property names</param>
    /// <param name="aCount">Number of entries in aParts to process</param>
    /// <returns>object at end of chain or null if none could be found</returns>
    private static object? GetValueWithPath(object aRoot, IEnumerable<string> aParts, int aCount)
    {
      object currentObject = aRoot;
      foreach (string propertyName in aParts)
      {
        if ((currentObject == null) || (propertyName == "") || (aCount <= 0))
        {
          break;
        }
        PropertyInfo? property = currentObject.GetType().GetProperty(propertyName);
        if (property == null)
        {
          return null;
        }
        currentObject = property.GetValue(currentObject);
        aCount--;
      }
      return currentObject;
    }

    #endregion
  }
}