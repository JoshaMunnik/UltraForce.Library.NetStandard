// <copyright file="UFValidateTextLength.cs" company="Ultra Force Development">
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
  /// Check if a text has certain number of characters. For example to 
  /// validate a minimum length on passwords.
  /// </summary>
  public class UFValidateTextLength : IUFValidateValue
  {
    #region private vars

    /// <summary>
    /// Minimum required size. 
    /// </summary>
    private readonly int m_min;

    /// <summary>
    /// Maximum required size
    /// </summary>
    private readonly int m_max;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFValidateTextLength"/> class.
    /// </summary>
    /// <param name='aMin'>
    /// Minimal number of characters allowed.
    /// </param>
    /// <param name='aMax'>
    /// Maximum number of characters allowed.
    /// </param>
    public UFValidateTextLength(int aMin, int aMax = int.MaxValue)
    {
      this.m_min = aMin;
      this.m_max = aMax;
    }

    #endregion

    #region iufvalidatevalue

    /// <summary>
    /// Verify if a text value length falls inside the set range.          
    /// </summary>
    /// <param name="aValue">Value to check</param>
    /// <returns>True if aValue is valid</returns>
    public bool IsValid(object? aValue)
    {
      if (aValue == null)
      {
        return false;
      }
      int charCount = aValue.ToString().Length;
      return (charCount >= this.m_min) && (charCount <= this.m_max);
    }

    #endregion
  }
}