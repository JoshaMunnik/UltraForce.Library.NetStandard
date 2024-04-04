// <copyright file="UFValidateEqualToField.cs" company="Ultra Force Development">
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

using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Models.Validators
{
  /// <summary>
  /// This validator checks if the value is equal to the value of property in a 
  /// data structure. It can be used for example with password properties.
  /// </summary>
  public class UFValidateEqualToProperty : IUFValidateProperty
  {
    #region private variables

    /// <summary>
    /// Name of property to compare the property to
    /// </summary>
    private readonly string m_propertyName;

    #endregion

    #region public methods

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="UFValidateEqualToProperty"/> class.
    /// </summary>
    /// <param name='aProperty'>
    /// Name of property inside data.
    /// </param>
    public UFValidateEqualToProperty(string aProperty)
    {
      this.m_propertyName = aProperty;
    }

    #endregion

    #region iufvalidateproperty

    /// <inheritdoc />
    public bool IsValid(string aPropertyName, IUFModel aData)
    {
      object? otherValue = aData.GetPropertyValue(aPropertyName);
      object? value = aData.GetPropertyValue(this.m_propertyName);
      return UFObjectTools.AreEqual(otherValue, value);
    }

    #endregion
  }
}