// <copyright file="UFValidateArray.cs" company="Ultra Force Development">
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

using System.Reflection;

namespace UltraForce.Library.NetStandard.Models.Validators
{
  /// <summary>
  /// Validate an array on the correct number of elements.
  /// </summary>
  public class UFValidateArray : IUFValidateValue
  {
    #region private vars

    /// <summary>
    /// Minimum number of elements
    /// </summary>
    private readonly int m_min;

    /// <summary>
    /// Maximum number of elements
    /// </summary>
    private readonly int m_max;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UFValidateArray"/> class.
    /// </summary>
    /// <param name='aMinLength'>
    /// A minimum length (inclusive).
    /// </param>
    /// <param name='aMaxLength'>
    /// A maximum length (inclusive).
    /// </param>
    public UFValidateArray(int aMinLength, int aMaxLength)
    {
      this.m_max = aMaxLength;
      this.m_min = aMinLength;
    }

    #endregion

    #region iufvalidatevalue

    /// <summary>
    /// See if aValue supports a Length property or field. If it does check 
    /// if the value falls inside the set range.
    /// In all other cases the method returns <c>false</c>.
    /// <para>
    /// The range includes the specified minimum and maximum value.
    /// </para>
    /// </summary>
    /// <param name='aValue'>
    /// Value to validate.
    /// </param>
    /// <returns>
    /// <c>true</c> if aValue has a Length property/field and its value falls 
    /// inside the range; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValid(object? aValue)
    {
      if (aValue == null)
      {
        return false;
      }
      int length;
      FieldInfo field = aValue.GetType().GetRuntimeField("Length");
      if (field != null)
      {
        length = (int)field.GetValue(aValue);
      }
      else
      {
        PropertyInfo property = aValue.GetType().GetRuntimeProperty("Length");
        if (property != null)
        {
          length = (int)property.GetValue(aValue, null);
        }
        else
        {
          return false;
        }
      }
      return (length >= this.m_min) && (length <= this.m_max);
    }

    #endregion
  }
}