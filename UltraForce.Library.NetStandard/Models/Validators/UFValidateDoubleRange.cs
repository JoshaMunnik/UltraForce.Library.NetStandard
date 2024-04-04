// <copyright file="UFValidateDoubleRange.cs" company="Ultra Force Development">
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
  /// Check if a value is an integer within a certain range.
  /// </summary>
  public class UFValidateDoubleRange : UFValidateDouble
  {
    #region private vars

    /// <summary>
    /// Minimal value allowed.
    /// </summary>
    private readonly double m_min;

    /// <summary>
    /// Maximum value allowed.
    /// </summary>
    private readonly double m_max;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFValidateDoubleRange"/> class.
    /// </summary>
    /// <param name='aMin'>
    /// Minimal value allowed (including this value)
    /// </param>
    /// <param name='aMax'>
    /// Maximal value allowed (including this value)
    /// </param>
    public UFValidateDoubleRange(double aMin, double aMax)
    {
      this.m_max = aMax;
      this.m_min = aMin;
    }

    #endregion

    #region iufvalidatevalue

    /// <summery>
    /// Try to parse aValue as double and check if value is within the set
    /// range.
    /// </summery>
    /// <param name="aValue">value to check</param>
    /// <returns><c>True</c> if aValue is parsed correctly</returns>
    public override bool IsValid(object? aValue)
    {
      if (!base.IsValid(aValue) || (aValue == null))
      {
        return false;
      }
      double doubleValue = (double)aValue;
      return (doubleValue >= this.m_min) && (doubleValue <= this.m_max);
    }

    #endregion
  }
}