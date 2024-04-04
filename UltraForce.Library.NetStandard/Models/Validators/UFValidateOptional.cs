// <copyright file="UFValidateOptional.cs" company="Ultra Force Development">
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
  /// This validator can be used to validate optional values. If a value 
  /// is empty or '' then this class will return a default value else it
  /// will pass the value to another validator.
  /// </summary>
  public class UFValidateOptional : IUFValidateValue
  {
    #region private vars

    /// <summary>
    /// Use this object to validate if value is non empty.
    /// </summary>
    private readonly IUFValidateValue m_validate;

    /// <summary>
    /// Result to return if value is empty
    /// </summary>
    private readonly bool m_emptyResult;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UFValidateOptional"/> 
    /// class.
    /// </summary>
    /// <param name='aValidate'>
    /// Validator to apply on non-null/non-empty values.
    /// </param>
    /// <param name="anEmptyResult">
    /// Result value to return for null/empty values.
    /// </param>
    public UFValidateOptional(IUFValidateValue aValidate, bool anEmptyResult = true)
    {
      this.m_validate = aValidate;
      this.m_emptyResult = anEmptyResult;
    }

    #endregion

    #region iufvalidatevalue

    /// <summary>
    /// Validate aValue; return the stored empty result value if aValue 
    /// is null or empty; else use the stored validator.
    /// </summary>
    /// <param name='aValue'>
    /// value to validate
    /// </param>
    /// <returns>
    /// Stored empty result if aValue is empty or valid, <c>false</c> if value 
    /// is not empty and not valid.
    /// </returns>
    public bool IsValid(object? aValue)
    {
      if (aValue == null)
      {
        return this.m_emptyResult;
      }
      return aValue.ToString().Length == 0 ? this.m_emptyResult : this.m_validate.IsValid(aValue);
    }

    #endregion
  }
}