// <copyright file="IUFServiceProvider.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (c) 2018 Ultra Force Development
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

namespace UltraForce.Library.NetStandard.Services
{
  /// <summary>
  /// This interface can be implemented by classes that can create instances of an object providing
  /// a certain service.
  /// <para>
  /// It is similar to <c>IServiceProvider</c> which is available from NetStandard 2.0+
  /// </para>
  /// </summary>
  /// <remarks>
  /// This interface is provided for backwards compatibility, the rest of the
  /// Ultra Force NetStandard Library uses <see cref="IServiceProvider"/>. 
  /// </remarks>
  [Obsolete("Use IServiceProvider instead of IUFServiceProvider")]
  public interface IUFServiceProvider : IServiceProvider
  {
  }
}