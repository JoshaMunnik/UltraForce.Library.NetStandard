// <copyright file="UFReflectionTools.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Support methods for <see cref="Type"/>.
  /// </summary>
  /// <remarks>
  /// Based on code from:
  /// https://stackoverflow.com/a/35370385/968451
  /// <para>
  /// When using this class with Unity 3D, it is faster just to query the
  /// Type with the correct GetXXXX method.
  /// </para>
  /// </remarks>
  public static class UFReflectionTools
  {
    #region private variables

    /// <summary>
    /// Contains all numeric types
    /// </summary>
    private static HashSet<Type>? s_numericTypes;

    #endregion

    #region public methods

    /// <summary>
    /// Gets all constructors for the current type and parent types.
    /// </summary>
    /// <param name="aType">Type to get constructors for</param>
    /// <returns>All constructors</returns>
    public static IEnumerable<ConstructorInfo> GetAllConstructors(Type aType)
    {
      return GetAll(aType, typeInfo => typeInfo.DeclaredConstructors);
    }

    /// <summary>
    /// Gets all events for the current type and parent types.
    /// </summary>
    /// <param name="aType">Type to get events for</param>
    /// <returns>All events</returns>
    public static IEnumerable<EventInfo> GetAllEvents(Type aType)
    {
      return GetAll(aType, typeInfo => typeInfo.DeclaredEvents);
    }

    /// <summary>
    /// Gets all fields for the current type and parent types.
    /// </summary>
    /// <param name="aType">Type to get fields for</param>
    /// <returns>All fields</returns>
    public static IEnumerable<FieldInfo> GetAllFields(Type aType)
    {
      return GetAll(aType, typeInfo => typeInfo.DeclaredFields);
    }

    /// <summary>
    /// Gets all members for the current type and parent types.
    /// </summary>
    /// <param name="aType">Type to get members for</param>
    /// <returns>All members</returns>
    public static IEnumerable<MemberInfo> GetAllMembers(Type aType)
    {
      return GetAll(aType, typeInfo => typeInfo.DeclaredMembers);
    }

    /// <summary>
    /// Gets all methods for the current type and parent types.
    /// </summary>
    /// <param name="aType">Type to get methods for</param>
    /// <returns>All methods</returns>
    public static IEnumerable<MethodInfo> GetAllMethods(Type aType)
    {
      return GetAll(aType, typeInfo => typeInfo.DeclaredMethods);
    }

    /// <summary>
    /// Gets all methods of a certain name for the current type and parent
    /// types. The first method will be from the type, the last method from
    /// the last base class implementing the method itself.
    /// </summary>
    /// <param name="aType">Type to get methods for</param>
    /// <param name="aMethodName">Name of method</param>
    /// <returns>All methods of the specified name</returns>
    public static IEnumerable<MethodInfo> GetAllMethods(
      Type aType,
      string aMethodName
    )
    {
      return GetAll(
        aType,
        typeInfo => typeInfo.GetDeclaredMethods(aMethodName)
      );
    }

    /// <summary>
    /// Gets all nested types for the current type and parent types.
    /// </summary>
    /// <param name="aType">Type to get nested types for</param>
    /// <returns>All nested types</returns>
    public static IEnumerable<TypeInfo> GetAllNestedTypes(Type aType)
    {
      return GetAll(aType, typeInfo => typeInfo.DeclaredNestedTypes);
    }

    /// <summary>
    /// Gets all properties for the current type and parent types.
    /// </summary>
    /// <param name="aType">Type to get properties for</param>
    /// <returns>All properties</returns>
    public static IEnumerable<PropertyInfo> GetAllProperties(Type aType)
    {
      return GetAll(aType, typeInfo => typeInfo.DeclaredProperties);
    }

    /// <summary>
    /// Checks if a type is numeric.
    /// <para>
    /// Code based on: https://stackoverflow.com/a/33776103/968451
    /// </para>
    /// </summary>
    /// <param name="aType">Type to check</param>
    /// <param name="anIncludeNullable">When true also check for nullable versions of the type</param>
    /// <returns>true if type is numeric</returns>
    public static bool IsNumeric(Type aType, bool anIncludeNullable = true)
    {
      // first time create hash set with all numeric types
      s_numericTypes ??= new HashSet<Type>()
      {
        typeof(int), typeof(double), typeof(decimal), typeof(long), typeof(short), typeof(sbyte),
        typeof(byte),
        typeof(ulong), typeof(ushort), typeof(uint), typeof(float)
      };
      return anIncludeNullable
        ? s_numericTypes.Contains(Nullable.GetUnderlyingType(aType) ?? aType)
        : s_numericTypes.Contains(aType);
    }

    /// <summary>
    /// Checks if the type of a value is numeric. This method just calls <see cref="IsNumeric"/> with the type of the
    /// parameter.
    /// </summary>
    /// <param name="aValue">Value to check its type of</param>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <returns>true if aValue is numeric</returns>
    public static bool IsNumericType<T>(T aValue)
    {
      return IsNumeric(typeof(T));
    }

    /// <summary>
    /// Copies a property value. The method handles the following:<br/>
    /// - null values with nullable strings copied to a non-nullable string are replaced by ""<br/>
    /// - enum values are copied to integers and vice versa (including support for nullable fields)
    /// </summary>
    /// <param name="aSourceProperty"></param>
    /// <param name="aTargetProperty"></param>
    /// <param name="aSource"></param>
    /// <param name="aTarget"></param>
    public static void CopyProperty(
      PropertyInfo aSourceProperty, PropertyInfo aTargetProperty, object aSource, object aTarget
    )
    {
      Type? underlyingSourceType = Nullable.GetUnderlyingType(aSourceProperty.PropertyType);
      Type? underlyingTargetType = Nullable.GetUnderlyingType(aTargetProperty.PropertyType);
      if ((underlyingSourceType == typeof(string)) && (underlyingTargetType != typeof(string)))
      {
        CopyNullableStringToString(aSourceProperty, aTargetProperty, aSource, aTarget);
        return;
      }
      Type? enumSourceType = GetEnumType(aSourceProperty);
      Type? enumTargetType = GetEnumType(aTargetProperty);
      if ((enumSourceType != null) && (enumTargetType == null))
      {
        CopyEnumToInt(aSourceProperty, aTargetProperty, aSource, aTarget, enumSourceType);
        return;
      }
      if ((enumSourceType == null) && (enumTargetType != null))
      {
        CopyIntToEnum(aSourceProperty, aTargetProperty, aSource, aTarget, enumTargetType);
        return;
      }
      aTargetProperty.SetValue(aTarget, aSourceProperty.GetValue(aSource));
    }

    /// <summary>
    /// Gets the enum type of a property. When the property is nullable, it returns
    /// the underlying type.
    /// </summary>
    /// <param name="aProperty"></param>
    /// <returns>Enum type or null if property is not a (nullable) enum.</returns>
    public static Type? GetEnumType(PropertyInfo aProperty)
    {
      if (aProperty.PropertyType.IsEnum)
      {
        return aProperty.PropertyType;
      }
      Type? underlyingType = Nullable.GetUnderlyingType(aProperty.PropertyType);
      return underlyingType is { IsEnum: true } ? underlyingType : null;
    }

    #endregion

    #region private methods

    /// <summary>
    /// Copies an integer property to an enum property. The method throws an exception if no
    /// match can be found.
    /// </summary>
    /// <param name="aSourceProperty"></param>
    /// <param name="aTargetProperty"></param>
    /// <param name="aSource"></param>
    /// <param name="aTarget"></param>
    /// <param name="enumTargetType"></param>
    private static void CopyIntToEnum(
      PropertyInfo aSourceProperty, PropertyInfo aTargetProperty,
      object aSource, object aTarget, Type enumTargetType
    )
    {
      if (!aTargetProperty.CanWrite || !aSourceProperty.CanRead)
      {
        return;
      }
      object sourceValue = aSourceProperty.GetValue(aSource);
      if (sourceValue == null)
      {
        aTargetProperty.SetValue(aTarget, null);
        return;
      }
      object enumValue = UFEnumTools.FindValue(enumTargetType, (int)sourceValue);
      aTargetProperty.SetValue(aTarget, enumValue);
    }

    /// <summary>
    /// Copies an enum property to an integer property. 
    /// </summary>
    /// <param name="aSourceProperty"></param>
    /// <param name="aTargetProperty"></param>
    /// <param name="aSource"></param>
    /// <param name="aTarget"></param>
    /// <param name="anEnumSourceType"></param>
    private static void CopyEnumToInt(
      PropertyInfo aSourceProperty,
      PropertyInfo aTargetProperty,
      object aSource,
      object aTarget,
      Type anEnumSourceType
    )
    {
      object? enumValue = aSourceProperty.GetValue(aSource);
      if (enumValue == null)
      {
        aTargetProperty.SetValue(aTarget, null);
        return;
      }
      string? enumValueAsString = enumValue.ToString();
      if (enumValueAsString == null)
      {
        aTargetProperty.SetValue(aTarget, null);
        return;
      }
      object? fieldValue = anEnumSourceType.GetField(enumValueAsString)?.GetValue(enumValue);
      if (fieldValue == null)
      {
        aTargetProperty.SetValue(aTarget, null);
        return;
      }
      aTargetProperty.SetValue(aTarget, (int)fieldValue);
    }

    /// <summary>
    /// Copies a nullable string to a normal string, replacing null with an empty string.
    /// </summary>
    /// <param name="aSourceProperty"></param>
    /// <param name="aTargetProperty"></param>
    /// <param name="aSource"></param>
    /// <param name="aTarget"></param>
    private static void CopyNullableStringToString(
      PropertyInfo aSourceProperty, PropertyInfo aTargetProperty, object aSource, object aTarget
    )
    {
      object? value = aSourceProperty.GetValue(aSource);
      aTargetProperty.SetValue(aTarget, value ?? "");
    }

    /// <summary>
    /// Gets all values of a certain type for the type and all its parent
    /// types.
    /// </summary>
    /// <typeparam name="T">Type of values</typeparam>
    /// <param name="aType">Type to process</param>
    /// <param name="anAccessor">
    /// A function that will return a list of <c>T</c> for a certain type info
    /// </param>
    /// <returns>List of values of type <c>T</c></returns>
    private static IEnumerable<T> GetAll<T>(
      Type aType,
      Func<TypeInfo, IEnumerable<T>> anAccessor
    )
    {
      TypeInfo? typeInfo = aType.GetTypeInfo();
      while (typeInfo != null)
      {
        foreach (T results in anAccessor(typeInfo))
        {
          yield return results;
        }
        typeInfo = typeInfo.BaseType?.GetTypeInfo();
      }
    }

    #endregion
  }
}