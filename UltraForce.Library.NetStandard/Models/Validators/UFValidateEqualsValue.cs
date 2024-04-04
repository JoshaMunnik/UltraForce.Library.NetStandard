// <copyright file="UFValidateEqualsValue.cs" company="Ultra Force Development">
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

using System.Linq;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Models.Validators
{
  /// <summary>
  /// This validator uses <see cref="object.Equals(object)"/>. A value is valid
  /// if it matches one of the values passed in the constructor.
  /// </summary>
  public class UFValidateEqualsValue : IUFValidateValue
  {
    #region private variables

    /// <summary>
    /// The values to test
    /// </summary>
    private readonly object[] m_values;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFValidateEqualsValue"/>.
    /// </summary>
    /// <param name="aValues">Values to test with</param>
    public UFValidateEqualsValue(params object[] aValues)
    {
      this.m_values = aValues;
    }

    #endregion

    #region iufvalidatevalue

    /// <inheritdoc />
    public bool IsValid(object? aValue)
    {
      // use UFObjectTools.AreEqual so null value is also correct if the
      // passed values contained a null entry.
      return this.m_values.Any(value => UFObjectTools.AreEqual(value, aValue));
    }

    #endregion
  }
}