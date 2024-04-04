// <copyright file="UFValidateAnotherField.cs" company="Ultra Force Development">
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
  /// Validate a value by validating another field in a data structure. If the 
  /// other field validates, this validator will return true as well.
  /// </summary>
  public class UFValidateAnotherField : IUFValidateProperty
  {
    #region private variables

    /// <summary>
    /// Name of other property to validate
    /// </summary>
    private readonly string m_propertyName;

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFValidateAnotherField"/> class.
    /// </summary>
    /// <param name='aProperty'>
    /// Name of field inside data.
    /// </param>
    public UFValidateAnotherField(string aProperty)
    {
      this.m_propertyName = aProperty;
    }

    #endregion

    #region iufvalidateproperty

    /// <inheritdoc />
    public bool IsValid(string aPropertyName, IUFModel aData)
    {
      return aData.IsValidPropertyValue(
        this.m_propertyName,
        aData.GetPropertyValue(this.m_propertyName)
      );
    }

    #endregion
  }
}