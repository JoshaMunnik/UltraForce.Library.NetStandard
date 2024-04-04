// <copyright file="UFDataChangedEventArgs.cs" company="Ultra Force Development">
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
using UltraForce.Library.NetStandard.Models;

namespace UltraForce.Library.NetStandard.Events
{
  /// <inheritdoc />
  /// <summary>
  /// Data changed event object. It is created by <see cref="UFModel" /> when 
  /// it fires change events.
  /// </summary>
  public class UFDataChangedEventArgs : EventArgs
  {
    #region private vars

    /// <summary>
    /// Contains information about every property changed.
    /// </summary>
    private readonly Dictionary<string, PropertyInfo> m_properties;

    #endregion

    #region public methods

    /// <inheritdoc />
    /// <summary>
    /// Create new event object. 
    /// </summary>
    /// <param name="aDataChangedToken">
    /// Value to use for data changed token.
    /// </param>
    /// <param name="aPropertyName">
    /// Optional Property name that changed
    /// </param>
    /// <param name="anOldValue">
    /// The old value of the property.
    /// </param>
    /// <param name="aNewValue">
    /// A new of the property.
    /// </param>
    public UFDataChangedEventArgs(
      int aDataChangedToken,
      string? aPropertyName = null,
      object? anOldValue = null,
      object? aNewValue = null
    )
    {
      this.m_properties = new Dictionary<string, PropertyInfo>();
      this.DataChangedToken = aDataChangedToken;
      if (aPropertyName != null)
      {
        this.m_properties[aPropertyName] =
          new PropertyInfo(anOldValue, aNewValue);
      }
    }

    /// <summary>
    /// This method can be used to add properties that have changed.
    /// </summary>
    /// <param name='aPropertyName'>
    /// Name of the property.
    /// </param>
    /// <param name='anOldValue'>
    /// Old value of the property.
    /// </param>
    /// <param name='aNewValue'>
    /// New value of the property
    /// </param>
    public void AddChanged(
      string aPropertyName,
      object? anOldValue = null,
      object? aNewValue = null
    )
    {
      this.m_properties[aPropertyName] =
        new PropertyInfo(anOldValue, aNewValue);
    }

    /// <summary>
    /// Check if a certain property has changed.
    /// </summary>
    /// <param name='aPropertyNames'>
    /// One or more property names to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if at least one of the property has changed; otherwise, <c>false</c>.
    /// </returns>
    public bool HasChanged(params string[] aPropertyNames)
    {
      return aPropertyNames.Any(
        propertyName => this.m_properties.ContainsKey(propertyName)
      );
    }

    /// <summary>
    /// Get the old value of a property.
    /// </summary>
    /// <param name='aPropertyName'>
    /// Property to get old value for.
    /// </param>
    /// <returns>
    /// The old value.
    /// </returns>
    public object? OldValue(string aPropertyName)
    {
      return this.m_properties[aPropertyName].OldValue;
    }

    /// <summary>
    /// Get the new value of a property.
    /// </summary>
    /// <returns>
    /// The new value.
    /// </returns>
    /// <param name='aPropertyName'>
    /// Property to get new value for.
    /// </param>
    public object? NewValue(string aPropertyName)
    {
      return this.m_properties[aPropertyName].NewValue;
    }

    /// <summary>
    /// Get all the properties that have changed.
    /// </summary>
    /// <returns>
    /// A list of property names.
    /// </returns>
    public string[] GetPropertyNames()
    {
      string[] result = new string[this.m_properties.Count];
      this.m_properties.Keys.CopyTo(result, 0);
      return result;
    }

    #endregion

    #region public properties

    /// <summary>
    /// This value contains a copy of the DataChangedToken value before it was 
    /// adjusted when firing this event.
    /// </summary>
    public int DataChangedToken { get; }

    #endregion

    #region private classes

    /// <summary>
    /// Structure to store property information
    /// </summary>
    private struct PropertyInfo
    {
      public object? OldValue { get; }
      public object? NewValue { get; }

      public PropertyInfo(object? anOldValue = null, object? aNewValue = null) : this()
      {
        this.OldValue = anOldValue;
        this.NewValue = aNewValue;
      }
    }

    #endregion
  }
}