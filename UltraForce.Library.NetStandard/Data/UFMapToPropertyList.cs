// <copyright file="UFMapToPropertyList.cs" company="Ultra Force Development">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UltraForce.Library.NetStandard.Interfaces;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Data
{
  /// <summary>
  /// This class can be used to create a list that maps to a certain property in a list of objects.
  /// <para>
  /// If the object implements <see cref="IUFAccessProperty" />, the class will use its methods to
  /// get and optionally set the property value. Else the class will use reflection to get and set
  /// the property value.
  /// </para>
  /// <para>
  /// It is not possible to delete, clear, add or insert items to the list. Trying to do so will
  /// throw an exception.
  /// </para>
  /// </summary>
  /// <typeparam name="TValue">The type of the property value</typeparam>
  public class UFMapToPropertyList<TValue> : IList<TValue> where TValue : notnull
  {
    #region private variables

    /// <summary>
    /// List of instances as list
    /// </summary>
    private readonly IList<object>? m_list;

    /// <summary>
    /// Name of property
    /// </summary>
    private readonly string m_propertyName;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFMapToPropertyList{T}" />
    /// </summary>
    /// <param name="anItems">
    /// Array of objects, each containing the property with specified name.
    /// </param>
    /// <param name="aPropertyName">
    /// Name of property
    /// </param>
    /// <param name="aReadOnly">
    /// When <c>true</c> trying to set a value in the list will throw an exception.
    /// </param>
    public UFMapToPropertyList(IEnumerable<object> anItems, string aPropertyName, bool aReadOnly = true)
      : this(anItems.ToList(), aPropertyName, aReadOnly)
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFMapToPropertyList{T}" />
    /// </summary>
    /// <param name="anItems">
    /// List of objects that contain the property with specified name.
    /// </param>
    /// <param name="aPropertyName">
    /// Name of property
    /// </param>
    /// <param name="aReadOnly">
    /// When <c>true</c> trying to set a value in the list will throw an exception.
    /// </param>
    public UFMapToPropertyList(IList<object> anItems, string aPropertyName, bool aReadOnly = true)
    {
      this.m_list = anItems;
      this.m_propertyName = aPropertyName;
      this.IsReadOnly = aReadOnly;
    }

    #endregion

    #region ilist

    /// <inheritdoc />
    public IEnumerator<TValue> GetEnumerator()
    {
      using IEnumerator<object> enumerator = this.m_list!.GetEnumerator();
      return new MapEnumerator(this.m_propertyName, enumerator);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    /// <summary>
    /// Calling this method will throw an exception, since the list is readonly.
    /// </summary>
    /// <param name="item">Item to add</param>
    public void Add(TValue item)
    {
      throw new Exception("Can not add item.");
    }

    /// <summary>
    /// Calling this method will throw an exception, since the list is readonly.
    /// </summary>
    public void Clear()
    {
      throw new Exception("Can not clear items.");
    }

    /// <inheritdoc />
    public bool Contains(TValue item)
    {
      return this.IndexOf(item) >= 0;
    }

    /// <inheritdoc />
    public void CopyTo(TValue[] array, int arrayIndex)
    {
      if (array == null)
      {
        throw new ArgumentException();
      }
      if ((arrayIndex < 0) || (arrayIndex >= array.Length))
      {
        throw new ArgumentOutOfRangeException();
      }
      if (array.Length - arrayIndex < this.Count)
      {
        throw new ArgumentException(
          $"Array starting at {arrayIndex} is too small ({array.Length - arrayIndex}) it requires at least {this.Count}"
        );
      }
      for (int index = 0; index < this.Count; index++)
      {
        array[index + arrayIndex] = this[index];
      }
    }

    /// <summary>
    /// Calling this method will throw an exception, since the list is
    /// readonly.
    /// </summary>
    /// <param name="item">Item to remove</param>
    public bool Remove(TValue item)
    {
      throw new Exception("Can not remove item.");
    }

    /// <inheritdoc />
    public int Count => this.m_list!.Count;

    /// <inheritdoc />
    public bool IsReadOnly { get; }

    /// <inheritdoc />
    public int IndexOf(TValue item)
    {
      int count = this.Count;
      for (int index = 0; index < count; index++)
      {
        if (this.GetPropertyValue(index)!.Equals(item))
        {
          return index;
        }
      }
      return -1;
    }

    /// <inheritdoc />
    public void Insert(int index, TValue item)
    {
      throw new Exception("Can not insert item.");
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
      throw new Exception("Can not remove item.");
    }

    /// <inheritdoc />
    public TValue this[int index] {
      get => this.GetPropertyValue(index);
      set {
        if (this.IsReadOnly)
        {
          throw new Exception("Trying to assign value to readonly list");
        }
        this.SetPropertyValue(index, value);
      }
    }

    #endregion

    #region private methods

    /// <summary>
    /// Gets the property value of an object at a certain index.
    /// </summary>
    /// <param name="anIndex">Index to get property from</param>
    /// <returns>Value of the property</returns>
    private TValue GetPropertyValue(int anIndex)
    {
      object? value = UFObjectTools.GetPropertyValue(this.m_list![anIndex], this.m_propertyName);
      if (value == null)
      {
        throw new InvalidCastException($"Can not cast null value to {typeof(TValue).Name}");
      }
      return (TValue) value;
    }

    /// <summary>
    /// Sets the property value of an object at a certain index.
    /// </summary>
    /// <param name="anIndex">Index to set property at</param>
    /// <param name="aValue">Value to assign</param>
    private void SetPropertyValue(int anIndex, TValue aValue)
    {
      UFObjectTools.SetPropertyValue(this.m_list![anIndex], this.m_propertyName, aValue);
    }

    #endregion

    #region private class: mapenumerator

    /// <summary>
    /// Enumerator that encapsulates another enumerator and returns a certain property value.
    /// </summary>
    private class MapEnumerator : IEnumerator<TValue> 
    {
      #region private variables

      /// <summary>
      /// Name of property
      /// </summary>
      private readonly string m_propertyName;

      /// <summary>
      /// Encapsulated enumerator
      /// </summary>
      private readonly IEnumerator m_enumerator;

      #endregion

      #region constructors

      /// <summary>
      /// Constructs an instance of <see cref="MapEnumerator"/>
      /// </summary>
      /// <param name="aPropertyName"></param>
      /// <param name="anEnumerator"></param>
      public MapEnumerator(string aPropertyName, IEnumerator anEnumerator)
      {
        this.m_propertyName = aPropertyName;
        this.m_enumerator = anEnumerator;
      }

      #endregion

      #region ienumerator

      /// <inheritdoc />
      public bool MoveNext()
      {
        return this.m_enumerator.MoveNext();
      }

      /// <inheritdoc />
      public void Reset()
      {
        this.m_enumerator.Reset();
      }

      /// <inheritdoc />
      public TValue Current
      {
        get
        {
          if (this.m_enumerator.Current == null)
          {
            throw new InvalidCastException("Property is null"); 
          }
          object? value = UFObjectTools.GetPropertyValue(
            this.m_enumerator.Current, this.m_propertyName
            );
          if (value == null)
          {
            throw new InvalidCastException("Can not cast null value to " + typeof(TValue).Name); 
          }
          return (TValue)value;
        }
      } 

      /// <inheritdoc />
      object IEnumerator.Current => this.Current;

      /// <inheritdoc />
      public void Dispose()
      {
      }

      #endregion
    }

    #endregion
  }
}