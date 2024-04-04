// <copyright file="UFValidateConditional.cs" company="Ultra Force Development">
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
  /// <see cref="UFValidateConditional"/> can be used to validate a value
  /// only if another property is valid as well.
  /// </summary>
  public class UFValidateConditional : IUFValidateProperty
  {
    #region private variables

    /// <summary>
    /// Name of the other property to validate
    /// </summary>
    private readonly string m_otherPropertyName;

    /// <summary>
    /// Validator to validate the other value with
    /// </summary>
    private readonly IUFValidateValue m_otherValidator;

    /// <summary>
    /// Validator to validate property value with
    /// </summary>
    private readonly IUFValidateValue m_validator;

    /// <summary>
    /// Value to return if other property is not valid.
    /// </summary>
    private readonly bool m_default;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFValidateConditional"/>.
    /// </summary>
    /// <param name="anOtherPropertyName">
    /// Name of other property to validate
    /// </param>
    /// <param name="anOtherValidator">
    /// Validator to use for other property
    /// </param>
    /// <param name="aValidator">
    /// Validator to use when other property is valid
    /// </param>
    /// <param name="aDefault">
    /// Default validation value to return if the other property is not valid
    /// </param>
    public UFValidateConditional(
      string anOtherPropertyName,
      IUFValidateValue anOtherValidator,
      IUFValidateValue aValidator,
      bool aDefault = true
    )
    {
      this.m_otherPropertyName = anOtherPropertyName;
      this.m_otherValidator = anOtherValidator;
      this.m_validator = aValidator;
      this.m_default = aDefault;
    }

    #endregion

    #region iufvalidateproperty

    /// <inheritdoc />
    public bool IsValid(string aPropertyName, IUFModel aData)
    {
      return this.m_otherValidator.IsValid(
        aData.GetPropertyValue(this.m_otherPropertyName)
      )
        ? this.m_validator.IsValid(aData.GetPropertyValue(aPropertyName))
        : this.m_default;
    }

    #endregion
  }
}