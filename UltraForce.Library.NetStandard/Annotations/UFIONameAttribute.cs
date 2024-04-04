// <copyright file="UFIONameAttribute.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Annotations
{
  /// <summary>
  /// This attribute can be used to specify a custom name to be used for 
  /// certain IO operations.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class UFIONameAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets the name to use with JSON operations.
    /// </summary>
    public string JsonName { get; set; } = "";

    /// <summary>
    /// Gets or sets the name to use with IO operations.
    /// </summary>
    public string IOName { get; set; } = "";

    /// <summary>
    /// Gets or sets the name to use with xml operations.
    /// </summary>
    public string XmlName { get; set; } = "";

    /// <summary>
    /// Initializes a new instance of 
    /// the <see cref="UFIONameAttribute"/> class.
    /// </summary>
    public UFIONameAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of 
    /// the <see cref="UFIONameAttribute"/> class.
    /// 
    /// Assign aName to all name properties
    /// </summary>
    /// <param name="aName">A name.</param>
    public UFIONameAttribute(string aName)
    {
      this.JsonName = aName;
      this.IOName = aName;
      this.XmlName = aName;
    }

    /// <summary>
    /// Get the name for IO operations.
    /// </summary>
    /// <returns>The IO name or aDefault if IOName is empty or null.</returns>
    /// <param name="aDefault">A default value to use.</param>
    public string GetIOName(string aDefault)
    {
      return string.IsNullOrEmpty(this.IOName) ? aDefault : this.IOName;
    }

    /// <summary>
    /// Get the name for JSON operations.
    /// </summary>
    /// <returns>
    /// The JSON name or aDefault if JSONName is empty or null.
    /// </returns>
    /// <param name="aDefault">A default value to use.</param>
    public string GetJSONName(string aDefault)
    {
      return string.IsNullOrEmpty(this.JsonName) ? aDefault : this.JsonName;
    }

    /// <summary>
    /// Get the name for xml operations.
    /// </summary>
    /// <returns>
    /// The xml name or aDefault if XmlName is empty or null.
    /// </returns>
    /// <param name="aDefault">A default value to use.</param>
    public string GetXmlName(string aDefault)
    {
      return string.IsNullOrEmpty(this.XmlName) ? aDefault : this.XmlName;
    }
  }
}