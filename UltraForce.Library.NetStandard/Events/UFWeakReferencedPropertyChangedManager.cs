﻿// <copyright file="UFWeakReferencedPropertyChangedManager.cs" company="Ultra Force Development">
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

using System.ComponentModel;
using UltraForce.Library.NetStandard.Delegates;

namespace UltraForce.Library.NetStandard.Events
{
  /// <summary>
  /// <see cref="UFWeakReferencedPropertyChangedManager"/> stores event
  /// handlers as methods and target. It uses a weak reference for
  /// the target and only invokes the handler only if the target is still
  /// available.
  /// </summary>
  public class UFWeakReferencedPropertyChangedManager
    : UFWeakReferencedDelegateManagerBase
  {
    #region public methods

    /// <summary>
    /// Adds a handler to the managed list. If there is already a handler
    /// with the same method and target stored, nothing happens.
    /// </summary>
    /// <param name="aHandler">Handler to add</param>
    public void Add(PropertyChangedEventHandler aHandler)
    {
      this.Add(new UFWeakReferencedEventHandler(aHandler));
    }

    /// <summary>
    /// Removes a handler from the managed list. 
    /// </summary>
    /// <param name="aHandler">Handler to remove</param>
    public void Remove(PropertyChangedEventHandler aHandler)
    {
      this.Remove(new UFWeakReferencedEventHandler(aHandler));
    }

    /// <summary>
    /// Invokes the handlers for the targets that are still available.
    /// </summary>
    /// <param name="aSender">Sender to use</param>
    /// <param name="anArguments">Arguments to use</param>
    public void Invoke(object aSender, PropertyChangedEventArgs anArguments)
    {
      base.Invoke(aSender, anArguments);
    }

    #endregion
  }
}