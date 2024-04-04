// <copyright file="UFStringCache.cs" company="Ultra Force Development">
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
  /// <see cref="UFStringCache{TKey}" /> is a subclass of <see cref="UFCache{K,V}"/> for use with
  /// <see cref="string"/> values. It uses a GUID as the unknown value.
  /// </summary>
  /// <typeparam name="TKey">Type for the key</typeparam>
  public class UFStringCache<TKey> : UFCache<TKey, string>
  {
    /// <inheritdoc />
    public UFStringCache(long aCapacity) : base(aCapacity, Guid.NewGuid().ToString())
    {
    }

    /// <inheritdoc />
    protected override long GetSize(string aValue)
    {
      return aValue.Length;
    }
  }
}