// <copyright file="UFValidateBoolean.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Models.Validators
{
  /// <summary>
  /// Validate that a value is either <c>true</c> or <c>false</c>.
  /// </summary>
  public class UFValidateBoolean : IUFValidateValue
  {
    #region private vars

    /// <summary>
    /// Values must equal this value to be valid.
    /// </summary>
    private readonly bool m_value;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UFValidateBoolean"/> 
    /// class.
    /// </summary>
    /// <param name='aValue'>
    /// Value that needs be matched.
    /// </param>
    public UFValidateBoolean(bool aValue)
    {
      this.m_value = aValue;
    }

    #endregion

    #region iufvalidatevalue

    /// <summary>
    /// A value is valid if it matches the stored boolean value.
    /// </summary>
    /// <param name='aValue'>
    /// Value to check.
    /// </param>
    /// <returns>
    /// <c>true</c> the value is equal to the stored boolean value; 
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool IsValid(object? aValue)
    {
      return (aValue != null) && ((bool) aValue == this.m_value);
    }

    #endregion
  }
}