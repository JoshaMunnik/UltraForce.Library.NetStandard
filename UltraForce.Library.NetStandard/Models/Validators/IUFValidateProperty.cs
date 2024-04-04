// <copyright file="IUFValidateProperty.cs" company="Ultra Force Development">
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
  /// A simple interface to validate a value of a property.
  /// </summary>
  /// <remarks>
  /// This interface can be implemented by validators that need access to
  /// data instance containing the property (for example if other property
  /// values need to be tested as well).
  /// </remarks>
  public interface IUFValidateProperty
  {
    /// <summary>
    /// Determines if the value is valid.
    /// </summary>
    /// <param name="aPropertyName">
    /// Name of property to validate
    /// </param>
    /// <param name="aData">
    /// Data where to retrieve the property value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the valid is valid; otherwise, <c>false</c>.
    /// </returns>
    bool IsValid(string aPropertyName, IUFModel aData);
  }
}