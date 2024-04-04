// <copyright file="UFDescriptionAttribute.cs" company="Ultra Force Development">
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

using System;
using System.Diagnostics.CodeAnalysis;
using UltraForce.Library.NetStandard.Extensions;

namespace UltraForce.Library.NetStandard.Annotations
{
  /// <summary>
  /// Implements a simple description attribute which can be used to add a 
  /// description to enum fields.
  /// <para>
  /// Include the <see cref="UFEnumExtensions" /> to install an extension 
  /// to access the description.
  /// </para>
  /// <para>
  /// This class has been defined because the 
  /// <c>System.ComponentModel.DescriptionAttribute</c> is not available to the
  /// <c>.NetStandard1</c> class libraries.
  /// </para>
  /// </summary>
  /// <example>
  /// <code>
  /// public enum State {
  /// 
  ///   [UFDescription("Idle state")]
  ///   Idle,
  /// 
  ///   [UFDescription("Busy state")]
  ///   Busy
  /// }
  /// </code>
  /// </example>
  [AttributeUsage(AttributeTargets.Field)]
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class UFDescriptionAttribute : Attribute
  {
    #region properties

    /// <summary>
    /// Description value as set by the attribute definition
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Short description value as set by the attribute definition
    /// </summary>
    public string ShortDescription { get; set; } = "";

    /// <summary>
    /// Name value as set by the attribute definition
    /// </summary>
    public string Name { get; set; } = "";

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFDescriptionAttribute"/> and set <see cref="Description"/>.
    /// </summary>
    /// <param name="aDescription">Description to use</param>
    public UFDescriptionAttribute(string aDescription)
    {
      this.Description = aDescription;
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFDescriptionAttribute"/>
    /// </summary>
    public UFDescriptionAttribute()
    {
    }

    #endregion
  }
}