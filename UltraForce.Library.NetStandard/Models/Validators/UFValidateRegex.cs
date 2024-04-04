// <copyright file="UFValidateRegex.cs" company="Ultra Force Development">
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

using System.Text.RegularExpressions;

namespace UltraForce.Library.NetStandard.Models.Validators
{
  /// <summary>
  /// Validator using regular expression to validate.
  /// </summary>
  public class UFValidateRegex : IUFValidateValue
  {
    #region private vars

    /// <summary>
    /// Regular expression to match.
    /// </summary>
    private readonly Regex m_regExp;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UFValidateRegex"/> class.
    /// </summary>
    /// <param name='aRegExp'>
    /// Regular expression to match.
    /// </param>
    public UFValidateRegex(Regex aRegExp)
    {
      this.m_regExp = aRegExp;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UFValidateRegex"/> class.
    /// </summary>
    /// <param name='aPattern'>
    /// Pattern to build reg expression from.
    /// </param>
    public UFValidateRegex(string aPattern) : this(new Regex(aPattern))
    {
    }

    #endregion

    #region iufvalidatevalue

    /// <summery>
    /// Checks if value matches regular expression.
    /// </summery>
    /// <param name="aValue">value to check</param>
    /// <returns><c>True</c> if aValue matches the expression</returns>
    public virtual bool IsValid(object? aValue)
    {
      return (aValue != null) && this.m_regExp.IsMatch(aValue.ToString());
    }

    #endregion
  }
}